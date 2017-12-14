using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace HttpConfig
{
	public class MainForm : System.Windows.Forms.Form
	{
		private bool _isInitialized = false;
        
        private Hashtable _ipListenItems;
        private Hashtable _urlAclItems;
        private Hashtable _sslItems;

        private System.Windows.Forms.TabControl typeTabStrip;
        private System.Windows.Forms.TabPage sslTabPage;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ListView urlAclListView;
        private System.Windows.Forms.ColumnHeader urlColumnHeader;
        private System.Windows.Forms.ColumnHeader aclColumnHeader;
        private System.Windows.Forms.Button removeUrlAclButton;
        private System.Windows.Forms.Button addUrlAclButton;
        private System.Windows.Forms.Button removeSslButton;
        private System.Windows.Forms.Button addSslButton;
        private System.Windows.Forms.ListView sslListView;
        private System.Windows.Forms.ColumnHeader addressColumnHeader;
        private System.Windows.Forms.ColumnHeader certNameColumnHeader;
        private System.Windows.Forms.ColumnHeader storeColumnHeader;
        private System.Windows.Forms.TabPage urlAclTabPage;
        private System.Windows.Forms.TabPage ipListenersTabPage;
        private System.Windows.Forms.Button removeIpListenerButton;
        private System.Windows.Forms.Button addIpListenerButton;
        private System.Windows.Forms.ListView ipListenersListView;
        private System.Windows.Forms.ColumnHeader ipListenerAddressColumnHeader;
		private Button editUrlAclButton;
        private Button editSslButton;

		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			InitializeComponent();
		}

		protected override void Dispose( bool disposing )
		{
			if(disposing)
			{
				if(components != null)
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.typeTabStrip = new System.Windows.Forms.TabControl();
            this.ipListenersTabPage = new System.Windows.Forms.TabPage();
            this.removeIpListenerButton = new System.Windows.Forms.Button();
            this.addIpListenerButton = new System.Windows.Forms.Button();
            this.ipListenersListView = new System.Windows.Forms.ListView();
            this.ipListenerAddressColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.sslTabPage = new System.Windows.Forms.TabPage();
            this.removeSslButton = new System.Windows.Forms.Button();
            this.addSslButton = new System.Windows.Forms.Button();
            this.sslListView = new System.Windows.Forms.ListView();
            this.addressColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.certNameColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.storeColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.urlAclTabPage = new System.Windows.Forms.TabPage();
            this.editUrlAclButton = new System.Windows.Forms.Button();
            this.removeUrlAclButton = new System.Windows.Forms.Button();
            this.addUrlAclButton = new System.Windows.Forms.Button();
            this.urlAclListView = new System.Windows.Forms.ListView();
            this.urlColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.aclColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.applyButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.editSslButton = new System.Windows.Forms.Button();
            this.typeTabStrip.SuspendLayout();
            this.ipListenersTabPage.SuspendLayout();
            this.sslTabPage.SuspendLayout();
            this.urlAclTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // typeTabStrip
            // 
            this.typeTabStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typeTabStrip.Controls.Add(this.ipListenersTabPage);
            this.typeTabStrip.Controls.Add(this.sslTabPage);
            this.typeTabStrip.Controls.Add(this.urlAclTabPage);
            this.typeTabStrip.Location = new System.Drawing.Point(0, 0);
            this.typeTabStrip.Multiline = true;
            this.typeTabStrip.Name = "typeTabStrip";
            this.typeTabStrip.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.typeTabStrip.SelectedIndex = 0;
            this.typeTabStrip.Size = new System.Drawing.Size(370, 366);
            this.typeTabStrip.TabIndex = 2;
            // 
            // ipListenersTabPage
            // 
            this.ipListenersTabPage.Controls.Add(this.removeIpListenerButton);
            this.ipListenersTabPage.Controls.Add(this.addIpListenerButton);
            this.ipListenersTabPage.Controls.Add(this.ipListenersListView);
            this.ipListenersTabPage.Location = new System.Drawing.Point(4, 22);
            this.ipListenersTabPage.Name = "ipListenersTabPage";
            this.ipListenersTabPage.Size = new System.Drawing.Size(362, 340);
            this.ipListenersTabPage.TabIndex = 3;
            this.ipListenersTabPage.Text = "Listeners";
            this.ipListenersTabPage.UseVisualStyleBackColor = true;
            // 
            // removeIpListenerButton
            // 
            this.removeIpListenerButton.BackColor = System.Drawing.Color.White;
            this.removeIpListenerButton.Enabled = false;
            this.removeIpListenerButton.Location = new System.Drawing.Point(72, 8);
            this.removeIpListenerButton.Name = "removeIpListenerButton";
            this.removeIpListenerButton.Size = new System.Drawing.Size(56, 23);
            this.removeIpListenerButton.TabIndex = 5;
            this.removeIpListenerButton.Text = "&Remove";
            this.removeIpListenerButton.UseVisualStyleBackColor = false;
            this.removeIpListenerButton.Click += new System.EventHandler(this.removeIpListenerButton_Click);
            // 
            // addIpListenerButton
            // 
            this.addIpListenerButton.BackColor = System.Drawing.Color.White;
            this.addIpListenerButton.Location = new System.Drawing.Point(16, 8);
            this.addIpListenerButton.Name = "addIpListenerButton";
            this.addIpListenerButton.Size = new System.Drawing.Size(56, 23);
            this.addIpListenerButton.TabIndex = 4;
            this.addIpListenerButton.Text = "A&dd";
            this.addIpListenerButton.UseVisualStyleBackColor = false;
            this.addIpListenerButton.Click += new System.EventHandler(this.addIpListenerButton_Click);
            // 
            // ipListenersListView
            // 
            this.ipListenersListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ipListenersListView.BackColor = System.Drawing.SystemColors.Window;
            this.ipListenersListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ipListenerAddressColumnHeader});
            this.ipListenersListView.FullRowSelect = true;
            this.ipListenersListView.HideSelection = false;
            this.ipListenersListView.Location = new System.Drawing.Point(0, 32);
            this.ipListenersListView.Name = "ipListenersListView";
            this.ipListenersListView.Size = new System.Drawing.Size(360, 312);
            this.ipListenersListView.TabIndex = 3;
            this.ipListenersListView.UseCompatibleStateImageBehavior = false;
            this.ipListenersListView.View = System.Windows.Forms.View.Details;
            this.ipListenersListView.SelectedIndexChanged += new System.EventHandler(this.ipListenersListView_SelectedIndexChanged);
            // 
            // ipListenerAddressColumnHeader
            // 
            this.ipListenerAddressColumnHeader.Text = "IP Address";
            this.ipListenerAddressColumnHeader.Width = 300;
            // 
            // sslTabPage
            // 
            this.sslTabPage.Controls.Add(this.editSslButton);
            this.sslTabPage.Controls.Add(this.removeSslButton);
            this.sslTabPage.Controls.Add(this.addSslButton);
            this.sslTabPage.Controls.Add(this.sslListView);
            this.sslTabPage.Location = new System.Drawing.Point(4, 22);
            this.sslTabPage.Name = "sslTabPage";
            this.sslTabPage.Size = new System.Drawing.Size(362, 340);
            this.sslTabPage.TabIndex = 1;
            this.sslTabPage.Text = "SSL";
            this.sslTabPage.UseVisualStyleBackColor = true;
            // 
            // removeSslButton
            // 
            this.removeSslButton.BackColor = System.Drawing.Color.White;
            this.removeSslButton.Enabled = false;
            this.removeSslButton.Location = new System.Drawing.Point(72, 8);
            this.removeSslButton.Name = "removeSslButton";
            this.removeSslButton.Size = new System.Drawing.Size(56, 23);
            this.removeSslButton.TabIndex = 11;
            this.removeSslButton.Text = "&Remove";
            this.removeSslButton.UseVisualStyleBackColor = false;
            this.removeSslButton.Click += new System.EventHandler(this.removeSslButton_Click);
            // 
            // addSslButton
            // 
            this.addSslButton.BackColor = System.Drawing.Color.White;
            this.addSslButton.Location = new System.Drawing.Point(16, 8);
            this.addSslButton.Name = "addSslButton";
            this.addSslButton.Size = new System.Drawing.Size(56, 23);
            this.addSslButton.TabIndex = 10;
            this.addSslButton.Text = "A&dd";
            this.addSslButton.UseVisualStyleBackColor = false;
            this.addSslButton.Click += new System.EventHandler(this.addSslButton_Click);
            // 
            // sslListView
            // 
            this.sslListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sslListView.BackColor = System.Drawing.SystemColors.Window;
            this.sslListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.addressColumnHeader,
            this.certNameColumnHeader,
            this.storeColumnHeader});
            this.sslListView.FullRowSelect = true;
            this.sslListView.Location = new System.Drawing.Point(0, 32);
            this.sslListView.Name = "sslListView";
            this.sslListView.Size = new System.Drawing.Size(360, 312);
            this.sslListView.TabIndex = 5;
            this.sslListView.UseCompatibleStateImageBehavior = false;
            this.sslListView.View = System.Windows.Forms.View.Details;
            this.sslListView.DoubleClick += new System.EventHandler(this.sslListView_DoubleClick);
            this.sslListView.SelectedIndexChanged += new System.EventHandler(this.sslListView_SelectedIndexChanged);
            // 
            // addressColumnHeader
            // 
            this.addressColumnHeader.Text = "Address:Port";
            this.addressColumnHeader.Width = 119;
            // 
            // certNameColumnHeader
            // 
            this.certNameColumnHeader.Text = "Cert Name";
            this.certNameColumnHeader.Width = 80;
            // 
            // storeColumnHeader
            // 
            this.storeColumnHeader.Text = "Store";
            this.storeColumnHeader.Width = 54;
            // 
            // urlAclTabPage
            // 
            this.urlAclTabPage.Controls.Add(this.editUrlAclButton);
            this.urlAclTabPage.Controls.Add(this.removeUrlAclButton);
            this.urlAclTabPage.Controls.Add(this.addUrlAclButton);
            this.urlAclTabPage.Controls.Add(this.urlAclListView);
            this.urlAclTabPage.Location = new System.Drawing.Point(4, 22);
            this.urlAclTabPage.Name = "urlAclTabPage";
            this.urlAclTabPage.Size = new System.Drawing.Size(362, 340);
            this.urlAclTabPage.TabIndex = 2;
            this.urlAclTabPage.Text = "Permissions";
            this.urlAclTabPage.UseVisualStyleBackColor = true;
            // 
            // editUrlAclButton
            // 
            this.editUrlAclButton.BackColor = System.Drawing.Color.White;
            this.editUrlAclButton.Enabled = false;
            this.editUrlAclButton.Location = new System.Drawing.Point(128, 8);
            this.editUrlAclButton.Name = "editUrlAclButton";
            this.editUrlAclButton.Size = new System.Drawing.Size(56, 23);
            this.editUrlAclButton.TabIndex = 10;
            this.editUrlAclButton.Text = "&Edit";
            this.editUrlAclButton.UseVisualStyleBackColor = false;
            this.editUrlAclButton.Click += new System.EventHandler(this.editUrlAclButton_Click);
            // 
            // removeUrlAclButton
            // 
            this.removeUrlAclButton.BackColor = System.Drawing.Color.White;
            this.removeUrlAclButton.Enabled = false;
            this.removeUrlAclButton.Location = new System.Drawing.Point(72, 8);
            this.removeUrlAclButton.Name = "removeUrlAclButton";
            this.removeUrlAclButton.Size = new System.Drawing.Size(56, 23);
            this.removeUrlAclButton.TabIndex = 9;
            this.removeUrlAclButton.Text = "&Remove";
            this.removeUrlAclButton.UseVisualStyleBackColor = false;
            this.removeUrlAclButton.Click += new System.EventHandler(this.removeUrlAclButton_Click);
            // 
            // addUrlAclButton
            // 
            this.addUrlAclButton.BackColor = System.Drawing.Color.White;
            this.addUrlAclButton.Location = new System.Drawing.Point(16, 8);
            this.addUrlAclButton.Name = "addUrlAclButton";
            this.addUrlAclButton.Size = new System.Drawing.Size(56, 23);
            this.addUrlAclButton.TabIndex = 8;
            this.addUrlAclButton.Text = "A&dd";
            this.addUrlAclButton.UseVisualStyleBackColor = false;
            this.addUrlAclButton.Click += new System.EventHandler(this.addUrlAclButton_Click);
            // 
            // urlAclListView
            // 
            this.urlAclListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.urlAclListView.BackColor = System.Drawing.SystemColors.Window;
            this.urlAclListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.urlColumnHeader,
            this.aclColumnHeader});
            this.urlAclListView.FullRowSelect = true;
            this.urlAclListView.Location = new System.Drawing.Point(0, 32);
            this.urlAclListView.Name = "urlAclListView";
            this.urlAclListView.Size = new System.Drawing.Size(360, 312);
            this.urlAclListView.TabIndex = 6;
            this.urlAclListView.UseCompatibleStateImageBehavior = false;
            this.urlAclListView.View = System.Windows.Forms.View.Details;
            this.urlAclListView.DoubleClick += new System.EventHandler(this.urlAclListView_DoubleClick);
            this.urlAclListView.SelectedIndexChanged += new System.EventHandler(this.urlAclListView_SelectedIndexChanged);
            // 
            // urlColumnHeader
            // 
            this.urlColumnHeader.Text = "URL";
            this.urlColumnHeader.Width = 143;
            // 
            // aclColumnHeader
            // 
            this.aclColumnHeader.Text = "Permissions";
            this.aclColumnHeader.Width = 210;
            // 
            // applyButton
            // 
            this.applyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.applyButton.BackColor = System.Drawing.Color.White;
            this.applyButton.Enabled = false;
            this.applyButton.Location = new System.Drawing.Point(384, 64);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 1;
            this.applyButton.Text = "&Apply";
            this.applyButton.UseVisualStyleBackColor = false;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.BackColor = System.Drawing.Color.White;
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(384, 40);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.BackColor = System.Drawing.Color.White;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(384, 16);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // editSslButton
            // 
            this.editSslButton.BackColor = System.Drawing.Color.White;
            this.editSslButton.Enabled = false;
            this.editSslButton.Location = new System.Drawing.Point(128, 8);
            this.editSslButton.Name = "editSslButton";
            this.editSslButton.Size = new System.Drawing.Size(56, 23);
            this.editSslButton.TabIndex = 12;
            this.editSslButton.Text = "&Edit";
            this.editSslButton.UseVisualStyleBackColor = false;
            this.editSslButton.Click += new System.EventHandler(this.editSslButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(472, 366);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.typeTabStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HTTP Configuration Utility";
            this.Closed += new System.EventHandler(this.MainForm_Closed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.typeTabStrip.ResumeLayout(false);
            this.ipListenersTabPage.ResumeLayout(false);
            this.sslTabPage.ResumeLayout(false);
            this.urlAclTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
		#endregion

		[STAThread]
		static void Main() 
		{
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
            /*if(!SecurityApi.IsAdmin)
            {
                MessageBox.Show(
                    this,
                    "HttpConfig must be run by a machine administrator.",
                    "Insufficient Privileges",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);

                Close();
            }*/

            try
            {
                HttpApi.Error error = HttpApi.HttpInitialize(
                    new HttpApi.HTTPAPI_VERSION(1, 0),
                    HttpApi.InitFlag.HTTP_INITIALIZE_CONFIG,
                    IntPtr.Zero);

                if(error != HttpApi.Error.NO_ERROR)
                {
                    MessageBox.Show(this, "HttpInitialize failed (" + error.ToString() + ").  The application will exit.", "Initialization Error");
			
                    Close();
                }
                else
                {
                    _isInitialized = true;

                    LoadHttpConfigData();
                }
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
		}

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if(this.DialogResult != DialogResult.Cancel)
                    e.Cancel = !QueryApplyChanges();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
            try
            {
                if(_isInitialized)
                    HttpApi.HttpTerminate(HttpApi.InitFlag.HTTP_INITIALIZE_CONFIG, IntPtr.Zero);
            }
            catch { }
		}

        private void okButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ApplyChanges();
                
                Close();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void applyButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ApplyChanges();

                PopulateConfigListView(_ipListenItems, ipListenersListView);

                PopulateConfigListView(_urlAclItems, urlAclListView);

                PopulateConfigListView(_sslItems, sslListView);
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void cancelButton_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void LoadHttpConfigData()
        {
            Load_IpListenItems();

            Load_UrlAclItems();
            
            Load_SslItems();
        }

        private void PopulateConfigListView(Hashtable list, ListView view)
        {
            view.Items.Clear();

            foreach(ConfigItem item in list.Values)
            {
                if(item.Status != ModifiedStatus.Removed)
                    view.Items.Add(item);
            }

            AutoSizeColumns(view);
        }

        private bool QueryApplyChanges()
        {
            bool changesApplied = true;

            try
            {
                if(IsDirty())
                {
                    DialogResult response = MessageBox.Show(this, "Changes have been made.  Would you like to save them now?", "Confirm", MessageBoxButtons.YesNoCancel);

                    switch(response)
                    {
                        case DialogResult.No:
                            changesApplied = true;
                            break;

                        case DialogResult.Cancel:
                            changesApplied = false;
                            break;

                        case DialogResult.Yes:
                            ApplyChanges();
                            changesApplied = true;
                            break;

                        default:
                            changesApplied = false;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                DisplayError(ex);
                changesApplied = false;
            }

            return changesApplied;
        }

        private void ApplyChanges()
        {
            ApplyChanges(_ipListenItems);

            ApplyChanges(_urlAclItems);

            ApplyChanges(_sslItems);

            applyButton.Enabled = false;
        }

        private void OLD_ApplyChanges(Hashtable items)
        {
            ArrayList deletedItems = new ArrayList();

            foreach(ConfigItem item in items.Values)
            {
                if(item.Status != ModifiedStatus.Unmodified)
                {
                    item.ApplyConfig();

                    if(item.Status == ModifiedStatus.Removed)
                        deletedItems.Add(item);
                    else
                        item.Status = ModifiedStatus.Unmodified;
                }
            }

            foreach(ConfigItem deletedItem in deletedItems)
                items.Remove(deletedItem.Key);
        }

        private void ApplyChanges(Hashtable items)
        {
            ArrayList deletedItems = new ArrayList();

            // Do removals first.  That way, if a user deleted a setting, and then added a new one
            // with the same key, we don't get an "already exists" error from the APIs.
            foreach(ConfigItem item in items.Values)
            {
                if(item.Status == ModifiedStatus.Removed)
                {
                    item.ApplyConfig();

                    deletedItems.Add(item);
                }
            }

            foreach(ConfigItem deletedItem in deletedItems)
                items.Remove(deletedItem.Key);


            // Now process the rest of the edits/adds
            foreach(ConfigItem item in items.Values)
            {
                if(item.Status != ModifiedStatus.Unmodified)
                {
                    item.ApplyConfig();

                    item.Status = ModifiedStatus.Unmodified;
                }
            }
        }

        private bool IsDirty()
        {
            return IsDirty(_ipListenItems) || IsDirty(_urlAclItems) || IsDirty(_sslItems);
        }

        private bool IsDirty(Hashtable items)
        {
            if(items != null)
            {
                foreach(ConfigItem item in items.Values)
                {
                    if(item.Status != ModifiedStatus.Unmodified)
                        return true;
                }
            }

            return false;
        }

        private void AutoSizeColumns(ListView lv)
        {
            foreach(ColumnHeader c in lv.Columns)
                c.Width = -2; // Documented in ColumnHeader.Width property documentation (-1 = widest text, -2 = widest of heading, text)
        }

        private void DisplayError(Exception ex)
        {
            MessageBox.Show(this, ex.ToString(), "An error has occurred");
        }

        private void Load_IpListenItems()
        {
            _ipListenItems = IpListenConfigItem.QueryConfig();

            PopulateConfigListView(_ipListenItems, ipListenersListView);
        }

        private void Load_UrlAclItems()
        {
            _urlAclItems = UrlAclConfigItem.QueryConfig();

            PopulateConfigListView(_urlAclItems, urlAclListView);
        }

        private void addUrlAclButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                using(InputBox input = new InputBox("New URL", "Please enter the URL:"))
                {
                    if(input.ShowDialog(this) == DialogResult.OK)
                    {
                        UrlAclConfigItem newItem = new UrlAclConfigItem();

                        newItem.Url = input.UserInput;

                        bool exists = false;
                        if(_urlAclItems.Contains(newItem.Key))
                        {
                            exists = true;

                            if(((ConfigItem)_urlAclItems[newItem.Key]).Status != ModifiedStatus.Removed)
                            {
                                MessageBox.Show(this, "Security is already configured for the Url you entered.", "Invalid Input");
                                return;
                            }
                        }

                        string originalSddl = newItem.Dacl != null ? newItem.Dacl.ToSddl() : "D:";

                        string editedSddl = SecurityEditor.EditSecurity(Handle, newItem.Url, originalSddl);

                        if(editedSddl != originalSddl)
                        {
                            if(exists)
                            {
                                newItem.Status = ModifiedStatus.Modified;
                                _urlAclItems[newItem.Key] = newItem;
                            }
                            else
                            {
                                newItem.Dacl = Acl.FromSddl(editedSddl);
                                newItem.Status = ModifiedStatus.Added;
                                _urlAclItems.Add(newItem.Key, newItem);
                            }

                            applyButton.Enabled = true;

                            PopulateConfigListView(_urlAclItems, urlAclListView);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                EnableUrlAclButtons();
            }
        }

        private void removeUrlAclButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ArrayList removedItems = new ArrayList();

                foreach(int i in urlAclListView.SelectedIndices)
                {
                    applyButton.Enabled = true;
                    removedItems.Add(((UrlAclConfigItem)urlAclListView.Items[i]).Key);
                }

                foreach(object key in removedItems)
                {
                    UrlAclConfigItem item = (UrlAclConfigItem)_urlAclItems[key];

                    switch(item.Status)
                    {
                        case ModifiedStatus.Added:
                            _urlAclItems.Remove(item.Key);
                            break;

                        case ModifiedStatus.Unmodified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        case ModifiedStatus.Modified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        default:
                            break;
                    }
                }

                PopulateConfigListView(_urlAclItems, urlAclListView);
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                EnableUrlAclButtons();
            }
        }

        private void editUrlAclButton_Click(object sender, EventArgs e)
		{
			try
			{
                EditUrlAcl();
			}
			catch(Exception ex)
			{
				DisplayError(ex);
			}
		}

        private void urlAclListView_DoubleClick(object sender, System.EventArgs e)
        {
            try
            {
                EditUrlAcl();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void urlAclListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                EnableUrlAclButtons();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void EditUrlAcl()
		{
            try
            {
                UrlAclConfigItem selectedItem = (UrlAclConfigItem)urlAclListView.SelectedItems[0];

                string originalSddl = selectedItem.Dacl.ToSddl();

                string editedSddl = SecurityEditor.EditSecurity(Handle, selectedItem.Url, originalSddl);

                if(editedSddl != originalSddl)
                {
                    selectedItem.Dacl = Acl.FromSddl(editedSddl);

                    if(selectedItem.Status == ModifiedStatus.Unmodified)
                        selectedItem.Status = ModifiedStatus.Modified;

                    applyButton.Enabled = true;

                    PopulateConfigListView(_urlAclItems, urlAclListView);
                }
            }
            finally
            {
                EnableUrlAclButtons();
            }
		}

        private void Load_SslItems()
        {
            _sslItems = SslConfigItem.QueryConfig();

            PopulateConfigListView(_sslItems, sslListView);
        }

        private void addSslButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                using(SslConfigForm editor = new SslConfigForm())
                {
                    if(editor.ShowDialog() == DialogResult.OK)
                    {
                        SslConfigItem newItem = editor.UpdatedItem;

                        newItem.Status = ModifiedStatus.Added;

                        if(_sslItems.Contains(newItem.Key))
                        {
                            if(((ConfigItem)_sslItems[newItem.Key]).Status != ModifiedStatus.Removed)
                                MessageBox.Show(this, "SSL is already configured for the address and port you entered.", "Invalid Input");
                            else
                            {
                                newItem.Status = ModifiedStatus.Modified;
                                _sslItems[newItem.Key] = newItem;
                            }
                        }
                        else
                        {
                            _sslItems.Add(newItem.Key, newItem);

                            applyButton.Enabled = true;
                        }

                        PopulateConfigListView(_sslItems, sslListView);
                    }
                }
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                EnableSslButtons();
            }
        }

        private void removeSslButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ArrayList removedItems = new ArrayList();

                foreach(int i in sslListView.SelectedIndices)
                {
                    applyButton.Enabled = true;
                    removedItems.Add(((SslConfigItem)sslListView.Items[i]).Key);
                }

                foreach(object key in removedItems)
                {
                    SslConfigItem item = (SslConfigItem)_sslItems[key];

                    switch(item.Status)
                    {
                        case ModifiedStatus.Added:
                            _sslItems.Remove(item);
                            break;

                        case ModifiedStatus.Unmodified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        case ModifiedStatus.Modified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        default:
                            break;
                    }
                }

                PopulateConfigListView(_sslItems, sslListView);
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                EnableSslButtons();
            }
        }

        private void editSslButton_Click(object sender, EventArgs e)
        {
            try
            {
                EditSsl();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void sslListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                EnableSslButtons();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void sslListView_DoubleClick(object sender, System.EventArgs e)
        {
            try
            {
                EditSsl();
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
        }

        private void EditSsl()
        {
            try
            {
                SslConfigItem selectedItem = (SslConfigItem)sslListView.SelectedItems[0];

                using(SslConfigForm editor = new SslConfigForm(selectedItem))
                {
                    if(editor.ShowDialog() == DialogResult.OK)
                    {
                        _sslItems[selectedItem.Key] = editor.UpdatedItem;

                        applyButton.Enabled = true;

                        PopulateConfigListView(_sslItems, sslListView);
                    }
                }
            }
            finally
            {
                EnableSslButtons();
            }
        }


        private void ipListenersListView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            removeIpListenerButton.Enabled = ipListenersListView.SelectedItems.Count > 0;
        }

        private void addIpListenerButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                using(InputBox input = new InputBox("New IP Listener", "Please enter an IP Address"))
                {
                    if(input.ShowDialog(this) == DialogResult.OK)
                    {
                        IPAddress address = IPAddress.Parse(input.UserInput);

                        IpListenConfigItem newItem = new IpListenConfigItem();

                        newItem.Address = address;

                        newItem.Status = ModifiedStatus.Added;

                        if(_ipListenItems.Contains(newItem.Key))
                            MessageBox.Show(this, "The IP address you entered is already configured.", "Invalid Input");
                        else
                        {
                            _ipListenItems.Add(newItem.Key, newItem);

                            ipListenersListView.Items.Add(newItem);

                            applyButton.Enabled = true;
                        }
                    }
                }
            }
            catch(FormatException)
            {
                MessageBox.Show(this, "The data you entered was not formatted correctly.  Please enter a correctly formatted IP address.", "Invalid Input");
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                removeIpListenerButton.Enabled = ipListenersListView.SelectedItems.Count > 0;
            }
        }

        private void removeIpListenerButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                foreach(IpListenConfigItem item in ipListenersListView.SelectedItems)
                {
                    switch(item.Status)
                    {
                        case ModifiedStatus.Added:
                            _ipListenItems.Remove(item.Key);
                            break;

                        case ModifiedStatus.Unmodified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        case ModifiedStatus.Modified:
                            item.Status = ModifiedStatus.Removed;
                            break;

                        default:
                            break;
                    }
                }

                applyButton.Enabled = true;

                PopulateConfigListView(_ipListenItems, ipListenersListView);
            }
            catch(Exception ex)
            {
                DisplayError(ex);
            }
            finally
            {
                removeIpListenerButton.Enabled = ipListenersListView.SelectedItems.Count > 0;
            }
        }

        private void EnableSslButtons()
        {
            removeSslButton.Enabled = sslListView.SelectedItems.Count > 0;
            editSslButton.Enabled   = sslListView.SelectedItems.Count > 0;
        }

        private void EnableUrlAclButtons()
        {
            removeUrlAclButton.Enabled = urlAclListView.SelectedItems.Count > 0;
            editUrlAclButton.Enabled   = urlAclListView.SelectedItems.Count == 1;
        }
	}
}
