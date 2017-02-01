using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;

namespace ZombieShooter360
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

        public static void SaveHighScores(HighScores data, string filename, StorageDevice storageDevice)
        {
            if (storageDevice.IsConnected)
            {
                IAsyncResult result = storageDevice.BeginOpenContainer("Highscores", null, null);

                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = storageDevice.EndOpenContainer(result);

                result.AsyncWaitHandle.Close();

                if (container.FileExists(filename))
                {
                    container.DeleteFile(filename);
                }

                Stream stream = container.CreateFile(filename);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                    serializer.Serialize(stream, data);
                }
                finally
                {
                    stream.Close();
                }
                container.Dispose();
            }
        }

        public static HighScores LoadHighScores(string filename, StorageDevice storageDevice)
        {
            HighScores data = new HighScores();

            if (storageDevice.IsConnected)
            {
                IAsyncResult result = storageDevice.BeginOpenContainer("Highscores", null, null);

                result.AsyncWaitHandle.WaitOne();

                StorageContainer container = storageDevice.EndOpenContainer(result);

                result.AsyncWaitHandle.Close();

                if (!container.FileExists(filename))
                {
                    container.Dispose();
                    data.Score[0] = 20000;
                    data.Name[0] = "jacketsj";
                    data.Score[1] = 18000;
                    data.Name[1] = "jacketsj";
                    data.Score[2] = 11000;
                    data.Name[2] = "jacketsj";
                    data.Score[3] = 9000;
                    data.Name[3] = "jacketsj";
                    data.Score[4] = 100;
                    data.Name[4] = "jacketsj";
                }
                else
                {
                    Stream stream = container.OpenFile(filename, FileMode.Open);
                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(HighScores));
                        data = (HighScores)serializer.Deserialize(stream);
                    }
                    finally
                    {
                        stream.Close();
                    }
                    container.Dispose();
                }
            }
            else
            {
                data.Score[0] = 20000;
                data.Name[0] = "jacketsj";
                data.Score[1] = 18000;
                data.Name[1] = "jacketsj";
                data.Score[2] = 11000;
                data.Name[2] = "jacketsj";
                data.Score[3] = 9000;
                data.Name[3] = "jacketsj";
                data.Score[4] = 100;
                data.Name[4] = "jacketsj";
            }
            return data;
        }
    }
}
