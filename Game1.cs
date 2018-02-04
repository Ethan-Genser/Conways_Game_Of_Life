using GenserSprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Created by: Ethan Genser

namespace ConwaysGameOfLife
{
    enum GameState { Start, Paused, Running}

    public class Game1 : Game
    {
        #region _Variables_

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Vector2 screenMiddle;
        float timer;
        float updatesPerSecond;
        GameState gameState;
        Key enter;

        long updates;

        bool[,] currentGeneration;

        Sprite background;

        Cursor cursor;

        public static Button speed2Button;
        public static Button speed5Button;
        public static Button speed10Button;
        public static Button speed30Button;
        public static Button speedPButton;
        public static Button startButton;
        public static Button pauseButton;
        Button exitButton;
        Button restartButton;
        Button[,] cellButtons;

        SpriteFont CourierNew;

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Grapics
            graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Sprite.Content = Content;
            Sprite.GraphicsDevice = GraphicsDevice;

            screenMiddle = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            updates = 0;
            currentGeneration = new bool[81,40];
            enter = new Key(Keys.Enter);
            gameState = GameState.Start;

            cellButtons = new Button[81, 40];
            for (int x = 0; x < 81; x++)
                for (int y = 0; y < 40; y++)
                {
                    cellButtons[x, y] = new Button(AnimationEffect.None, PressEffect.Press, new Vector2(screenMiddle.X + ((x - 40) * 16) , screenMiddle.Y + (int)(((float)y - 21.5) * 16)), 1);
                }

            background = new Sprite(screenMiddle);
            cursor = new Cursor(1);

