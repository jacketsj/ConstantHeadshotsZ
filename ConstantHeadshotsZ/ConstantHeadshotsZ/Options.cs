using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConstantHeadshotsZ
{
    public class Options
    {
        public bool usingController = false;
        public bool player1CameraRotation = false;
        public bool player2CameraRotation = false;
        public float player1CameraRotationSpeed = 0.08f;
        public float player2CameraRotationSpeed = 0.08f;
        public bool player13D = false;
        public bool player23D = false;

        public Options()
        {
            #if XBOX
            usingController = true;
            #endif
        }
    }
}
