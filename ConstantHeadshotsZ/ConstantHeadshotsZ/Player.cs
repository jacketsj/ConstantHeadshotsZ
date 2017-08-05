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
        public Weapon weapon = Weapon.ROCKETLAUNCHER;
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
                if (oldX < x)
                {
                    sprite.setX(collisionSolid.sprite.getX() - collisionSolid.sprite.getTexture().Width / 2);
                }
                else if (oldX > x)
                {
                    sprite.setX(collisionSolid.sprite.getX() + collisionSolid.sprite.getTexture().Width * 1.5f);
                }
            }
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
                }
                else if (oldY > y)
                {
                    sprite.setY(collisionSolid.sprite.getY() + collisionSolid.sprite.getTexture().Height * 1.5f);
                }
            }
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
