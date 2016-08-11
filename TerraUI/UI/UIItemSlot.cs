using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace TerraUI {
    public class UIItemSlot : UIObject {
        protected Item item;
        protected const int defaultSize = 52;
        protected Rectangle tickRect;

        public delegate void DrawItemSlotHandler(SpriteBatch spriteBatch, UIItemSlot slot);
        public delegate bool ConditionHandler(Item item);

        /// <summary>
        /// Method that checks whether an item can go in the slot.
        /// </summary>
        public ConditionHandler Conditions { get; set; }
        /// <summary>
        /// Method that draws the background of the item slot.
        /// </summary>
        public DrawItemSlotHandler DrawBackground { get; set; }
        /// <summary>
        /// Method that draws the item in the slot.
        /// </summary>
        public DrawItemSlotHandler DrawItem { get; set; }
        /// <summary>
        /// Method called after the item in the slot is drawn.
        /// </summary>
        public DrawItemSlotHandler PostDrawItem { get; set; }
        /// <summary>
        /// Whether the item in the slot is visible on the player character.
        /// </summary>
        public bool ItemVisible { get; set; }
        /// <summary>
        /// Whether to draw the slot as a normal item slot.
        /// </summary>
        public bool DrawAsNormalItemSlot { get; set; }
        /// <summary>
        /// The context for the slot.
        /// </summary>
        public Contexts Context { get; set; }
        /// <summary>
        /// Whether to scale the slot with the inventory's scale.
        /// </summary>
        public bool ScaleToInventory { get; set; }
        /// <summary>
        /// The item shown in the slot.
        /// </summary>
        public Item Item {
            get { return item; }
            set { item = value; }
        }

        /// <summary>
        /// Create a new UIItemSlot.
        /// </summary>
        /// <param name="position">position of slot in pixels</param>
        /// <param name="size">size of slot in pixels</param>
        /// <param name="context">context for slot</param>
        /// <param name="parent">parent UIObject</param>
        /// <param name="conditions">checked before item is placed in slot; if null, all items are permitted</param>
        /// <param name="drawBackground">run when slot background is drawn; if null, slot is drawn with background texture</param>
        /// <param name="drawItem">run when item in slot is drawn; if null, item is drawn in center of slot</param>
        /// <param name="postDrawItem">run after item in slot is drawn; use to draw elements over the item</param>
        /// <param name="drawAsNormalItemSlot">draw as a normal inventory ItemSlot</param>
        public UIItemSlot(Vector2 position, int size = 52, Contexts context = Contexts.InventoryItem, UIObject parent = null,
                          ConditionHandler conditions = null, DrawItemSlotHandler drawBackground = null, DrawItemSlotHandler drawItem = null,
                          DrawItemSlotHandler postDrawItem = null, bool drawAsNormalItemSlot = false, bool scaleToInventory = false)
            : base(position, new Vector2(size), parent, false) {
            Item = new Item();
            Context = context;
            Conditions = conditions;
            DrawBackground = drawBackground;
            DrawItem = drawItem;
            PostDrawItem = postDrawItem;
            DrawAsNormalItemSlot = drawAsNormalItemSlot;
        }

        /// <summary>
        /// The default left click event.
        /// </summary>
        protected override void DefaultLeftClick() {
            ItemSlot.LeftClick(ref item, 0);
            Recipe.FindRecipes();
        }

        /// <summary>
        /// Toggle the visibility of the item in the slot.
        /// </summary>
        protected void ToggleVisibility() {
            ItemVisible = !ItemVisible;
            UIUtils.PlaySound(Sounds.MenuTick);
        }

        /// <summary>
        /// Update the item slot.
        /// </summary>
        public override void Update() {
            if(!PlayerInput.IgnoreMouseInterface) {
                if(MouseUtils.Rectangle.Intersects(tickRect) && HasTick()) {
                    Main.player[Main.myPlayer].mouseInterface = true;

                    if(MouseUtils.JustPressed(MouseButtons.Left)) {
                        ToggleVisibility();
                    }
                }
                else {
                    base.Update();
                }
            }
        }

        /// <summary>
        /// The default right click event.
        /// </summary>
        protected override void DefaultRightClick() {
            ItemSlot.RightClick(ref item, 0);
        }

        /// <summary>
        /// Draw the object. Call during any Draw() function.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch) {
            Vector2 position = Position;
            Point mouse = new Point(Main.mouseX, Main.mouseY);

            if(Parent != null) {
                position += Parent.Position;
            }

            Rectangle = new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y);

            if(DrawAsNormalItemSlot) {
                ItemSlot.Draw(spriteBatch, ref item, (int)Context, position);
            }
            else {
                if(DrawBackground != null) {
                    DrawBackground(spriteBatch, this);
                }
                else {
                    Texture2D backTex = UIUtils.GetContextTexture(Context);
                    spriteBatch.Draw(backTex, Rectangle, Color.White);
                }
            }

            if(item.type > 0) {
                if(DrawItem != null) {
                    DrawItem(spriteBatch, this);
                }
                else {
                    Texture2D texture2D = Main.itemTexture[item.type];
                    Rectangle rectangle2;

                    if(Main.itemAnimations[item.type] != null) {
                        rectangle2 = Main.itemAnimations[item.type].GetFrame(texture2D);
                    }
                    else {
                        rectangle2 = texture2D.Frame(1, 1, 0, 0);
                    }

                    Vector2 origin = new Vector2(rectangle2.Width / 2, rectangle2.Height / 2);

                    spriteBatch.Draw(
                        Main.itemTexture[item.type],
                        new Vector2(Rectangle.X + Rectangle.Width / 2,
                                    Rectangle.Y + Rectangle.Height / 2),
                        new Rectangle?(rectangle2),
                        Color.White,
                        0f,
                        origin,
                        (ScaleToInventory ? Main.inventoryScale : 1f),
                        SpriteEffects.None,
                        0f);
                }
            }

            if(PostDrawItem != null) {
                PostDrawItem(spriteBatch, this);
            }
            else {
                if(HasTick()) {
                    Texture2D tickTexture = Main.inventoryTickOnTexture;

                    if(!ItemVisible) {
                        tickTexture = Main.inventoryTickOffTexture;
                    }

                    tickRect = new Rectangle(Rectangle.Left + 34, Rectangle.Top - 2, tickTexture.Width, tickTexture.Height);
                    spriteBatch.Draw(tickTexture, tickRect, Color.White * 0.7f);
                }
            }

            base.Draw(spriteBatch);
        }

        protected bool HasTick() {
            if(Context == Contexts.EquipAccessory ||
               Context == Contexts.EquipLight ||
               Context == Contexts.EquipPet) {
                return true;
            }

            return false;
        }
    }
}
