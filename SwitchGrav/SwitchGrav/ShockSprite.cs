using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SwitchGrav
{
    class ShockSprite : Sprite
    {
        public Texture2D sprite;

        public ShockSprite(Texture2D newSpriteSheet, Texture2D newCollisionTex, Vector2 newPos, bool hor, bool leftSide)
            : base(newSpriteSheet, newCollisionTex, newPos)
        {
            sprite = newSpriteSheet;

            spriteOrigin = new Vector2(0f, 1f);           //Set platform's origin to middle bottom
            isColliding = true;                             //Colliding is always true for shockers
            frameTime = 0.05f;
            drawCollision = false;

            anims = new List<List<Rectangle>>();            //Initialize 2D list for sprite animations
            anims.Add(new List<Rectangle>());               //Add empty animation
            if (hor)
            {
                if (!leftSide)
                {
                    collisionInsetMin = new Vector2(0.1f, 0.65f);
                    collisionInsetMax = new Vector2(0.1f, 0f);

                    anims[0].Add(new Rectangle(0, 64, 32, 32));
                    anims[0].Add(new Rectangle(32, 64, 32, 32));
                    anims[0].Add(new Rectangle(64, 64, 32, 32));
                    anims[0].Add(new Rectangle(32, 64, 32, 32));
                    anims[0].Add(new Rectangle(64, 64, 32, 32));
                    anims[0].Add(new Rectangle(32, 64, 32, 32));
                }
                else
                {
                    collisionInsetMin = new Vector2(0.1f, 0f);
                    collisionInsetMax = new Vector2(0.1f, 0.65f);

                    anims[0].Add(new Rectangle(0, 96, 32, 32));
                    anims[0].Add(new Rectangle(32, 96, 32, 32));
                    anims[0].Add(new Rectangle(64, 96, 32, 32));
                    anims[0].Add(new Rectangle(32, 96, 32, 32));
                    anims[0].Add(new Rectangle(64, 96, 32, 32));
                    anims[0].Add(new Rectangle(32, 96, 32, 32));
                }
            }
            else
            {
                if (!leftSide)
                {
                    flipped = true;

                    collisionInsetMin = new Vector2(0f, 0.1f);
                    collisionInsetMax = new Vector2(0.65f, 0.1f);
                }
                else
                {
                    collisionInsetMin = new Vector2(0.65f, 0.1f);
                    collisionInsetMax = new Vector2(0f, 0.1f);
                }

                anims[0].Add(new Rectangle(128, 0, 32, 32));
                anims[0].Add(new Rectangle(128, 32, 32, 32));
                anims[0].Add(new Rectangle(128, 64, 32, 32));
                anims[0].Add(new Rectangle(128, 32, 32, 32));
                anims[0].Add(new Rectangle(128, 64, 32, 32));
                anims[0].Add(new Rectangle(128, 32, 32, 32));
            }
        }
    }
}
