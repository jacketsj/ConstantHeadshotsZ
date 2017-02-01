namespace LevelEditor
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.levelPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLevel = new System.Windows.Forms.TabPage();
            this.panelSolids = new System.Windows.Forms.Panel();
            this.comboBoxClickMode = new System.Windows.Forms.ComboBox();
            this.labelClickMode = new System.Windows.Forms.Label();
            this.labelSolidTexture = new System.Windows.Forms.Label();
            this.comboBoxSolidTexture = new System.Windows.Forms.ComboBox();
            this.tabPageProperties = new System.Windows.Forms.TabPage();
            this.textBoxTimer = new System.Windows.Forms.TextBox();
            this.labelTimer = new System.Windows.Forms.Label();
            this.textBoxZombieMax = new System.Windows.Forms.TextBox();
            this.labelZombieMax = new System.Windows.Forms.Label();
            this.comboBoxSpawnAcceleration = new System.Windows.Forms.ComboBox();
            this.labelSpawnAcceleration = new System.Windows.Forms.Label();
            this.comboBoxBackground = new System.Windows.Forms.ComboBox();
            this.labelBackGround = new System.Windows.Forms.Label();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.labelHeight = new System.Windows.Forms.Label();
            this.textBoxWidth = new System.Windows.Forms.TextBox();
            this.labelWidth = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.texturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPageLevel.SuspendLayout();
            this.panelSolids.SuspendLayout();
            this.tabPageProperties.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // levelPanel
            // 
            this.levelPanel.AutoScroll = true;
            this.levelPanel.BackColor = System.Drawing.Color.Gray;
            this.levelPanel.Location = new System.Drawing.Point(6, 6);
            this.levelPanel.Name = "levelPanel";
            this.levelPanel.Size = new System.Drawing.Size(822, 578);
            this.levelPanel.TabIndex = 0;
            this.levelPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.levelPanel_Paint);
            this.levelPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.levelPanel_MouseDown);
            this.levelPanel.MouseLeave += new System.EventHandler(this.levelPanel_MouseLeave);
            this.levelPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.levelPanel_MouseMove);
            this.levelPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.levelPanel_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLevel);
            this.tabControl1.Controls.Add(this.tabPageProperties);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1045, 616);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageLevel
            // 
            this.tabPageLevel.BackColor = System.Drawing.Color.Black;
            this.tabPageLevel.Controls.Add(this.panelSolids);
            this.tabPageLevel.Controls.Add(this.levelPanel);
            this.tabPageLevel.Location = new System.Drawing.Point(4, 22);
            this.tabPageLevel.Name = "tabPageLevel";
            this.tabPageLevel.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLevel.Size = new System.Drawing.Size(1037, 590);
            this.tabPageLevel.TabIndex = 0;
            this.tabPageLevel.Text = "Level";
            // 
            // panelSolids
            // 
            this.panelSolids.BackColor = System.Drawing.Color.Gray;
            this.panelSolids.Controls.Add(this.comboBoxClickMode);
            this.panelSolids.Controls.Add(this.labelClickMode);
            this.panelSolids.Controls.Add(this.labelSolidTexture);
            this.panelSolids.Controls.Add(this.comboBoxSolidTexture);
            this.panelSolids.Location = new System.Drawing.Point(834, 6);
            this.panelSolids.Name = "panelSolids";
            this.panelSolids.Size = new System.Drawing.Size(197, 578);
            this.panelSolids.TabIndex = 3;
            // 
            // comboBoxClickMode
            // 
            this.comboBoxClickMode.FormattingEnabled = true;
            this.comboBoxClickMode.Items.AddRange(new object[] {
            "Place",
            "Delete",
            "Player Spawn",
            "Zombie Spawn",
            "Pointer"});
            this.comboBoxClickMode.Location = new System.Drawing.Point(16, 117);
            this.comboBoxClickMode.Name = "comboBoxClickMode";
            this.comboBoxClickMode.Size = new System.Drawing.Size(159, 21);
            this.comboBoxClickMode.TabIndex = 4;
            // 
            // labelClickMode
            // 
            this.labelClickMode.AutoSize = true;
            this.labelClickMode.Location = new System.Drawing.Point(13, 101);
            this.labelClickMode.Name = "labelClickMode";
            this.labelClickMode.Size = new System.Drawing.Size(60, 13);
            this.labelClickMode.TabIndex = 3;
            this.labelClickMode.Text = "Click Mode";
            // 
            // labelSolidTexture
            // 
            this.labelSolidTexture.AutoSize = true;
            this.labelSolidTexture.Location = new System.Drawing.Point(13, 10);
            this.labelSolidTexture.Name = "labelSolidTexture";
            this.labelSolidTexture.Size = new System.Drawing.Size(69, 13);
            this.labelSolidTexture.TabIndex = 2;
            this.labelSolidTexture.Text = "Solid Texture";
            // 
            // comboBoxSolidTexture
            // 
            this.comboBoxSolidTexture.FormattingEnabled = true;
            this.comboBoxSolidTexture.Location = new System.Drawing.Point(14, 26);
            this.comboBoxSolidTexture.Name = "comboBoxSolidTexture";
            this.comboBoxSolidTexture.Size = new System.Drawing.Size(161, 21);
            this.comboBoxSolidTexture.TabIndex = 1;
            this.comboBoxSolidTexture.SelectedIndexChanged += new System.EventHandler(this.comboBoxSolidTexture_SelectedIndexChanged);
            // 
            // tabPageProperties
            // 
            this.tabPageProperties.Controls.Add(this.textBoxTimer);
            this.tabPageProperties.Controls.Add(this.labelTimer);
            this.tabPageProperties.Controls.Add(this.textBoxZombieMax);
            this.tabPageProperties.Controls.Add(this.labelZombieMax);
            this.tabPageProperties.Controls.Add(this.comboBoxSpawnAcceleration);
            this.tabPageProperties.Controls.Add(this.labelSpawnAcceleration);
            this.tabPageProperties.Controls.Add(this.comboBoxBackground);
            this.tabPageProperties.Controls.Add(this.labelBackGround);
            this.tabPageProperties.Controls.Add(this.textBoxHeight);
            this.tabPageProperties.Controls.Add(this.labelHeight);
            this.tabPageProperties.Controls.Add(this.textBoxWidth);
            this.tabPageProperties.Controls.Add(this.labelWidth);
            this.tabPageProperties.Location = new System.Drawing.Point(4, 22);
            this.tabPageProperties.Name = "tabPageProperties";
            this.tabPageProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProperties.Size = new System.Drawing.Size(1037, 590);
            this.tabPageProperties.TabIndex = 1;
            this.tabPageProperties.Text = "Properties";
            this.tabPageProperties.UseVisualStyleBackColor = true;
            // 
            // textBoxTimer
            // 
            this.textBoxTimer.Location = new System.Drawing.Point(128, 471);
            this.textBoxTimer.Name = "textBoxTimer";
            this.textBoxTimer.Size = new System.Drawing.Size(100, 20);
            this.textBoxTimer.TabIndex = 8;
            this.textBoxTimer.Text = "300";
            // 
            // labelTimer
            // 
            this.labelTimer.AutoSize = true;
            this.labelTimer.Location = new System.Drawing.Point(53, 474);
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(69, 13);
            this.labelTimer.TabIndex = 7;
            this.labelTimer.Text = "Spawn Timer";
            // 
            // textBoxZombieMax
            // 
            this.textBoxZombieMax.Location = new System.Drawing.Point(154, 375);
            this.textBoxZombieMax.Name = "textBoxZombieMax";
            this.textBoxZombieMax.Size = new System.Drawing.Size(100, 20);
            this.textBoxZombieMax.TabIndex = 6;
            this.textBoxZombieMax.Text = "60";
            this.textBoxZombieMax.TextChanged += new System.EventHandler(this.textBoxZombieMax_TextChanged);
            this.textBoxZombieMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxZombieMax_KeyPress);
            // 
            // labelZombieMax
            // 
            this.labelZombieMax.AutoSize = true;
            this.labelZombieMax.Location = new System.Drawing.Point(52, 378);
            this.labelZombieMax.Name = "labelZombieMax";
            this.labelZombieMax.Size = new System.Drawing.Size(96, 13);
            this.labelZombieMax.TabIndex = 5;
            this.labelZombieMax.Text = "Zombie Max Count";
            // 
            // comboBoxSpawnAcceleration
            // 
            this.comboBoxSpawnAcceleration.FormattingEnabled = true;
            this.comboBoxSpawnAcceleration.Items.AddRange(new object[] {
            "true",
            "false"});
            this.comboBoxSpawnAcceleration.Location = new System.Drawing.Point(160, 280);
            this.comboBoxSpawnAcceleration.Name = "comboBoxSpawnAcceleration";
            this.comboBoxSpawnAcceleration.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSpawnAcceleration.TabIndex = 4;
            this.comboBoxSpawnAcceleration.SelectedIndexChanged += new System.EventHandler(this.comboBoxSpawnAcceleration_SelectedIndexChanged);
            // 
            // labelSpawnAcceleration
            // 
            this.labelSpawnAcceleration.AutoSize = true;
            this.labelSpawnAcceleration.Location = new System.Drawing.Point(48, 283);
            this.labelSpawnAcceleration.Name = "labelSpawnAcceleration";
            this.labelSpawnAcceleration.Size = new System.Drawing.Size(102, 13);
            this.labelSpawnAcceleration.TabIndex = 3;
            this.labelSpawnAcceleration.Text = "Spawn Acceleration";
            // 
            // comboBoxBackground
            // 
            this.comboBoxBackground.FormattingEnabled = true;
            this.comboBoxBackground.Location = new System.Drawing.Point(119, 186);
            this.comboBoxBackground.Name = "comboBoxBackground";
            this.comboBoxBackground.Size = new System.Drawing.Size(121, 21);
            this.comboBoxBackground.TabIndex = 2;
            this.comboBoxBackground.SelectedIndexChanged += new System.EventHandler(this.comboBoxBackground_SelectedIndexChanged);
            // 
            // labelBackGround
            // 
            this.labelBackGround.AutoSize = true;
            this.labelBackGround.Location = new System.Drawing.Point(48, 189);
            this.labelBackGround.Name = "labelBackGround";
            this.labelBackGround.Size = new System.Drawing.Size(65, 13);
            this.labelBackGround.TabIndex = 0;
            this.labelBackGround.Text = "Background";
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Location = new System.Drawing.Point(89, 81);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(100, 20);
            this.textBoxHeight.TabIndex = 1;
            this.textBoxHeight.TextChanged += new System.EventHandler(this.textBoxHeight_TextChanged);
            this.textBoxHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxHeight_KeyPress);
            // 
            // labelHeight
            // 
            this.labelHeight.AutoSize = true;
            this.labelHeight.Location = new System.Drawing.Point(48, 84);
            this.labelHeight.Name = "labelHeight";
            this.labelHeight.Size = new System.Drawing.Size(38, 13);
            this.labelHeight.TabIndex = 0;
            this.labelHeight.Text = "Height";
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.Location = new System.Drawing.Point(89, 55);
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(100, 20);
            this.textBoxWidth.TabIndex = 1;
            this.textBoxWidth.TextChanged += new System.EventHandler(this.textBoxWidth_TextChanged);
            this.textBoxWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxWidth_KeyPress);
            // 
            // labelWidth
            // 
            this.labelWidth.AutoSize = true;
            this.labelWidth.Location = new System.Drawing.Point(48, 58);
            this.labelWidth.Name = "labelWidth";
            this.labelWidth.Size = new System.Drawing.Size(35, 13);
            this.labelWidth.TabIndex = 0;
            this.labelWidth.Text = "Width";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.texturesToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1045, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // texturesToolStripMenuItem
            // 
            this.texturesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem});
            this.texturesToolStripMenuItem.Name = "texturesToolStripMenuItem";
            this.texturesToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.texturesToolStripMenuItem.Text = "Textures";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1045, 640);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.tabPageLevel.ResumeLayout(false);
            this.panelSolids.ResumeLayout(false);
            this.panelSolids.PerformLayout();
            this.tabPageProperties.ResumeLayout(false);
            this.tabPageProperties.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel levelPanel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLevel;
        private System.Windows.Forms.TabPage tabPageProperties;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxWidth;
        private System.Windows.Forms.Label labelWidth;
        private System.Windows.Forms.TextBox textBoxHeight;
        private System.Windows.Forms.Label labelHeight;
        private System.Windows.Forms.Label labelBackGround;
        private System.Windows.Forms.ComboBox comboBoxBackground;
        private System.Windows.Forms.ToolStripMenuItem texturesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.Label labelSolidTexture;
        private System.Windows.Forms.ComboBox comboBoxSolidTexture;
        private System.Windows.Forms.Panel panelSolids;
        private System.Windows.Forms.ComboBox comboBoxClickMode;
        private System.Windows.Forms.Label labelClickMode;
        private System.Windows.Forms.ComboBox comboBoxSpawnAcceleration;
        private System.Windows.Forms.Label labelSpawnAcceleration;
        private System.Windows.Forms.TextBox textBoxZombieMax;
        private System.Windows.Forms.Label labelZombieMax;
        private System.Windows.Forms.TextBox textBoxTimer;
        private System.Windows.Forms.Label labelTimer;

    }
}

