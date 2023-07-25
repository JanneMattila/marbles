namespace BoxClient
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            DrawTimer = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // DrawTimer
            // 
            DrawTimer.Interval = 10;
            DrawTimer.Tick += DrawTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.Disable;
            BackColor = Color.Black;
            CausesValidation = false;
            ClientSize = new Size(998, 697);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 2, 4, 2);
            MinimumSize = new Size(1024, 768);
            Name = "MainForm";
            Text = "Box";
            Activated += MainForm_Activated;
            Deactivate += MainForm_Deactivate;
            Load += MainForm_Load;
            Paint += MainForm_Paint;
            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer DrawTimer;
    }
}