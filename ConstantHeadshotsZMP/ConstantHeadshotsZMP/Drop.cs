using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ConstantHeadshotsZMP
{
    [Serializable]
    public class Drop
    {
        public Player.Weapon weapon;
        public Sprite sprite;
        public Vector2 velocity;
        public int timeleft;
        static Random random = new Random();

        public Drop()
        {

        }

        public Drop(Player.Weapon weapon, Sprite sprite, Vector2 velocity, int timeleft)
        {
            this.weapon = weapon;
            this.sprite = sprite;
            this.velocity = velocity;
            this.timeleft = timeleft;
        }

        /*
        public bool Update(Player player)
        {
            sprite.vector += velocity;
            velocity = (velocity / 3) * 2;
            timeleft -= 1;
            if (timeleft == 0 || CollidesWithPlayer(player))
            {
                return true;
            }
            return false;
        }
        */

        public bool Update(Player[] players)
        {
            sprite.vector += velocity;
            velocity = (velocity / 3) * 2;
            timeleft -= 1;
            if (timeleft == 0 || CollidesWithPlayers(players))
            {
                return true;
            }
            return false;
        }
        public bool CollidesWithPlayers(Player[] players)
        {
            Rectangle dropRec = new Rectangle((int)sprite.getX(), (int)sprite.getY(), sprite.getTexture().Width, sprite.getTexture().Height);
            foreach (Player player in players)
            {
                if (dropRec.Intersects(new Rectangle((int)player.sprite.getX(), (int)player.sprite.getY(), player.sprite.getTexture().Width, player.sprite.getTexture().Height)))
                {
                    player.weapon = weapon;
                    return true;
                }
            }
            return false;
        }

        public bool CollidesWithPlayer(Player player)
        {
            if ((new Rectangle((int)sprite.getX(), (int)sprite.getY(), sprite.getTexture().Width, sprite.getTexture().Height)).Intersects(new Rectangle((int)player.sprite.getX(), (int)player.sprite.getY(), player.sprite.getTexture().Width, player.sprite.getTexture().Height)))
            {
                player.weapon = weapon;
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, ContentManager Content)
        {
            spriteBatch.Draw(sprite.getTexture(), new Vector2(sprite.getX() - sprite.getTexture().Width / 2, sprite.getY() - sprite.getTexture().Height / 2), sprite.getTint());
            //sprite.Draw(spriteBatch);
        }

        public static Player.Weapon GetRandomWeapon()
        {
            Player.Weapon weapon;
            int randomNum = (int)(random.NextDouble() * 4);
            //return Player.Weapon.LAZERSWORD;
            if (randomNum == 1)
            {
                weapon = Player.Weapon.LAZERSWORD;
            }
            else if (randomNum == 2)
            {
                weapon = Player.Weapon.HAMMER;
            }
            else if (randomNum == 3)
            {
                weapon = Player.Weapon.ROCKETLAUNCHER;
            }
            else
            {
                weapon = Player.Weapon.BASIC;
            }
            return weapon;
        }

        public static Drop[] DropWeapon(Player.Weapon weapon, Vector2 location, ContentManager Content, Drop[] drops)
        {
            Sprite sprite;
            if (weapon == Player.Weapon.LAZERSWORD)
            {
                sprite = new Sprite(Content.Load<Texture2D>("LazerSwordDrop"), location);
            }
            else if (weapon == Player.Weapon.HAMMER)
            {
                sprite = new Sprite(Content.Load<Texture2D>("HammerDrop"), location);
            }
            else if (weapon == Player.Weapon.ROCKETLAUNCHER)
            {
                sprite = new Sprite(Content.Load<Texture2D>("RocketLauncherDrop"), location);
            }
            else
            {
                sprite = new Sprite(Content.Load<Texture2D>("BasicWeaponDrop"), location);
            }
            Drop drop = new Drop(weapon, sprite, Vector2.Zero, 1000);
            if (drops != null && drops.Length != 0)
            {
                Drop[] newDrops = new Drop[drops.Length + 1];

                for (int i = 0; i < drops.Length; i++)
                {
                    newDrops[i + 1] = drops[i];
                }
                newDrops[0] = drop;
                drops = newDrops;
                return drops;
            }
            else
            {
                drops = new Drop[1];
                drops[0] = drop;
                return drops;
            }
            
        }
    }
}
