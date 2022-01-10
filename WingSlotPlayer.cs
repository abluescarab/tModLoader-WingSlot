using System;
using CustomSlot;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    internal class WingSlotPlayer : ModPlayer {
        public enum EquipType {
            Accessory,
            Social,
            Dye
        }

        private const string PanelXTag = "panelx";
        private const string PanelYTag = "panely";
        private const string HiddenTag = "hidden";
        private const string WingsTag = "wings";
        private const string SocialWingsTag = "vanitywings";
        private const string WingDyeTag = "wingdye";

        public override void OnEnterWorld(Player player) {
            EquipItem(WingSlot.UI.EquipSlot.Item, EquipType.Accessory, false);
            EquipItem(WingSlot.UI.SocialSlot.Item, EquipType.Social, false);
            EquipItem(WingSlot.UI.DyeSlot.Item, EquipType.Dye, false);
        }

        // TODO: fix sending packets to other players
        //public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
        //    ModPacket packet = mod.GetPacket();
        //    packet.Write((byte)PacketMessageType.All);
        //    packet.Write((byte)player.whoAmI);
        //    ItemIO.Send(WingSlot.UI.EquipSlot.Item, packet);
        //    ItemIO.Send(WingSlot.UI.SocialSlot.Item, packet);
        //    ItemIO.Send(WingSlot.UI.DyeSlot.Item, packet);
        //    packet.Send(toWho, fromWho);
        //}

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            if(WingSlot.UI.DyeSlot.Item.stack > 0 && (WingSlot.UI.EquipSlot.Item.stack > 0 || WingSlot.UI.SocialSlot.Item.stack > 0)) {
                drawInfo.wingShader = WingSlot.UI.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            if(WingSlot.UI.EquipSlot.Item.stack > 0) {
                player.VanillaUpdateAccessory(player.whoAmI, WingSlot.UI.EquipSlot.Item, !WingSlot.UI.EquipSlot.ItemVisible, ref wallSpeedBuff, 
                                              ref tileSpeedBuff, ref tileRangeBuff);
                player.VanillaUpdateEquip(WingSlot.UI.EquipSlot.Item);
            }

            if(WingSlot.UI.SocialSlot.Item.stack > 0) {
                player.VanillaUpdateVanityAccessory(WingSlot.UI.SocialSlot.Item);
            }
        }

        /// <summary>
        /// Since there is no tModLoader hook in UpdateDyes, we use PreUpdateBuffs which is right after that.
        /// </summary>
        public override void PreUpdateBuffs() {
            // Cleaned up vanilla code
            // TODO: throws "object reference not set to instance" error here on joining dedserv
            if(WingSlot.UI.DyeSlot.Item.stack <= 0) 
                return;

            if(WingSlot.UI.SocialSlot.Item.stack > 0 || (WingSlot.UI.EquipSlot.Item.stack > 0 && WingSlot.UI.EquipSlot.ItemVisible)) {
                player.cWings = WingSlot.UI.DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Drop items if the player character is Medium or Hardcore.
        /// </summary>
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
            if(player.difficulty == 0) return;

            player.QuickSpawnClonedItem(WingSlot.UI.EquipSlot.Item);
            player.QuickSpawnClonedItem(WingSlot.UI.SocialSlot.Item);
            player.QuickSpawnClonedItem(WingSlot.UI.DyeSlot.Item);

            WingSlot.UI.EquipSlot.Item = new Item();
            WingSlot.UI.SocialSlot.Item = new Item();
            WingSlot.UI.DyeSlot.Item = new Item();
        }

        /// <summary>
        /// Save player settings.
        /// </summary>
        public override TagCompound Save() {
            return new TagCompound {
                { PanelXTag, WingSlot.UI.Panel.Left.Pixels },
                { PanelYTag, WingSlot.UI.Panel.Top.Pixels },
                { HiddenTag, WingSlot.UI.EquipSlot.ItemVisible },
                { WingsTag, ItemIO.Save(WingSlot.UI.EquipSlot.Item) },
                { SocialWingsTag, ItemIO.Save(WingSlot.UI.SocialSlot.Item) },
                { WingDyeTag, ItemIO.Save(WingSlot.UI.DyeSlot.Item) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            if(tag.ContainsKey(WingsTag))
                WingSlot.UI.EquipSlot.Item = ItemIO.Load(tag.GetCompound(WingsTag));

            if(tag.ContainsKey(SocialWingsTag))
                WingSlot.UI.SocialSlot.Item = ItemIO.Load(tag.GetCompound(SocialWingsTag));

            if(tag.ContainsKey(WingDyeTag))
                WingSlot.UI.DyeSlot.Item = ItemIO.Load(tag.GetCompound(WingDyeTag));

            if(tag.ContainsKey(HiddenTag))
                WingSlot.UI.EquipSlot.ItemVisible = tag.GetBool(HiddenTag);

            if(tag.ContainsKey(PanelXTag))
                WingSlot.UI.Panel.Left.Set(tag.GetFloat(PanelXTag), 0);

            if(tag.ContainsKey(PanelYTag))
                WingSlot.UI.Panel.Top.Set(tag.GetFloat(PanelYTag), 0);
        }

        /// <summary>
        /// Equip either wings or a dye.
        /// </summary>
        /// <param name="item">item to equip</param>
        /// <param name="type">what type of slot to equip in</param>
        /// <param name="fromInventory">whether the item is being equipped from the inventory</param>
        public void EquipItem(Item item, EquipType type, bool fromInventory) {
            if(item == null) return;

            CustomItemSlot slot;

            if(type == EquipType.Dye) slot = WingSlot.UI.DyeSlot;
            else if(type == EquipType.Social) slot = WingSlot.UI.SocialSlot;
            else slot = WingSlot.UI.EquipSlot;

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
