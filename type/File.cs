using System.IO;
public class File {
	public FileInfo info;
	public bool enabled;

	public File(FileInfo info, bool enabled = true) {
		this.info = info;
		this.enabled = enabled;
	}
}
