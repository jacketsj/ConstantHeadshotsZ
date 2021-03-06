﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConstantHeadshotsZ
{
    public class Camera
    {
        protected float zoom;
        public Matrix transform;
        public Vector2 position;
        protected float yaw;
        protected float pitch;
        public Sprite followedSprite = null;
        public BasicEffect effect;
        public float effectRotation = 0.02f;
 
        public Camera(GraphicsDevice graphics)
        {
            zoom = 1.0f;
            yaw = 0.0f;
            position = Vector2.Zero;
            effect = new BasicEffect(graphics);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, 0, 1) * Matrix.CreateRotationY(effectRotation);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = halfPixelOffset * projection;

            effect.Projection = Matrix.CreateRotationY(0.02f);
        }

        public Camera(Sprite newFollowedSprite, GraphicsDevice graphics)
        {
            zoom = 1.0f;
            yaw = 0.0f;
            position = Vector2.Zero;
            followedSprite = newFollowedSprite;
            effect = new BasicEffect(graphics);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, 0, 1) * Matrix.CreateRotationY(effectRotation);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = halfPixelOffset * projection;

            effect.Projection = Matrix.CreateRotationY(0.02f);
        }

        public void updateEffect(GraphicsDevice graphics)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, graphics.Viewport.Width, graphics.Viewport.Height, 0, 0, 1) * Matrix.CreateRotationY(effectRotation);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = halfPixelOffset * projection;

            effect.Projection = Matrix.CreateRotationY(0.02f);
        }

        public void setZoom(float newZoom)
        {
            zoom = newZoom;
            if (zoom < 0.1f)
            {
                zoom = 0.1f;
            }
        }

        public float getZoom()
        {
            return zoom;
        }

        public void setRotation(float newRotation)
        {
            yaw = newRotation;
        }

        public void setYaw(float newYaw)
        {
            yaw = newYaw;
        }

        public void setPitch(float newPitch)
        {
            if (Options.GetInstance().enablePitchChange)
            {
                pitch = newPitch;
            }
        }

        public float getRotation()
        {
            return yaw;
        }

        public float getYaw()
        {
            return yaw;
        }

        public float getPitch()
        {
            return pitch;
        }

        public void setPosition(Vector2 newPosition, Level level, Vector2 screenWidthAndHeight)
        {
            position = newPosition;
            if (!Options.GetInstance().player13D)
            {
                if (position.X - screenWidthAndHeight.X / 2 < 0)
                {
                    position.X = screenWidthAndHeight.X / 2;
                }
                if (position.Y - screenWidthAndHeight.Y / 2 < 0)
                {
                    position.Y = screenWidthAndHeight.Y / 2;
                }
                if (position.X + screenWidthAndHeight.X / 2 > level.levelWidth)
                {
                    position.X = level.levelWidth - screenWidthAndHeight.X / 2;
                }
                if (position.Y + screenWidthAndHeight.Y / 2 > level.levelHeight)
                {
                    position.Y = level.levelHeight - screenWidthAndHeight.Y / 2;
                }
                if (level.levelWidth < screenWidthAndHeight.X)
                {
                    position.X = level.levelWidth / 2;
                }
                if (level.levelHeight < screenWidthAndHeight.Y)
                {
                    position.Y = level.levelHeight / 2;
                }
            }
        }

        public void SetPositionRegardless(Vector2 newPosition)
        {
            position = newPosition;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public void move(Vector2 addedVector)
        {
            position += addedVector;
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateRotationZ(yaw)
                * Matrix.CreateScale(new Vector3(zoom, zoom, 1))
                * Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));                       
            return transform;
        }

        public Matrix get_transformation_3d(GraphicsDevice graphicsDevice)
        {
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateRotationZ(yaw);
            transform *= Matrix.CreateRotationX(pitch);
            transform *= Matrix.CreateScale(new Vector3(zoom, zoom, 1)) * Matrix.CreateScale(1, -1, 1);
            return transform;
        }

        public void Update(Level level, Vector2 screenWidthAndHeight)
        {
            if (followedSprite != null)
            {
                setPosition(new Vector2(followedSprite.getX(), followedSprite.getY()), level, screenWidthAndHeight);
            }
        }

        public void UpdateWithRotation(float newRotation)
        {
            if (followedSprite != null)
            {
                SetPositionRegardless(new Vector2(followedSprite.getX(), followedSprite.getY()));
            }
            setRotation(-newRotation);
        }

        public void UpdateWithRotation(float newYaw, float newPitch)
        {
            if (followedSprite != null)
            {
                SetPositionRegardless(new Vector2(followedSprite.getX(), followedSprite.getY()));
            }
            setYaw(-newYaw);
            if (Options.GetInstance().enablePitchChange)
            {
                setPitch(-newPitch);
            }
        }

        public void Update2Player(Level level, Vector2 screenWidthAndHeight)
        {
            if (followedSprite != null)
            {
                setPosition(new Vector2(followedSprite.getX(), followedSprite.getY()), level, new Vector2(screenWidthAndHeight.X / 2, screenWidthAndHeight.Y));
            }
        }
    }
}
