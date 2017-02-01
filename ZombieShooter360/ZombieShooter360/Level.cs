using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZombieShooter360
{
    class Level
    {
        public Solid[] solids;
        public int levelHeight;
        public int levelWidth;
        public Texture2D background;
        public Color backgroundColor;
        public Vector2 playerSpawn;
        public Vector2[] zombieSpawners;
        public Zombie[] zombies;
        public BasicWeaponBullet[] basicWeaponBullets;
        Random random;
        public int spawnTimer;
        public int timesSpawned = 0;
        public Particle[] bloodParticles;
        public int maxAmountOfZombies = 13;

        public Level(Solid[] newSolids, Vector2[] newZombieSpawners, ContentManager Content, Color newBackgroundColor, int newLevelWidth, int newLevelHeight, Vector2 newPlayerSpawn)
        {
            solids = newSolids;
            levelWidth = newLevelWidth;
            levelHeight = newLevelHeight;
            background = Content.Load<Texture2D>("White");
            backgroundColor = newBackgroundColor;
            playerSpawn = newPlayerSpawn;
            zombies = new Zombie[0];
            zombieSpawners = newZombieSpawners;
            random = new Random();
            spawnTimer = random.Next(600, 900);
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
        }

        public Level(Solid[] newSolids, Vector2[] newZombieSpawners, ContentManager Content, Color newBackgroundColor, Vector2 newWidthAndHeight, Vector2 newPlayerSpawn)
        {
            solids = newSolids;
            levelWidth = (int)newWidthAndHeight.X;
            levelHeight = (int)newWidthAndHeight.Y;
            background = Content.Load<Texture2D>("White");
            backgroundColor = newBackgroundColor;
            playerSpawn = newPlayerSpawn;
            zombies = new Zombie[0];
            zombieSpawners = newZombieSpawners;
            random = new Random();
            spawnTimer = random.Next(300, 600);
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
        }

        public void Update2Player(Player[] players, ContentManager Content)
        {
            if (bloodParticles.Length != 0)
            {
                bool[] deleteThisParticle = new bool[bloodParticles.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (bloodParticles[i].Update())
                    {
                        numberOfDeletes += 1;
                        deleteThisParticle[i] = true;
                    }
                    else
                    {
                        deleteThisParticle[i] = false;
                    }
                }
                Particle[] newBloodParticles = new Particle[bloodParticles.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (!deleteThisParticle[i])
                    {
                        newBloodParticles[i - currentNoOfDeletes] = bloodParticles[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                bloodParticles = newBloodParticles;
            }
            if (basicWeaponBullets != null)
            {
                bool[] deleteThisBullet = new bool[basicWeaponBullets.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < basicWeaponBullets.Length; i++)
                {
                    if (basicWeaponBullets[i].Update(this, players[0]))
                    {
                        numberOfDeletes += 1;
                        deleteThisBullet[i] = true;
                    }
                    else
                    {
                        deleteThisBullet[i] = false;
                    }
                }
                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[basicWeaponBullets.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < deleteThisBullet.Length; i++)
                {
                    if (!deleteThisBullet[i])
                    {
                        newBasicWeaponBullets[i - currentNoOfDeletes] = basicWeaponBullets[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                basicWeaponBullets = newBasicWeaponBullets;
            }
            if (zombies != null)
            {
                bool[] deleteThisZombie = new bool[zombies.Length];
                int amountOfZombiesToDelete = 0;
                bool zombiesToDelete = false;
                for (int i = 0; i < zombies.Length; i++)
                {
                    if (zombies[i].health < 1)
                    {
                        players[0].score += 90;
                    }
                    if (zombies[i].health < 1 || zombieOutOfBounds(zombies[i]))
                    {
                        deleteThisZombie[i] = true;
                        zombiesToDelete = true;
                        amountOfZombiesToDelete += 1;
                    }
                    else
                    {
                        deleteThisZombie[i] = false;
                        zombies[i].Update2Player(players, this, i);
                    }
                }
                if (zombiesToDelete == true)
                {
                    Zombie[] newZombies = new Zombie[zombies.Length - amountOfZombiesToDelete];
                    int amountOfZombiesToIgnore = 0;
                    for (int i = 0; i < zombies.Length; i++)
                    {
                        if (deleteThisZombie[i])
                        {
                            amountOfZombiesToIgnore += 1;
                        }
                        else
                        {
                            newZombies[i - amountOfZombiesToIgnore] = zombies[i];
                        }
                    }
                    zombies = newZombies;
                }
            }
            spawnTimer -= 1;
            if (spawnTimer == 0)
            {
                if (zombies.Length <= maxAmountOfZombies)
                {
                    Zombie[] newZombies;
                    if (zombies.Length != 0)
                    {
                        newZombies = new Zombie[zombies.Length + 1];
                        int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                        Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                        randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                        randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                        Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                        newZombies[0] = newZombie;
                        int i;
                        for (i = 0; i < zombies.Length; i++)
                        {
                            newZombies[i + 1] = zombies[i];
                        }
                        zombies = newZombies;
                    }
                    else
                    {
                        zombies = new Zombie[1];
                        int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                        Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                        randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                        randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                        Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                        zombies[0] = newZombie;
                    }
                    timesSpawned += 1;
                }
                int timesSpawnedDivision = timesSpawned / 3 * 2;
                if (timesSpawnedDivision < 1)
                {
                    timesSpawnedDivision = 1;
                }
                spawnTimer = random.Next(300, 600) / timesSpawnedDivision;
            }
            if (zombies.Length != 0)
            {
                if (playerCollidesWithZombiesPixel(players[0]) && players[0].delayUntilHit == 0)
                {
                    players[0].health -= 7;
                    players[0].delayUntilHit = 30;
                }
                if (playerCollidesWithZombiesPixel(players[1]) && players[1].delayUntilHit == 0)
                {
                    players[1].health -= 7;
                    players[1].delayUntilHit = 30;
                }
            }
        }

        public void Update(Player player, ContentManager Content)
        {
            if (bloodParticles.Length != 0)
            {
                bool[] deleteThisParticle = new bool[bloodParticles.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (bloodParticles[i].Update())
                    {
                        numberOfDeletes += 1;
                        deleteThisParticle[i] = true;
                    }
                    else
                    {
                        deleteThisParticle[i] = false;
                    }
                }
                Particle[] newBloodParticles = new Particle[bloodParticles.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (!deleteThisParticle[i])
                    {
                        newBloodParticles[i - currentNoOfDeletes] = bloodParticles[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                bloodParticles = newBloodParticles;
            }
            if (basicWeaponBullets != null)
            {
                bool[] deleteThisBullet = new bool[basicWeaponBullets.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < basicWeaponBullets.Length; i++)
                {
                    if (basicWeaponBullets[i].Update(this, player))
                    {
                        numberOfDeletes += 1;
                        deleteThisBullet[i] = true;
                    }
                    else
                    {
                        deleteThisBullet[i] = false;
                    }
                }
                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[basicWeaponBullets.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < deleteThisBullet.Length; i++)
                {
                    if(!deleteThisBullet[i])
                    {
                        newBasicWeaponBullets[i - currentNoOfDeletes] = basicWeaponBullets[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                basicWeaponBullets = newBasicWeaponBullets;
            }
            if (zombies != null)
            {
                bool[] deleteThisZombie = new bool[zombies.Length];
                int amountOfZombiesToDelete = 0;
                bool zombiesToDelete = false;
                for (int i = 0; i < zombies.Length; i++)
                {
                    if (zombies[i].health < 1)
                    {
                        player.score += 90;
                    }
                    if (zombies[i].health < 1 || zombieOutOfBounds(zombies[i]))
                    {
                        deleteThisZombie[i] = true;
                        zombiesToDelete = true;
                        amountOfZombiesToDelete += 1;
                    }
                    else
                    {
                        deleteThisZombie[i] = false;
                        zombies[i].Update(player, this, i);
                    }
                }
                if (zombiesToDelete == true)
                {
                    Zombie[] newZombies = new Zombie[zombies.Length - amountOfZombiesToDelete];
                    int amountOfZombiesToIgnore = 0;
                    for (int i = 0; i < zombies.Length; i++)
                    {
                        if (deleteThisZombie[i])
                        {
                            amountOfZombiesToIgnore += 1;
                        }
                        else
                        {
                            newZombies[i - amountOfZombiesToIgnore] = zombies[i];
                        }
                    }
                    zombies = newZombies;
                }
            }
            spawnTimer -= 1;
            if (spawnTimer == 0)
            {
                if (zombies.Length <= maxAmountOfZombies)
                {
                    Zombie[] newZombies;
                    if (zombies.Length != 0)
                    {
                        newZombies = new Zombie[zombies.Length + 1];
                        int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                        Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                        randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                        randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                        Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                        newZombies[0] = newZombie;
                        int i;
                        for (i = 0; i < zombies.Length; i++)
                        {
                            newZombies[i + 1] = zombies[i];
                        }
                        zombies = newZombies;
                    }
                    else
                    {
                        zombies = new Zombie[1];
                        int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                        Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                        randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                        randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                        Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                        zombies[0] = newZombie;
                    }
                    timesSpawned += 1;
                }
                int timesSpawnedDivision = timesSpawned / 3 * 2;
                if (timesSpawnedDivision < 1)
                {
                    timesSpawnedDivision = 1;
                }
                spawnTimer = random.Next(300, 600) / timesSpawnedDivision;
            }
            if (zombies.Length != 0)
            {
                if (playerCollidesWithZombiesPixel(player) && player.delayUntilHit == 0)
                {
                    player.health -= 7;
                    player.delayUntilHit = 30;
                }
            }
        }

        public static bool intersects(Sprite circleSprite, Sprite rectSprite)
        {
            Vector2 circleDistance = new Vector2();
            circleDistance.X = circleSprite.getX() - rectSprite.getX();
            circleDistance.Y = circleSprite.getY() - rectSprite.getY();

            if (circleDistance.X > (rectSprite.getTexture().Width / 2 + circleSprite.getTexture().Width / 2))
            {
                return false;
            }
            if (circleDistance.Y > (rectSprite.getTexture().Height / 2 + circleSprite.getTexture().Height / 2))
            {
                return false;
            }

            if (circleDistance.X <= (rectSprite.getTexture().Width / 2))
            {
                return true;
            }
            if (circleDistance.Y <= (rectSprite.getTexture().Height / 2))
            {
                return true;
            }

            float cornerDistance_sq = (float)Math.Pow((circleDistance.X - rectSprite.getTexture().Width / 2), 2) + (float)Math.Pow((circleDistance.Y - rectSprite.getTexture().Height / 2), 2);

            return (cornerDistance_sq <= (circleSprite.getTexture().Width ^ 2));
        }

        public bool zombieOutOfBounds(Zombie zombie)
        {
            if (zombie.sprite.vector.X + zombie.sprite.getTexture().Width <= 0)
            {
                return true;
            }
            else if (zombie.sprite.vector.Y + zombie.sprite.getTexture().Height <= 0)
            {
                return true;
            }
            else if (zombie.sprite.vector.X > levelWidth)
            {
                return true;
            }
            else if (zombie.sprite.vector.Y > levelHeight)
            {
                return true;
            }
            return false;
        }

        public bool zombieCollidesWithSolidsPixel(Zombie zombie, Vector2 oldVector)
        {
            foreach(Solid solid in solids)
            {
                if(intersects(zombie.sprite, solid.sprite))
                {
                    return true;
                }
            }
            return false;
            /*
            Vector2 currentVector = zombie.sprite.vector;
            Vector2 difference = new Vector2();
            Vector2 differencePositive = new Vector2();
            difference.X = zombie.sprite.getX() - oldVector.X;
            difference.Y = zombie.sprite.getY() - oldVector.Y;
            difference.X = (int)difference.X;
            difference.Y = (int)difference.Y;
            differencePositive = difference;
            if (differencePositive.X < 0)
            {
                differencePositive.X /= -1;
            }
            if (differencePositive.Y < 0)
            {
                differencePositive.Y /= -1;
            }
            if (difference.X == 0)
            {
                difference.X = 1;
                differencePositive.X = 1;
            }
            if (difference.Y == 0)
            {
                difference.Y = 1;
                differencePositive.Y = 1;
            }
            for (int y = 0; y != difference.Y; y += ((int)difference.Y / (int)differencePositive.Y))
            {
                for (int x = 0; x != difference.X; x += ((int)difference.X / (int)differencePositive.X))
                {
                    foreach (Solid solid in solids)
                    {
                        currentVector.X = zombie.sprite.getX() - x;
                        currentVector.Y = zombie.sprite.getY() - y;
                        if (solid.sprite.IntersectsPixel(new Sprite(zombie.sprite.getTexture(), currentVector)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
            */
        }

        public bool zombieCollidesWithUncollidablePixel(Zombie zombie, Vector2 oldVector)
        {
            if (zombieCollidesWithSolidsPixel(zombie, oldVector))
            {
                return true;
            }
            if (zombieCollidesWithZombiesPixel(zombie, oldVector))
            {
                return true;
            }
            return false;
        }

        public bool intersectsCircles(Sprite circleSprite1, Sprite circleSprite2)
        {
            int dx = (int)(circleSprite2.getX() - circleSprite1.getX());
            int dy = (int)(circleSprite2.getY() - circleSprite1.getY());
            int radii = circleSprite1.getTexture().Width / 2 + circleSprite2.getTexture().Width / 2;
            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return true;
            }
            return false;
        }

        public bool zombieCollidesWithZombiesPixel(Zombie zombie, Vector2 oldVector)
        {
            foreach (Solid solid in solids)
            {
                if (intersectsCircles(zombie.sprite, solid.sprite))
                {
                    return true;
                }
            }
            return false;
            /*
            Vector2 currentVector = zombie.sprite.vector;
            Vector2 difference = new Vector2();
            Vector2 differencePositive = new Vector2();
            difference.X = zombie.sprite.getX() - oldVector.X;
            difference.Y = zombie.sprite.getY() - oldVector.Y;
            difference.X = (int)difference.X;
            difference.Y = (int)difference.Y;
            differencePositive = difference;
            if (differencePositive.X < 0)
            {
                differencePositive.X /= -1;
            }
            if (differencePositive.Y < 0)
            {
                differencePositive.Y /= -1;
            }
            if (difference.X == 0)
            {
                difference.X = 1;
                differencePositive.X = 1;
            }
            if (difference.Y == 0)
            {
                difference.Y = 1;
                differencePositive.Y = 1;
            }
            for (int y = 0; y != difference.Y; y += ((int)difference.Y / (int)differencePositive.Y))
            {
                for (int x = 0; x != difference.X; x += ((int)difference.X / (int)differencePositive.X))
                {
                    foreach (Zombie currentZombie in zombies)
                    {
                        if (zombie.sprite != currentZombie.sprite)
                        {
                            currentVector.X = zombie.sprite.getX() - x;
                            currentVector.Y = zombie.sprite.getY() - y;
                            Sprite currentSprite = currentZombie.sprite;
                            Sprite zombieSprite = new Sprite(zombie.sprite.getTexture(), currentVector);
                            zombieSprite.vector.X -= zombieSprite.getTexture().Width / 2;
                            zombieSprite.vector.Y -= zombieSprite.getTexture().Height / 2;
                            if (currentSprite.IntersectsPixel(zombieSprite))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
            */
        }

        public bool zombieCollidesWithUncollidablePixel(Zombie zombie)
        {
            if(zombieCollidesWithSolidPixel(zombie) || zombieCollidesWithZombiesPixel(zombie))
            {
                return true;
            }
            return false;
        }

        public bool zombieCollidesWithZombiesPixel(Zombie zombie)
        {
            foreach (Zombie currentZombie in zombies)
            {
                if (currentZombie.sprite != zombie.sprite)
                {
                    if (currentZombie.sprite.IntersectsPixelZombie(zombie.sprite))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool zombieCollidesWithSolidPixel(Zombie zombie)
        {
            foreach (Solid solid in solids)
            {
                if (solid.sprite.IntersectsPixel(zombie.sprite))
                {
                    return true;
                }
            }
            return false;
        }

        /*
        public bool zombieCollidesWithZombiesPixel(int index, Vector2 oldVector)
        {

            Vector2 currentVector = zombies[index].sprite.vector;
            Vector2 difference = new Vector2();
            Vector2 differencePositive = new Vector2();
            difference.X = zombies[index].sprite.getX() - oldVector.X;
            difference.Y = zombies[index].sprite.getY() - oldVector.Y;
            difference.X = (int)difference.X;
            difference.Y = (int)difference.Y;
            differencePositive = difference;
            if (differencePositive.X < 0)
            {
                differencePositive.X /= -1;
            }
            if (differencePositive.Y < 0)
            {
                differencePositive.Y /= -1;
            }
            if (difference.X == 0)
            {
                difference.X = 1;
                differencePositive.X = 1;
            }
            if (difference.Y == 0)
            {
                difference.Y = 1;
                differencePositive.Y = 1;
            }
            for (int y = 0; y != difference.Y; y += ((int)difference.Y / (int)differencePositive.Y))
            {
                for (int x = 0; x != difference.X; x += ((int)difference.X / (int)differencePositive.X))
                {
                    for (int i = 0; i < zombies.Length; i++)
                    {
                        if (i != index)
                        {
                            currentVector.X = zombies[index].sprite.getX() - x;
                            currentVector.Y = zombies[index].sprite.getY() - y;
                            if (zombies[i].sprite.IntersectsPixel(new Sprite(zombies[index].sprite.getTexture(), currentVector)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        */

        public bool playerCollidesWithZombiesPixel(Player player)
        {
            foreach (Zombie zombie in zombies)
            {
                if (intersectsCircles(zombie.sprite, player.sprite))
                {
                    return true;
                }
            }
            return false;
        }

        public bool playerCollidesWithSolidsPixel(Player player, Vector2 oldVector)
        {
            if (player.sprite.getX() - player.sprite.getTexture().Width / 2 < 0)
            {
                return true;
            }
            if (player.sprite.getY() - player.sprite.getTexture().Height / 2 < 0)
            {
                return true;
            }
            if (player.sprite.getX() + player.sprite.getTexture().Width / 2 > levelWidth)
            {
                return true;
            }
            if (player.sprite.getY() + player.sprite.getTexture().Height / 2 > levelHeight)
            {
                return true;
            }
            Vector2 currentVector = player.sprite.vector;
            Vector2 difference = new Vector2();
            Vector2 differencePositive = new Vector2();
            difference.X = player.sprite.getX() - oldVector.X;
            difference.Y = player.sprite.getY() - oldVector.Y;
            difference.X = (int)difference.X;
            difference.Y = (int)difference.Y;
            differencePositive = difference;
            if(differencePositive.X < 0)
            {
                differencePositive.X /= -1;
            }
            if(differencePositive.Y < 0)
            {
                differencePositive.Y /= -1;
            }
            if (difference.X == 0)
            {
                difference.X = 1;
                differencePositive.X = 1;
            }
            if (difference.Y == 0)
            {
                difference.Y = 1;
                differencePositive.Y = 1;
            }
            for (int y = 0; y != difference.Y; y += ((int)difference.Y / (int)differencePositive.Y))
            {
                for(int x = 0; x != difference.X; x += ((int)difference.X / (int)differencePositive.X))
                {
                    foreach (Solid solid in solids)
                    {
                        currentVector.X = player.sprite.getX() - x;
                        currentVector.Y = player.sprite.getY() - y;
                        if (solid.sprite.IntersectsPixel(new Sprite(player.sprite.getTexture(), currentVector)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool playerCollidesWithSolidsPixel(Player player)
        {
            if (player.sprite.getX() < 0 + player.sprite.getTexture().Width / 2)
            {
                return true;
            }
            if (player.sprite.getX() > levelWidth - player.sprite.getTexture().Width / 2)
            {
                return true;
            }
            if (player.sprite.getY() < 0 + player.sprite.getTexture().Height / 2)
            {
                return true;
            }
            if (player.sprite.getY() > levelHeight - player.sprite.getTexture().Height / 2)
            {
                return true;
            }
            foreach (Solid solid in solids)
            {
                if (solid.sprite.IntersectsPixel(player.sprite))
                {
                    return true;
                }
            }
            return false;
        }

        public bool playerCollidesWithSolids(Player player)
        {
            foreach (Solid solid in solids)
            {
                if (solid.sprite.Intersects(player.sprite))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, levelWidth, levelHeight), backgroundColor);

            foreach (Solid solid in solids)
            {
                solid.sprite.Draw(spriteBatch);
            }

            foreach (Vector2 spawner in zombieSpawners)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("ZombieSpawner"), spawner, Color.White);
            }

            foreach (Particle particle in bloodParticles)
            {
                particle.Draw(spriteBatch, Content);
            }

            for (int i = 0; i < zombies.Length; i++)
            {
                Zombie currentZombie = zombies[i];
                currentZombie.Draw(spriteBatch);
            }

            foreach (BasicWeaponBullet bullet in basicWeaponBullets)
            {
                bullet.Draw(spriteBatch);
            }

            for (int i = 0; i < zombies.Length; i++)
            {
                String health = zombies[i].health.ToString();
                Vector2 healthOrigin = zombies[i].Font.MeasureString(health) / 2;
                spriteBatch.DrawString(zombies[i].Font, health, new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY() + zombies[i].sprite.getTexture().Height / 4 * 3), Color.Black, 0, healthOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
        }

        public void DrawWithoutHealth(SpriteBatch spriteBatch, ContentManager Content)
        {

            spriteBatch.Draw(background, new Rectangle(0, 0, levelWidth, levelHeight), backgroundColor);

            foreach (Solid solid in solids)
            {
                solid.sprite.Draw(spriteBatch);
            }

            foreach (Vector2 spawner in zombieSpawners)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("ZombieSpawner"), spawner, Color.White);
            }

            foreach (Particle particle in bloodParticles)
            {
                particle.Draw(spriteBatch, Content);
            }

            for (int i = 0; i < zombies.Length; i++)
            {
                Zombie currentZombie = zombies[i];
                currentZombie.Draw(spriteBatch);
            }

            foreach (BasicWeaponBullet bullet in basicWeaponBullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
    }
}
