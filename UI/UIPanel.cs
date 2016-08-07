using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WingSlot.UI {
    public class UIPanel : UIObject {
        public Texture2D t;
        /// <summary>
        /// The Constructor for the UIPanel. This tends to be your base object if you want a window.
        /// </summary>
        /// <param name="pos">The position of the object in pixels. If the parent parameter is not null, then the position is added on top of that.</param>
        /// <param name="size">The size of the object in pixels.</param>
        /// <param name="parent">The parent to the label</param>
        /// <param name="fullTexture">The texture to draw at the position with the size.</param>
        public UIPanel(Vector2 pos, Vector2 size, UIObject parent = null, Texture2D fullTexture = null) : base(pos, size, parent) {
            t = fullTexture;
        }
        public override void Draw(SpriteBatch sb) {
            Vector2 position = this.position;
            if(parent != null) {
                position += this.parent.position;
            }
            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y);
            if(this.t != null) {
                sb.Draw(this.t, this.rectangle, Color.White);
            }
            else {
                BaseTextureDrawing.DrawTerrariaStyledBox(sb, new Color(10, 10, 140), this.rectangle);
            }
            base.Draw(sb);
        }
    }
}
