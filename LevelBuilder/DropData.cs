using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LevelBuilder
{
    public class DropData
    {
        public PlayerData.Weapon weapon;
        public Vector2 velocity;
        public int timeleft;
        static Random random = new Random();
    }
}
