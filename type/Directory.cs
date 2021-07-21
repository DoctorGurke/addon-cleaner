using System.IO;

namespace AddonCleaner.Type {
	public class Directory {
		public DirectoryInfo info;
		public bool enabled;

		public Directory(DirectoryInfo info, bool enabled = true) {
			this.info = info;
			this.enabled = enabled;
		}
	}
}
