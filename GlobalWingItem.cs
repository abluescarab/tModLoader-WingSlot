﻿using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using TerraUI.Utilities;

namespace WingSlot {
    internal class GlobalWingItem : GlobalItem {
        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            bool allowAccessorySlots = (bool)WingSlot.Config.Get(WingSlot.AllowAccessorySlots);
            return ((item.wingSlot > 0) && allowAccessorySlots) || base.CanEquipAccessory(item, player, slot);
        }

        public override bool CanRightClick(Item item) {
            return (item.wingSlot > 0 && !WingSlot.OverrideRightClick());
        }

        public override void RightClick(Item item, Player player) {
            if(!CanRightClick(item)) {
                return;
            }

            WingSlotPlayer mp = player.GetModPlayer<WingSlotPlayer>();
            mp.EquipWings(KeyboardUtils.HeldDown(Keys.LeftShift), item);
        }
    }
}
