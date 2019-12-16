using System;
using Terraria;

namespace CustomSlot {
    public interface ICustomSlot {
        int Context { get; }
        CroppedTexture2D BackgroundTexture { get; set; }
        CroppedTexture2D EmptyTexture { get; set; }
        Item Item { get; set; }
        Func<Item, bool> IsValidItem { get; set; }
        float Scale { get; set; }
    }
}
