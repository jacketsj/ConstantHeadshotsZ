using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ZombieShooter360
{
    class ScreenResizer
    {
        public Viewport newViewport;
        Viewport oldViewport;
        float movementMultiplier = 1;
        SpriteFont font;
        public bool firstUpdate = true;

        public ScreenResizer(Viewport viewPort)
        {
            newViewport = new Viewport(viewPort.Bounds);
            oldViewport = viewPort;
        }

        public void Update()
        {
            GamePadState currentGamePad = GamePad.GetState(PlayerIndex.One);

            Vector2 position1 = new Vector2(newViewport.X, newViewport.Y);
            Vector2 position2 = new Vector2(newViewport.X + newViewport.Width, newViewport.Y + newViewport.Height);

            int addedLeftStickX = (int)(currentGamePad.ThumbSticks.Left.X * movementMultiplier);
            int addedLeftStickY = (int)(currentGamePad.ThumbSticks.Left.Y * movementMultiplier);
            newViewport.X += addedLeftStickX;
            newViewport.Width -= addedLeftStickX;
            newViewport.Y += addedLeftStickY;
            newViewport.Height += addedLeftStickY;
            if (newViewport.X < 0)
            {
                newViewport.X = 0;
            }
            if (newViewport.Y < 0)
            {
                newViewport.Y = 0;
            }
            if (newViewport.X + newViewport.Width > oldViewport.X + oldViewport.Width)
            {
                newViewport.Width = newViewport.X - oldViewport.X;
            }
            if (newViewport.Y + newViewport.Height > oldViewport.Y + oldViewport.Height)
            {
                newViewport.Height = newViewport.Y - oldViewport.Y;
            }

            int addedRightStickX = (int)(currentGamePad.ThumbSticks.Right.X * movementMultiplier);
            int addedRightStickY = (int)(currentGamePad.ThumbSticks.Right.Y * movementMultiplier);
            newViewport.Width += addedRightStickX;
            newViewport.Height += addedRightStickY;
            if (newViewport.X < 0)
            {
                newViewport.X = 0;
            }
            if (newViewport.Y < 0)
            {
                newViewport.Y = 0;
            }
            if (newViewport.X + newViewport.Width > oldViewport.X + oldViewport.Width)
            {
                newViewport.Width = newViewport.X - oldViewport.X;
            }
            if (newViewport.Y + newViewport.Height > oldViewport.Y + oldViewport.Height)
            {
                newViewport.Height = newViewport.Y - oldViewport.Y;
            }

            firstUpdate = false;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphics, ContentManager Content)
        {
            graphics.Viewport = newViewport;
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("White"), newViewport.Bounds, Color.White);
            spriteBatch.End();
        }
    }
}
