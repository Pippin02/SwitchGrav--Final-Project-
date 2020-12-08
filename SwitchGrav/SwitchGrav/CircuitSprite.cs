using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace SwitchGrav
{
    class CircuitSprite : Sprite
    {
        Random rnd = new Random();
        public int newTex = 0;

        public CircuitSprite(Texture2D newSpriteSheet, Texture2D newCollisionTex, Vector2 newPos)
            : base(newSpriteSheet, newCollisionTex, newPos)
        {
            spriteOrigin = new Vector2(0.5f, 0.5f);         //Set circuit's origin to middle center
            isColliding = true;                             //Colliding is always true for circuits
            frameTime = 0.3f;                               //Set animation speed

            collisionInsetMin = new Vector2(0.2f, 0.2f);    //Correction for collision box
            collisionInsetMax = new Vector2(0.2f, 0.2f);    //^

            anims = new List<List<Rectangle>>();            //Initialize 2D list for sprite animations
            anims.Add(new List<Rectangle>());               //Add empty animation
            anims[0].Add(new Rectangle(0, 0, 48, 48));      //Add first image
            anims[0].Add(new Rectangle(48, 0, 48, 48));     //Add second image
            anims[0].Add(new Rectangle(96, 0, 48, 48));     //Add third image
            anims[0].Add(new Rectangle(48, 0, 48, 48));     //Add fourth image
        }

        public void randomSprite()
        {
            int lastTex = newTex;
            while(newTex == lastTex)
                newTex = rnd.Next(3);
            anims[0].Clear();

            if (newTex == 0)
            {
                anims[0].Add(new Rectangle(0, 0, 48, 48));      //Add first image
                anims[0].Add(new Rectangle(48, 0, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(96, 0, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(48, 0, 48, 48));     //Add last image
            }
            else if (newTex == 1)
            {
                anims[0].Add(new Rectangle(0, 48, 48, 48));      //Add first image
                anims[0].Add(new Rectangle(48, 48, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(96, 48, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(48, 48, 48, 48));     //Add last image
            }
            else if (newTex == 2)
            {
                anims[0].Add(new Rectangle(0, 96, 48, 48));      //Add first image
                anims[0].Add(new Rectangle(48, 96, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(96, 96, 48, 48));     //Add first image
                anims[0].Add(new Rectangle(48, 96, 48, 48));     //Add last image
            }
        }
    }
}