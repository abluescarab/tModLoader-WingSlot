using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Terraria;

namespace WingSlot.UI {
    public class UIButton : UIObject {
        public bool hover;
        public Texture2D texture;
        public Action Function;
        /// <summary>
        /// The Constructor for the UIButton.
        /// </summary>
        /// <param name="pos">The position of the object in pixels. If the parent parameter is not null, then the position is added on top of that.</param>
        /// <param name="size">The size of the button in pixels.</param>
        /// <param name="function">The function to run when the button is pressed. Can implement existing objects you've created.</param>        
        /// <param name="parent">The parent to the button</param>
        /// <param name="texture">The button's texture. Leaving default will draw a rectangle</param>
        public UIButton(Vector2 pos, Vector2 size, Action function, UIObject parent = null, Texture2D texture = null) : base(pos, size, parent) {
            this.Function += function;
            this.texture = texture;
        }
        public override void Draw(SpriteBatch sb) {
            Vector2 position = this.position;
            if(parent != null) {
                position += this.parent.position;
            }
            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y);
            Color color = Color.White;
            if(new Rectangle(Main.mouseX, Main.mouseY, 1, 1).Intersects(this.rectangle)) {
                this.hover = true;
                color = Color.LightGray;
                if(UIParameters.mouseState.LeftButton == ButtonState.Pressed && UIParameters.mouseRect.Intersects(new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y))) {
                    color = new Color(167, 167, 167, 255);
                }
                if(UIParameters.LeftMouseClick(new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y))) {
                    this.Function();
                }
            }
            if(this.texture == null)
                BaseTextureDrawing.DrawRectangleBox(sb, color, Color.Black, this.rectangle, 1);
            else
                sb.Draw(this.texture, this.rectangle, color);
            base.Draw(sb);
        }
    }
}
