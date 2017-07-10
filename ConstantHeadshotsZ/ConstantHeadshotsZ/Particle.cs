using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZ
{
    public class Particle
    {
        public Sprite sprite;
        public Vector3 direction;
        public float posZ = 0;
        public TimeSpan timeUntilDissapearance;

        public Particle(Color color, Vector2 position, Vector3 newDirection, ContentManager Content, int timeUntilDissapearance)
        {
            sprite = new Sprite(Content.Load<Texture2D>("Particle"), position, color);
            this.timeUntilDissapearance = new TimeSpan(0, 0, 0, 0, timeUntilDissapearance * 16);
            this.direction = newDirection;
        }

        public Particle(Color color, Vector3 position, Vector3 newDirection, ContentManager Content, int timeUntilDissapearance)
        {
            sprite = new Sprite(Content.Load<Texture2D>("Particle"), new Vector2(position.X, position.Y), color);
            posZ = position.Z;
            this.timeUntilDissapearance = new TimeSpan(0, 0, 0, 0, timeUntilDissapearance * 16);
            this.direction = newDirection;
        }

        public bool Update(TimeSpan elapsedTime)
        {
            sprite.vector.X += direction.X * 11 * elapsedTime.Milliseconds / 17;
            sprite.vector.Y += direction.Y * 11 * elapsedTime.Milliseconds / 17;
            posZ += direction.Z * 11 * elapsedTime.Milliseconds * 17;
            direction = (direction / 3) * 2;
            timeUntilDissapearance -= elapsedTime;
            if (timeUntilDissapearance <= TimeSpan.Zero)
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
