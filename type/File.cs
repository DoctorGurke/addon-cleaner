using System.IO;

namespace AddonCleaner.Type {
	public class File {
		public FileInfo info;
		public bool enabled;

		public File(FileInfo info) {
			this.info = info;
			this.enabled = VerifyFile();

		}

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
