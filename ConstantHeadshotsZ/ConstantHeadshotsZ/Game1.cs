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
        Button buttonController;
        Button buttonLoadLevel;
        int screenWidth = 800;
        int windowedScreenWidth = 800;
        int screenHeight = 600;
        int windowedScreenHeight = 600;
        Player[] players;
        Level currentLevel;
        public static Options options;
        public static readonly string highscoresFilename = "highscores.xml";
        int logoTime = 150;
        int maxLogoTime = 150;
        Viewport leftView;
        Viewport leftOverlayView;
        Viewport rightView;
        Viewport rightOverlayView;
        Viewport spView;
        Viewport overlayView;
        _3DView threeD;
        MouseState oldMouse;
        BasicEffect batchRotation;

        //360
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
            //TYPINGMAPURLBEGIN, TYPINGMAPURLEND
        }

        #if WINDOWS
        GameState CurrentGameState = GameState.LOGO;
        #endif
        #if XBOX
        GameState CurrentGameState = GameState.CHOOSINGSTORAGEDEVICEBEGIN;
        #endif

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
            //Components.Add(new GamerServicesComponent(this));

            // TODO: Add your initialization logic here

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

            options = new Options();


            //graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
            //graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
            //screenHeight = graphics.PreferredBackBufferHeight;
            //screenWidth = graphics.PreferredBackBufferWidth;
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
            threeD = new _3DView(graphics.GraphicsDevice);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            oldMouse = Mouse.GetState();
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
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

            buttonLoadLevel = new Button(new Sprite(Content.Load<Texture2D>("LoadLevelButton"), new Vector2()));
            buttonLoadLevel.sprite.vector = new Vector2(buttonLoadLevel.sprite.getTexture().Width / 2, (screenHeight / 3 + (screenHeight / 6)));

            if (options.usingController)
            {
                buttonController = new Button(new Sprite(Content.Load<Texture2D>("ControllerTrue"), new Vector2(((screenWidth / 2) - (Content.Load<Texture2D>("ControllerTrue").Width / 2)), screenHeight - Content.Load<Texture2D>("ControllerFalse").Height - buttonQuit.sprite.getTexture().Height / 2)));
            }
            else
            {
                buttonController = new Button(new Sprite(Content.Load<Texture2D>("ControllerFalse"), new Vector2(((screenWidth / 2) - (Content.Load<Texture2D>("ControllerFalse").Width / 2)), screenHeight - Content.Load<Texture2D>("ControllerFalse").Height - buttonQuit.sprite.getTexture().Height / 2)));
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
        bool justPressedControllerButton = false;
        bool justPressedRotationCameraButton = false;
        bool detectedRotationCameraButton = false;

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

            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                if (!justPressedFullscreenButton)
                {
                    graphics.IsFullScreen = !graphics.IsFullScreen;
                    if (!graphics.IsFullScreen)
                    {
                        graphics.PreferredBackBufferWidth = windowedScreenWidth;
                        graphics.PreferredBackBufferHeight = windowedScreenHeight;
                    }
                    else
                    {
                        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    }
                    screenWidth = graphics.PreferredBackBufferWidth;
                    screenHeight = graphics.PreferredBackBufferHeight;
                    //GraphicsDevice.Viewport = new Viewport(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y, screenWidth, screenHeight);
                    //GraphicsDevice.Viewport.Height = screenHeight;
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
                logoTime -= 1;
                if (logoTime <= 0)
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
            //else if (CurrentGameState == GameState.TYPINGMAPURLBEGIN)
            //{
            //    mapurlResult = Guide.BeginShowKeyboardInput(PlayerIndex.One, "Map URL", "Type in the URL for your .chz map.", "http://", null, null);
            //    CurrentGameState = GameState.TYPINGMAPURLEND;
            //}
            //else if (CurrentGameState == GameState.TYPINGMAPURLEND)
            //{
            //    if (mapurlResult.IsCompleted)
            //    {
            //        mapurl = Guide.EndShowKeyboardInput(mapurlResult);

            //        LevelData levelData = null;
            //        //LevelData levelData = LevelData.LoadLevel360(mapurl);
            //        if (levelData != null)
            //        {
            //            currentLevel = new Level(levelData, graphics.GraphicsDevice);
            //            players[0].sprite.vector = currentLevel.playerSpawn + new Vector2(players[0].sprite.getTexture().Width / 2, players[0].sprite.getTexture().Height / 2);
            //            Vector2 player2Spawn = currentLevel.playerSpawn;
            //            player2Spawn.X += Content.Load<Texture2D>("Player").Width;
            //            players[1].sprite.vector = player2Spawn;
            //        }
            //        CurrentGameState = GameState.MAINMENU;
            //    }
            //}
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
                    players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                }
                if (options.player2CameraRotation)
                {
                    players[1].camera.UpdateWithRotation(players[1].playerRotation);
                }
                else
                {
                    players[1].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
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
                    players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
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
                    
                    /*
                    LevelData levelData = new LevelData();
                    levelData.backgroundColor = currentLevel.backgroundColor;
                    levelData.levelHeight = currentLevel.levelHeight;
                    levelData.levelWidth = currentLevel.levelWidth;
                    levelData.maxAmountOfZombies = currentLevel.maxAmountOfZombies;
                    levelData.playerSpawn = currentLevel.playerSpawn;
                    levelData.solids = new SolidData[currentLevel.solids.Length];
                    levelData.textures = new TextureData[0];
                    Color[] BackgroundTextureData = new Color[currentLevel.background.Width * currentLevel.background.Height];
                    currentLevel.background.GetData(BackgroundTextureData);
                    int textureReference2 = 0;
                    bool foundMatch2 = false;
                    for (int i5 = 0; i5 < levelData.textures.Length; i5++)
                    {
                        if (levelData.textures[i5].Colors == BackgroundTextureData)
                        {
                            textureReference2 = i5;
                            foundMatch2 = true;
                        }
                    }
                    if (!foundMatch2)
                    {
                        TextureData[] newTextureData = new TextureData[levelData.textures.Length + 1];
                        for (int i4 = 0; i4 < levelData.textures.Length; i4++)
                        {
                            newTextureData[i4] = levelData.textures[i4];
                        }
                        newTextureData[levelData.textures.Length] = new TextureData(BackgroundTextureData, currentLevel.background.Width, currentLevel.background.Height);
                        textureReference2 = levelData.textures.Length;
                        levelData.textures = newTextureData;
                    }
                    levelData.backgroundReference = textureReference2;
                    for (int i = 0; i < levelData.solids.Length; i++)
                    {
                        currentLevel.solids[i].sprite.UpdateTextureData();
                        int textureReference = 0;
                        bool foundMatch = false;
                        for (int i2 = 0; i2 < levelData.textures.Length; i2++)
                        {
                            if (levelData.textures[i2].Colors == currentLevel.solids[i].sprite.textureData)
                            {
                                textureReference = i2;
                                foundMatch = true;
                            }
                        }
                        if (!foundMatch)
                        {
                            TextureData[] newTextureData = new TextureData[levelData.textures.Length + 1];
                            for (int i3 = 0; i3 < levelData.textures.Length; i3++)
                            {
                                newTextureData[i3] = levelData.textures[i3];
                            }
                            newTextureData[levelData.textures.Length] = new TextureData(currentLevel.solids[i].sprite.textureData, currentLevel.solids[i].sprite.getTexture().Width, currentLevel.solids[i].sprite.getTexture().Height);
                            textureReference = levelData.textures.Length;
                            levelData.textures = newTextureData;
                        }
                        levelData.solids[i] = new SolidData(currentLevel.solids[i].sprite.vector, textureReference, currentLevel.solids[i].sprite.getTint());
                    }
                    levelData.spawnTimer = currentLevel.spawnTimer;
                    levelData.zombieSpawnAcceleration = currentLevel.zombieSpawnAcceleration;
                    levelData.zombieSpawners = currentLevel.zombieSpawners;
                    LevelData.SaveLevel(levelData);
                    */

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
                                //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                                //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                                //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
                                Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                                players[0].SetX(newPos.X, currentLevel);
                                players[0].SetY(newPos.Y, currentLevel);
                                //players[0].SetX(players[0].sprite.getX() + (GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f))) * (float)Math.Cos(players[0].playerRotation), currentLevel);
                                //players[0].SetY(players[0].sprite.getY() - (GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f))) * (float)Math.Sin(players[0].playerRotation), currentLevel);
                                /*
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
                                */
                            }
                            else
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                                //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                                //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                                //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
                                Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                                players[0].SetX(newPos.X, currentLevel);
                                players[0].SetY(newPos.Y, currentLevel);
                                //players[0].SetX(players[0].sprite.getX() + newPositionChange.X, currentLevel);
                                //players[0].SetY(players[0].sprite.getY() - newPositionChange.Y, currentLevel);
                                /*
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
                                */
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
                                //Vector2 rotationChange = new Vector2((float)Math.Cos(players[1].playerRotation), (float)Math.Sin(players[1].playerRotation));
                                //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[1].playerRotation) - positionChange.Y * (float)Math.Sin(players[1].playerRotation), positionChange.X * (float)Math.Sin(players[1].playerRotation) + positionChange.Y * (float)Math.Cos(players[1].playerRotation));
                                //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[1].playerRotation), Vector2.Zero);
                                Vector2 newPos = RotateVector2(players[1].sprite.vector + positionChange, players[1].playerRotation, players[1].sprite.vector);
                                players[1].SetX(newPos.X, currentLevel);
                                players[1].SetY(newPos.Y, currentLevel);
                                //players[1].SetX(players[1].sprite.getX() + (GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f))) * (float)Math.Cos(players[1].playerRotation), currentLevel);
                                //players[1].SetY(players[1].sprite.getY() - (GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f))) * (float)Math.Sin(players[1].playerRotation), currentLevel);
                                /*
                                float addedX = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X;
                                float multipliedX = (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f));
                                float addedY = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y;
                                float multipliedY = (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f));
                                float newX = players[1].sprite.getX();
                                float newY = players[1].sprite.getY();
                                float rotation = MathHelper.ToDegrees((float)Math.Atan2(addedY, addedX));
                                rotation += players[1].playerRotation;
                                //rotation += 300;
                                while (rotation > 359)
                                {
                                    rotation -= 360;
                                }
                                while (rotation < 0)
                                {
                                    rotation += 360;
                                }
                                players[1].SetX(newX + (float)Math.Cos(MathHelper.ToRadians(rotation)), currentLevel);
                                players[1].SetY(newY - (float)Math.Sin(MathHelper.ToRadians(rotation)), currentLevel);
                                //players[1].SetX(players[1].sprite.getX() + GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f)), currentLevel);
                                //players[1].SetY(players[1].sprite.getY() - GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.Two).Triggers.Left / 5f)), currentLevel);
                                */
                            }
                            else
                            {
                                Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                                //Vector2 rotationChange = new Vector2((float)Math.Cos(players[1].playerRotation), (float)Math.Sin(players[1].playerRotation));
                                //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[1].playerRotation) - positionChange.Y * (float)Math.Sin(players[1].playerRotation), positionChange.X * (float)Math.Sin(players[1].playerRotation) + positionChange.Y * (float)Math.Cos(players[1].playerRotation));
                                //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[1].playerRotation), Vector2.Zero);
                                Vector2 newPos = RotateVector2(players[1].sprite.vector + positionChange, players[1].playerRotation, players[1].sprite.vector);
                                players[1].SetX(newPos.X, currentLevel);
                                players[1].SetY(newPos.Y, currentLevel);
                                //players[1].SetX(players[1].sprite.getX() + newPositionChange.X, currentLevel);
                                //players[1].SetY(players[1].sprite.getY() - newPositionChange.Y, currentLevel);
                                /*
                                float addedX = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X;
                                float multipliedX = 7;
                                float addedY = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y;
                                float multipliedY = 7;
                                float newX = players[1].sprite.getX();
                                float newY = players[1].sprite.getY();
                                float rotation = MathHelper.ToDegrees((float)Math.Atan2(addedY, addedX));
                                rotation += players[1].playerRotation;
                                //rotation += 300;
                                while (rotation > 359)
                                {
                                    rotation -= 360;
                                }
                                while (rotation < 0)
                                {
                                    rotation += 360;
                                }
                                players[1].SetX(newX + (float)Math.Cos(MathHelper.ToRadians(rotation)) * multipliedX, currentLevel);
                                players[1].SetY(newY - (float)Math.Sin(MathHelper.ToRadians(rotation)) * multipliedY, currentLevel);
                                //players[1].SetX(players[1].sprite.getX() + GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                                //players[1].SetY(players[1].sprite.getY() - GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                                */
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
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
                            Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                            players[0].SetX(newPos.X, currentLevel);
                            players[0].SetY(newPos.Y, currentLevel);
                        }
                        else
                        {
                            Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
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
                else
                {
                    if (justPressedRotationCameraButton)
                    {
                        options.player1CameraRotation = !options.player1CameraRotation;
                        if (!options.player1CameraRotation)
                        {
                            players[0].camera.setRotation(0);
                        }
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
                        //players[0].SetX(players[0].sprite.getX() + toMove.X * speed, currentLevel);
                        //players[0].SetY(players[0].sprite.getY() + toMove.Y * speed, currentLevel);
                        //if (players[0].aiming)
                        //{
                        //    Vector2 positionChange = new Vector2(0, 0);
                        //    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        //    {
                        //        //movementY -= 1;
                        //        positionChange.Y -= 5 * (1 / 5f);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.A))
                        //    {
                        //        //movementX -= 1;
                        //        positionChange.X -= 5 * (1 / 5f);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        //    {
                        //        //movementY += 1;
                        //        positionChange.Y += 5 * (1 / 5f);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.D))
                        //    {
                        //        //movementX += 1;
                        //        positionChange.X += 5 * (1 / 5f);
                        //    }
                        //    //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                        //    //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                        //    //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
                        //    Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                        //    players[0].SetX(newPos.X, currentLevel);
                        //    players[0].SetY(newPos.Y, currentLevel);
                        //}
                        //else
                        //{
                        //    Vector2 positionChange = new Vector2(0, 0);
                        //    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        //    {
                        //        //movementY -= 1;
                        //        positionChange.Y -= 7;
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.A))
                        //    {
                        //        //movementX -= 1;
                        //        positionChange.X -= 7;
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        //    {
                        //        //movementY += 1;
                        //        positionChange.Y += 7;
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.D))
                        //    {
                        //        //movementX += 1;
                        //        positionChange.X += 7;
                        //    }
                        //    //Vector2 rotationChange = new Vector2((float)Math.Cos(players[0].playerRotation), (float)Math.Sin(players[0].playerRotation));
                        //    //positionChange = new Vector2(positionChange.X * (float)Math.Cos(players[0].playerRotation) - positionChange.Y * (float)Math.Sin(players[0].playerRotation), positionChange.X * (float)Math.Sin(players[0].playerRotation) + positionChange.Y * (float)Math.Cos(players[0].playerRotation));
                        //    //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(players[0].playerRotation), Vector2.Zero);
                        //    Vector2 newPos = RotateVector2(players[0].sprite.vector + positionChange, players[0].playerRotation, players[0].sprite.vector);
                        //    players[0].SetX(newPos.X, currentLevel);
                        //    players[0].SetY(newPos.Y, currentLevel);
                        //}
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
                        //if (players[0].aiming)
                        //{
                        //    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        //    {
                        //        //movementY -= 1;
                        //        players[0].SetY(players[0].sprite.getY() + -1 * (5 * (1 / 5f)), currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.A))
                        //    {
                        //        //movementX -= 1;
                        //        players[0].SetX(players[0].sprite.getX() + -1 * (5 * (1 / 5f)), currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        //    {
                        //        //movementY += 1;
                        //        players[0].SetY(players[0].sprite.getY() + 1 * (5 * (1 / 5f)), currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.D))
                        //    {
                        //        //movementX += 1;
                        //        players[0].SetX(players[0].sprite.getX() + 1 * (5 * (1 / 5f)), currentLevel);
                        //    }
                        //}
                        //else
                        //{
                        //    if (Keyboard.GetState().IsKeyDown(Keys.W))
                        //    {
                        //        //movementY -= 1;
                        //        players[0].SetY(players[0].sprite.getY() + -1 * 7, currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.A))
                        //    {
                        //        //movementX -= 1;
                        //        players[0].SetX(players[0].sprite.getX() + -1 * 7, currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        //    {
                        //        //movementY += 1;
                        //        players[0].SetY(players[0].sprite.getY() + 1 * 7, currentLevel);
                        //    }
                        //    if (Keyboard.GetState().IsKeyDown(Keys.D))
                        //    {
                        //        //movementX += 1;
                        //        players[0].SetX(players[0].sprite.getX() + 1 * 7, currentLevel);
                        //    }
                        //}
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
                    /*
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
                    */
                    Vector2 distance;
                    if (options.player1CameraRotation)
                    {
                        distance = new Vector2(Mouse.GetState().X - screenWidth / 2, Mouse.GetState().Y - screenHeight / 2);
                    }
                    else
                    {
                        distance = new Vector2((Mouse.GetState().X + (players[0].camera.getPosition().X - screenWidth / 2)) - players[0].sprite.getX(), (Mouse.GetState().Y + (players[0].camera.getPosition().Y - screenHeight / 2)) - players[0].sprite.getY());
                    }
                    if (options.player1CameraRotation)
                    {
                        players[0].playerRotation += distance.X * options.player1CameraRotationSpeed / 3;
                    }
                    else
                    {
                        players[0].playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    }
                    //players[0].playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    if (options.player1CameraRotation)
                    {
                        players[0].camera.UpdateWithRotation(players[0].playerRotation);
                    }
                    else
                    {
                        players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    }
                    //players[0].camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    Vector2 newVector = players[0].camera.getPosition();
                    /*
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
                    */
                    if (!options.player1CameraRotation)
                    {
                        players[0].camera.setPosition(newVector, currentLevel, new Vector2(screenWidth, screenHeight));
                    }

                    if (options.player1CameraRotation)
                    {
                        Mouse.SetPosition(screenWidth / 2, screenHeight / 2);
                    }
                }
                currentLevel.Update(players[0], Content);
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

            //GamerServicesDispatcher.Update();

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            if (1 == 2)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
            }
            else if (CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE || CurrentGameState == GameState.SINGLEPLAYER || CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE)
            {

            }
            else
            {
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content);
                }
                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(screenWidth / 2, screenHeight / 2), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.End();
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
                        spriteBatch.DrawString(Font, scores, new Vector2(screenWidth - screenWidth / 200, screenHeight - screenHeight / 200), Color.Black, 0, scoresOrigin, 1.0f, SpriteEffects.None, 0.5f);
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content);
                }

                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(0 + Font.MeasureString(health).X, screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2(screenWidth - screenWidth / 300, 0 + screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                spriteBatch.Draw(Content.Load<Texture2D>("PauseBackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content);
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 2, Content);
                }


                GraphicsDevice.Viewport = overlayView;

                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y + (0)), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename).Score[0].ToString();
                String youScored = "You scored " + players[0].score.ToString() + "\nScore to beat:" + scoreToBeat;
                Vector2 youScoredOrigin = new Vector2(Font.MeasureString(youScored).X / 2, 0);
                spriteBatch.DrawString(Font, youScored, new Vector2(screenWidth / 2, screenHeight / 2), Color.Black, 0, youScoredOrigin, 2.0f, SpriteEffects.None, 0.5f);
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content);
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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content);
                }


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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 1, Content);
                }

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
                    threeD.Draw(GraphicsDevice, currentLevel, players, true, 2, Content);
                }

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
                /*
                GraphicsDevice.Viewport = spView;

                Matrix projection = Matrix.CreateOrthographicOffCenter(0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height, 0, 0, 1);
                Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                BasicEffect basicEffect = new BasicEffect(graphics.GraphicsDevice);

                basicEffect.World = Matrix.Identity;
                basicEffect.View = Matrix.Identity;
                basicEffect.Projection = halfPixelOffset * projection;

                basicEffect.TextureEnabled = true;
                basicEffect.VertexColorEnabled = true;
                 * */

                //players[0].camera.updateEffect(graphics.GraphicsDevice);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, players[0].camera.get_transformation(graphics.GraphicsDevice));
                if (!options.player13D)
                {
                    currentLevel.Draw(spriteBatch, Content, players[0], 1);
                    players[0].Draw(spriteBatch, Content);
                    spriteBatch.End();
                    GraphicsDevice.Viewport = overlayView;
                    spriteBatch.Begin();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content);
                }


                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + players[0].health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(Font.MeasureString(health).X, screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + players[0].score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2( screenWidth - screenWidth / 300, 0 + screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                if (!options.usingController && !options.player1CameraRotation)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("Crosshair"), new Vector2(Mouse.GetState().X - (Content.Load<Texture2D>("Crosshair").Width / 2), Mouse.GetState().Y - (Content.Load<Texture2D>("Crosshair").Height / 2)), Color.White);
                }
                spriteBatch.End();
            }
            

            //TEST spriteBatch.Draw(testBallTexture, new Vector2(10f, 10f), Color.White);

            if (CurrentGameState == GameState.TWOPLAYER || CurrentGameState == GameState.TWOPLAYERLOSE || CurrentGameState == GameState.TWOPLAYERPAUSE || CurrentGameState == GameState.SINGLEPLAYER || CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE)
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
