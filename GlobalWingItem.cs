using Terraria;
using Terraria.ModLoader;
using WingSlot.UI;

namespace WingSlot {
    internal class GlobalWingItem : GlobalItem {
        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            if(item.wingSlot > 0) {
                WingSlotUI ui = WingSlot.UI;

                return ui.Panel.ContainsPoint(Main.MouseScreen) ||
                       (WingSlotConfig.Instance.AllowAccessorySlots && WingSlot.UI.EquipSlot.Item.IsAir);
            }
            
            return base.CanEquipAccessory(item, player, slot);
        }

        public override bool CanRightClick(Item item) {
            return item.wingSlot > 0 &&
                   !WingSlot.OverrideRightClick() &&
                   (!WingSlotConfig.Instance.AllowAccessorySlots ||
                    !WingSlot.UI.EquipSlot.Item.IsAir ||
                    Main.LocalPlayer.wingTimeMax == 0);
        }

        public override void RightClick(Item item, Player player) {
            if(item.wingSlot > 0) {
                player.GetModPlayer<WingSlotPlayer>().EquipItem(
                    item,
                    KeyboardUtils.Shift ? WingSlotPlayer.EquipType.Social : WingSlotPlayer.EquipType.Accessory,
                    true);
            }
        }
    }
}
