using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModConfiguration;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string ALLOW_ACCESSORY_SLOTS = "allowWingsInAccessorySlots";
        public const string SLOT_LOCATION = "slotLocation";
        public const string WING_SLOT_BACK_TEX = "WingSlotBackground";
        public static readonly ModConfig Config = new ModConfig("WingSlot");

        internal static readonly Dictionary<Mod, Func<bool>> SlotConditionsOverrides = new Dictionary<Mod, Func<bool>>();

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            TerraUI.Utilities.UIUtils.Mod = this;
            TerraUI.Utilities.UIUtils.Subdirectory = "TerraUI";

            Config.Add(ALLOW_ACCESSORY_SLOTS, false);
            Config.Add(SLOT_LOCATION, 1);
            Config.Load();
        }

        public override object Call(params object[] args) {
            Mod mod = args[0] as Mod;
            string keyword = args[1] as string;
            Func<bool> func = args[2] as Func<bool>;
            
            if(mod != null) {
                if(keyword?.ToLower() == "add" && func != null) {
                    SlotConditionsOverrides[mod] = func;
                }
                else if(keyword?.ToLower() == "remove") {
                    SlotConditionsOverrides.Remove(mod);
                }
            }

            return null;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WingSlotPlayer wsp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
            wsp.Draw(spriteBatch);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketMessageType message = (PacketMessageType)reader.ReadByte();
            byte player = reader.ReadByte();
            WingSlotPlayer modPlayer = Main.player[player].GetModPlayer<WingSlotPlayer>();

            switch(message) {
                case PacketMessageType.All:
                    modPlayer.EquipSlot.Item = ItemIO.Receive(reader);
                    modPlayer.VanitySlot.Item = ItemIO.Receive(reader);
                    modPlayer.DyeSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == 2) {
                        ModPacket packet = GetPacket();
                        packet.Write((byte)PacketMessageType.All);
                        packet.Write(player);
                        ItemIO.Send(modPlayer.EquipSlot.Item, packet);
                        ItemIO.Send(modPlayer.VanitySlot.Item, packet);
                        ItemIO.Send(modPlayer.DyeSlot.Item, packet);
                        packet.Send(-1, whoAmI);
                    }
                    break;
                case PacketMessageType.EquipSlot:
                    modPlayer.EquipSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == 2) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.EquipSlot, modPlayer.EquipSlot.Item, -1, whoAmI);
                    }
                    break;
                case PacketMessageType.VanitySlot:
                    modPlayer.VanitySlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == 2) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.VanitySlot, modPlayer.VanitySlot.Item, -1, whoAmI);
                    }
                    break;
                case PacketMessageType.DyeSlot:
                    modPlayer.DyeSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == 2) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.DyeSlot, modPlayer.DyeSlot.Item, -1, whoAmI);
                    }
                    break;
                default:
                    ErrorLogger.Log("Wing Slot: Unknown message type: " + message);
                    break;
            }
        }
    }
}
