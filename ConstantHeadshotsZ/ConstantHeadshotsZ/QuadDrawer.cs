﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ConstantHeadshotsZ
{
    public class QuadDrawer
    {
        GraphicsDevice graphicsDevice;
        AlphaTestEffect effect;
        VertexPositionTexture[] vertices;



        /// <summary>
        /// Constructs a new quadrilateral drawing worker.
        /// </summary>
        public QuadDrawer(GraphicsDevice device)
        {
            graphicsDevice = device;

            effect = new AlphaTestEffect(device);

            effect.AlphaFunction = CompareFunction.Greater;
            effect.ReferenceAlpha = 128;

            // Preallocate an array of four vertices.
            vertices = new VertexPositionTexture[4];

            vertices[0].Position = new Vector3(1, 1, 0);
            vertices[1].Position = new Vector3(-1, 1, 0);
            vertices[2].Position = new Vector3(1, -1, 0);
            vertices[3].Position = new Vector3(-1, -1, 0);
        }


        /// <summary>
        /// Draws a quadrilateral as part of the 3D world.
        /// </summary>
        public void DrawQuad(Texture2D texture, float textureRepeats,
                             Matrix world, Matrix view, Matrix projection)
        {
            // Set our effect to use the specified texture and camera matrices.
            effect.Texture = texture;

            effect.World = world;
            effect.View = view;
            effect.Projection = projection;

            // Update our vertex array to use the specified number of texture repeats.
            vertices[0].TextureCoordinate = new Vector2(0, 0);
            vertices[1].TextureCoordinate = new Vector2(textureRepeats, 0);
            vertices[2].TextureCoordinate = new Vector2(0, textureRepeats);
            vertices[3].TextureCoordinate = new Vector2(textureRepeats, textureRepeats);

            // Draw the quad.
            effect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
        }
    }
}
