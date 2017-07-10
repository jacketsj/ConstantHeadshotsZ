using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZ
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
            #if XBOX
            return;
            #endif

            string fullpath = @"Content\" + filename;

            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                serializer.Serialize(stream, data);
            }
            finally
            {
                stream.Close();
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

            string fullpath = @"Content\" + filename;

            FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                data = (HighScores)serializer.Deserialize(stream);
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
    }
}
