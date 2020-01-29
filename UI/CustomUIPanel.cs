// Code modified from tModLoader ExampleMod
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace WingSlot.UI {
    public class CustomUIPanel : UIPanel {
        private readonly Color _defaultBackgroundColor = new Color(63, 82, 151) * 0.7f;
        private readonly Color _defaultBorderColor = Color.Black;

        private Vector2 _offset;
        private bool _dragging = false;
        private bool _visible = true;

        public bool CanDrag { get; set; } = true;

        public bool Visible {
            get => _visible;
            set {
                _visible = value;

                BackgroundColor = _visible ? _defaultBackgroundColor : Color.Transparent;
                BorderColor = _visible ? _defaultBorderColor : Color.Transparent;
            }
        }

        public override void MouseDown(UIMouseEvent evt) {
            if(!_visible) return;

            base.MouseDown(evt);

            if(!CanDrag) return;

            if(ContainsPoint(evt.MousePosition) && 
               !GetInnerDimensions().ToRectangle().Contains(evt.MousePosition.ToPoint())) {
                DragBegin(evt);
            }
        }

        public override void MouseUp(UIMouseEvent evt) {
            if(!_visible) return;

            base.MouseUp(evt);

            if(!CanDrag) return;

            if(_dragging)
                DragEnd(evt);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if(!_visible) return;

            if(ContainsPoint(Main.MouseScreen)) {
                Main.LocalPlayer.mouseInterface = true;
            }

            if(_dragging) {
                Left.Set(Main.mouseX - _offset.X, 0);
                Top.Set(Main.mouseY - _offset.Y, 0);
            }

            Rectangle parentDimensions = Parent.GetDimensions().ToRectangle();

            if(!GetDimensions().ToRectangle().Intersects(parentDimensions)) {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentDimensions.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentDimensions.Bottom - Height.Pixels);
            }
        }

        private void DragBegin(UIMouseEvent e) {
            _offset = new Vector2(e.MousePosition.X - Left.Pixels, e.MousePosition.Y - Top.Pixels);
            _dragging = true;
        }

        private void DragEnd(UIMouseEvent e) {
            Vector2 end = e.MousePosition;
            _dragging = false;

            Left.Set(end.X - _offset.X, 0);
            Top.Set(end.Y - _offset.Y, 0);
        }
    }
}
