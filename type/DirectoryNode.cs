using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AddonCleaner.Type {
	public class DirectoryNode {
		// this naming is super ambiguous and I should rethink this
		public DirectoryNode parentNode;
		public SelectionDirectory self;
		public int indent;
		public List<SelectionFile> files = new();
		public List<DirectoryNode> directories = new();

		public DirectoryNode(DirectoryInfo info, DirectoryNode parentNode = null, int indent = 0) {
			this.self = new SelectionDirectory(info, this);
			this.parentNode = parentNode;
			this.indent = indent;
			foreach(var file in info.GetFiles()) {
				var newFile = new SelectionFile(file, this);
				this.files.Add(newFile);
			}
			foreach(var dir in info.GetDirectories()) {
				this.directories.Add(new DirectoryNode(dir, this, indent + 1));
			}
			InitializeIntegrity();
		}

		public void InitializeIntegrity() {
			// whole directory is disabled, disable sub files/directories and be done
			if(!self.enabled) {
				foreach(var file in files) {
					file.enabled = false;
				}
				foreach(var dir in directories) {
					dir.self.enabled = false;
					dir.DisableRecursively();
				}
				return;
			}

			CheckEmpty();

			VerifyContents();
		}

		public bool CheckEmpty() {
			// empty folders should be disabled
			if(files.Count <= 0 && directories.Count <= 0) {
				self.enabled = false;
				return true;
			}
			return false;
		}

		public void VerifyContents(bool ascending = false) {
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
			else
				self.Enable();

			// enabled/disabled via ui, make sure ascending nodes get enabled/disabled accordingly
			if(ascending) {
				parentNode?.VerifyContents(true);
			}
		}

		public void EnableRecursively() {
			foreach(var file in files) {
				file.Enable(false);
			}
			foreach(var dir in directories) {
				dir.self.Enable(false);
				dir.EnableRecursively();
			}
		}

		public void DisableRecursively() {
			foreach(var file in files) {
				file.Disable(false);
			}
			foreach(var dir in directories) {
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
