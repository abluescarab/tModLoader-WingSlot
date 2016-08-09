using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace WingSlot.UI {
    public class UIItemSlot : UIObject {
        //variables
        public Item item = new Item();
        public Condition conditions;
        public DrawInItemSlot drawBack;
        public DrawInItemSlot drawItem;
        public DrawInItemSlot postDrawItem;
        public LeftClick leftClick;
        public RightClick rightClick;
        public bool drawAsNormalItemSlot;
        public int contextForItemSlot;

        //Delegates, voids
        public delegate void DrawInItemSlot(SpriteBatch sb, UIItemSlot obj);
        public delegate bool Condition(Item item);
        public delegate bool LeftClick(UIItemSlot slot);
        public delegate bool RightClick(UIItemSlot slot);

        /// <summary>
        /// Create a new UIItemSlot.
        /// </summary>
        /// <param name="position">Position of the slot (px). If parent is not null, position is added to parent position.</param>
        /// <param name="size">Size of the slot</param>
        /// <param name="parent">Parent UIObject</param>
        /// <param name="condition">Condition checked before item is placed in slot. If null, all items are permitted.</param>
        /// <param name="drawBackground">Method run when slot background is drawn. If null, slot is drawn with default background texture.</param>
        /// <param name="drawItem">Method run when item in slot is drawn. If null, item is drawn in center of slot.</param>
        /// <param name="postDrawItem">Method run after item in slot is drawn. Use to draw elements over the item.</param>
        /// <param name="leftClick">Method run when slot is left clicked. Leave null for default behavior.</param>
        /// <param name="rightClick">Method run when slot is right clicked. Leave null for default behavior.</param>
        /// <param name="drawAsNormalItemSlot">Draw as an inventory ItemSlot.</param>
        /// <param name="contextForItemSlot">Context for slot if drawAsNormalItemSlot is true.</param>
        public UIItemSlot(Vector2 position, int size = 52, UIObject parent = null, Condition condition = null,
            DrawInItemSlot drawBackground = null, DrawInItemSlot drawItem = null, DrawInItemSlot postDrawItem = null,
            LeftClick leftClick = null, RightClick rightClick = null, bool drawAsNormalItemSlot = false,
            int contextForItemSlot = 1) : base(position, new Vector2(size), parent) {
            this.item = new Item();
            this.conditions = condition;
            this.drawBack = drawBackground;
            this.drawItem = drawItem;
            this.postDrawItem = postDrawItem;
            this.leftClick = leftClick;
            this.rightClick = rightClick;
            this.drawAsNormalItemSlot = drawAsNormalItemSlot;
            this.contextForItemSlot = contextForItemSlot;
        }

        /// <summary>
        /// Handle the mouse click events.
        /// </summary>
        /// <returns>bool specified by custom click handler</returns>
        public void Handle() {
            if(Main.mouseLeftRelease && Main.mouseLeft) {
                if(leftClick == null) {
                    ItemSlot.LeftClick(ref this.item, 0);
                    Recipe.FindRecipes();
                }
                else {
                    if(leftClick(this)) {
                        Main.mouseLeftRelease = false; // prevent repeat
                    }
                    else {
                        ItemSlot.LeftClick(ref this.item, 0);
                        Recipe.FindRecipes();
                    }
                }
            }
            else if(Main.mouseRight && Main.mouseRightRelease) {
                if(rightClick == null) {
                    ItemSlot.RightClick(ref this.item, 0);
                }
                else {
                    if(rightClick(this)) {
                        Main.mouseRightRelease = false; // prevent repeat
                    }
                    else {
                        ItemSlot.RightClick(ref this.item, 0);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb) {
            Vector2 position = this.position;
            Point mouse = new Point(Main.mouseX, Main.mouseY);

            if(parent != null) {
                position += this.parent.position;
            }

            this.rectangle = new Rectangle((int)position.X, (int)position.Y, (int)this.size.X, (int)this.size.Y);

            if(this.rectangle.Contains(mouse) && !PlayerInput.IgnoreMouseInterface) {
                Main.player[Main.myPlayer].mouseInterface = true;
                if(this.conditions != null) {
                    if(this.conditions(Main.mouseItem)) {
                        Handle();
                    }
                }
                else {
                    Handle();
                }
            }

            Mod mod = ModLoader.GetMod(UIParameters.MODNAME);

            if(drawAsNormalItemSlot) {
                ItemSlot.Draw(sb, ref this.item, contextForItemSlot, this.position);
            }
            else {
                if(drawBack != null) {
                    drawBack(sb, this);
                }
                else {
                    sb.Draw(Main.inventoryBackTexture, this.rectangle, Color.White); // Draws as default texture.
                }
            }

            if(this.item.type > 0) {
                if(drawItem != null) {
                    drawItem(sb, this);
                }
                else {
                    Texture2D texture2D = Main.itemTexture[this.item.type];
                    Rectangle rectangle2;
                    if(Main.itemAnimations[item.type] != null) {
                        rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D);
                    }
                    else {
                        rectangle2 = texture2D.Frame(1, 1, 0, 0);
                    }
                    Vector2 origin = new Vector2(rectangle2.Width / 2, rectangle2.Height / 2);
                    sb.Draw(Main.itemTexture[this.item.type],
                        new Vector2(this.rectangle.X + this.rectangle.Width / 2, this.rectangle.Y + this.rectangle.Height / 2),
                        new Rectangle?(rectangle2), //Big thanks to Abluescarab for this! :D
                        Color.White,
                        0f,
                        origin,
                        1f,
                        SpriteEffects.None,
                        0f);
                }
            }

            if(postDrawItem != null) {
                postDrawItem(sb, this);
            }

            base.Draw(sb);
        }
    }
}