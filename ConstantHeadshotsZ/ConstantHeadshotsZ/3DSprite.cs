using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZ
{
    public class _3DSprite
    {
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector3 position;


        /// <summary>
        /// Gets or sets which way the entity is facing.
        /// </summary>
        public Vector3 Forward
        {
            get { return forward; }
            set { forward = value; }
        }

        Vector3 forward;


        /// <summary>
        /// Gets or sets the orientation of this entity.
        /// </summary>
        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        Vector3 up;


        /// <summary>
        /// Gets or sets the texture used to display this entity.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        Texture2D texture;

        /// <summary>
        /// Draws the entity as a billboard sprite.
        /// </summary>
        public void Draw(QuadDrawer quadDrawer, Vector3 cameraPosition,
                         Matrix view, Matrix projection)
        {
            
            quadDrawer.DrawQuad(Texture, 1, Matrix.Identity, view, projection);
        }

        /// <summary>
        /// Draws the entity as a billboard sprite.
        /// </summary>
        public void Draw(QuadDrawer quadDrawer, Vector3 cameraPosition,
                         Matrix view, Matrix projection, Matrix world)
        {
            quadDrawer.DrawQuad(Texture, 1,
                world
                * Matrix.CreateScale(texture.Width / 2, texture.Height / 2, 1)
                    * Matrix.CreateRotationX(Up.X) * Matrix.CreateRotationY(Up.Y) * Matrix.CreateRotationZ(Up.Z) * Matrix.CreateRotationZ(MathHelper.Pi)
                    * (Matrix.CreateTranslation(position + new Vector3(texture.Width / 2, texture.Height / 2, 0)))
                , view, projection);
        }
    }
}
