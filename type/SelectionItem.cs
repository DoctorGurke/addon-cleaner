namespace AddonCleaner.Type {
	public abstract class SelectionItem {
		public DirectoryNode node;
		public bool enabled;

		public virtual void Enable() {
			enabled = true;
			node.VerifyIntegrity(true);
		}
		public virtual void Disable() {
			enabled = false;
			node.VerifyIntegrity(true);
		}
	}
}
