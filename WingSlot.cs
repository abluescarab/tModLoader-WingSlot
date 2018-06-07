using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModConfiguration;
using Terraria;
using Terraria.ModLoader;
using TerraUI.Objects;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string ALLOW_ACCESSORY_SLOTS = "allowWingsInAccessorySlots";
        public const string SLOT_LOCATION = "slotLocation";
        public const string WING_SLOT_BACK_TEX = "WingSlotBackground";
        public static readonly ModConfig Config = new ModConfig("WingSlot");

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

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WingSlotPlayer wsp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
            wsp.Draw(spriteBatch);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketMessageType message = (PacketMessageType)reader.ReadByte();
            WingSlotPlayer modPlayer = Main.player[reader.ReadInt32()].GetModPlayer<WingSlotPlayer>();
            int item = reader.ReadInt32();

            switch(message) {
                case PacketMessageType.All:
                    HandleItemPacket(ref modPlayer.EquipSlot, item);
                    HandleItemPacket(ref modPlayer.VanitySlot, reader.ReadInt32());
                    HandleItemPacket(ref modPlayer.DyeSlot, reader.ReadInt32());
                    break;
                case PacketMessageType.EquipSlot:
                    HandleItemPacket(ref modPlayer.EquipSlot, item);
                    break;
                case PacketMessageType.VanitySlot:
                    HandleItemPacket(ref modPlayer.VanitySlot, item);
                    break;
                case PacketMessageType.DyeSlot:
                    HandleItemPacket(ref modPlayer.DyeSlot, item);
                    break;
                default:
                    ErrorLogger.Log("Wing Slot: Unknown message type: " + message);
                    break;
            }
        }

        private void HandleItemPacket(ref UIItemSlot slot, int item) {
            if(slot.Item.whoAmI != item) {
                slot.Item.CloneDefaults(item);
            }
        }
    }
}
