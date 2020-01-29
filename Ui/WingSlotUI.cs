using CustomSlot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace WingSlot.UI {
    public class WingSlotUI : UIState {
        private const int SlotMargin = 3;

        public CustomItemSlot EquipSlot;
        public CustomItemSlot SocialSlot;
        public CustomItemSlot DyeSlot;

        public CustomUIPanel Panel { get; private set; }

        public bool IsVisible {
            get {
                bool nextToUniques = WingSlotConfig.Instance.SlotLocation == WingSlotConfig.Location.Uniques;
                return Main.playerInventory && (!nextToUniques || Main.EquipPage == 2);
            }
        }

        public override void OnInitialize() {
            WingSlot mod = ModContent.GetInstance<WingSlot>();
            CroppedTexture2D emptyTexture = new CroppedTexture2D(mod.GetTexture("WingSlotBackground"), 
                                                                 CustomItemSlot.DefaultColors.EmptyTexture);

            EquipSlot = new CustomItemSlot(ItemSlot.Context.EquipAccessory, 0.85f) {
                IsValidItem = item => item.wingSlot > 0,
                EmptyTexture = emptyTexture,
                HoverText = Language.GetTextValue("Mods.WingSlot.Wings")
            };

            SocialSlot = new CustomItemSlot(ItemSlot.Context.EquipAccessoryVanity, 0.85f) {
                IsValidItem = item => item.wingSlot > 0,
                EmptyTexture = emptyTexture,
                HoverText = Language.GetTextValue("Mods.WingSlot.SocialWings"),
                Partner = EquipSlot
            };

            DyeSlot = new CustomItemSlot(ItemSlot.Context.EquipDye, 0.85f) {
                IsValidItem = item => item.dye > 0
            };

            float slotSize = EquipSlot.Width.Pixels;

            Panel = new CustomUIPanel();
            Panel.Width.Set((slotSize * 3) + (SlotMargin * 2) + Panel.PaddingLeft + Panel.PaddingRight, 0);
            Panel.Height.Set(slotSize + Panel.PaddingTop + Panel.PaddingBottom, 0);
            SetPosition();

            SocialSlot.Left.Set(slotSize + SlotMargin, 0);
            EquipSlot.Left.Set((slotSize * 2) + (SlotMargin * 2), 0);

            Panel.Append(EquipSlot);
            Panel.Append(SocialSlot);
            Panel.Append(DyeSlot);

            Append(Panel);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);

            if(WingSlotConfig.Instance.SlotLocation == WingSlotConfig.Location.Custom) return;

            SetPosition();
        }

        private Vector2 CalculatePosition() {
            int slotSize = (int)EquipSlot.Width.Pixels;
            int mapH = 0;
            int rX;
            int rY;

            if(Main.mapEnabled) {
                if(!Main.mapFullscreen && Main.mapStyle == 1) {
                    mapH = 256;
                }
            }

            if(WingSlotConfig.Instance.SlotLocation == WingSlotConfig.Location.Uniques) {
                if(Main.mapEnabled) {
                    if((mapH + 600) > Main.screenHeight) {
                        mapH = Main.screenHeight - 600;
                    }
                }

                rX = Main.screenWidth - 92 - ((slotSize + SlotMargin) * 2);
                rY = mapH + 174;

                if(Main.netMode == 1) {
                    rX -= slotSize + SlotMargin;
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

                int slotCount = 7 + Main.player[Main.myPlayer].extraAccessorySlots;

                if((Main.screenHeight < 900) && (slotCount >= 8)) {
                    slotCount = 7;
                }

                rX = Main.screenWidth - 92 - 14 - ((slotSize + SlotMargin) * 3) 
                     - (int)(Main.extraTexture[58].Width * EquipSlot.Scale);
                rY = (int)(mapH + 174 + 4 + slotCount * 56 * EquipSlot.Scale);
            }

            return new Vector2(rX, rY);
        }

        private void SetPosition() {
            Vector2 pos = CalculatePosition();

            Panel.Left.Set(pos.X - Panel.PaddingLeft - ((EquipSlot.Width.Pixels + SlotMargin) * 2), 0);
            Panel.Top.Set(pos.Y - Panel.PaddingTop, 0);
        }
    }
}
