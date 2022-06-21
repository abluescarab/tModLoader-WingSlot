using CustomSlot;
using CustomSlot.UI;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace UtilitySlots.UI {
    public class SlotProps
    {
        public string Tag { get; set; }
        public string TextureName { get; set; }
        public Func<Item, bool> IsValidItem { get; set; }
        public string HoverTextLanguageField { get; set; }
        public string SocialHoverTextLanguageField { get; set; }
        public int AccessoriesRowsToSkip { get; set; }
        public int UniquesRowsToSkip { get; set; }
    }


    //TODO: Figure out how to make it not just balloons, but ballons/cloud in a bottle/horseshoe, and then rename to jump slot
    public class BalloonSlotUI : UtilitySlotUI
    {
        public BalloonSlotUI() : base(new SlotProps
        {
            Tag = "BalloonSlot",
            TextureName = "BalloonSlotBackground",
            IsValidItem = item => item.balloonSlot > 0,
            HoverTextLanguageField = "Mods.UtilitySlots.Balloons",
            SocialHoverTextLanguageField = "Mods.UtilitySlots.SocialBalloons",
            AccessoriesRowsToSkip = 8,
            UniquesRowsToSkip = 5
        })
        { }
    }

    public class WingSlotUI : UtilitySlotUI
    {
        public WingSlotUI() : base(new SlotProps
        {
            Tag = "WingSlot",
            TextureName = "WingSlotBackground",
            IsValidItem = item => item.wingSlot > 0,
            HoverTextLanguageField = "Mods.UtilitySlots.Wings",
            SocialHoverTextLanguageField = "Mods.UtilitySlots.SocialWings",
            AccessoriesRowsToSkip = 7,
            UniquesRowsToSkip = 4
        })
        { }
    }
    public class ShoeSlotUI : UtilitySlotUI
    {
        public ShoeSlotUI() : base(new SlotProps
        {
            Tag = "ShoeSlot",
            TextureName = "ShoeSlotBackground",
            IsValidItem = item => item.shoeSlot > 0,
            HoverTextLanguageField = "Mods.UtilitySlots.Shoes",
            SocialHoverTextLanguageField = "Mods.UtilitySlots.SocialShoes",
            AccessoriesRowsToSkip = 9,
            UniquesRowsToSkip = 6
        }){ }
    }


    public class UtilitySlotUI : AccessorySlotsUI {
        public readonly SlotProps Props;

        public UtilitySlotUI(SlotProps props) : base(props.Tag) {
            Props = props;
        }

        public override void OnInitialize() {
            base.OnInitialize();
            UtilitySlots mod = ModContent.GetInstance<UtilitySlots>();
            CroppedTexture2D emptyTexture = new CroppedTexture2D(mod.GetTexture(Props.TextureName),
                                                                 CustomItemSlot.DefaultColors.EmptyTexture);

            EquipSlot.IsValidItem = Props.IsValidItem;
            EquipSlot.EmptyTexture = emptyTexture;
            EquipSlot.HoverText = Language.GetTextValue(Props.HoverTextLanguageField);

            SocialSlot.IsValidItem = Props.IsValidItem;
            SocialSlot.EmptyTexture = emptyTexture;
            SocialSlot.HoverText = Language.GetTextValue(Props.SocialHoverTextLanguageField);

            EquipSlot.ItemChanged += ItemChanged;
            SocialSlot.ItemChanged += ItemChanged;
            DyeSlot.ItemChanged += ItemChanged;
            EquipSlot.ItemVisibilityChanged += ItemVisibilityChanged;
        }

        protected override Vector2 CalculatePosition() {
            if(PanelLocation == Location.Accessories)
                RowsToSkip = Props.AccessoriesRowsToSkip;
            else if(PanelLocation == Location.Uniques)
                RowsToSkip = Props.UniquesRowsToSkip;

            return base.CalculatePosition();
        }

        internal void ItemChanged(CustomItemSlot slot, ItemChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<UtilitySlotsPlayer>().ItemChanged(slot, e);
        }

        internal void ItemVisibilityChanged(CustomItemSlot slot, ItemVisibilityChangedEventArgs e) {
            Main.LocalPlayer.GetModPlayer<UtilitySlotsPlayer>().ItemVisibilityChanged(slot, e);
        }

        internal void Unload() {
            EquipSlot.ItemChanged -= ItemChanged;
            SocialSlot.ItemChanged -= ItemChanged;
            DyeSlot.ItemChanged -= ItemChanged;
            EquipSlot.ItemVisibilityChanged -= ItemVisibilityChanged;
        }
    }
}
