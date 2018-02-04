using Microsoft.Xna.Framework.Input;

namespace ConwaysGameOfLife
{
    /// <summary>
    /// Tracks the input from a key on the user's keyboard.
    /// Created by: Ethan Genser
    /// </summary>
    public class Key
    {
        #region _Variables_

        private Keys key;
        private bool down;
        public bool pressed { get; private set; }

        #endregion

        // Constructor
        public Key(Keys key)
        {
            this.key = key;
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(key))
                down = true;

            if (Keyboard.GetState().IsKeyUp(key) && down)
            {
                down = false;
                pressed = true;
            }
            else
                pressed = false;
        }
    }
}
