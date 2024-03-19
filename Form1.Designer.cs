namespace WinFormsApp1
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            PlayerConnecttoServer = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            Rooms = new Button();
            SuspendLayout();
            // 
            // PlayerConnecttoServer
            // 
            PlayerConnecttoServer.BackColor = Color.Purple;
            resources.ApplyResources(PlayerConnecttoServer, "PlayerConnecttoServer");
            PlayerConnecttoServer.Name = "PlayerConnecttoServer";
            PlayerConnecttoServer.UseVisualStyleBackColor = false;
            PlayerConnecttoServer.Click += PlayerConnecttoServer_Click;
            // 
            // textBox1
            // 
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.BackColor = Color.Transparent;
            label1.ForeColor = SystemColors.ControlLight;
            label1.Name = "label1";
            // 
            // Rooms
            // 
            Rooms.BackColor = Color.ForestGreen;
            resources.ApplyResources(Rooms, "Rooms");
            Rooms.Name = "Rooms";
            Rooms.UseVisualStyleBackColor = false;
            Rooms.Click += Rooms_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Rooms);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(PlayerConnecttoServer);
            MaximizeBox = false;
            Name = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button PlayerConnecttoServer;
        private PictureBox pictureBox1;
        private TextBox textBox1;
        private Label label1;
        private Button Rooms;
    }
}
