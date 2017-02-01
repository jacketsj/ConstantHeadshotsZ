using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ConstantHeadshotsZ
{
    public class Button
    {
        public Sprite sprite;
        public Color selectedTint = Color.Red;
        public bool clicked = false;

        public Button(ContentManager content, String textureName, Vector2 spriteVector)
        {
            Texture2D spriteTexture = content.Load<Texture2D>(textureName);
            sprite = new Sprite(spriteTexture, spriteVector);
        }

        public Button(Texture2D spriteTexture, Vector2 spriteVector)
        {
            sprite = new Sprite(spriteTexture, spriteVector);
        }

        public Button(Sprite newSprite)
        {
            sprite = newSprite;
        }

        public Button(ContentManager content, String textureName, Vector2 spriteVector, Color newSelectedTint)
        {
            Texture2D spriteTexture = content.Load<Texture2D>(textureName);
            sprite = new Sprite(spriteTexture, spriteVector);
            selectedTint = newSelectedTint;
        }

        public Button(Texture2D spriteTexture, Vector2 spriteVector, Color newSelectedTint)
        {
            sprite = new Sprite(spriteTexture, spriteVector);
            selectedTint = newSelectedTint;
        }

        public Button(Sprite newSprite, Color newSelectedTint)
        {
            sprite = newSprite;
            selectedTint = newSelectedTint;
        }

        public void Update(MouseState mouse)
        {
            clicked = false;
            if (sprite.Intersects(new Vector2(mouse.X, mouse.Y)))
            {
                sprite.setTint(selectedTint);
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    clicked = true;
                }
            }
            else
            {
                sprite.setTint(Color.White);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }
    }
}
