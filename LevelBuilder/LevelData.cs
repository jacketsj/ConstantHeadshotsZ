using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.Net;
#if WINDOWS
using System.Windows.Forms;
#endif

namespace LevelBuilder
{
    public class LevelData
    {
        public SolidData[] solids;
        public SolidData[] backSolids;
        public SolidData[] foreSolids;
        public int levelHeight;
        public int levelWidth;
        public int backgroundReference;
        public Color backgroundColor;
        public Vector2 playerSpawn;
        public Vector2[] zombieSpawners;
        public int maxAmountOfZombies = 60;
        public bool zombieSpawnAcceleration = false;
        public int spawnTimer = 0;
        public TextureData[] textures;

        public LevelData()
        {
            //Do nothing
        }

        /*
        public LevelData(SolidData[] newSolids, int newLevelHeight, int newLevelWidth, Texture2D newBackground, Color newBackgroundColor, Vector2 newPlayerSpawn, Vector2[] newZombieSpawners)
        {
            solids = newSolids;
            levelHeight = newLevelHeight;
            levelWidth = newLevelWidth;
            background = newBackground;
            backgroundColor = newBackgroundColor;
            playerSpawn = newPlayerSpawn;
            zombieSpawners = newZombieSpawners;
        }
        */

        public static void SaveLevel(LevelData data)
        {
            #if WINDOWS
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Constant Headshots Z Map|*.chz";
            saveFileDialog1.Title = "Save your map";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {

                FileStream stream = (System.IO.FileStream)saveFileDialog1.OpenFile();
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
                    serializer.Serialize(stream, data);
                }
                finally
                {
                    stream.Close();
                }
            }
            #endif
        }

        public static LevelData LoadLevel()
        {
            #if WINDOWS
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Constant Headshots Z Map|*.chz";
            openFileDialog1.Title = "Load a map";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {

                FileStream stream = (System.IO.FileStream)openFileDialog1.OpenFile();
                LevelData data = new LevelData();

                //string fullpath = @"Content\" + filename;

                //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
                    data = (LevelData)serializer.Deserialize(stream);
                }
                catch (InvalidOperationException e)
                {

                }
                finally
                {
                    stream.Close();
                }

                return data;
            }
            #endif
            return null;
        }

        public static LevelData LoadLevel360(string URL)
        {
            #if XBOX

            

            /*
            LevelData data = new LevelData();
            FileStream stream = (System.IO.FileStream)
            XmlSerializer serializer = new XmlSerializer(typeof(LevelData));
            data = (LevelData)serializer.Deserialize(stream);
            return data;
            */
            #endif

            return null;
        }
    }
}
