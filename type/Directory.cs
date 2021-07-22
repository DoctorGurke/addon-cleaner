using System.IO;

namespace AddonCleaner.Type {
	public class Directory : Item {
		public readonly DirectoryInfo info;

		public Directory(DirectoryInfo info, DirectoryNode node) {
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
