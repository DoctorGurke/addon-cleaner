using System;
using System.IO;
using AddonCleaner.Type;

namespace AddonCleaner {
	class Program {
		static void Main(string[] args) {
			//"C:\Users\docgu\Desktop\addon"
			DirectoryInfo addonDir = new("C:\\Users\\docgu\\Desktop\\addon");
			var rootNode = new DirectoryNode(addonDir);

			//rootNode.PrintTree();
			System.Console.WriteLine(rootNode.self.info.FullName);
		}
	}
}
