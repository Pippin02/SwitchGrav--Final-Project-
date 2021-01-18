using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using System.Linq;

namespace SwitchGrav
{
    class PlayerSprite : Sprite
    {
        bool jumping, walking, falling, jumpPressed, hasCollided;   //is player jumping, walking, falling, has jum,p been pressed, has player sprite collided
        const float jumpSpeed = 3f, walkSpeed = 150f, gravStr = 8f;               //Constant variables for the player's jump and walk speed

        public PlayerSprite(Texture2D newSpriteSheet, Texture2D newColTex, Vector2 newPos) : base(newSpriteSheet, newColTex, newPos)
        {
            spriteOrigin = new Vector2(0.5f, 1f);                   //set origin of player sprite
            isColliding = true;                                     //Player can collide
            drawCollision = false;                                  //set whether to draw collision box

            collisionInsetMin = new Vector2(0.15f, 0.3f);           //Correction for collision box
            collisionInsetMax = new Vector2(0.15f, 0.04f);          //^

            frameTime = 0.1f;                                       //How long each frame of animation takes

            anims = new List<List<Rectangle>>();                    //Define 2D list for all animations

            //Idle Anim
            anims.Add(new List<Rectangle>());                       //Add empty animation
            anims[0].Add(new Rectangle(0, 0, 48, 48));              //Add first frame
            anims[0].Add(new Rectangle(0, 0, 48, 48));              //Duplicate first frame, to ensure correct sprite speed
            anims[0].Add(new Rectangle(0, 0, 48, 48));
            anims[0].Add(new Rectangle(48, 0, 48, 48));             //Add second frame
            anims[0].Add(new Rectangle(48, 0, 48, 48));             //Duplicate second frame, to ensure correct sprite speed
            anims[0].Add(new Rectangle(48, 0, 48, 48));

            //Walk Anim
            anims.Add(new List<Rectangle>());
            anims[1].Add(new Rectangle(96, 0, 48, 48));
            anims[1].Add(new Rectangle(48, 0, 48, 48));
            anims[1].Add(new Rectangle(144, 0, 48, 48));
            anims[1].Add(new Rectangle(48, 0, 48, 48));

            //Different Anim
            anims.Add(new List<Rectangle>());
            anims[2].Add(new Rectangle(98, 0, 48, 48));

            //Different Anim
            anims.Add(new List<Rectangle>());
            anims[3].Add(new Rectangle(0, 48, 48, 48));

            jumping = false;                                        //Player isn't jumping
            walking = false;                                        //Player isn't walking
            falling = true;                                         //Player isn't falling
            jumpPressed = false;                                    //Jump key hasn't been pressed yet
        }

        public void Update(GameTime gameTime, List<PlatformSprite> platforms, SoundEffect jumpSound, bool grav)
        {
            KeyboardState keyboardState = Keyboard.GetState();                                                  //Get current state of the keyboard
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);                                      //Get current state of the controller

            if (grav)
            {
                if (!jumpPressed && !jumping && !falling &&                                                         //If the player can jump
                    (keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A) || (keyboardState.IsKeyDown(Keys.W))))
                {
                    jumpPressed = true;                                                                             //Jump
                    jumping = true;
                    walking = false;
                    falling = false;
                    spriteVel.Y -= jumpSpeed;
                    jumpSound.Play();
                }
                else if (jumpPressed && !jumping && !falling &&
                    !(keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A)))
                {
                    jumpPressed = false;
                }

                if (keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyDown(Keys.D))
                {
                    walking = false;
                    spriteVel.X = 0;
                }
                else if (keyboardState.IsKeyDown(Keys.A))
                {
                    walking = true;
                    spriteVel.X = -walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    flipped = true;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    walking = true;
                    spriteVel.X = walkSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    flipped = false;
                }
                else
                {
                    walking = false;
                    spriteVel.X = 0;
                }

                if ((falling || jumping) && spriteVel.Y < 500)
                    spriteVel.Y += gravStr * (float)gameTime.ElapsedGameTime.TotalSeconds;              //If player is falling or jumping, increase player's velocity downwards until max is reached
            }

            spritePos += spriteVel;                                                                 //Update player's position based on velocity

            bool hasCollided = false;

            foreach (PlatformSprite platform in platforms)                                          //Check for collision on all sides of the player
            {
                if (checkCollisionBelow(platform))
                {
                    if (grav)
                    {
                        hasCollided = true;
                        while (checkCollision(platform))
                            spritePos.Y--;

                        spriteVel.Y = 0;
                        jumping = false;
                        falling = false;
                    }
                    else
                    {
                        spriteVel.Y *= -1;
                    }
                }
                else if (checkCollisionAbove(platform))
                {
                    if (grav)
                    {
                        hasCollided = true;
                        while (checkCollision(platform)) spritePos.Y++;
                        spriteVel.Y = 0;
                        jumping = false;
                        falling = true;
                    }
                    else
                    {
                        spriteVel.Y *= -1;
                    }
            }

                if (checkCollisionLeft(platform))
                {
                    if (grav)
                    {
                        hasCollided = true;
                        while (checkCollision(platform))
                            spritePos.X--;
                        spriteVel.X = 0;
                    }
                    else
                    {
                        spriteVel.X *= -1;
                    }
            }
                else if (checkCollisionRight(platform))
                {
                    if (grav)
                    {
                        hasCollided = true;
                        while (checkCollision(platform))
                            spritePos.X++;
                        spriteVel.X = 0;
                    }
                    else
                    {
                        spriteVel.X *= -1;
                    }
                }

                if (!hasCollided && walking)
                    falling = true;

                if (jumping && spriteVel.Y > 0)
                {
                    jumping = false;
                    falling = true;
                }

                if (grav)
                {
                    if (walking) setAnim(1);                //If walking is true, set player's anim to walking anim
                    else if (falling) setAnim(3);           //Same for falling
                    else if (jumping) setAnim(2);           //Same for jumping
                    else setAnim(0);                        //If none of these, default to idle animation
                }
                else
                {
                    setAnim(3);
                }
            }
        }
        public void resetPlayer(Vector2 newPos)             //Function to reset player position
        {
            spritePos = newPos;                             //Set sprite position
            spriteVel = new Vector2();                      //Set player's velocity to nothing
            jumping = false;                                //Set not jumping
            walking = false;                                //Set not walking
            falling = true;                                 //Set not falling
            jumpPressed = false;                            //Set jump button to not pressed
        }
    }
}