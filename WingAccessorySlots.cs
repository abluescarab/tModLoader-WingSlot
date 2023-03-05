using CustomSlot.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace WingSlot {
    [Autoload(true)]
    public class WingAccessorySlots : DraggableAccessorySlots {
        public override AccessorySlotsUI UI => WingSlotSystem.UI;
        public override string FunctionalTexture => "Terraria/Images/Item_" + ItemID.AngelWings;
        public override string VanityTexture => "Terraria/Images/Item_" + ItemID.RedsWings;

        public override bool UseCustomLocation => ModContent.GetInstance<WingSlotConfig>().SlotLocation == WingSlotConfig.Location.Custom;

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
    }
}
