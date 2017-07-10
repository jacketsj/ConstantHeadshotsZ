using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZ
{
    public class Player
    {
        public Sprite sprite;
        public float playerRotation;
        public Camera camera;
        public bool aiming = false;
        public Weapon weapon = Weapon.LAZERSWORD;
        public int delay = 0;
        public int health = 100;
        public int delayUntilHit = 0;
        public float score = 0;
        public bool isDead = false;
        public LazerSword lazerSword;
        public Hammer hammer;
        public RocketLauncher rocketLauncher;

        public enum Weapon
        {
            NONE, BASIC, LAZERSWORD, HAMMER, ROCKETLAUNCHER
        }

        public Player(Sprite newSprite, float newPlayerRotation, ContentManager Content, GraphicsDevice graphics)
        {
            sprite = newSprite;
            playerRotation = newPlayerRotation;
            camera = new Camera(sprite, graphics);
            lazerSword = new LazerSword(Content, new Sprite(Content.Load<Texture2D>("LazerSwordStill"), sprite.vector), this);
            hammer = new Hammer(Content, new Sprite(Content.Load<Texture2D>("HammerStill"), sprite.vector), this);
            rocketLauncher = new RocketLauncher(Content, new Sprite(Content.Load<Texture2D>("RocketLauncher"), sprite.vector), this);
        }

        public void SetX(float x, Level level)
        {
            Vector2 oldVector = sprite.vector;
            float oldX = sprite.getX();
            sprite.setX(x);

            Solid collisionSolid = level.playerCollidesWithSolidsPixel(this);

            if (collisionSolid != null)
            {
                /*
                if (sprite.getY() <= collisionSolid.sprite.getY() - collisionSolid.sprite.getTexture().Height / 2 && sprite.getY() >= collisionSolid.sprite.getY() + collisionSolid.sprite.getTexture().Height / 2)
                {
                */
                /*
                int space = 0;
                if (sprite.getY() < collisionSolid.sprite.getY())
                {
                    space = (int)((collisionSolid.sprite.getY() - collisionSolid.sprite.getTexture().Height / 2) - sprite.getY());
                }
                if (sprite.getY() > collisionSolid.sprite.getY())
                {
                    space = (int)((collisionSolid.sprite.getY() + collisionSolid.sprite.getTexture().Height / 2) - sprite.getY());
                }
                */
                    if (oldX < x)
                    {
                        //int thingy = ((int)Math.Sqrt(Math.Pow(sprite.getTexture().Width / 2, 2) - Math.Pow(space, 2)));
                        //sprite.setX(collisionSolid.sprite.getX() - thingy);
                        sprite.setX(collisionSolid.sprite.getX() - collisionSolid.sprite.getTexture().Width / 2);
                        //sprite.setX(sprite.vector.X = collisionSolid.sprite.vector.X - (float)Math.Sqrt((float)Math.Pow(collisionSolid.sprite.getTexture().Width, 2) - (float)Math.Pow(collisionSolid.sprite.vector.Y - sprite.vector.Y, 2)) /*+ sprite.getTexture().Width / 2*/);
                    }
                    else if (oldX > x)
                    {
                        sprite.setX(collisionSolid.sprite.getX() + collisionSolid.sprite.getTexture().Width * 1.5f);
                        //sprite.setX(sprite.vector.X = collisionSolid.sprite.vector.X + collisionSolid.sprite.getTexture().Width + (float)Math.Sqrt((float)Math.Pow(collisionSolid.sprite.getTexture().Width, 2) - (float)Math.Pow(collisionSolid.sprite.vector.Y - sprite.vector.Y, 2)) /*- sprite.getTexture().Width / 2*/);
                    }
                /*
                }
                else if (sprite.getY() > collisionSolid.sprite.getY() - collisionSolid.sprite.getTexture().Height / 2)
                {
                    if (oldX < x)
                    {
                        sprite.setX(collisionSolid.sprite.getX() - ((float)Math.Sqrt(Math.Pow(sprite.getTexture().Width / 2, 2) - Math.Pow((collisionSolid.sprite.getY() - collisionSolid.sprite.getTexture().Height / 2) - (sprite.getX()), 2))));
                    }
                    else if (oldX > x)
                    {
                        sprite.setX(collisionSolid.sprite.getX() + collisionSolid.sprite.getTexture().Width * 1.5f);
                    }
                }
                else if (sprite.getY() < collisionSolid.sprite.getY() + collisionSolid.sprite.getTexture().Height / 2)
                {
                    if (oldX < x)
                    {
                        sprite.setX(collisionSolid.sprite.getX() - collisionSolid.sprite.getTexture().Width / 2);
                    }
                    else if (oldX > x)
                    {
                        sprite.setX(collisionSolid.sprite.getX() + collisionSolid.sprite.getTexture().Width * 1.5f);
                    }
                }
                 * */

                //collisionSolid = level.playerCollidesWithSolidsPixel(this);
            }

            /*
            if (level.playerCollidesWithSolidsPixel(this))
            {
                if (oldX > x)
                {
                    while (level.playerCollidesWithSolidsPixel(this))
                    {
                        sprite.setX(sprite.getX() + 1);
                    }
                }
                else if (oldX < x)
                {
                    while (level.playerCollidesWithSolidsPixel(this))
                    {
                        sprite.setX(sprite.getX() - 1);
                    }
                }
                else if (oldX == x)
                {

                }
            }
             * */
        }

        public void SetY(float y, Level level)
        {
            Vector2 oldVector = sprite.vector;
            float oldY = sprite.getY();
            sprite.setY(y);

            Solid collisionSolid = level.playerCollidesWithSolidsPixel(this);

            if (collisionSolid != null)
            {
                if (oldY < y)
                {
                    sprite.setY(collisionSolid.sprite.getY() - collisionSolid.sprite.getTexture().Height / 2);
                    //sprite.setY(sprite.vector.Y = collisionSolid.sprite.vector.Y + (float)Math.Sqrt((float)Math.Pow(collisionSolid.sprite.getTexture().Width, 2) - (float)Math.Pow(collisionSolid.sprite.vector.X - sprite.vector.X, 2)) - sprite.getTexture().Width / 2);
                }
                else if (oldY > y)
                {
                    sprite.setY(collisionSolid.sprite.getY() + collisionSolid.sprite.getTexture().Height * 1.5f);
                    //sprite.setY(sprite.vector.Y = collisionSolid.sprite.vector.Y + collisionSolid.sprite.getTexture().Height + (float)Math.Sqrt((float)Math.Pow(collisionSolid.sprite.getTexture().Width, 2) - (float)Math.Pow(collisionSolid.sprite.vector.X - sprite.vector.X, 2)) /*- sprite.getTexture().Width / 2*/);
                }

                //collisionSolid = level.playerCollidesWithSolidsPixel(this);
            }

            /*
            if (level.playerCollidesWithSolidsPixel(this))
            {
                if (oldY > y)
                {
                    while (level.playerCollidesWithSolidsPixel(this))
                    {
                        sprite.setY(sprite.getY() + 1);
                    }
                }
                else if (oldY < y)
                {
                    while (level.playerCollidesWithSolidsPixel(this))
                    {
                        sprite.setY(sprite.getY() - 1);
                    }
                }
                else if (oldY == y)
                {

                }
            }
            */
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), playerRotation, new Vector2(sprite.getTexture().Width / 2, sprite.getTexture().Height / 2), 1f, SpriteEffects.None, 0f);
            if (weapon == Weapon.BASIC)
            {
                Vector2 weaponVector = sprite.vector;
                spriteBatch.Draw(Content.Load<Texture2D>("BasicWeapon"), weaponVector, null, Color.White, playerRotation, new Vector2(Content.Load<Texture2D>("BasicWeapon").Width / 2, Content.Load<Texture2D>("BasicWeapon").Height / 2), 1f, SpriteEffects.None, 0f);
            }
            if (weapon == Weapon.LAZERSWORD)
            {
                lazerSword.Draw(spriteBatch, Content);
            }
            if (weapon == Weapon.HAMMER)
            {
                hammer.Draw(spriteBatch, Content);
            }
            if (weapon == Weapon.ROCKETLAUNCHER)
            {
                rocketLauncher.Draw(spriteBatch, Content);
            }
        }
    }
}
