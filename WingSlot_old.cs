//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.ModLoader;

//namespace WingSlot {
//    public class WingSlot : Mod {
//        internal const string PERMISSION_NAME = "ChangeWingSlotSettings";
//        internal const string PERMISSION_DISPLAY_NAME = "Change Wing Slot Settings";
//        public const string WING_SLOT_BACK_TEX = "WingSlotBackground";
//        public const string SETTINGS_BUTTON_TEX = "SettingsButton";

//        public override void Load() {
//            Properties = new ModProperties() {
//                Autoload = true,
//                AutoloadBackgrounds = true,
//                AutoloadSounds = true
//            };

//            TerraUI.Utilities.UIUtils.Mod = this;
//        }

//        public override void PostDrawInterface(SpriteBatch spriteBatch) {
//            DrawElements(spriteBatch);
//            base.PostDrawInterface(spriteBatch);
//        }

//        public override void PostSetupContent() {
//            Mod cheatSheet = ModLoader.GetMod("CheatSheet");
//            Mod herosMod = ModLoader.GetMod("HEROsMod");

//            if(cheatSheet != null && !Main.dedServ) {
//                SetupCheatSheetIntegration(cheatSheet);
//            }

//            if(herosMod != null) {
//                SetupHerosModIntegration(herosMod);
//            }
//        }

//        private void SetupCheatSheetIntegration(Mod cheatSheet) {
//            cheatSheet.Call("AddButton_Test",
//                this.GetTexture(SETTINGS_BUTTON_TEX),
//                (Action)OpenSettingsAction,
//                (Func<string>)OpenSettingsTooltip);
//        }

//        private void SetupHerosModIntegration(Mod herosMod) {
//            herosMod.Call(
//                "AddPermission",
//                PERMISSION_NAME,
//                PERMISSION_DISPLAY_NAME);

//            if(!Main.dedServ) {
//                herosMod.Call(
//                    "AddSimpleButton",
//                    PERMISSION_NAME,
//                    GetTexture(SETTINGS_BUTTON_TEX),
//                    (Action)OpenSettingsAction,
//                    (Action<bool>)PermissionsChanged,
//                    (Func<string>)OpenSettingsTooltip);
//            }
//        }

//        public void PermissionsChanged(bool hasPermission) {
//            if(!hasPermission) {
//                ResetSettings();
//            }
//        }

//        private void OpenSettingsAction() {
//            WingSlotPlayer mp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
//            mp.ToggleWindow();
//        }

//        private string OpenSettingsTooltip() {
//            return "Wing Slot Settings";
//        }

//        private void ResetSettings() {
//            WingSlotPlayer mp = Main.player[Main.myPlayer].GetModPlayer<WingSlotPlayer>(this);
//            mp.ResetSettings();
//        }

//        /// <summary>
//        /// Whether to draw the UIItemSlots.
//        /// </summary>
//        /// <returns>whether to draw the slots</returns>
//        public bool ShouldDrawSlots() {
//            if(Main.playerInventory && Main.EquipPage == 2) {
//                return true;
//            }

//            return false;
//        }
//    }
//}
