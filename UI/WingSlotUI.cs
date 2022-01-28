using CustomSlot;
using CustomSlot.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WingSlot.UI {
    public class WingSlotUI : AccessorySlotsUI {
        public override void OnInitialize() {
            base.OnInitialize();

            WingSlot mod = ModContent.GetInstance<WingSlot>();
            CroppedTexture2D emptyTexture = new CroppedTexture2D(mod.GetTexture("WingSlotBackground"),
                                                                 CustomItemSlot.DefaultColors.EmptyTexture);

            EquipSlot.IsValidItem = item => item.wingSlot > 0;
            EquipSlot.EmptyTexture = emptyTexture;
            EquipSlot.HoverText = Language.GetTextValue("Mods.WingSlot.Wings");

            SocialSlot.IsValidItem = item => item.wingSlot > 0;
            SocialSlot.EmptyTexture = emptyTexture;
            SocialSlot.HoverText = Language.GetTextValue("Mods.WingSlot.SocialWings");

            EquipSlot.ItemChanged += ItemChanged;
            SocialSlot.ItemChanged += ItemChanged;
            DyeSlot.ItemChanged += ItemChanged;
            EquipSlot.ItemVisibilityChanged += ItemVisibilityChanged;
        }

        protected override Vector2 CalculatePosition() {
            if(PanelLocation == Location.Accessories)
                RowsToSkip = 7;
            else if(PanelLocation == Location.Uniques)
                RowsToSkip = 4;

            return base.CalculatePosition();
        }

        internal void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemChanged(slot, e);
        }

        internal void ItemVisibilityChanged(CustomItemSlot slot, ItemVisibilityChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemVisibilityChanged(slot, e);
        }

        internal void Unload() {
            EquipSlot.ItemChanged -= ItemChanged;
            SocialSlot.ItemChanged -= ItemChanged;
            DyeSlot.ItemChanged -= ItemChanged;
            EquipSlot.ItemVisibilityChanged -= ItemVisibilityChanged;
        }
    }
}
