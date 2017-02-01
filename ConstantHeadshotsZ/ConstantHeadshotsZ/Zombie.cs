using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZ
{
    public class Zombie
    {
        public Sprite sprite;
        public float rotation;
        public int health = 100;
        public SpriteFont Font;
        int followedPlayerIndex = 0;

        public Zombie(Sprite newSprite, float newRotation, ContentManager Content)
        {
            sprite = newSprite;
            rotation = newRotation;
            Font = Content.Load<SpriteFont>("basic");
            Random random = new Random();
            followedPlayerIndex = random.Next(0, 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), rotation, new Vector2(sprite.getTexture().Width / 2, sprite.getTexture().Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public void Update2Player(Player[] players, Level level, int index)
        {
            float playerLocationX = players[followedPlayerIndex].sprite.getX() + players[followedPlayerIndex].sprite.getTexture().Width / 2;
            float playerLocationY = players[followedPlayerIndex].sprite.getY() + players[followedPlayerIndex].sprite.getTexture().Height / 2;
            if (players[0].isDead && !players[1].isDead)
            {
                playerLocationX = players[1].sprite.getX() + players[1].sprite.getTexture().Width / 2;
                playerLocationY = players[1].sprite.getY() + players[1].sprite.getTexture().Height / 2;
            }
            if (!players[0].isDead && players[1].isDead)
            {
                playerLocationX = players[0].sprite.getX() + players[0].sprite.getTexture().Width / 2;
                playerLocationY = players[0].sprite.getY() + players[0].sprite.getTexture().Height / 2;
            }
            float zombieLocationX = sprite.getX() + sprite.getTexture().Width / 2;
            float zombieLocationY = sprite.getY() + sprite.getTexture().Height / 2;
            rotation = (float)Math.Atan2(zombieLocationY - playerLocationY, zombieLocationX - playerLocationX);
            zombieLocationX -= (float)(2 * Math.Cos(rotation));
            zombieLocationY -= (float)(2 * Math.Sin(rotation));
            rotation += 300;
            while (rotation > 359)
            {
                rotation -= 360;
            }
            while (rotation < 0)
            {
                rotation += 360;
            }
            SetX(zombieLocationX - sprite.getTexture().Width / 2, level);
            SetY(zombieLocationY - sprite.getTexture().Height / 2, level);
        }

        public void Update(Player player, Level level, int index)
        {
            float playerLocationX = player.sprite.getX();
            float playerLocationY = player.sprite.getY();
            //float zombieLocationX = sprite.getX() + sprite.getTexture().Width / 2;
            //float zombieLocationY = sprite.getY() + sprite.getTexture().Height / 2;
            float zombieLocationX = sprite.getX();
            float zombieLocationY = sprite.getY();
            rotation = (float)Math.Atan2(zombieLocationY - playerLocationY, zombieLocationX - playerLocationX);
            zombieLocationX -= (float)(2 * Math.Cos(rotation));
            zombieLocationY -= (float)(2 * Math.Sin(rotation));
            rotation += 300;
            while (rotation > 359)
            {
                rotation -= 360;
            }
            while (rotation < 0)
            {
                rotation += 360;
            }
            //SetX(zombieLocationX - sprite.getTexture().Width / 2, level);
            //SetY(zombieLocationY - sprite.getTexture().Height / 2, level);
            SetX(zombieLocationX, level);
            SetY(zombieLocationY, level);
        }

        public void SetX(float x, Level level)
        {
            Vector2 oldVector = sprite.vector;
            sprite.setX(x);
            Sprite uncollidable = level.zombieCollidesWithUncollidablePixel(this, oldVector);
            if (uncollidable != null)
            {
                //if (uncollidable.textureData[0].A <= 254)
                //{
                    Sprite collisionSolid = (Sprite)uncollidable;
                    if (oldVector.X > x)
                    {
                        sprite.setX(collisionSolid.getX() + collisionSolid.getTexture().Width * 1.5f);
                        //sprite.setX((collisionSolid.getX() + collisionSolid.getTexture().Width) + sprite.getTexture().Width / 2);
                        //sprite.setX(sprite.vector.X = collidingSprite.vector.X + (float)Math.Cos(Math.Atan2(collidingSprite.getY() - sprite.getY(), collidingSprite.getX() - sprite.getX())) * sprite.getTexture().Width);
                    }
                    else if (oldVector.X < x)
                    {
                        sprite.setX(collisionSolid.getX() - collisionSolid.getTexture().Width / 2);
                        //sprite.setX((collisionSolid.getX() - collisionSolid.getTexture().Width) + sprite.getTexture().Width / 2);
                        //sprite.setX(sprite.vector.X = collidingSprite.vector.X + (float)Math.Cos(Math.Atan2(collidingSprite.getY() - sprite.getY(), collidingSprite.getX() - sprite.getX())) * sprite.getTexture().Width + collidingSprite.getTexture().Width);
                    }
                /*
                }
                else
                {
                    Sprite collisionSolid = uncollidable;
                    if (oldVector.X < x)
                    {
                        sprite.setX(collisionSolid.getX() + collisionSolid.getTexture().Width / 2);
                        //sprite.setX(sprite.vector.X = collidingSprite.vector.X + (float)Math.Sqrt((float)Math.Pow(collidingSprite.getTexture().Width, 2) - (float)Math.Pow(collidingSprite.vector.Y - sprite.vector.Y, 2)) * 2 / 2);
                    }
                    else if (oldVector.X > x)
                    {
                        sprite.setX(collisionSolid.getX() - collisionSolid.getTexture().Width * 1.5f);
                        //sprite.setX(sprite.vector.X = collidingSprite.vector.X + collidingSprite.getTexture().Width - (float)Math.Sqrt((float)Math.Pow(collidingSprite.getTexture().Width, 2) - (float)Math.Pow(collidingSprite.vector.Y - sprite.vector.Y, 2)) * 2 / 2);
                    }
                }
                 * */
                /*
                if (oldVector.X > x)
                {
                    while (level.zombieCollidesWithUncollidablePixel(this))
                    {
                        sprite.setX(sprite.getX() + 1);
                    }
                }
                else if (oldVector.X < x)
                {
                    while (level.zombieCollidesWithUncollidablePixel(this))
                    {
                        sprite.setX(sprite.getX() - 1);
                    }
                }
                else if (oldVector.X == x)
                {

                }
                */
            }
        }

        public void SetY(float y, Level level)
        {
            Vector2 oldVector = sprite.vector;
            sprite.setY(y);
            Sprite uncollidable = level.zombieCollidesWithUncollidablePixel(this, oldVector);
            if (uncollidable != null)
            {
                //if (uncollidable.textureData[0].A <= 254)
                //{
                    Sprite collisionSolid = (Sprite)uncollidable;
                    if (oldVector.Y < y)
                    {
                        sprite.setY(collisionSolid.getY() - collisionSolid.getTexture().Height / 2);
                        //sprite.setY((collisionSolid.getY() - collisionSolid.getTexture().Height) + sprite.getTexture().Width / 2 + sprite.getTexture().Width * 2);
                        //sprite.setY(sprite.vector.Y = collidingSprite.vector.Y + (float)Math.Sin(Math.Atan2(collidingSprite.getY() - sprite.getY(), collidingSprite.getY() - sprite.getY())) * sprite.getTexture().Width);
                    }
                    else if (oldVector.Y > y)
                    {
                        sprite.setY(collisionSolid.getY() + collisionSolid.getTexture().Height * 1.5f);
                        //sprite.setY((collisionSolid.getY() + collisionSolid.getTexture().Height) + sprite.getTexture().Width / 2 - sprite.getTexture().Width * 2);
                        //sprite.setY(sprite.vector.Y = collidingSprite.vector.Y + (float)Math.Sin(Math.Atan2(collidingSprite.getY() - sprite.getY(), collidingSprite.getY() - sprite.getY())) * sprite.getTexture().Width + collidingSprite.getTexture().Width);
                    }
                /*
                }
                else
                {
                    Sprite collisionSolid = uncollidable;
                    if (oldVector.Y < y)
                    {
                        sprite.setY(collisionSolid.getY() - collisionSolid.getTexture().Height / 2);
                        //sprite.setY(sprite.vector.Y = collidingSprite.vector.Y + (float)Math.Sqrt((float)Math.Pow(collidingSprite.getTexture().Width, 2) - (float)Math.Pow(collidingSprite.vector.X - sprite.vector.X, 2)) * 2 / 2);
                    }
                    else if (oldVector.Y > y)
                    {
                        sprite.setY(collisionSolid.getY() + collisionSolid.getTexture().Height * 1.5f);
                        //sprite.setY(sprite.vector.Y = collidingSprite.vector.Y + collidingSprite.getTexture().Width - (float)Math.Sqrt((float)Math.Pow(collidingSprite.getTexture().Width, 2) - (float)Math.Pow(collidingSprite.vector.X - sprite.vector.X, 2)) * 2 / 2);
                    }
                }
                 * */
                /*
                if (oldVector.Y > y)
                {
                    while (level.zombieCollidesWithUncollidablePixel(this))
                    {
                        sprite.setY(sprite.getY() + 1);
                    }
                }
                else if (oldVector.Y < y)
                {
                    while (level.zombieCollidesWithUncollidablePixel(this))
                    {
                        sprite.setY(sprite.getY() - 1);
                    }
                }
                else if (oldVector.Y == y)
                {

                }
                */
            }
        }
    }
}
