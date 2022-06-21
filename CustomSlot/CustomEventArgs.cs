using System;
using CustomSlot.UI;
using Terraria;

namespace CustomSlot {
    public delegate void ItemChangedEventHandler(CustomItemSlot slot, ItemChangedEventArgs e);
    public delegate void ItemVisiblityChangedEventHandler(CustomItemSlot slot, ItemVisibilityChangedEventArgs e);

    public class ItemChangedEventArgs : EventArgs {
        public readonly Item OldItem;
        public readonly Item NewItem;

        public ItemChangedEventArgs(Item oldItem, Item newItem) {
            OldItem = oldItem;
            NewItem = newItem;
        }
    }

    public class ItemVisibilityChangedEventArgs : EventArgs {
        public readonly bool Visibility;

        public ItemVisibilityChangedEventArgs(bool visibility) {
            Visibility = visibility;
        }
    }
}
