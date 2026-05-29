namespace CyberSecurityChatBotGUI
{
    partial class CyberSecurityChatBot
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtChat = new RichTextBox();
            txtUserInput = new TextBox();
            btnSend = new Button();
            SuspendLayout();
            // 
            // txtChat
            // 
            txtChat.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtChat.BackColor = SystemColors.Menu;
            txtChat.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtChat.Location = new Point(33, 45);
            txtChat.Name = "txtChat";
            txtChat.Size = new Size(1734, 462);
            txtChat.TabIndex = 0;
            txtChat.Text = "";
            // 
            // txtUserInput
            // 
            txtUserInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtUserInput.Location = new Point(33, 538);
            txtUserInput.Name = "txtUserInput";
            txtUserInput.Size = new Size(629, 27);
            txtUserInput.TabIndex = 1;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.Location = new Point(668, 536);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(94, 29);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // CyberSecurityChatBot
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ScrollBar;
            ClientSize = new Size(1817, 750);
            Controls.Add(btnSend);
            Controls.Add(txtUserInput);
            Controls.Add(txtChat);
            Name = "CyberSecurityChatBot";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox txtChat;
        private TextBox txtUserInput;
        private Button btnSend;
    }
}
