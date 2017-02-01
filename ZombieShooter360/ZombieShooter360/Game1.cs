using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace ZombieShooter360
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //TEST Texture2D testBallTexture;
        Button buttonPlay;
        Button buttonResume;
        Button buttonMainMenu;
        Button buttonQuit;
        Button button2PlayerSplitscreen;
        int screenWidth = 800;
        int screenHeight = 600;
        Player[] players;
        Level currentLevel;
        Options options;
        public readonly string highscoresFilename = "highscores.xml";
        int logoTime = 150;
        int maxLogoTime = 150;
        Viewport leftView;
        Viewport leftOverlayView;
        Viewport rightView;
        Viewport rightOverlayView;
        Viewport overlayView;
        Viewport fullView;
        IAsyncResult storageResult;
        StorageDevice storageDevice;
        ScreenResizer screenResizer;

        enum GameState
        {
            LOGO, MAINMENU, SINGLEPLAYER, PAUSE, LOSE, TWOPLAYER, TWOPLAYERPAUSE, TWOPLAYERLOSE, CHOOSINGSTORAGEDEVICEBEGIN, CHOOSINGSTORAGEDEVICEEND, RESIZINGSCREEN
        }

        GameState CurrentGameState = GameState.CHOOSINGSTORAGEDEVICEBEGIN;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            players = new Player[2];

            setScreenSettingsToDefault();

            screenResizer = new ScreenResizer(fullView);

            base.Initialize();
        }

        public void setScreenSettingsToDefault()
        {
            //graphics.PreferredBackBufferHeight = screenHeight;
            //graphics.PreferredBackBufferWidth = screenWidth;
            //graphics.ApplyChanges();
            //graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            //graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            //graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            fullView = GraphicsDevice.Viewport;
            screenHeight = graphics.PreferredBackBufferHeight;
            screenWidth = graphics.PreferredBackBufferWidth;
            overlayView = GraphicsDevice.Viewport;
            UpdateLeftAndRightViews();
            UpdateScreenSettings(GraphicsDevice.Viewport);
            overlayView.Width = screenWidth;
            overlayView.Height = screenHeight;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            options = new Options();

            currentLevel = new Level(new Solid[4], new Vector2[4], Content, Color.LightGreen, new Vector2(1000, 1000), new Vector2(150f, 150f));
            currentLevel.solids[0] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(10, 7)));
            currentLevel.solids[1] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(300, 702)));
            currentLevel.solids[2] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(602, 300)));
            currentLevel.solids[3] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(503, 850)));
            //currentLevel.solids[4] = new Solid(new Sprite(Content.Load<Texture2D>("TestLevelWidth"), new Vector2(0, -(Content.Load<Texture2D>("TestLevelWidth").Height))));
            //currentLevel.solids[5] = new Solid(new Sprite(Content.Load<Texture2D>("TestLevelHeight"), new Vector2(-(Content.Load<Texture2D>("TestLevelHeight").Width), 0)));
            //currentLevel.solids[6] = new Solid(new Sprite(Content.Load<Texture2D>("TestLevelWidth"), new Vector2(0, currentLevel.levelHeight)));
            //currentLevel.solids[7] = new Solid(new Sprite(Content.Load<Texture2D>("TestLevelHeight"), new Vector2(currentLevel.levelWidth, 0)));
            currentLevel.zombieSpawners[0] = new Vector2(170, 20);
            currentLevel.zombieSpawners[1] = new Vector2(20, 170);
            currentLevel.zombieSpawners[2] = new Vector2(700, 200);
            currentLevel.zombieSpawners[3] = new Vector2(400, 700);

            buttonPlay = new Button(new Sprite(Content.Load<Texture2D>("PlayButton"), new Vector2()));
            buttonPlay.sprite.vector = new Vector2(((screenWidth / 2) - (buttonPlay.sprite.getTexture().Width / 2)), (screenHeight / 3 + (screenHeight / 6)));

            //buttonOptions = new Button(new Sprite(Content.Load<Texture2D>("OptionsButton"), new Vector2()));
            //buttonOptions.sprite.vector = new Vector2(((screenWidth / 2) - (buttonOptions.sprite.getTexture().Width / 2)), (screenHeight / 3 + (screenHeight / 3)));

            button2PlayerSplitscreen = new Button(new Sprite(Content.Load<Texture2D>("2PlayerSplitscreenButton"), new Vector2()));
            button2PlayerSplitscreen.sprite.vector = new Vector2(((screenWidth / 2) - (button2PlayerSplitscreen.sprite.getTexture().Width / 2)), (screenHeight / 3 + (screenHeight / 3)));

            buttonResume = new Button(new Sprite(Content.Load<Texture2D>("ResumeButton"), new Vector2()));
            buttonResume.sprite.vector = new Vector2(((screenWidth / 2) - (buttonResume.sprite.getTexture().Width / 2)), (screenHeight / 3 + (screenHeight / 6)));

            buttonMainMenu = new Button(new Sprite(Content.Load<Texture2D>("MainMenuButton"), new Vector2()));
            buttonMainMenu.sprite.vector = new Vector2(((screenWidth / 2) - (buttonMainMenu.sprite.getTexture().Width / 2)), (screenHeight / 3 + (screenHeight / 3)));

            buttonQuit = new Button(new Sprite(Content.Load<Texture2D>("QuitButton"), new Vector2()));
            buttonQuit.sprite.vector = new Vector2(buttonQuit.sprite.getTexture().Width / 2, screenHeight - buttonQuit.sprite.getTexture().Height - buttonQuit.sprite.getTexture().Height / 2);

            players[0] = new Player(new Sprite(Content.Load<Texture2D>("Player"), currentLevel.playerSpawn, Color.Blue), 0f);
            Vector2 player2Spawn = currentLevel.playerSpawn;
            player2Spawn.X += Content.Load<Texture2D>("Player").Width;
            players[1] = new Player(new Sprite(Content.Load<Texture2D>("Player"), player2Spawn, Color.Red), 0f);
        }

        public void UpdateScreenSettings(Viewport viewport)
        {
            screenWidth = viewport.Width;
            screenHeight = viewport.Height;
        }

        public void UpdateLeftAndRightViews()
        {
            leftView = overlayView;
            leftOverlayView = overlayView;
            rightView = overlayView;
            rightOverlayView = overlayView;
            leftView.Width /= 2;
            leftOverlayView.Width /= 2;
            rightView.Width /= 2;
            rightOverlayView.Width /= 2;
            rightView.X = leftView.Width;
            rightOverlayView.X = leftOverlayView.Width;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool justPressedFullscreenButton = false;
        bool justPressedPauseButton = false;
        bool justClickedOnscreenButton = false;
        bool justPressedGamePadKey = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            
            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                if (!justPressedFullscreenButton)
                {
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                    graphics.ApplyChanges();
                    justPressedFullscreenButton = true;
                }
            }
            else
            {
                justPressedFullscreenButton = false;
            }
            if (CurrentGameState == GameState.CHOOSINGSTORAGEDEVICEBEGIN)
            {
                Components.Add(new GamerServicesComponent(this));
                storageResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                CurrentGameState = GameState.CHOOSINGSTORAGEDEVICEEND;
            }
            else if (CurrentGameState == GameState.CHOOSINGSTORAGEDEVICEEND)
            {
                if (storageResult.IsCompleted)
                {
                    storageDevice = StorageDevice.EndShowSelector(storageResult);
                    HighScores.LoadHighScores(highscoresFilename, storageDevice);
                    CurrentGameState = GameState.RESIZINGSCREEN;
                }
            }
            else if (CurrentGameState == GameState.RESIZINGSCREEN)
            {
                if(screenResizer.firstUpdate)
                {
                    if(GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                    {
                        justPressedGamePadKey = true;
                    }
                }
                screenResizer.Update();
                if (!justPressedGamePadKey && GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    overlayView = new Viewport(screenResizer.newViewport.Bounds);
                    UpdateLeftAndRightViews();
                    UpdateScreenSettings(overlayView);
                    CurrentGameState = GameState.LOGO;
                }
            }
            else if (CurrentGameState == GameState.LOGO)
            {
                logoTime -= 1;
                if (logoTime <= 0)
                {
                    CurrentGameState = GameState.MAINMENU;
                }
            }
            else if (CurrentGameState == GameState.TWOPLAYERLOSE)
            {
                IsMouseVisible = true;
                if ((buttonMainMenu.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonMainMenu.clicked = false;
                    justClickedOnscreenButton = true;
                    LoadContent();
                    CurrentGameState = GameState.MAINMENU;
                    justPressedPauseButton = false;
                    justClickedOnscreenButton = true;
                }
                else
                {
                    justPressedPauseButton = false;
                    buttonMainMenu.Update(Mouse.GetState());
                }
                players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                players[1].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
            }
            else if (CurrentGameState == GameState.LOSE)
            {
                IsMouseVisible = true;
                if ((buttonMainMenu.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonMainMenu.clicked = false;
                    justClickedOnscreenButton = true;
                    LoadContent();
                    CurrentGameState = GameState.MAINMENU;
                    justPressedPauseButton = false;
                    justClickedOnscreenButton = true;
                }
                else
                {
                    justPressedPauseButton = false;
                    buttonMainMenu.Update(Mouse.GetState());
                }
                players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
            }
            else if (CurrentGameState == GameState.MAINMENU)
            {
                IsMouseVisible = true;
                if ((buttonPlay.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonPlay.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.SINGLEPLAYER;
                }
                /*
                else if ((buttonOptions.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonOptions.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.OPTIONS;
                }
                */
                else if ((button2PlayerSplitscreen.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    button2PlayerSplitscreen.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.TWOPLAYER;
                }
                else if ((buttonQuit.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    justClickedOnscreenButton = true;
                    this.Exit();
                }
                else
                {
                    buttonPlay.Update(Mouse.GetState());
                    button2PlayerSplitscreen.Update(Mouse.GetState());
                    buttonQuit.Update(Mouse.GetState());
                }
            }
            else if (CurrentGameState == GameState.TWOPLAYERPAUSE)
            {
                IsMouseVisible = true;

                if ((Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && justPressedPauseButton == false)
                {
                    justPressedPauseButton = true;
                    CurrentGameState = GameState.TWOPLAYER;
                }
                else if (justPressedPauseButton == true && (Keyboard.GetState().IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released))
                {
                    justPressedPauseButton = false;
                }

                if ((buttonResume.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonResume.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.TWOPLAYER;
                }
                else if ((buttonMainMenu.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonMainMenu.clicked = false;
                    justClickedOnscreenButton = true;
                    LoadContent();
                    CurrentGameState = GameState.MAINMENU;
                    justPressedPauseButton = false;
                    justClickedOnscreenButton = true;
                }
                else if ((buttonQuit.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    justClickedOnscreenButton = true;
                    this.Exit();
                }
                else
                {
                    buttonResume.Update(Mouse.GetState());
                    buttonMainMenu.Update(Mouse.GetState());
                    buttonQuit.Update(Mouse.GetState());
                }
            }
            else if (CurrentGameState == GameState.PAUSE)
            {
                IsMouseVisible = true;

                if ((Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && justPressedPauseButton == false)
                {
                    justPressedPauseButton = true;
                    CurrentGameState = GameState.SINGLEPLAYER;
                }
                else if (justPressedPauseButton == true && (Keyboard.GetState().IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released))
                {
                    justPressedPauseButton = false;
                }

                if ((buttonResume.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonResume.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.SINGLEPLAYER;
                }
                else if ((buttonMainMenu.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.B == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonMainMenu.clicked = false;
                    justClickedOnscreenButton = true;
                    LoadContent();
                    CurrentGameState = GameState.MAINMENU;
                    justPressedPauseButton = false;
                    justClickedOnscreenButton = true;
                }
                else if ((buttonQuit.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    justClickedOnscreenButton = true;
                    this.Exit();
                }
                else
                {
                    buttonResume.Update(Mouse.GetState());
                    buttonMainMenu.Update(Mouse.GetState());
                    buttonQuit.Update(Mouse.GetState());
                }
            }
            else if (CurrentGameState == GameState.TWOPLAYER)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && justPressedPauseButton == false)
                {
                    justPressedPauseButton = true;
                    CurrentGameState = GameState.TWOPLAYERPAUSE;
                }
                else if ((justPressedPauseButton == true && (Keyboard.GetState().IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released)))
                {
                    justPressedPauseButton = false;
                }
                if (players[0].delayUntilHit > 0)
                {
                    players[0].delayUntilHit -= 1;
                }
                if (players[1].delayUntilHit > 0)
                {
                    players[1].delayUntilHit -= 1;
                }
                if (options.usingController)
                {
                    IsMouseVisible = false;
                    if (players[0].delay > 0)
                    {
                        players[0].delay -= 1;
                    }
                    if (players[1].delay > 0)
                    {
                        players[1].delay -= 1;
                    }
                    if (!players[0].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0)
                        {
                            players[0].aiming = true;
                        }
                        else
                        {
                            players[0].aiming = false;
                        }
                    }
                    if (!players[1].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.Two).Triggers.Left > 0)
                        {
                            players[1].aiming = true;
                        }
                        else
                        {
                            players[1].aiming = false;
                        }
                    }
                    if (!players[0].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                        {
                            if (players[0].weapon == Player.Weapon.BASIC && players[0].delay <= 0)
                            {
                                if (currentLevel.basicWeaponBullets.Length != 0)
                                {
                                    BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                    int i;
                                    for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                    {
                                        newBullets[i] = currentLevel.basicWeaponBullets[i];
                                    }
                                    newBullets[i] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                    currentLevel.basicWeaponBullets = newBullets;
                                    players[0].delay = 20;
                                }
                                else
                                {
                                    currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                    currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                    players[0].delay = 20;
                                }
                            }
                        }
                    }
                    if (!players[1].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.Two).Triggers.Right > 0)
                        {
                            if (players[1].weapon == Player.Weapon.BASIC && players[1].delay <= 0)
                            {
                                if (currentLevel.basicWeaponBullets.Length != 0)
                                {
                                    BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                    int i;
                                    for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                    {
                                        newBullets[i] = currentLevel.basicWeaponBullets[i];
                                    }
                                    newBullets[i] = new BasicWeaponBullet(Content, players[1].sprite.vector, players[1].playerRotation);
                                    currentLevel.basicWeaponBullets = newBullets;
                                    players[1].delay = 20;
                                }
                                else
                                {
                                    currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                    currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, players[1].sprite.vector, players[1].playerRotation);
                                    players[1].delay = 20;
                                }
                            }
                        }
                    }
                    if (!players[0].isDead)
                    {
                        if (options.player1CameraRotation)
                        {
                            if (players[0].aiming)
                            {
                                float addedX = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X;
                                float multipliedX = (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f));
                                float addedY = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y;
                                float multipliedY = (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f));
                                float newX = players[0].sprite.getX();
                                float newY = players[0].sprite.getY();
                                float rotation = MathHelper.ToDegrees((float)Math.Atan2(addedY, addedX));
                                rotation += players[0].playerRotation;
                                //rotation += 300;
                                while (rotation > 359)
                                {
                                    rotation -= 360;
                                }
                                while (rotation < 0)
                                {
                                    rotation += 360;
                                }
                                players[0].SetX(newX + (float)Math.Cos(MathHelper.ToRadians(rotation)), currentLevel);
                                players[0].SetY(newY - (float)Math.Sin(MathHelper.ToRadians(rotation)), currentLevel);
                                //players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                                //players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                            }
                            else
                            {
                                float addedX = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X;
                                float multipliedX = 7;
                                float addedY = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y;
                                float multipliedY = 7;
                                float newX = players[0].sprite.getX();
                                float newY = players[0].sprite.getY();
                                float rotation = MathHelper.ToDegrees((float)Math.Atan2(addedY, addedX));
                                rotation += players[0].playerRotation;
                                //rotation += 300;
                                while (rotation > 359)
                                {
                                    rotation -= 360;
                                }
                                while (rotation < 0)
                                {
                                    rotation += 360;
                                }
                                players[0].SetX(newX + (float)Math.Cos(MathHelper.ToRadians(rotation)) * multipliedX, currentLevel);
                                players[0].SetY(newY - (float)Math.Sin(MathHelper.ToRadians(rotation)) * multipliedY, currentLevel);
                                //players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                                //players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                            }
                        }
                        else
                        {
                            if (players[0].aiming)
                            {
                                players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                                players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                            }
                            else
                            {
                                players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                                players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                            }
                        }
                    }
                    if (!players[1].isDead)
                    {
                        if (players[1].aiming)
                        {
                            players[1].SetX(players[1].sprite.getX() + GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f)), currentLevel);
                            players[1].SetY(players[1].sprite.getY() - GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f)), currentLevel);
                        }
                        else
                        {
                            players[1].SetX(players[1].sprite.getX() + GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                            players[1].SetY(players[1].sprite.getY() - GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                        }
                    }
                    if (!players[0].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                        {
                            if (options.player1CameraRotation)
                            {
                                players[0].playerRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                            }
                            else
                            {
                                players[0].playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                            }
                        }
                        if (options.player1CameraRotation)
                        {
                            players[0].camera.UpdateWithRotation(players[0].playerRotation);
                        }
                        else
                        {
                            players[0].camera.Update2Player(currentLevel, new Vector2(screenWidth, screenHeight));
                        }
                        if (!options.player1CameraRotation)
                        {
                            Vector2 newVector = players[0].camera.getPosition();
                            if (players[0].aiming)
                            {
                                newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                                newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                            }
                            else
                            {
                                newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                                newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                            }
                            players[0].camera.setPosition(newVector, currentLevel, new Vector2(screenWidth / 2, screenHeight));
                        }
                    }
                    if (!players[1].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.Two).ThumbSticks.Right != Vector2.Zero)
                        {
                            if (options.player2CameraRotation)
                            {
                                players[1].playerRotation += GamePad.GetState(PlayerIndex.Two).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                            }
                            else
                            {
                                players[1].playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                            }
                        }
                        if (options.player2CameraRotation)
                        {
                            players[1].camera.UpdateWithRotation(players[1].playerRotation);
                        }
                        else
                        {
                            players[1].camera.Update2Player(currentLevel, new Vector2(screenWidth, screenHeight));
                        }
                        if (!options.player2CameraRotation)
                        {
                            Vector2 newVector2 = players[1].camera.getPosition();
                            if (players[1].aiming)
                            {
                                newVector2.X += GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.X * (150 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 2.1f));
                                newVector2.Y -= GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.Y * (150 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 2.1f));
                            }
                            else
                            {
                                newVector2.X += GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                                newVector2.Y -= GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                            }
                            players[1].camera.setPosition(newVector2, currentLevel, new Vector2(screenWidth / 2, screenHeight));
                        }
                    }
                }
                currentLevel.Update2Player(players, Content);
                if (players[0].health <= 0)
                {
                    players[0].isDead = true;
                }
                if (players[1].health <= 0)
                {
                    players[1].isDead = true;
                }
                if (players[0].isDead && players[1].isDead)
                {
                    CurrentGameState = GameState.TWOPLAYERLOSE;
                    SaveHighScore();
                }
            }
            else if (CurrentGameState == GameState.SINGLEPLAYER)
            {
                if ((Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && justPressedPauseButton == false)
                {
                    justPressedPauseButton = true;
                    CurrentGameState = GameState.PAUSE;
                }
                else if ((justPressedPauseButton == true && (Keyboard.GetState().IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released)))
                {
                    justPressedPauseButton = false;
                }
                if (players[0].delayUntilHit > 0)
                {
                    players[0].delayUntilHit -= 1;
                }
                if (options.usingController)
                {
                    IsMouseVisible = false;
                    if (players[0].delay > 0)
                    {
                        players[0].delay -= 1;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0)
                    {
                        players[0].aiming = true;
                    }
                    else
                    {
                        players[0].aiming = false;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                    {
                        if (players[0].weapon == Player.Weapon.BASIC && players[0].delay <= 0)
                        {
                            if (currentLevel.basicWeaponBullets.Length != 0)
                            {
                                /*
                                List<BasicWeaponBullet> newBullets = new List<BasicWeaponBullet>();
                                foreach (BasicWeaponBullet bullet in currentLevel.basicWeaponBullets)
                                {
                                    newBullets.Add(bullet);
                                }
                                newBullets.Add(new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation));
                                currentLevel.basicWeaponBullets = newBullets.ToArray();
                                */
                                BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBullets[i] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                currentLevel.basicWeaponBullets = newBullets;
                                /*
                                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i = 0;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBasicWeaponBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBasicWeaponBullets[newBasicWeaponBullets.Length - 1] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                */
                                players[0].delay = 20;
                            }
                            else
                            {
                                currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                players[0].delay = 20;
                            }
                        }
                    }
                    if (players[0].aiming)
                    {
                        players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                        players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                    }
                    else
                    {
                        players[0].SetX(players[0].sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                        players[0].SetY(players[0].sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                    }
                    if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                    {
                        players[0].playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                    }
                    players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    Vector2 newVector = players[0].camera.getPosition();
                    if (players[0].aiming)
                    {
                        newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                        newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                    }
                    else
                    {
                        newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                        newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                    }
                    players[0].camera.setPosition(newVector, currentLevel, new Vector2(screenWidth, screenHeight));
                }
                else
                {
                    IsMouseVisible = false;
                    if (players[0].delay > 0)
                    {
                        players[0].delay -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        players[0].aiming = true;
                    }
                    else
                    {
                        players[0].aiming = false;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (players[0].weapon == Player.Weapon.BASIC && players[0].delay <= 0)
                        {
                            if (currentLevel.basicWeaponBullets.Length != 0)
                            {
                                BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBullets[i] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                currentLevel.basicWeaponBullets = newBullets;
                                /*
                                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i = 0;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBasicWeaponBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBasicWeaponBullets[newBasicWeaponBullets.Length - 1] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                */
                                players[0].delay = 20;
                            }
                            else
                            {
                                currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                players[0].delay = 20;
                            }
                        }
                    }
                    //float movementX = 0;
                    //float movementY = 0;

                    /*
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        movementY -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        movementX -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        movementY += 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        movementX += 1;
                    }
                    */

                    if (players[0].aiming)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            //movementY -= 1;
                            players[0].SetY(players[0].sprite.getY() + -1 * (5 * (1 / 5f)), currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            //movementX -= 1;
                            players[0].SetX(players[0].sprite.getX() + -1 * (5 * (1 / 5f)), currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            //movementY += 1;
                            players[0].SetY(players[0].sprite.getY() + 1 * (5 * (1 / 5f)), currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            //movementX += 1;
                            players[0].SetX(players[0].sprite.getX() + 1 * (5 * (1 / 5f)), currentLevel);
                        }
                    }
                    else
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            //movementY -= 1;
                            players[0].SetY(players[0].sprite.getY() + -1 * 7, currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            //movementX -= 1;
                            players[0].SetX(players[0].sprite.getX() + -1 * 7, currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            //movementY += 1;
                            players[0].SetY(players[0].sprite.getY() + 1 * 7, currentLevel);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            //movementX += 1;
                            players[0].SetX(players[0].sprite.getX() + 1 * 7, currentLevel);
                        }
                    }
                    //float movementRotation = (float)Math.Atan2(movementX, -movementY);
                    /*
                    if (players[0].aiming)
                    {
                        players[0].SetX(players[0].sprite.getX() - (float)((5 * (1 / 5f)) * Math.Cos(movementRotation)), currentLevel);
                        players[0].SetY(players[0].sprite.getY() - (float)((5 * (1 / 5f)) * Math.Sin(movementRotation)), currentLevel);
                    }
                    else
                    {
                        players[0].SetX(players[0].sprite.getX() - (float)(5 * Math.Cos(movementRotation)), currentLevel);
                        players[0].SetX(players[0].sprite.getX() - (float)(5 * Math.Sin(movementRotation)), currentLevel);
                    }
                     */
                    Vector2 distance = new Vector2((Mouse.GetState().X + (players[0].camera.getPosition().X - screenWidth / 2)) - players[0].sprite.getX(), (Mouse.GetState().Y + (players[0].camera.getPosition().Y - screenHeight / 2)) - players[0].sprite.getY());
                    players[0].playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    Vector2 newVector = players[0].camera.getPosition();
                    if (players[0].aiming)
                    {
                        newVector.X += (Mouse.GetState().X + (players[0].camera.getPosition().X - screenWidth / 2)) - players[0].sprite.getX();
                        newVector.Y += (Mouse.GetState().Y + (players[0].camera.getPosition().Y - screenWidth / 2)) - players[0].sprite.getY();
                    }
                    else
                    {
                        //newVector.X += ((Mouse.GetState().X + (players[0].camera.getPosition().X - screenWidth / 2)) - players[0].sprite.getX()) / 7;
                        //newVector.Y += ((Mouse.GetState().Y + (players[0].camera.getPosition().Y - screenWidth / 2)) - players[0].sprite.getY()) / 7;
                        //newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                        //newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                    }
                    players[0].camera.setPosition(newVector, currentLevel, new Vector2(screenWidth, screenHeight));
                }
                currentLevel.Update(players[0], Content);
                if (players[0].health <= 0)
                {
                    CurrentGameState = GameState.LOSE;
                    SaveHighScore();
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                justClickedOnscreenButton = false;
            }

            /*
            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
            {
                Vector2 newVector = players[0].camera.getPosition();
                newVector.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 7;
                newVector.Y -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 7;
                players[0].camera.setPosition(newVector);
            }
            if (GamePad.GetState(PlayerIndex.One).Triggers.Left != 0f)
            {
                players[0].camera.setZoom(players[0].camera.getZoom() - (GamePad.GetState(PlayerIndex.One).Triggers.Left / 3f));
            }
            if (GamePad.GetState(PlayerIndex.One).Triggers.Right != 0f)
            {
                players[0].camera.setZoom(players[0].camera.getZoom() + (GamePad.GetState(PlayerIndex.One).Triggers.Right / 3f));
            }
            */

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private float DegreeToRadian(float angle)
        {
            return ((float)Math.PI) * angle / 180f;
        }

        private void SaveHighScore()
        {
            HighScores data = HighScores.LoadHighScores(highscoresFilename, storageDevice);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (players[0].score > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.Name[i] = data.Name[i - 1];
                    data.Score[i] = data.Score[i - 1];
                }

                data.Name[scoreIndex] = "Player"; //TODO Get player name
                data.Score[scoreIndex] = players[0].score;

                HighScores.SaveHighScores(data, highscoresFilename, storageDevice);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.Viewport = fullView;
            spriteBatch.Begin();
            spriteBatch.Draw(Content.Load<Texture2D>("White"), fullView.Bounds, Color.Black);
            spriteBatch.End();

            if (CurrentGameState == GameState.SINGLEPLAYER || CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE)
            {
                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
            }
            else if (CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE)
            {

            }
            else if (CurrentGameState == GameState.RESIZINGSCREEN)
            {
                screenResizer.Draw(spriteBatch, GraphicsDevice, Content);
            }
            else
            {
                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
            }
            
            
            //spriteBatch.Draw(buttonPlay.sprite.getTexture(), buttonPlay.sprite.vector, buttonPlay.selectedTint);

            if (CurrentGameState == GameState.LOGO)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("White"), new Rectangle(0, 0, screenWidth, screenHeight), Color.Black);
                float logoAlpha;
                    logoAlpha = ((logoTime / 2) / (maxLogoTime / 2)) * 255;
                spriteBatch.Draw(Content.Load<Texture2D>("jacketsjpresents"), new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, logoAlpha));
            }
            else if (CurrentGameState == GameState.LOSE)
            {
                currentLevel.DrawWithoutHealth(spriteBatch, Content);
                spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Vector2(players[0].camera.getPosition().X - screenWidth / 2, players[0].camera.getPosition().Y - screenHeight / 2), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (players[0].camera.getPosition().X - screenWidth / 2), buttonMainMenu.sprite.vector.Y + (players[0].camera.getPosition().Y - screenHeight / 2)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (players[0].camera.getPosition().X - screenWidth / 2), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (players[0].camera.getPosition().Y - screenHeight / 2)), Color.White);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename, storageDevice).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(players[0].camera.getPosition().X, players[0].camera.getPosition().Y), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
            }
            else if (CurrentGameState == GameState.MAINMENU)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("ConstantHeadshotsZTitlePage"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                buttonPlay.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("AButton"), new Vector2(buttonPlay.sprite.vector.X + buttonPlay.sprite.getTexture().Width + Content.Load<Texture2D>("AButton").Width / 2, buttonPlay.sprite.vector.Y + Content.Load<Texture2D>("AButton").Height / 2), Color.White);
                button2PlayerSplitscreen.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("YButton"), new Vector2(button2PlayerSplitscreen.sprite.vector.X + button2PlayerSplitscreen.sprite.getTexture().Width + Content.Load<Texture2D>("YButton").Width / 2, button2PlayerSplitscreen.sprite.vector.Y + Content.Load<Texture2D>("YButton").Height / 2), Color.White);
                buttonQuit.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("BackButton"), new Vector2(buttonQuit.sprite.vector.X + buttonQuit.sprite.getTexture().Width + Content.Load<Texture2D>("BackButton").Width / 2, buttonQuit.sprite.vector.Y + Content.Load<Texture2D>("BackButton").Height / 2), Color.White);
                HighScores highScores = HighScores.LoadHighScores(highscoresFilename, storageDevice);
                if (highScores.Score != null)
                {
                    if (highScores.Score.Length != 0)
                    {
                        string scores = "";
                        for (int i = 0; i < highScores.Score.Length; i++)
                        {
                            if (i != 0)
                            {
                                scores += "\n";
                            }
                            scores += highScores.Name[i] + ": " + highScores.Score[i];
                        }
                        SpriteFont Font = Content.Load<SpriteFont>("basic");
                        Vector2 scoresOrigin = Font.MeasureString(scores);
                        spriteBatch.DrawString(Font, scores, new Vector2(screenWidth - screenWidth / 18, screenHeight - screenHeight / 18), Color.Black, 0, scoresOrigin, 1.0f, SpriteEffects.None, 0.5f);
                    }
                }
            }
            else if (CurrentGameState == GameState.PAUSE)
            {
                currentLevel.Draw(spriteBatch, Content);
                players[0].Draw(spriteBatch, Content);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(players[0].camera.getPosition().X - screenWidth / 2 + Font.MeasureString(health).X, players[0].camera.getPosition().Y + screenHeight / 2 - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(players[0].camera.getPosition().X + screenWidth / 2 - screenWidth / 300, players[0].camera.getPosition().Y - screenHeight / 2 - screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(Content.Load<Texture2D>("PauseBackground"), new Rectangle((int)(players[0].camera.getPosition().X - screenWidth / 2), (int)(players[0].camera.getPosition().Y - screenHeight / 2), screenWidth, screenHeight), Color.White);
                spriteBatch.Draw(buttonResume.sprite.getTexture(), new Vector2(buttonResume.sprite.vector.X + (players[0].camera.getPosition().X - screenWidth / 2), buttonResume.sprite.vector.Y + (players[0].camera.getPosition().Y - screenHeight / 2)), buttonResume.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("AButton"), new Vector2((buttonResume.sprite.vector.X + buttonResume.sprite.getTexture().Width + Content.Load<Texture2D>("AButton").Width / 2) + (players[0].camera.getPosition().X - screenWidth / 2), (buttonResume.sprite.vector.Y + Content.Load<Texture2D>("AButton").Height / 2) + (players[0].camera.getPosition().Y - screenHeight / 2)), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (players[0].camera.getPosition().X - screenWidth / 2), buttonMainMenu.sprite.vector.Y + (players[0].camera.getPosition().Y - screenHeight / 2)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (players[0].camera.getPosition().X - screenWidth / 2), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (players[0].camera.getPosition().Y - screenHeight / 2)), Color.White);
                spriteBatch.Draw(buttonQuit.sprite.getTexture(), new Vector2(buttonQuit.sprite.vector.X + (players[0].camera.getPosition().X - screenWidth / 2), buttonQuit.sprite.vector.Y + (players[0].camera.getPosition().Y - screenHeight / 2)), buttonQuit.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BackButton"), new Vector2((buttonQuit.sprite.vector.X + buttonQuit.sprite.getTexture().Width + Content.Load<Texture2D>("BackButton").Width / 2) + (players[0].camera.getPosition().X - screenWidth / 2), (buttonQuit.sprite.vector.Y + Content.Load<Texture2D>("BackButton").Height / 2) + (players[0].camera.getPosition().Y - screenHeight / 2)), Color.White);
            }
            else if (CurrentGameState == GameState.TWOPLAYERLOSE)
            {
                GraphicsDevice.Viewport = leftView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                spriteBatch.End();


                GraphicsDevice.Viewport = rightView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                spriteBatch.End();


                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Vector2(0, 0), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y + (0)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename, storageDevice).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(screenWidth / 2, screenHeight / 2), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.TWOPLAYERPAUSE)
            {
                GraphicsDevice.Viewport = leftView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                if (!players[0].isDead)
                {
                    players[0].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                }
                if (!players[1].isDead)
                {
                    players[1].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                }
                spriteBatch.End();


                GraphicsDevice.Viewport = rightView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                if (!players[1].isDead)
                {
                    players[1].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                }
                if (!players[0].isDead)
                {
                    players[0].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                }
                spriteBatch.End();


                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("PauseBackground"), new Rectangle((int)(0), (int)(0), screenWidth, screenHeight), Color.White);
                spriteBatch.Draw(buttonResume.sprite.getTexture(), new Vector2(buttonResume.sprite.vector.X + (0), buttonResume.sprite.vector.Y + (0)), buttonResume.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("AButton"), new Vector2((buttonResume.sprite.vector.X + buttonResume.sprite.getTexture().Width + Content.Load<Texture2D>("AButton").Width / 2) + (0), (buttonResume.sprite.vector.Y + Content.Load<Texture2D>("AButton").Height / 2) + (0)), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y + (0)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                spriteBatch.Draw(buttonQuit.sprite.getTexture(), new Vector2(buttonQuit.sprite.vector.X + (0), buttonQuit.sprite.vector.Y + (0)), buttonQuit.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BackButton"), new Vector2((buttonQuit.sprite.vector.X + buttonQuit.sprite.getTexture().Width + Content.Load<Texture2D>("BackButton").Width / 2) + (0), (buttonQuit.sprite.vector.Y + Content.Load<Texture2D>("BackButton").Height / 2) + (0)), Color.White);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.TWOPLAYER)
            {
                GraphicsDevice.Viewport = leftView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                if (!players[0].isDead)
                {
                    players[0].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                }
                if (!players[1].isDead)
                {
                    players[1].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                }
                spriteBatch.End();

                GraphicsDevice.Viewport = leftOverlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                if (!players[0].isDead)
                {
                    String health = "HP:" + players[0].health.ToString();
                    Vector2 healthOrigin = Font.MeasureString(health) / 2;
                    spriteBatch.DrawString(Font, health, new Vector2(screenWidth / 4 - screenWidth / 4 + Font.MeasureString(health).X, screenHeight / 2 + screenHeight / 2 - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                }
                spriteBatch.End();

                GraphicsDevice.Viewport = rightView;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                currentLevel.Draw(spriteBatch, Content);
                if (!players[1].isDead)
                {
                    players[1].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                }
                if (!players[0].isDead)
                {
                    players[0].Draw(spriteBatch, Content);
                }
                else
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                }
                spriteBatch.End();

                GraphicsDevice.Viewport = rightOverlayView;
                spriteBatch.Begin();
                SpriteFont Font2 = Content.Load<SpriteFont>("basic");
                if (!players[1].isDead)
                {
                    String health2 = "HP:" + players[1].health.ToString();
                    Vector2 healthOrigin2 = Font.MeasureString(health2) / 2;
                    spriteBatch.DrawString(Font, health2, new Vector2(screenWidth / 4 - screenWidth / 4 + Font.MeasureString(health2).X, screenHeight / 2 + screenHeight / 2 - Font.MeasureString(health2).Y), Color.Black, 0, healthOrigin2, 2.0f, SpriteEffects.None, 0.5f);
                }
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(screenWidth / 4 + screenWidth / 4 - screenWidth / 300, screenHeight / 2 - screenHeight / 2 - screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();

                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("White"), new Rectangle(screenWidth / 2 - 1, 0, 2, screenHeight), Color.Black);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.SINGLEPLAYER)
            {
                currentLevel.Draw(spriteBatch, Content);
                players[0].Draw(spriteBatch, Content);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(players[0].camera.getPosition().X - screenWidth / 2 + Font.MeasureString(health).X, players[0].camera.getPosition().Y + screenHeight / 2 - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(players[0].camera.getPosition().X + screenWidth / 2 - screenWidth / 300, players[0].camera.getPosition().Y - screenHeight / 2 - screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                if (!options.usingController)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("Crosshair"), new Vector2(Mouse.GetState().X - (Content.Load<Texture2D>("Crosshair").Width / 2) + players[0].camera.getPosition().X - screenWidth / 2, Mouse.GetState().Y - (Content.Load<Texture2D>("Crosshair").Height / 2) + players[0].camera.getPosition().Y - screenHeight / 2), Color.White);
                }
            }
            

            //TEST spriteBatch.Draw(testBallTexture, new Vector2(10f, 10f), Color.White);

            if (CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE || CurrentGameState == GameState.RESIZINGSCREEN)
            {

            }
            else
            {
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
