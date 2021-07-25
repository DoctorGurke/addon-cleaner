using System.IO;

namespace AddonCleaner.Type {
	public class SelectionDirectory : SelectionItem {
		public readonly DirectoryInfo info;

		public bool collapsed = false;

		public SelectionDirectory(DirectoryInfo info, DirectoryNode node) {
			this.info = info;
			this.node = node;
			this.enabled = VerifyDirectory();
		}

		// blacklisted directories
		private bool VerifyDirectory() {
			switch(info.Name) {
				case "obj":
				case "Properties":
					return false;
			}
			return true;
		}
	}
}
