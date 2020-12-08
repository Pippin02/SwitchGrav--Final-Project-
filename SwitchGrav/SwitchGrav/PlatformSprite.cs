using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SwitchGrav
{
    class PlatformSprite : Sprite
    {
        public PlatformSprite(Texture2D newSpriteSheet, Texture2D newCollisionTex, Vector2 newPos)
            : base(newSpriteSheet, newCollisionTex, newPos)
        {
            spriteOrigin = new Vector2(0.5f, 0f);           //Set platform's origin to middle top
            isColliding = true;                             //Colliding is always true for platforms

            anims = new List<List<Rectangle>>();            //Initialize 2D list for sprite animations
            anims.Add(new List<Rectangle>());               //Add empty animation
            anims[0].Add(new Rectangle(0, 0, 96, 32));     //Add the single frame for the only animation
        }
    }
}
