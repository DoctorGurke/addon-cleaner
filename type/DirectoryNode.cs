using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AddonCleaner.Type {
	public class DirectoryNode {
		public DirectoryNode node;
		public SelectionDirectory self;
		public int indent;
		public List<SelectionFile> files = new();
		public List<DirectoryNode> directories = new();

		public DirectoryNode(DirectoryInfo info, DirectoryNode node = null, int indent = 0) {
			this.self = new SelectionDirectory(info, this);
			this.node = node;
			this.indent = indent;
			foreach(var file in info.GetFiles()) {
				var newFile = new SelectionFile(file, this);
				this.files.Add(newFile);
			}
			foreach(var dir in info.GetDirectories()) {
				this.directories.Add(new DirectoryNode(dir, this, indent + 1));
			}
			VerifyIntegrity();
		}

		public void VerifyIntegrity(bool ascending = false) {

			if(!self.enabled) {
				DisableRecursively();
				return;
			}

			// empty folders should be disabled
			if(files.Count <= 0 && directories.Count <= 0) {
				self.enabled = false;
			}

			// dirty recursive way to check if a directory's files and sub directories are ALL disabled
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
				self.Disable();
				//self.enabled = false;

			if(ascending) {
				node?.VerifyIntegrity(ascending);
			}
		}

		public void EnableRecursively() {
			foreach(var file in files) {
				//file.enabled = true;
				file.Enable(false);
			}
			foreach(var dir in directories) {
				//dir.self.enabled = true;
				dir.self.Enable(false);
				dir.EnableRecursively();
			}
		}

		public void DisableRecursively() {
			foreach(var file in files) {
				//file.enabled = false;
				file.Disable(false);
			}
			foreach(var dir in directories) {
				//dir.self.enabled = false;
				dir.self.Disable(false);
				dir.DisableRecursively();
			}
		}

		public void PrintTree() {
			{ // scope for reusable indentString var
				var indentString = "";
				for(int i = 0; i < indent; i++) {
					indentString += "\t";
				}
				MainWindow.PrintToConsole($"{indentString}d {self.info.Name} {self.enabled}");
				foreach(var dir in directories) {
					dir.PrintTree();
				}
			}
			foreach(var file in files) {
				var indentString = "";
				for(int i = 0; i < indent + 1; i++) {
					indentString += "\t";
				}
				MainWindow.PrintToConsole($"{indentString}f {file.info.Name} {file.enabled}");
			}
		}

		public string[] GetFilesRecursively(HashSet<string> fileList = null) {
			var set = fileList;
			if(set == null)
				set = new HashSet<string>();
			foreach(var file in files.Where((f) => f.enabled)) {
				set.Add(file.info.FullName);
			}
			foreach(var dir in directories.Where((d) => d.self.enabled)) {
				dir.GetFilesRecursively(set);
			}
			return set.ToArray();
		}
	}
}
