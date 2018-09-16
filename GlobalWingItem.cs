using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using TerraUI.Utilities;

namespace WingSlot {
    class GlobalWingItem : GlobalItem {
        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            if(!(bool)WingSlot.Config.Get(WingSlot.AllowAccessorySlots) && (item.wingSlot > 0)) {
                return false;
            }

            return base.CanEquipAccessory(item, player, slot);
        }

        public override bool CanRightClick(Item item) {
            if(item.wingSlot > 0) {
                return true;
            }

            return base.CanRightClick(item);
        }

        public override void RightClick(Item item, Player player) {
            if(item.wingSlot > 0) {
                WingSlotPlayer mp = player.GetModPlayer<WingSlotPlayer>(mod);

                if(KeyboardUtils.HeldDown(Keys.LeftShift)) {
                    mp.EquipWings(true, item);
                }
                else {
                    mp.EquipWings(false, item);
                }
            }
            else {
                base.RightClick(item, player);
            }
        }
    }
}
