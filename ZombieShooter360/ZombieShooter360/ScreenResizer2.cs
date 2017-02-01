using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace ZombieShooter360
{
    class ScreenResizer2
    {
        Viewport newViewport;
        //Vector2 position1;
        Vector2 minPosition1;
        //Vector2 position2;
        Vector2 minPosition2;
        ContentManager Content;
        Texture2D visibleTexture;
        Texture2D whiteTexture;
        float movementMultiplier = 3;
        SpriteFont font;
        Vector2 oldScreenWidthHeight;
        public bool firstUpdate = true;

        public ScreenResizer2(ContentManager Content, Viewport viewPort)
        {
            minPosition1 = new Vector2(viewPort.Width / 2 - 40, viewPort.Height / 2 - 40);
            minPosition2 = new Vector2(viewPort.Width / 2 + 40, viewPort.Height / 2 + 40);
            newViewport = new Viewport(viewPort.Bounds);
            //position1 = new Vector2(0, 0);
            //position2 = EstimatedScreenWidthAndHeight;
            this.Content = Content;
            visibleTexture = Content.Load<Texture2D>("VisibleArea");
            whiteTexture = Content.Load<Texture2D>("White");
            font = Content.Load<SpriteFont>("basic");
            oldScreenWidthHeight = new Vector2(viewPort.Width, viewPort.Height);
        }

        public Viewport Finalize(GraphicsDevice graphics)
        {
            //Viewport returnView = graphics.Viewport;
            /*
            returnView.X = (int)position1.X;
            returnView.Y = (int)position1.Y;
            returnView.Width = (int)position2.X - (int)position1.X;
            returnView.Height = (int)position2.Y - (int)position1.Y;
            */
            return newViewport;
            //returnView = new Viewport(new Rectangle((int)position1.X, (int)position1.Y, (int)position2.X - (int)position1.X, (int)position2.Y - (int)position1.Y));
            //return returnView;
        }

        public void Update()
        {
            GamePadState currentGamePad = GamePad.GetState(PlayerIndex.One);

            Vector2 position1 = new Vector2(newViewport.X, newViewport.Y);
            Vector2 position2 = new Vector2(newViewport.X + newViewport.Width, newViewport.Y + newViewport.Height);

            setPosition1(new Vector2(position1.X + currentGamePad.ThumbSticks.Left.X * movementMultiplier, position1.X + currentGamePad.ThumbSticks.Left.Y * movementMultiplier));
            setPosition2(new Vector2(position2.X + currentGamePad.ThumbSticks.Right.X * movementMultiplier, position2.X + currentGamePad.ThumbSticks.Right.Y * movementMultiplier));

            firstUpdate = false;
        }

        public void setPosition1(Vector2 newPosition)
        {
            Vector2 oldPosition = new Vector2(newViewport.X, newViewport.Y);
            newViewport.X = (int)newPosition.X;
            newViewport.Y = (int)newPosition.Y;
            newViewport.Width += (int)oldPosition.X - newViewport.X;
            newViewport.Height += (int)oldPosition.Y - newViewport.Y;
            if (newViewport.X > (int)minPosition1.X)
            {
                newViewport.X = (int)minPosition1.X;
            }
            if (newViewport.Y > (int)minPosition1.Y)
            {
                newViewport.Y = (int)minPosition1.Y;
            }
            if (newViewport.X < (int)oldScreenWidthHeight.X)
            {
                newViewport.X = (int)oldScreenWidthHeight.X;
            }
            if (newViewport.Y < (int)oldScreenWidthHeight.Y)
            {
                newViewport.Y = (int)oldScreenWidthHeight.Y;
            }
        }

        public void setPosition2(Vector2 newPosition)
        {
            newViewport.Width = (int)newPosition.X + newViewport.X;
            newViewport.Height = (int)newPosition.Y + newViewport.Y;
            if (newViewport.Width + newViewport.X < (int)minPosition2.X)
            {
                newViewport.Width = (int)minPosition2.X - newViewport.X;
            }
            if (newViewport.Height + newViewport.Y < (int)minPosition2.Y)
            {
                newViewport.Height = (int)minPosition2.Y - newViewport.Y;
            }
            if (newViewport.Width + newViewport.X > (int)oldScreenWidthHeight.X)
            {
                newViewport.Width = (int)oldScreenWidthHeight.X - newViewport.X;
            }
            if (newViewport.Height + newViewport.Y > (int)oldScreenWidthHeight.Y)
            {
                newViewport.Height = (int)oldScreenWidthHeight.Y - newViewport.Y;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            Vector2 position1 = new Vector2(newViewport.X, newViewport.Y);
            Vector2 position2 = new Vector2(newViewport.X + newViewport.Width, newViewport.Y + newViewport.Width);
            spriteBatch.Draw(whiteTexture, new Rectangle((int)position1.X, (int)position1.Y, (int)position2.X - (int)position1.X, 6), Color.Black);
            spriteBatch.Draw(whiteTexture, new Rectangle((int)position1.X, (int)position1.Y, 6, (int)position2.Y - (int)position1.Y), Color.Black);
            spriteBatch.Draw(whiteTexture, new Rectangle((int)position1.X, (int)position2.Y, (int)position2.X - (int)position1.X, 6), Color.Black);
            spriteBatch.Draw(whiteTexture, new Rectangle((int)position2.X, (int)position1.Y, 6, (int)position2.Y - (int)position1.Y), Color.Black);
            String LS = "LS";
            Vector2 LSOrigin = new Vector2(3);
            String RS = "RS";
            Vector2 RSOrigin = font.MeasureString(LS) + new Vector2(3);
            String Finalize = "FINALIZE";
            Vector2 FinalizeOrigin = new Vector2(font.MeasureString(Finalize).X / 2, font.MeasureString(Finalize).Y);
            spriteBatch.DrawString(font, LS, position1 + LSOrigin, Color.Black);
            spriteBatch.DrawString(font, RS, position2 - RSOrigin, Color.Black);
            spriteBatch.DrawString(font, Finalize,new Vector2(position1.X + (position2.X - position1.X) / 2, position2.Y - 3) - FinalizeOrigin, Color.Black);
            Texture2D AButtonTexture = Content.Load<Texture2D>("AButton");
            spriteBatch.Draw(AButtonTexture, new Vector2(position1.X + (position2.X - position1.X) / 2 - AButtonTexture.Width / 2, position2.Y - font.MeasureString(Finalize).Y - AButtonTexture.Height - 3), Color.White);
            spriteBatch.End();
        }
    }
}
