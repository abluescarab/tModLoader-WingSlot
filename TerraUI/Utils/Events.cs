using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TerraUI {
    public delegate void FocusHandler(UIObject sender);
    public delegate bool ClickHandler(UIObject sender, ClickEventArgs e);

    public class ClickEventArgs {
        public Vector2 Position { get; private set; }

        public ClickEventArgs(Vector2 position) {
            Position = position;
        }
    }
}
