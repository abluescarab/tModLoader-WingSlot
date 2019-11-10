using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace WingSlot {
    public class WingSlot : Mod {
        public const string WingSlotBackTex = "WingSlotBackground";
        public static bool AllowAccessorySlots = false;
        public static bool SlotsNextToAccessories = true;

        private static List<Func<bool>> RightClickOverrides;

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            RightClickOverrides = new List<Func<bool>>();
        }

        public override void Unload() {
            RightClickOverrides.Clear();
            RightClickOverrides = null;
        }

        public override object Call(params object[] args) {
            try {
                string keyword = args[0] as string;

                if(string.IsNullOrEmpty(keyword)) {
                    return null;
                }

                keyword = keyword.ToLower();

                switch(keyword) {
                    case "add":
                    case "remove":
                        // wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to cancel/false to continue */);
                        // These two should be called in PostSetupContent
                        Func<bool> func = args[1] as Func<bool>;

                        if(func == null) return null;

                        if(keyword == "add") {
                            RightClickOverrides.Add(func);
                        }
                        else if(keyword == "remove") {
                            RightClickOverrides.Remove(func);
                        }
                        break;
                    case "getEquip":
                    case "getVanity":
                    case "getVisible":
                        /* Can't use these three in PostSetupContent because EquipSlot is a field in WingSlotPlayer, but
                         * that's not initialized yet, hence why I couldn't make some sort of delegate as an argument
                         * that assigned it */

                        // Item wingItem = (Item)wingSlot.Call(/* "getEquip"/"getVanity"/"getVisible" */, player.whoAmI);
                        // These three should be called on demand
                        int whoAmI = Convert.ToInt32(args[1]);
                        WingSlotPlayer wsp = Main.player[whoAmI].GetModPlayer<WingSlotPlayer>();

                        if(keyword == "getEquip") {
                            return wsp.EquipSlot.Item;
                        }
                        else if(keyword == "getVanity") {
                            return wsp.VanitySlot.Item;
                        }
                        // Returns the item that is responsible for the wings to display on the player (at all times or during flight)
                        else if(keyword == "getVisible") {
                            if(wsp.VanitySlot.Item.wingSlot > 0) {
                                return wsp.VanitySlot.Item;
                            }
                            else {
                                return wsp.EquipSlot.Item;
                            }
                        }
                        break;
                }
            }
            catch {
                return null;
            }

            return null;
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch) {
            WingSlotPlayer wsp = Main.LocalPlayer.GetModPlayer<WingSlotPlayer>();
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
                    Logger.InfoFormat("[Wing Slot] Unknown message type: {0}", message);
                    break;
            }
        }

        public static bool OverrideRightClick() {
            foreach(var func in RightClickOverrides) {
                if(func()) {
                    return true;
                }
            }

            return false;
        }
    }
}
