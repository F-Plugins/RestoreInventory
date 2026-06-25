using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Feli.RestoreInventory.Models;

public class StoredClothing
{
    public StoredClothingType Type { get; set; }
    public ushort Id { get; set; }
    public byte Quality { get; set; }
    public byte[] State { get; set; }

    public static IEnumerable<StoredClothing> FromPlayer(Player player)
    {
        var clothing = player.clothing;

        if (clothing.shirt != 0)
            yield return new StoredClothing { Type = StoredClothingType.Shirt, Id = clothing.shirt, Quality = clothing.shirtQuality, State = clothing.shirtState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.pants != 0)
            yield return new StoredClothing { Type = StoredClothingType.Pants, Id = clothing.pants, Quality = clothing.pantsQuality, State = clothing.pantsState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.hat != 0)
            yield return new StoredClothing { Type = StoredClothingType.Hat, Id = clothing.hat, Quality = clothing.hatQuality, State = clothing.hatState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.backpack != 0)
            yield return new StoredClothing { Type = StoredClothingType.Backpack, Id = clothing.backpack, Quality = clothing.backpackQuality, State = clothing.backpackState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.vest != 0)
            yield return new StoredClothing { Type = StoredClothingType.Vest, Id = clothing.vest, Quality = clothing.vestQuality, State = clothing.vestState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.mask != 0)
            yield return new StoredClothing { Type = StoredClothingType.Mask, Id = clothing.mask, Quality = clothing.maskQuality, State = clothing.maskState?.ToArray() ?? Array.Empty<byte>() };

        if (clothing.glasses != 0)
            yield return new StoredClothing { Type = StoredClothingType.Glasses, Id = clothing.glasses, Quality = clothing.glassesQuality, State = clothing.glassesState?.ToArray() ?? Array.Empty<byte>() };
    }

    public void Restore(Player player)
    {
        if (Id == 0)
            return;

        switch (Type)
        {
            case StoredClothingType.Shirt when player.clothing.shirt == 0:
                player.clothing.askWearShirt(Id, Quality, State, true);
                return;
            case StoredClothingType.Pants when player.clothing.pants == 0:
                player.clothing.askWearPants(Id, Quality, State, true);
                return;
            case StoredClothingType.Hat when player.clothing.hat == 0:
                player.clothing.askWearHat(Id, Quality, State, true);
                return;
            case StoredClothingType.Backpack when player.clothing.backpack == 0:
                player.clothing.askWearBackpack(Id, Quality, State, true);
                return;
            case StoredClothingType.Vest when player.clothing.vest == 0:
                player.clothing.askWearVest(Id, Quality, State, true);
                return;
            case StoredClothingType.Mask when player.clothing.mask == 0:
                player.clothing.askWearMask(Id, Quality, State, true);
                return;
            case StoredClothingType.Glasses when player.clothing.glasses == 0:
                player.clothing.askWearGlasses(Id, Quality, State, true);
                return;
        }

        StoredInventoryItem.AddOrDrop(player, Id, 1, Quality, State);
    }
}
