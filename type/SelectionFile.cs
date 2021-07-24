using System.IO;

namespace AddonCleaner.Type {
	public class SelectionFile : SelectionItem {
		public readonly FileInfo info;

		public SelectionFile(FileInfo info, DirectoryNode node) {
			this.info = info;
			this.node = node;
			this.enabled = VerifyFile();
		}

		// whitelisted default file extensions
		private bool VerifyFile() {
			switch(info.Extension) {
				// assets
				case ".vmdl_c": // model
				case ".vpcf_c": // particle
				case ".vmat_c": // material
				case ".vtex_c": // texture
				case ".vsnd_c": // sound
				case ".sound_c": // sount asset
				case ".decal_c": // decal asset
				case ".surface_c": // surface asset
				case ".vpk": // archive, usually a map

				// code 
				case ".cs": // csharp
				case ".scss": // scss style
				case ".html": // html template

				// misc
				case ".fgd": // config file
				case ".addon": // addon info

					return true;
			}
			return false;
		}
	}
}
