using System;
using System.Collections.Generic;
using System.IO;
using CustomSlot;
using CustomSlot.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using UtilitySlots.UI;

namespace UtilitySlots {
    public class UtilitySlots : Mod {
        private static List<Func<bool>> rightClickOverrides;

        private UserInterface wingSlotInterface;
        private UserInterface balloonSlotInterface;
        private UserInterface shoeSlotInterface;

        public static WingSlotUI WingUI;
        public static BalloonSlotUI BalloonUI;
        public static ShoeSlotUI ShoeUI;

        public static bool WingSlotModInstalled;

        public override void Load() {
            rightClickOverrides = new List<Func<bool>>();
            WingSlotModInstalled = ModLoader.GetMod("WingSlot") != null;

            if(!Main.dedServ) {
                if (!WingSlotModInstalled)
                {
                    wingSlotInterface = new UserInterface();
                    WingUI = new WingSlotUI();
                    WingUI.Activate();
                    wingSlotInterface.SetState(WingUI);
                }

                balloonSlotInterface = new UserInterface();
                BalloonUI = new BalloonSlotUI();
                BalloonUI.Activate();
                balloonSlotInterface.SetState(BalloonUI);

                shoeSlotInterface = new UserInterface();
                ShoeUI = new ShoeSlotUI();
                ShoeUI.Activate();
                shoeSlotInterface.SetState(ShoeUI);
            }
        }

        public override void Unload()
        {
            if (!WingSlotModInstalled)
                WingUI.Unload();
            BalloonUI.Unload();
            ShoeUI.Unload();

            if(rightClickOverrides == null) return;

            rightClickOverrides.Clear();
            rightClickOverrides = null;
        }

        public override void UpdateUI(GameTime gameTime) {
            if (!WingSlotModInstalled)
                if (WingUI.IsVisible)
                    wingSlotInterface?.Update(gameTime);
            if(BalloonUI.IsVisible)
                balloonSlotInterface?.Update(gameTime);
            if(ShoeUI.IsVisible)
                shoeSlotInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            
            if (inventoryLayer != -1) {
                if (!WingSlotModInstalled)
                {
                    layers.Insert(
                        inventoryLayer,
                        new LegacyGameInterfaceLayer(
                          "Wing Slot: Custom Slot UI",
                          () =>
                          {
                              if (WingUI.IsVisible)
                                  wingSlotInterface.Draw(Main.spriteBatch, new GameTime());
                              return true;
                          },
                          InterfaceScaleType.UI));
                }
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                      "Balloon Slot: Custom Slot UI",
                      () => {
                          if (BalloonUI.IsVisible)
                              balloonSlotInterface.Draw(Main.spriteBatch, new GameTime());
                          return true;
                      },
                      InterfaceScaleType.UI));
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                      "Shoe Slot: Custom Slot UI",
                      () => {
                          if (ShoeUI.IsVisible)
                              shoeSlotInterface.Draw(Main.spriteBatch, new GameTime());
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
                            { "AllowAccessorySlots", UtilitySlotsConfig.Instance.AllowAccessorySlots },
                            //{ "SlotLocation", UtilitySlotsConfig.Instance.SlotLocation },
                            //{ "ShowCustomLocationPanel", UtilitySlotsConfig.Instance.ShowCustomLocationPanel }
                        };
                    case "getwingequip":
                        return WingSlotModInstalled ? null : WingUI.EquipSlot.Item;
                    case "getwingvanity":
                    case "getwingsocial":
                        return WingSlotModInstalled ? null : WingUI.SocialSlot.Item;
                    case "getwingdye":
                        return WingSlotModInstalled ? null : WingUI.DyeSlot.Item;
                    case "getwingvisible":
                        return WingSlotModInstalled ? null : WingUI.SocialSlot.Item.stack > 0 ? WingUI.SocialSlot.Item
                                                                    : WingUI.EquipSlot.Item;
                    case "getballoonequip":
                        return BalloonUI.EquipSlot.Item;
                    case "getballoonvanity":
                    case "getballoonsocial":
                        return BalloonUI.SocialSlot.Item;
                    case "getballoondye":
                        return BalloonUI.DyeSlot.Item;
                    case "getballoonvisible":
                        return BalloonUI.SocialSlot.Item.stack > 0 ? BalloonUI.SocialSlot.Item
                                                                    : BalloonUI.EquipSlot.Item;
                    case "getshoeequip":
                        return ShoeUI.EquipSlot.Item;
                    case "getshoevanity":
                    case "getshoesocial":
                        return ShoeUI.SocialSlot.Item;
                    case "getshoedye":
                        return ShoeUI.DyeSlot.Item;
                    case "getshoevisible":
                        return ShoeUI.SocialSlot.Item.stack > 0 ? ShoeUI.SocialSlot.Item
                                                                    : ShoeUI.EquipSlot.Item;
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

        //public override void HandlePacket(BinaryReader reader, int whoAmI) {
        //    PacketMessageType message = (PacketMessageType)reader.ReadByte();
        //    byte player = reader.ReadByte();
        //    WingSlotPlayer modPlayer = Main.player[player].GetModPlayer<WingSlotPlayer>();

        //    switch(message) {
        //        case PacketMessageType.All:
        //            UI.EquipSlot.SetItem(ItemIO.Receive(reader), false);
        //            UI.SocialSlot.SetItem(ItemIO.Receive(reader), false);
        //            UI.DyeSlot.SetItem(ItemIO.Receive(reader), false);

        //            if(Main.netMode == NetmodeID.Server) {
        //                ModPacket packet = GetPacket();
        //                packet.Write((byte)PacketMessageType.All);
        //                packet.Write(player);
        //                ItemIO.Send(UI.EquipSlot.Item, packet);
        //                ItemIO.Send(UI.SocialSlot.Item, packet);
        //                ItemIO.Send(UI.DyeSlot.Item, packet);
        //                packet.Send(-1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.EquipSlot:
        //            UI.EquipSlot.SetItem(ItemIO.Receive(reader), false);
        //            if(Main.netMode == NetmodeID.Server) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.EquipSlot, UI.EquipSlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.VanitySlot:
        //            UI.SocialSlot.SetItem(ItemIO.Receive(reader), false);
        //            if(Main.netMode == NetmodeID.Server) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.VanitySlot, UI.SocialSlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        case PacketMessageType.DyeSlot:
        //            UI.DyeSlot.SetItem(ItemIO.Receive(reader), false);
        //            if(Main.netMode == NetmodeID.Server) {
        //                modPlayer.SendSingleItemPacket(PacketMessageType.DyeSlot, UI.DyeSlot.Item, -1, whoAmI);
        //            }
        //            break;
        //        default:
        //            Logger.InfoFormat("[Wing Slot] Unknown message type: {0}", message);
        //            break;
        //    }
        //}

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
