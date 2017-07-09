using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LevelBuilder;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace LevelEditor
{
    public partial class Form1 : Form
    {
        public LevelData level;
        TextureBaseData[] textures;
        bool selectedSolidTexture = false;
        bool selectedBGTexture = false;
        System.Drawing.Point levelPanelScrollPos;
        bool movingScreen = false;
        int selected = -1;
        System.Drawing.Point oldMouse;
        System.Drawing.Point newMouse;
        bool textureMoved = false;

        public Form1()
        {
            InitializeComponent();
            level = new LevelData();
            level.backgroundColor = Microsoft.Xna.Framework.Color.White;
            level.backgroundReference = 0;
            level.levelHeight = 1024;
            level.levelWidth = 1024;
            level.maxAmountOfZombies = 60;
            level.playerSpawn = new Vector2(0, 0);
            level.solids = new SolidData[0];
            level.spawnTimer = 300;
            level.textures = new TextureData[0];
            level.zombieSpawners = new Vector2[0];

            levelPanelScrollPos = new System.Drawing.Point(0, 0);

            textures = new TextureBaseData[5]
            {
                new TextureBaseData(Properties.Resources.White, "White"),
                new TextureBaseData(Properties.Resources.ZombieSpawner, "ZombieSpawner"),
                new TextureBaseData(Properties.Resources.Player, "Player"),
                new TextureBaseData(Properties.Resources.Block, "Block"),
                new TextureBaseData(Properties.Resources.jacketsjlogo, "jacketsj Logo")
            };

            textBoxWidth.Text = level.levelWidth.ToString();
            textBoxHeight.Text = level.levelHeight.ToString();

            UpdateTextureLists();
            PutPicturesInPanel();

            comboBoxBackground.SelectedIndex = 0;
            comboBoxSpawnAcceleration.SelectedIndex = 0;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (level.zombieSpawners.Length > 0)
            {
                level.textures = new TextureData[textures.Length];
                for (int i = 0; i < textures.Length; i++)
                {
                    level.textures[i] = textures[i].ToTextureData();
                }
                LevelData.SaveLevel(level);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LevelData OpenedLevel = LevelData.LoadLevel();
            if (OpenedLevel != null)
            {
                level = OpenedLevel;
                textures = new TextureBaseData[level.textures.Length];
                for (int i = 0; i < level.textures.Length; i++)
                {
                    textures[i] = new TextureBaseData(level.textures[i]);
                }
                UpdateTextureLists();
                PutPicturesInPanel();

                textBoxWidth.Text = level.levelWidth.ToString();
                textBoxHeight.Text = level.levelHeight.ToString();
                if (level.zombieSpawnAcceleration)
                {
                    comboBoxSpawnAcceleration.SelectedIndex = 0;
                }
                else
                {
                    comboBoxSpawnAcceleration.SelectedIndex = 1;
                }


            }
        }

        private void textBoxWidth_TextChanged(object sender, EventArgs e)
        {
            if (textBoxWidth.Text != "")
            {
                try
                {
                    level.levelWidth = Convert.ToInt32(textBoxWidth.Text);
                }
                catch (Exception ex)
                {
                    level.levelWidth = int.MaxValue;
                    textBoxWidth.Text = int.MaxValue.ToString();
                }
            }
            else
            {
                level.levelWidth = 0;
            }
            PutPicturesInPanel();
        }

        private void textBoxHeight_TextChanged(object sender, EventArgs e)
        {
            if (textBoxHeight.Text != "")
            {
                try
                {
                    level.levelHeight = Convert.ToInt32(textBoxHeight.Text);
                }
                catch (Exception ex)
                {
                    level.levelHeight = int.MaxValue;
                    textBoxHeight.Text = int.MaxValue.ToString();
                }
            }
            else
            {
                level.levelHeight = 0;
            }
            PutPicturesInPanel();
        }

        private void textBoxWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)System.Windows.Forms.Keys.Back)
            {
                e.Handled = !char.IsDigit(e.KeyChar);
            }
        }

        private void textBoxHeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)System.Windows.Forms.Keys.Back)
            {
                e.Handled = !char.IsDigit(e.KeyChar);
            }
        }

        private void comboBoxBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            level.backgroundReference = comboBoxBackground.SelectedIndex;
            selectedBGTexture = true;
            PutPicturesInPanel();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Open Image";
            //openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "PNG files (*.png)|*.png";
            //openFileDialog1.FilterIndex = 2;
            //openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    TextureBaseData texture = new TextureBaseData(new Bitmap(openFileDialog1.FileName), openFileDialog1.SafeFileName);

                    TextureBaseData[] newTextures = new TextureBaseData[textures.Length + 1];
                    for (int i = 0; i < textures.Length; i++)
                    {
                        newTextures[i] = textures[i];
                    }
                    newTextures[textures.Length] = texture;
                    textures = newTextures;
                    UpdateTextureLists();
                    comboBoxSolidTexture.SelectedIndex = textures.Length - 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            openFileDialog1.Dispose();
        }

        public void UpdateTextureLists()
        {
            comboBoxBackground.Items.Clear();
            comboBoxSolidTexture.Items.Clear();
            foreach (TextureBaseData texture in textures)
            {
                comboBoxBackground.Items.Add(texture.Name);
                comboBoxSolidTexture.Items.Add(texture.Name);
            }
        }

        public void UpdatePanelWidthAndHeight()
        {
            
        }

        public void renderBoxOntoMap(Graphics g, Image pBox, System.Drawing.Point location)
        {
            g.DrawImage(pBox, location);
        }

        public void renderBoxOntoMap(Graphics g, Image pBox, System.Drawing.Point location, Size size)
        {
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            Bitmap pBmp = new Bitmap(pBox, size);
            //g.DrawImage(pBmp, location);
            renderBoxOntoMap(g, pBmp, location);
            //pBox.Image.(bmp, new System.Drawing.Rectangle(Math.Max(location.X, 0), Math.Max(location.Y, 0), Math.Min(location.X + pBox.Width, level.levelWidth), Math.Min(location.Y + pBox.Height, level.levelWidth)));
        }

        public void PutPicturesInPanel()
        {
            List<Control> newControls = new List<Control>();
            //levelPanel.Controls.Clear();
            //levelPanel.AutoScrollPosition = levelPanelScrollPos;
            //levelPanel.Controls
            Bitmap render = new Bitmap(level.levelWidth, level.levelHeight);
            PictureBox drawn = new PictureBox();
            drawn.MouseClick += new MouseEventHandler(levelPanel_Click);
            drawn.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
            drawn.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
            drawn.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            drawn.Location = new System.Drawing.Point(0,0);
            drawn.Width = level.levelHeight;
            drawn.Height = level.levelWidth;
            Graphics g = Graphics.FromImage(render);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.AssumeLinear;
            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;

            if (selectedBGTexture)
            {
                //PictureBox bgPictureBox = new PictureBox();
                //bgPictureBox.BackColor = System.Drawing.Color.Transparent;
                //bgPictureBox.Image = textures[level.backgroundReference].Image;
                //bgPictureBox.Width = level.levelWidth;
                //bgPictureBox.Height = level.levelHeight;
                //bgPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                //bgPictureBox.Location = new System.Drawing.Point(0, 0);
                renderBoxOntoMap(g, textures[level.backgroundReference].Image, new System.Drawing.Point(0, 0), new Size(level.levelWidth, level.levelHeight));
                //bgPictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                //bgPictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                //bgPictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                //bgPictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }

            //PictureBox PlayerPictureBox = new PictureBox();
            //PlayerPictureBox.BackColor = System.Drawing.Color.Transparent;
            //PlayerPictureBox.Image = textures[2].Image;
            //PlayerPictureBox.Width = PlayerPictureBox.Image.Width;
            //PlayerPictureBox.Height = PlayerPictureBox.Image.Height;
            //PlayerPictureBox.Location = new System.Drawing.Point((int)level.playerSpawn.X, (int)level.playerSpawn.Y);
            renderBoxOntoMap(g, textures[2].Image, new System.Drawing.Point((int)level.playerSpawn.X, (int)level.playerSpawn.Y));
            //PlayerPictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
            //PlayerPictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
            //PlayerPictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
            //PlayerPictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);

            foreach (Vector2 spawner in level.zombieSpawners)
            {
                //PictureBox pictureBox = new PictureBox();
                //pictureBox.BackColor = System.Drawing.Color.Transparent;
                //pictureBox.Image = textures[1].Image;
                //pictureBox.Width = pictureBox.Image.Width;
                //pictureBox.Height = pictureBox.Image.Height;
                //pictureBox.Location = new System.Drawing.Point((int)spawner.X, (int)spawner.Y);
                renderBoxOntoMap(g, textures[1].Image, new System.Drawing.Point((int)spawner.X, (int)spawner.Y));
                //pictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                //pictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                //pictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                //pictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }

            foreach (SolidData solid in level.solids)
            {
                //PictureBox pictureBox = new PictureBox();
                //pictureBox.BackColor = System.Drawing.Color.Transparent;
                //pictureBox.Image = textures[solid.textureNo].Image;
                //pictureBox.Width = pictureBox.Image.Width;
                //pictureBox.Height = pictureBox.Image.Height;
                //pictureBox.Location = new System.Drawing.Point((int)solid.position.X, (int)solid.position.Y);
                renderBoxOntoMap(g, textures[solid.textureNo].Image, new System.Drawing.Point((int)solid.position.X, (int)solid.position.Y));
                //pictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                //pictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                //pictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                //pictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }

            if (checkBoxShowGrid.Checked)
            {
                //draw vertical lines
                Bitmap lineImage = new Bitmap(1, 1);
                lineImage.SetPixel(0, 0, System.Drawing.Color.Black);
                for (int i = 0; i < level.levelWidth; i += (int)numericUpDownGrid.Value)
                {
                    //PictureBox linePictureBox = new PictureBox();
                    //linePictureBox.Image = lineImage; //the black texture
                    //linePictureBox.Width = 2;
                    //linePictureBox.Height = level.levelHeight;
                    //linePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //linePictureBox.Location = new System.Drawing.Point(i - 1, 0);
                    //renderBoxOntoMap(g, lineImage, new System.Drawing.Point(i - 1, 0), new Size(2, level.levelHeight));
                    g.DrawRectangle(new Pen(System.Drawing.Color.Black), i - 1, 0, 1, level.levelHeight);
                }
                //draw horizontal lines
                for (int i = 0; i < level.levelWidth; i += (int)numericUpDownGrid.Value)
                {
                    //PictureBox linePictureBox = new PictureBox();
                    //linePictureBox.Image = lineImage; //the black texture
                    //linePictureBox.Width = level.levelWidth;
                    //linePictureBox.Height = 2;
                    //linePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    //linePictureBox.Location = new System.Drawing.Point(0, i - 1);
                    //renderBoxOntoMap(g, lineImage, new System.Drawing.Point(0, i - 1), new Size(level.levelWidth, 2));
                    g.DrawRectangle(new Pen(System.Drawing.Color.Black), 0, i - 1, level.levelWidth, 1);
                }
            }

            /*
            if (checkBoxShowGrid.Checked)
            {
                //draw vertical lines
                Bitmap lineImage = new Bitmap(1, 1);
                lineImage.SetPixel(0, 0, System.Drawing.Color.Black);
                for (int i = 0; i < level.levelWidth; i += (int)numericUpDownGrid.Value)
                {
                    PictureBox linePictureBox = new PictureBox();
                    linePictureBox.Image = lineImage; //the black texture
                    linePictureBox.Width = 2;
                    linePictureBox.Height = level.levelHeight;
                    linePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    linePictureBox.Location = new System.Drawing.Point(i - 1, 0);
                    newControls.Add(linePictureBox);
                }
                //draw horizontal lines
                for (int i = 0; i < level.levelWidth; i += (int)numericUpDownGrid.Value)
                {
                    PictureBox linePictureBox = new PictureBox();
                    linePictureBox.Image = lineImage; //the black texture
                    linePictureBox.Width = level.levelWidth;
                    linePictureBox.Height = 2;
                    linePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    linePictureBox.Location = new System.Drawing.Point(0, i - 1);
                    newControls.Add(linePictureBox);
                }
            }
            
            foreach (SolidData solid in level.solids)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.BackColor = System.Drawing.Color.Transparent;
                pictureBox.Image = textures[solid.textureNo].Image;
                pictureBox.Width = pictureBox.Image.Width;
                pictureBox.Height = pictureBox.Image.Height;
                pictureBox.Location = new System.Drawing.Point((int)solid.position.X, (int)solid.position.Y);
                newControls.Add(pictureBox);
                pictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                pictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                pictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                pictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }

            foreach (Vector2 spawner in level.zombieSpawners)
            {
                PictureBox pictureBox = new PictureBox();
                pictureBox.BackColor = System.Drawing.Color.Transparent;
                pictureBox.Image = textures[1].Image;
                pictureBox.Width = pictureBox.Image.Width;
                pictureBox.Height = pictureBox.Image.Height;
                pictureBox.Location = new System.Drawing.Point((int)spawner.X, (int)spawner.Y);
                newControls.Add(pictureBox);
                pictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                pictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                pictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                pictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }

            PictureBox PlayerPictureBox = new PictureBox();
            PlayerPictureBox.BackColor = System.Drawing.Color.Transparent;
            PlayerPictureBox.Image = textures[2].Image;
            PlayerPictureBox.Width = PlayerPictureBox.Image.Width;
            PlayerPictureBox.Height = PlayerPictureBox.Image.Height;
            PlayerPictureBox.Location = new System.Drawing.Point((int)level.playerSpawn.X, (int)level.playerSpawn.Y);
            newControls.Add(PlayerPictureBox);
            PlayerPictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
            PlayerPictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
            PlayerPictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
            PlayerPictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);

            if (selectedBGTexture)
            {
                PictureBox bgPictureBox = new PictureBox();
                bgPictureBox.BackColor = System.Drawing.Color.Transparent;
                bgPictureBox.Image = textures[level.backgroundReference].Image;
                bgPictureBox.Width = level.levelWidth;
                bgPictureBox.Height = level.levelHeight;
                bgPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                bgPictureBox.Location = new System.Drawing.Point(0, 0);
                newControls.Add(bgPictureBox);
                bgPictureBox.MouseClick += new MouseEventHandler(levelPanel_Click);
                bgPictureBox.MouseDown += new MouseEventHandler(levelPanel_MouseDown);
                bgPictureBox.MouseMove += new MouseEventHandler(levelPanel_MouseMove);
                bgPictureBox.MouseUp += new MouseEventHandler(levelPanel_MouseUp);
            }
            */

            //levelPanel.AutoScrollPosition = levelPanelScrollPos;
            //List<Control> toRemove = new List<Control>();
            //foreach (Control control in levelPanel.Controls)
            //{
            //    if (!newControls.Contains(control))
            //    {
            //        toRemove.Add(control);
            //    }
            //}
            //newControls.RemoveAll(delegate(Control c){return !levelPanel.Controls.Contains(c);});
            //foreach (Control control in newControls)
            //{
            //    if (!levelPanel.Controls.Contains(control))
            //    {
            //        newControls.Remove(control);
            //    }
            //}
            //foreach (Control control in toRemove)
            //{
            //    levelPanel.Controls.Remove(control);
            //}
            drawn.Image = render;
            g.Dispose();
            ControlPause.SuspendDrawing(levelPanel);
            //levelPanel.AutoScrollPosition = levelPanelScrollPos;
            levelPanel.Controls.Clear();
            levelPanel.Controls.Add(drawn);
            //levelPanel.Controls.AddRange(newControls.ToArray());
            levelPanel.AutoScrollPosition = levelPanelScrollPos;
            ControlPause.ResumeDrawing(levelPanel);
            //foreach (Control control in newControls)
            //{
            //    levelPanel.Controls.Add(control);
            //}
            //levelPanel.AutoScrollPosition = levelPanelScrollPos;
            levelPanel.Focus();
        }

        private void levelPanel_Paint(object sender, PaintEventArgs e)
        {
            if (movingScreen)
            {
                resetLevelPanelScroll();
            }
        }

        private void updateScrollPos()
        {
            levelPanelScrollPos = new System.Drawing.Point(levelPanel.AutoScrollPosition.X / -1, levelPanel.AutoScrollPosition.Y / -1);
        }

        private void levelPanel_Click(object sender, EventArgs e)
        {
            //levelPanel.AutoScrollPosition = levelPanelScrollPos;
            //movingScreen = true;

            alterLevelElement(levelPanelScrollPos);
        }

        private void alterLevelElement(System.Drawing.Point levelPanelScrollPos)
        {
            updateScrollPos();
            if (comboBoxClickMode.SelectedIndex == 0)
            {
                if (selectedSolidTexture)
                {
                    SolidData[] newSolids = new SolidData[level.solids.Length + 1];
                    for (int i = 0; i < level.solids.Length; i++)
                    {
                        newSolids[i] = level.solids[i];
                    }
                    System.Drawing.Point point = levelPanel.PointToClient(Cursor.Position);
                    point.X += levelPanel.HorizontalScroll.Value;
                    point.Y += levelPanel.VerticalScroll.Value;
                    point.X -= textures[comboBoxSolidTexture.SelectedIndex].Image.Width / 2;
                    point.Y -= textures[comboBoxSolidTexture.SelectedIndex].Image.Height / 2;
                    point = snap(point);
                    newSolids[level.solids.Length] = new SolidData(new Vector2(point.X, point.Y), comboBoxSolidTexture.SelectedIndex, Microsoft.Xna.Framework.Color.White);
                    level.solids = newSolids;
                    PutPicturesInPanel();
                }
            }
            if (comboBoxClickMode.SelectedIndex == 1)
            {
                bool deletedSomething = false;
                for (int i = 0; i < level.solids.Length; i++)
                {
                    SolidData solid = level.solids[i];
                    System.Drawing.Point mouse = levelPanel.PointToClient(Cursor.Position);
                    mouse.X += levelPanel.HorizontalScroll.Value;
                    mouse.Y += levelPanel.VerticalScroll.Value;
                    if ((mouse.X >= solid.position.X) && (mouse.Y >= solid.position.Y) && (mouse.X <= solid.position.X + textures[solid.textureNo].Image.Width) && (mouse.Y <= solid.position.Y + textures[solid.textureNo].Image.Height))
                    {
                        int newLength = level.solids.Length - 1;
                        SolidData[] newSolids = new SolidData[newLength];
                        bool foundDeleted = false;
                        for (int i2 = 0; i2 < level.solids.Length; i2++)
                        {
                            if (foundDeleted)
                            {
                                newSolids[i2 - 1] = level.solids[i2];
                            }
                            else
                            {
                                if (i == i2)
                                {
                                    foundDeleted = true;
                                }
                                else
                                {
                                    newSolids[i2] = level.solids[i2];
                                }
                            }
                        }
                        level.solids = newSolids;
                        deletedSomething = true;
                        break;
                    }
                }
                if (!deletedSomething)
                {
                    for (int i2 = 0; i2 < level.zombieSpawners.Length; i2++)
                    {
                        Vector2 spawner = level.zombieSpawners[i2];
                        System.Drawing.Point mouse = levelPanel.PointToClient(Cursor.Position);
                        mouse.X += levelPanel.HorizontalScroll.Value;
                        mouse.Y += levelPanel.VerticalScroll.Value;
                        if ((mouse.X >= spawner.X) && (mouse.Y >= spawner.Y) && (mouse.X <= spawner.X + textures[2].Image.Width) && (mouse.Y <= spawner.Y + textures[2].Image.Height))
                        {
                            int newLength = level.zombieSpawners.Length - 1;
                            Vector2[] newSpawners = new Vector2[newLength];
                            bool foundDeleted = false;
                            for (int i3 = 0; i3 < level.zombieSpawners.Length; i3++)
                            {
                                if (foundDeleted)
                                {
                                    newSpawners[i3 - 1] = level.zombieSpawners[i3];
                                }
                                else
                                {
                                    if (i2 == i3)
                                    {
                                        foundDeleted = true;
                                    }
                                    else
                                    {
                                        newSpawners[i3] = level.zombieSpawners[i3];
                                    }
                                }
                            }
                            level.zombieSpawners = newSpawners;
                            break;
                        }
                    }
                }
                PutPicturesInPanel();
            }
            if (comboBoxClickMode.SelectedIndex == 2)
            {
                System.Drawing.Point point = levelPanel.PointToClient(Cursor.Position);
                //System.Drawing.Point point =  new System.Drawing.Point(levelPanel.PointToClient(Cursor.Position).X - textures[2].Image.Width / 2,
                //                                                        levelPanel.PointToClient(Cursor.Position).Y - textures[2].Image.Height / 2);
                point.X -= textures[2].Image.Width / 2;
                point.Y -= textures[2].Image.Height / 2;
                point = snap(point);
                level.playerSpawn.X = point.X + levelPanel.HorizontalScroll.Value;
                level.playerSpawn.Y = point.Y + levelPanel.VerticalScroll.Value;
                PutPicturesInPanel();
            }
            if (comboBoxClickMode.SelectedIndex == 3)
            {
                Vector2[] newSpawns = new Vector2[level.zombieSpawners.Length + 1];
                for (int i = 0; i < level.zombieSpawners.Length; i++)
                {
                    newSpawns[i] = level.zombieSpawners[i];
                }
                System.Drawing.Point point = levelPanel.PointToClient(Cursor.Position);
                point.X += levelPanel.HorizontalScroll.Value;
                point.Y += levelPanel.VerticalScroll.Value;
                point.X -= textures[2].Image.Width / 2;
                point.Y -= textures[2].Image.Height / 2;
                point = snap(point);
                newSpawns[level.zombieSpawners.Length] = new Vector2(point.X, point.Y);
                level.zombieSpawners = newSpawns;
                PutPicturesInPanel();
            }
        }

        private void comboBoxSolidTexture_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSolidTexture = true;
        }

        private void comboBoxSpawnAcceleration_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSpawnAcceleration.SelectedIndex == 0)
            {
                level.zombieSpawnAcceleration = true;
            }
            else if (comboBoxSpawnAcceleration.SelectedIndex == 1)
            {
                level.zombieSpawnAcceleration = false;
            }
        }

        private void textBoxZombieMax_TextChanged(object sender, EventArgs e)
        {
            if (textBoxZombieMax.Text != "")
            {
                try
                {
                    level.maxAmountOfZombies = Convert.ToInt32(textBoxWidth.Text);
                }
                catch (Exception ex)
                {
                    level.maxAmountOfZombies = int.MaxValue;
                    textBoxZombieMax.Text = int.MaxValue.ToString();
                }
            }
            else
            {
                level.maxAmountOfZombies = 0;
            }
            PutPicturesInPanel();
        }

        private void textBoxZombieMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)System.Windows.Forms.Keys.Back)
            {
                e.Handled = !char.IsDigit(e.KeyChar);
            }
        }

        private void resetLevelPanelScroll()
        {
            if (movingScreen)
            {
                levelPanel.AutoScrollPosition = levelPanelScrollPos;
                movingScreen = false;
            }
        }

        private void levelPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboBoxClickMode.SelectedIndex == 4)
            {
                System.Drawing.Point mouse = levelPanel.PointToClient(Cursor.Position);
                mouse.X += levelPanel.HorizontalScroll.Value;
                mouse.Y += levelPanel.VerticalScroll.Value;
                for (int i = 0; i < level.solids.Length; i++)
                {
                    if ((mouse.X >= level.solids[i].position.X) && (mouse.Y >= level.solids[i].position.Y) && (mouse.X <= level.solids[i].position.X + textures[level.solids[i].textureNo].Image.Width) && (mouse.Y <= level.solids[i].position.Y + textures[level.solids[i].textureNo].Image.Height))
                    {
                        selected = i;
                        textureMoved = true;
                        break;
                    }
                }
            }
        }

        private void levelPanel_MouseUp(object sender, MouseEventArgs e)
        {
            selected = -1;
            if (textureMoved)
            {
                textureMoved = false;
                levelPanelScrollPos = new System.Drawing.Point(levelPanel.AutoScrollPosition.X / -1, levelPanel.AutoScrollPosition.Y / -1);
                PutPicturesInPanel();
            }
        }

        private void levelPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected != -1)
            {
                oldMouse = new System.Drawing.Point(newMouse.X, newMouse.Y);
                newMouse = levelPanel.PointToClient(Cursor.Position);
                newMouse.X += levelPanel.HorizontalScroll.Value;
                newMouse.Y += levelPanel.VerticalScroll.Value;

                level.solids[selected].position += new Vector2(newMouse.X - oldMouse.X, newMouse.Y - oldMouse.Y);
                level.solids[selected].position = snap(level.solids[selected].position);
            }
            else
            {
                newMouse = levelPanel.PointToClient(Cursor.Position);
                newMouse.X += levelPanel.HorizontalScroll.Value;
                newMouse.Y += levelPanel.VerticalScroll.Value;
                oldMouse = new System.Drawing.Point(newMouse.X, newMouse.Y);

                if (checkBoxSnap.Checked && Mouse.GetState().LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                {
                    bool canDragPlace = true;
                    for (int i = 0; i < level.solids.Length && canDragPlace; i++)
                    {
                        SolidData solid = level.solids[i];
                        System.Drawing.Point mouse = levelPanel.PointToClient(Cursor.Position);
                        mouse.X += levelPanel.HorizontalScroll.Value;
                        mouse.Y += levelPanel.VerticalScroll.Value;

                        if ((mouse.X >= solid.position.X) && (mouse.Y >= solid.position.Y) && (mouse.X <= solid.position.X + textures[solid.textureNo].Image.Width) && (mouse.Y <= solid.position.Y + textures[solid.textureNo].Image.Height))
                        {
                            canDragPlace = false;
                        }
                    }
                    for (int i2 = 0; i2 < level.zombieSpawners.Length && canDragPlace; i2++)
                    {
                        Vector2 spawner = level.zombieSpawners[i2];
                        System.Drawing.Point mouse = levelPanel.PointToClient(Cursor.Position);
                        mouse.X += levelPanel.HorizontalScroll.Value;
                        mouse.Y += levelPanel.VerticalScroll.Value;
                        if ((mouse.X >= spawner.X) && (mouse.Y >= spawner.Y) && (mouse.X <= spawner.X + textures[2].Image.Width) && (mouse.Y <= spawner.Y + textures[2].Image.Height))
                        {
                            canDragPlace = false;
                        }
                    }
                    if (comboBoxClickMode.SelectedIndex == 1)
                    {
                        canDragPlace = true;
                    }
                    if (canDragPlace)
                    {
                        alterLevelElement(newMouse);
                    }
                }
            }

        }

        private void levelPanel_MouseLeave(object sender, EventArgs e)
        {
            selected = -1;
            if (textureMoved)
            {
                textureMoved = false;
                levelPanelScrollPos = new System.Drawing.Point(levelPanel.AutoScrollPosition.X / -1, levelPanel.AutoScrollPosition.Y / -1);
                PutPicturesInPanel();
            }
        }

        private void numericUpDownGrid_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownGrid.Value < 1)
            {
                numericUpDownGrid.Value = 1;
            }
            numericUpDownGrid.Value = (int)numericUpDownGrid.Value;
            PutPicturesInPanel();
        }

        private void checkBoxShowGrid_CheckedChanged(object sender, EventArgs e)
        {
            PutPicturesInPanel();
        }

        private System.Drawing.Point snap(System.Drawing.Point inPoint)
        {
            System.Drawing.Point point = new System.Drawing.Point(inPoint.X, inPoint.Y);
            if (checkBoxSnap.Checked)
            {
                int snap = (int)numericUpDownGrid.Value;
                point.X = (int)(point.X / snap) * snap + ((float)point.X % snap > (float)snap / 2 ? snap : 0);
                point.Y = (int)(point.Y / snap) * snap + ((float)point.Y % snap > (float)snap / 2 ? snap : 0);
            }
            return point;
        }

        private Vector2 snap(Vector2 inPoint)
        {
            Vector2 point = new Vector2(inPoint.X, inPoint.Y);
            if (checkBoxSnap.Checked)
            {
                int snap = (int)numericUpDownGrid.Value;
                point.X = (int)(point.X / snap) * snap + (point.X % snap > (float)snap / 2 ? snap : 0);
                point.Y = (int)(point.Y / snap) * snap + (point.Y % snap > (float)snap / 2 ? snap : 0);
            }
            return point;
        }
    }
}
