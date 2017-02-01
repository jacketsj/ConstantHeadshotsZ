using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using LevelBuilder;

namespace ConstantHeadshotsZMP
{
    public class RocketLauncher
    {
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

        public void Attack(ContentManager Content, Level level, Player player, ClientInfo clientInfo)
        {
            Rocket[] newRockets = new Rocket[level.rockets.Length + 1];
            int i;
            for (i = 0; i < level.rockets.Length; i++)
            {
                newRockets[i] = level.rockets[i];
            }
            newRockets[i] = new Rocket(Content, player.sprite.vector, player.playerRotation);
            level.rockets = newRockets;

            RocketData[] newInfoRockets = new RocketData[clientInfo.newRockets.Length + 1];
            for (int i2 = 0; i2 < clientInfo.newRockets.Length; i2++)
            {
                newInfoRockets[i2] = new RocketData();
                newInfoRockets[i2].position = clientInfo.newRockets[i2].position;
                newInfoRockets[i2].rotation = clientInfo.newRockets[i2].rotation;
                newInfoRockets[i2].exploding = clientInfo.newRockets[i2].exploding;
            }
            newInfoRockets[clientInfo.newRockets.Length] = new RocketData();
            newInfoRockets[clientInfo.newRockets.Length].exploding = newRockets[i].exploding;
            newInfoRockets[clientInfo.newRockets.Length].position = newRockets[i].sprite.vector;
            newInfoRockets[clientInfo.newRockets.Length].rotation = newRockets[i].rotation;
            clientInfo.newRockets = newInfoRockets;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            //spriteBatch.Draw(Content.Load<Texture2D>("White"), HitBox, Color.Red);
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), rotation, origin, 1f, SpriteEffects.None, 0f);
            //HitBoxRot.TestDraw(spriteBatch, Content);
        }
    }
}
