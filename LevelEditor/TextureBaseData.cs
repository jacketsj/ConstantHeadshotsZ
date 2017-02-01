using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using LevelBuilder;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LevelEditor
{
    class TextureBaseData
    {
        public Bitmap Image;
        public string Name;

        public TextureBaseData(Bitmap newImage, string newName)
        {
            Image = newImage;
            Name = newName;
        }

        public TextureBaseData(TextureData texture)
        {
            Image = new Bitmap(texture.Width, texture.Height);
            for (int y = 0; y < Image.Height; y++)
            {
                for (int x = 0; x < Image.Width; x++)
                {
                    //Microsoft.Xna.Framework.Color c = new Microsoft.Xna.Framework.Color(Image.GetPixel(x, y).R, Image.GetPixel(x, y).G, Image.GetPixel(x, y).B, Image.GetPixel(x, y).A);
                    //pixels[(y * Image.Width) + x] = new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
                    Image.SetPixel(x, y, XnaColorToDrawingColor(texture.Colors[(y * Image.Width) + x]));
                }
            }
            Name = texture.Name;
        }

        public System.Drawing.Color XnaColorToDrawingColor(Microsoft.Xna.Framework.Color XnaColor)
        {
            return System.Drawing.Color.FromArgb(XnaColor.A, XnaColor.R, XnaColor.G, XnaColor.B);
        }

        public TextureData ToTextureData()
        {
            TextureData texture = new TextureData(GetData(), Image.Height, Image.Width, Name);
            return texture;
        }

        private Microsoft.Xna.Framework.Color[] GetData()
        {
            Microsoft.Xna.Framework.Color[] pixels = new Microsoft.Xna.Framework.Color[Image.Width * Image.Height];
            for (int y = 0; y < Image.Height; y++)
            {
                for (int x = 0; x < Image.Width; x++)
                {
                    Microsoft.Xna.Framework.Color c = new Microsoft.Xna.Framework.Color(Image.GetPixel(x, y).R, Image.GetPixel(x, y).G, Image.GetPixel(x, y).B, Image.GetPixel(x, y).A);
                    pixels[(y * Image.Width) + x] = new Microsoft.Xna.Framework.Color(c.R, c.G, c.B, c.A);
                }
            }
            return pixels;
        }
    }
}
