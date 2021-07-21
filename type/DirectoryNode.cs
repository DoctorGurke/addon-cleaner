using System.Collections.Generic;
using System.IO;

namespace AddonCleaner.Type {
	public class DirectoryNode {
		public DirectoryNode node;
		public Directory self;
		public int indent;
		public List<File> files = new();
		public List<DirectoryNode> directories = new();

		public DirectoryNode(DirectoryInfo info, DirectoryNode node, int indent = 0) {
			this.self = new Directory(info, this);
			this.node = node;
			this.indent = indent;
			foreach(var file in info.GetFiles()) {
				var newFile = new File(file, this);
				this.files.Add(newFile);
			}
			foreach(var dir in info.GetDirectories()) {
				this.directories.Add(new DirectoryNode(dir, this, indent + 1));
			}
			VerifyIntegrity();
		}

		public void VerifyIntegrity(bool ascending = false) {
			// empty folders should be disabled
			if(files.Count <= 0 && directories.Count <= 0) {
				self.enabled = false;
			}

			// dirty recursive way to check if a directorie's files and sub directories are ALL disabled
			// in this case, disable it as well
			var check = false;
			foreach(var dir in directories) {
				if(dir.self.enabled)
					check = true;
			}
			foreach(var file in files) {
				if(file.enabled)
					check = true;
			}
			if(!check)
				self.enabled = false;

			if(ascending) {
				node.VerifyIntegrity(ascending);
			}
		}

		public void PrintTree() {
			for(int i = 0; i < indent; i++) {
				System.Console.Write("\t");
			}
			System.Console.WriteLine($"d {self.info.Name} {self.enabled}");
			foreach(var dir in directories) {
				dir.PrintTree();
			}
			foreach(var file in files) {
				for(int i = 0; i < indent + 1; i++) {
					System.Console.Write("\t");
				}
				System.Console.WriteLine($"f {file.info.Name} {file.enabled}");
			}
		}
	}
}
