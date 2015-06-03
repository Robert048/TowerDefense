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
        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D BG;

        //Objecten
        private Player player = new Player();
        private Level level = new Level();
        private Wave_manager manager;

        //buttons
        private Button btnMenuPlay;
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
        //Place Tower bool
        private bool arrow;
        private bool canon;
        private bool freeze;

        


        //gameStates for the different states the game has
        enum GameState
        {
            MainMenu, Credits, Settings, LevelSelect, Playing, EndGame, Pause
        }
        GameState CurrentGameState = GameState.MainMenu;

        public TowerDefense()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            int levelIndex = 1;
            manager = new Wave_manager(level.getWaypoints(), levelIndex);
            towerList = new List<Tower>();
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
            //add sprites for tiles to level
            level.AddTexture(grass);
            level.AddTexture(road);
            level.AddTexture(turn);

            //load content for the game
            font = Content.Load<SpriteFont>("font");
            BG = Content.Load<Texture2D>("BG_ingame");

            //buttons
            btnMenuPlay = new Button(Content.Load<Texture2D>("Play"), graphics.GraphicsDevice);
            btnMenuPlay.setPosition(new Vector2(525, 125));

            btnArrow = new Button(Content.Load<Texture2D>("arrowTowerFront"), graphics.GraphicsDevice);
            btnArrow.setPosition(new Vector2(250, 600));

            btnFreeze = new Button(Content.Load<Texture2D>("slowTowerFront"), graphics.GraphicsDevice);
            btnFreeze.setPosition(new Vector2(400, 600));

            btnCanon = new Button(Content.Load<Texture2D>("canonTowerFront"), graphics.GraphicsDevice);
            btnCanon.setPosition(new Vector2(650, 650));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //gamestates
            MouseState mouse = Mouse.GetState();
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                    IsMouseVisible = true;
                    if (btnMenuPlay.isClicked == true) CurrentGameState = GameState.Playing;
                    btnMenuPlay.Update(mouse);
                    break;
                case GameState.Settings:
                    break;
                case GameState.Credits:
                    break;
                case GameState.LevelSelect:
                    break;
                case GameState.Playing:
                    //keep game fullscreen
                    //graphics.PreferredBackBufferWidth = level.Width * 50;
                    //graphics.PreferredBackBufferHeight = (level.Height * 50) + 200;
                    //graphics.ApplyChanges();

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
                        //todo cursor icon tower
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
                        //todo cursor icon tower
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
                        //todo cursor icon tower
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
                    btnArrow.Update(mouse);
                    btnFreeze.Update(mouse);
                    btnCanon.Update(mouse);



                    manager.Update(gameTime);
                    break;
                case GameState.Pause:
                    break;
                case GameState.EndGame:
                    break;
            }

            base.Update(gameTime);
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
                case GameState.MainMenu:
                    //draw buttons
                    btnMenuPlay.Draw(batch);

                    break;
                case GameState.Settings:
                    break;
                case GameState.Credits:
                    break;
                case GameState.LevelSelect:
                    break;
                case GameState.Playing:
                    level.Draw(batch);
                    //draw enemies from wave manager > wave
                    manager.Draw(batch, Content);
                    //onderste gedeelte
                    batch.Draw(BG, new Rectangle(0, 550, 1200, 200), Color.White);
                    batch.DrawString(font, "Lives: ", new Vector2(level.Width, level.Height + 550 ), Color.Black);
                    batch.DrawString(font, "Gold: ", new Vector2(level.Width, level.Height + 570), Color.Black);
                    batch.DrawString(font, "iets: ", new Vector2(level.Width, level.Height + 590), Color.Black);
                   
                    batch.DrawString(font, "Towers: ", new Vector2(level.Width + 225, level.Height + 550), Color.Black);

                    //if (arrow)
                    //{
                    //    batch.DrawString(font, ArrowTower.getCost().ToString(), new Vector2(level.Width + 225, level.Height + 550), Color.Black);
                    //}
                    //if (freeze)
                    //{
                    //    batch.DrawString(font, arowTower.getCost().ToString(), new Vector2(level.Width + 225, level.Height + 550), Color.Black);
                    //}
                    //if (canon)
                    //{
                    //    batch.DrawString(font, arowTower.getCost().ToString(), new Vector2(level.Width + 225, level.Height + 550), Color.Black);
                    //}

                    // arowTower.getCost().ToString()

                    foreach (Tower item in towerList)
                    {                        
                        item.Draw(batch);
                    }

                    btnArrow.Draw(batch);
                    btnFreeze.Draw(batch);
                    btnCanon.Draw(batch);
                    
                    break;
                case GameState.Pause:
                    break;
                case GameState.EndGame:
                    break;
            }
            batch.End();

            base.Draw(gameTime);
        }

         public void newTower(int tileX, int tileY)
        {
            Tower towerToAdd = null;
            switch (towerType)
            {
                case "arrowTower":
                {
                    towerToAdd = new ArrowTower(Content.Load<Texture2D>("arrowTower"), new Vector2(tileX, tileY));
                    break;
                }
                case "freezeTower":
                {
                    towerToAdd = new ArrowTower(Content.Load<Texture2D>("slowTower"), new Vector2(tileX, tileY));
                    break;
                }
                case "canonTower":
                {
                    towerToAdd = new ArrowTower(Content.Load<Texture2D>("canonTower"), new Vector2(tileX, tileY));
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

            if (onPath = (level.getTileType(new Vector2(cellX, cellY)) != 1))
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
