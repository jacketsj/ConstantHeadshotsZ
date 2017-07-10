using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using LevelBuilder;
using Microsoft.Xna.Framework.Storage;

namespace ConstantHeadshotsZ
{
    /// <summary>
    /// Effective main class (the "game" class) - singleton
    /// </summary>
    public class CHZ : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Button buttonPlay;
        Button buttonResume;
        Button buttonMainMenu;
        Button buttonQuit;
        Button button2PlayerSplitscreen;
        Button buttonController;
        Button buttonLoadLevel;
        Player[] players;
        Level currentLevel;
        public static Options options;
        public static readonly string highscoresFilename = "highscores.xml";
        TimeSpan logoTime;
        TimeSpan maxLogoTime;
        Viewport leftView;
        Viewport leftOverlayView;
        Viewport rightView;
        Viewport rightOverlayView;
        Viewport spView;
        Viewport overlayView;
        _3DView threeD;
        MouseState oldMouse;
        BasicEffect batchRotation;

#if XBOX
        IAsyncResult storageResult;
        StorageDevice storageDevice;
        IAsyncResult mapurlResult;
        string mapurl;
#endif

        enum GameState
        {
            LOGO, MAINMENU, SINGLEPLAYER, PAUSE, LOSE, TWOPLAYER, TWOPLAYERPAUSE, TWOPLAYERLOSE,
#if XBOX
            CHOOSINGSTORAGEDEVICEBEGIN, CHOOSINGSTORAGEDEVICEEND,
#endif
        }

        #if WINDOWS
        GameState CurrentGameState = GameState.LOGO;
        #endif
        #if XBOX
        GameState CurrentGameState = GameState.CHOOSINGSTORAGEDEVICEBEGIN;
        #endif

