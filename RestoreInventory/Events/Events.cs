using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using RestoreInventory.Models;

namespace RestoreInventory.Events
{
    public class Events
    {
        public class DamagePlayerRequestedEvent : IEventListener<UnturnedPlayerDamagingEvent>
        {
            private readonly RestoreInventory m_Plugin;
            public DamagePlayerRequestedEvent(RestoreInventory plugin)
            {
                m_Plugin = plugin;
            }

            public Task HandleEventAsync(object? sender, UnturnedPlayerDamagingEvent @event)
            {
                var player = @event.Player;

                if (player.Health <= @event.DamageAmount)
                {
                    var inventories = m_Plugin.GetInventories().Where(x => x.PlayerId == player.SteamId.ToString()).ToList();
                    if (inventories.Count == 3)
                    {
                        var oldest = inventories.OrderBy(x => x.Date).First();
                        m_Plugin.RemoveInventory(oldest);
                    }

                    var cloths = new List<ushort>();

                    cloths.Add(player.Player.clothing.backpack);
                    cloths.Add(player.Player.clothing.glasses);
                    cloths.Add(player.Player.clothing.hat);
                    cloths.Add(player.Player.clothing.mask);
                    cloths.Add(player.Player.clothing.pants);
                    cloths.Add(player.Player.clothing.shirt);
                    cloths.Add(player.Player.clothing.vest);

                    var items = new List<PlayerItem>();

                    for (byte i = 0; i < 7; i++)
                    {
                        byte ic = player.Player.inventory.getItemCount(i);
                        if (ic > 0)
                        {
                            for (byte j = 0; j < ic; j++)
                            {
                                var item = player.Player.inventory.getItem(i, j);
                                if (item != null)
                                {
                                    items.Add(new PlayerItem
                                    {
                                        Id = item.item.id,
                                        Amount = item.item.amount,
                                        Durability = item.item.durability,
                                        Metadata = Convert.ToBase64String(item.item.metadata),
                                        Quality = item.item.quality,
                                        Rot = item.rot,
                                        State = Convert.ToBase64String(item.item.state),
                                        Page = i,
                                        X = item.x,
                                        Y = item.y
                                    });
                                }
                            }
                        }
                    }

                    if (items.Count == 0) return Task.CompletedTask;

                    var playerInventory = new Models.PlayerInventory()
                    {
                        PlayerId = player.SteamId.ToString(),
                        Items = items,
                        Clothing = cloths,
                        Date = DateTime.Now
                    };


                    m_Plugin.AddInventory(playerInventory);
                }

                return Task.CompletedTask;
            }
        }
    }
}
