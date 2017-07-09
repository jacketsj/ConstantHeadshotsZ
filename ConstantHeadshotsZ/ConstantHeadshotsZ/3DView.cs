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
        Vector3 cameraPosition = new Vector3(0, 0, 20);
        Vector3 cameraForward = new Vector3(0, 0, -1);
        Vector3 cameraUp = Vector3.Up;
        QuadDrawer quadDrawer;

        public _3DView(GraphicsDevice device)
        {
            quadDrawer = new QuadDrawer(device);
        }

        public void Draw(GraphicsDevice device, Level level, Player[] players, bool inTwoPlayer, int playerNo, ContentManager Content, Options options)
        {
            device.Clear(Color.CornflowerBlue);

            device.BlendState = BlendState.AlphaBlend;

            //cameraPosition.X = (players[playerNo - 1].sprite.getX());
            //cameraPosition.Y = (-players[playerNo - 1].sprite.getY());

            //cameraForward = Vector3.TransformNormal(Vector3.Forward, Matrix.CreateRotationY(players[playerNo - 1].playerRotation));

            if (options.player1CameraRotation)
            {
                //cameraUp = Matrix.CreateRotationZ(players[playerNo - 1].playerRotation + MathHelper.Pi).Up;
            }
            Matrix view = players[playerNo - 1].camera.get_transformation_3d(device) * Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);//Matrix.CreateScale(1, -1, 1) * 

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI * 63 / 65, device.Viewport.AspectRatio, 0.001f, 20);//Matrix.CreatePerspectiveFieldOfView(0.7853982f, device.Viewport.AspectRatio, 1, 2);

            Matrix groundTransform = Matrix.CreateScale(1, -1, 1) * Matrix.CreateRotationX(MathHelper.Pi);// *Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateRotationZ(MathHelper.Pi);// *Matrix.CreateRotationY(MathHelper.Pi);//Matrix.CreateScale(level.levelWidth, level.levelHeight, 0);// *Matrix.CreateRotationX(MathHelper.PiOver2);// *Matrix.CreateRotationX(MathHelper.PiOver2);

            quadDrawer.DrawQuad(level.background, 1, groundTransform * Matrix.CreateScale((float)level.levelWidth / level.background.Width, (float)level.levelHeight / level.background.Height, 0), view, projection);
            //quadDrawer.DrawQuad(level.background, 1, Matrix.CreateScale(level.levelWidth, level.levelHeight, 0), view, projection);
            
            foreach (Drop drop in level.drops)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = drop.sprite.getTexture();
                sprite.Position = new Vector3(drop.sprite.vector.X, drop.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Solid solid in level.solids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, solid.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Solid solid in level.backSolids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, solid.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Vector2 spawner in level.zombieSpawners)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = Content.Load<Texture2D>("ZombieSpawner");
                sprite.Position = new Vector3(spawner.X, spawner.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Particle particle in level.bloodParticles)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = particle.sprite.getTexture();
                sprite.Position = new Vector3(particle.sprite.vector.X, particle.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Zombie zombie in level.zombies)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = zombie.sprite.getTexture();
                sprite.Position = new Vector3(zombie.sprite.vector.X, zombie.sprite.vector.Y, 0);
                sprite.Up = new Vector3(0, 0, zombie.rotation);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (BasicWeaponBullet bullet in level.basicWeaponBullets)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = bullet.sprite.getTexture();
                sprite.Position = new Vector3(bullet.sprite.vector.X, bullet.sprite.vector.Y, 0);
                sprite.Up = new Vector3(0, 0, bullet.rotation);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Rocket bullet in level.rockets)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = bullet.sprite.getTexture();
                sprite.Position = new Vector3(bullet.sprite.vector.X, bullet.sprite.vector.Y, 0);
                sprite.Up = new Vector3(0, 0, bullet.rotation);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            if (inTwoPlayer)
            {
                foreach (Player player in players)
                {
                    //_3DSprite sprite = new _3DSprite();
                    //sprite.Texture = player.sprite.getTexture();
                    //sprite.Position = new Vector3(player.sprite.vector.X, player.sprite.vector.Y, 0);
                    //sprite.Up = new Vector3(0, 0, player.playerRotation);
                    //sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
                    DrawPlayer(player, Content, quadDrawer, cameraPosition, view, projection, groundTransform);
                }
            }
            else
            {
                //Player player = players[0];
                //_3DSprite sprite = new _3DSprite();
                //sprite.Texture = player.sprite.getTexture();
                //sprite.Position = new Vector3(player.sprite.vector.X, player.sprite.vector.Y, 0);
                //sprite.Up = new Vector3(0, 0, player.playerRotation);
                //sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
                //sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
                DrawPlayer(players[0], Content, quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Solid solid in level.foreSolids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, solid.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }
        }

        private void DrawPlayer(Player player, ContentManager Content, QuadDrawer quadDrawer, Vector3 cameraPosition, Matrix view, Matrix projection, Matrix groundTransform)
        {
            _3DSprite playerSprite = new _3DSprite();
            playerSprite.Texture = player.sprite.getTexture();
            playerSprite.Position = new Vector3(player.sprite.vector, 0);
            playerSprite.Up = new Vector3(0, 0, player.playerRotation);
            playerSprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            _3DSprite weaponSprite = new _3DSprite();
            Sprite weapon2DSprite = GetPlayerWeaponTexture(player, Content);
            weaponSprite.Texture = weapon2DSprite.getTexture();
            weaponSprite.Position = new Vector3(weapon2DSprite.vector, 0);
            weaponSprite.Up = new Vector3(0, 0, player.playerRotation);
            weaponSprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
        }

        private Sprite GetPlayerWeaponTexture(Player player, ContentManager Content)
        {
            Player.Weapon weapon = player.weapon;
            if (weapon == Player.Weapon.BASIC)
            {
                //Vector2 weaponVector = sprite.vector;
                return new Sprite(Content.Load<Texture2D>("BasicWeapon"), player.sprite.vector);
            }
            if (weapon == Player.Weapon.LAZERSWORD)
            {
                return player.lazerSword.sprite;
            }
            if (weapon == Player.Weapon.HAMMER)
            {
                return player.hammer.sprite;
            }
            if (weapon == Player.Weapon.ROCKETLAUNCHER)
            {
                return player.rocketLauncher.sprite;
            }
            return null;
        }
    }
}
