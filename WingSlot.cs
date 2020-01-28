using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace WingSlot {
    public class WingSlot : Mod {
        private static List<Func<bool>> _rightClickOverrides;

        private UserInterface _wingSlotInterface;

        public WingSlotUI WingSlotUI;

        public override void Load() {
            _rightClickOverrides = new List<Func<bool>>();

            if(!Main.dedServ) {
                _wingSlotInterface = new UserInterface();
                WingSlotUI = new WingSlotUI();

                WingSlotUI.Activate();
                _wingSlotInterface.SetState(WingSlotUI);
            }
        }

        public override void Unload() {
            if(_rightClickOverrides == null) return;

            _rightClickOverrides.Clear();
            _rightClickOverrides = null;
        }

        public override void UpdateUI(GameTime gameTime) {
            if(WingSlotUI.IsVisible) {
                _wingSlotInterface?.Update(gameTime);
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
                          if(WingSlotUI.IsVisible) {
                              _wingSlotInterface.Draw(Main.spriteBatch, new GameTime());
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

                switch(keyword) {
                    case "getConfig":
                        return new Dictionary<string, object> {
                            { "SlotsNextToAccessories", WingSlotConfig.Instance.SlotsNextToAccessories },
                            { "AllowAccessorySlots", WingSlotConfig.Instance.AllowAccessorySlots }
                        };
                    default:
                        return "Error: not a valid command";
                }
            }
            catch {
                return null;
            }
        }

        //public override object Call(params object[] args) {
        //    try {
        //        string keyword = args[0] as string;

        //        if(string.IsNullOrEmpty(keyword)) {
        //            return null;
        //        }

        //        switch(keyword) {
        //            case "add":
        //            case "remove":
        //                // wingSlot.Call(/* "add" or "remove" */, /* func<bool> returns true to cancel/false to continue */);
        //                // These two should be called in PostSetupContent
        //                Func<bool> func = args[1] as Func<bool>;

        //                if(func == null) return null;

        //                switch(keyword) {
        //                    case "add":
        //                        _rightClickOverrides.Add(func);
        //                        break;
        //                    case "remove":
        //                        _rightClickOverrides.Remove(func);
        //                        break;
        //                }

        //                break;
        //            case "getEquip":
        //            case "getVanity":
        //            case "getVisible":
        //                /* Can't use these three in PostSetupContent because EquipSlot is a field in WingSlotPlayer, but
        //                 * that's not initialized yet, hence why I couldn't make some sort of delegate as an argument
        //                 * that assigned it */

        //                // Item wingItem = (Item)wingSlot.Call(/* "getEquip"/"getVanity"/"getVisible" */, player.whoAmI);
        //                // These three should be called on demand
        //                int whoAmI = Convert.ToInt32(args[1]);
        //                WingSlotPlayer wsp = Main.player[whoAmI].GetModPlayer<WingSlotPlayer>();

        //                switch(keyword) {
        //                    case "getEquip":
        //                        return wsp.EquipSlot.Item;
        //                    case "getVanity":
        //                        return wsp.VanitySlot.Item;
        //                    // Returns the item that is responsible for the wings to display on the player (at all times or during flight)
        //                    case "getVisible":
        //                        return wsp.VanitySlot.Item.stack > 0 ? wsp.VanitySlot.Item : wsp.EquipSlot.Item;
        //                }

        //                break;
        //        }
        //    }
        //    catch {
        //        return null;
        //    }

        //    return null;
        //}

        //public override void HandlePacket(BinaryReader reader, int whoAmI) {
        //    PacketMessageType message = (PacketMessageType)reader.ReadByte();
        //    byte player = reader.ReadByte();
        //    WingSlotPlayer modPlayer = Main.player[player].GetModPlayer<WingSlotPlayer>();

        //    switch(message) {
        //        case PacketMessageType.All:
        //            modPlayer.EquipSlot.Item = ItemIO.Receive(reader);
        //            modPlayer.VanitySlot.Item = ItemIO.Receive(reader);
        //            modPlayer.DyeSlot.Item = ItemIO.Receive(reader);
        //            if(Main.netMode == 2) {
        //                ModPacket packet = GetPacket();
        //                packet.Write((byte)PacketMessageType.All);
        //                packet.Write(player);
        //                ItemIO.Send(modPlayer.EquipSlot.Item, packet);
        //                ItemIO.Send(modPlayer.VanitySlot.Item, packet);
        //                ItemIO.Send(modPlayer.DyeSlot.Item, packet);
        //                packet.Send(-1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.EquipSlot:
        //            modPlayer.EquipSlot.Item = ItemIO.Receive(reader);
        //            if(Main.netMode == 2) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.EquipSlot, modPlayer.EquipSlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.VanitySlot:
        //            modPlayer.VanitySlot.Item = ItemIO.Receive(reader);
        //            if(Main.netMode == 2) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.VanitySlot, modPlayer.VanitySlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.DyeSlot:
        //            modPlayer.DyeSlot.Item = ItemIO.Receive(reader);
        //            if(Main.netMode == 2) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.DyeSlot, modPlayer.DyeSlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        default:
        //            Logger.InfoFormat("[Wing Slot] Unknown message type: {0}", message);
        //            break;
        //    }
        //}

        public static bool OverrideRightClick() {
            foreach(var func in _rightClickOverrides) {
                if(func()) {
                    return true;
                }
            }

            return false;
        }
    }
}
