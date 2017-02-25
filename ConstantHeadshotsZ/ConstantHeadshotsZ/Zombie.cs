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
            //Vector2 oldVector = sprite.vector;
            //sprite.setX(x);
            //Sprite uncollidable = level.zombieCollidesWithUncollidablePixel(this, oldVector);
            //if (uncollidable != null)
            //{
            //    Sprite collisionSolid = (Sprite)uncollidable;
            //    if (oldVector.X > x)
            //    {
            //        sprite.setX(collisionSolid.getX() + collisionSolid.getTexture().Width * 1.5f);
            //    }
            //    else if (oldVector.X < x)
            //    {
            //        sprite.setX(collisionSolid.getX() - collisionSolid.getTexture().Width / 2);
            //    }
            //}
            Vector2 oldVector = sprite.vector;
            sprite.setX(x);
            int ihat = (int)(Math.Abs(oldVector.X - x) / (oldVector.X - x));
            Zombie zUnc = level.zombieCollidesWithZombies(this);
            List<Zombie> detected = new List<Zombie>();
            detected.Add(zUnc);
            while (zUnc != null)
            {
                float newPos = (float)Math.Ceiling(Math.Sqrt(Math.Abs(Math.Pow(sprite.getY() - zUnc.sprite.getY(), 2) - Math.Pow(sprite.getTexture().Width / 2 + zUnc.sprite.getTexture().Width / 2, 2))));
                sprite.setX(zUnc.sprite.getX() + ihat * newPos);
                zUnc = level.zombieCollidesWithZombies(this);
                if (detected.Contains(zUnc))
                {
                    zUnc = null;
                }
                detected.Add(zUnc);

                if (oldVector.X * ihat < sprite.getX() * ihat)
                {
                    sprite.vector = oldVector;
                    break;
                }
            }

            //sprite.setX(x);
            Solid uncollidableS = level.zombieCollidesWithSolidsPixel(this, oldVector);
            while (uncollidableS != null)
            {
                Sprite uncollidable = uncollidableS.sprite;
                Sprite collisionSolid = (Sprite)uncollidable;
                if (oldVector.X > x)
                {
                    sprite.setX(collisionSolid.getX() + collisionSolid.getTexture().Width * 1.5f);
                }
                else if (oldVector.X < x)
                {
                    sprite.setX(collisionSolid.getX() - collisionSolid.getTexture().Width / 2);
                }
                uncollidableS = level.zombieCollidesWithSolidsPixel(this, oldVector);
            }
        }

        public void SetY(float y, Level level)
        {
            Vector2 oldVector = sprite.vector;
            sprite.setY(y);
            int jhat = (int) (Math.Abs(oldVector.Y - y) / (oldVector.Y - y));
            Zombie zUnc = level.zombieCollidesWithZombies(this);
            List<Zombie> detected = new List<Zombie>();
            detected.Add(zUnc);
            while (zUnc != null)
            {
                float newPos = (float)Math.Ceiling(Math.Sqrt(Math.Abs(Math.Pow(sprite.getX() - zUnc.sprite.getX(), 2) - Math.Pow(sprite.getTexture().Width / 2 + zUnc.sprite.getTexture().Width / 2, 2))));
                sprite.setY(zUnc.sprite.getY() + jhat * newPos);
                zUnc = level.zombieCollidesWithZombies(this);
                if (detected.Contains(zUnc))
                {
                    zUnc = null;
                }
                detected.Add(zUnc);

                if (oldVector.Y * jhat < sprite.getY() * jhat)
                {
                    sprite.vector = oldVector;
                    break;
                }
            }

            //sprite.setY(y);
            Solid uncollidableS = level.zombieCollidesWithSolidsPixel(this, oldVector);
            while (uncollidableS != null)
            {
                Sprite uncollidable = uncollidableS.sprite;
                Sprite collisionSolid = (Sprite)uncollidable;
                if (oldVector.Y > y)
                {
                    sprite.setY(collisionSolid.getY() + collisionSolid.getTexture().Height * 1.5f);
                }
                else if (oldVector.Y < y)
                {
                    sprite.setY(collisionSolid.getY() - collisionSolid.getTexture().Height / 2);
                }
                uncollidableS = level.zombieCollidesWithSolidsPixel(this, oldVector);
            }
        }
    }
}
