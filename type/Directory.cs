using System.IO;

namespace AddonCleaner.Type {
	public class Directory {
		public DirectoryInfo info;
		public bool enabled;

		public Directory(DirectoryInfo info) {
			this.info = info;
			this.enabled = VerifyDirectory();
		}

		// blacklisted directories
		private bool VerifyDirectory() {
			switch(info.Name) {
				case "obj":
				case "Propterties":
					return false;
			}
			return true;
		}
	}
}
