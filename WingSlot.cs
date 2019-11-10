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
        public const string AllowAccessorySlots = "allowWingsInAccessorySlots";
        public const string SlotLocation = "slotLocation";
        public const string WingSlotBackTex = "WingSlotBackground";
        public static ModConfig Config;

        private static List<Func<bool>> RightClickOverrides;

        public override void Load() {
            Properties = new ModProperties() {
                Autoload = true,
                AutoloadBackgrounds = true,
                AutoloadSounds = true
            };

            Config = new ModConfig("WingSlot");
            RightClickOverrides = new List<Func<bool>>();

            Config.Add(AllowAccessorySlots, false);
            Config.Add(SlotLocation, 1);
            Config.Load();
        }

        public override void Unload() {
            RightClickOverrides.Clear();
            RightClickOverrides = null;
            Config = null;
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
                        //wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to cancel/false to continue normal execution */);
                        //These two should be called in PostSetupContent
                        Func<bool> func = args[1] as Func<bool>;
                        if(func == null) return null;
                        if(keyword == "add") {
                            RightClickOverrides.Add(func);
                        }
                        else if(keyword == "remove") {
                            RightClickOverrides.Remove(func);
                        }
                        break;
                    case "getequipslotitem":
                    case "getvanityslotitem":
                    case "getvisibleitem":
                        //Can't use these three in PostSetupContent because EquipSlot is a field in WingSlotPlayer, but that's not initialized yet
                        //Hence why I couldn't make some sort of delegate as an argument that assigned it

                        //Item wingItem = (Item)wingSlot.Call(/* "getequipslotitem" or "getvanityslotitem" or "getdisplayeditem"*/);
                        //These three should be called on demand
                        WingSlotPlayer wsp = Main.LocalPlayer.GetModPlayer<WingSlotPlayer>();
                        if(keyword == "getequipslotitem") {
                            return wsp.EquipSlot.Item;
                        }
                        else if(keyword == "getvanityslotitem") {
                            return wsp.VanitySlot.Item;
                        }
                        //Returns the item that is responsible for the wings to display on the player (at all times or during flight)
                        else if(keyword == "getvisibleitem") {
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
