using GenserSprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ConwaysGameOfLife
{
    public enum AnimationEffect { None, Grow, Shade, Animate }
    public enum PressEffect { Press, StickyPress}

    /// <summary>
    /// An AdvancedSprite that allows the user to interact with the player's cursor-based input.
    /// Created by: Ethan Genser
    /// </summary>
    public class Button : AdvancedSprite
    {
        #region _Variables_

        private AnimationEffect buttonEffect;
        private PressEffect pressEffect;
        private float scale;
        public bool pressed;

        #endregion

        // Constructors
        public Button(AnimationEffect buttonEffect, PressEffect pressEffect, Vector2 position, int spriteSheetLength) : base(position, 0f, spriteSheetLength, 1)
        {
            this.position = position;
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHieght = GraphicsDevice.Viewport.Height;

            hitbox = new Rectangle((int)position.X - (int)frameSize.X / 2, (int)position.Y - (int)frameSize.Y / 2, (int)frameSize.X, (int)frameSize.Y);
            rectangle = new Rectangle(0, 0, 0, 0);

            scale = 1;
            this.buttonEffect = buttonEffect;
            this.pressEffect = pressEffect;
        }

        // Update
        public void Update(Cursor cursor, int channel)
        {
            // Seting rectangle and origin for animation
            base.Update();

            // Hovering animations
            if (pressEffect == PressEffect.Press)
            {
                if (hitbox.Intersects(cursor.hitbox))
                    Animate();
                else
                    RevertAnimations();
            }

            // Pressed animations
            if (pressEffect == PressEffect.StickyPress)
            {
                if (pressed)
                    Animate();
                else
                    RevertAnimations();
            }

            // Pressing
            if (pressEffect == PressEffect.Press)
            {
                if (hitbox.Intersects(cursor.hitbox) && cursor.LMBpressed())
                    pressed = true;
                else pressed = false;
            }
            else if (pressEffect == PressEffect.StickyPress)
            {
                if (hitbox.Intersects(cursor.hitbox) && cursor.LMBpressed())
                {
                    if (channel == 1)
                    {
                        Game1.speed2Button.pressed = false;
                        Game1.speed5Button.pressed = false;
                        Game1.speed10Button.pressed = false;
                        Game1.speed30Button.pressed = false;
                        Game1.speedPButton.pressed = false;
                    }
                    else if (channel == 2)
                    {
                        Game1.startButton.pressed = false;
                        Game1.pauseButton.pressed = false;
                    }

                    pressed = true;
                }
            }
        }

        // Hovering Animations
        private void Animate()
        {
            // Grow
            if (buttonEffect == AnimationEffect.Grow)
                if (scale <= 1.1f)
                    scale += 0.05f;

            // Shade
            else if (buttonEffect == AnimationEffect.Shade)
                color = Color.Gray;

            // Animate
            else if (buttonEffect == AnimationEffect.Animate)
            {
                if (currentFrame < spriteSheetLength)
                    currentFrame++;
                else currentFrame = spriteSheetLength;
            }
        }
        private void RevertAnimations()
        {
            // Grow
            if (buttonEffect == AnimationEffect.Grow)
                scale = 1.0f;

            // Shade
            else if (buttonEffect == AnimationEffect.Shade)
                color = Color.White;

            // Animate
            else if (buttonEffect == AnimationEffect.Animate)
                currentFrame = 0;

        }

        // Draw
        public override void Draw()
        {
            spriteBatch.Draw(currentTexture, position, rectangle, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
