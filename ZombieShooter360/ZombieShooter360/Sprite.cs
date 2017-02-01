using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieShooter360
{
    class Sprite
    {
        private Texture2D texture;
        public Vector2 vector;
        private Color tint = Color.White;
        public Color[] textureData;

        public Sprite(Texture2D newTexture, Vector2 newVector)
        {
            texture = newTexture;
            vector = newVector;
            UpdateTextureData();
        }

        public Sprite(Texture2D newTexture, Vector2 newVector, Color newTint)
        {
            texture = newTexture;
            vector = newVector;
            tint = newTint;
            UpdateTextureData();
        }

        public void UpdateTextureData()
        {
            textureData = new Color[getTexture().Width * getTexture().Height];
            texture.GetData(textureData);
        }

        public void setTexture(Texture2D newTexture)
        {
            texture = newTexture;
            UpdateTextureData();
        }

        public Texture2D getTexture()
        {
            return texture;
        }

        public void setX(float x)
        {
            vector.X = x;
        }

        public void setY(float y)
        {
            vector.Y = y;
        }

        public float getX()
        {
            return vector.X;
        }

        public float getY()
        {
            return vector.Y;
        }

        public void setTint(Color newTint)
        {
            tint = newTint;
        }

        public Color getTint()
        {
            return tint;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, vector, tint);
        }

        public bool Intersects(Vector2 intersectingVector)
        {
            if (intersectingVector.X >= vector.X && intersectingVector.X <= (vector.X + texture.Width) && intersectingVector.Y >= vector.Y && intersectingVector.Y <= (vector.Y + texture.Height))
            {
                return true;
            }
            return false;
        }

        public bool Intersects(Sprite intersectingSprite)
        {
            if (intersectingSprite == this)
            {
                return false;
            }
            Rectangle originalRec = new Rectangle((int)vector.X, (int)vector.Y, texture.Width, texture.Height);
            //Rectangle intersectingRec = new Rectangle((int)intersectingSprite.vector.X, (int)intersectingSprite.vector.Y, intersectingSprite.texture.Width, intersectingSprite.texture.Height);
            Rectangle intersectingRec = new Rectangle((int)intersectingSprite.vector.X - (intersectingSprite.getTexture().Width / 2), (int)intersectingSprite.vector.Y - (intersectingSprite.getTexture().Height / 2), intersectingSprite.texture.Width, intersectingSprite.texture.Height);
            if (originalRec.Intersects(intersectingRec))
            {
                return true;
            }
            return false;
        }

        public bool IntersectsPixel(Sprite intersectingSprite)
        {
            if (intersectingSprite == this)
            {
                return false;
            }
            Rectangle rec1 = new Rectangle((int)vector.X, (int)vector.Y, texture.Width, texture.Height);
            //Rectangle rec2 = new Rectangle((int)intersectingSprite.vector.X, (int)intersectingSprite.vector.Y, intersectingSprite.texture.Width, intersectingSprite.texture.Height);
            Rectangle rec2 = new Rectangle((int)intersectingSprite.vector.X - (intersectingSprite.getTexture().Width / 2), (int)intersectingSprite.vector.Y - (intersectingSprite.getTexture().Height / 2), intersectingSprite.texture.Width, intersectingSprite.texture.Height);

            int top = Math.Max(rec1.Top, rec2.Top);
            int bottom = Math.Min(rec1.Bottom, rec2.Bottom);
            int left = Math.Max(rec1.Left, rec2.Left);
            int right = Math.Min(rec1.Right, rec2.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color color1 = textureData[(x - rec1.Left) + (y - rec1.Top) * rec1.Width];
                    Color color2 = intersectingSprite.textureData[(x - rec2.Left) + (y - rec2.Top) * rec2.Width];

                    if (color1.A != 0 && color2.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IntersectsPixelZombie(Sprite intersectingSprite)
        {
            if (intersectingSprite == this)
            {
                return false;
            }
            Rectangle rec1 = new Rectangle((int)vector.X, (int)vector.Y, texture.Width, texture.Height);
            //Rectangle rec2 = new Rectangle((int)intersectingSprite.vector.X, (int)intersectingSprite.vector.Y, intersectingSprite.texture.Width, intersectingSprite.texture.Height);
            Rectangle rec2 = new Rectangle((int)intersectingSprite.vector.X, (int)intersectingSprite.vector.Y, intersectingSprite.texture.Width, intersectingSprite.texture.Height);

            int top = Math.Max(rec1.Top, rec2.Top);
            int bottom = Math.Min(rec1.Bottom, rec2.Bottom);
            int left = Math.Max(rec1.Left, rec2.Left);
            int right = Math.Min(rec1.Right, rec2.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color color1 = textureData[(x - rec1.Left) + (y - rec1.Top) * rec1.Width];
                    Color color2 = intersectingSprite.textureData[(x - rec2.Left) + (y - rec2.Top) * rec2.Width];

                    if (color1.A != 0 && color2.A != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
