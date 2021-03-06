﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TowerDefense
{
    /// <summary>
    /// This is the main game class
    /// </summary>
    public class TowerDefense : Game
    {
        //fields

        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D BG;
        private Texture2D MenuBG;
        private Texture2D howToPlay;
        private Texture2D CreditsBG;
        private Texture2D VictoryBG;
        private Texture2D DefeatBG;

        //Objecten
        private Player player = new Player();
        private Level level = new Level();
        private Wave_manager manager;
        private ArrowTower infoArrow;
        private FreezeTower infoFreeze;
        private CanonTower infoCanon;

        //buttons
        private Button btnMenuPlay;
        private Button btnMenuCredits;
        private Button btnMenuHowto;
        private Button btnBack;
        private Button btnNext;

        //tower buttons
        private Button btnArrow;        
        private Button btnFreeze;
        private Button btnCanon;

        //Strings
        private string towerType;

        //towerList
        private List<Tower> towerList;

        //Locatie
        private int cellX;
        private int cellY;

        private int tileX;
        private int tileY;

        //Bool
        private bool onPath;
        private bool arrow;
        private bool canon;
        private bool freeze;

        private int levelIndex = 0;

        //GameStates voor het spel
        enum GameState
        {
            MainMenu, Credits, Howto, Playing, EndGame, Pause
        }
        GameState CurrentGameState = GameState.MainMenu;

        public TowerDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            Vector2 newVector = new Vector2(0,0);
            infoArrow = new ArrowTower(null, null, newVector);
            infoFreeze = new FreezeTower(null, null, newVector);
            infoCanon = new CanonTower(null, null, newVector);
        }

        /// <summary>
        /// Initialize before game starts to run
        /// Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            batch = new SpriteBatch(GraphicsDevice);

            //load level sprites
            Texture2D grass = Content.Load<Texture2D>("grass");
            Texture2D road = Content.Load<Texture2D>("road");
            Texture2D turn = Content.Load<Texture2D>("turn");
            Texture2D end = Content.Load<Texture2D>("end");

            //add sprites for tiles to level and initialize level
            level.AddTexture(grass);
            level.AddTexture(road);
            level.AddTexture(turn);
            level.AddTexture(end);
            makeLevel();

            //load content for the game
            font = Content.Load<SpriteFont>("font");
            BG = Content.Load<Texture2D>("BG_ingame");
            MenuBG = Content.Load<Texture2D>("Menu Background");
            howToPlay = Content.Load<Texture2D>("howToPlay");
            CreditsBG = Content.Load<Texture2D>("CreditsBackground");
            VictoryBG = Content.Load<Texture2D>("victoryscreen");
            DefeatBG = Content.Load<Texture2D>("defeatscreen");

            //buttons for Main Menu
            btnMenuPlay = new Button(Content.Load<Texture2D>("Buttons/Play"), graphics.GraphicsDevice);
            btnMenuPlay.setPosition(new Vector2(550, 200));
            
            btnMenuHowto = new Button(Content.Load<Texture2D>("Buttons/Howto"), graphics.GraphicsDevice);
            btnMenuHowto.setPosition(new Vector2(550, 300));

            btnMenuCredits = new Button(Content.Load<Texture2D>("Buttons/Credits"), graphics.GraphicsDevice);
            btnMenuCredits.setPosition(new Vector2(550, 400));

            //Other menu buttons
            btnBack = new Button(Content.Load<Texture2D>("Buttons/Back"), graphics.GraphicsDevice);

            btnNext = new Button(Content.Load<Texture2D>("Buttons/Next"), graphics.GraphicsDevice);
            btnNext.setPosition(new Vector2(750, 500));

            //Tower buttons
            btnArrow = new Button(Content.Load<Texture2D>("arrowTowerFront"), graphics.GraphicsDevice);
            btnArrow.setPosition(new Vector2(250, 600));

            btnFreeze = new Button(Content.Load<Texture2D>("slowTowerFront"), graphics.GraphicsDevice);
            btnFreeze.setPosition(new Vector2(400, 600));

            btnCanon = new Button(Content.Load<Texture2D>("canonTowerFront"), graphics.GraphicsDevice);
            btnCanon.setPosition(new Vector2(650, 650));
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //muis en toetsenbord status
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();
            //check gameStates en update
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    IsMouseVisible = true;
                    if (btnMenuPlay.isClicked == true) CurrentGameState = GameState.Playing;
                    btnMenuPlay.Update(mouse);
                    if (btnMenuHowto.isClicked == true) CurrentGameState = GameState.Howto;
                    btnMenuHowto.Update(mouse);
                    if (btnMenuCredits.isClicked == true) CurrentGameState = GameState.Credits;
                    btnMenuCredits.Update(mouse);
                    break;
                case GameState.Howto:
                    if (btnBack.isClicked == true) CurrentGameState = GameState.MainMenu;
                    btnBack.Update(mouse);
                    break;
                case GameState.Credits:
                    if (btnBack.isClicked == true) CurrentGameState = GameState.MainMenu;
                    btnBack.Update(mouse);
                    break;
                case GameState.Playing:
                    if (keyboardState.IsKeyDown(Keys.Escape)) CurrentGameState = GameState.Pause;
                    foreach (Tower tower in towerList)
                    {
                        tower.Update(gameTime, manager);                       
                    }

                    if (btnArrow.isClicked) 
                    {
                        MouseState newState = Mouse.GetState();
                        if (newState.LeftButton == ButtonState.Released)
                        {
                            arrow = true;                            
                            freeze = false;
                            canon = false;
                        }
                    }
                    else if (btnFreeze.isClicked)
                    {
                        MouseState newState = Mouse.GetState();
                        if (newState.LeftButton == ButtonState.Released)
                        {
                            arrow = false;                            
                            freeze = true;
                            canon = false;
                        }
                    }
                    else if (btnCanon.isClicked)
                    {
                        MouseState newState = Mouse.GetState();
                        if (newState.LeftButton == ButtonState.Released)
                        {
                            arrow = false;                           
                            freeze = false;
                            canon = true;
                        }
                    }
                    

                    if (arrow)
                    {
                        MouseState nextState = Mouse.GetState();
                        if (nextState.LeftButton == ButtonState.Pressed)
                        {
                            arrow = false;
                            cellX = nextState.X / 50;
                            cellY = nextState.Y / 50;
                            tileX = cellX * 50;
                            tileY = cellY * 50;
                            towerType = "arrowTower";
                            newTower(tileX, tileY);
                            Draw(gameTime);            
                        }
                        else if (nextState.RightButton == ButtonState.Pressed)
                        {
                            arrow = false;
                        }
                    }
                    else if (freeze)
                    {
                        MouseState nextState = Mouse.GetState();
                        if (nextState.LeftButton == ButtonState.Pressed)
                        {
                            freeze = false;
                            cellX = nextState.X / 50;
                            cellY = nextState.Y / 50;
                            tileX = cellX * 50;
                            tileY = cellY * 50;
                            towerType = "freezeTower";
                            newTower(tileX, tileY);
                            Draw(gameTime);                            
                        }
                        else if (nextState.RightButton == ButtonState.Pressed)
                        {
                            freeze = false;
                        }
                    }
                    else if (canon)
                    {
                        MouseState nextState = Mouse.GetState();
                        if (nextState.LeftButton == ButtonState.Pressed)
                        {
                            canon = false;
                            cellX = nextState.X / 50;
                            cellY = nextState.Y / 50;
                            tileX = cellX * 50;
                            tileY = cellY * 50;
                            towerType = "canonTower";
                            newTower(tileX, tileY);
                            Draw(gameTime);                            
                        }
                        else if (nextState.RightButton == ButtonState.Pressed)
                        {
                            canon = false;
                        }
                    }

                    //Update de tower buttons
                    btnArrow.Update(mouse);
                    btnFreeze.Update(mouse);
                    btnCanon.Update(mouse);

                    //check player's levens
                    if (player.lives <= 0)
                    {
                        CurrentGameState = GameState.EndGame;
                    }

                    //check of level klaar is
                    if(manager.isFinished())
                    {
                        CurrentGameState = GameState.EndGame;
                    }

                    manager.Update(gameTime);
                    break;
                case GameState.Pause:
                    if (btnMenuPlay.isClicked == true) CurrentGameState = GameState.Playing;
                    btnMenuPlay.Update(mouse);
                    break;
                case GameState.EndGame:
                    //level klaar & player leeft nog
                    if (manager.isFinished() && player.lives > 0)
                    {
                        if (levelIndex >= 1)
                        {
                            levelIndex = 0;
                            makeLevel();
                        }
                        else
                        {
                            levelIndex = 1;
                            makeLevel();
                        }
                    }
                    //player leeft niet meer
                    else if (player.lives <= 0)
                    {
                        levelIndex = 0;
                        makeLevel();
                    }
                    //update buttons
                    if (btnBack.isClicked == true) CurrentGameState = GameState.MainMenu;
                    btnBack.Update(mouse);
                    if (btnNext.isClicked == true) CurrentGameState = GameState.Playing;
                    btnNext.Update(mouse);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// method om een nieuw level te maken.
        /// </summary>
        private void makeLevel()
        {
            level.setLevel(levelIndex);
            manager = new Wave_manager(level.getWaypoints(), levelIndex, player);
            towerList = new List<Tower>();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Begin the drawing
            batch.Begin();

            //gamestates
            switch (CurrentGameState)
            {
                //Draw BackGrounds and buttons for each gamestate
                case GameState.MainMenu:
                    batch.Draw(MenuBG, new Rectangle(0, 0, 1200, 750), Color.White);
                    btnMenuPlay.Draw(batch);
                    btnMenuHowto.Draw(batch);
                    btnMenuCredits.Draw(batch);
                    break;
                case GameState.Howto:
                    batch.Draw(howToPlay, new Rectangle(0, 0, 1200, 750), Color.White);
                    btnBack.Draw(batch);
                    btnBack.setPosition(new Vector2(1000, 400));
                    break;
                case GameState.Credits:
                    batch.Draw(CreditsBG, new Rectangle(0, 0, 1200, 750), Color.White);
                    btnBack.Draw(batch);
                    btnBack.setPosition(new Vector2(550, 500));
                    break;
                case GameState.Playing:
                    level.Draw(batch);
                    //draw enemies from wave manager > wave
                    manager.Draw(batch, Content);
                    //onderste gedeelte op scherm
                    batch.Draw(BG, new Rectangle(0, 550, 1200, 200), Color.White);
                    batch.DrawString(font, "Level: " + (levelIndex + 1), new Vector2(level.Width, level.Height + 550), Color.Black);
                    batch.DrawString(font, "Total Waves: " + manager.numberOfWaves, new Vector2(level.Width, level.Height + 570), Color.Black);
                    batch.DrawString(font, "Currentwave: " + manager.currentWave, new Vector2(level.Width, level.Height + 590), Color.Black);
                    batch.DrawString(font, "Enemies: " + manager.enemies.Count, new Vector2(level.Width, level.Height + 610), Color.Black);
                    batch.DrawString(font, "Lives: " + player.lives, new Vector2(level.Width, level.Height + 650), Color.Black);
                    batch.DrawString(font, "Gold: " + player.money, new Vector2(level.Width, level.Height + 670), Color.Black);

                    batch.DrawString(font, "Towers: ", new Vector2(level.Width + 225, level.Height + 550), Color.Black);

                    if (arrow)
                    {
                        batch.DrawString(font, "ArrowTower", new Vector2(level.Width + 900, level.Height + 550), Color.Black);
                        batch.DrawString(font, "Cost " + infoArrow.getCost().ToString(), new Vector2(level.Width + 900, level.Height + 570), Color.Black);
                        batch.DrawString(font, "Damage " + infoArrow.getDamage().ToString(), new Vector2(level.Width + 900, level.Height + 590), Color.Black);
                        batch.DrawString(font, "Attack speed " + infoArrow.getAttackSpeed().ToString(), new Vector2(level.Width + 900, level.Height + 610), Color.Black);
                        batch.DrawString(font, "Range " + infoArrow.getRange().ToString(), new Vector2(level.Width + 900, level.Height + 630), Color.Black);
                    }
                    if (freeze)
                    {
                        batch.DrawString(font, "FreezeTower", new Vector2(level.Width + 900, level.Height + 550), Color.Black);
                        batch.DrawString(font, "Cost " + infoFreeze.getCost().ToString(), new Vector2(level.Width + 900, level.Height + 570), Color.Black);
                        batch.DrawString(font, "Damage " + infoFreeze.getDamage().ToString(), new Vector2(level.Width + 900, level.Height + 590), Color.Black);
                        batch.DrawString(font, "Attack speed " + infoFreeze.getAttackSpeed().ToString(), new Vector2(level.Width + 900, level.Height + 610), Color.Black);
                        batch.DrawString(font, "Range " + infoFreeze.getRange().ToString(), new Vector2(level.Width + 900, level.Height + 630), Color.Black);
                        batch.DrawString(font, "Slows", new Vector2(level.Width + 900, level.Height + 650), Color.Black);
                    }
                    if (canon)
                    {
                        batch.DrawString(font, "CanonTower", new Vector2(level.Width + 900, level.Height + 550), Color.Black);
                        batch.DrawString(font, "Cost " + infoCanon.getCost().ToString(), new Vector2(level.Width + 900, level.Height + 570), Color.Black);
                        batch.DrawString(font, "Damage " + infoCanon.getDamage().ToString(), new Vector2(level.Width + 900, level.Height + 590), Color.Black);
                        batch.DrawString(font, "Attack speed " + infoCanon.getAttackSpeed().ToString(), new Vector2(level.Width + 900, level.Height + 610), Color.Black);
                        batch.DrawString(font, "Range " + infoCanon.getRange().ToString(), new Vector2(level.Width + 900, level.Height + 630), Color.Black);
                    }

                    foreach (Tower item in towerList)
                    {
                        item.Draw(batch);
                    }

                    //draw de tower buttons
                    btnArrow.Draw(batch);
                    btnFreeze.Draw(batch);
                    btnCanon.Draw(batch);
                    break;
                case GameState.Pause:
                    batch.Draw(MenuBG, new Rectangle(0, 0, 1200, 750), Color.White);
                    btnMenuPlay.Draw(batch);
                    break;
                case GameState.EndGame:
                    if (player.lives > 0)
                    {
                        batch.Draw(VictoryBG, new Rectangle(0, 0, 1200, 750), Color.White);
                    }
                    else
                    {
                        batch.Draw(DefeatBG, new Rectangle(0, 0, 1200, 750), Color.White);
                    }
                    btnBack.Draw(batch);
                    btnNext.Draw(batch);
                    btnBack.setPosition(new Vector2(550, 500));
                    break;
            }
            batch.End();
            base.Draw(gameTime);
        }
         
        /// <summary>
        /// method om tower te plaatsen en toe te voegen aan de lijst
        /// </summary>
        /// <param name="tileX">X coördinaat</param>
        /// <param name="tileY">Y coördinaat</param>
        public void newTower(int tileX, int tileY)
        {
            Tower towerToAdd = null;
            //check het tower type
            switch (towerType)
            {
                case "arrowTower":
                {
                    towerToAdd = new ArrowTower(Content.Load<Texture2D>("arrowTower"), Content.Load<Texture2D>("Arrow"), new Vector2(tileX, tileY));
                    break;
                }
                case "freezeTower":
                {
                    towerToAdd = new FreezeTower(Content.Load<Texture2D>("slowTower"), Content.Load<Texture2D>("freezeBullet"), new Vector2(tileX, tileY));
                    break;
                }
                case "canonTower":
                {
                    towerToAdd = new CanonTower(Content.Load<Texture2D>("canonTower"), Content.Load<Texture2D>("CanonBall"), new Vector2(tileX, tileY));
                    break;
                } 
            }

            if (IsCellClear() && towerToAdd.getCost() <= player.money)
            {
                towerList.Add(towerToAdd);
                player.money -= towerToAdd.getCost();
                
                towerType = string.Empty;
            }
            else
            {
                towerType = string.Empty;
            }
        }

        /// <summary>
        /// check of de tile leeg is
        /// </summary>
        /// <returns></returns>
        private bool IsCellClear()
        {
            // Hier checken we dat het in het speelveld is en niet er buiten
            bool inBounds = cellX >= 0 && cellY >= 0 && cellX < level.Width && cellY < level.Height;
            bool spaceClear = true;

            // Check voor elke toren of de positie vrij is zo niet? stop met het plaatsen van de toren
            foreach (Tower tower in towerList)
            {
                spaceClear = (tower.getPosition() != new Vector2(tileX, tileY));
                if (!spaceClear)
                {
                    break;
                }
            }

            if (onPath = (level.getTileType(new Vector2(cellY, cellX)).Equals("0")))
            {
                if (onPath == false)
                {
                    return true;
                }
            }
            else
            {
                if (onPath)
                {
                    return false;
                }
            }
            // bool onPath = (level.GetIndex(cellX, cellY) != 1);
            return inBounds && spaceClear && onPath;
        }
    }
}
