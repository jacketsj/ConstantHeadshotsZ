using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LevelBuilder
{
    public class PlayerData
    {
        public float playerRotation;
        public Weapon weapon;
        public int swordAttacking;
        public int hammerAttacking;
        public int delayUntilHit;
        public int delay;
        public int health;
        public float score;
        public Vector2 position;
        public Color color;
        public string name;
        public bool isDead;

        public enum Weapon
        {
            NONE, BASIC, LAZERSWORD, HAMMER, ROCKETLAUNCHER
        }
    }
}
