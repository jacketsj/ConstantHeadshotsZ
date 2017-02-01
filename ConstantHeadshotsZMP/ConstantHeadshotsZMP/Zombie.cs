using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZMP
{
    public class Zombie
    {
        public Sprite sprite;
        public float rotation;
        public int health = 100;
        public SpriteFont Font;
        Player followedPlayer;
        Random random;

        public Zombie(Sprite newSprite, float newRotation, ContentManager Content, Player[] players)
        {
            sprite = newSprite;
            rotation = newRotation;
            Font = Content.Load<SpriteFont>("basic");
            random = new Random();
            followedPlayer = players[random.Next(0, players.Length)];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), rotation, new Vector2(sprite.getTexture().Width / 2, sprite.getTexture().Height / 2), 1f, SpriteEffects.None, 0f);
        }

        public void Update(Player[] players, Level level, int index)
        {
            float playerLocationX = followedPlayer.sprite.getX() + followedPlayer.sprite.getTexture().Width / 2;
            float playerLocationY = followedPlayer.sprite.getY() + followedPlayer.sprite.getTexture().Height / 2;
            bool allDead = true;

            foreach (Player player in players)
            {
                if (player.isDead)
                {
                    allDead = false;
                }
            }

            if (!allDead)
            {
                while (followedPlayer.isDead)
                {
                    followedPlayer = players[random.Next(0, players.Length)];
                }
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

        /*
        public void Update(Player player, Level level, int index)
        {
            float playerLocationX = player.sprite.getX() + player.sprite.getTexture().Width / 2;
            float playerLocationY = player.sprite.getY() + player.sprite.getTexture().Height / 2;
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
        */

        public void SetX(float x, Level level)
        {
            Vector2 oldVector = sprite.vector;
            sprite.setX(x);
            if (level.zombieCollidesWithUncollidablePixel(this))
            {
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
            }
        }

        public void SetY(float y, Level level)
        {
            Vector2 oldVector = sprite.vector;
            sprite.setY(y);
            if (level.zombieCollidesWithUncollidablePixel(this))
            {
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
            }
        }
    }
}
