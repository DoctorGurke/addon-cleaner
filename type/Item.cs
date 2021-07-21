namespace AddonCleaner.Type {
	public abstract class Item {
		public DirectoryNode node;
		public bool enabled;
		public void Enable() {
			enabled = true;
			node.VerifyIntegrity(true);
		}
		public void Disable() {
			enabled = false;
			node.VerifyIntegrity(true);
		}
	}
}
