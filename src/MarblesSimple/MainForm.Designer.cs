namespace MarblesSimple
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.GroupPlay = new System.Windows.Forms.GroupBox();
            this.PlayLocalButton = new System.Windows.Forms.Button();
            this.PlayOnlineButton = new System.Windows.Forms.Button();
            this.GroupGame = new System.Windows.Forms.GroupBox();
            this.GroupOddOrEven = new System.Windows.Forms.GroupBox();
            this.OddButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.EvenButton = new System.Windows.Forms.Button();
            this.GroupSelectMarbles = new System.Windows.Forms.GroupBox();
            this.YouHaveMarblesLeftLabel = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.SelectMarblesNum = new System.Windows.Forms.NumericUpDown();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.GroupPlay.SuspendLayout();
            this.GroupGame.SuspendLayout();
            this.GroupOddOrEven.SuspendLayout();
            this.GroupSelectMarbles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectMarblesNum)).BeginInit();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupPlay
            // 
            this.GroupPlay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupPlay.Controls.Add(this.PlayLocalButton);
            this.GroupPlay.Controls.Add(this.PlayOnlineButton);
            this.GroupPlay.Location = new System.Drawing.Point(17, 17);
            this.GroupPlay.Margin = new System.Windows.Forms.Padding(4);
            this.GroupPlay.Name = "GroupPlay";
            this.GroupPlay.Padding = new System.Windows.Forms.Padding(4);
            this.GroupPlay.Size = new System.Drawing.Size(1349, 229);
            this.GroupPlay.TabIndex = 0;
            this.GroupPlay.TabStop = false;
            this.GroupPlay.Text = "Play";
            // 
            // PlayLocalButton
            // 
            this.PlayLocalButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayLocalButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PlayLocalButton.Location = new System.Drawing.Point(702, 75);
            this.PlayLocalButton.Margin = new System.Windows.Forms.Padding(4);
            this.PlayLocalButton.Name = "PlayLocalButton";
            this.PlayLocalButton.Size = new System.Drawing.Size(573, 101);
            this.PlayLocalButton.TabIndex = 1;
            this.PlayLocalButton.Text = "Play Local";
            this.PlayLocalButton.UseVisualStyleBackColor = true;
            this.PlayLocalButton.Click += new System.EventHandler(this.PlayLocalButton_Click);
            // 
            // PlayOnlineButton
            // 
            this.PlayOnlineButton.Enabled = false;
            this.PlayOnlineButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PlayOnlineButton.Location = new System.Drawing.Point(69, 75);
            this.PlayOnlineButton.Margin = new System.Windows.Forms.Padding(4);
            this.PlayOnlineButton.Name = "PlayOnlineButton";
            this.PlayOnlineButton.Size = new System.Drawing.Size(573, 101);
            this.PlayOnlineButton.TabIndex = 0;
            this.PlayOnlineButton.Text = "Play Online";
            this.PlayOnlineButton.UseVisualStyleBackColor = true;
            // 
            // GroupGame
            // 
            this.GroupGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupGame.Controls.Add(this.GroupOddOrEven);
            this.GroupGame.Controls.Add(this.GroupSelectMarbles);
            this.GroupGame.Location = new System.Drawing.Point(17, 276);
            this.GroupGame.Margin = new System.Windows.Forms.Padding(4);
            this.GroupGame.Name = "GroupGame";
            this.GroupGame.Padding = new System.Windows.Forms.Padding(4);
            this.GroupGame.Size = new System.Drawing.Size(1349, 687);
            this.GroupGame.TabIndex = 1;
            this.GroupGame.TabStop = false;
            this.GroupGame.Text = "Game";
            // 
            // GroupOddOrEven
            // 
            this.GroupOddOrEven.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupOddOrEven.Controls.Add(this.OddButton);
            this.GroupOddOrEven.Controls.Add(this.label1);
            this.GroupOddOrEven.Controls.Add(this.EvenButton);
            this.GroupOddOrEven.Enabled = false;
            this.GroupOddOrEven.Location = new System.Drawing.Point(37, 362);
            this.GroupOddOrEven.Name = "GroupOddOrEven";
            this.GroupOddOrEven.Size = new System.Drawing.Size(1277, 276);
            this.GroupOddOrEven.TabIndex = 4;
            this.GroupOddOrEven.TabStop = false;
            this.GroupOddOrEven.Text = "Odd or even";
            // 
            // OddButton
            // 
            this.OddButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OddButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.OddButton.Location = new System.Drawing.Point(623, 146);
            this.OddButton.Margin = new System.Windows.Forms.Padding(4);
            this.OddButton.Name = "OddButton";
            this.OddButton.Size = new System.Drawing.Size(281, 87);
            this.OddButton.TabIndex = 3;
            this.OddButton.Text = "Odd";
            this.OddButton.UseVisualStyleBackColor = true;
            this.OddButton.Click += new System.EventHandler(this.OddButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(17, 68);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(863, 45);
            this.label1.TabIndex = 2;
            this.label1.Text = "Does your opponent have odd or even number of marbles?";
            // 
            // EvenButton
            // 
            this.EvenButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EvenButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.EvenButton.Location = new System.Drawing.Point(957, 146);
            this.EvenButton.Margin = new System.Windows.Forms.Padding(4);
            this.EvenButton.Name = "EvenButton";
            this.EvenButton.Size = new System.Drawing.Size(281, 87);
            this.EvenButton.TabIndex = 2;
            this.EvenButton.Text = "Even";
            this.EvenButton.UseVisualStyleBackColor = true;
            this.EvenButton.Click += new System.EventHandler(this.EvenButton_Click);
            // 
            // GroupSelectMarbles
            // 
            this.GroupSelectMarbles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupSelectMarbles.Controls.Add(this.YouHaveMarblesLeftLabel);
            this.GroupSelectMarbles.Controls.Add(this.SelectButton);
            this.GroupSelectMarbles.Controls.Add(this.SelectMarblesNum);
            this.GroupSelectMarbles.Enabled = false;
            this.GroupSelectMarbles.Location = new System.Drawing.Point(37, 75);
            this.GroupSelectMarbles.Name = "GroupSelectMarbles";
            this.GroupSelectMarbles.Size = new System.Drawing.Size(1277, 252);
            this.GroupSelectMarbles.TabIndex = 3;
            this.GroupSelectMarbles.TabStop = false;
            this.GroupSelectMarbles.Text = "Choose number of marbles";
            // 
            // YouHaveMarblesLeftLabel
            // 
            this.YouHaveMarblesLeftLabel.AutoSize = true;
            this.YouHaveMarblesLeftLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.YouHaveMarblesLeftLabel.Location = new System.Drawing.Point(17, 68);
            this.YouHaveMarblesLeftLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.YouHaveMarblesLeftLabel.Name = "YouHaveMarblesLeftLabel";
            this.YouHaveMarblesLeftLabel.Size = new System.Drawing.Size(815, 45);
            this.YouHaveMarblesLeftLabel.TabIndex = 2;
            this.YouHaveMarblesLeftLabel.Text = "You have 10 marbles to left. How many will you choose?";
            // 
            // SelectButton
            // 
            this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SelectButton.Location = new System.Drawing.Point(957, 126);
            this.SelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(281, 87);
            this.SelectButton.TabIndex = 2;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // SelectMarblesNum
            // 
            this.SelectMarblesNum.Font = new System.Drawing.Font("Segoe UI", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SelectMarblesNum.Location = new System.Drawing.Point(737, 141);
            this.SelectMarblesNum.Margin = new System.Windows.Forms.Padding(6);
            this.SelectMarblesNum.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SelectMarblesNum.Name = "SelectMarblesNum";
            this.SelectMarblesNum.Size = new System.Drawing.Size(167, 57);
            this.SelectMarblesNum.TabIndex = 0;
            this.SelectMarblesNum.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // StatusStrip
            // 
            this.StatusStrip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StatusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 1035);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(1382, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 12);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(18F, 45F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 1057);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.GroupGame);
            this.Controls.Add(this.GroupPlay);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1408, 1051);
            this.Name = "MainForm";
            this.Text = "Marbles";
            this.GroupPlay.ResumeLayout(false);
            this.GroupGame.ResumeLayout(false);
            this.GroupOddOrEven.ResumeLayout(false);
            this.GroupOddOrEven.PerformLayout();
            this.GroupSelectMarbles.ResumeLayout(false);
            this.GroupSelectMarbles.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SelectMarblesNum)).EndInit();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GroupBox GroupPlay;
        private Button PlayLocalButton;
        private Button PlayOnlineButton;
        private GroupBox GroupGame;
        private GroupBox GroupOddOrEven;
        private Button OddButton;
        private Label label1;
        private Button EvenButton;
        private GroupBox GroupSelectMarbles;
        private Label YouHaveMarblesLeftLabel;
        private Button SelectButton;
        private NumericUpDown SelectMarblesNum;
        private StatusStrip StatusStrip;
        private ToolStripStatusLabel StatusLabel;
    }
}