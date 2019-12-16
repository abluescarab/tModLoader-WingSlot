using System;
using CustomSlot;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        private const string HiddenTag = "hidden";
        private const string WingsTag = "wings";
        private const string VanityWingsTag = "vanitywings";
        private const string WingDyeTag = "wingdye";

        //public override void clientClone(ModPlayer clientClone) {
        //    WingSlotPlayer clone = clientClone as WingSlotPlayer;

        //    if(clone == null) {
        //        return;
        //    }

        //    clone.WingSlotUI.EquipSlot.Item = WingSlotUI.EquipSlot.Item.Clone();
        //    clone.WingSlotUI.VanitySlot.Item = WingSlotUI.Item.Clone();
        //    clone.WingSlotUI.DyeSlot.Item = WingSlotUI.DyeSlot.Item.Clone();
        //}

        //public override void SendClientChanges(ModPlayer clientPlayer) {
        //    WingSlotPlayer oldClone = clientPlayer as WingSlotPlayer;

        //    if(oldClone == null) {
        //        return;
        //    }

        //    if(oldClone.WingSlotUI.EquipSlot.Item.IsNotTheSameAs(WingSlotUI.EquipSlot.Item)) {
        //        SendSingleItemPacket(PacketMessageType.EquipSlot, WingSlotUI.EquipSlot.Item, -1, player.whoAmI);
        //    }

        //    if(oldClone.VanitySlot.Item.IsNotTheSameAs(VanitySlot.Item)) {
        //        SendSingleItemPacket(PacketMessageType.VanitySlot, VanitySlot.Item, -1, player.whoAmI);
        //    }

        //    if(oldClone.DyeSlot.Item.IsNotTheSameAs(DyeSlot.Item)) {
        //        SendSingleItemPacket(PacketMessageType.DyeSlot, DyeSlot.Item, -1, player.whoAmI);
        //    }
        //}

        //internal void SendSingleItemPacket(PacketMessageType message, Item item, int toWho, int fromWho) {
        //    ModPacket packet = mod.GetPacket();
        //    packet.Write((byte)message);
        //    packet.Write((byte)player.whoAmI);
        //    ItemIO.Send(item, packet);
        //    packet.Send(toWho, fromWho);
        //}

        //public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        //    ModPacket packet = mod.GetPacket();
        //    packet.Write((byte)PacketMessageType.All);
        //    packet.Write((byte)player.whoAmI);
        //    ItemIO.Send(WingSlotUI.EquipSlot.Item, packet);
        //    //ItemIO.Send(VanitySlot.Item, packet);
        //    //ItemIO.Send(DyeSlot.Item, packet);
        //    packet.Send(toWho, fromWho);
        //}

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            if(ui.DyeSlot.Item.stack > 0 && (ui.EquipSlot.Item.wingSlot > 0 || ui.VanitySlot.Item.wingSlot > 0)) {
                drawInfo.wingShader = ui.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;
            Item wings = ui.EquipSlot.Item;
            Item vanityWings = ui.VanitySlot.Item;

            if(wings.stack > 0) {
                player.VanillaUpdateAccessory(player.whoAmI, wings, !ui.EquipSlot.ItemVisible, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
                player.VanillaUpdateEquip(wings);
            }

            if(vanityWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(vanityWings);
            }
        }

        /// <summary>
        /// Since there is no tModLoader hook in UpdateDyes, we use PreUpdateBuffs which is right after that.
        /// </summary>
        public override void PreUpdateBuffs() {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            // Cleaned up vanilla code
            if(ui.DyeSlot.Item == null) return;

            if(ui.VanitySlot.Item.wingSlot > 0 || (ui.EquipSlot.Item.wingSlot > 0 && ui.EquipSlot.ItemVisible)) {
                player.cWings = ui.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Save the mod settings.
        /// </summary>
        public override TagCompound Save() {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            return new TagCompound {
                { HiddenTag, ui.EquipSlot.ItemVisible },
                { WingsTag, ItemIO.Save(ui.EquipSlot.Item) },
                { VanityWingsTag, ItemIO.Save(ui.VanitySlot.Item) },
                { WingDyeTag, ItemIO.Save(ui.DyeSlot.Item) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            EquipItem(ItemIO.Load(tag.GetCompound(WingsTag)), false, false);
            EquipItem(ItemIO.Load(tag.GetCompound(VanityWingsTag)), true, false);
            EquipItem(ItemIO.Load(tag.GetCompound(WingDyeTag)), false, false);
            ((WingSlot)mod).WingSlotUI.EquipSlot.ItemVisible = tag.GetBool(HiddenTag);
        }

        /// <summary>
        /// Equip either wings or a dye.
        /// </summary>
        /// <param name="item">item to equip</param>
        /// <param name="isVanity">whether the item should be equipped in the vanity slot</param>
        /// <param name="fromInventory">whether the item is being equipped from the inventory</param>
        public void EquipItem(Item item, bool isVanity, bool fromInventory) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;
            CustomItemSlot slot;

            if(item.dye > 0) slot = ui.DyeSlot;
            else if(isVanity) slot = ui.VanitySlot;
            else slot = ui.EquipSlot;

            if(fromInventory) {
                item.favorited = false;

                int fromSlot = Array.FindIndex(player.inventory, i => i == item);

                if(fromSlot < 0) return;

                player.inventory[fromSlot] = slot.Item.Clone();
                Main.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
            }

            slot.Item = item.Clone();
        }

        private bool ShowDye() {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;
            return ui.DyeSlot.Item.stack > 0 && 
                   (ui.VanitySlot.Item.stack > 0 || (ui.EquipSlot.Item.stack > 0 && ui.EquipSlot.ItemVisible));
        }
    }
}
