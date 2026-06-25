using System;
using System.Collections.Generic;
using System.Linq;
using SDG.Unturned;

namespace Feli.RestoreInventory.Models;

public class StoredInventory
{
    public int Id { get; set; }
    public ulong PlayerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<StoredInventoryItem> Items { get; set; } = new();
    public List<StoredClothing> Clothing { get; set; } = new();

    public int ItemCount => Items.Count + Clothing.Count(x => x.Id != 0);

    public static StoredInventory FromPlayer(Player player)
    {
        var inventory = new StoredInventory
        {
            PlayerId = player.channel.owner.playerID.steamID.m_SteamID,
            CreatedAt = DateTime.Now,
            Clothing = StoredClothing.FromPlayer(player).ToList()
        };

        for (byte page = 0; page < PlayerInventory.PAGES - 2; page++)
        {
            var pageItems = player.inventory.items[page];

            if (pageItems is null)
                continue;

            foreach (var item in pageItems.items.ToList())
            {
                inventory.Items.Add(new StoredInventoryItem
                {
                    Id = item.item.id,
                    Amount = item.item.amount,
                    Quality = item.item.quality,
                    State = item.item.state?.ToArray() ?? Array.Empty<byte>(),
                    Page = page,
                    PositionX = item.x,
                    PositionY = item.y,
                    Rotation = item.rot
                });
            }
        }

        return inventory;
    }

    public void Restore(Player player)
    {
        foreach (var clothing in Clothing)
            clothing.Restore(player);

        foreach (var item in Items)
            item.Restore(player);
    }
}
