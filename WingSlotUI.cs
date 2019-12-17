using CustomSlot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace WingSlot {
    public class WingSlotUI : UIState {
        public CustomItemSlot EquipSlot;
        public CustomItemSlot VanitySlot;
        public CustomItemSlot DyeSlot;

        public bool IsVisible {
            get {
                bool slotsNextToAccessories = ModContent.GetInstance<WingSlotConfig>().SlotsNextToAccessories;
                return Main.playerInventory && ((slotsNextToAccessories && Main.EquipPage == 0) ||
                                                (!slotsNextToAccessories && Main.EquipPage == 2));
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

            VanitySlot = new CustomItemSlot(ItemSlot.Context.EquipAccessoryVanity, 0.85f) {
                IsValidItem = item => item.wingSlot > 0,
                EmptyTexture = emptyTexture,
                HoverText = Language.GetTextValue("Mods.WingSlot.SocialWings"),
                Partner = EquipSlot
            };

            DyeSlot = new CustomItemSlot(ItemSlot.Context.EquipDye, 0.85f) {
                IsValidItem = item => item.dye > 0
            };

            Append(EquipSlot);
            Append(VanitySlot);
            Append(DyeSlot);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            Vector2 pos = CalculatePosition();

            EquipSlot.Left.Set(pos.X, EquipSlot.Left.Percent);
            EquipSlot.Top.Set(pos.Y, EquipSlot.Top.Percent);

            VanitySlot.Left.Set(pos.X - 47, VanitySlot.Left.Percent);
            VanitySlot.Top.Set(pos.Y, VanitySlot.Top.Percent);

            DyeSlot.Left.Set(pos.X - (47 * 2), DyeSlot.Left.Percent);
            DyeSlot.Top.Set(pos.Y, DyeSlot.Top.Percent);

            base.DrawSelf(spriteBatch);
        }

        private Vector2 CalculatePosition() {
            int mapH = 0;
            int rX;
            int rY;

            if(Main.mapEnabled) {
                if(!Main.mapFullscreen && Main.mapStyle == 1) {
                    mapH = 256;
                }
            }

            if(!ModContent.GetInstance<WingSlotConfig>().SlotsNextToAccessories) {
                if(Main.mapEnabled) {
                    if((mapH + 600) > Main.screenHeight) {
                        mapH = Main.screenHeight - 600;
                    }
                }

                rX = Main.screenWidth - 92 - (47 * 2);
                rY = mapH + 174;

                if(Main.netMode == 1) {
                    rX -= 47;
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

                rX = Main.screenWidth - 92 - 14 - (47 * 3) - (int)(Main.extraTexture[58].Width * EquipSlot.Scale);
                rY = (int)(mapH + 174 + 4 + slotCount * 56 * EquipSlot.Scale);
            }

            return new Vector2(rX, rY);
        }
    }
}
