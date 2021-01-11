using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SwitchGrav
{
    class PlatformSprite : Sprite
    {
        public Texture2D sprite;

        public PlatformSprite(Texture2D newSpriteSheet, Texture2D newCollisionTex, Vector2 newPos, bool hor)
            : base(newSpriteSheet, newCollisionTex, newPos)
        {
            sprite = newSpriteSheet;
            drawCollision = false;

            isColliding = true;                             //Colliding is always true for platforms

            anims = new List<List<Rectangle>>();            //Initialize 2D list for sprite animations
            anims.Add(new List<Rectangle>());               //Add empty animation
            if (hor)
            {
                spriteOrigin = new Vector2(0.5f, 0f);           //Set platform's origin to middle top
                anims[0].Add(new Rectangle(0, 0, 96, 32));     //Add the single frame for the only animation
            }
            else
            {
                spriteOrigin = new Vector2(0f, 0.5f);           //Set platform's origin to middle top
                anims[0].Add(new Rectangle(97, 0, 32, 96));
            }
        }
    }
}
