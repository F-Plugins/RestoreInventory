using SDG.Unturned;
using System;
using UnityEngine;

namespace Feli.RestoreInventory.Models;

public class StoredInventoryItem
{
    public ushort Id { get; set; }
    public byte Amount { get; set; }
    public byte Quality { get; set; }
    public byte[] State { get; set; }
    public byte Page { get; set; }
    public byte PositionX { get; set; }
    public byte PositionY { get; set; }
    public byte Rotation { get; set; }

    public void Restore(Player player)
    {
        var item = new Item(Id, Amount, Quality, State ?? Array.Empty<byte>());

        if (player.inventory.tryAddItem(item, PositionX, PositionY, Page, Rotation))
            return;

        AddOrDrop(player, Id, Amount, Quality, State);
    }

    public static void AddOrDrop(Player player, ushort id, byte amount, byte quality, byte[] state)
    {
        var item = new Item(id, amount, quality, state ?? Array.Empty<byte>());

        if (player.inventory.tryAddItem(item, true))
            return;

        ItemManager.dropItem(item, player.transform.position + Vector3.up, true, true, true);
    }
}
