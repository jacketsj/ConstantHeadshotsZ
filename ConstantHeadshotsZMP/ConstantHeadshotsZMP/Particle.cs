using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZMP
{
    public class Particle
    {
        public Sprite sprite;
        public Vector2 direction;
        public int timeUntilDissapearance;

        public Particle(Color color, Vector2 position, Vector2 newDirection, ContentManager Content, int timeUntilDissapearance)
        {
            sprite = new Sprite(Content.Load<Texture2D>("Particle"), position, color);
            this.timeUntilDissapearance = timeUntilDissapearance;
            this.direction = newDirection;
        }

        public bool Update()
        {
            sprite.vector += direction * 11;
            direction = (direction / 3) * 2;
            timeUntilDissapearance -= 1;
            if (timeUntilDissapearance <= 0)
            {
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, sprite.getTint());
        }
    }
}
