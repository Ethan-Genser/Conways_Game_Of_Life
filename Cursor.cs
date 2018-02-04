using GenserSprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ConwaysGameOfLife
{
    /// <summary>
    /// Tracks the input from the user's cursor.
    /// Created by: Ethan Genser
    /// </summary>
    public class Cursor : AdvancedSprite
    {
        #region _Variables_

        public bool LMBdown { get; protected set; }
        public bool RMBdown { get; protected set; }

        public int activeTexture
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        #endregion

        // Constructor
        public Cursor(int spriteSheetLength) : base(Vector2.Zero, 0f, spriteSheetLength, 1)
        {
            LMBdown = false;
            RMBdown = false;
        }

        // Update
        public override void Update(GameTime gameTime)
        {
            base.Update();

            hitbox = new Rectangle((int)position.X - currentTexture.Width / 2, (int)position.Y - currentTexture.Height / 2, 1, 1);

            // Following mouse position & screen bounderies
            if (Mouse.GetState().X >= GraphicsDevice.Viewport.Width)
                Mouse.SetPosition(GraphicsDevice.Viewport.Width, Mouse.GetState().Y);

            if (Mouse.GetState().Y >= GraphicsDevice.Viewport.Height)
                Mouse.SetPosition(Mouse.GetState().X, GraphicsDevice.Viewport.Height);

            if ((Mouse.GetState().X > 0) && (Mouse.GetState().X < GraphicsDevice.Viewport.Width - 1))
                position.X = Mouse.GetState().X;
            else if (Mouse.GetState().X <= 0)
                position.X = 0;
            else position.X = GraphicsDevice.Viewport.Width - 1;

            if ((Mouse.GetState().Y > 0) && (Mouse.GetState().Y < GraphicsDevice.Viewport.Height - 1))
                position.Y = Mouse.GetState().Y;
            else if (Mouse.GetState().Y <= 0)
                position.Y = 0;
            else position.Y = GraphicsDevice.Viewport.Height - 1;
        }

        // Pressing and releasing mouse buttons
        public bool LMBpressed()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                LMBdown = true;

            if ((Mouse.GetState().LeftButton == ButtonState.Released) && (LMBdown))
            {
                LMBdown = false;
                return true;
            }
            else return false;
        }
        public bool RMBpressed()
        {
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
                RMBdown = true;

            if ((Mouse.GetState().RightButton == ButtonState.Released) && (RMBdown))
            {
                RMBdown = false;
                return true;
            }
            else return false;
        }
    }
}
