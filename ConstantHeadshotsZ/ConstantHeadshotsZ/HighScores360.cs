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
    public class HighScores360
    {
        public float[] Score;
        public string[] Name;

        public int Count;

        public HighScores360()
        {
            Count = 5;

            Score = new float[Count];
            Name = new string[Count];
        }

        public static void SaveHighScores(HighScores360 data, string filename, StorageDevice storageDevice)
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
                    XmlSerializer serializer = new XmlSerializer(typeof(HighScores360));
                    serializer.Serialize(stream, data);
                }
                finally
                {
                    stream.Close();
                }
                container.Dispose();
            }
        }

        public static HighScores360 LoadHighScores(string filename, StorageDevice storageDevice)
        {
            HighScores360 data = new HighScores360();

            try
            {
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
                            XmlSerializer serializer = new XmlSerializer(typeof(HighScores360));
                            data = (HighScores360)serializer.Deserialize(stream);
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
            }
            catch (Exception e)
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
