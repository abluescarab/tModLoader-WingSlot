using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using WingSlot.UI;

namespace WingSlot {
    internal class WingAccessorySlots : ModAccessorySlot {
        private readonly string foregroundTexture = "Terraria/Images/Item_" + ItemID.AngelWings;

        public override string FunctionalTexture => foregroundTexture;
        public override string VanityTexture => foregroundTexture;
        public override Vector2? CustomLocation => GetCustomLocation();

        public override bool CanAcceptItem(Item checkItem, AccessorySlotType context) {
            return checkItem.wingSlot > 0;
        }

        public override bool ModifyDefaultSwapSlot(Item item, int accSlotToSwapTo) {
            return item.wingSlot > 0;
        }

        public override void OnMouseHover(AccessorySlotType context) {
            switch(context) {
                case AccessorySlotType.FunctionalSlot:
                    Main.hoverItemName = Language.GetTextValue("Mods.WingSlot.Wings");
                    break;
                case AccessorySlotType.VanitySlot:
                    Main.hoverItemName = Language.GetTextValue("Mods.WingSlot.SocialWings");
                    break;
            }
        }

        private Vector2? GetCustomLocation() {
            if(WingSlotConfig.Instance.SlotLocation != WingSlotConfig.Location.Custom)
                return null;

            UIPanel panel = WingSlotSystem.UI.Panel;

            return new Vector2(panel.Left.Pixels + panel.PaddingLeft + WingSlotUI.SlotSize * 2 + 6,
                               panel.Top.Pixels + panel.PaddingTop);
        }
    }
}
