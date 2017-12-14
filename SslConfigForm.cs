using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HttpConfig
{
	public class SslConfigForm : System.Windows.Forms.Form
	{
        private byte[]         _certHashBytes = null;
        private ModifiedStatus _status        = ModifiedStatus.Added;

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox certHashTextBox;
        private System.Windows.Forms.TextBox certNameTextBox;
        private System.Windows.Forms.TextBox certStoreTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox serverCertGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox addressTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button viewCertButton;
        private System.Windows.Forms.Button certBrowseButton;
		private GroupBox groupBox2;
		private TextBox ctlStoreTextBox;
		private Label label11;
		private TextBox ctlIdTextBox;
		private Label label8;
		private TextBox retrievalTimeoutTextBox;
		private Label label7;
		private TextBox refreshTimeTextBox;
		private Label label6;
		private CheckBox noRouteCheckBox;
		private CheckBox clientCertCheckBox;
		private CheckBox dsMapperCheckBox;
		private TextBox guidTextBox;
		private Label label9;
		private Button newGuidButton;
		private CheckBox noUsageCheckCheckBox;
		private CheckBox useFreshnessTimeCheckBox;
		private CheckBox onlyCachedRevocationCheckBox;
		private CheckBox noRevocationCheckBox;
		private System.ComponentModel.Container components = null;

		public SslConfigForm()
		{
			InitializeComponent();
		}

        public SslConfigForm(SslConfigItem item) : this()
        {
            addressTextBox.Enabled = false; // we're editing an existing item and address:port is the key in the collection

            portTextBox.Enabled = false; // we're editing an existing item and address:port is the key in the collection

            _status = item.Status == ModifiedStatus.Added ? ModifiedStatus.Added : ModifiedStatus.Modified;

            ItemToControls(item);            
        }

        public SslConfigItem UpdatedItem
        {
            get { return ControlsToItem(); }
        }

        private void ItemToControls(SslConfigItem item)
        {
			SetGuidText(item.AppId);

            addressTextBox.Text           = item.Address.ToString();
            portTextBox.Text              = item.Port.ToString();
			refreshTimeTextBox.Text      = item.RevocationFreshnessTime.ToString();
			retrievalTimeoutTextBox.Text = item.RevocationUrlRetrievalTimeout.ToString();
			ctlIdTextBox.Text             = item.SslCtlIdentifier;
			ctlStoreTextBox.Text          = item.SslCtlStoreName;

			noRevocationCheckBox.Checked =
				(item.CertCheckMode & HttpApi.ClientCertCheckMode.NoVerifyRevocation) != 0;

			onlyCachedRevocationCheckBox.Checked = 
				(item.CertCheckMode & HttpApi.ClientCertCheckMode.CachedRevocationOnly) != 0;

			useFreshnessTimeCheckBox.Checked =
				(item.CertCheckMode & HttpApi.ClientCertCheckMode.UseRevocationFreshnessTime) != 0;

			noUsageCheckCheckBox.Checked =
				(item.CertCheckMode & HttpApi.ClientCertCheckMode.NoUsageCheck) != 0;

			dsMapperCheckBox.Checked   = (item.Flags & HttpApi.SslConfigFlag.UseDSMapper) != 0;
			clientCertCheckBox.Checked = (item.Flags & HttpApi.SslConfigFlag.NegotiateClientCertificates) != 0;
			noRouteCheckBox.Checked    = (item.Flags & HttpApi.SslConfigFlag.DoNotRouteToRawIsapiFilters) != 0;

            string storeName = (item.CertStoreName == null) ? "MY" : item.CertStoreName;

            if((item.Hash != null) && (item.Hash.Length > 0))
            {
                certStoreTextBox.Text = storeName;
                certHashTextBox.Text  = CertUtil.BytesToHex(item.Hash);
                certNameTextBox.Text  = CertUtil.GetCertNameFromStoreAndHash(storeName, item.Hash);

                _certHashBytes = item.Hash;
            }
        }

        private SslConfigItem ControlsToItem()
        {
            SslConfigItem newItem = new SslConfigItem();

            newItem.Status        = _status;
            newItem.Address       = IPAddress.Parse(addressTextBox.Text);            
            newItem.Port          = ushort.Parse(portTextBox.Text);
            newItem.CertStoreName = certStoreTextBox.Text;
            newItem.Hash          = _certHashBytes;
			newItem.AppId         = guidTextBox.Text.Length > 0 ? new Guid(guidTextBox.Text) : Guid.Empty;

			newItem.RevocationFreshnessTime       = string.IsNullOrEmpty(refreshTimeTextBox.Text) ? 0 : int.Parse(refreshTimeTextBox.Text);
			newItem.RevocationUrlRetrievalTimeout = string.IsNullOrEmpty(retrievalTimeoutTextBox.Text) ? 0 : int.Parse(retrievalTimeoutTextBox.Text);
			newItem.SslCtlIdentifier              = ctlIdTextBox.Text;
			newItem.SslCtlStoreName               = ctlStoreTextBox.Text;

			newItem.CertCheckMode = 0;

			if(noRevocationCheckBox.Checked)
				newItem.CertCheckMode |= HttpApi.ClientCertCheckMode.NoVerifyRevocation;

			if(onlyCachedRevocationCheckBox.Checked)
				newItem.CertCheckMode |= HttpApi.ClientCertCheckMode.CachedRevocationOnly;

			if(useFreshnessTimeCheckBox.Checked)
				newItem.CertCheckMode |= HttpApi.ClientCertCheckMode.UseRevocationFreshnessTime;

			if(noUsageCheckCheckBox.Checked)
				newItem.CertCheckMode |= HttpApi.ClientCertCheckMode.NoUsageCheck;
			
			newItem.Flags = 0;

			if(dsMapperCheckBox.Checked)
				newItem.Flags |= HttpApi.SslConfigFlag.UseDSMapper;

			if(clientCertCheckBox.Checked)
				newItem.Flags |= HttpApi.SslConfigFlag.NegotiateClientCertificates;

			if(noRouteCheckBox.Checked)
				newItem.Flags |= HttpApi.SslConfigFlag.DoNotRouteToRawIsapiFilters;

            return newItem;
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.serverCertGroupBox = new System.Windows.Forms.GroupBox();
			this.viewCertButton = new System.Windows.Forms.Button();
			this.certBrowseButton = new System.Windows.Forms.Button();
			this.certHashTextBox = new System.Windows.Forms.TextBox();
			this.certNameTextBox = new System.Windows.Forms.TextBox();
			this.certStoreTextBox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.newGuidButton = new System.Windows.Forms.Button();
			this.guidTextBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.addressTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.noUsageCheckCheckBox = new System.Windows.Forms.CheckBox();
			this.useFreshnessTimeCheckBox = new System.Windows.Forms.CheckBox();
			this.onlyCachedRevocationCheckBox = new System.Windows.Forms.CheckBox();
			this.noRevocationCheckBox = new System.Windows.Forms.CheckBox();
			this.noRouteCheckBox = new System.Windows.Forms.CheckBox();
			this.clientCertCheckBox = new System.Windows.Forms.CheckBox();
			this.dsMapperCheckBox = new System.Windows.Forms.CheckBox();
			this.ctlStoreTextBox = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.ctlIdTextBox = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.retrievalTimeoutTextBox = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.refreshTimeTextBox = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.serverCertGroupBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.CausesValidation = false;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(368, 36);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 24);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(368, 8);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 24);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "&OK";
			// 
			// serverCertGroupBox
			// 
			this.serverCertGroupBox.Controls.Add(this.viewCertButton);
			this.serverCertGroupBox.Controls.Add(this.certBrowseButton);
			this.serverCertGroupBox.Controls.Add(this.certHashTextBox);
			this.serverCertGroupBox.Controls.Add(this.certNameTextBox);
			this.serverCertGroupBox.Controls.Add(this.certStoreTextBox);
			this.serverCertGroupBox.Controls.Add(this.label10);
			this.serverCertGroupBox.Controls.Add(this.label4);
			this.serverCertGroupBox.Controls.Add(this.label3);
			this.serverCertGroupBox.Location = new System.Drawing.Point(8, 99);
			this.serverCertGroupBox.Name = "serverCertGroupBox";
			this.serverCertGroupBox.Size = new System.Drawing.Size(347, 104);
			this.serverCertGroupBox.TabIndex = 1;
			this.serverCertGroupBox.TabStop = false;
			this.serverCertGroupBox.Text = "Certificate";
			// 
			// viewCertButton
			// 
			this.viewCertButton.Enabled = false;
			this.viewCertButton.Location = new System.Drawing.Point(256, 56);
			this.viewCertButton.Name = "viewCertButton";
			this.viewCertButton.Size = new System.Drawing.Size(75, 24);
			this.viewCertButton.TabIndex = 4;
			this.viewCertButton.Text = "&View";
			this.viewCertButton.Click += new System.EventHandler(this.viewCertButton_Click);
			// 
			// certBrowseButton
			// 
			this.certBrowseButton.Location = new System.Drawing.Point(256, 32);
			this.certBrowseButton.Name = "certBrowseButton";
			this.certBrowseButton.Size = new System.Drawing.Size(75, 24);
			this.certBrowseButton.TabIndex = 3;
			this.certBrowseButton.Text = "&Browse";
			this.certBrowseButton.Click += new System.EventHandler(this.certBrowseButton_Click);
			// 
			// certHashTextBox
			// 
			this.certHashTextBox.Enabled = false;
			this.certHashTextBox.Location = new System.Drawing.Point(72, 72);
			this.certHashTextBox.Name = "certHashTextBox";
			this.certHashTextBox.Size = new System.Drawing.Size(176, 20);
			this.certHashTextBox.TabIndex = 2;
			this.certHashTextBox.TextChanged += new System.EventHandler(this.certHashTextBox_TextChanged);
			// 
			// certNameTextBox
			// 
			this.certNameTextBox.Enabled = false;
			this.certNameTextBox.Location = new System.Drawing.Point(72, 48);
			this.certNameTextBox.Name = "certNameTextBox";
			this.certNameTextBox.Size = new System.Drawing.Size(176, 20);
			this.certNameTextBox.TabIndex = 1;
			// 
			// certStoreTextBox
			// 
			this.certStoreTextBox.Enabled = false;
			this.certStoreTextBox.Location = new System.Drawing.Point(72, 24);
			this.certStoreTextBox.Name = "certStoreTextBox";
			this.certStoreTextBox.Size = new System.Drawing.Size(176, 20);
			this.certStoreTextBox.TabIndex = 0;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 72);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(64, 20);
			this.label10.TabIndex = 36;
			this.label10.Text = "Cert hash:";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 20);
			this.label4.TabIndex = 35;
			this.label4.Text = "Cert name:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 20);
			this.label3.TabIndex = 34;
			this.label3.Text = "Cert store:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.newGuidButton);
			this.groupBox1.Controls.Add(this.guidTextBox);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.portTextBox);
			this.groupBox1.Controls.Add(this.addressTextBox);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(347, 82);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Application";
			// 
			// newGuidButton
			// 
			this.newGuidButton.Location = new System.Drawing.Point(302, 49);
			this.newGuidButton.Name = "newGuidButton";
			this.newGuidButton.Size = new System.Drawing.Size(38, 22);
			this.newGuidButton.TabIndex = 40;
			this.newGuidButton.Text = "&New";
			this.newGuidButton.UseVisualStyleBackColor = true;
			this.newGuidButton.Click += new System.EventHandler(this.newGuidButton_Click);
			// 
			// guidTextBox
			// 
			this.guidTextBox.Location = new System.Drawing.Point(69, 50);
			this.guidTextBox.Name = "guidTextBox";
			this.guidTextBox.Size = new System.Drawing.Size(233, 20);
			this.guidTextBox.TabIndex = 38;
			this.guidTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.guidTextBox_Validating);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(5, 50);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(64, 20);
			this.label9.TabIndex = 39;
			this.label9.Text = "GUID:";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// portTextBox
			// 
			this.portTextBox.Location = new System.Drawing.Point(262, 24);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(40, 20);
			this.portTextBox.TabIndex = 1;
			this.portTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.intTextBox_Validating);
			this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
			// 
			// addressTextBox
			// 
			this.addressTextBox.Location = new System.Drawing.Point(69, 24);
			this.addressTextBox.Name = "addressTextBox";
			this.addressTextBox.Size = new System.Drawing.Size(150, 20);
			this.addressTextBox.TabIndex = 0;
			this.addressTextBox.TextChanged += new System.EventHandler(this.addressTextBox_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(230, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 20);
			this.label2.TabIndex = 37;
			this.label2.Text = "Port:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "IP Address:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.noUsageCheckCheckBox);
			this.groupBox2.Controls.Add(this.useFreshnessTimeCheckBox);
			this.groupBox2.Controls.Add(this.onlyCachedRevocationCheckBox);
			this.groupBox2.Controls.Add(this.noRevocationCheckBox);
			this.groupBox2.Controls.Add(this.noRouteCheckBox);
			this.groupBox2.Controls.Add(this.clientCertCheckBox);
			this.groupBox2.Controls.Add(this.dsMapperCheckBox);
			this.groupBox2.Controls.Add(this.ctlStoreTextBox);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.ctlIdTextBox);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.retrievalTimeoutTextBox);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.refreshTimeTextBox);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Location = new System.Drawing.Point(8, 214);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(347, 268);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Client Certificates";
			// 
			// noUsageCheckCheckBox
			// 
			this.noUsageCheckCheckBox.AutoSize = true;
			this.noUsageCheckCheckBox.Location = new System.Drawing.Point(33, 184);
			this.noUsageCheckCheckBox.Name = "noUsageCheckCheckBox";
			this.noUsageCheckCheckBox.Size = new System.Drawing.Size(154, 17);
			this.noUsageCheckCheckBox.TabIndex = 51;
			this.noUsageCheckCheckBox.Text = "No client cert usage check";
			this.noUsageCheckCheckBox.UseVisualStyleBackColor = true;
			// 
			// useFreshnessTimeCheckBox
			// 
			this.useFreshnessTimeCheckBox.AutoSize = true;
			this.useFreshnessTimeCheckBox.Location = new System.Drawing.Point(33, 165);
			this.useFreshnessTimeCheckBox.Name = "useFreshnessTimeCheckBox";
			this.useFreshnessTimeCheckBox.Size = new System.Drawing.Size(202, 17);
			this.useFreshnessTimeCheckBox.TabIndex = 50;
			this.useFreshnessTimeCheckBox.Text = "Use revocation freshness time setting";
			this.useFreshnessTimeCheckBox.UseVisualStyleBackColor = true;
			// 
			// onlyCachedRevocationCheckBox
			// 
			this.onlyCachedRevocationCheckBox.AutoSize = true;
			this.onlyCachedRevocationCheckBox.Location = new System.Drawing.Point(33, 146);
			this.onlyCachedRevocationCheckBox.Name = "onlyCachedRevocationCheckBox";
			this.onlyCachedRevocationCheckBox.Size = new System.Drawing.Size(208, 17);
			this.onlyCachedRevocationCheckBox.TabIndex = 49;
			this.onlyCachedRevocationCheckBox.Text = "Use only cached client cert revocation";
			this.onlyCachedRevocationCheckBox.UseVisualStyleBackColor = true;
			// 
			// noRevocationCheckBox
			// 
			this.noRevocationCheckBox.AutoSize = true;
			this.noRevocationCheckBox.Location = new System.Drawing.Point(33, 127);
			this.noRevocationCheckBox.Name = "noRevocationCheckBox";
			this.noRevocationCheckBox.Size = new System.Drawing.Size(196, 17);
			this.noRevocationCheckBox.TabIndex = 48;
			this.noRevocationCheckBox.Text = "Client cert not verified for revocation";
			this.noRevocationCheckBox.UseVisualStyleBackColor = true;
			// 
			// noRouteCheckBox
			// 
			this.noRouteCheckBox.AutoSize = true;
			this.noRouteCheckBox.Location = new System.Drawing.Point(33, 241);
			this.noRouteCheckBox.Name = "noRouteCheckBox";
			this.noRouteCheckBox.Size = new System.Drawing.Size(174, 17);
			this.noRouteCheckBox.TabIndex = 47;
			this.noRouteCheckBox.Text = "Do not route to raw ISAPI filters";
			this.noRouteCheckBox.UseVisualStyleBackColor = true;
			// 
			// clientCertCheckBox
			// 
			this.clientCertCheckBox.AutoSize = true;
			this.clientCertCheckBox.Location = new System.Drawing.Point(33, 222);
			this.clientCertCheckBox.Name = "clientCertCheckBox";
			this.clientCertCheckBox.Size = new System.Drawing.Size(154, 17);
			this.clientCertCheckBox.TabIndex = 46;
			this.clientCertCheckBox.Text = "Negotiate client certificates";
			this.clientCertCheckBox.UseVisualStyleBackColor = true;
			// 
			// dsMapperCheckBox
			// 
			this.dsMapperCheckBox.AutoSize = true;
			this.dsMapperCheckBox.Location = new System.Drawing.Point(33, 203);
			this.dsMapperCheckBox.Name = "dsMapperCheckBox";
			this.dsMapperCheckBox.Size = new System.Drawing.Size(101, 17);
			this.dsMapperCheckBox.TabIndex = 45;
			this.dsMapperCheckBox.Text = "Use DS mapper";
			this.dsMapperCheckBox.UseVisualStyleBackColor = true;
			// 
			// ctlStoreTextBox
			// 
			this.ctlStoreTextBox.Location = new System.Drawing.Point(149, 96);
			this.ctlStoreTextBox.Name = "ctlStoreTextBox";
			this.ctlStoreTextBox.Size = new System.Drawing.Size(171, 20);
			this.ctlStoreTextBox.TabIndex = 43;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 96);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(143, 20);
			this.label11.TabIndex = 44;
			this.label11.Text = "SSL CTL store name:";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// ctlIdTextBox
			// 
			this.ctlIdTextBox.Location = new System.Drawing.Point(149, 70);
			this.ctlIdTextBox.Name = "ctlIdTextBox";
			this.ctlIdTextBox.Size = new System.Drawing.Size(171, 20);
			this.ctlIdTextBox.TabIndex = 41;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 70);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(143, 20);
			this.label8.TabIndex = 42;
			this.label8.Text = "SSL CTL identifier:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// retrievalTimeoutTextBox
			// 
			this.retrievalTimeoutTextBox.Location = new System.Drawing.Point(149, 44);
			this.retrievalTimeoutTextBox.Name = "retrievalTimeoutTextBox";
			this.retrievalTimeoutTextBox.Size = new System.Drawing.Size(171, 20);
			this.retrievalTimeoutTextBox.TabIndex = 39;
			this.retrievalTimeoutTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.intTextBox_Validating);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 44);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(143, 20);
			this.label7.TabIndex = 40;
			this.label7.Text = "URL retrieval timeout:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// refreshTimeTextBox
			// 
			this.refreshTimeTextBox.Location = new System.Drawing.Point(149, 18);
			this.refreshTimeTextBox.Name = "refreshTimeTextBox";
			this.refreshTimeTextBox.Size = new System.Drawing.Size(171, 20);
			this.refreshTimeTextBox.TabIndex = 37;
			this.refreshTimeTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.intTextBox_Validating);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 18);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(143, 20);
			this.label6.TabIndex = 38;
			this.label6.Text = "Revocation freshness time:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// SslConfigForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(450, 497);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.serverCertGroupBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SslConfigForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SSL Configuration";
			this.serverCertGroupBox.ResumeLayout(false);
			this.serverCertGroupBox.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

        }
		#endregion

        private void certBrowseButton_Click(object sender, System.EventArgs e)
        {
            IntPtr pCert = IntPtr.Zero;
			IntPtr pCsc  = IntPtr.Zero;

            IntPtr[] stores = new IntPtr[2];

            IntPtr pStores = Marshal.AllocHGlobal(2 * Marshal.SizeOf(typeof(IntPtr)));

            try
            {
                stores[0] = CertUtil.CertOpenStore(CertUtil.CERT_STORE_PROV_SYSTEM_A, 0, 0, (int)CertUtil.CertStoreLocation.LocalMachine, "MY");
                if(stores[0] == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception("CertOpenStore failed.  Error = " + error.ToString());
                }

                stores[1] = CertUtil.CertOpenStore(CertUtil.CERT_STORE_PROV_SYSTEM_A, 0, 0, (int)CertUtil.CertStoreLocation.LocalMachine, "TRUST");
                if(stores[1] == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new Exception("CertOpenStore failed.  Error = " + error.ToString());
                }

                Marshal.WriteIntPtr(pStores, 0, stores[0]);

                Marshal.WriteIntPtr(pStores, Marshal.SizeOf(typeof(IntPtr)), stores[1]);

                CertUtil.CRYPTUI_SELECTCERTIFICATE_STRUCT csc = new CertUtil.CRYPTUI_SELECTCERTIFICATE_STRUCT();

                csc.dwSize = (uint)Marshal.SizeOf(typeof(CertUtil.CRYPTUI_SELECTCERTIFICATE_STRUCT));

                csc.hwndParent       = this.Handle;
                csc.cDisplayStores   = 2;
                csc.rghDisplayStores = pStores;

				pCsc = Marshal.AllocHGlobal((int)(csc.dwSize));

				Marshal.StructureToPtr(csc, pCsc, false);

                pCert = CertUtil.CryptUIDlgSelectCertificate(pCsc);

                if(pCert != IntPtr.Zero)
                {
                    CertUtil.CERT_CONTEXT context = (CertUtil.CERT_CONTEXT)Marshal.PtrToStructure(pCert, typeof(CertUtil.CERT_CONTEXT));

                    certStoreTextBox.Text = context.hCertStore == stores[0] ? "MY" : "TRUST";

                    certNameTextBox.Text = CertUtil.GetCertNameAttribute(pCert, CertUtil.CertNameType.CERT_NAME_FRIENDLY_DISPLAY_TYPE);

                    _certHashBytes = CertUtil.GetCertHash(pCert);

                    certHashTextBox.Text = CertUtil.BytesToHex(_certHashBytes);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "An error has occurred");
            }
            finally
            {
				if(pCsc != IntPtr.Zero)
					Marshal.FreeHGlobal(pCsc);

                if(pCert != IntPtr.Zero)
                    CertUtil.CertFreeCertificateContext(pCert);

                foreach(IntPtr store in stores)
                {
                    if(store != IntPtr.Zero)
                        CertUtil.CertCloseStore(store, 0);
                }

                Marshal.FreeHGlobal(pStores);
            }
        }

        private void addressTextBox_TextChanged(object sender, System.EventArgs e)
        {
            CheckOkEnabled();
        }

        private void portTextBox_TextChanged(object sender, System.EventArgs e)
        {
            CheckOkEnabled();
        }

        private void CheckOkEnabled()
        {
            okButton.Enabled = (addressTextBox.Text.Length > 0) &&
                               (portTextBox.Text.Length > 0)    &&
                               ((certHashTextBox.Text.Length > 0) && (!certHashTextBox.Text.Equals(CertUtil.UNKNOWN_CERT_NAME)));
        }

		private void portTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				ushort.Parse(((TextBox)sender).Text);
			}
			catch(FormatException)
			{
				e.Cancel = true;
				MessageBox.Show(this, "Value must be an integer between 1 and 65535.", "Invalid Input");
			}
		}

        private void intTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
				if(!string.IsNullOrEmpty(((TextBox)sender).Text))
	                int.Parse(((TextBox)sender).Text);
            }
            catch(FormatException)
            {
                e.Cancel = true;
                MessageBox.Show(this, "Value must be an integer.", "Invalid Input");
            }
        }

        private void viewCertButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                CertUtil.ViewCertificate(certStoreTextBox.Text, _certHashBytes, this.Handle);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "The certificate cannot be viewed due to an error.\r\n\r\n" + ex.ToString(), "An error has occurred");
            }
        }

        private void certHashTextBox_TextChanged(object sender, System.EventArgs e)
        {
            viewCertButton.Enabled = certHashTextBox.Text.Length > 0;
            CheckOkEnabled();
        }

		private void newGuidButton_Click(object sender, EventArgs e)
		{
			try
			{
				SetGuidText(Guid.NewGuid());
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "Error");
			}
		}

		private void SetGuidText(Guid g)
		{
			guidTextBox.Text = g.ToString("B").ToUpper();
		}

		private void guidTextBox_Validating(object sender, CancelEventArgs e)
		{
			try
			{
				Guid g;

				if(!string.IsNullOrEmpty(guidTextBox.Text))
					g = new Guid(guidTextBox.Text);
			}
			catch(FormatException ex)
			{
				e.Cancel = true;
				MessageBox.Show(this, ex.Message, "Invalid Input");
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.ToString(), "Error");
			}
		}
	}
}
