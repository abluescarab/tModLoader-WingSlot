using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.UI;
using TerraUI.Utilities;

namespace TerraUI.Objects {
    public class UIItemSlot : UIObject {
        protected Item item;
        protected const int defaultSize = 52;
        protected Rectangle tickRect;

        /// <summary>
        /// Method that checks whether an item can go in the slot.
        /// </summary>
        public ConditionHandler Conditions { get; set; }
        /// <summary>
        /// Method that draws the background of the item slot.
        /// </summary>
        public DrawHandler DrawBackground { get; set; }
        /// <summary>
        /// Method that draws the item in the slot.
        /// </summary>
        public DrawHandler DrawItem { get; set; }
        /// <summary>
        /// Method called after the item in the slot is drawn.
        /// </summary>
        public DrawHandler PostDrawItem { get; set; }
        /// <summary>
        /// Whether the item in the slot is visible on the player character.
        /// </summary>
        public bool ItemVisible { get; set; }
        /// <summary>
        /// Whether to draw the slot as a normal item slot.
        /// </summary>
        public bool DrawAsNormalSlot { get; set; }
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
        /// The slot to swap items with if this slot is right-clicked.
        /// </summary>
        public UIItemSlot Partner { get; set; }

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
        /// <param name="drawAsNormalSlot">draw as a normal inventory ItemSlot</param>
        public UIItemSlot(Vector2 position, int size = 52, Contexts context = Contexts.InventoryItem, UIObject parent = null,
                          ConditionHandler conditions = null, DrawHandler drawBackground = null, DrawHandler drawItem = null,
                          DrawHandler postDrawItem = null, bool drawAsNormalSlot = false, bool scaleToInventory = false)
            : base(position, new Vector2(size), parent, false) {
            Item = new Item();
            Context = context;
            Conditions = conditions;
            DrawBackground = drawBackground;
            DrawItem = drawItem;
            PostDrawItem = postDrawItem;
            DrawAsNormalSlot = drawAsNormalSlot;
        }

        /// <summary>
        /// The default left click event.
        /// </summary>
        public override void OnLeftClick() {
            if(Item.stack > 0 || Conditions(Main.mouseItem)) {
                Swap(ref item, ref Main.mouseItem);
            }
        }

        /// <summary>
        /// The default right click event.
        /// </summary>
        public override void OnRightClick() {
            if(Conditions(Main.mouseItem)) {
                Swap(ref item, ref Main.mouseItem);
            }
            else if(Partner != null && (Item.stack > 0 || Partner.Item.stack > 0)) {
                Swap(ref item, ref Partner.item);
            }
        }

        /// <summary>
        /// Swap two items between slots or the slot and the mouse cursor.
        /// </summary>
        /// <param name="item1">first item</param>
        /// <param name="item2">second item</param>
        public void Swap(ref Item item1, ref Item item2) {
            UIUtils.SwitchItems(ref item1, ref item2);
            UIUtils.PlaySound(Sounds.Grab);
            Recipe.FindRecipes();
        }

        /// <summary>
        /// Toggle the visibility of the item in the slot.
        /// </summary>
        public void ToggleVisibility() {
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
        /// Draw the object. Call during any Draw() function.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public override void Draw(SpriteBatch spriteBatch) {
            Rectangle = new Rectangle((int)RelativePosition.X, (int)RelativePosition.Y, (int)Size.X, (int)Size.Y);

            if(DrawAsNormalSlot) {
                ItemSlot.Draw(spriteBatch, ref item, (int)Context, RelativePosition);
            }
            else {
                if(DrawBackground != null) {
                    DrawBackground(this, spriteBatch);
                }
                else {
                    OnDrawBackground(spriteBatch);
                }

                if(item.type > 0) {
                    if(DrawItem != null) {
                        DrawItem(this, spriteBatch);
                    }
                    else {
                        OnDrawItem(spriteBatch);
                    }
                }
            }

            if(PostDrawItem != null) {
                PostDrawItem(this, spriteBatch);
            }
            else {
                if(HasTick() && !DrawAsNormalSlot) {
                    DrawTick(spriteBatch);
                }
            }

            base.Draw(spriteBatch);
        }

        /// <summary>
        /// The default DrawBackground function.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public void OnDrawBackground(SpriteBatch spriteBatch) {
            Texture2D backTex = UIUtils.GetContextTexture(Context);
            spriteBatch.Draw(
                backTex,
                Rectangle.TopLeft(),
                null,
                Color.White,
                0f,
                Vector2.Zero,
                (ScaleToInventory ? Main.inventoryScale : 1f),
                SpriteEffects.None,
                1f);
        }

        /// <summary>
        /// The default DrawItem function.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public void OnDrawItem(SpriteBatch spriteBatch) {
            Texture2D texture2D = Main.itemTexture[item.type];
            Rectangle rectangle;
            float scale = (ScaleToInventory ? Main.inventoryScale : 1f);

            if(Main.itemAnimations[item.type] != null) {
                rectangle = Main.itemAnimations[item.type].GetFrame(texture2D);
            }
            else {
                rectangle = texture2D.Frame(1, 1, 0, 0);
            }

            Vector2 origin = rectangle.Size() / 2f * scale;
            Vector2 position = Rectangle.TopLeft();

            spriteBatch.Draw(
                texture2D,
                position + (Rectangle.Size() / 2f) - (origin / 2f),
                new Rectangle?(rectangle),
                Color.White,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f);
        }

        /// <summary>
        /// Draws the visibility tick in the slot.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void DrawTick(SpriteBatch spriteBatch) {
            Texture2D tickTexture = Main.inventoryTickOnTexture;

            if(!ItemVisible) {
                tickTexture = Main.inventoryTickOffTexture;
            }

            tickRect = new Rectangle(Rectangle.Left + 34, Rectangle.Top - 2, tickTexture.Width, tickTexture.Height);
            spriteBatch.Draw(tickTexture, tickRect, Color.White * 0.7f);
        }

        /// <summary>
        /// Checks if the slot has a context that needs a tick.
        /// </summary>
        /// <returns>whether the slot has a tick</returns>
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
