using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LevelBuilder
{
    public class ZombieData
    {
        public float rotation;
        public int health;
        PlayerData followedPlayer;
        Random random;
        public Vector2 position;
    }
}
