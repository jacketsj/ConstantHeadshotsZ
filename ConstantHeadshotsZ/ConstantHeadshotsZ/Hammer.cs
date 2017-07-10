using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZ
{
    public class Hammer
    {
        public static int delay = 70;

        public RotatedRectangle HitBoxRot;
        public Rectangle HitBox;
        public int Attacking = 0;
        public Sprite sprite;
        private Texture2D stillTexture;
        private Texture2D attackTexture;
        private Player player;
        private float rotation;
        private Vector2 origin;
        private Vector2 originNew;

        public Hammer(ContentManager Content, Sprite sprite, Player player)
        {
            stillTexture = Content.Load<Texture2D>("HammerStill");
            attackTexture = Content.Load<Texture2D>("HammerAttack");
            this.player = player;
            this.sprite = sprite;
            this.sprite.setTexture(stillTexture);
            rotation = player.playerRotation;
            sprite.setX(player.sprite.getX());
            sprite.setY(player.sprite.getY());
            rotation = player.playerRotation;
            originNew = new Vector2(64, 96);
            origin = new Vector2(64, 64);
            HitBoxRot = new RotatedRectangle(new Rectangle((int)sprite.getX(), (int)sprite.getY(), sprite.getTexture().Width, sprite.getTexture().Height / 2), rotation, origin);
        }

        public void Update(ContentManager Content, Level level)
        {
            sprite.setX(player.sprite.getX());
            sprite.setY(player.sprite.getY());
            rotation = player.playerRotation;
            HitBoxRot = new RotatedRectangle(new Rectangle((int)sprite.getX(), (int)sprite.getY(), sprite.getTexture().Width, sprite.getTexture().Height / 4 * 3), rotation, originNew);
            Matrix hitBoxTransform = Matrix.CreateTranslation(new Vector3(-(originNew), 0.0f)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(sprite.vector, 0.0f));
            HitBox = CalculateBoundingRectangle(new Rectangle(0, 0, sprite.getTexture().Width, sprite.getTexture().Height / 4 * 3), hitBoxTransform);
            if (Attacking > 0)
            {
                sprite.setTexture(attackTexture);
                foreach (Zombie zombie in level.zombies)
                {
                    Rectangle zombieRectangle = new Rectangle((int)zombie.sprite.vector.X - zombie.sprite.getTexture().Width / 2, (int)zombie.sprite.vector.Y - zombie.sprite.getTexture().Height / 2, zombie.sprite.getTexture().Width, zombie.sprite.getTexture().Height);
                    if (HitBox.Intersects(zombieRectangle))
                    {
                        zombie.health -= 100;
                        level.bloodParticles = level.GenerateBurst(level.bloodParticles, CHZ.options.bldCol, new Vector3(zombie.sprite.vector, 0), Content, 60, 360, 600, 1100, 0.1f, 2.7f);
                        /*
                        Random random = new Random();
                        int amountOfNewParticles = random.Next(230, 600);
                        int amountOfOldParticles = level.bloodParticles.Length;
                        Particle[] newParticles = new Particle[amountOfOldParticles + amountOfNewParticles];
                        for (int i2 = 0; i2 < amountOfOldParticles; i2++)
                        {
                            newParticles[i2] = level.bloodParticles[i2];
                        }
                        for (int currentAmountOfNewParticles = 0; currentAmountOfNewParticles < amountOfNewParticles; currentAmountOfNewParticles++)
                        {
                            float randomDirectionRotation = rotation + (float)random.NextDouble() * MathHelper.PiOver4 * 10 / 4 - MathHelper.PiOver4 * 5 / 4;
                            float randomDirectionRotation2 = rotation + (float)random.NextDouble() * MathHelper.PiOver4 * 10 / 4 - MathHelper.PiOver4 * 5 / 4;
                            float directionX = (float)(Math.Cos(randomDirectionRotation));
                            float directionY = (float)(Math.Sin(randomDirectionRotation));
                            float directionZ = (float)Math.Abs(Math.Cos(randomDirectionRotation2));
                            //float randomDirectionChange = (float)random.NextDouble() * (2.7f - 0.1f) + 0.1f;
                            //Vector3 vectorDirection = new Vector3(directionX + ((float)Math.Cos(currentAmountOfNewParticles * amountOfNewParticles / MathHelper.TwoPi)),
                            //                                        directionY + ((float)Math.Sin(currentAmountOfNewParticles * amountOfNewParticles / MathHelper.TwoPi)),
                            //                                        );
                            Vector3 vectorDirection = new Vector3(directionX, directionY, directionZ);
                            newParticles[amountOfOldParticles + currentAmountOfNewParticles] = new Particle(CHZ.options.bldCol, zombie.sprite.vector, vectorDirection, Content, random.Next(60, 360));

                            /*
                            float randomDirectionRotation = rotation + random.Next(-55, 55);
                            int directionY = (int)(Math.Sin(randomDirectionRotation));
                            int directionX = (int)(Math.Cos(randomDirectionRotation));
                            float randomDirectionChange = (float)random.NextDouble() * (2.7f - 0.1f) + 0.1f;
                            Vector2 vectorDirection = new Vector2(randomDirectionChange * ((float)Math.Cos(currentAmountOfNewParticles * amountOfNewParticles / 360)), randomDirectionChange * ((float)Math.Sin(currentAmountOfNewParticles * amountOfNewParticles / 360)));
                            newParticles[amountOfOldParticles + currentAmountOfNewParticles] = new Particle(CHZ.options.bldCol, zombie.sprite.vector, vectorDirection, Content, random.Next(60, 360));
                             * 
                        }
                        level.bloodParticles = newParticles;
                        */
                    }
                }
                Attacking -= 1;
            }
            else
            {
                sprite.setTexture(stillTexture);
            }
            if (Attacking == 19)
            {
                Vector2 location = player.sprite.vector;
                location.X += sprite.getTexture().Height / 2 * (float)Math.Cos(rotation - Math.PI / 2);
                location.Y += sprite.getTexture().Height / 2 * (float)Math.Sin(rotation - Math.PI / 2);
                //location.Y -= sprite.getTexture().Height / 2;
                //location.X = (float)Math.Cos(rotation) * (location.X - player.sprite.vector.X) - (float)Math.Sin(rotation) * (location.Y - player.sprite.vector.Y) + player.sprite.vector.X;
                //location.Y = (float)Math.Sin(rotation) * (location.X - player.sprite.vector.X) + (float)Math.Cos(rotation) * (location.Y - player.sprite.vector.Y) + player.sprite.vector.Y;

                level.bloodParticles = level.GenerateBurst(level.bloodParticles, Color.Yellow, new Vector3(location, 0), Content, 60, 360, 230, 600, 2.6f, 2.7f);
                /*
                Random random = new Random();
                int amountOfNewParticles = random.Next(230, 600);
                int amountOfOldParticles = level.bloodParticles.Length;
                Particle[] newParticles = new Particle[amountOfOldParticles + amountOfNewParticles];
                for (int i2 = 0; i2 < amountOfOldParticles; i2++)
                {
                    newParticles[i2] = level.bloodParticles[i2];
                }
                for (int currentAmountOfNewParticles = 0; currentAmountOfNewParticles < amountOfNewParticles; currentAmountOfNewParticles++)
                {
                    float randomDirectionChange = (float)random.NextDouble() * (2.7f - 0.1f) + 0.1f;
                    float randomPitch = (float)random.NextDouble() * MathHelper.PiOver4 / 2;
                    Vector3 vectorDirection = new Vector3(2.7f * ((float)Math.Cos(currentAmountOfNewParticles * amountOfNewParticles / 360))
                                                            , 2.7f * ((float)Math.Sin(currentAmountOfNewParticles * amountOfNewParticles / 360))
                                                            , 2.7f * ((float)Math.Sin(randomPitch)));
                    newParticles[amountOfOldParticles + currentAmountOfNewParticles] = new Particle(Color.Yellow, location, vectorDirection, Content, 14);
                }
                level.bloodParticles = newParticles;
                */
            }
        }

        private static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop), Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop), Vector2.Max(leftBottom, rightBottom));

            return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            //spriteBatch.Draw(Content.Load<Texture2D>("White"), HitBox, Color.Red);
            spriteBatch.Draw(sprite.getTexture(), sprite.vector, null, sprite.getTint(), rotation, origin, 1f, SpriteEffects.None, 0f);
            //HitBoxRot.TestDraw(spriteBatch, Content);
        }
    }
}
