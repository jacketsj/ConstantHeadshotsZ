using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace LevelBuilder
{
    public class SolidData
    {
        public Vector2 position;
        public int textureNo;
        public Color tint;

        
        public SolidData(Vector2 newPosition, int newTextureNo, Color newTint)
        {
            position = newPosition;
            textureNo = newTextureNo;
            tint = newTint;
        }
        
        public SolidData()
        {

        }
    }
}
