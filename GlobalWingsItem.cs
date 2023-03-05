using Terraria;
using Terraria.ModLoader;

namespace WingSlot {
    internal class GlobalWingsItem : GlobalItem {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.wingSlot > 0;

        public override bool CanEquipAccessory(Item item, Player player, int slot, bool modded) {
            return modded || ModContent.GetInstance<WingSlotConfig>().AllowEquippingInOtherSlots;
        }
    }
}
