using Terraria;
using Terraria.ModLoader;
using UtilitySlots.UI;

namespace UtilitySlots {
    internal class GlobalWingItem : GlobalItem {
        public override bool CanEquipAccessory(Item item, Player player, int slot) {
            if(UtilitySlots.WingSlotModInstalled || item.wingSlot <= 0) return base.CanEquipAccessory(item, player, slot);

            WingSlotUI ui = UtilitySlots.WingUI;

            return ui.Panel.ContainsPoint(Main.MouseScreen) ||
                   (UtilitySlotsConfig.Instance.AllowAccessorySlots && UtilitySlots.WingUI.EquipSlot.Item.IsAir);

        }

        public override bool CanRightClick(Item item) {
            if (UtilitySlots.WingSlotModInstalled) return base.CanRightClick(item);

            return item.wingSlot > 0 &&
                   !UtilitySlots.OverrideRightClick() &&
                   (!UtilitySlotsConfig.Instance.AllowAccessorySlots ||
                    !UtilitySlots.WingUI.EquipSlot.Item.IsAir ||
                    Main.LocalPlayer.wingTimeMax == 0);
        }

        public override void RightClick(Item item, Player player) {
            if(UtilitySlots.WingSlotModInstalled || item.wingSlot <= 0) return;

            player.GetModPlayer<UtilitySlotsPlayer>().EquipItem(
                item,
                UtilitySlotsPlayer.UtilityType.Wing,
                KeyboardUtils.Shift ? UtilitySlotsPlayer.EquipType.Social : UtilitySlotsPlayer.EquipType.Accessory,
                true);
        }
    }

    internal class GlobalBalloonItem : GlobalItem
    {
        public override bool CanEquipAccessory(Item item, Player player, int slot)
        {
            if (item.balloonSlot <= 0) return base.CanEquipAccessory(item, player, slot);

            BalloonSlotUI ui = UtilitySlots.BalloonUI;

            return ui.Panel.ContainsPoint(Main.MouseScreen) ||
                   (UtilitySlotsConfig.Instance.AllowAccessorySlots && UtilitySlots.BalloonUI.EquipSlot.Item.balloonSlot > 0);

        }

        public override bool CanRightClick(Item item)
        {
            return item.balloonSlot > 0 &&
                   !UtilitySlots.OverrideRightClick() &&
                   (!UtilitySlotsConfig.Instance.AllowAccessorySlots ||
                    !(UtilitySlots.BalloonUI.EquipSlot.Item.balloonSlot > 0));
        }

        public override void RightClick(Item item, Player player)
        {
            if (item.balloonSlot <= 0) return;

            player.GetModPlayer<UtilitySlotsPlayer>().EquipItem(
                item,
                UtilitySlotsPlayer.UtilityType.Balloon,
                KeyboardUtils.Shift ? UtilitySlotsPlayer.EquipType.Social : UtilitySlotsPlayer.EquipType.Accessory,
                true);
        }
    }
    internal class GlobalShoeItem : GlobalItem
    {
        public override bool CanEquipAccessory(Item item, Player player, int slot)
        {
            if (item.shoeSlot <= 0) return base.CanEquipAccessory(item, player, slot);

            ShoeSlotUI ui = UtilitySlots.ShoeUI;

            return ui.Panel.ContainsPoint(Main.MouseScreen) ||
                   (UtilitySlotsConfig.Instance.AllowAccessorySlots && UtilitySlots.ShoeUI.EquipSlot.Item.shoeSlot > 0);

        }

        public override bool CanRightClick(Item item)
        {
            return item.shoeSlot > 0 &&
                   !UtilitySlots.OverrideRightClick() &&
                   (!UtilitySlotsConfig.Instance.AllowAccessorySlots ||
                    !(UtilitySlots.ShoeUI.EquipSlot.Item.shoeSlot > 0));
        }

        public override void RightClick(Item item, Player player)
        {
            if (item.shoeSlot <= 0) return;

            player.GetModPlayer<UtilitySlotsPlayer>().EquipItem(
                item,
                UtilitySlotsPlayer.UtilityType.Shoe,
                KeyboardUtils.Shift ? UtilitySlotsPlayer.EquipType.Social : UtilitySlotsPlayer.EquipType.Accessory,
                true);
        }
    }
}
