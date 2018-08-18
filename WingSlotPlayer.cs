using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using TerraUI.Objects;

namespace WingSlot {
    public class WingSlotPlayer : ModPlayer {
        private const string HIDDEN_TAG = "hidden";
        private const string WINGS_TAG = "wings";
        private const string VANITY_WINGS_TAG = "vanitywings";
        private const string WING_DYE_TAG = "wingdye";
        private const string WING_DYE_LAYER = "WingDye";

        public UIItemSlot EquipSlot;
        public UIItemSlot VanitySlot;
        public UIItemSlot DyeSlot;

        public override void clientClone(ModPlayer clientClone) {
            WingSlotPlayer clone = clientClone as WingSlotPlayer;

            if(clone != null) {
                clone.EquipSlot.Item = EquipSlot.Item.Clone();
                clone.VanitySlot.Item = VanitySlot.Item.Clone();
                clone.DyeSlot.Item = DyeSlot.Item.Clone();
            }
        }

        public override void SendClientChanges(ModPlayer clientPlayer) {
            WingSlotPlayer oldClone = clientPlayer as WingSlotPlayer;

            if(oldClone.EquipSlot.Item.IsNotTheSameAs(EquipSlot.Item)) {
                SendSingleItemPacket(PacketMessageType.EquipSlot, EquipSlot.Item, -1, player.whoAmI);
            }

            if(oldClone.VanitySlot.Item.IsNotTheSameAs(VanitySlot.Item)) {
                SendSingleItemPacket(PacketMessageType.VanitySlot, VanitySlot.Item, -1, player.whoAmI);
            }

            if(oldClone.DyeSlot.Item.IsNotTheSameAs(DyeSlot.Item)) {
                SendSingleItemPacket(PacketMessageType.DyeSlot, DyeSlot.Item, -1, player.whoAmI);
            }
        }

