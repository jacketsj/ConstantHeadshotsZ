using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace LevelBuilder
{
    public class TextureData
    {
        public Color[] Colors;
        public int Height;
        public int Width;
        public string Name;

        public TextureData()
        {

        }

        public TextureData(Color[] newColors, int newHeight, int newWidth)
        {
            Colors = newColors;
            Height = newHeight;
            Width = newWidth;
        }

        public TextureData(Color[] newColors, int newHeight, int newWidth, string newName)
        {
            Colors = newColors;
            Height = newHeight;
            Width = newWidth;
            Name = newName;
        }
    }
}
