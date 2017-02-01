using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZombieShooter360
{
    class Camera
    {
        protected float zoom;
        public Matrix transform;
        public Vector2 position;
        protected float rotation;
        public Sprite followedSprite = null;
 
        public Camera()
        {
            zoom = 1.0f;
            rotation = 0.0f;
            position = Vector2.Zero;
        }

        public Camera(Sprite newFollowedSprite)
        {
            zoom = 1.0f;
            rotation = 0.0f;
            position = Vector2.Zero;
            followedSprite = newFollowedSprite;
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
            rotation = newRotation;
        }

        public float getRotation()
        {
            return rotation;
        }

        public void setPosition(Vector2 newPosition, Level level, Vector2 screenWidthAndHeight)
        {
            position = newPosition;
            if (position.X - screenWidthAndHeight.X / 2 < 0)
            {
                position.X = (screenWidthAndHeight.X) / 2;
            }
            if (position.Y - screenWidthAndHeight.Y / 2 < 0)
            {
                position.Y = (screenWidthAndHeight.Y ) / 2;
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
            transform = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0)) * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(new Vector3(zoom, zoom, 1)) * Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f, graphicsDevice.Viewport.Height * 0.5f, 0));                       
            return transform;
        }

        public void Update(Level level, Vector2 screenWidthAndHeight)
        {
            if (followedSprite != null)
            {
                setPosition(new Vector2(followedSprite.getX(), followedSprite.getY()), level, screenWidthAndHeight);
                //setPosition(new Vector2(followedSprite.getX() + (followedSprite.getTexture().Width / 2), followedSprite.getY() + (followedSprite.getTexture().Height / 2)));
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

        public void Update2Player(Level level, Vector2 screenWidthAndHeight)
        {
            if (followedSprite != null)
            {
                setPosition(new Vector2(followedSprite.getX(), followedSprite.getY()), level, new Vector2(screenWidthAndHeight.X / 2, screenWidthAndHeight.Y));
            }
        }
    }
}