            speed2Button = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 560, screenMiddle.Y + 350), 1);
            speed2Button.pressed = true;
            speed5Button = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 495, screenMiddle.Y + 350), 1);
            speed10Button = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 430, screenMiddle.Y + 350), 1);
            speed30Button = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 365, screenMiddle.Y + 350), 1);
            speedPButton = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 300, screenMiddle.Y + 350), 1);
            startButton = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X - 100, screenMiddle.Y + 338), 1);
            pauseButton = new Button(AnimationEffect.Shade, PressEffect.StickyPress, new Vector2(screenMiddle.X, screenMiddle.Y + 338), 1);
            restartButton = new Button(AnimationEffect.Shade, PressEffect.Press, new Vector2(screenMiddle.X + 100, screenMiddle.Y + 338), 1);
            exitButton = new Button(AnimationEffect.Shade, PressEffect.Press, new Vector2(GraphicsDevice.Viewport.Width - 15, 15), 1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Sprite.spriteBatch = spriteBatch;

            CourierNew = Content.Load<SpriteFont>("Courier New");

            background.LoadContent("Background");
            cursor.LoadContent("Cursor2");

            foreach (Button b in cellButtons)
                b.LoadContent(@"Cell");

            speed2Button.LoadContent(@"Buttons\Speed_2");
            speed5Button.LoadContent(@"Buttons\Speed_5");
            speed10Button.LoadContent(@"Buttons\Speed_10");
            speed30Button.LoadContent(@"Buttons\Speed_30");
            speedPButton.LoadContent(@"Buttons\Speed_P");
            startButton.LoadContent(@"Buttons\Play");
            pauseButton.LoadContent(@"Buttons\Pause");
            restartButton.LoadContent(@"Buttons\Restart");
            exitButton.LoadContent(@"Buttons\Exit");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            background.Update();
            cursor.Update(gameTime);
            speed2Button.Update(cursor, 1);
            speed5Button.Update(cursor, 1);
            speed10Button.Update(cursor, 1);
            speed30Button.Update(cursor, 1);
            speedPButton.Update(cursor, 1);
            startButton.Update(cursor, 2);
            pauseButton.Update(cursor, 2);
            restartButton.Update(cursor, 0);
            exitButton.Update(cursor, 0);
            enter.Update();

            // Exit button
            if (exitButton.pressed)
                Exit();

            // Restart button
            if (restartButton.pressed)
            {
                startButton.pressed = false;
                pauseButton.pressed = false;
                updates = 0;

                for (int x = 0; x < 81; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        currentGeneration[x, y] = false;
                    }
                }
            }

            // Speed selection
            if (speed2Button.pressed)
                updatesPerSecond = 250;
            else if (speed5Button.pressed)
                updatesPerSecond = 100;
            else if (speed10Button.pressed)
                updatesPerSecond = 50;
            else if (speed30Button.pressed)
                updatesPerSecond = 16.6666f;

            // Play/Pause buttons
            if (startButton.pressed)
                gameState = GameState.Running;
            else if (pauseButton.pressed)
                gameState = GameState.Paused;
            else
                gameState = GameState.Start;
            
            // Start logic
            if (gameState == GameState.Start)
            {
                for (int x = 0; x < 81; x++)
                {
                    for (int y = 0; y < 40; y++)
                    {
                        cellButtons[x, y].Update(cursor, 0);

                        if (cellButtons[x, y].pressed)
                            currentGeneration[x, y] ^= true;
                    }
                }
            }

            // Running logic
            if (gameState == GameState.Running)
            {
                if (!speedPButton.pressed)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                    if (timer > updatesPerSecond)
                    {
                        updates++;
                        currentGeneration = nextGeneration(currentGeneration);
                        timer = 0;
                    }
                }
                else
                {
                    if (enter.pressed)
                    {
                        updates++;
                        currentGeneration = nextGeneration(currentGeneration);
                    }
                }
            }

                base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            background.Draw();
            speed2Button.Draw();
            speed5Button.Draw();
            speed10Button.Draw();
            speed30Button.Draw();
            speedPButton.Draw();
            startButton.Draw();
            pauseButton.Draw();
            restartButton.Draw();
            exitButton.Draw();

            spriteBatch.DrawString(CourierNew, updates.ToString(), new Vector2(screenMiddle.X + 520, screenMiddle.Y + 325), Color.Black, 0f, Vector2.Zero, .75f, SpriteEffects.None, 0f);

            for (int x = 0; x < 81; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    if (currentGeneration[x, y])
                        cellButtons[x, y].Draw();
                }
            }

            cursor.Draw();

            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected bool[,] nextGeneration(bool[,] oldGeneration)
        {
            bool[,] returnValue = new bool[81, 40];

            for (int x = 0; x < 81; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    if (oldGeneration[x, y])
                    {
                        // Each cell with one or no neighbors dies, as if by solitude. Each cell with four or more neighbors dies, as if by overpopulation.
                        if (GetNeighbors(oldGeneration, x, y) <= 1 || GetNeighbors(oldGeneration, x, y) >= 4)
                            returnValue[x, y] = false;
                        else
                            returnValue[x, y] = true;                                             
                    }
                    else
                    {
                        if (GetNeighbors(oldGeneration, x, y) == 3)
                            returnValue[x, y] = true;
                        else
                            returnValue[x, y] = false;
                    }
                }
            }

            return returnValue;
        }

        protected int GetNeighbors(bool[,] oldGeneration, int x, int y)
        {
            int neighbors = 0;

            // Checks up
            if (y != 0)
            {
                if (oldGeneration[x, y - 1])
                    neighbors++;
            }
            // Checks up-right
            if (x != 80 && y != 0)
            {
                if (oldGeneration[x + 1, y - 1])
                    neighbors++;
            }
            // Checks right
            if (x != 80)
            {
                if (oldGeneration[x + 1, y])
                    neighbors++;
            }
            // Checks down-right
            if (x != 80 && y != 39)
            {
                if (oldGeneration[x + 1, y + 1])
                    neighbors++;
            }
            // Checks down
            if (y != 39)
            {
                if (oldGeneration[x, y + 1])
                    neighbors++;
            }
            // Checks down-left
            if (x != 0 && y != 39)
            {
                if (oldGeneration[x - 1, y + 1])
                    neighbors++;
            }
            // Checks left
            if (x != 0)
            {
                if (oldGeneration[x - 1, y])
                    neighbors++;
            }
            // Checks up-left
            if (x != 0 && y != 0)
            {
                if (oldGeneration[x - 1, y - 1])
                    neighbors++;
            }

            return neighbors;
        }
    }
}

// Started: 8/13/17 9:32pm
// Finished: 8/14/17 2:28am
// Total work time:  4hr + 56min