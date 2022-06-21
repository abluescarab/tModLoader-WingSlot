using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace CustomSlot.UI {
    public class AccessorySlotsUI : UIState {
        public readonly string Tag;

        public AccessorySlotsUI(string tag) : base()
        {
            Tag = tag;
        }


        /// <summary>
        /// Where to place the slot panel.
        /// </summary>
        public enum Location {
            Accessories,
            Uniques,
            Custom
        }

        /// <summary>
        /// The horizontal margin between slots.
        /// Default: 3 (from game source code)
        /// </summary>
        protected int HorizontalSlotMargin = 3;
        /// <summary>
        /// The number of rows to skip when on the accessories or uniques page.
        /// </summary>
        public int RowsToSkip;
        /// <summary>
        /// The current panel coordinates.
        /// </summary>
        public Vector2 PanelCoordinates;

        /// <summary>
        /// The slot holding the equipped item.
        /// </summary>
        public CustomItemSlot EquipSlot { get; protected set; }
        /// <summary>
        /// The slot holding the social (vanity) item.
        /// </summary>
        public CustomItemSlot SocialSlot { get; protected set; }
        /// <summary>
        /// The slot holding the dye.
        /// </summary>
        public CustomItemSlot DyeSlot { get; protected set; }
        /// <summary>
        /// Where to place the slot panel.
        /// </summary>
        public Location PanelLocation { get; set; }
        /// <summary>
        /// The panel holding the item slots.
        /// </summary>
        public DraggableUIPanel Panel { get; protected set; }
        /// <summary>
        /// Whether the UI is visible or not.
        /// </summary>
        public virtual bool IsVisible => Main.playerInventory &&
                                         (PanelLocation == Location.Accessories && Main.EquipPage != 2 ||
                                          PanelLocation == Location.Uniques && Main.EquipPage == 2 ||
                                          PanelLocation == Location.Custom);

        public override void OnInitialize() {
            EquipSlot = new CustomItemSlot(Tag, ItemSlot.Context.EquipAccessory, 0.85f);
            SocialSlot = new CustomItemSlot(Tag, ItemSlot.Context.EquipAccessoryVanity, 0.85f) {
                Partner = EquipSlot
            };
            DyeSlot = new CustomItemSlot(Tag, ItemSlot.Context.EquipDye, 0.85f);

            float slotSize = EquipSlot.Width.Pixels;

            Panel = new DraggableUIPanel();
            Panel.Width.Set((slotSize * 3) + (HorizontalSlotMargin * 2) + Panel.PaddingLeft + Panel.PaddingRight, 0);
            Panel.Height.Set(slotSize + Panel.PaddingTop + Panel.PaddingBottom, 0);

            if(PanelLocation == Location.Custom)
                MoveToCustomPosition();
            else
                ResetPosition();

            SocialSlot.Left.Set(slotSize + HorizontalSlotMargin, 0);
            EquipSlot.Left.Set((slotSize * 2) + (HorizontalSlotMargin * 2), 0);

            Panel.Append(EquipSlot);
            Panel.Append(SocialSlot);
            Panel.Append(DyeSlot);

            Append(Panel);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            if(PanelLocation == Location.Custom) {
                PanelCoordinates = new Vector2(Panel.Left.Pixels, Panel.Top.Pixels);
                return;
            }

            ResetPosition();
        }

        public virtual void MoveToCustomPosition() {
            Panel.Left.Set(PanelCoordinates.X, 0);
            Panel.Top.Set(PanelCoordinates.Y, 0);
        }

        protected virtual Vector2 CalculatePosition() {
            int slotSize = (int)EquipSlot.Width.Pixels;
            int mapH = 0;
            int rX;
            int rY;

            // Most of this function is just copied from the game source.
            if(Main.mapEnabled) {
                if(!Main.mapFullscreen && Main.mapStyle == 1) {
                    mapH = 256;
                }
            }

            if(PanelLocation == Location.Uniques) {
                if(Main.mapEnabled) {
                    if((mapH + 600) > Main.screenHeight) {
                        mapH = Main.screenHeight - 600;
                    }
                }

                rX = Main.screenWidth - 92 - ((slotSize + HorizontalSlotMargin) * 3);
                rY = mapH + 174;

                if(Main.netMode == NetmodeID.MultiplayerClient) {
                    rX -= slotSize + HorizontalSlotMargin;
                }
            }
            else {
                if(Main.mapEnabled) {
                    int adjustY = 600;

                    if(Main.player[Main.myPlayer].ExtraAccessorySlotsShouldShow) {
                        adjustY = 610 + PlayerInput.UsingGamepad.ToInt() * 30;
                    }

                    if((mapH + adjustY) > Main.screenHeight) {
                        mapH = Main.screenHeight - adjustY;
                    }
                }

                rX = Main.screenWidth - 92 - 14 - ((slotSize + HorizontalSlotMargin) * 3)
                     - (int)(Main.extraTexture[58].Width * EquipSlot.Scale);
                rY = mapH + 174;
            }

            // Skip the armor section.
            if(PanelLocation == Location.Accessories && RowsToSkip >= 3)
                rY += 4;
            // Fix the minor offset issue that occurs on the uniques page.
            else if(PanelLocation == Location.Uniques)
                rY -= (RowsToSkip / 2);

            // Skip the number of rows specified.
            rY += (int)(Math.Max(RowsToSkip, 0) * 56 * EquipSlot.Scale);

            return new Vector2(rX, rY);
        }

        public virtual void ResetPosition() {
            Vector2 pos = CalculatePosition();

            Panel.Left.Set(pos.X - Panel.PaddingLeft - ((EquipSlot.Width.Pixels + HorizontalSlotMargin) * 2), 0);
            Panel.Top.Set(pos.Y - Panel.PaddingTop, 0);
        }
    }
}
