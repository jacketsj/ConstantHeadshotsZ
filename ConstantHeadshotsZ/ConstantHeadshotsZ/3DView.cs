using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZ
{
    public class _3DView
    {
        Vector3 cameraPosition = new Vector3(0, 0, 0);
        Vector3 cameraForward = Vector3.Forward;
        Vector3 cameraUp = Vector3.Up;
        QuadDrawer quadDrawer;

        public _3DView(GraphicsDevice device)
        {
            quadDrawer = new QuadDrawer(device);
        }

        public void Draw(GraphicsDevice device, Level level, Player[] players, bool inTwoPlayer, int playerNo, ContentManager Content)
        {
            device.Clear(Color.CornflowerBlue);

            device.BlendState = BlendState.AlphaBlend;

            cameraPosition.X = players[playerNo - 1].sprite.getX();
            cameraPosition.Y = players[playerNo - 1].sprite.getY();

            cameraForward = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateRotationY(players[playerNo - 1].playerRotation));

            Matrix view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, device.Viewport.AspectRatio, 1, 100000);

            Matrix groundTransform = Matrix.CreateScale(level.levelWidth, level.levelHeight, 0) * Matrix.CreateRotationX(MathHelper.PiOver2);

            quadDrawer.DrawQuad(level.background, 1, groundTransform, view, projection);
            
            foreach (Drop drop in level.drops)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = drop.sprite.getTexture();
                sprite.Position = new Vector3(drop.sprite.vector.X, 0, drop.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (Solid solid in level.solids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, 0, solid.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (Vector2 spawner in level.zombieSpawners)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = Content.Load<Texture2D>("ZombieSpawner");
                sprite.Position = new Vector3(spawner.X, 0, spawner.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (Particle particle in level.bloodParticles)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = particle.sprite.getTexture();
                sprite.Position = new Vector3(particle.sprite.vector.X, 0, particle.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (Zombie zombie in level.zombies)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = zombie.sprite.getTexture();
                sprite.Position = new Vector3(zombie.sprite.vector.X, 0, zombie.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (BasicWeaponBullet bullet in level.basicWeaponBullets)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = bullet.sprite.getTexture();
                sprite.Position = new Vector3(bullet.sprite.vector.X, 0, bullet.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            foreach (Rocket bullet in level.rockets)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = bullet.sprite.getTexture();
                sprite.Position = new Vector3(bullet.sprite.vector.X, 0, bullet.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }

            if (inTwoPlayer)
            {
                foreach (Player player in players)
                {
                    _3DSprite sprite = new _3DSprite();
                    sprite.Texture = player.sprite.getTexture();
                    sprite.Position = new Vector3(player.sprite.vector.X, 0, player.sprite.vector.Y);
                    sprite.Draw(quadDrawer, cameraPosition, view, projection);
                }
            }
            else
            {
                Player player = players[0];
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = player.sprite.getTexture();
                sprite.Position = new Vector3(player.sprite.vector.X, 0, player.sprite.vector.Y);
                sprite.Draw(quadDrawer, cameraPosition, view, projection);
            }
        }
    }
}
