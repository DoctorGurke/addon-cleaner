namespace AddonCleaner.Type {
	public abstract class SelectionItem {
		public DirectoryNode node;
		public bool enabled;

		public virtual void Enable(bool ascending = true) {
			enabled = true;
			node.VerifyIntegrity(ascending);
			OnEnabledChanged?.Invoke(true);
		}

		public virtual void Disable(bool ascending = true) {
			enabled = false;
			node.VerifyIntegrity(ascending);
			OnEnabledChanged?.Invoke(false);
		}

		public delegate void EnabledChanged(bool state);
		public event EnabledChanged OnEnabledChanged;
	}
}
