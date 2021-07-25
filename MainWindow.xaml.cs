using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AddonCleaner.Type;
using System.IO;

namespace AddonCleaner {

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
				this.Content = "F " + file.info.Name;
			} else if(this.selectionItem is SelectionDirectory dir) {
				this.Content = "D " + dir.info.Name;
			}
			this.selectionItem.OnEnabledChanged += HandleCheckedChanged;

			// we don't care about empty folders, do we? 
			if(this.selectionItem.node.CheckEmpty()) {
				this.IsEnabled = false;
			}
			
			this.Click += new RoutedEventHandler(selection_node_checked);
		}

		private void HandleCheckedChanged(bool state) {
			//MainWindow.PrintToConsole($"{state}");
			IsChecked = state;
		}

		private static void selection_node_checked(object sender, RoutedEventArgs e) {
			if(sender is SelectionNode selection) {
				// switch statement cause why not, clean enough
				
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
			//MainWindow.PrintToConsole($"{((SelectionNode)sender).IsDirectory}");
		}

		public static readonly DependencyProperty IsDirectoryProperty = DependencyProperty.Register("Directory", typeof(bool), typeof(SelectionNode));
	}

	public partial class MainWindow : Window {
		private static RichTextBox Output;
		private static StackPanel MainPanel;

		public MainWindow() {
            InitializeComponent();
			Output = (RichTextBox)this.FindName("debugoutput");
			MainPanel = (StackPanel)this.FindName("SelectionItemPanel");

			DirectoryInfo addonDir = new("C:\\Users\\docgu\\Desktop\\addon");
			var rootNode = new DirectoryNode(addonDir);

			rootNode.PrintTree();
			InitSelectionTree(rootNode);
		}

		private static void InitSelectionTree(DirectoryNode node) {
			//MainWindow.PrintToConsole($"{node.indent}d {node.self.info.Name} {node.self.enabled}");
			var dirNode = new SelectionNode(node.self);
			dirNode.Margin = new Thickness(node.indent * 35, 0, 0, 0);
			MainPanel.Children.Add(dirNode);
			foreach(var dir in node.directories) {
				InitSelectionTree(dir);
			}
			foreach(var file in node.files) {
				//MainWindow.PrintToConsole($"{node.indent + 1}f {file.info.Name} {file.enabled}");
				var fileNode = new SelectionNode(file);
				fileNode.Margin = new Thickness((node.indent + 1) * 35, 0, 0, 0);//(node.indent + 1) * 5
				
				MainPanel.Children.Add(fileNode);
			}
		}

		public static void PrintToConsole(string text) {
			Application.Current.Dispatcher.Invoke(() => {
				Output.Document.Blocks.Add(new Paragraph(new Run(text)));
				Output.ScrollToEnd();
			});
		}
	}
}
