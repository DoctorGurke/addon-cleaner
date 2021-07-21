using System;
using System.IO;

namespace asset_cleaner_cli {
	class Program {
		static void Main(string[] args) {
			//"C:\Users\docgu\Desktop\addon"
			DirectoryInfo addonDir = new("C:\\Users\\docgu\\Desktop\\addon");
			var rootNode = new DirectoryNode("addon", addonDir);

			rootNode.PrintTree();

			//foreach(var item in rootNode.directories) {
			//	System.Console.WriteLine(item.name);
			//}
		}
	}
}
