namespace Poker.Interface
{
    partial class HelpForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpForm));
            tbHelp = new TextBox();
            SuspendLayout();
            // 
            // tbHelp
            // 
            tbHelp.BackColor = Color.Lavender;
            tbHelp.Font = new Font("News706 BT", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            tbHelp.Location = new Point(-2, -4);
            tbHelp.Multiline = true;
            tbHelp.Name = "tbHelp";
            tbHelp.ScrollBars = ScrollBars.Vertical;
            tbHelp.Size = new Size(805, 452);
            tbHelp.TabIndex = 0;
            tbHelp.Text = resources.GetString("tbHelp.Text");
            // 
            // HelpForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tbHelp);
            Name = "HelpForm";
            Text = "Help";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tbHelp;
    }
}