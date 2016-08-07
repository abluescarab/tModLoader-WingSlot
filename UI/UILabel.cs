using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;

namespace WingSlot.UI {
    public class UILabel : UIObject {
        public string text;
        public Color color;
        public Color borderColor;
        public delegate string GetText();
        public GetText Update;
        public SpriteFont font;
        /// <summary>
        /// The Constructor for the UILabel
        /// </summary>
        /// <param name="pos">The position of the object in pixels. If the parent parameter is not null, then the position is added on top of that.</param>
        /// <param name="font">The font that the text is drawn with.</param>
        /// <param name="size">The size of the textbox. The text wraps to fit the width, though the height doesn't matter too much.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="borderColour">The colour of the border.</param>
        /// <param name="updateText">This is called every frame, and updates the text of the label.</param>
        /// <param name="parent">The parent to the label</param>
        public UILabel(Vector2 pos, SpriteFont font, Vector2 size, Color color, Color borderColour, GetText updateText, UIObject parent = null) : base(pos, size, parent) {
            this.color = color;
            this.borderColor = borderColour;
            this.font = font;
            this.Update = updateText;
        }
        public override void Draw(SpriteBatch sb) {
            Vector2 position = this.position;
            if(parent != null) {
                position += this.parent.position;
            }
            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y);
            this.text = this.Update();
            string text = this.WrapText(this.font, this.text, 430f);
            Utils.DrawBorderStringFourWay(sb, this.font, text, position.X, position.Y, this.color, this.borderColor, default(Vector2));
            base.Draw(sb);
        }
        //Credit to Alina B. On StackOverflow for this code. :)
        //http://stackoverflow.com/questions/15986473/how-do-i-implement-word-wrap
        public string WrapText(SpriteFont spriteFont, string text, float maxLineWidth) {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = spriteFont.MeasureString(" ").X;
            foreach(string word in words) {
                Vector2 size = spriteFont.MeasureString(word);

                if(lineWidth + size.X < maxLineWidth) {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }
            return sb.ToString();
        }
    }
}
