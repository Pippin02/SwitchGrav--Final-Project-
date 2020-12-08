using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace SwitchGrav
{
    class StarSprite : Sprite
    {
        float speed = 100f;
        Random rnd = new Random();

        public StarSprite(Texture2D newSpriteSheet, Texture2D newCollisionTex, Vector2 newPos, Point screenSize)
            : base(newSpriteSheet, newCollisionTex, newPos)
        {
            spriteOrigin = new Vector2(0.5f, 0.5f);         //Set door's origin to middle center
            isColliding = false;                            //Colliding is always true for doors
            frameTime = 0f;                                 //Set animation speed

            collisionInsetMin = new Vector2(0.2f, 0.2f);    //Correction for collision box
            collisionInsetMax = new Vector2(0.2f, 0.2f);    //^

            anims = new List<List<Rectangle>>();            //Initialize 2D list for sprite animations
            anims.Add(new List<Rectangle>());               //Add empty animation
            anims[0].Add(new Rectangle(0, 0, 2, 2));        //Add first image

            spritePos.X = (float)rnd.Next(screenSize.X);
            spritePos.Y = (float)rnd.Next(screenSize.Y);

            if (newPos.X == 0)
            {
                spritePos.X += screenSize.X;
            }

            speed = (float)rnd.Next(50, 100);

            }

        public override void Update(GameTime gameTime)
        {
            spritePos.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
        }
    }
}
