using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;

namespace LevelBuilder
{
    public class OnlineLevelData
    {
        //public LevelData levelData;
        public SolidData[] solidData;
        public TextureData[] textureData;
        public PlayerData[] players;
        public BasicWeaponBulletData[] bullets;
        public RocketData[] rockets;
        public Color backgroundColor;
        public Vector2 playerSpawn;
        public int levelHeight;
        public int levelWidth;
        public Vector2[] zombieSpawners;
        public int BackgroundReference;
        public Random random;
        public int spawnTimer;
        public int defaultSpawnTimer;
        public int timesSpawned;
        public int maxAmountOfZombies;
        public bool zombieSpawnAcceleration;
    }
}