        internal void SendSingleItemPacket(PacketMessageType message, Item item, int toWho, int fromWho) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)message);
            packet.Write((byte)player.whoAmI);
            ItemIO.Send(item, packet);
            packet.Send(toWho, fromWho);
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
            ModPacket packet = mod.GetPacket();
            packet.Write((byte)PacketMessageType.All);
            packet.Write((byte)player.whoAmI);
            ItemIO.Send(EquipSlot.Item, packet);
            ItemIO.Send(VanitySlot.Item, packet);
            ItemIO.Send(DyeSlot.Item, packet);
            packet.Send(toWho, fromWho);
        }

        /// <summary>
        /// Initialize the ModPlayer.
        /// </summary>
        public override void Initialize() {
            EquipSlot = new UIItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipAccessory, hoverText: "Wings",
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground, scaleToInventory: true);
            VanitySlot = new UIItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipAccessoryVanity, hoverText:
                Language.GetTextValue("LegacyInterface.11") + " Wings",
                conditions: Slot_Conditions, drawBackground: Slot_DrawBackground, scaleToInventory: true);
            DyeSlot = new UIDyeItemSlot(Vector2.Zero, context: ItemSlot.Context.EquipDye, conditions: WingDyeSlot_Conditions,
                drawBackground: WingDyeSlot_DrawBackground, scaleToInventory: true);
            VanitySlot.Partner = EquipSlot;
            EquipSlot.BackOpacity = VanitySlot.BackOpacity = DyeSlot.BackOpacity = .8f;

            InitializeWings();
        }

        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            if(DyeSlot.Item != null && !EquipSlot.Item.IsAir && EquipSlot.ItemVisible && EquipSlot.Item.wingSlot > 0) {
                if(EquipSlot.Item.wingSlot > 0)
                    drawInfo.wingShader = DyeSlot.Item.dye;
            }
            if(DyeSlot.Item != null && !VanitySlot.Item.IsAir) {
                if(VanitySlot.Item.wingSlot > 0)
                    drawInfo.wingShader = DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Update player with the equipped wings.
        /// </summary>
        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff) {
            Item wings = EquipSlot.Item;
            Item vanityWings = VanitySlot.Item;

            if(wings.stack > 0) {
                player.VanillaUpdateAccessory(player.whoAmI, wings, !EquipSlot.ItemVisible, ref wallSpeedBuff, ref tileSpeedBuff,
                    ref tileRangeBuff);
            }

            if(vanityWings.stack > 0) {
                player.VanillaUpdateVanityAccessory(vanityWings);
            }
        }

        /// <summary>
        /// Since there is no tModLoader hook in UpdateDyes, we use PreUpdateBuffs which is right after that.
        /// </summary>
        public override void PreUpdateBuffs() {
            // A little redundant code, but mirrors vanilla code exactly.
            if(DyeSlot.Item != null && !EquipSlot.Item.IsAir && EquipSlot.ItemVisible && EquipSlot.Item.wingSlot > 0) {
                if(EquipSlot.Item.wingSlot > 0)
                    player.cWings = DyeSlot.Item.dye;
            }
            if(DyeSlot.Item != null && !VanitySlot.Item.IsAir) {
                if(VanitySlot.Item.wingSlot > 0)
                    player.cWings = DyeSlot.Item.dye;
            }
        }

        /// <summary>
        /// Save the mod settings.
        /// </summary>
        public override TagCompound Save() {
            return new TagCompound {
                { HIDDEN_TAG, EquipSlot.ItemVisible },
                { WINGS_TAG, ItemIO.Save(EquipSlot.Item) },
                { VANITY_WINGS_TAG, ItemIO.Save(VanitySlot.Item) },
                { WING_DYE_TAG, ItemIO.Save(DyeSlot.Item) }
            };
        }

        /// <summary>
        /// Load the mod settings.
        /// </summary>
        public override void Load(TagCompound tag) {
            SetWings(false, ItemIO.Load(tag.GetCompound(WINGS_TAG)));
            SetWings(true, ItemIO.Load(tag.GetCompound(VANITY_WINGS_TAG)));
            SetDye(ItemIO.Load(tag.GetCompound(WING_DYE_TAG)));
            EquipSlot.ItemVisible = tag.GetBool(HIDDEN_TAG);
        }

        /// <summary>
        /// Load legacy mod settings.
        /// </summary>
        public override void LoadLegacy(BinaryReader reader) {
            int hide = 0;

            InitializeWings();

            ushort installedFlag = reader.ReadUInt16();

            if(installedFlag == 0) {
                try { hide = reader.ReadInt32(); }
                catch(EndOfStreamException) { hide = 0; }

                EquipSlot.ItemVisible = (hide == 1 ? false : true);

                Item wings1 = EquipSlot.Item;
                Item wings2 = VanitySlot.Item;

                int context = ReadWingsLegacy(ref wings1, reader);
                ReadWingsLegacy(ref wings2, reader);

                if(context == ItemSlot.Context.EquipAccessory) {
                    SetWings(false, wings1);
                    SetWings(true, wings2);
                }
                else if(context == ItemSlot.Context.EquipAccessoryVanity) {
                    SetWings(true, wings1);
                    SetWings(false, wings2);
                }
            }
        }

        /// <summary>
        /// Read the wings in legacy mod settings.
        /// </summary>
        internal static int ReadWingsLegacy(ref Item wings, BinaryReader reader) {
            try {
                ItemIO.LoadLegacy(wings, reader, false, false);
                return reader.ReadInt32();
            }
            catch(EndOfStreamException) {
                return -1;
            }
        }

        /// <summary>
        /// Draw the wing slot backgrounds.
        /// </summary>
        private void Slot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(ShouldDrawSlots()) {
                slot.OnDrawBackground(spriteBatch);

                if(slot.Item.stack == 0) {
                    Texture2D tex = mod.GetTexture(WingSlot.WING_SLOT_BACK_TEX);
                    Vector2 origin = tex.Size() / 2f * Main.inventoryScale;
                    Vector2 position = slot.Rectangle.TopLeft();

                    spriteBatch.Draw(
                        tex,
                        position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
                        null,
                        Color.White * 0.35f,
                        0f,
                        origin,
                        Main.inventoryScale,
                        SpriteEffects.None,
                        0f); // layer depth 0 = front
                }
            }
        }

        /// <summary>
        /// Control what can be placed in the wing slots.
        /// </summary>
        public bool Slot_Conditions(Item item) {
            if(item.wingSlot > 0) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the wing dye slot background.
        /// </summary>
        private void WingDyeSlot_DrawBackground(UIObject sender, SpriteBatch spriteBatch) {
            UIItemSlot slot = (UIItemSlot)sender;

            if(ShouldDrawSlots()) {
                slot.OnDrawBackground(spriteBatch);

                if(slot.Item.stack == 0) {
                    Texture2D tex = Main.extraTexture[54];
                    Rectangle rectangle = tex.Frame(3, 6, 1 % 3, 1 / 3);
                    rectangle.Width -= 2;
                    rectangle.Height -= 2;
                    Vector2 origin = rectangle.Size() / 2f * Main.inventoryScale;
                    Vector2 position = slot.Rectangle.TopLeft();

                    spriteBatch.Draw(
                        tex,
                        position + (slot.Rectangle.Size() / 2f) - (origin / 2f),
                        new Rectangle?(rectangle),
                        Color.White * 0.35f,
                        0f,
                        origin,
                        Main.inventoryScale,
                        SpriteEffects.None,
                        0f); // layer depth 0 = front
                }
            }
        }

        /// <summary>
        /// Control what can be placed in the wing dye slot.
        /// </summary>
        public bool WingDyeSlot_Conditions(Item item) {
            if(item.dye > 0 && item.hairDye < 0) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the wing slots.
        /// </summary>
        /// <param name="spriteBatch">drawing SpriteBatch</param>
        public void Draw(SpriteBatch spriteBatch) {
            int slotLocation = 1;

            if(ShouldDrawSlots(out slotLocation)) {
                int mapH = 0;
                int rX = 0;
                int rY = 0;
                float origScale = Main.inventoryScale;

                Main.inventoryScale = 0.85f;

                if(Main.mapEnabled) {
                    if(!Main.mapFullscreen && Main.mapStyle == 1) {
                        mapH = 256;
                    }
                }

                if(slotLocation == 2) {
                    if(Main.mapEnabled) {
                        if((mapH + 600) > Main.screenHeight) {
                            mapH = Main.screenHeight - 600;
                        }
                    }

                    rX = Main.screenWidth - 92 - (47 * 2);
                    rY = mapH + 174;

                    if(Main.netMode == 1) {
                        rX -= 47;
                    }
                }
                else {
                    if(Main.mapEnabled) {
                        int adjustY = 600;

                        if(Main.player[Main.myPlayer].ExtraAccessorySlotsShouldShow) {
                            adjustY = 610 + PlayerInput.UsingGamepad.ToInt() * 30;
                        }

                        if((mapH + adjustY) > Main.screenHeight) {
                            mapH = Main.screenHeight - adjustY;
                        }
                    }

                    int slotCount = 7 + Main.player[Main.myPlayer].extraAccessorySlots;

                    if((Main.screenHeight < 900) && (slotCount >= 8)) {
                        slotCount = 7;
                    }

                    rX = Main.screenWidth - 92 - 14 - (47 * 3) - (int)(Main.extraTexture[58].Width * Main.inventoryScale);
                    rY = (int)(mapH + 174 + 4 + slotCount * 56 * Main.inventoryScale);
                }

                EquipSlot.Position = new Vector2(rX, rY);
                VanitySlot.Position = new Vector2(rX -= 47, rY);
                DyeSlot.Position = new Vector2(rX -= 47, rY);

                VanitySlot.Draw(spriteBatch);
                EquipSlot.Draw(spriteBatch);
                DyeSlot.Draw(spriteBatch);

                Main.inventoryScale = origScale;

                EquipSlot.Update();
                VanitySlot.Update();
                DyeSlot.Update();
            }
        }

        /// <summary>
        /// Whether to draw the UIItemSlots.
        /// </summary>
        /// <returns>whether to draw the slots</returns>
        public bool ShouldDrawSlots(out int slotLocation) {
            if(Main.playerInventory) {
                slotLocation = Convert.ToInt32(WingSlot.Config.Get(WingSlot.SLOT_LOCATION));

                if((slotLocation == 1 && Main.EquipPage == 0) ||
                   (slotLocation == 2 && Main.EquipPage == 2)) {
                    return true;
                }
            }

            slotLocation = 1;
            return false;
        }

        /// <summary>
        /// Whether to draw the UIItemSlots.
        /// </summary>
        /// <returns>whether to draw the slots</returns>
        public bool ShouldDrawSlots() {
            int slotLocation = 1;
            return ShouldDrawSlots(out slotLocation);
        }

        /// <summary>
        /// Initialize the items in the UIItemSlots.
        /// </summary>
        private void InitializeWings() {
            EquipSlot.Item = new Item();
            VanitySlot.Item = new Item();
            DyeSlot.Item = new Item();
            EquipSlot.Item.SetDefaults(0, true); // Can remove "0, true" once 0.10.1.5 comes out.
            VanitySlot.Item.SetDefaults(0, true);
            DyeSlot.Item.SetDefaults(0, true);
        }

        /// <summary>
        /// Set the item in the specified slot.
        /// </summary>
        /// <param name="isVanity">whether to equip in the vanity slot</param>
        /// <param name="item">wings</param>
        public void SetWings(bool isVanity, Item item) {
            if(!isVanity) {
                EquipSlot.Item = item.Clone();
            }
            else {
                VanitySlot.Item = item.Clone();
            }
        }

        /// <summary>
        /// Clear the wings from the specified slot.
        /// </summary>
        /// <param name="isVanity">whether to unequip from the vanity slot</param>
        public void ClearWings(bool isVanity) {
            if(!isVanity) {
                EquipSlot.Item = new Item();
                EquipSlot.Item.SetDefaults();
            }
            else {
                VanitySlot.Item = new Item();
                VanitySlot.Item.SetDefaults();
            }
        }

        /// <summary>
        /// Set the wing dye.
        /// </summary>
        /// <param name="item">dye</param>
        public void SetDye(Item item) {
            DyeSlot.Item = item.Clone();
        }

        /// <summary>
        /// Clear the wing dye.
        /// </summary>
        public void ClearDye() {
            DyeSlot.Item = new Item();
            DyeSlot.Item.SetDefaults();
        }

        /// <summary>
        /// Equip a set of wings.
        /// </summary>
        /// <param name="isVanity">whether the wings should go in the vanity slot</param>
        /// <param name="item">wings</param>
        public void EquipWings(bool isVanity, Item item) {
            UIItemSlot slot = (isVanity ? VanitySlot : EquipSlot);
            int fromSlot = Array.FindIndex(player.inventory, i => i == item);

            // from inv to slot
            if(fromSlot > -1) {
                item.favorited = false;
                player.inventory[fromSlot] = slot.Item.Clone();
                Main.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
                SetWings(isVanity, item);
            }
        }

        /// <summary>
        /// Equip a dye.
        /// </summary>
        /// <param name="item">dye to equip</param>
        public void EquipDye(Item item) {
            int fromSlot = Array.FindIndex(player.inventory, i => i == item);

            // from inv to slot
            if(fromSlot > -1) {
                item.favorited = false;
                player.inventory[fromSlot] = DyeSlot.Item.Clone();
                Main.PlaySound(SoundID.Grab);
                Recipe.FindRecipes();
                SetDye(item);
            }
        }

        /// <summary>
        /// Get the set of wings that a dye should be applied to.
        /// </summary>
        /// <returns>dyed wings</returns>
        public Item GetDyedWings() {
            if(VanitySlot.Item.stack > 0) {
                return VanitySlot.Item;
            }
            else if(EquipSlot.Item.stack > 0) {
                return EquipSlot.Item;
            }

            return new Item();
        }
    }
}
