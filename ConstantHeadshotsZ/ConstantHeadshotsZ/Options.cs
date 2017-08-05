using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZ
{

    public class Options
    {
        private static Options options = new Options();

        public bool usingController = false;
        public bool player1CameraRotation = true;
        public bool player2CameraRotation = false;
        public float player1CameraRotationSpeed = 0.02f;
        public float player2CameraRotationSpeed = 0.02f;
        public bool player13D = false;
        public bool player23D = false;
        public Color bldCol = Color.MediumSeaGreen;
        public int minLParticles = 100;
        public int maxLParticles = 200;
        public bool enablePitchChange = true;
        public int screenWidth = 800;
        public int windowedScreenWidth = 800;
        public int screenHeight = 600;
        public int windowedScreenHeight = 600;

        private Options()
        {
            #if XBOX
            usingController = true;
            #endif
        }

        public static Options GetInstance()
        {
            return options;
        }
    }
}
