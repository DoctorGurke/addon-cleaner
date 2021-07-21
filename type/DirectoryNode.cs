using System.Collections.Generic;
using System.IO;

public class DirectoryNode {
	public Directory self;
	public int indent;
	public List<File> files = new();
	public List<DirectoryNode> directories = new();

	public DirectoryNode(DirectoryInfo info, int indent = 0) {
		this.self = new Directory(info);
		this.indent = indent;
		foreach(var file in info.GetFiles()) {
			this.files.Add(new File(file));
		}
		foreach(var dir in info.GetDirectories()) {
			this.directories.Add(new DirectoryNode(dir, indent + 1));
		}
	}

	public void PrintTree() {
		for(int i = 0; i < indent; i++) {
			System.Console.Write("\t");
		}
		System.Console.WriteLine($"d {self.info.Name}");
		foreach(var dir in directories) {
			dir.PrintTree();
		}
		foreach(var file in files) {
			for(int i = 0; i < indent + 1; i++) {
				System.Console.Write("\t");
			}
			System.Console.WriteLine($"f {file}");
		}
	}
}
