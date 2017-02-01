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
using LevelBuilder;
using Microsoft.Xna.Framework.Net;

namespace ConstantHeadshotsZMP
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
        public readonly string highscoresFilename = "highscores.xml";
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
        const int maxGamers = 16;
        const int maxLocalGamers = 2;
        bool server = false;
        Player player;
        public ClientInfo clientInfo;
        bool UpdateTextures = false;

        enum GameState
        {
            LOGO, MAINMENU, PLAYING, PAUSE, LOSE
        }

        GameState CurrentGameState = GameState.LOGO;

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
            //UpdateServerTexturesWithoutLock(serverInf);


            player = new Player(new Sprite(Content.Load<Texture2D>("player"), currentLevel.playerSpawn), 0f, Content, graphics.GraphicsDevice, "player");
            players = new Player[1];
            players[0] = player;


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

            server = false;
            clientInfo = new ClientInfo();

            //GameServer.serverInfo = UpdateServerTextures(GameServer.serverInfo);

            // TODO: use this.Content to load your game content here

            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.ApplyChanges();

            currentLevel.ResetLevel();

            UpdateButtons();

            /*
            player = new Player(new Sprite(Content.Load<Texture2D>("Player"), currentLevel.playerSpawn + new Vector2(Content.Load<Texture2D>("Player").Width, Content.Load<Texture2D>("Player").Height), Color.Blue), 0f, Content, graphics.GraphicsDevice);
            Vector2 player2Spawn = currentLevel.playerSpawn;
            player2Spawn.X += Content.Load<Texture2D>("Player").Width;
            players[1] = new Player(new Sprite(Content.Load<Texture2D>("Player"), player2Spawn, Color.Red), 0f, Content, graphics.GraphicsDevice);
            */

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

        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            Player[] newPlayers = new Player[players.Length + 1];

            for (int i = 0; i < players.Length; i++)
            {
                newPlayers[i] = players[i];
            }

            newPlayers[players.Length] = new Player(new Sprite(Content.Load<Texture2D>("Player"), currentLevel.playerSpawn + new Vector2(Content.Load<Texture2D>("Player").Width, Content.Load<Texture2D>("Player").Height), Color.Blue), 0f, Content, graphics.GraphicsDevice, e.Gamer.DisplayName);

            players = newPlayers;

            //e.Gamer.Tag = new Tank(gamerIndex, Content, screenWidth, screenHeight);
        }

        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            CurrentGameState = GameState.MAINMENU;
        }

        void DrawMessage(string message)
        {
            if (!BeginDraw())
                return;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            spriteBatch.DrawString(Content.Load<SpriteFont>("basic"), message, new Vector2(161, 161), Color.Black);
            spriteBatch.DrawString(Content.Load<SpriteFont>("basic"), message, new Vector2(160, 160), Color.White);

            spriteBatch.End();

            EndDraw();
        }

        bool justPressedFullscreenButton = false;
        bool justPressedPauseButton = false;
        bool justClickedOnscreenButton = false;
        bool justPressedControllerButton = false;

        public void SendInfo()
        {
            lock (GameServer.syncLock)
            {
                ServerInfo serverInfo = new ServerInfo();
                serverInfo.level = new OnlineLevelData();
                serverInfo.level.backgroundColor = currentLevel.backgroundColor;
                if (UpdateTextures)
                {
                    UpdateTextures = false;
                    serverInfo = UpdateServerTexturesWithoutLock(serverInfo);
                }
                //serverInfo = UpdateServerTexturesWithoutLock(serverInfo);
            }
        }

        public void UpdatePlayer()
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].Name == player.Name)
                {
                    players[i] = player;
                }
            }
        }

        private ServerInfo UpdateServerTexturesWithoutLock(ServerInfo serverInfo)
        {
            TextureData[] textures = new TextureData[0];

            for (int i = 0; i < currentLevel.solids.Length; i++)
            {
                bool newTexture = true;
                Solid solid = currentLevel.solids[i];
                solid.sprite.UpdateTextureData();
                foreach (Solid otherSolid in currentLevel.solids)
                {
                    otherSolid.sprite.UpdateTextureData();
                    if (solid.sprite.textureData == otherSolid.sprite.textureData)
                    {
                        newTexture = false;
                    }
                }
                if (newTexture)
                {
                    TextureData[] newTextures = new TextureData[textures.Length + 1];
                    for (int i2 = 0; i2 < textures.Length; i2++)
                    {
                        newTextures[i2] = textures[i2];
                    }
                    newTextures[newTextures.Length - 1] = new TextureData(solid.sprite.textureData, solid.sprite.getTexture().Height, solid.sprite.getTexture().Width);
                }
            }
            serverInfo.level.textureData = textures;
            return serverInfo;
        }

        public ServerInfo UpdateServerTextures(ServerInfo serverInfo)
        {
            TextureData[] textures = new TextureData[0];

            for (int i = 0; i < currentLevel.solids.Length; i++)
            {
                bool newTexture = true;
                Solid solid = currentLevel.solids[i];
                solid.sprite.UpdateTextureData();
                foreach (Solid otherSolid in currentLevel.solids)
                {
                    otherSolid.sprite.UpdateTextureData();
                    if(solid.sprite.textureData == otherSolid.sprite.textureData)
                    {
                        newTexture = false;
                    }
                }
                if (newTexture)
                {
                    TextureData[] newTextures = new TextureData[textures.Length + 1];
                    for (int i2 = 0; i2 < textures.Length; i2++)
                    {
                        newTextures[i2] = textures[i2];
                    }
                    newTextures[newTextures.Length - 1] = new TextureData(solid.sprite.textureData, solid.sprite.getTexture().Height, solid.sprite.getTexture().Width);
                }
            }
            serverInfo.level.textureData = textures;
            return serverInfo;
        }

        public void RecieveInfo(ServerInfo serverInfo)
        {
            currentLevel.backgroundColor = serverInfo.level.backgroundColor;
            Color[] bgdata = new Color[currentLevel.background.Width * currentLevel.background.Height];
            currentLevel.background.GetData(bgdata);
            if (bgdata != serverInfo.level.textureData[serverInfo.level.BackgroundReference].Colors)
            {
                currentLevel.background = new Texture2D(graphics.GraphicsDevice, serverInfo.level.textureData[serverInfo.level.BackgroundReference].Width, serverInfo.level.textureData[serverInfo.level.BackgroundReference].Height);
                currentLevel.background.SetData(serverInfo.level.textureData[serverInfo.level.BackgroundReference].Colors);
            }
            currentLevel.basicWeaponBullets = new BasicWeaponBullet[serverInfo.level.bullets.Length];
            for (int i = 0; i < serverInfo.level.bullets.Length; i++)
            {
                currentLevel.basicWeaponBullets[i] = new BasicWeaponBullet(Content, serverInfo.level.bullets[i].position, serverInfo.level.bullets[i].rotation);
            }
            currentLevel.defaultSpawnTimer = serverInfo.level.defaultSpawnTimer;
            currentLevel.levelHeight = serverInfo.level.levelHeight;
            currentLevel.levelWidth = serverInfo.level.levelWidth;
            currentLevel.maxAmountOfZombies = serverInfo.level.maxAmountOfZombies;
            players = new Player[serverInfo.level.players.Length];
            for (int i2 = 0; i2 < serverInfo.level.players.Length; i2++)
            {
                players[i2] = new Player();
                players[i2].delay = serverInfo.level.players[i2].delay;
                players[i2].delayUntilHit = serverInfo.level.players[i2].delayUntilHit;
                players[i2].hammer.Attacking = serverInfo.level.players[i2].hammerAttacking;
                players[i2].health = serverInfo.level.players[i2].health;
                players[i2].isDead = serverInfo.level.players[i2].isDead;
                players[i2].lazerSword.Attacking = serverInfo.level.players[i2].swordAttacking;
                players[i2].playerRotation = serverInfo.level.players[i2].playerRotation;
                players[i2].score = serverInfo.level.players[i2].score;
                players[i2].sprite.vector = serverInfo.level.players[i2].position;
                players[i2].weapon = (Player.Weapon)serverInfo.level.players[i2].weapon;
            }
            currentLevel.playerSpawn = serverInfo.level.playerSpawn;
            currentLevel.random = serverInfo.level.random;
            Rocket[] newRockets = new Rocket[serverInfo.level.rockets.Length];
            for (int i3 = 0; i3 < currentLevel.rockets.Length; i3++)
            {
                newRockets[i3] = new Rocket(Content, serverInfo.level.rockets[i3].position, serverInfo.level.rockets[i3].rotation);
            }
            currentLevel.rockets = newRockets;
            currentLevel.solids = new Solid[serverInfo.level.solidData.Length];
            for (int i4 = 0; i4 < serverInfo.level.solidData.Length; i4++)
            {
                Texture2D texture = new Texture2D(graphics.GraphicsDevice, serverInfo.level.textureData[serverInfo.level.solidData[i4].textureNo].Width, serverInfo.level.textureData[serverInfo.level.solidData[i4].textureNo].Height);
                texture.SetData(serverInfo.level.textureData[serverInfo.level.solidData[i4].textureNo].Colors);
                currentLevel.solids[i4] = new Solid(new Sprite(texture, serverInfo.level.solidData[i4].position, serverInfo.level.solidData[i4].tint));
            }
            currentLevel.spawnTimer = serverInfo.level.spawnTimer;
            currentLevel.timesSpawned = serverInfo.level.timesSpawned;
            currentLevel.zombieSpawnAcceleration = serverInfo.level.zombieSpawnAcceleration;
            currentLevel.zombieSpawners = serverInfo.level.zombieSpawners;
        }

        public void RecieveInfo(ClientInfo clientInfo)
        {
            BasicWeaponBullet[] newBullets  = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + clientInfo.newBullets.Length];
            int i;
            for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
            {
                newBullets[i] = currentLevel.basicWeaponBullets[i];
            }
            for (int i2 = 0; i2 < clientInfo.newBullets.Length; i2++)
            {
                newBullets[i + i2] = new BasicWeaponBullet(Content, clientInfo.newBullets[i2].position, clientInfo.newBullets[i2].rotation);
            }
            currentLevel.basicWeaponBullets = newBullets;
            Rocket[] newRockets = new Rocket[currentLevel.rockets.Length + clientInfo.newRockets.Length];
            int i3;
            for (i3 = 0; i3 < currentLevel.rockets.Length; i3++)
            {
                newRockets[i3] = currentLevel.rockets[i3];
            }
            for (int i4 = 0; i4 < clientInfo.newRockets.Length; i4++)
            {
                newRockets[i3 + i4] = new Rocket(Content, clientInfo.newRockets[i4].position, clientInfo.newRockets[i4].rotation);
            }
            currentLevel.rockets = newRockets;
            for (int i5 = 0; i5 < players.Length; i5++)
            {
                if (players[i5].Name == clientInfo.player.name)
                {
                    players[i5].delay = clientInfo.player.delay;
                    players[i5].delayUntilHit = clientInfo.player.delayUntilHit;
                    players[i5].hammer.Attacking = clientInfo.player.hammerAttacking;
                    players[i5].health = clientInfo.player.health;
                    players[i5].isDead = clientInfo.player.isDead;
                    players[i5].lazerSword.Attacking = clientInfo.player.swordAttacking;
                    players[i5].playerRotation = clientInfo.player.playerRotation;
                    players[i5].score = clientInfo.player.score;
                    players[i5].sprite.vector = clientInfo.player.position;
                    players[i5].weapon = (Player.Weapon)clientInfo.player.weapon;
                }
            }
        }

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

            if (server)
            {
                //GameServer.Update();
            }

            UpdatePlayer();

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
            else if (CurrentGameState == GameState.LOSE)
            {
                UpdatePlayer();
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
                    player.camera.UpdateWithRotation(player.playerRotation);
                }
                else
                {
                    player.camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                }
            }
            else if (CurrentGameState == GameState.MAINMENU)
            {
                IsMouseVisible = true;
                GameServer.Stop = true;
                GameClient.Stop = true;
                if ((buttonPlay.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonPlay.clicked = false;
                    justClickedOnscreenButton = true;

                    server = true;

                    DrawMessage("Creating session...");

                    string name = Microsoft.VisualBasic.Interaction.InputBox("Choose Name", "Name", "player");
                    foreach (Player player in players)
                    {
                        if (player.Name == this.player.Name)
                        {
                            player.Name = name;
                        }
                    }
                    this.player = new Player(new Sprite(Content.Load<Texture2D>("player"), currentLevel.playerSpawn + new Vector2(Content.Load<Texture2D>("Player").Width, Content.Load<Texture2D>("Player").Height), Color.Blue), 0f, Content, graphics.GraphicsDevice, name);
                    UpdatePlayer();

                    string joinIP = Microsoft.VisualBasic.Interaction.InputBox("Enter IP:", "Create Session", "localhost");
                    GameServer.ip = joinIP;
                    GameServer.game = this;

                    GameServer.Stop = false;

                    UpdateTextures = true;
                    CurrentGameState = GameState.PLAYING;

                    GameServer.StartListening();
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

                    server = false;

                    DrawMessage("Joining session...");

                    string name = Microsoft.VisualBasic.Interaction.InputBox("Choose Name", "Name", "player");
                    player = new Player(new Sprite(Content.Load<Texture2D>("Player"), currentLevel.playerSpawn + new Vector2(Content.Load<Texture2D>("Player").Width, Content.Load<Texture2D>("Player").Height), Color.Blue), 0f, Content, graphics.GraphicsDevice, name);
                    UpdatePlayer();

                    string joinIP = Microsoft.VisualBasic.Interaction.InputBox("Enter IP:", "Join Session", "localhost");
                    GameClient.ip = joinIP;
                    GameClient.Stop = false;
                    GameClient.game = this;
                    CurrentGameState = GameState.PLAYING;
                    GameClient.StartClient();
                }
                else if ((buttonController.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed) && !justClickedOnscreenButton && !justPressedControllerButton)
                {
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
                }
                else if ((buttonQuit.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    justClickedOnscreenButton = true;
                    this.Exit();
                }
                else if ((buttonLoadLevel.clicked == true) && !justClickedOnscreenButton)
                {
                    
                    LevelData levelData = LevelData.LoadLevel();
                    if (levelData != null)
                    {
                        currentLevel = new Level(levelData, graphics.GraphicsDevice);
                        player.sprite.vector = currentLevel.playerSpawn + new Vector2(player.sprite.getTexture().Width / 2, player.sprite.getTexture().Height / 2);
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
                    GameServer.serverInfo = UpdateServerTextures(GameServer.serverInfo);
                    
                }
                else
                {
                    buttonPlay.Update(Mouse.GetState());
                    button2PlayerSplitscreen.Update(Mouse.GetState());
                    buttonQuit.Update(Mouse.GetState());
                    buttonController.Update(Mouse.GetState());
                    buttonLoadLevel.Update(Mouse.GetState());
                }
            }
            else if (CurrentGameState == GameState.PAUSE)
            {
                UpdatePlayer();
                IsMouseVisible = true;

                if ((Keyboard.GetState().IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed) && justPressedPauseButton == false)
                {
                    justPressedPauseButton = true;
                    CurrentGameState = GameState.PLAYING;
                }
                else if (justPressedPauseButton == true && (Keyboard.GetState().IsKeyUp(Keys.Escape) && GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Released))
                {
                    justPressedPauseButton = false;
                }

                if ((buttonResume.clicked == true || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) && !justClickedOnscreenButton)
                {
                    buttonResume.clicked = false;
                    justClickedOnscreenButton = true;
                    CurrentGameState = GameState.PLAYING;
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
            else if (CurrentGameState == GameState.PLAYING)
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
                if (player.delayUntilHit > 0)
                {
                    player.delayUntilHit -= 1;
                }
                if (options.usingController)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        player.camera.setZoom(player.camera.getZoom() / 1.1f);
                    }
                    else if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        player.camera.setZoom(player.camera.getZoom() * 1.1f);
                    }
                    IsMouseVisible = false;
                    if (player.delay > 0)
                    {
                        player.delay -= 1;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Triggers.Left > 0)
                    {
                        player.aiming = true;
                    }
                    else
                    {
                        player.aiming = false;
                    }
                    if (GamePad.GetState(PlayerIndex.One).Triggers.Right > 0)
                    {
                        if (player.weapon == Player.Weapon.BASIC && player.delay <= 0)
                        {
                            if (currentLevel.basicWeaponBullets.Length != 0)
                            {
                                /*
                                List<BasicWeaponBullet> newBullets = new List<BasicWeaponBullet>();
                                foreach (BasicWeaponBullet bullet in currentLevel.basicWeaponBullets)
                                {
                                    newBullets.Add(bullet);
                                }
                                newBullets.Add(new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation));
                                currentLevel.basicWeaponBullets = newBullets.ToArray();
                                */
                                BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBullets[i] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);
                                currentLevel.basicWeaponBullets = newBullets;

                                BasicWeaponBulletData[] newInfoBullets = new BasicWeaponBulletData[clientInfo.newBullets.Length + 1];
                                for (int i2 = 0; i2 < clientInfo.newBullets.Length; i2++)
                                {
                                    newInfoBullets[i2] = clientInfo.newBullets[i2];
                                }
                                BasicWeaponBulletData newBulletData = new BasicWeaponBulletData();
                                newBulletData.position = newBullets[i].sprite.vector;
                                newBulletData.rotation = newBullets[i].rotation;
                                newInfoBullets[newInfoBullets.Length - 1] = newBulletData;
                                clientInfo.newBullets = newInfoBullets;
                                /*
                                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i = 0;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBasicWeaponBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBasicWeaponBullets[newBasicWeaponBullets.Length - 1] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);
                                */
                                player.delay = 20;
                            }
                            else
                            {
                                currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);
                                player.delay = 20;

                                BasicWeaponBulletData[] newInfoBullets = new BasicWeaponBulletData[clientInfo.newBullets.Length + 1];
                                for (int i2 = 0; i2 < clientInfo.newBullets.Length; i2++)
                                {
                                    newInfoBullets[i2] = clientInfo.newBullets[i2];
                                }
                                BasicWeaponBulletData newBulletData = new BasicWeaponBulletData();
                                newBulletData.position = currentLevel.basicWeaponBullets[0].sprite.vector;
                                newBulletData.rotation = currentLevel.basicWeaponBullets[0].rotation;
                                newInfoBullets[newInfoBullets.Length - 1] = newBulletData;
                                clientInfo.newBullets = newInfoBullets;
                            }
                        }
                        else if (player.weapon == Player.Weapon.LAZERSWORD && player.delay <= 0)
                        {
                            if (player.lazerSword.Attacking == 0)
                            {
                                player.lazerSword.Attacking = 30;
                                player.delay = 70;
                            }
                        }
                        else if (player.weapon == Player.Weapon.HAMMER && player.delay <= 0)
                        {
                            if (player.hammer.Attacking == 0)
                            {
                                player.hammer.Attacking = 20;
                                player.delay = 120;
                            }
                        }
                        else if (player.weapon == Player.Weapon.ROCKETLAUNCHER && player.delay <= 0)
                        {
                            player.rocketLauncher.Attack(Content, currentLevel, player, clientInfo);
                            player.delay = 150;
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        if (player.aiming)
                        {
                            Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f))));
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(player.playerRotation), (float)Math.Sin(player.playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(player.playerRotation) - positionChange.Y * (float)Math.Sin(player.playerRotation), positionChange.X * (float)Math.Sin(player.playerRotation) + positionChange.Y * (float)Math.Cos(player.playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(player.playerRotation), Vector2.Zero);
                            Vector2 newPos = RotateVector2(player.sprite.vector + positionChange, player.playerRotation, player.sprite.vector);
                            player.SetX(newPos.X, currentLevel);
                            player.SetY(newPos.Y, currentLevel);
                        }
                        else
                        {
                            Vector2 positionChange = new Vector2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, -(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7));
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(player.playerRotation), (float)Math.Sin(player.playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(player.playerRotation) - positionChange.Y * (float)Math.Sin(player.playerRotation), positionChange.X * (float)Math.Sin(player.playerRotation) + positionChange.Y * (float)Math.Cos(player.playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(player.playerRotation), Vector2.Zero);
                            Vector2 newPos = RotateVector2(player.sprite.vector + positionChange, player.playerRotation, player.sprite.vector);
                            player.SetX(newPos.X, currentLevel);
                            player.SetY(newPos.Y, currentLevel);
                        }
                    }
                    else
                    {
                        if (player.aiming)
                        {
                            player.SetX(player.sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                            player.SetY(player.sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * (5 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 5f)), currentLevel);
                        }
                        else
                        {
                            player.SetX(player.sprite.getX() + GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.X * 7, currentLevel);
                            player.SetY(player.sprite.getY() - GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Left.Y * 7, currentLevel);
                        }
                    }
                    if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                    {
                        if (options.player1CameraRotation)
                        {
                            player.playerRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                        }
                        else
                        {
                            player.playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        player.camera.UpdateWithRotation(player.playerRotation);
                    }
                    else
                    {
                        player.camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    }
                    if (!options.player1CameraRotation)
                    {
                        Vector2 newVector = player.camera.getPosition();
                        if (player.aiming)
                        {
                            newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                            newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                        }
                        else
                        {
                            newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                            newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                        }
                        player.camera.setPosition(newVector, currentLevel, new Vector2(screenWidth / 2, screenHeight));
                    }
                }
                else
                {
                    if (oldMouse.ScrollWheelValue > Mouse.GetState().ScrollWheelValue)
                    {
                        player.camera.setZoom(player.camera.getZoom() / 1.1f);
                    }
                    else if (oldMouse.ScrollWheelValue < Mouse.GetState().ScrollWheelValue)
                    {
                        player.camera.setZoom(player.camera.getZoom() * 1.1f);
                    }
                    oldMouse = Mouse.GetState();
                    IsMouseVisible = false;
                    if (player.delay > 0)
                    {
                        player.delay -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) || Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        player.aiming = true;
                    }
                    else
                    {
                        player.aiming = false;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                    {
                        if (player.weapon == Player.Weapon.BASIC && player.delay <= 0)
                        {
                            if (currentLevel.basicWeaponBullets.Length != 0)
                            {
                                BasicWeaponBullet[] newBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBullets[i] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);
                                currentLevel.basicWeaponBullets = newBullets;

                                BasicWeaponBulletData[] newInfoBullets = new BasicWeaponBulletData[clientInfo.newBullets.Length + 1];
                                for (int i2 = 0; i2 < clientInfo.newBullets.Length; i2++)
                                {
                                    newInfoBullets[i2] = clientInfo.newBullets[i2];
                                }
                                BasicWeaponBulletData newBulletData = new BasicWeaponBulletData();
                                newBulletData.position = newBullets[i].sprite.vector;
                                newBulletData.rotation = newBullets[i].rotation;
                                newInfoBullets[newInfoBullets.Length - 1] = newBulletData;
                                clientInfo.newBullets = newInfoBullets;

                                /*
                                BasicWeaponBullet[] newBasicWeaponBullets = new BasicWeaponBullet[currentLevel.basicWeaponBullets.Length + 1];
                                int i = 0;
                                for (i = 0; i < currentLevel.basicWeaponBullets.Length; i++)
                                {
                                    newBasicWeaponBullets[i] = currentLevel.basicWeaponBullets[i];
                                }
                                newBasicWeaponBullets[newBasicWeaponBullets.Length - 1] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);
                                */
                                player.delay = 20;
                            }
                            else
                            {
                                currentLevel.basicWeaponBullets = new BasicWeaponBullet[1];
                                currentLevel.basicWeaponBullets[0] = new BasicWeaponBullet(Content, player.sprite.vector, player.playerRotation);

                                BasicWeaponBulletData[] newInfoBullets = new BasicWeaponBulletData[clientInfo.newBullets.Length + 1];
                                for (int i2 = 0; i2 < clientInfo.newBullets.Length; i2++)
                                {
                                    newInfoBullets[i2] = clientInfo.newBullets[i2];
                                }
                                BasicWeaponBulletData newBulletData = new BasicWeaponBulletData();
                                newBulletData.position = currentLevel.basicWeaponBullets[0].sprite.vector;
                                newBulletData.rotation = currentLevel.basicWeaponBullets[0].rotation;
                                newInfoBullets[newInfoBullets.Length - 1] = newBulletData;
                                clientInfo.newBullets = newInfoBullets;

                                player.delay = 20;
                            }
                        }
                        else if (player.weapon == Player.Weapon.LAZERSWORD && player.delay <= 0)
                        {
                            if (player.lazerSword.Attacking == 0)
                            {
                                player.lazerSword.Attacking = 30;
                                player.delay = 70;
                            }
                        }
                        else if (player.weapon == Player.Weapon.HAMMER && player.delay <= 0)
                        {
                            if (player.hammer.Attacking == 0)
                            {
                                player.hammer.Attacking = 20;
                                player.delay = 120;
                            }
                        }
                        else if (player.weapon == Player.Weapon.ROCKETLAUNCHER && player.delay <= 0)
                        {
                            player.rocketLauncher.Attack(Content, currentLevel, player, clientInfo);
                            player.delay = 150;
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
                        if (player.aiming)
                        {
                            Vector2 positionChange = new Vector2(0, 0);
                            if (Keyboard.GetState().IsKeyDown(Keys.W))
                            {
                                //movementY -= 1;
                                positionChange.Y -= 5 * (1 / 5f);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.A))
                            {
                                //movementX -= 1;
                                positionChange.X -= 5 * (1 / 5f);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.S))
                            {
                                //movementY += 1;
                                positionChange.Y += 5 * (1 / 5f);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D))
                            {
                                //movementX += 1;
                                positionChange.X += 5 * (1 / 5f);
                            }
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(player.playerRotation), (float)Math.Sin(player.playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(player.playerRotation) - positionChange.Y * (float)Math.Sin(player.playerRotation), positionChange.X * (float)Math.Sin(player.playerRotation) + positionChange.Y * (float)Math.Cos(player.playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(player.playerRotation), Vector2.Zero);
                            Vector2 newPos = RotateVector2(player.sprite.vector + positionChange, player.playerRotation, player.sprite.vector);
                            player.SetX(newPos.X, currentLevel);
                            player.SetY(newPos.Y, currentLevel);
                        }
                        else
                        {
                            Vector2 positionChange = new Vector2(0, 0);
                            if (Keyboard.GetState().IsKeyDown(Keys.W))
                            {
                                //movementY -= 1;
                                positionChange.Y -= 7;
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.A))
                            {
                                //movementX -= 1;
                                positionChange.X -= 7;
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.S))
                            {
                                //movementY += 1;
                                positionChange.Y += 7;
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D))
                            {
                                //movementX += 1;
                                positionChange.X += 7;
                            }
                            //Vector2 rotationChange = new Vector2((float)Math.Cos(player.playerRotation), (float)Math.Sin(player.playerRotation));
                            //positionChange = new Vector2(positionChange.X * (float)Math.Cos(player.playerRotation) - positionChange.Y * (float)Math.Sin(player.playerRotation), positionChange.X * (float)Math.Sin(player.playerRotation) + positionChange.Y * (float)Math.Cos(player.playerRotation));
                            //Vector2 newPositionChange = RotateVector2(positionChange, MathHelper.ToRadians(player.playerRotation), Vector2.Zero);
                            Vector2 newPos = RotateVector2(player.sprite.vector + positionChange, player.playerRotation, player.sprite.vector);
                            player.SetX(newPos.X, currentLevel);
                            player.SetY(newPos.Y, currentLevel);
                        }
                    }
                    else
                    {
                        if (player.aiming)
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.W))
                            {
                                //movementY -= 1;
                                player.SetY(player.sprite.getY() + -1 * (5 * (1 / 5f)), currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.A))
                            {
                                //movementX -= 1;
                                player.SetX(player.sprite.getX() + -1 * (5 * (1 / 5f)), currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.S))
                            {
                                //movementY += 1;
                                player.SetY(player.sprite.getY() + 1 * (5 * (1 / 5f)), currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D))
                            {
                                //movementX += 1;
                                player.SetX(player.sprite.getX() + 1 * (5 * (1 / 5f)), currentLevel);
                            }
                        }
                        else
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.W))
                            {
                                //movementY -= 1;
                                player.SetY(player.sprite.getY() + -1 * 7, currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.A))
                            {
                                //movementX -= 1;
                                player.SetX(player.sprite.getX() + -1 * 7, currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.S))
                            {
                                //movementY += 1;
                                player.SetY(player.sprite.getY() + 1 * 7, currentLevel);
                            }
                            if (Keyboard.GetState().IsKeyDown(Keys.D))
                            {
                                //movementX += 1;
                                player.SetX(player.sprite.getX() + 1 * 7, currentLevel);
                            }
                        }
                    }
                    //float movementRotation = (float)Math.Atan2(movementX, -movementY);
                    /*
                    if (player.aiming)
                    {
                        player.SetX(player.sprite.getX() - (float)((5 * (1 / 5f)) * Math.Cos(movementRotation)), currentLevel);
                        player.SetY(player.sprite.getY() - (float)((5 * (1 / 5f)) * Math.Sin(movementRotation)), currentLevel);
                    }
                    else
                    {
                        player.SetX(player.sprite.getX() - (float)(5 * Math.Cos(movementRotation)), currentLevel);
                        player.SetX(player.sprite.getX() - (float)(5 * Math.Sin(movementRotation)), currentLevel);
                    }
                     */
                    /*
                    if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right != Vector2.Zero)
                    {
                        if (options.player1CameraRotation)
                        {
                            player.playerRotation += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * options.player1CameraRotationSpeed;
                        }
                        else
                        {
                            player.playerRotation = (float)Math.Atan2(GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X, GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y);
                        }
                    }
                    if (options.player1CameraRotation)
                    {
                        player.camera.UpdateWithRotation(player.playerRotation);
                    }
                    else
                    {
                        player.camera.Update2Player(currentLevel, new Vector2(screenWidth, screenHeight));
                    }
                    if (!options.player1CameraRotation)
                    {
                        Vector2 newVector = player.camera.getPosition();
                        if (player.aiming)
                        {
                            newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                            newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * (150 * (GamePad.GetState(PlayerIndex.One).Triggers.Left / 2.1f));
                        }
                        else
                        {
                            newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                            newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                        }
                        player.camera.setPosition(newVector, currentLevel, new Vector2(screenWidth / 2, screenHeight));
                    }
                    */
                    Vector2 distance;
                    if (options.player1CameraRotation)
                    {
                        distance = new Vector2(Mouse.GetState().X - screenWidth / 2, Mouse.GetState().Y - screenHeight / 2);
                    }
                    else
                    {
                        distance = new Vector2((Mouse.GetState().X + (player.camera.getPosition().X - screenWidth / 2)) - player.sprite.getX(), (Mouse.GetState().Y + (player.camera.getPosition().Y - screenHeight / 2)) - player.sprite.getY());
                    }
                    if (options.player1CameraRotation)
                    {
                        player.playerRotation += distance.X * options.player1CameraRotationSpeed / 3;
                    }
                    else
                    {
                        player.playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    }
                    //player.playerRotation = (float)Math.Atan2(distance.Y, distance.X) + 165;
                    if (options.player1CameraRotation)
                    {
                        player.camera.UpdateWithRotation(player.playerRotation);
                    }
                    else
                    {
                        player.camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    }
                    //player.camera.Update(currentLevel, new Vector2(screenWidth, screenHeight));
                    Vector2 newVector = player.camera.getPosition();
                    /*
                    if (player.aiming)
                    {
                        newVector.X += (Mouse.GetState().X + (player.camera.getPosition().X - screenWidth / 2)) - player.sprite.getX();
                        newVector.Y += (Mouse.GetState().Y + (player.camera.getPosition().Y - screenWidth / 2)) - player.sprite.getY();
                    }
                    else
                    {
                        //newVector.X += ((Mouse.GetState().X + (player.camera.getPosition().X - screenWidth / 2)) - player.sprite.getX()) / 7;
                        //newVector.Y += ((Mouse.GetState().Y + (player.camera.getPosition().Y - screenWidth / 2)) - player.sprite.getY()) / 7;
                        //newVector.X += GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.X * 7;
                        //newVector.Y -= GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular).ThumbSticks.Right.Y * 7;
                    }
                    */
                    if (!options.player1CameraRotation)
                    {
                        player.camera.setPosition(newVector, currentLevel, new Vector2(screenWidth, screenHeight));
                    }

                    if (options.player1CameraRotation)
                    {
                        Mouse.SetPosition(screenWidth / 2, screenHeight / 2);
                    }
                }
                currentLevel.Update(players, Content, player);
                if (player.health <= 0)
                {
                    CurrentGameState = GameState.LOSE;
                    SaveHighScore();
                }
                if (player.weapon == Player.Weapon.LAZERSWORD)
                {
                    player.lazerSword.Update(Content, currentLevel);
                }
                if (player.weapon == Player.Weapon.HAMMER)
                {
                    player.hammer.Update(Content, currentLevel);
                }
                if (player.weapon == Player.Weapon.ROCKETLAUNCHER)
                {
                    player.rocketLauncher.Update(Content, currentLevel);
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
                Vector2 newVector = player.camera.getPosition();
                newVector.X += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * 7;
                newVector.Y -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * 7;
                player.camera.setPosition(newVector);
            }
            if (GamePad.GetState(PlayerIndex.One).Triggers.Left != 0f)
            {
                player.camera.setZoom(player.camera.getZoom() - (GamePad.GetState(PlayerIndex.One).Triggers.Left / 3f));
            }
            if (GamePad.GetState(PlayerIndex.One).Triggers.Right != 0f)
            {
                player.camera.setZoom(player.camera.getZoom() + (GamePad.GetState(PlayerIndex.One).Triggers.Right / 3f));
            }
            */

            // TODO: Add your update logic here

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
                if (player.score > data.Score[i])
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
                data.Score[scoreIndex] = player.score;

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
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.camera.get_transformation(graphics.GraphicsDevice));
            }
            else if (CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE || CurrentGameState == GameState.PLAYING)
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
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.DrawWithoutHealth(spriteBatch, Content);
                    spriteBatch.Draw(Content.Load<Texture2D>("GraveStone"), player.sprite.vector - new Vector2(player.sprite.getTexture().Width / 2, player.sprite.getTexture().Height / 2), Color.White);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, player);
                }
                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                spriteBatch.Draw(Content.Load<Texture2D>("LoseBackground"), new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
                spriteBatch.Draw(buttonMainMenu.sprite.getTexture(), new Vector2(buttonMainMenu.sprite.vector.X + (0), buttonMainMenu.sprite.vector.Y), buttonMainMenu.sprite.getTint());
                spriteBatch.Draw(Content.Load<Texture2D>("BButton"), new Vector2((buttonMainMenu.sprite.vector.X + buttonMainMenu.sprite.getTexture().Width + Content.Load<Texture2D>("BButton").Width / 2) + (0), (buttonMainMenu.sprite.vector.Y + Content.Load<Texture2D>("BButton").Height / 2) + (0)), Color.White);
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                string scoreToBeat = HighScores.LoadHighScores(highscoresFilename).Score[0].ToString();
                String youScored = "You scored " + player.score.ToString() + "\nScore to beat:" + scoreToBeat;
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
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.camera.get_transformation(graphics.GraphicsDevice));
                    currentLevel.Draw(spriteBatch, Content, player);
                    player.Draw(spriteBatch, Content);
                    spriteBatch.End();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, player);
                }

                GraphicsDevice.Viewport = overlayView;
                spriteBatch.Begin();
                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + player.health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(0 + Font.MeasureString(health).X, screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + player.score.ToString();
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
            else if (CurrentGameState == GameState.PLAYING)
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

                //player.camera.updateEffect(graphics.GraphicsDevice);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, player.camera.get_transformation(graphics.GraphicsDevice));
                if (!options.player13D)
                {
                    currentLevel.Draw(spriteBatch, Content, player);
                    foreach (Player CurrentPlayer in players)
                    {
                        CurrentPlayer.Draw(spriteBatch, Content);
                    }
                    //player.Draw(spriteBatch, Content);
                    spriteBatch.End();
                    GraphicsDevice.Viewport = overlayView;
                    spriteBatch.Begin();
                }
                else
                {
                    threeD.Draw(GraphicsDevice, currentLevel, players, false, 1, Content, player);
                }


                SpriteFont Font = Content.Load<SpriteFont>("basic");
                String health = "HP:" + player.health.ToString();
                Vector2 healthOrigin = Font.MeasureString(health) / 2;
                spriteBatch.DrawString(Font, health, new Vector2(Font.MeasureString(health).X, screenHeight - Font.MeasureString(health).Y), Color.Black, 0, healthOrigin, 2.0f, SpriteEffects.None, 0.5f);
                String score = "SCORE:" + player.score.ToString();
                Vector2 scoreOrigin = new Vector2(Font.MeasureString(score).X, 0);
                spriteBatch.DrawString(Font, score, new Vector2( screenWidth - screenWidth / 300, 0 + screenHeight / 300), Color.Black, 0, scoreOrigin, 2.0f, SpriteEffects.None, 0.5f);
                if (!options.usingController && !options.player1CameraRotation)
                {
                    spriteBatch.Draw(Content.Load<Texture2D>("Crosshair"), new Vector2(Mouse.GetState().X - (Content.Load<Texture2D>("Crosshair").Width / 2) + player.camera.getPosition().X - screenWidth / 2, Mouse.GetState().Y - (Content.Load<Texture2D>("Crosshair").Height / 2) + player.camera.getPosition().Y - screenHeight / 2), Color.White);
                }
                spriteBatch.End();
            }
            

            //TEST spriteBatch.Draw(testBallTexture, new Vector2(10f, 10f), Color.White);

            if (CurrentGameState == GameState.PLAYING || CurrentGameState == GameState.PAUSE || CurrentGameState == GameState.LOSE)
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
