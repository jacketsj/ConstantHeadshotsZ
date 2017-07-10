using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using LevelBuilder;

namespace ConstantHeadshotsZ
{
    public class Level
    {
        public Solid[] solids;
        public Solid[] backSolids;
        public Solid[] foreSolids;
        public int levelHeight;
        public int levelWidth;
        public Texture2D background;
        public Color backgroundColor;
        public Vector2 playerSpawn;
        public Vector2[] zombieSpawners;
        public Zombie[] zombies;
        public BasicWeaponBullet[] basicWeaponBullets;
        public Rocket[] rockets;
        public Random random;
        public int spawnTimer;
        public int defaultSpawnTimer;
        public int timesSpawned = 0;
        public Particle[] bloodParticles;
        public int maxAmountOfZombies = 60;
        public Drop[] drops;
        public bool zombieSpawnAcceleration = true;

        public void ResetLevel()
        {
            spawnTimer = defaultSpawnTimer;
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
            drops = new Drop[0];
            rockets = new Rocket[0];
            zombies = new Zombie[0];
            random = new Random();
        }

        public Level(LevelData levelData, GraphicsDevice graphicsDevice)
        {
            solids = new Solid[levelData.solids.Length];
            if (levelData.backSolids != null)
            {
                backSolids = new Solid[levelData.backSolids.Length];
            }
            else
            {
                backSolids = new Solid[0];
            }
            if (levelData.backSolids != null)
            {
                foreSolids = new Solid[levelData.foreSolids.Length];
            }
            else
            {
                foreSolids = new Solid[0];
            }
            for (int i = 0; i < levelData.solids.Length; i++)
            {
                Texture2D texture = new Texture2D(graphicsDevice, levelData.textures[levelData.solids[i].textureNo].Width, levelData.textures[levelData.solids[i].textureNo].Height);
                texture.SetData(levelData.textures[levelData.solids[i].textureNo].Colors);
                solids[i] = new Solid(new Sprite(texture, levelData.solids[i].position, levelData.solids[i].tint));
            }
            for (int i = 0; levelData.backSolids != null && i < levelData.backSolids.Length; i++)
            {
                Texture2D texture = new Texture2D(graphicsDevice, levelData.textures[levelData.backSolids[i].textureNo].Width, levelData.textures[levelData.backSolids[i].textureNo].Height);
                texture.SetData(levelData.textures[levelData.backSolids[i].textureNo].Colors);
                backSolids[i] = new Solid(new Sprite(texture, levelData.backSolids[i].position, levelData.backSolids[i].tint));
            }
            for (int i = 0; levelData.foreSolids != null && i < levelData.foreSolids.Length; i++)
            {
                Texture2D texture = new Texture2D(graphicsDevice, levelData.textures[levelData.foreSolids[i].textureNo].Width, levelData.textures[levelData.foreSolids[i].textureNo].Height);
                texture.SetData(levelData.textures[levelData.foreSolids[i].textureNo].Colors);
                foreSolids[i] = new Solid(new Sprite(texture, levelData.foreSolids[i].position, levelData.foreSolids[i].tint));
            }
            levelHeight = levelData.levelHeight;
            levelWidth = levelData.levelWidth;
            background = new Texture2D(graphicsDevice, levelData.textures[levelData.backgroundReference].Width, levelData.textures[levelData.backgroundReference].Height);
            background.SetData(levelData.textures[levelData.backgroundReference].Colors);
            backgroundColor = levelData.backgroundColor;
            playerSpawn = levelData.playerSpawn;
            zombieSpawners = levelData.zombieSpawners;
            maxAmountOfZombies = levelData.maxAmountOfZombies;
            zombieSpawnAcceleration = levelData.zombieSpawnAcceleration;
            defaultSpawnTimer = levelData.spawnTimer;
            spawnTimer = defaultSpawnTimer;
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
            drops = new Drop[0];
            rockets = new Rocket[0];
            zombies = new Zombie[0];
            random = new Random();
        }

