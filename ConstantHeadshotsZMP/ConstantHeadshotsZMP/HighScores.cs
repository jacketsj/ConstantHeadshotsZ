using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZMP
{
    public class HighScores
    {
        public float[] Score;
        public string[] Name;

        public int Count;

        public HighScores()
        {
            Count = 5;

            Score = new float[Count];
            Name = new string[Count];
        }

        public static void SaveHighScores(HighScores data, string filename)
        {
            //StorageDevice device = GetDevice(data.result);
            //IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
            
            try
            {
                string fullpath = @"Content\" + filename;

                FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
                XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                serializer.Serialize(stream, data);
                stream.Close();
            }
            finally
            {
            }
        }

        static StorageDevice GetDevice(IAsyncResult result)
        {
            StorageDevice device = StorageDevice.EndShowSelector(result);
            return device;
        }

        public static HighScores LoadHighScores(string filename)
        {
            HighScores data = new HighScores();

            
            try
            {
                string fullpath = @"Content\" + filename;

                FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                data = (HighScores)serializer.Deserialize(stream);
                stream.Close();
            }
            catch (Exception e)
            {

            }
            finally
            {
                
            }

            return data;
        }
    }
}
