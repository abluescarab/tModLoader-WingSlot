using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using WingSlot.UI;

namespace WingSlot {
    public class WingSlot : Mod {
        private static List<Func<bool>> rightClickOverrides;

        private UserInterface wingSlotInterface;

        public static WingSlotUI UI;

        public override void Load() {
            rightClickOverrides = new List<Func<bool>>();

            if(!Main.dedServ) {
                wingSlotInterface = new UserInterface();
                UI = new WingSlotUI();

                UI.Activate();
                wingSlotInterface.SetState(UI);
            }
        }

        public override void Unload() {
            UI = null;

            if(rightClickOverrides == null) return;

            rightClickOverrides.Clear();
            rightClickOverrides = null;
        }

        public override void UpdateUI(GameTime gameTime) {
            if(UI.IsVisible) {
                wingSlotInterface?.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if(inventoryLayer != -1) {
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                      "Wing Slot: Custom Slot UI",
                      () => {
                          if(UI.IsVisible) {
                              wingSlotInterface.Draw(Main.spriteBatch, new GameTime());
                          }

                          return true;
                      },
                      InterfaceScaleType.UI));
            }
        }

        public override object Call(params object[] args) {
            try {
                string keyword = args[0] as string;

                if(string.IsNullOrEmpty(keyword)) {
                    return "Error: no command provided";
                }

                switch(keyword.ToLower()) {
                    case "getconfig":
                        return new Dictionary<string, object> {
                            { "AllowAccessorySlots", WingSlotConfig.Instance.AllowAccessorySlots },
                            { "SlotLocation", WingSlotConfig.Instance.SlotLocation },
                            { "ShowCustomLocationPanel", WingSlotConfig.Instance.ShowCustomLocationPanel }
                        };
                    case "getequip":
                        return UI.EquipSlot.Item;
                    case "getvanity":
                    case "getsocial":
                        return UI.SocialSlot.Item;
                    case "getdye":
                        return UI.DyeSlot.Item;
                    case "getvisible":
                        return UI.SocialSlot.Item.stack > 0 ? UI.SocialSlot.Item
                                                                    : UI.EquipSlot.Item;
                    case "add":
                    case "remove":
                        // wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to cancel/false to continue */);
                        // These two should be called in PostSetupContent
                        if(!(args[1] is Func<bool> func))
                            return "Error: not a valid Func<bool>";

                        if(keyword == "add") {
                            rightClickOverrides.Add(func);
                        }
                        else {
                            rightClickOverrides.Remove(func);
                        }

                        break;
                    default:
                        return "Error: not a valid command";
                }
            }
            catch {
                return null;
            }

            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI) {
            PacketMessageType message = (PacketMessageType)reader.ReadByte();
            byte player = reader.ReadByte();
            WingSlotPlayer modPlayer = Main.player[player].GetModPlayer<WingSlotPlayer>();

            switch(message) {
                case PacketMessageType.All:
                    UI.EquipSlot.Item = ItemIO.Receive(reader);
                    UI.SocialSlot.Item = ItemIO.Receive(reader);
                    UI.DyeSlot.Item = ItemIO.Receive(reader);

                    if(Main.netMode == NetmodeID.Server) {
                        ModPacket packet = GetPacket();
                        packet.Write((byte)PacketMessageType.All);
                        packet.Write(player);
                        ItemIO.Send(UI.EquipSlot.Item, packet);
                        ItemIO.Send(UI.SocialSlot.Item, packet);
                        ItemIO.Send(UI.DyeSlot.Item, packet);
                        packet.Send(-1, whoAmI);
                    }
                    break;
                case PacketMessageType.EquipSlot:
                    UI.EquipSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == NetmodeID.Server) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.EquipSlot, UI.EquipSlot.Item, -1, whoAmI);
                    }
                    break;
                case PacketMessageType.VanitySlot:
                    UI.SocialSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == NetmodeID.Server) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.VanitySlot, UI.SocialSlot.Item, -1, whoAmI);
                    }
                    break;
                case PacketMessageType.DyeSlot:
                    UI.DyeSlot.Item = ItemIO.Receive(reader);
                    if(Main.netMode == NetmodeID.Server) {
                        modPlayer.SendSingleItemPacket(PacketMessageType.DyeSlot, UI.DyeSlot.Item, -1, whoAmI);
                    }
                    break;
                default:
                    Logger.InfoFormat("[Wing Slot] Unknown message type: {0}", message);
                    break;
            }
        }

        public static bool OverrideRightClick() {
            foreach(var func in rightClickOverrides) {
                if(func()) {
                    return true;
                }
            }

            return false;
        }
    }
}
