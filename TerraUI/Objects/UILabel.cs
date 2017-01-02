using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraUI.Utilities;

namespace TerraUI.Objects {
    public class UILabel : UIObject {
        /// <summary>
        /// Text displayed in the label.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color TextColor { get; set; }
        /// <summary>
        /// Color of the border around the text.
        /// </summary>
        public Color BorderColor { get; set; }
        /// <summary>
        /// Whether to draw a border around the text.
        /// </summary>
        public bool DrawBorder { get; set; }
        /// <summary>
        /// Font used to draw the text.
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// Create a new UILabel.
        /// </summary>
        /// <param name="position">position of object in pixels</param>
        /// <param name="size">size of object in pixels</param>
        /// <param name="text">text of label</param>
        /// <param name="font">font used for text</param>
        /// <param name="textColor">text color</param>
        /// <param name="borderColor">border color for text</param>
        /// <param name="drawBorder">whether to draw text with border</param>
        /// <param name="parent">parent UIObject</param>
        public UILabel(Vector2 position, Vector2 size, string text, SpriteFont font, bool drawBorder = true, UIObject parent = null)
            : base(position, size, parent, false) {
            Text = text;
            Font = font;
            TextColor = UIColors.Label.TextColor;
            BorderColor = UIColors.Label.BorderColor;
            DrawBorder = drawBorder;
        }

        /// <summary>
        /// Draw the UILabel.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch) {
            Rectangle = new Rectangle((int)RelativePosition.X, (int)RelativePosition.Y, (int)Size.X, (int)Size.Y);

            string text = WrapText(Font, Text, Size.X);

            if(DrawBorder) {
                Terraria.Utils.DrawBorderStringFourWay(spriteBatch, Font, text, RelativePosition.X, RelativePosition.Y,
                    TextColor, BorderColor, Vector2.Zero, 1f);
            }
            else {
                spriteBatch.DrawString(Font, text, RelativePosition, TextColor);
            }

            base.Draw(spriteBatch);
        }

        /// <summary>
        /// Wrap the text in the label.
        /// Source: <a href="http://stackoverflow.com/questions/15986473/how-do-i-implement-word-wrap">Stack Overflow</a>
        /// </summary>
        /// <param name="font">font used for text</param>
        /// <param name="text">text to wrap</param>
        /// <param name="maxLineWidth">max line width in pixels</param>
        /// <returns>formatted text</returns>
        public string WrapText(SpriteFont font, string text, float maxLineWidth) {
            string[] words = text.Split(' ');
            StringBuilder builder = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach(string word in words) {
                Vector2 size = font.MeasureString(word);

                if(lineWidth + size.X < maxLineWidth) {
                    builder.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else {
                    builder.Append(Environment.NewLine + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return builder.ToString();
        }
    }
}
