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
        Vector3 cameraPosition = new Vector3(0, 0, 500);
        Vector3 cameraForward = new Vector3(0, 0, -1);
        Vector3 cameraUp = Vector3.Up;
        QuadDrawer quadDrawer;

        public _3DView(GraphicsDevice device)
        {
            quadDrawer = new QuadDrawer(device);
        }

        public void Draw(GraphicsDevice device, Level level, Player[] players, bool inTwoPlayer, int playerNo, ContentManager Content, Options options)
        {
            device.Clear(Color.BlueViolet);

            device.BlendState = BlendState.AlphaBlend;


            Matrix view = players[playerNo - 1].camera.get_transformation_3d(device) * Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, cameraUp);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.Atan(Math.Max(options.screenWidth, options.screenHeight)/cameraPosition.Z)*1.07f,
                                                                device.Viewport.AspectRatio, 0.001f, 3000);

            Matrix groundTransform = Matrix.CreateScale(1, -1, 1) * Matrix.CreateRotationX(MathHelper.Pi);

            quadDrawer.DrawQuad(Sprite.AddTint(level.background, level.backgroundColor), 1, groundTransform * Matrix.CreateScale((float)level.levelWidth / 2, (float)level.levelHeight / 2, 0)
                                            * Matrix.CreateTranslation((float)level.levelWidth / 2, (float)level.levelHeight / 2, 0), view, projection);

            foreach (Solid solid in level.backSolids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, solid.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }
            
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

            foreach (Vector2 spawner in level.zombieSpawners)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = Content.Load<Texture2D>("ZombieSpawner");
                sprite.Position = new Vector3(spawner.X, spawner.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Zombie zombie in level.zombies)
            {
                _3DSprite sprite = new _3DSprite();

                sprite.Texture = zombie.sprite.getTintedTexture();
                sprite.Position = new Vector3(zombie.sprite.vector - new Vector2(zombie.sprite.getTexture().Width / 2, zombie.sprite.getTexture().Height / 2), 0);
                sprite.Up = new Vector3(0, 0, zombie.rotation);

                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (BasicWeaponBullet bullet in level.basicWeaponBullets)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = bullet.sprite.getTexture();
                sprite.Position = new Vector3(bullet.sprite.vector, 0) - Vector3.Transform(new Vector3(bullet.origin, 0), Matrix.CreateRotationZ(bullet.rotation));
                sprite.Up = new Vector3(0, 0, bullet.rotation);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Rocket bullet in level.rockets)
            {
                if (bullet.exploding == 0)
                {
                    _3DSprite sprite = new _3DSprite();
                    sprite.Texture = bullet.sprite.getTexture();
                    sprite.Position = new Vector3(bullet.sprite.vector, 0) - Vector3.Transform(new Vector3(bullet.origin, 0), Matrix.CreateRotationZ(bullet.rotation));
                    sprite.Up = new Vector3(0, 0, bullet.rotation);
                    sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
                }
            }

            if (inTwoPlayer)
            {
                foreach (Player player in players)
                {
                    DrawPlayer(player, Content, quadDrawer, cameraPosition, view, projection, groundTransform);
                }
            }
            else
            {
                DrawPlayer(players[0], Content, quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Solid solid in level.foreSolids)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = solid.sprite.getTexture();
                sprite.Position = new Vector3(solid.sprite.vector.X, solid.sprite.vector.Y, 0);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }

            foreach (Particle particle in level.bloodParticles)
            {
                _3DSprite sprite = new _3DSprite();
                sprite.Texture = particle.sprite.getTintedTexture();
                sprite.Position = new Vector3(particle.sprite.vector.X, particle.sprite.vector.Y, particle.posZ);
                sprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            }
        }

        private void DrawPlayer(Player player, ContentManager Content, QuadDrawer quadDrawer, Vector3 cameraPosition, Matrix view, Matrix projection, Matrix groundTransform)
        {
            _3DSprite playerSprite = new _3DSprite();
            playerSprite.Texture = player.sprite.getTintedTexture();
            playerSprite.Position = new Vector3(player.sprite.vector - new Vector2(player.sprite.getTexture().Width / 2, player.sprite.getTexture().Height / 2), 0);
            playerSprite.Up = new Vector3(0, 0, player.playerRotation);
            playerSprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
            _3DSprite weaponSprite = new _3DSprite();
            Sprite weapon2DSprite = GetPlayerWeaponTexture(player, Content);
            weaponSprite.Texture = weapon2DSprite.getTexture();
            weaponSprite.Position = new Vector3(weapon2DSprite.vector - new Vector2(player.sprite.getTexture().Width, player.sprite.getTexture().Height), 0);
            weaponSprite.Up = new Vector3(0, 0, player.playerRotation);
            weaponSprite.Draw(quadDrawer, cameraPosition, view, projection, groundTransform);
        }

        private Sprite GetPlayerWeaponTexture(Player player, ContentManager Content)
        {
            Player.Weapon weapon = player.weapon;
            if (weapon == Player.Weapon.BASIC)
            {
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
