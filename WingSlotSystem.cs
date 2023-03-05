using CustomSlot.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace WingSlot {
    public class WingSlotSystem : ModSystem {
        public static AccessorySlotsUI UI;
        private UserInterface wingSlotInterface;

        public override void Load() {
            if(!Main.dedServ) {
                wingSlotInterface = new UserInterface();
                UI = new AccessorySlotsUI();

                UI.Activate();
                wingSlotInterface.SetState(UI);
            }
        }

        public override void OnWorldLoad() {
            UI.PanelLocation = WingSlotConfig.Instance.SlotLocation;
            UI.EquipSlot.ItemChanged += ItemChanged;
            UI.SocialSlot.ItemChanged += ItemChanged;
            UI.DyeSlot.ItemChanged += ItemChanged;
            UI.EquipSlot.ItemVisibilityChanged += ItemVisibilityChanged;
        }

        public override void OnWorldUnload() {
            UI.EquipSlot.ItemChanged -= ItemChanged;
            UI.SocialSlot.ItemChanged -= ItemChanged;
            UI.DyeSlot.ItemChanged -= ItemChanged;
            UI.EquipSlot.ItemVisibilityChanged -= ItemVisibilityChanged;
        }

        public override void Unload() {
            UI = null;
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

        private void ItemChanged(CustomItemSlot slot, CustomSlot.ItemChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemChanged(slot, e);
        }

        private void ItemVisibilityChanged(CustomItemSlot slot, CustomSlot.ItemVisibilityChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<WingSlotPlayer>().ItemVisibilityChanged(slot, e);
        }
    }
}
