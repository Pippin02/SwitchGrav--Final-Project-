using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwitchGrav
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Point screenSize = new Point(1600, 900);                                            //Two integers that define the size of the window
        int currentLevel = 0, prevLevel = 0;                                                //Integer for the current level number
        int lives = 0, startLives = 4;                                                      //Number of player lives
        int circuitCount = 0;                                                               //Number of levels completed
        bool gameOver = true;                                                               //Set to true if player is out of lives
        int starNum = 0;
        bool gravityOn = true;
        float gravTimer = 3f, gravPercent, gravWidth;
        const float maxTimer = 3f;
        string gravString = "On";
        Color gravColor;
        int gameOverType = 0;
        bool firstRoundDone = false;
        Random rnd = new Random();

        Texture2D backTex, circuitSheet, platformSheet, whiteBox, playerSheet;              //Define all Texture2Ds in the game
        SpriteFont font, bigFont;                                                           //Define fonts for the UI
        public SoundEffect jumpSound, deathSound, circuitSound, gravUp, gravDown;           //Define sound effects
        Song music;

        List<Vector2> startPos = new List<Vector2>();                                       //List of player starting locations for each level
        PlayerSprite player;                                                                //Define player as a PlayerSprite
        CircuitSprite circuit;                                                              //Define a single circuit per level
        List<Vector2> circuitPos = new List<Vector2>();
        List<List<PlatformSprite>> levels = new List<List<PlatformSprite>>();               //Define 2D list for all platforms in all levels
        List<List<ShockSprite>> shocks = new List<List<ShockSprite>>();
        List<StarSprite> stars = new List<StarSprite>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = screenSize.X;                              //Set width of the screen
            _graphics.PreferredBackBufferHeight = screenSize.Y;                             //Set height of the screen
            _graphics.ApplyChanges();                                                       //Apply graphics changes

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            circuitSheet = Content.Load<Texture2D>("circuitSheet");                         //Load sprite sheet for circuits
            playerSheet = Content.Load<Texture2D>("PlayerSheet");
            platformSheet = Content.Load<Texture2D>("platformSheet");                       //Load sprite sheet 2 (platforms)
            font = Content.Load<SpriteFont>("Font1");                                       //Load font file for UI
            bigFont = Content.Load<SpriteFont>("BigFont");                                  //Load big font file for game over
            jumpSound = Content.Load<SoundEffect>("JumpSound");                             //Load jump sound
            deathSound = Content.Load<SoundEffect>("DeathSound");                           //Load death sound
            circuitSound = Content.Load<SoundEffect>("CircuitSound");                       //Load circuit collction sound
            gravUp = Content.Load<SoundEffect>("GravUp");                                   //Gravity switching on sound
            gravDown = Content.Load<SoundEffect>("GravDown");                               //Gravity switching off sound
            music = Content.Load<Song>("music");

            whiteBox = new Texture2D(GraphicsDevice, 1, 1);                                 //Create empty sprite for drawing collision (1 by 1 pixel only)
            whiteBox.SetData(new[] { Color.White });                                        //Fill collision sprite with a white pixel

            BuildLevels();                                                                  //Run BuildLevels (places all platforms for current level)

            player = new PlayerSprite(playerSheet, whiteBox, startPos[currentLevel]);       //Create player
            circuit = new CircuitSprite(circuitSheet, whiteBox, circuitPos[currentLevel]);  //Create circuit sprite
            for(int i = 0; i < 50; i++)                                                     //Add first stars for background
            {
                stars.Add(new StarSprite(whiteBox, whiteBox, new Vector2(1, 1), screenSize));
                starNum = i;
            }

            MediaPlayer.Volume = 0.6f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(starNum < 100)
            {
                stars.Add(new StarSprite(whiteBox, whiteBox, new Vector2(), screenSize));
                starNum++;
            }

            gravityOn = changeGravity(gameTime);

            foreach(StarSprite star in stars)
            {
                star.Update(gameTime);

                if (star.spritePos.X < 0)
                {
                    starNum--;
                }
            }

            stars.RemoveAll(star => star.spritePos.X < 0);

            if (!gameOver)
            {
                player.Update(gameTime, levels[currentLevel], jumpSound, gravityOn);                   //Update player

                if (player.spritePos.Y - 150 > screenSize.Y)
                {
                    if (lives > 0)
                    {
                        player.resetPlayer(startPos[currentLevel]);                         //Reset player to the starting position if they fall below the screen
                        lives--;
                        circuit.randomSprite();
                        gravityOn = true;
                        gravTimer = maxTimer;
                        deathSound.Play();
                    }
                    if (lives == 0)
                    {
                        gameOver = true;
                    }
                }

                if (player.checkCollision(circuit))
                {
                    if (!firstRoundDone)
                    {
                        currentLevel++;
                        if (currentLevel >= levels.Count)
                        {
                            currentLevel = 0;
                            if (!firstRoundDone)
                                firstRoundDone = true;
                        }
                    }
                    else
                    {
                        prevLevel = currentLevel;
                        while(prevLevel == currentLevel)
                        {
                            currentLevel = rnd.Next(levels.Count);
                        }
                    }
                    circuit.spritePos = circuitPos[currentLevel];
                    player.resetPlayer(startPos[currentLevel]);
                    circuit.randomSprite();
                    gravityOn = true;
                    gravTimer = maxTimer;
                    circuitSound.Play();
                    if (circuitCount == 9)
                    {
                        circuitCount = 0;
                        lives++;
                    }
                    else
                        circuitCount++;
                }
                foreach (ShockSprite shock in shocks[currentLevel])
                {
                    if (player.checkCollision(shock))
                    {
                        if (lives > 0)
                        {
                            player.resetPlayer(startPos[currentLevel]);                         //Reset player to the starting position if they fall below the screen
                            lives--;
                            circuit.randomSprite();
                            gravityOn = true;
                            gravTimer = maxTimer;
                            deathSound.Play();
                        }
                        if (lives == 0)
                        {
                            gameOver = true;
                        }
                    }
                }
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (gameOverType == 0)
                        gameOverType = 1;
                    currentLevel = 0;
                    player.resetPlayer(startPos[currentLevel]);
                    circuit.spritePos = circuitPos[currentLevel];
                    circuitCount = 0;
                    lives = startLives;
                    gameOver = false;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

            foreach (StarSprite star in stars)
                star.Draw(_spriteBatch, gameTime);

            if (!gameOver)
            {

                if (gravityOn) gravString = "on";
                else gravString = "off";
                if (gravityOn) gravColor = Color.Green;
                else gravColor = Color.Red;
                gravPercent = gravTimer / maxTimer;
                gravWidth = 196 * gravPercent;

                _spriteBatch.Draw(whiteBox, new Rectangle(screenSize.X / 2 - 100, 5, 200, 20), new Rectangle(0, 0, 1, 1), Color.White);
                _spriteBatch.Draw(whiteBox, new Rectangle(screenSize.X / 2 - 98, 7, (int)gravWidth, 16), new Rectangle(0, 0, 1, 1), gravColor);
                Vector2 gravSize = font.MeasureString("gravity" + gravString);
                _spriteBatch.DrawString(font, "Gravity " + gravString, new Vector2(screenSize.X / 2 - gravSize.X / 2 - 9, 11), Color.Gray);
                _spriteBatch.DrawString(font, "Gravity " + gravString, new Vector2(screenSize.X / 2 - gravSize.X / 2 - 6, 14), Color.White);

                _spriteBatch.DrawString(font, "Lives: ", new Vector2(8, -10), Color.Gray);
                _spriteBatch.DrawString(font, "Lives: ", new Vector2(5, -13), Color.White);
                int heartPos = 90;
                for (int i = 0; i < lives; i++)
                {
                    _spriteBatch.Draw(playerSheet, new Rectangle(heartPos, 8, 28, 24), new Rectangle(48, 48, 14, 12), Color.White);
                    heartPos += 35;
                }

                Vector2 circuitTextSize = font.MeasureString("circuits: " + circuitCount.ToString());
                _spriteBatch.DrawString(font, "circuits: " + circuitCount.ToString(), new Vector2(screenSize.X - circuitTextSize.X - 8, -13), Color.Gray);
                _spriteBatch.DrawString(font, "circuits: " + circuitCount.ToString(), new Vector2(screenSize.X - circuitTextSize.X - 5, -10), Color.White);

                if (!firstRoundDone)
                {
                    if (currentLevel == 0)
                    {
                        _spriteBatch.DrawString(font, "<--- The gravity circuit's broken!\n     Grab the circuit boards to fix it!", new Vector2(637, 227), Color.Gray);
                        _spriteBatch.DrawString(font, "<--- The gravity circuit's broken!\n     Grab the circuit boards to fix it!", new Vector2(640, 230), Color.White);
                    }
                    else if (currentLevel == 1)
                    {
                        Vector2 textSize = font.MeasureString("collect 10 circuits for another life!");
                        _spriteBatch.DrawString(font, "collect 10 circuits for another life!", new Vector2(screenSize.X / 2 - (textSize.X / 2) - 3, 300), Color.Gray);
                        _spriteBatch.DrawString(font, "collect 10 circuits for another life!", new Vector2(screenSize.X / 2 - (textSize.X / 2), 300), Color.White);
                    }
                }

                player.Draw(_spriteBatch, gameTime);                                        //Draw player

                foreach (PlatformSprite platform in levels[currentLevel])                   //Draw all platforms
                    platform.Draw(_spriteBatch, gameTime);
                foreach (ShockSprite shock in shocks[currentLevel])
                    shock.Draw(_spriteBatch, gameTime);

                circuit.Draw(_spriteBatch, gameTime);
            }
            else
            {
                if(gameOverType == 0)
                {
                    Vector2 textSize = bigFont.MeasureString("SWITCHGRAV");
                    _spriteBatch.DrawString(bigFont, "SWITCHGRAV", new Vector2(screenSize.X / 2 - (textSize.X / 2) - 3, screenSize.Y / 2 - (textSize.Y / 2) - 3), Color.White);
                    _spriteBatch.DrawString(bigFont, "SWITCHGRAV", new Vector2(screenSize.X / 2 - (textSize.X / 2), screenSize.Y / 2 - (textSize.Y / 2)), Color.DarkRed);
                    textSize = font.MeasureString("Press Enter to start");
                    _spriteBatch.DrawString(font, "Press Enter to start", new Vector2(screenSize.X / 2 - (textSize.X / 2) - 5, screenSize.Y / 2 - (textSize.Y / 2) + 45), Color.Gray);
                    _spriteBatch.DrawString(font, "Press Enter to start", new Vector2(screenSize.X / 2 - (textSize.X / 2), screenSize.Y / 2 - (textSize.Y / 2) + 48), Color.White);
                }
                if (gameOverType == 1)
                {
                    Vector2 textSize = bigFont.MeasureString("GAME OVER");
                    _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2(screenSize.X / 2 - (textSize.X / 2) - 3, screenSize.Y / 2 - (textSize.Y / 2) - 3), Color.White);
                    _spriteBatch.DrawString(bigFont, "GAME OVER", new Vector2(screenSize.X / 2 - (textSize.X / 2), screenSize.Y / 2 - (textSize.Y / 2)), Color.DarkRed);
                    textSize = font.MeasureString("Press Enter to restart.");
                    _spriteBatch.DrawString(font, "Press Enter to restart.", new Vector2(screenSize.X / 2 - (textSize.X / 2) - 5, screenSize.Y / 2 - (textSize.Y / 2) + 45), Color.Gray);
                    _spriteBatch.DrawString(font, "Press Enter to restart.", new Vector2(screenSize.X / 2 - (textSize.X / 2), screenSize.Y / 2 - (textSize.Y / 2) + 48), Color.White);
                }
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        void BuildLevels()
        {
            //Level 0
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());
            levels[0].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(600, 650), true));
            //shocks[0].Add(new ShockSprite(platformSheet, whiteBox, new Vector2(519, 683), false, true));
            //shocks[0].Add(new ShockSprite(platformSheet, whiteBox, new Vector2(647, 683), false, false));
            circuitPos.Add(new Vector2(600, 250));
            startPos.Add(new Vector2(600, 645));

            //Level 1
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());                                                                                         //Add first level
            levels[1].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2((screenSize.X / 3) + 15, screenSize.Y / 2), true));       //Add first platform
            levels[1].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(((screenSize.X / 3) * 2) - 15, screenSize.Y / 2), true)); //Add second platform
            circuitPos.Add(new Vector2(((screenSize.X / 3) * 2) - 15, screenSize.Y / 2 - 60));                                              //Add first level's circuit
            startPos.Add(new Vector2(screenSize.X / 3, screenSize.Y / 2 - 5));                                                              //Define starting position for the player

            //Level 2
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());
            levels[2].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(200, 600), true));
            levels[2].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(500, 500), true));
            levels[2].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(596, 500), true));
            levels[2].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(600, 200), false));
            levels[2].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(600, 296), false));
            circuitPos.Add(new Vector2(1000, 475));
            startPos.Add(new Vector2(200, 595));

            //Level 3
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());
            levels[3].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X / 3, 600), true));
            levels[3].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(((screenSize.X / 3) * 2), 450), true));
            levels[3].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X / 3, 300), true));
            levels[3].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X / 3 - 80, 300 - 16), false));
            levels[3].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X / 3 + 45, 300 - 16), false));
            circuitPos.Add(new Vector2((screenSize.X / 3), 275));
            startPos.Add(new Vector2(screenSize.X / 3, 595));

            //Level 4
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());
            levels[4].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2((screenSize.X / 2) - 240, 600), true));
            levels[4].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2((screenSize.X / 2) + 240, 600), true));
            int pos = (screenSize.X / 2) - 96;
            for(int i = 0; i < 3; i++)
            {
                levels[4].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(pos, 300), true));
                pos += 96;
            }
            pos = (screenSize.Y / 2) + 120;
            for (int i = 0; i < 2; i++)
            {
                levels[4].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2((screenSize.X / 2), pos), false));
                pos -= 96;
            }
            shocks[4].Add(new ShockSprite(platformSheet, whiteBox, new Vector2(screenSize.X / 2 - 1, pos + 49), true, false));
            circuitPos.Add(new Vector2((screenSize.X / 2) + 350, 550));
            startPos.Add(new Vector2((screenSize.X / 2) - 240, 595));

            //Level 5
            shocks.Add(new List<ShockSprite>());
            levels.Add(new List<PlatformSprite>());
            circuitPos.Add(new Vector2(screenSize.X - 300, screenSize.Y / 2));
            startPos.Add(new Vector2(300, (screenSize.Y / 2) + 91));
            levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(240, (screenSize.Y / 2) - 48), false));
            levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(240, (screenSize.Y / 2) + 48), false));
            levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X - 239, (screenSize.Y / 2) - 48), false));
            levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(screenSize.X - 239, (screenSize.Y / 2) + 48), false));
            pos = 289;
            for(int i = 0; i < 12; i++)
            {
                levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(pos, (screenSize.Y / 2) + 96), true));
                pos += 96;
            }
            pos = 289;
            for (int i = 0; i < 12; i++)
            {
                levels[5].Add(new PlatformSprite(platformSheet, whiteBox, new Vector2(pos, (screenSize.Y / 2) - 124), true));
                pos += 96;
            }
            pos = 350;
            for (int i = 0; i < 4; i++)
            {
                shocks[5].Add(new ShockSprite(platformSheet, whiteBox, new Vector2(pos, (screenSize.Y / 2) - 59), true, true));
                pos += 250;
            }
            pos = 450;
            for (int i = 0; i < 6; i++)
            {
                shocks[5].Add(new ShockSprite(platformSheet, whiteBox, new Vector2(pos, (screenSize.Y / 2) + 96), true, false));
                pos += 250;
            }

        }

        bool changeGravity(GameTime gameTime)
        {
            if (!gameOver)
            {
                if (gravityOn)
                {
                    if (gravTimer > 0)
                        gravTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    else
                    {
                        gravDown.Play();
                        return false;
                    }
                }
                else
                {
                    if (gravTimer < maxTimer)
                        gravTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else
                    {
                        gravUp.Play();
                        return true;
                    }
                }
            }
            return gravityOn;
        }
    }
}