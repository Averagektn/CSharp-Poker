namespace Poker.Interface
{
    partial class MainMenuForm
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
            btnCreate = new Button();
            btnHelp = new Button();
            btnCombinations = new Button();
            btnExit = new Button();
            btnConnect = new Button();
            tbName = new TextBox();
            tbIP = new TextBox();
            lblServerIP = new Label();
            lblPlayerName = new Label();
            SuspendLayout();
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.MistyRose;
            btnCreate.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            btnCreate.Location = new Point(175, 68);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(160, 50);
            btnCreate.TabIndex = 0;
            btnCreate.Text = "CREATE ROOM";
            btnCreate.UseVisualStyleBackColor = false;
            btnCreate.Click += Create_Click;
            // 
            // btnHelp
            // 
            btnHelp.BackColor = Color.MistyRose;
            btnHelp.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            btnHelp.Location = new Point(175, 326);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(160, 50);
            btnHelp.TabIndex = 1;
            btnHelp.Text = "HELP";
            btnHelp.UseVisualStyleBackColor = false;
            btnHelp.Click += Help_Click;
            // 
            // btnCombinations
            // 
            btnCombinations.BackColor = Color.MistyRose;
            btnCombinations.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            btnCombinations.Location = new Point(175, 270);
            btnCombinations.Name = "btnCombinations";
            btnCombinations.Size = new Size(160, 50);
            btnCombinations.TabIndex = 2;
            btnCombinations.Text = "COMBINATIONS";
            btnCombinations.UseVisualStyleBackColor = false;
            btnCombinations.Click += Combinations_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.MistyRose;
            btnExit.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            btnExit.Location = new Point(175, 412);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(160, 50);
            btnExit.TabIndex = 3;
            btnExit.Text = "EXIT";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += Exit_Click;
            // 
            // btnConnect
            // 
            btnConnect.BackColor = Color.MistyRose;
            btnConnect.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            btnConnect.Location = new Point(175, 190);
            btnConnect.Name = "btnConnect";
            btnConnect.Size = new Size(160, 50);
            btnConnect.TabIndex = 4;
            btnConnect.Text = "CONNECT";
            btnConnect.UseVisualStyleBackColor = false;
            btnConnect.Click += Connect_Click;
            // 
            // tbName
            // 
            tbName.BackColor = Color.MistyRose;
            tbName.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            tbName.Location = new Point(162, 31);
            tbName.Name = "tbName";
            tbName.Size = new Size(188, 31);
            tbName.TabIndex = 5;
            // 
            // tbIP
            // 
            tbIP.BackColor = Color.MistyRose;
            tbIP.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            tbIP.Location = new Point(162, 158);
            tbIP.Name = "tbIP";
            tbIP.Size = new Size(195, 31);
            tbIP.TabIndex = 6;
            tbIP.Text = "127.0.0.1";
            // 
            // lblServerIP
            // 
            lblServerIP.AutoSize = true;
            lblServerIP.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            lblServerIP.Location = new Point(214, 136);
            lblServerIP.Name = "lblServerIP";
            lblServerIP.Size = new Size(100, 24);
            lblServerIP.TabIndex = 7;
            lblServerIP.Text = "Server IP";
            // 
            // lblPlayerName
            // 
            lblPlayerName.AutoSize = true;
            lblPlayerName.Font = new Font("Humnst777 Blk BT", 14.25F, FontStyle.Italic, GraphicsUnit.Point);
            lblPlayerName.Location = new Point(223, 9);
            lblPlayerName.Name = "lblPlayerName";
            lblPlayerName.Size = new Size(67, 24);
            lblPlayerName.TabIndex = 8;
            lblPlayerName.Text = "Name";
            // 
            // MainMenuForm
            // 
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightGoldenrodYellow;
            ClientSize = new Size(521, 512);
            Controls.Add(lblPlayerName);
            Controls.Add(lblServerIP);
            Controls.Add(tbIP);
            Controls.Add(tbName);
            Controls.Add(btnConnect);
            Controls.Add(btnExit);
            Controls.Add(btnCombinations);
            Controls.Add(btnHelp);
            Controls.Add(btnCreate);
            Name = "MainMenuForm";
            Text = "MainMenu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCreate;
        private Button btnHelp;
        private Button btnCombinations;
        private Button btnExit;
        private Button btnConnect;
        private TextBox tbName;
        private TextBox tbIP;
        private Label lblServerIP;
        private Label lblPlayerName;
    }
}