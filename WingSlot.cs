using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using WingSlot.UI;

namespace WingSlot {
    public class WingSlot : Mod {
        public static WingSlotUI UI;

        public override void Load() {
            
        }

        public class WingSlotSystem : ModSystem {
            private UserInterface wingSlotInterface;

            public override void Load() {
                if(!Main.dedServ) {
                    wingSlotInterface = new UserInterface();
                    UI = new WingSlotUI();

                    UI.Activate();
                    wingSlotInterface.SetState(UI);
                }
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
        }
    }
}
