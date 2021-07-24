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
			this.Click += new RoutedEventHandler(selection_node_checked);
		}

		private static void selection_node_checked(object sender, RoutedEventArgs e) {
			if(sender is SelectionNode selection) {
				if(selection.selectionItem is SelectionDirectory dir) {
					// falling disable
				} else if (selection.selectionItem is SelectionFile file) {
					// verify ascending
				}
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

			//rootNode.PrintTree();
			//System.Console.WriteLine(rootNode.self.info.FullName);
			//rootNode.PrintTree();
			InitSelectionTree(rootNode);
			//foreach(var path in rootNode.GetFilesRecursively()) {
			//	PrintToConsole(path);
			//}
		}

		private static void InitSelectionTree(DirectoryNode node) {
			MainWindow.PrintToConsole($"{node.indent}d {node.self.info.Name} {node.self.enabled}");
			var dirNode = new SelectionNode(node.self);
			dirNode.Margin = new Thickness(node.indent * 25, 0, 0, 0);
			MainPanel.Children.Add(dirNode);
			foreach(var dir in node.directories) {
				InitSelectionTree(dir);
			}
			foreach(var file in node.files) {
				MainWindow.PrintToConsole($"{node.indent + 1}f {file.info.Name} {file.enabled}");
				var fileNode = new SelectionNode(file);
				fileNode.Margin = new Thickness((node.indent + 1) * 25, 0, 0, 0);//(node.indent + 1) * 5
				
				MainPanel.Children.Add(fileNode);
			}
		}

		/*
		private static void InitTestButtons() {
			
			var button1 = new TestButton();
			var style1 = new Style(typeof(Button));
			var setter1 = new Setter(TestButton.IsDirectoryProperty, true);
			style1.Setters.Add(setter1);
			setter1 = new Setter(ContentProperty, "Directory");
			style1.Setters.Add(setter1);
			button1.Style = style1;
			InputArea.Children.Add(button1);

			Grid.SetColumn(button1, 0);
			Grid.SetColumnSpan(button1, 1);
			Grid.SetRow(button1, 0);
			Grid.SetRowSpan(button1, 1);

			button1.Click += new RoutedEventHandler(button_test_click);

			var button2 = new TestButton();
			var style2 = new Style(typeof(Button));
			var setter2 = new Setter(TestButton.IsDirectoryProperty, false);
			style2.Setters.Add(setter2);
			setter2 = new Setter(ContentProperty, "File");
			style2.Setters.Add(setter2);
			button2.Style = style2;
			InputArea.Children.Add(button2);

			Grid.SetColumn(button2, 1);
			Grid.SetColumnSpan(button2, 1);
			Grid.SetRow(button2, 0);
			Grid.SetRowSpan(button2, 1);

			button2.Click += new RoutedEventHandler(button_test_click);
			
		}
		*/

		public static void PrintToConsole(string text) {
			Application.Current.Dispatcher.Invoke(() => {
				Output.Document.Blocks.Add(new Paragraph(new Run(text)));
				Output.ScrollToEnd();
			});
		}
	}
}
