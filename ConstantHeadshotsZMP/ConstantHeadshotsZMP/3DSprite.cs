using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZMP
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
        /// Gets or sets how fast this entity is moving.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            protected set { velocity = value; }
        }

        Vector3 velocity;


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
            Matrix world = Matrix.CreateTranslation(0, 1, 0) * Matrix.CreateScale(800) * Matrix.CreateConstrainedBillboard(Position, cameraPosition, Up, null, null);

            quadDrawer.DrawQuad(Texture, 1, world, view, projection);
        }
    }
}
