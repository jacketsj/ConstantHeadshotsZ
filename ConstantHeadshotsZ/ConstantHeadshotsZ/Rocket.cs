using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZ
{
    public class Rocket
    {
        public Sprite sprite;
        public float rotation = 0f;
        private ContentManager Content;
        public Vector2 origin;
        public Vector2 spriteOrigin;
        public Vector2 centrepos;
        Rectangle collisionRec;
        Vector2 realToCollisionDifference;
        public int exploding = 0;

        public Rocket(ContentManager Content, Vector2 newVector)
        {
            sprite = new Sprite(Content.Load<Texture2D>("RocketSmall"), newVector);
            this.Content = Content;
            origin = new Vector2(0, 58);
            spriteOrigin = new Vector2(64, 58);
            centrepos = new Vector2(1, 2);
            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(-(origin), 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(sprite.vector, 0.0f));
            collisionRec = CalculateBoundingRectangle(new Rectangle(0, 0, sprite.getTexture().Width, sprite.getTexture().Height), bulletTransform);
            realToCollisionDifference = new Vector2(newVector.X - collisionRec.X, newVector.Y - collisionRec.Y);
        }

        public Rocket(ContentManager Content, Vector2 newVector, float newRotation)
        {
            sprite = new Sprite(Content.Load<Texture2D>("RocketSmall"), newVector);
            rotation = newRotation;
            this.Content = Content;
            origin = new Vector2(0, 58);
            spriteOrigin = new Vector2(64, 58);
            centrepos = new Vector2(1, 2);
            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(-(origin), 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(sprite.vector, 0.0f));
            collisionRec = CalculateBoundingRectangle(new Rectangle(0, 0, sprite.getTexture().Width, sprite.getTexture().Height), bulletTransform);
            realToCollisionDifference = new Vector2(newVector.X - collisionRec.X, newVector.Y - collisionRec.Y);
        }

        public void UpdateCollisionRec()
        {
            if (exploding == 0)
            {
                collisionRec.X = (int)sprite.getX() - (int)realToCollisionDifference.X;
                collisionRec.Y = (int)sprite.getY() - (int)realToCollisionDifference.Y;
            }
            else
            {
                collisionRec.X = ((int)sprite.getX()) - (int)realToCollisionDifference.X - 64;
                collisionRec.Y = ((int)sprite.getY()) - (int)realToCollisionDifference.Y - 64;
                collisionRec.Width = 128;
                collisionRec.Height = 128;
            }
        }

        public bool Update(Level level, Player player)
        {
            Vector2 direction = new Vector2((float)Math.Cos(rotation + 300), (float)Math.Sin(rotation + 300));
            if (exploding == 0)
            {
                sprite.vector += direction * 11;
            }
            UpdateCollisionRec();
            bool collidedWithZombies = CollidesWithZombies(level);
            if (collidedWithZombies)
            {
                player.score += 10;
            }
            if ((CollidesWithSolids(level.solids) || collidedWithZombies || isOutOfBounds(level)) && exploding == 0)
            {
                exploding = 50;
                Vector2 location = sprite.vector;
                location.X += 64 * (float)Math.Cos(rotation - Math.PI / 2);
                location.Y += 64 * (float)Math.Sin(rotation - Math.PI / 2);
                level.bloodParticles = level.GenerateBurst(level.bloodParticles, Color.Yellow, new Vector3(location - centrepos, 0), Content, 50, 50,
                                                            Options.GetInstance().minLParticles, Options.GetInstance().maxLParticles, 2.6f, 2.7f);
            }
            else if (exploding == 0)
            {
                Vector2 location = sprite.vector;
                location.X += 64 * (float)Math.Cos(rotation - Math.PI / 2);
                location.Y += 64 * (float)Math.Sin(rotation - Math.PI / 2);
                level.bloodParticles = level.GenerateBurst(level.bloodParticles, Color.Yellow, new Vector3(location - centrepos, 0), Content, 14, 14, 10, 20, 1, 1,
                                                                        rotation - MathHelper.PiOver4 / 2 - MathHelper.PiOver2, rotation + MathHelper.PiOver4 / 2 - MathHelper.PiOver2, 0, 0);
            }
            if (exploding == 1)
            {
                return true;
            }
            if (exploding > 1)
            {
                exploding -= 1;
            }
            return false;
        }

        private bool isOutOfBounds(Level level)
        {
            if ((sprite.vector.X < 0 - sprite.getTexture().Width)  || (sprite.vector.X > level.levelWidth) || (sprite.vector.Y < 0 - sprite.getTexture().Height) || (sprite.vector.Y > level.levelHeight))
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            if (exploding == 0)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("Rocket"), sprite.vector, null, sprite.getTint(), rotation,
                                 spriteOrigin,/*new Vector2(16 * sprite.getTexture().Width / 2, 16 * sprite.getTexture().Height / 2),*/ 1f, SpriteEffects.None, 0f);
            }
        }

        private bool CollidesWithSolids(Solid[] solids)
        {
            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(-(origin), 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(sprite.vector, 0.0f));
            Rectangle bulletRectangle = CalculateBoundingRectangle(new Rectangle(0, 0, sprite.getTexture().Width, sprite.getTexture().Height), bulletTransform);
            foreach (Solid solid in solids)
            {
                if (CollidesWithSolidRotatedRec(solid, bulletRectangle))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CollidesWithZombies(Level level)
        {
            for (int i = 0; i < level.zombies.Length; i++)
            {
                if (CollidesWithZombie(level.zombies[i], collisionRec))
                {
                    level.zombies[i].health -= 100;
                    level.bloodParticles = level.GenerateBurst(level.bloodParticles, CHZ.options.bldCol, new Vector3(level.zombies[i].sprite.vector, 0), Content, 60, 360,
                                                                Options.GetInstance().minLParticles, Options.GetInstance().maxLParticles, 0.1f, 2.7f);
                    return true;
                }
            }
            return false;
        }

        public bool CollidesWithZombie(Zombie zombie, Matrix bulletTransform, Rectangle bulletRectangle)
        {
            Vector2 zombieOrigin = new Vector2(zombie.sprite.getTexture().Width / 2, zombie.sprite.getTexture().Height / 2);

            Matrix zombieTransform = Matrix.CreateTranslation(new Vector3(-(zombieOrigin), 0.0f)) * Matrix.CreateTranslation(new Vector3(zombie.sprite.vector, 0.0f));
            
            Rectangle zombieRectangle = new Rectangle((int)zombie.sprite.vector.X - zombie.sprite.getTexture().Width / 2, (int)zombie.sprite.vector.Y - zombie.sprite.getTexture().Height / 2, zombie.sprite.getTexture().Width, zombie.sprite.getTexture().Height);
            

            sprite.UpdateTextureData();
            zombie.sprite.UpdateTextureData();

            if (bulletRectangle.Intersects(zombieRectangle))
            {
                if (IntersectPixels(bulletTransform, sprite.getTexture().Width, sprite.getTexture().Height, sprite.textureData, zombieTransform, zombie.sprite.getTexture().Width, zombie.sprite.getTexture().Height, zombie.sprite.textureData))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CollidesWithZombie(Zombie zombie, Rectangle bulletRectangle)
        {
            Rectangle zombieRectangle = new Rectangle((int)zombie.sprite.vector.X - zombie.sprite.getTexture().Width / 2, (int)zombie.sprite.vector.Y - zombie.sprite.getTexture().Height / 2, zombie.sprite.getTexture().Width, zombie.sprite.getTexture().Height);
            if (zombieRectangle.Intersects(bulletRectangle))
            {
                return true;
            }
            return false;
        }

        public bool CollidesWithSolidRotatedRec(Solid solid, Rectangle bulletRectangle)
        {
            Rectangle solidRectangle = new Rectangle((int)solid.sprite.vector.X, (int)solid.sprite.vector.Y, solid.sprite.getTexture().Width, solid.sprite.getTexture().Height);
            if (solidRectangle.Intersects(bulletRectangle))
            {
                return true;
            }
            return false;
        }

        private bool CollidesWithSolid(Solid solid)
        {
            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(-(origin), 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(sprite.vector, 0.0f));
            
            Rectangle bulletRectangle = CalculateBoundingRectangle(new Rectangle(0, 0, sprite.getTexture().Width, sprite.getTexture().Height), bulletTransform);
            
            Matrix solidTransform = Matrix.CreateTranslation(new Vector3(solid.sprite.vector, 0.0f));
            
            Rectangle solidRectangle = new Rectangle((int)solid.sprite.vector.X, (int)solid.sprite.vector.Y, solid.sprite.getTexture().Width, solid.sprite.getTexture().Height);
            

            sprite.UpdateTextureData();
            solid.sprite.UpdateTextureData();

            if(bulletRectangle.Intersects(solidRectangle))
            {
                if (IntersectPixels(bulletTransform, sprite.getTexture().Width, sprite.getTexture().Height, sprite.textureData, solidTransform, solid.sprite.getTexture().Width, solid.sprite.getTexture().Height, solid.sprite.textureData))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB) * 3;
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB) * 3;

            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < heightA; yA+=3)
            {
                Vector2 posInB = yPosInB;

                for (int xA = 0; xA < widthA; xA+=3)
                {
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    if (0 <= xB && xB < widthB && 0 <= yB && yB < heightB)
                    {
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            return true;
                        }
                    }
                    posInB += stepX;
                }
                yPosInB += stepY;
            }
            return false;
        }

        private static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }
    }
}
