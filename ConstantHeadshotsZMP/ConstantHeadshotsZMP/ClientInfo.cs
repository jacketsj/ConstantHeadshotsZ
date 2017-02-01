using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelBuilder;

namespace ConstantHeadshotsZMP
{
    [Serializable]
    public class ClientInfo
    {
        public PlayerData player;
        public LevelBuilder.BasicWeaponBulletData[] newBullets;
        public RocketData[] newRockets;

        public ClientInfo()
        {
            newBullets = new BasicWeaponBulletData[0];
            newRockets = new RocketData[0];
            player = new PlayerData();
            player.name = "player";
        }
    }
}
