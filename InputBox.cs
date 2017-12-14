using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace HttpConfig
{
	public class InputBox : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox userInputTextBox;
        private System.Windows.Forms.Label promptLabel;
		private System.ComponentModel.Container components = null;

		public InputBox()
		{
			InitializeComponent();
		}

        public InputBox(string caption, string prompt) : this()
        {
            Text = caption;

            promptLabel.Text = prompt;
        }

        public string Caption
        {
            get { return Text;  }
            set { Text = value; }
        }

        public string Prompt
        {
            get { return promptLabel.Text;  }
            set { promptLabel.Text = value; }
        }

        public string UserInput
        {
            get { return userInputTextBox.Text;  }
            set { userInputTextBox.Text = value; }
        }

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.userInputTextBox = new System.Windows.Forms.TextBox();
            this.promptLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(56, 64);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            // 
            // cancelButton
            // 
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(136, 64);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            // 
            // userInputTextBox
            // 
            this.userInputTextBox.Location = new System.Drawing.Point(8, 32);
            this.userInputTextBox.Name = "userInputTextBox";
            this.userInputTextBox.Size = new System.Drawing.Size(248, 20);
            this.userInputTextBox.TabIndex = 2;
            this.userInputTextBox.Text = "";
            // 
            // promptLabel
            // 
            this.promptLabel.Location = new System.Drawing.Point(8, 8);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(240, 16);
            this.promptLabel.TabIndex = 3;
            this.promptLabel.Text = "Prompt";
            // 
            // InputBox
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(266, 104);
            this.ControlBox = false;
            this.Controls.Add(this.promptLabel);
            this.Controls.Add(this.userInputTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "InputBox";
            this.Activated += new System.EventHandler(this.InputBox_Activated);
            this.ResumeLayout(false);

        }
		#endregion

        private void InputBox_Activated(object sender, System.EventArgs e)
        {
            userInputTextBox.Focus();
        }
	}
}
