using System;
using System.Collections.Generic;
using CustomSlot;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WingSlot.UI;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        private const string PanelX = "panelx";
        private const string PanelY = "panely";
        private const string HiddenTag = "hidden";
        private const string WingsTag = "wings";
        private const string VanityWingsTag = "vanitywings";
        private const string WingDyeTag = "wingdye";

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            ModPacket packet = mod.GetPacket();
            packet.Write((byte)PacketMessageType.All);
            packet.Write((byte)player.whoAmI);
            ItemIO.Send(ui.EquipSlot.Item, packet);
            ItemIO.Send(ui.SocialSlot.Item, packet);
            ItemIO.Send(ui.DyeSlot.Item, packet);
            packet.Send(toWho, fromWho);
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            if(ui.DyeSlot.Item.stack > 0 && (ui.EquipSlot.Item.wingSlot > 0 || ui.SocialSlot.Item.wingSlot > 0)) {
                drawInfo.wingShader = ui.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;
            Item wings = ui.EquipSlot.Item;
            Item vanityWings = ui.SocialSlot.Item;

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

            if(ui.SocialSlot.Item.wingSlot > 0 || (ui.EquipSlot.Item.wingSlot > 0 && ui.EquipSlot.ItemVisible)) {
                player.cWings = ui.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Save the mod settings.
        /// </summary>
        public override TagCompound Save() {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            return new TagCompound {
                { PanelX, ui.Panel.Left.Pixels },
                { PanelY, ui.Panel.Top.Pixels },
                { HiddenTag, ui.EquipSlot.ItemVisible },
                { WingsTag, ItemIO.Save(ui.EquipSlot.Item) },
                { VanityWingsTag, ItemIO.Save(ui.SocialSlot.Item) },
                { WingDyeTag, ItemIO.Save(ui.DyeSlot.Item) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            WingSlotUI ui = ((WingSlot)mod).WingSlotUI;

            EquipItem(ItemIO.Load(tag.GetCompound(WingsTag)), false, false);
            EquipItem(ItemIO.Load(tag.GetCompound(VanityWingsTag)), true, false);
            EquipItem(ItemIO.Load(tag.GetCompound(WingDyeTag)), false, false);

            ui.EquipSlot.ItemVisible = tag.GetBool(HiddenTag);
            ui.Panel.Left.Set(tag.GetFloat(PanelX), 0);
            ui.Panel.Top.Set(tag.GetFloat(PanelY), 0);
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
            else if(isVanity) slot = ui.SocialSlot;
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
    }
}
