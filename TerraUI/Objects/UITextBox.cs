using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TerraUI.Utilities;
using Input = Microsoft.Xna.Framework.Input;

namespace TerraUI.Objects {
    public class UITextBox : UIObject {
        private const int frameDelay = 9;
        private int selectionStart = 0;
        private int leftArrow = 0;
        private int rightArrow = 0;
        private int delete = 0;

        /// <summary>
        /// The text displayed in the UITextBox.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The font used in the UITextBox.
        /// </summary>
        public SpriteFont Font { get; set; }
        /// <summary>
        /// The default border color.
        /// </summary>
        public Color BorderColor { get; set; }
        /// <summary>
        /// The default background color.
        /// </summary>
        public Color BackColor { get; set; }
        /// <summary>
        /// The default text color.
        /// </summary>
        public Color TextColor { get; set; }
        /// <summary>
        /// The index where the selection in the UITextBox begins.
        /// </summary>
        public int SelectionStart {
            get { return selectionStart; }
            private set {
                if(value < 0) {
                    selectionStart = 0;
                }
                else if(value > Text.Length) {
                    selectionStart = Text.Length;
                }
                else {
                    selectionStart = value;
                }
            }
        }

        /// <summary>
        /// Create a new UITextBox.
        /// </summary>
        /// <param name="position">position of object in pixels</param>
        /// <param name="size">size of object in pixels</param>
        /// <param name="font">text font</param>
        /// <param name="text">displayed text</param>
        /// <param name="parent">parent object</param>
        public UITextBox(Vector2 position, Vector2 size, SpriteFont font, string text = "", UIObject parent = null)
            : base(position, size, parent, true, true) {
            Text = text;
            Focused = false;
            Font = font;
            BorderColor = UIColors.TextBox.BorderColor;
            BackColor = UIColors.TextBox.BackColor;
            TextColor = UIColors.TextBox.TextColor;
        }

        /// <summary>
        /// Give the object focus.
        /// </summary>
        public override void Focus() {
            base.Focus();
            SelectionStart = Text.Length;
        }

        /// <summary>
        /// Update the object. Call during any Update() function.
        /// </summary>
        public override void Update() {
            if(Focused) {
                bool skip = false;

                if(Text.Length > 0) {
                    if(KeyboardUtils.JustPressed(Input.Keys.Left) || KeyboardUtils.HeldDown(Input.Keys.Left)) {
                        if(leftArrow == 0) {
                            SelectionStart--;
                            leftArrow = frameDelay;
                        }
                        leftArrow--;
                        skip = true;
                    }
                    else if(KeyboardUtils.JustPressed(Input.Keys.Right) || KeyboardUtils.HeldDown(Input.Keys.Right)) {
                        if(rightArrow == 0) {
                            SelectionStart++;
                            rightArrow = frameDelay;
                        }
                        rightArrow--;
                        skip = true;
                    }
                    else if(KeyboardUtils.JustPressed(Input.Keys.Delete) || KeyboardUtils.HeldDown(Input.Keys.Delete)) {
                        if(delete == 0) {
                            if(SelectionStart < Text.Length) {
                                Text = Text.Remove(SelectionStart, 1);
                            }
                            delete = frameDelay;
                        }
                        delete--;
                        skip = true;
                    }
                    else if(KeyboardUtils.JustPressed(Input.Keys.Enter)) {
                        Unfocus();
                    }
                    else {
                        leftArrow = 0;
                        rightArrow = 0;
                        delete = 0;
                    }
                }

                if(!skip) {
                    int oldLength = Text.Length;
                    string substring = Text.Substring(0, SelectionStart);
                    string input = Main.GetInputText(substring);

                    // first, we check if the length of the string has changed, indicating
                    // text has been added or removed
                    if(input.Length != substring.Length) {
                        // we remove the old text and replace it with the new, storing it
                        // in a temporary variable
                        string newText = Text.Remove(0, SelectionStart).Insert(0, input);

                        // now if the text is smaller than previously or if not, the string is
                        // an appropriate size,
                        if(newText.Length < Text.Length || Font.MeasureString(newText).X < Size.X - 12) {
                            // we set the old text to the new text
                            Text = newText;

                            // if the length of the text is now longer,
                            if(Text.Length > oldLength) {
                                // adjust the selection start accordingly
                                SelectionStart += (Text.Length - oldLength);
                            }
                            // or if the length of the text is now shorter
                            else if(Text.Length < oldLength) {
                                // adjust the selection start accordingly
                                SelectionStart -= (oldLength - Text.Length);
                            }
                        }
                    }
                }
            }

            base.Update();
        }

        /// <summary>
        /// Draw the UITextBox.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch) {
            Rectangle = new Rectangle((int)RelativePosition.X, (int)RelativePosition.Y, (int)Size.X, (int)Size.Y);

            BaseTextureDrawing.DrawRectangleBox(spriteBatch, BorderColor, BackColor, Rectangle, 2);

            if(Focused) {
                spriteBatch.DrawString(Font, Text.Insert(SelectionStart, "|"), RelativePosition + new Vector2(2), TextColor);
            }
            else {
                spriteBatch.DrawString(Font, Text, RelativePosition + new Vector2(2), TextColor);
            }

            base.Draw(spriteBatch);
        }
    }
}