        public Level(Solid[] newSolids, Solid[] newBackSolids, Solid[] newForeSolids, Vector2[] newZombieSpawners, ContentManager Content, Color newBackgroundColor, int newLevelWidth, int newLevelHeight, Vector2 newPlayerSpawn)
        {
            solids = newSolids;
            backSolids = newBackSolids;
            foreSolids = newForeSolids;
            levelWidth = newLevelWidth;
            levelHeight = newLevelHeight;
            background = Content.Load<Texture2D>("White");
            backgroundColor = newBackgroundColor;
            playerSpawn = newPlayerSpawn;
            zombies = new Zombie[0];
            zombieSpawners = newZombieSpawners;
            random = new Random();
            defaultSpawnTimer = random.Next(600, 900);
            spawnTimer = defaultSpawnTimer;
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
            drops = new Drop[0];
            rockets = new Rocket[0];
            //drops = new Drop[1];
            //drops[0] = new Drop(Player.Weapon.LAZERSWORD, new Sprite(Content.Load<Texture2D>("LazerSwordDrop"), new Vector2(600, 600)), Vector2.Zero, 1000);
        }

        public Level(Solid[] newSolids, Solid[] newBackSolids, Solid[] newForeSolids, Vector2[] newZombieSpawners, ContentManager Content, Color newBackgroundColor, Vector2 newWidthAndHeight, Vector2 newPlayerSpawn)
        {
            solids = newSolids;
            backSolids = newBackSolids;
            foreSolids = newForeSolids;
            levelWidth = (int)newWidthAndHeight.X;
            levelHeight = (int)newWidthAndHeight.Y;
            background = Content.Load<Texture2D>("White");
            backgroundColor = newBackgroundColor;
            playerSpawn = newPlayerSpawn;
            zombies = new Zombie[0];
            zombieSpawners = newZombieSpawners;
            random = new Random();
            defaultSpawnTimer = random.Next(600, 900);
            spawnTimer = defaultSpawnTimer;
            basicWeaponBullets = new BasicWeaponBullet[0];
            bloodParticles = new Particle[0];
            drops = new Drop[0];
            rockets = new Rocket[0];
            //drops = new Drop[1];
            //drops[0] = new Drop(Player.Weapon.BASIC, new Sprite(Content.Load<Texture2D>("BasicWeaponDrop"), new Vector2(600, 600)), Vector2.Zero, 1000);
        }

