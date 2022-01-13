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
        private const string WingsDyeTag = "wingdye";

        public Item EquippedWings { get; set; }
        public Item SocialWings { get; set; }
        public Item WingsDye { get; set; }
        public bool WingsVisible { get; set; }

        public override void Initialize() {
            EquippedWings = new Item();
            SocialWings = new Item();
            WingsDye = new Item();
            WingsVisible = true;

            EquippedWings.SetDefaults();
            SocialWings.SetDefaults();
            WingsDye.SetDefaults();
        }

        public override void OnEnterWorld(Player player) {
            WingSlot.UI.EquipSlot.ItemPlaced += (sender, e) => {
                EquippedWings = e.Item.Clone();
            };

            WingSlot.UI.SocialSlot.ItemPlaced += (sender, e) => {
                SocialWings = e.Item.Clone();
            };

            WingSlot.UI.DyeSlot.ItemPlaced += (sender, e) => {
                WingsDye = e.Item.Clone();
            };

            WingSlot.UI.EquipSlot.ItemVisibilityChanged += (sender, e) => {
                WingsVisible = e.Visibility;
            };

            EquipItem(EquippedWings, EquipType.Accessory, false);
            EquipItem(SocialWings, EquipType.Social, false);
            EquipItem(WingsDye, EquipType.Dye, false);
        }

        public override void clientClone(ModPlayer clientClone) {
            WingSlotPlayer clone = clientClone as WingSlotPlayer;

            if(clone == null) {
                return;
            }

            clone.EquippedWings = EquippedWings.Clone();
            clone.SocialWings = SocialWings.Clone();
            clone.WingsDye = WingsDye.Clone();
        }

        public override void SendClientChanges(ModPlayer clientPlayer) {
            WingSlotPlayer oldClone = clientPlayer as WingSlotPlayer;

            if(oldClone == null) {
                return;
            }

            if(oldClone.EquippedWings.IsNotTheSameAs(EquippedWings)) {
                SendSingleItemPacket(PacketMessageType.EquipSlot, EquippedWings, -1, player.whoAmI);
            }

            if(oldClone.SocialWings.IsNotTheSameAs(SocialWings)) {
                SendSingleItemPacket(PacketMessageType.VanitySlot, SocialWings, -1, player.whoAmI);
            }

            if(oldClone.WingsDye.IsNotTheSameAs(WingsDye)) {
                SendSingleItemPacket(PacketMessageType.DyeSlot, WingsDye, -1, player.whoAmI);
            }
        }

        internal void SendSingleItemPacket(PacketMessageType message, Item item, int toWho, int fromWho) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)message);
            packet.Write((byte)player.whoAmI);
            ItemIO.Send(item, packet);
            packet.Send(toWho, fromWho);
        }

        // TODO: fix sending packets to other players
        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)PacketMessageType.All);
            packet.Write((byte)player.whoAmI);
            ItemIO.Send(EquippedWings, packet);
            ItemIO.Send(SocialWings, packet);
            ItemIO.Send(WingsDye, packet);
            packet.Send(toWho, fromWho);
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            if(WingsDye.stack > 0 && (EquippedWings.stack > 0 || SocialWings.stack > 0)) {
                drawInfo.wingShader = WingsDye.dye;
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            if(WingSlot.UI == null) return;

            if(EquippedWings.stack > 0) {
                player.VanillaUpdateAccessory(player.whoAmI, EquippedWings, !WingsVisible, ref wallSpeedBuff, 
                                              ref tileSpeedBuff, ref tileRangeBuff);
                player.VanillaUpdateEquip(EquippedWings);
            }

            if(SocialWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(SocialWings);
            }
        }

        /// <summary>
        /// Since there is no tModLoader hook in UpdateDyes, we use PreUpdateBuffs which is right after that.
        /// </summary>
        public override void PreUpdateBuffs() {
            // Cleaned up vanilla code
            if(WingSlot.UI == null) return;

            if(WingsDye.stack <= 0) 
                return;

            if(SocialWings.stack > 0 || (EquippedWings.stack > 0 && WingsVisible)) {
                player.cWings = WingsDye.dye;
            }
        }

        /// <summary>
        /// Drop items if the player character is Medium or Hardcore.
        /// </summary>
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
            if(player.difficulty == 0) return;

            player.QuickSpawnClonedItem(EquippedWings);
            player.QuickSpawnClonedItem(SocialWings);
            player.QuickSpawnClonedItem(WingsDye);

            EquippedWings = new Item();
            SocialWings = new Item();
            WingsDye = new Item();
        }

        /// <summary>
        /// Save player settings.
        /// </summary>
        public override TagCompound Save() {
            return new TagCompound {
                { PanelXTag, WingSlot.UI.CustomPanelX },
                { PanelYTag, WingSlot.UI.CustomPanelY },
                { HiddenTag, WingsVisible },
                { WingsTag, ItemIO.Save(EquippedWings) },
                { SocialWingsTag, ItemIO.Save(SocialWings) },
                { WingsDyeTag, ItemIO.Save(WingsDye) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            if(tag.ContainsKey(WingsTag))
                EquippedWings = ItemIO.Load(tag.GetCompound(WingsTag));

            if(tag.ContainsKey(SocialWingsTag))
                SocialWings = ItemIO.Load(tag.GetCompound(SocialWingsTag));

            if(tag.ContainsKey(WingsDyeTag))
                WingsDye = ItemIO.Load(tag.GetCompound(WingsDyeTag));

            if(tag.ContainsKey(HiddenTag))
                WingsVisible = tag.GetBool(HiddenTag);

            if(tag.ContainsKey(PanelXTag))
                WingSlot.UI.CustomPanelX = tag.GetFloat(PanelXTag);

            if(tag.ContainsKey(PanelYTag))
                WingSlot.UI.CustomPanelY = tag.GetFloat(PanelYTag);
        }

        /// <summary>
        /// Equip either wings or a dye in a slot.
        /// </summary>
        /// <param name="item">item to equip</param>
        /// <param name="type">what type of slot to equip in</param>
        /// <param name="fromInventory">whether the item is being equipped from the inventory</param>
        public void EquipItem(Item item, EquipType type, bool fromInventory) {
            if(item == null) return;

            CustomItemSlot slot;

            if(type == EquipType.Dye) {
                slot = WingSlot.UI.DyeSlot;

                if(WingsDye.type != item.type)
                    WingsDye = item.Clone();
            }
            else if(type == EquipType.Social) {
                slot = WingSlot.UI.SocialSlot;

                if(SocialWings.type != item.type)
                    SocialWings = item.Clone();
            }
            else {
                slot = WingSlot.UI.EquipSlot;

                if(EquippedWings.type != item.type)
                    EquippedWings = item.Clone();
            }

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
