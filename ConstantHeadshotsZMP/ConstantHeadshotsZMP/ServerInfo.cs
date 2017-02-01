using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelBuilder;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZMP
{
    [Serializable]
    public class ServerInfo
    {
        public OnlineLevelData level;

        public ServerInfo()
        {
            level = new OnlineLevelData();
            level.backgroundColor = Color.White;
            level.bullets = new BasicWeaponBulletData[0];
            level.players = new PlayerData[0];
            level.playerSpawn = Vector2.Zero;
            level.random = new Random();
            level.rockets = new RocketData[0];
            level.solidData = new SolidData[0];
            level.textureData = new TextureData[0];
            level.zombieSpawners = new Vector2[0];
        }
    }
}