        public void Update2Player(Player[] players, ContentManager Content, TimeSpan elapsedTime)
        {
            if (drops.Length != 0)
            {
                bool[] deleteThisDrop = new bool[drops.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < drops.Length; i++)
                {
                    if (drops[i].Update2Player(players))
                    {
                        numberOfDeletes += 1;
                        deleteThisDrop[i] = true;
                    }
                    else
                    {
                        deleteThisDrop[i] = false;
                    }
                }
                Drop[] newdrops = new Drop[drops.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < drops.Length; i++)
                {
                    if (!deleteThisDrop[i])
                    {
                        newdrops[i - currentNoOfDeletes] = drops[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                drops = newdrops;
            }
            if (bloodParticles.Length != 0)
            {
                bool[] deleteThisParticle = new bool[bloodParticles.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (bloodParticles[i].Update(elapsedTime))
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
            if (rockets != null)
            {
                bool[] deleteThisRocket = new bool[rockets.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < rockets.Length; i++)
                {
                    if (rockets[i].Update(this, players[0]))
                    {
                        numberOfDeletes += 1;
                        deleteThisRocket[i] = true;
                    }
                    else
                    {
                        deleteThisRocket[i] = false;
                    }
                }
                Rocket[] newRockets = new Rocket[rockets.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < deleteThisRocket.Length; i++)
                {
                    if (!deleteThisRocket[i])
                    {
                        newRockets[i - currentNoOfDeletes] = rockets[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                rockets = newRockets;
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
                        Random random = new Random();
                        int randomNum = random.Next(0, 7);
                        if (randomNum == 6)
                        {
                            drops = Drop.DropWeapon(Drop.GetRandomWeapon(), zombies[i].sprite.vector, Content, drops);
                        }
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
            if (spawnTimer <= 0)
            {
                if (zombies.Length <= maxAmountOfZombies)
                {
                    //SPAWN NEW ZOMBIE
                    Zombie[] newZombies;
                    //if (zombies.Length != 0)
                    //{
                    newZombies = new Zombie[zombies.Length + 1];
                    int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                    Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                    randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                    randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                    Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                    if (zombieCollidesWithZombies(newZombie) == null)
                    {
                        newZombies[0] = newZombie;
                        for (int i = 0; i < zombies.Length; i++)
                        {
                            newZombies[i + 1] = zombies[i];
                        }
                        zombies = newZombies;
                    }
                    //}
                    //else
                    //{
                    //    zombies = new Zombie[1];
                    //    int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                    //    Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                    //    randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                    //    randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                    //    Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                    //    zombies[0] = newZombie;
                    //}
                    timesSpawned += 1;
                }
                if (zombieSpawnAcceleration)
                {
                    int timesSpawnedDivision = timesSpawned / 3 * 2;
                    if (timesSpawnedDivision < 1)
                    {
                        timesSpawnedDivision = 1;
                    }
                    spawnTimer = random.Next(300, 600) / timesSpawnedDivision;
                }
                else
                {
                    spawnTimer = defaultSpawnTimer;
                }
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

        public void Update(Player player, ContentManager Content, TimeSpan elapsedTime)
        {
            if (drops.Length != 0)
            {
                bool[] deleteThisDrop = new bool[drops.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < drops.Length; i++)
                {
                    if (drops[i].Update(player))
                    {
                        numberOfDeletes += 1;
                        deleteThisDrop[i] = true;
                    }
                    else
                    {
                        deleteThisDrop[i] = false;
                    }
                }
                Drop[] newdrops = new Drop[drops.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < drops.Length; i++)
                {
                    if (!deleteThisDrop[i])
                    {
                        newdrops[i - currentNoOfDeletes] = drops[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                drops = newdrops;
            }
            if (bloodParticles.Length != 0)
            {
                bool[] deleteThisParticle = new bool[bloodParticles.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < bloodParticles.Length; i++)
                {
                    if (bloodParticles[i].Update(elapsedTime))
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
            if (rockets != null)
            {
                bool[] deleteThisRocket = new bool[rockets.Length];
                int numberOfDeletes = 0;
                for (int i = 0; i < rockets.Length; i++)
                {
                    if (rockets[i].Update(this, player))
                    {
                        numberOfDeletes += 1;
                        deleteThisRocket[i] = true;
                    }
                    else
                    {
                        deleteThisRocket[i] = false;
                    }
                }
                Rocket[] newRockets = new Rocket[rockets.Length - numberOfDeletes];
                int currentNoOfDeletes = 0;
                for (int i = 0; i < deleteThisRocket.Length; i++)
                {
                    if (!deleteThisRocket[i])
                    {
                        newRockets[i - currentNoOfDeletes] = rockets[i];
                    }
                    else
                    {
                        currentNoOfDeletes += 1;
                    }
                }
                rockets = newRockets;
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
                        Random random = new Random();
                        int randomNum = random.Next(0, 7);
                        if (randomNum == 6)
                        {
                            //Drop.DropWeapon(Player.Weapon.LAZERSWORD, zombies[i].sprite.vector, Content, drops);
                            drops = Drop.DropWeapon(Drop.GetRandomWeapon(), zombies[i].sprite.vector, Content, drops);
                        }
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
            if (spawnTimer <= 0)
            {
                if(zombies.Length <= maxAmountOfZombies)
                {
                    //SPAWN NEW ZOMBIE
                    Zombie[] newZombies;
                    //if (zombies.Length != 0)
                    //{
                    newZombies = new Zombie[zombies.Length + 1];
                    int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                    Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                    randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                    randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                    Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                    if (zombieCollidesWithZombies(newZombie) == null)
                    {
                        newZombies[0] = newZombie;
                        for (int i = 0; i < zombies.Length; i++)
                        {
                            newZombies[i + 1] = zombies[i];
                        }
                        zombies = newZombies;
                    }
                    //}
                    //else
                    //{
                    //    zombies = new Zombie[1];
                    //    int randomZombieSpawnerIndex = random.Next(0, zombieSpawners.Length);
                    //    Vector2 randomZombieSpawner = zombieSpawners[randomZombieSpawnerIndex];
                    //    randomZombieSpawner.X += Content.Load<Texture2D>("Player").Width / 2;
                    //    randomZombieSpawner.Y += Content.Load<Texture2D>("Player").Height / 2;
                    //    Zombie newZombie = new Zombie(new Sprite(Content.Load<Texture2D>("Player"), randomZombieSpawner, Color.Green), 0, Content);
                    //    zombies[0] = newZombie;
                    //}
                    timesSpawned += 1;
                }
                if (zombieSpawnAcceleration)
                {
                    int timesSpawnedDivision = timesSpawned / 3 * 2;
                    if (timesSpawnedDivision < 1)
                    {
                        timesSpawnedDivision = 1;
                    }
                    spawnTimer = random.Next(300, 600) / timesSpawnedDivision;
                }
                else
                {
                    spawnTimer = defaultSpawnTimer;
                }
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

        public Solid zombieCollidesWithSolidsPixel(Zombie zombie, Vector2 oldVector)
        {
            foreach (Solid solid in solids)
            {
                if (solid.sprite.Intersects(zombie.sprite))
                {
                    return solid;
                }
            }
            return null;
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

        public Sprite zombieCollidesWithUncollidablePixel(Zombie zombie, Vector2 oldVector)
        {
            Solid solid = zombieCollidesWithSolidsPixel(zombie, oldVector);
            if (solid != null)
            {
                return solid.sprite;
            }
            Sprite sprite = zombieCollidesWithZombiesPixel(zombie, oldVector);
            if (sprite != null)
            {
                return sprite;
            }
            return null;
        }

        public object zombieCollidesWithUncollidable(Zombie zombie, Vector2 oldVector)
        {
            Solid solid = zombieCollidesWithSolidsPixel(zombie, oldVector);
            if (solid != null)
            {
                return solid;
            }
            Sprite sprite = zombieCollidesWithZombiesPixel(zombie, oldVector);
            if (sprite != null)
            {
                return sprite;
            }
            return null;
        }

        public Sprite intersectsCircles(Sprite circleSprite1, Sprite circleSprite2)
        {
            int dx = (int)(circleSprite2.getX() - circleSprite1.getX());
            int dy = (int)(circleSprite2.getY() - circleSprite1.getY());
            int radii = circleSprite1.getTexture().Width / 2 + circleSprite2.getTexture().Width / 2;
            if ((dx * dx) + (dy * dy) < radii * radii)
            {
                return circleSprite2;
            }
            return null;
        }

        public Sprite zombieCollidesWithZombiesPixel(Zombie zombie, Vector2 oldVector)
        {
            foreach (Zombie zombie2 in zombies)
            {
                if (zombie2.sprite.Intersects(zombie.sprite))
                {
                    return zombie2.sprite;
                }
            }
            return null;
            //zombie.sprite.vector.X -= 32;
            //zombie.sprite.vector.Y -= 32;
            //Sprite sprite = zombie.sprite;
            //Sprite sprite = new Sprite(zombie.sprite.getTexture(), new Vector2(zombie.sprite.vector.X + 32, zombie.sprite.vector.Y - 32));
            /*
            foreach (Zombie zombie2 in zombies)
            {
                if (sprite.Intersects(zombie2.sprite))
                {
                    Sprite sprite2 = new Sprite(zombie2.sprite.getTexture(), new Vector2(zombie2.sprite.vector.X, zombie2.sprite.vector.Y));
                    if (sprite2 != null)
                    {
                        return sprite2;
                    }
                }
            }
            return null;
             * */
            /*
            foreach (Solid solid in solids)
            {
                if (zombie.sprite.Intersects(solid.sprite))
                {
                    Sprite sprite = solid.sprite;
                    if (sprite != null)
                    {
                        return sprite;
                    }
                }
            }
            return null;
             * */
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

        public Zombie zombieCollidesWithZombies(Zombie zombie)
        {
            foreach (Zombie currentZombie in zombies)
            {
                if (currentZombie.sprite != zombie.sprite)
                {
                    if (zombie.sprite.getTexture().Width / 2 + currentZombie.sprite.getTexture().Width / 2
                        > Math.Sqrt(Math.Pow(zombie.sprite.getX() - currentZombie.sprite.getX(), 2) + Math.Pow(zombie.sprite.getY() - currentZombie.sprite.getY(), 2)))
                    {
                        return currentZombie;
                    }
                    //if (currentZombie.sprite.IntersectsPixelZombie(zombie.sprite))
                    //{
                    //    return true;
                    //}
                }
            }
            return null;
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
                if (intersectsCircles(zombie.sprite, player.sprite) != null)
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

        public Solid playerCollidesWithSolidsPixel(Player player)
        {
            if (player.sprite.getX() < 0 + player.sprite.getTexture().Width / 2)
            {
                player.sprite.vector.X = 0 + player.sprite.getTexture().Width / 2;
                return null;
            }
            if (player.sprite.getX() > levelWidth - player.sprite.getTexture().Width / 2)
            {
                player.sprite.vector.X = levelWidth - player.sprite.getTexture().Width / 2;
                return null;
            }
            if (player.sprite.getY() < 0 + player.sprite.getTexture().Height / 2)
            {
                player.sprite.vector.Y = 0 + player.sprite.getTexture().Height / 2;
                return null;
            }
            if (player.sprite.getY() > levelHeight - player.sprite.getTexture().Height / 2)
            {
                player.sprite.vector.Y = levelHeight - player.sprite.getTexture().Height / 2;
                return null;
            }
            foreach (Solid solid in solids)
            {
                if (solid.sprite.Intersects(player.sprite))
                {
                    return solid;
                }
            }
            return null;
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

        public void Draw(SpriteBatch spriteBatch, ContentManager Content, Player player, int playerNo)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, levelWidth, levelHeight), backgroundColor);

            foreach (Solid solid in backSolids)
            {
                solid.sprite.Draw(spriteBatch);
            }

            foreach (Drop drop in drops)
            {
                drop.Draw(spriteBatch, Content);
            }

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

            if (zombies != null)
            {
                for (int i = 0; i < zombies.Length; i++)
                {
                    Zombie currentZombie = zombies[i];
                    currentZombie.Draw(spriteBatch);
                }
            }

            foreach (BasicWeaponBullet bullet in basicWeaponBullets)
            {
                bullet.Draw(spriteBatch);
            }

            foreach (Rocket rocket in rockets)
            {
                rocket.Draw(spriteBatch, Content);
            }

            if (zombies != null)
            {
                for (int i = 0; i < zombies.Length; i++)
                {
                    if (playerNo == 1)
                    {
                        if (CHZ.options.player1CameraRotation)
                        {
                            String health = zombies[i].health.ToString();
                            Vector2 healthOrigin = zombies[i].Font.MeasureString(health) / 2;
                            spriteBatch.DrawString(zombies[i].Font, health, CHZ.RotateVector2(new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY() + zombies[i].sprite.getTexture().Height / 4 * 3), player.playerRotation, new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY())), Color.Black, player.playerRotation, healthOrigin, 1.0f, SpriteEffects.None, 0.5f);
                        }
                        else
                        {
                            String health = zombies[i].health.ToString();
                            Vector2 healthOrigin = zombies[i].Font.MeasureString(health) / 2;
                            spriteBatch.DrawString(zombies[i].Font, health, new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY() + zombies[i].sprite.getTexture().Height / 4 * 3), Color.Black, 0, healthOrigin, 1.0f, SpriteEffects.None, 0.5f);
                        }
                    }
                    else if (playerNo == 2)
                    {
                        if (CHZ.options.player2CameraRotation)
                        {
                            String health = zombies[i].health.ToString();
                            Vector2 healthOrigin = zombies[i].Font.MeasureString(health) / 2;
                            spriteBatch.DrawString(zombies[i].Font, health, CHZ.RotateVector2(new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY() + zombies[i].sprite.getTexture().Height / 4 * 3), player.playerRotation, new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY())), Color.Black, player.playerRotation, healthOrigin, 1.0f, SpriteEffects.None, 0.5f);
                        }
                        else
                        {
                            String health = zombies[i].health.ToString();
                            Vector2 healthOrigin = zombies[i].Font.MeasureString(health) / 2;
                            spriteBatch.DrawString(zombies[i].Font, health, new Vector2(zombies[i].sprite.getX(), zombies[i].sprite.getY() + zombies[i].sprite.getTexture().Height / 4 * 3), Color.Black, 0, healthOrigin, 1.0f, SpriteEffects.None, 0.5f);
                        }
                    }
                }
            }

            foreach (Solid solid in foreSolids)
            {
                solid.sprite.Draw(spriteBatch);
            }
        }

        public Particle[] GenerateBurst(Particle[] parts, Color color, Vector3 position, ContentManager Content, int minLife, int maxLife, int minNoParticles, int maxNoParticles, float minVel, float maxVel)
        {
            return GenerateBurst(parts, color, position, Content, minLife, maxLife, minNoParticles, maxNoParticles, minVel, maxVel, 0, MathHelper.TwoPi, 0, MathHelper.Pi);
        }

        public Particle[] GenerateBurst(Particle[] parts, Color color, Vector3 position, ContentManager Content, int minLife, int maxLife, int minNoParticles, int maxNoParticles,
                                                                                float minVel, float maxVel, float minAngle, float maxAngle, float minPitch, float maxPitch)
        {
            minPitch = MathHelper.PiOver2 - minPitch;
            maxPitch = MathHelper.PiOver2 - maxPitch;

            int noParticles = random.Next(maxNoParticles - minNoParticles) + minNoParticles;
            Particle[] finalParts = new Particle[parts.Length + noParticles];
            for (int i = 0; i < noParticles; ++i)
            {
                float vel = (float)(random.NextDouble() * (Math.Sqrt(maxVel) - Math.Sqrt(minVel)) + Math.Sqrt(minVel));
                vel *= vel;
                float angle = (float)random.NextDouble() * (maxAngle - minAngle) + minAngle;

                //get a uniform spherical distribution
                float pitchHelper = (float)(random.NextDouble() * (Math.Cos(maxPitch) - Math.Cos(minPitch)) + Math.Cos(minPitch));//(float)random.NextDouble()*2-1;
                float pitch = (float)Math.Acos(pitchHelper);
                //float pitch = (float)random.NextDouble() * (maxPitch - minPitch) + minPitch;
                int life = random.Next(maxLife - minLife) + minLife;
                Vector3 vVel = vel * (new Vector3((float)Math.Cos(angle) * (float)Math.Sin(pitch), (float)Math.Sin(angle) * (float)Math.Sin(pitch), (float)Math.Cos(pitch)));
                if (angle > MathHelper.PiOver2 && angle < 3 * MathHelper.PiOver2)
                {
                    //vVel.X *= -1;
                }
                if (angle > MathHelper.Pi && angle < MathHelper.TwoPi)
                {
                    //vVel.Y *= -1;
                }
                if (pitch > MathHelper.Pi && pitch < MathHelper.TwoPi)
                {
                    //vVel.Z *= -1;
                }
                finalParts[i + parts.Length] = new Particle(color, position, vVel, Content, life);
            }
            for (int i = 0; i < parts.Length; ++i)
            {
                finalParts[i] = parts[i];
            }
            return finalParts;
        }

        public void DrawWithoutHealth(SpriteBatch spriteBatch, ContentManager Content)
        {

            spriteBatch.Draw(background, new Rectangle(0, 0, levelWidth, levelHeight), backgroundColor);

            foreach (Solid solid in backSolids)
            {
                solid.sprite.Draw(spriteBatch);
            }
            
            foreach (Drop drop in drops)
            {
                drop.Draw(spriteBatch, Content);
            }

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

            foreach (Rocket rocket in rockets)
            {
                rocket.Draw(spriteBatch, Content);
            }

            foreach (Solid solid in foreSolids)
            {
                solid.sprite.Draw(spriteBatch);
            }
        }
    }
}