        public CHZ()
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
            batchRotation = new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };

            //HIGHSCORES
            #if WINDOWS
            string fullpath = @"Content\" + highscoresFilename;
            if (!File.Exists(fullpath))
            {
                HighScores data = new HighScores();
                data.Score[0] = 20000;
                data.Name[0] = "jacketsj";
                data.Score[1] = 18000;
                data.Name[1] = "jacketsj";
                data.Score[2] = 11000;
                data.Name[2] = "jacketsj";
                data.Score[3] = 9000;
                data.Name[3] = "jacketsj";
                data.Score[4] = 100;
                data.Name[4] = "jacketsj";
                HighScores.SaveHighScores(data, highscoresFilename);
            }
            #endif

            players = new Player[2];

            options = Options.GetInstance();

            currentLevel = new Level(new Solid[4], new Solid[0], new Solid[0], new Vector2[4], Content, Color.LightGreen, new Vector2(1000, 1000), new Vector2(150f, 150f));
            currentLevel.solids[0] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(10, 7)));
            currentLevel.solids[1] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(300, 702)));
            currentLevel.solids[2] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(602, 300)));
            currentLevel.solids[3] = new Solid(new Sprite(Content.Load<Texture2D>("Block"), new Vector2(503, 850)));
            
            currentLevel.zombieSpawners[0] = new Vector2(170, 20);
            currentLevel.zombieSpawners[1] = new Vector2(20, 170);
            currentLevel.zombieSpawners[2] = new Vector2(700, 200);
            currentLevel.zombieSpawners[3] = new Vector2(400, 700);
            threeD = new _3DView(graphics.GraphicsDevice);

            logoTime = new TimeSpan(0, 0, 6);
            maxLogoTime = new TimeSpan(logoTime.Ticks);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place where all content is loaded.
        /// </summary>
        protected override void LoadContent()
        {
            oldMouse = Mouse.GetState();
            // Create a new SpriteBatch to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            graphics.PreferredBackBufferHeight = options.screenHeight;
            graphics.PreferredBackBufferWidth = options.screenWidth;
            graphics.ApplyChanges();

            currentLevel.ResetLevel();

            UpdateButtons();

            players[0] = new Player(new Sprite(Content.Load<Texture2D>("Player"), currentLevel.playerSpawn + new Vector2(Content.Load<Texture2D>("Player").Width, Content.Load<Texture2D>("Player").Height), Color.Blue), 0f, Content, graphics.GraphicsDevice);
            Vector2 player2Spawn = currentLevel.playerSpawn;
            player2Spawn.X += Content.Load<Texture2D>("Player").Width;
            players[1] = new Player(new Sprite(Content.Load<Texture2D>("Player"), player2Spawn, Color.Red), 0f, Content, graphics.GraphicsDevice);

            UpdateViewports();
        }

        public void UpdateButtons()
        {
            buttonPlay = new Button(new Sprite(Content.Load<Texture2D>("PlayButton"), new Vector2()));
            buttonPlay.sprite.vector = new Vector2(((options.screenWidth / 2) - (buttonPlay.sprite.getTexture().Width / 2)), (options.screenHeight / 3 + (options.screenHeight / 6)));

            button2PlayerSplitscreen = new Button(new Sprite(Content.Load<Texture2D>("2PlayerSplitscreenButton"), new Vector2()));
            button2PlayerSplitscreen.sprite.vector = new Vector2(((options.screenWidth / 2) - (button2PlayerSplitscreen.sprite.getTexture().Width / 2)), (options.screenHeight / 3 + (options.screenHeight / 3)));

            buttonResume = new Button(new Sprite(Content.Load<Texture2D>("ResumeButton"), new Vector2()));
            buttonResume.sprite.vector = new Vector2(((options.screenWidth / 2) - (buttonResume.sprite.getTexture().Width / 2)), (options.screenHeight / 3 + (options.screenHeight / 6)));

            buttonMainMenu = new Button(new Sprite(Content.Load<Texture2D>("MainMenuButton"), new Vector2()));
            buttonMainMenu.sprite.vector = new Vector2(((options.screenWidth / 2) - (buttonMainMenu.sprite.getTexture().Width / 2)), (options.screenHeight / 3 + (options.screenHeight / 3)));

            buttonQuit = new Button(new Sprite(Content.Load<Texture2D>("QuitButton"), new Vector2()));
            buttonQuit.sprite.vector = new Vector2(buttonQuit.sprite.getTexture().Width / 2, options.screenHeight - buttonQuit.sprite.getTexture().Height - buttonQuit.sprite.getTexture().Height / 2);

            buttonLoadLevel = new Button(new Sprite(Content.Load<Texture2D>("LoadLevelButton"), new Vector2()));
            buttonLoadLevel.sprite.vector = new Vector2(buttonLoadLevel.sprite.getTexture().Width / 2, (options.screenHeight / 3 + (options.screenHeight / 6)));

            if (options.usingController)
            {
                buttonController = new Button(new Sprite(Content.Load<Texture2D>("ControllerTrue"), new Vector2(((options.screenWidth / 2) - (Content.Load<Texture2D>("ControllerTrue").Width / 2)), options.screenHeight - Content.Load<Texture2D>("ControllerFalse").Height - buttonQuit.sprite.getTexture().Height / 2)));
            }
            else
            {
                buttonController = new Button(new Sprite(Content.Load<Texture2D>("ControllerFalse"), new Vector2(((options.screenWidth / 2) - (Content.Load<Texture2D>("ControllerFalse").Width / 2)), options.screenHeight - Content.Load<Texture2D>("ControllerFalse").Height - buttonQuit.sprite.getTexture().Height / 2)));
            }
        }

        public void UpdateViewports()
        {
            leftView = GraphicsDevice.Viewport;
            leftOverlayView = GraphicsDevice.Viewport;
            rightView = GraphicsDevice.Viewport;
            rightOverlayView = GraphicsDevice.Viewport;
            overlayView = GraphicsDevice.Viewport;
            leftView.Width /= 2;
            leftOverlayView.Width /= 2;
            rightView.Width /= 2;
            rightOverlayView.Width /= 2;
            rightView.X = leftView.Width;
            rightOverlayView.X = leftOverlayView.Width;
            spView = GraphicsDevice.Viewport;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place where all content is unloaded.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here if needed
        }

        bool justPressedFullscreenButton = false;
        bool justPressedPauseButton = false;
        bool justClickedOnscreenButton = false;
        bool justPressedControllerButton = false;
        bool justPressedRotationCameraButton = false;
        bool detectedRotationCameraButton = false;
        bool detectedPitchEnableCameraButton = false;

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

            if (Keyboard.GetState().IsKeyDown(Keys.F) && !justPressedRotationCameraButton && !detectedRotationCameraButton)
            {
                justPressedRotationCameraButton = true;
                detectedRotationCameraButton = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.F))
            {
                justPressedRotationCameraButton = false;
                detectedRotationCameraButton = false;
            }
            else
            {
                justPressedRotationCameraButton = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Q) && !detectedPitchEnableCameraButton)
            {
                options.enablePitchChange = !options.enablePitchChange;
                detectedPitchEnableCameraButton = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Q))
            {
                detectedPitchEnableCameraButton = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                if (!justPressedFullscreenButton)
                {
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                    if (!graphics.IsFullScreen)
                    {
                        graphics.PreferredBackBufferWidth = options.windowedScreenWidth;
                        graphics.PreferredBackBufferHeight = options.windowedScreenHeight;
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    }
                    options.screenWidth = graphics.PreferredBackBufferWidth;
                    options.screenHeight = graphics.PreferredBackBufferHeight;

                    graphics.ApplyChanges();
                    UpdateViewports();
                    UpdateButtons();
                    justPressedFullscreenButton = true;
                }
            }
            else
            {
                justPressedFullscreenButton = false;
            }
            if (CurrentGameState == GameState.LOGO)
            {
                logoTime -= gameTime.ElapsedGameTime;
                if (logoTime <= TimeSpan.Zero || Mouse.GetState().LeftButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space) || justPressedControllerButton || justPressedPauseButton)
                {
                    CurrentGameState = GameState.MAINMENU;
                }
            }
