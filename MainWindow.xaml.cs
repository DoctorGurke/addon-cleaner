using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AddonCleaner.Type;
using System.IO;
using System.IO.Compression;

namespace AddonCleaner {

	public partial class MainWindow : Window {
		private static RichTextBox Output;
		private static StackPanel MainPanel;
		private static DirectoryNode activeRootNode;

		public MainWindow() {
            InitializeComponent();
			Output = (RichTextBox)this.FindName("debugoutput");
			MainPanel = (StackPanel)this.FindName("SelectionItemPanel");

			PrintToConsole("DEBUG OUTPUT:");
		}

		private static void InitSelectionTree(DirectoryNode node) {
			if(node.parentNode == null) // root node
				activeRootNode = node;
			var dirNode = new SelectionNode(node.self);
			dirNode.Margin = new Thickness(node.indent * 35, 0, 0, 3);
			MainPanel.Children.Add(dirNode);
			foreach(var dir in node.directories) {
				InitSelectionTree(dir);
			}
			foreach(var file in node.files) {
				var fileNode = new SelectionNode(file);
				fileNode.Margin = new Thickness((node.indent + 1) * 35, 0, 0, 3);
				
				MainPanel.Children.Add(fileNode);
			}
		}

		public static void PrintToConsole(string text = null) {
			if(text == null) return;
			Application.Current.Dispatcher.Invoke(() => {
				Output.Document.Blocks.Add(new Paragraph(new Run(text)));
				Output.ScrollToEnd();
			});
		}

		private void ValidateInputBox(string inputName, out string text) {
			var inputBox = (System.Windows.Controls.RichTextBox)this.FindName(inputName);
			string inputText = new TextRange(inputBox.Document.ContentStart, inputBox.Document.ContentEnd).Text.Replace("\r\n", "");
			text = inputText;
		}

		private void PackAddon(object sender, RoutedEventArgs e) {

			string addonName;
			ValidateInputBox("AddonNameInput", out addonName);
			if(addonName == null) { PrintToConsole("Invalid addon name"); return; }

			// technically not used since the activeRootNode gets set as soon as you select a directory
			string inputPath;
			ValidateInputBox("InputLocation", out inputPath);
			if(addonName == null) { PrintToConsole("Invalid input directory"); return; }

			string outputPath;
			ValidateInputBox("OutputLocation", out outputPath);
			if(addonName == null) { PrintToConsole("Invalid output directory"); return; }

			var rootParentDir = activeRootNode.self.info.Parent.FullName;

			// create temp dir
			Directory.CreateDirectory($"{outputPath}\\addon-release-temp");

			//copy enabled files/dirs recursively
			CopyEnabledFiles(activeRootNode, rootParentDir, $"{outputPath}\\addon-release-temp");

			try {
				// zip up the addon with the addon name
				ZipFile.CreateFromDirectory($"{outputPath}\\addon-release-temp\\{activeRootNode.self.info.Name}", $"{outputPath}\\{addonName}.zip");
				PrintToConsole("Pack complete!");
			} catch {
				PrintToConsole("Packing addon failed! A zip with that name possibly already exists!");
			}

			// delete temp dir
			Directory.Delete($"{outputPath}\\addon-release-temp", true);
		}

		private void CopyEnabledFiles(DirectoryNode node, string rootParentDir, string outputLocation) {
			var dirPath = node.self.info.FullName.Replace(rootParentDir, outputLocation);
			if(node.self.enabled && !Directory.Exists(dirPath)) {
				Directory.CreateDirectory(dirPath);
			}
			foreach(var file in node.files) {
				if(file.enabled) {
					File.Copy(file.info.FullName, file.info.FullName.Replace(rootParentDir, outputLocation));
				}
			}
			foreach(var dir in node.directories) {
				CopyEnabledFiles(dir, rootParentDir, outputLocation);
			}
		}

		private void OpenInputExplorer(object sender, RoutedEventArgs e) {
			using(var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
				dialog.Description = "Select your Addon Directory";
				dialog.UseDescriptionForTitle = true;

				System.Windows.Forms.DialogResult result = dialog.ShowDialog();

				var myTextBlock = (RichTextBox)this.FindName("InputLocation");
				if(myTextBlock != null && result.ToString() == "OK") {
					myTextBlock.Document.Blocks.Clear();
					myTextBlock.Document.Blocks.Add(new Paragraph(new Run(dialog.SelectedPath)));
					MainPanel.Children.Clear();
					DirectoryInfo addonDir = new(dialog.SelectedPath);
					var rootNode = new DirectoryNode(addonDir);
					InitSelectionTree(rootNode);
				}
			}
		}

		private void OpenOutputExplorer(object sender, RoutedEventArgs e) {
			using(var dialog = new System.Windows.Forms.FolderBrowserDialog()) {
				dialog.Description = "Select your Output Directory";
				dialog.UseDescriptionForTitle = true;

				System.Windows.Forms.DialogResult result = dialog.ShowDialog();

				var myTextBlock = (RichTextBox)this.FindName("OutputLocation");
				if(myTextBlock != null && result.ToString() == "OK") {
					myTextBlock.Document.Blocks.Clear();
					myTextBlock.Document.Blocks.Add(new Paragraph(new Run(dialog.SelectedPath)));
				}
			}
		}
	}

	public partial class SelectionNode : CheckBox {
		public bool IsDirectory {
			get { return (bool)GetValue(IsDirectoryProperty); }
			set { SetValue(IsDirectoryProperty, value); }
		}

		private SelectionItem selectionItem;

		public SelectionNode(SelectionItem selectionItem) {
			this.selectionItem = selectionItem;
			this.IsChecked = this.selectionItem.enabled;
			if(this.selectionItem is SelectionFile file) {
				this.Content = "File | " + file.info.Name;
			} else if(this.selectionItem is SelectionDirectory dir) {
				this.Content = "Dir | " + dir.info.Name;
			}
			this.selectionItem.OnEnabledChanged += HandleCheckedChanged;

			// we don't care about empty folders, do we? disable the button entirely
			if(this.selectionItem.node.CheckEmpty()) {
				this.IsEnabled = false;
			}

			this.Click += new RoutedEventHandler(selection_node_checked);
		}

		private void HandleCheckedChanged(bool state) {
			IsChecked = state;
		}

		private static void selection_node_checked(object sender, RoutedEventArgs e) {
			if(sender is SelectionNode selection) {
				if(selection.selectionItem is SelectionDirectory dir) {
					switch(selection.IsChecked) {
						case true: dir.node.EnableRecursively(); break;
						case false: dir.node.DisableRecursively(); break;
					}
				} else if(selection.selectionItem is SelectionFile file) {
					switch(selection.IsChecked) {
						case true: file.Enable(); break;
						case false: file.Disable(); break;
					}
				}
				// make sure ascending dir gets enabled/disabled accordingly
				// example: all files are disabled = disable folder
				//			folder is disabled but file is enabled = enable folders
				selection.selectionItem.node.VerifyContents(true);
			}
		}

		public static readonly DependencyProperty IsDirectoryProperty = DependencyProperty.Register("Directory", typeof(bool), typeof(SelectionNode));
	}
}
