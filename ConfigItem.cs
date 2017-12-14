using System;
using System.Windows.Forms;

namespace HttpConfig
{
    public enum ModifiedStatus
    {
        Unmodified,
        Added,
        Modified,
        Removed
    }

	public abstract class ConfigItem : ListViewItem
	{
        private ModifiedStatus _status = ModifiedStatus.Added;

        protected ConfigItem() { }

        public ModifiedStatus Status
        {
            get { return _status;  }
            set { _status = value; }
        }

        public abstract void ApplyConfig();
        public abstract string Key { get; }
	}
}
