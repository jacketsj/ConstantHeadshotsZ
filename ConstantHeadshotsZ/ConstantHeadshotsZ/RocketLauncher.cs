using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZ
{
    public class RocketLauncher
    {
        public static int delay = 90;

        public Sprite sprite;
        private Player player;
        private float rotation;
        private Vector2 origin;

        public RocketLauncher(ContentManager Content, Sprite sprite, Player player)
        {
            this.player = player;
            this.sprite = sprite;
            this.sprite.setTexture(Content.Load<Texture2D>("RocketLauncher"));
            rotation = player.playerRotation;
            sprite.setX(player.sprite.getX());
            sprite.setY(player.sprite.getY());
            rotation = player.playerRotation;
            origin = new Vector2(64, 64);
        }

        public void Update(ContentManager Content, Level level)
        {
            sprite.setX(player.sprite.getX());
            sprite.setY(player.sprite.getY());
            rotation = player.playerRotation;
        }

        public void Attack(ContentManager Content, Level level, Player player)
        {
            Rocket[] newRockets = new Rocket[level.rockets.Length + 1];
            int i;
            for (i = 0; i < level.rockets.Length; i++)
            {
                newRockets[i] = level.rockets[i];
            }
            newRockets[i] = new Rocket(Content, player.sprite.vector, player.playerRotation);
            level.rockets = newRockets;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), rotation, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
