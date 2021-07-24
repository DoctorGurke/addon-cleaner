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

	public partial class TestButton : Button {
		public bool IsDirectory {
			get { return (bool)GetValue(IsDirectoryProperty); }
			set { SetValue(IsDirectoryProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Property1.  
		// This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsDirectoryProperty = DependencyProperty.Register("Directory", typeof(bool), typeof(MainWindow));
	}

	public partial class MainWindow : Window {
		private static RichTextBox Output;
		private static Grid InputArea;

		

		public MainWindow() {
            InitializeComponent();
			Output = (RichTextBox)this.FindName("debugoutput");
			InputArea = (Grid)this.FindName("MainGrid");

			InitTestButtons();

			DirectoryInfo addonDir = new("C:\\Users\\docgu\\Desktop\\addon");
			var rootNode = new DirectoryNode(addonDir);

			//rootNode.PrintTree();
			//System.Console.WriteLine(rootNode.self.info.FullName);
			rootNode.PrintTree();
			//foreach(var path in rootNode.GetFilesRecursively()) {
			//	PrintToConsole(path);
			//}
		}

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

		private static void button_test_click(object sender, RoutedEventArgs e) {
			if(sender is TestButton button) {
				PrintToConsole($"{button.IsDirectory}");
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
