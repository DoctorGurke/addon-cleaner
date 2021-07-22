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
    public partial class MainWindow : Window {
		private static RichTextBox Output;

        public MainWindow() {
            InitializeComponent();
			Output = (RichTextBox)this.FindName("debugoutput");

			DirectoryInfo addonDir = new("C:\\Users\\docgu\\Desktop\\addon");
			var rootNode = new DirectoryNode(addonDir);

			//rootNode.PrintTree();
			//System.Console.WriteLine(rootNode.self.info.FullName);
			rootNode.PrintTree();
			//foreach(var path in rootNode.GetFilesRecursively()) {
			//	PrintToConsole(path);
			//}
		}

		public static void PrintToConsole(string text) {
			Application.Current.Dispatcher.Invoke(() => {
				Output.Document.Blocks.Add(new Paragraph(new Run(text)));
				Output.ScrollToEnd();
			});
		}
	}
}