#if XBOX
            else if (CurrentGameState == GameState.CHOOSINGSTORAGEDEVICEBEGIN)
            {
                storageResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                CurrentGameState = GameState.CHOOSINGSTORAGEDEVICEEND;
            }
            else if (CurrentGameState == GameState.CHOOSINGSTORAGEDEVICEEND)
            {
                if (storageResult.IsCompleted)
                {
                    storageDevice = StorageDevice.EndShowSelector(storageResult);
                    HighScores360.LoadHighScores(highscoresFilename, storageDevice);
                    CurrentGameState = GameState.LOGO;
                }
            }
#endif
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
                if (options.player1CameraRotation)
                {
                    players[0].camera.UpdateWithRotation(players[0].playerRotation);
                }
                else
                {
                    players[0].camera.Update(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
                }
                if (options.player2CameraRotation)
                {
                    players[1].camera.UpdateWithRotation(players[1].playerRotation);
                }
                else
                {
                    players[1].camera.Update(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
                }
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
                if (options.player1CameraRotation)
                {
                    players[0].camera.UpdateWithRotation(players[0].playerRotation);
                }
                else
                {
                    players[0].camera.Update(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
                }
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
                else if ((button2PlayerSplitscreen.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    button2PlayerSplitscreen.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.TWOPLAYER;
                }
                else if ((buttonController.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !justClickedOnscreenButton && !justPressedControllerButton)
                {
                    #if WINDOWS
                    options.usingController = !options.usingController;
                    justClickedOnscreenButton = true;
                    justPressedControllerButton = true;
                    if (options.usingController)
                    {
                        buttonController.sprite.setTexture(Content.Load<Texture2D>("ControllerTrue"));
                    }
                    else
                    {
                        buttonController.sprite.setTexture(Content.Load<Texture2D>("ControllerFalse"));
                    }
                    #endif
                }
                else if ((buttonQuit.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    justClickedOnscreenButton = true;
                    this.Exit();
                }
                #if XBOX
                else if ((GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    CurrentGameState = GameState.TYPINGMAPURLBEGIN;

                    buttonLoadLevel.Update(Mouse.GetState());
                    justClickedOnscreenButton = true;
                    
                }
                #endif
                #if WINDOWS
                else if ((buttonLoadLevel.clicked == true) && !justClickedOnscreenButton)
                {
                    LevelData levelData = LevelData.LoadLevel();
                    if (levelData != null)
                    {
                        currentLevel = new Level(levelData, graphics.GraphicsDevice);
                        players[0].sprite.vector = currentLevel.playerSpawn + new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2);
                        Vector2 player2Spawn = currentLevel.playerSpawn;
                        player2Spawn.X += Content.Load<Texture2D>("Player").Width;
                        players[1].sprite.vector = player2Spawn;
                    }

                    buttonLoadLevel.Update(Mouse.GetState());
                    justClickedOnscreenButton = true;
                    
                }
                #endif
                else
                {
                    buttonPlay.Update(Mouse.GetState());
                    button2PlayerSplitscreen.Update(Mouse.GetState());
                    buttonQuit.Update(Mouse.GetState());
                    buttonController.Update(Mouse.GetState());
                    buttonLoadLevel.Update(Mouse.GetState());
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
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() / 1.1f);
                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() * 1.1f);
                    }
                    if (GamePad.GetState(PlayerIndex.Two).Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        players[1].camera.setZoom(players[1].camera.getZoom() / 1.1f);
                    }
                    else if (GamePad.GetState(PlayerIndex.Two).Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        players[1].camera.setZoom(players[1].camera.getZoom() * 1.1f);
                    }
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
                            else if (players[0].weapon == Player.Weapon.LAZERSWORD && players[0].delay <= 0)
                            {
                                if (players[0].lazerSword.Attacking == 0)
                                {
                                    players[0].lazerSword.Attacking = 30;
                                    players[0].delay = LazerSword.delay;
                                }
                            }
                            else if (players[0].weapon == Player.Weapon.HAMMER && players[0].delay <= 0)
                            {
                                if (players[0].hammer.Attacking == 0)
                                {
                                    players[0].hammer.Attacking = 20;
                                    players[0].delay = Hammer.delay;
                                }
                            }
                            else if (players[0].weapon == Player.Weapon.ROCKETLAUNCHER && players[0].delay <= 0)
                            {
                                players[0].rocketLauncher.Attack(Content, currentLevel, players[0]);
                                players[0].delay = RocketLauncher.delay;
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
                            else if (players[1].weapon == Player.Weapon.LAZERSWORD && players[1].delay <= 0)
                            {
                                if (players[1].lazerSword.Attacking == 0)
                                {
                                    players[1].lazerSword.Attacking = 30;
                                    players[1].delay = LazerSword.delay;
                                }
                            }
                            else if (players[1].weapon == Player.Weapon.HAMMER && players[1].delay <= 0)
                            {
                                if (players[1].hammer.Attacking == 0)
                                {
                                    players[1].hammer.Attacking = 20;
                                    players[1].delay = Hammer.delay;
                                }
                            }
                            else if (players[1].weapon == Player.Weapon.ROCKETLAUNCHER && players[1].delay <= 0)
                            {
                                players[1].rocketLauncher.Attack(Content, currentLevel, players[1]);
                                players[1].delay = RocketLauncher.delay;
                            }
                        }
                    }
                    if (!players[0].isDead)
                    {
                        if (options.player1CameraRotation)
                        {
                            if (players[0].aiming)
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f))));
                                Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                                players[0].SetX(newPos.X, currentLevel);
                                players[0].SetY(newPos.Y, currentLevel);
                            }
                            else
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                                Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                                players[0].SetX(newPos.X, currentLevel);
                                players[0].SetY(newPos.Y, currentLevel);
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
                        if (options.player2CameraRotation)
                        {
                            if (players[1].aiming)
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f)), -(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f))));
                                Vector2 newPos = RotateVector2(players[1].sprite.vector + positionChange, players[1].playerRotation, players[1].sprite.vector);
                                players[1].SetX(newPos.X, currentLevel);
                                players[1].SetY(newPos.Y, currentLevel);
                            }
                            else
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                                Vector2 newPos = RotateVector2(players[1].sprite.vector + positionChange, players[1].playerRotation, players[1].sprite.vector);
                                players[1].SetX(newPos.X, currentLevel);
                                players[1].SetY(newPos.Y, currentLevel);
                            }
                        }
                        else
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
                    }
                    if (!players[0].isDead)
                    {
                        if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                        {
                            if (options.player1CameraRotation)
                            {
                                if (!justPressedRotationCameraButton)
                                {
                                    players[0].playerRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                                    players[0].camera.setPitch(players[0].camera.getPitch() + GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * options.player1CameraRotationSpeed);
                                }
                            }
                            else
                            {
                                players[0].camera.setPitch(0);
                                players[0].playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                            }
                        }
                        if (options.player1CameraRotation)
                        {
                            players[0].camera.UpdateWithRotation(players[0].playerRotation);
                        }
                        else
                        {
                            players[0].camera.Update2Player(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
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
                            players[0].camera.setPosition(newVector, currentLevel, new Vector2(options.screenWidth / 2, options.screenHeight));
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
                            players[1].camera.Update2Player(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
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
                            players[1].camera.setPosition(newVector2, currentLevel, new Vector2(options.screenWidth / 2, options.screenHeight));
                        }
                    }
                }
                currentLevel.Update2Player(players, Content, gameTime.ElapsedGameTime);
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
                if (players[0].weapon == Player.Weapon.LAZERSWORD)
                {
                    players[0].lazerSword.Update(Content, currentLevel);
                }
                if (players[1].weapon == Player.Weapon.LAZERSWORD)
                {
                    players[1].lazerSword.Update(Content, currentLevel);
                }
                if (players[0].weapon == Player.Weapon.HAMMER)
                {
                    players[0].hammer.Update(Content, currentLevel);
                }
                if (players[1].weapon == Player.Weapon.HAMMER)
                {
                    players[1].hammer.Update(Content, currentLevel);
                }
                if (players[0].weapon == Player.Weapon.ROCKETLAUNCHER)
                {
                    players[0].rocketLauncher.Update(Content, currentLevel);
                }
                if (players[1].weapon == Player.Weapon.ROCKETLAUNCHER)
                {
                    players[1].rocketLauncher.Update(Content, currentLevel);
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
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() / 1.1f);
                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() * 1.1f);
                    }
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
                        else if (players[0].weapon == Player.Weapon.LAZERSWORD && players[0].delay <= 0)
                        {
                            if (players[0].lazerSword.Attacking == 0)
                            {
                                players[0].lazerSword.Attacking = 30;
                                players[0].delay = LazerSword.delay;
                            }
                        }
                        else if (players[0].weapon == Player.Weapon.HAMMER && players[0].delay <= 0)
                        {
                            if (players[0].hammer.Attacking == 0)
                            {
                                players[0].hammer.Attacking = 20;
                                players[0].delay = Hammer.delay;
                            }
                        }
                        else if (players[0].weapon == Player.Weapon.ROCKETLAUNCHER && players[0].delay <= 0)
                        {
                            players[0].rocketLauncher.Attack(Content, currentLevel, players[0]);
                            players[0].delay = RocketLauncher.delay;
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        if (players[0].aiming)
                        {
                            Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f))));
                            Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                            players[0].SetX(newPos.X, currentLevel);
                            players[0].SetY(newPos.Y, currentLevel);
                        }
                        else
                        {
                            Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                            Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                            players[0].SetX(newPos.X, currentLevel);
                            players[0].SetY(newPos.Y, currentLevel);
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
                    if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                    {
                        if (options.player1CameraRotation)
                        {
                            if (!justPressedRotationCameraButton)
                            {
                                players[0].playerRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                                players[0].camera.setPitch(players[0].camera.getPitch() + GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * options.player1CameraRotationSpeed);
                            }
                        }
                        else
                        {
                            players[0].camera.setPitch(0);
                            players[0].playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        players[0].camera.UpdateWithRotation(players[0].playerRotation);
                    }
                    else
                    {
                        players[0].camera.Update2Player(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
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
                        players[0].camera.setPosition(newVector, currentLevel, new Vector2(options.screenWidth / 2, options.screenHeight));
                    }
                }
                else
                {
                    if (justPressedRotationCameraButton)
                    {
                        options.player1CameraRotation = !options.player1CameraRotation;
                        if (!options.player1CameraRotation)
                        {
                            players[0].camera.setYaw(0);
                        }
                        players[0].camera.setPitch(0);
                    }
                    if (oldMouse.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() / 1.1f);
                    }
                    else if (oldMouse.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                    {
                        players[0].camera.setZoom(players[0].camera.getZoom() * 1.1f);
                    }
                    oldMouse = Mouse.GetState();
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
                                players[0].delay = 20;
                            }
                            else
                            {
                                currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, players[0].sprite.vector, players[0].playerRotation);
                                players[0].delay = 20;
                            }
                        }
                        else if (players[0].weapon == Player.Weapon.LAZERSWORD && players[0].delay <= 0)
                        {
                            if (players[0].lazerSword.Attacking == 0)
                            {
                                players[0].lazerSword.Attacking = 30;
                                players[0].delay = LazerSword.delay;
                            }
                        }
                        else if (players[0].weapon == Player.Weapon.HAMMER && players[0].delay <= 0)
                        {
                            if (players[0].hammer.Attacking == 0)
                            {
                                players[0].hammer.Attacking = 20;
                                players[0].delay = Hammer.delay;
                            }
                        }
                        else if (players[0].weapon == Player.Weapon.ROCKETLAUNCHER && players[0].delay <= 0)
                        {
                            players[0].rocketLauncher.Attack(Content, currentLevel, players[0]);
                            players[0].delay = RocketLauncher.delay;
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        Vector2 toMove = Vector2.Zero;
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            toMove.Y--;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            toMove.X--;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            toMove.Y++;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            toMove.X++;
                        }
                        if (toMove != Vector2.Zero)
                        {
                            toMove.Normalize();
                        }
                        float speed = (5 * (1 / 5f));
                        if (!players[0].aiming)
                        {
                            speed = 7;
                        }
                        toMove *= speed;
                        Vector2 newPos = RotateVector2(players[0].sprite.vector + toMove, players[0].playerRotation, players[0].sprite.vector);
                        players[0].SetX(newPos.X, currentLevel);
                        players[0].SetY(newPos.Y, currentLevel);
                    }
                    else
                    {
                        Vector2 toMove = Vector2.Zero;
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            toMove.Y--;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.A))
                        {
                            toMove.X--;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            toMove.Y++;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.D))
                        {
                            toMove.X++;
                        }
                        if (toMove != Vector2.Zero)
                        {
                            toMove.Normalize();
                        }
                        float speed = (5 * (1 / 5f));
                        if (!players[0].aiming)
                        {
                            speed = 7;
                        }
                        players[0].SetX(players[0].sprite.getX() + toMove.X * speed, currentLevel);
                        players[0].SetY(players[0].sprite.getY() + toMove.Y * speed, currentLevel);
                    }
                    Vector2 distance;
                    if (options.player1CameraRotation)
                    {
                        distance = new Vector2(Mouse.GetState().X - options.screenWidth / 2, Mouse.GetState().Y - options.screenHeight / 2);
                    }
                    else
                    {
                        distance = new Vector2((Mouse.GetState().X + (players[0].camera.getPosition().X - options.screenWidth / 2)) - players[0].sprite.getX(), (Mouse.GetState().Y + (players[0].camera.getPosition().Y - options.screenHeight / 2)) - players[0].sprite.getY());
                    }
                    if (options.player1CameraRotation)
                    {
                        if (!justPressedRotationCameraButton)
                        {
                            players[0].playerRotation += distance.X * options.player1CameraRotationSpeed / 3;
                            players[0].camera.setPitch(players[0].camera.getPitch() + distance.Y * options.player1CameraRotationSpeed / 3);
                        }
                    }
                    else
                    {
                        players[0].camera.setPitch(0);
                        players[0].playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    }
                    if (options.player1CameraRotation)
                    {
                        players[0].camera.UpdateWithRotation(players[0].playerRotation);
                    }
                    else
                    {
                        players[0].camera.Update(currentLevel, new Vector2(options.screenWidth, options.screenHeight));
                    }
                    Vector2 newVector = players[0].camera.getPosition();
                    if (!options.player1CameraRotation)
                    {
                        players[0].camera.setPosition(newVector, currentLevel, new Vector2(options.screenWidth, options.screenHeight));
                    }

                    if (options.player1CameraRotation)
                    {
                        Mouse.SetPosition(options.screenWidth / 2, options.screenHeight / 2);
                    }
                }
                currentLevel.Update(players[0], Content, gameTime.ElapsedGameTime);
                if (players[0].health <= 0)
                {
                    CurrentGameState = GameState.LOSE;
                    SaveHighScore();
                }
                if (players[0].weapon == Player.Weapon.LAZERSWORD)
                {
                    players[0].lazerSword.Update(Content, currentLevel);
                }
                if (players[0].weapon == Player.Weapon.HAMMER)
                {
                    players[0].hammer.Update(Content, currentLevel);
                }
                if (players[0].weapon == Player.Weapon.ROCKETLAUNCHER)
                {
                    players[0].rocketLauncher.Update(Content, currentLevel);
                }
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                justClickedOnscreenButton = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Released)
            {
                justPressedControllerButton = false;
            }

            base.Update(gameTime);
        }

        public static Vector2 RotateVector2(Vector2 point, float radians)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            return new Vector2(point.X * cosRadians - point.Y * sinRadians, point.X * sinRadians + point.Y * cosRadians);
        }

        public static Vector2 RotateVector2(Vector2 point, float radians, Vector2 pivot)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            Vector2 translatedPoint = new Vector2();
            translatedPoint.X = point.X - pivot.X;
            translatedPoint.Y = point.Y - pivot.Y;

            Vector2 rotatedPoint = new Vector2();
            rotatedPoint.X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + pivot.X;
            rotatedPoint.Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + pivot.Y;

            return rotatedPoint;
        }

        private float DegreeToRadian(float angle)
        {
            return ((float)Math.PI) * angle / 180f;
        }

        private void SaveHighScore()
        {
            HighScores loadingData = HighScores.LoadHighScores(highscoresFilename);
            HighScores data = new HighScores();
            data.Score[0] = loadingData.Score[0];
            data.Name[0] = loadingData.Name[0];
            data.Score[1] = loadingData.Score[1];
            data.Name[1] = loadingData.Name[1];
            data.Score[2] = loadingData.Score[2];
            data.Name[2] = loadingData.Name[2];
            data.Score[3] = loadingData.Score[3];
            data.Name[3] = loadingData.Name[3];
            data.Score[4] = loadingData.Score[4];
            data.Name[4] = loadingData.Name[4];

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

                HighScores.SaveHighScores(data, highscoresFilename);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //Drawing code

            if (!(CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE || CurrentGameState == GameState.SINGLEPLAYER || CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE))
            {
                spriteBatch.Begin();
            }


            if (CurrentGameState == GameState.LOGO)
            {
                float logoAlpha = (((float)logoTime.Milliseconds) / ((float)maxLogoTime.Milliseconds));
                spriteBatch.Draw(Content.Load<Texture2D>("jacketsjpresents"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), Color.White);
                Color col = Color.Black;
                col.A = (byte)(col.A * ((float)logoTime.Ticks / (float)maxLogoTime.Ticks));
                spriteBatch.Draw(Content.Load<Texture2D>("Particle"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), col);
            }
            else if (CurrentGameState == GameState.LOSE)
            {
                GraphicsDevice.Viewport = spView;
                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.DrawWithoutHealth(spriteBatch, Content);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, options);
                }
                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(options.screenWidth / 2, options.screenHeight / 2), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.MAINMENU)
            {
                spriteBatch.Draw(Content.Load<Texture2D>("ConstantHeadshotsZTitlePage"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), Color.White);
                buttonPlay.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("AButton"), new Vector2(buttonPlay.sprite.vector.X + buttonPlay.sprite.getTexture().Width + Content.Load<Texture2D>("AButton").Width / 2, buttonPlay.sprite.vector.Y + Content.Load<Texture2D>("AButton").Height / 2), Color.White);
                button2PlayerSplitscreen.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("YButton"), new Vector2(button2PlayerSplitscreen.sprite.vector.X + button2PlayerSplitscreen.sprite.getTexture().Width + Content.Load<Texture2D>("YButton").Width / 2, button2PlayerSplitscreen.sprite.vector.Y + Content.Load<Texture2D>("YButton").Height / 2), Color.White);
                buttonQuit.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("BackButton"), new Vector2(buttonQuit.sprite.vector.X + buttonQuit.sprite.getTexture().Width + Content.Load<Texture2D>("BackButton").Width / 2, buttonQuit.sprite.vector.Y + Content.Load<Texture2D>("BackButton").Height / 2), Color.White);
                buttonController.Draw(spriteBatch);
                spriteBatch.Draw(Content.Load<Texture2D>("XButton"), new Vector2(buttonController.sprite.vector.X + buttonController.sprite.getTexture().Width + Content.Load<Texture2D>("XButton").Width / 2, buttonController.sprite.vector.Y + Content.Load<Texture2D>("XButton").Height / 2), Color.White);
                buttonLoadLevel.Draw(spriteBatch);
                HighScores highScores = HighScores.LoadHighScores(highscoresFilename);
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
                        spriteBatch.DrawString(Font, scores, new Vector2(options.screenWidth - options.screenWidth / 200, options.screenHeight - options.screenHeight / 200), Color.Black, 0, scoresOrigin, 1.0f, SpriteEffects.None, 0.5f);
                    }
                }
            }
            else if (CurrentGameState == GameState.PAUSE)
            {
                GraphicsDevice.Viewport = spView;

                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
                    players[0].Draw(spriteBatch, Content);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, options);
                }

                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(0 + Font.MeasureString(health).X, options.screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(options.screenWidth - options.screenWidth / 300, 0 + options.screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(Content.Load<Texture2D>("PauseBackground"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), Color.White);
                spriteBatch.Draw(buttonResume.sprite.getTexture(), new Vector2(buttonResume.sprite.vector.X + (0), buttonResume.sprite.vector.Y + (0)), buttonResume.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("AButton"), new Vector2((buttonResume.sprite.vector.X + buttonResume.sprite.getTexture().Width + Content.Load<Texture2D>("AButton").Width / 2) + (0), (buttonResume.sprite.vector.Y + Content.Load<Texture2D>("AButton").Height / 2) + (0)), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y + (0)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                spriteBatch.Draw(buttonQuit.sprite.getTexture(), new Vector2(buttonQuit.sprite.vector.X + (0), buttonQuit.sprite.vector.Y + (0)), buttonQuit.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BackButton"), new Vector2((buttonQuit.sprite.vector.X + buttonQuit.sprite.getTexture().Width + Content.Load<Texture2D>("BackButton").Width / 2) + (0), (buttonQuit.sprite.vector.Y + Content.Load<Texture2D>("BackButton").Height / 2) + (0)), Color.White);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.TWOPLAYERLOSE)
            {
                GraphicsDevice.Viewport = leftView;

                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content, options);
                }


                GraphicsDevice.Viewport = rightView;

                if (!options.player23D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[1], 2);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[1].sprite.vector - new Vector2(players[1].sprite.getTexture().Width / 2, players[1].sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), players[0].sprite.vector - new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 2, Content, options);
                }


                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Rectangle(0, 0, options.screenWidth, options.screenHeight), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y + (0)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(options.screenWidth / 2, options.screenHeight / 2), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.TWOPLAYERPAUSE)
            {
                GraphicsDevice.Viewport = leftView;

                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
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
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content, options);
                }


                GraphicsDevice.Viewport = rightView;

                if (!options.player23D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[1], 2);
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
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content, options);
                }


                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("PauseBackground"), new Rectangle((int)(0), (int)(0), options.screenWidth, options.screenHeight), Color.White);
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

                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
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
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content, options);
                }

                GraphicsDevice.Viewport = leftOverlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                if (!players[0].isDead)
                {
                    String health = "HP:" + players[0].health.ToString();
                    Vector2 healthOrigin = Font.MeasureString(health) / 2;
                    spriteBatch.DrawString(Font, health, new Vector2(options.screenWidth / 4 - options.screenWidth / 4 + Font.MeasureString(health).X, options.screenHeight / 2 + options.screenHeight / 2 - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                }
                spriteBatch.End();

                GraphicsDevice.Viewport = rightView;

                if (!options.player23D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[1].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[1], 2);
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
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 2, Content, options);
                }

                GraphicsDevice.Viewport = rightOverlayView;
                spriteBatch.Begin();
                SpriteFont Font2 = Content.Load<SpriteFont>("basic");
                if (!players[1].isDead)
                {
                    String health2 = "HP:" + players[1].health.ToString();
                    Vector2 healthOrigin2 = Font.MeasureString(health2) / 2;
                    spriteBatch.DrawString(Font, health2, new Vector2(options.screenWidth / 4 - options.screenWidth / 4 + Font.MeasureString(health2).X, options.screenHeight / 2 + options.screenHeight / 2 - Font.MeasureString(health2).Y), Color.Black, 0, healthOrigin2, 2.0f, SpriteEffects.None, 0.5f);
                }
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(options.screenWidth / 4 + options.screenWidth / 4 - options.screenWidth / 300, options.screenHeight / 2 - options.screenHeight / 2 - options.screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();

                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("White"), new Rectangle(options.screenWidth / 2 - 1, 0, 2, options.screenHeight), Color.Black);
                spriteBatch.End();
            }
            else if (CurrentGameState == GameState.SINGLEPLAYER)
            {
                if (!options.player13D)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
                    players[0].Draw(spriteBatch, Content);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, options);
                }


                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(Font.MeasureString(health).X, options.screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2( options.screenWidth - options.screenWidth / 300, 0 + options.screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                if (!options.usingController && !options.player1CameraRotation)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("Crosshair"), new Vector2(Mouse.GetState().X - (Content.Load<Texture2D>("Crosshair").Width / 2), Mouse.GetState().Y - (Content.Load<Texture2D>("Crosshair").Height / 2)), Color.White);
                }
                spriteBatch.End();
            }


            if (!(CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE || CurrentGameState == GameState.SINGLEPLAYER || CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE))
            {
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}