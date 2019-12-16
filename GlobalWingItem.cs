using Terraria;
using Terraria.ModLoader;

namespace WingSlot {
    internal class GlobalWingItem : GlobalItem {
        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            return item.wingSlot > 0 ? ModContent.GetInstance<WingSlotConfig>().AllowAccessorySlots :
                base.CanEquipAccessory(item, player, slot);
        }

        public override bool CanRightClick(Item item) {
            return (item.wingSlot > 0 && !WingSlot.OverrideRightClick());
        }

        public override void RightClick(Item item, Player player) {
            if(!CanRightClick(item)) {
                return;
            }

            WingSlotPlayer mp = player.GetModPlayer<WingSlotPlayer>();
            mp.EquipItem(item, KeyboardUtils.Shift, true);
        }
    }
}
