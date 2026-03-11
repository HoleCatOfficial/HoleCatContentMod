using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.RiftArsenal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class DeprecatedItemPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<Rift_Katana>()))
            {
                foreach (Item t in Player.inventory)
                {
                    if (t.type == ModContent.ItemType<Rift_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<RiftHypersabre>(), 1);
                    }
                    if (t.type == ModContent.ItemType<Tenebrous_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<Colossus>(), 1);
                    }
                }

                foreach (Item t in Player.bank.item)
                {
                    if (t.type == ModContent.ItemType<Rift_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<RiftHypersabre>(), 1);
                    }
                    if (t.type == ModContent.ItemType<Tenebrous_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<Colossus>(), 1);
                    }
                }

                foreach (Item t in Player.bank2.item)
                {
                    if (t.type == ModContent.ItemType<Rift_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<RiftHypersabre>(), 1);
                    }
                    if (t.type == ModContent.ItemType<Tenebrous_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<Colossus>(), 1);
                    }
                }

                foreach (Item t in Player.bank3.item)
                {
                    if (t.type == ModContent.ItemType<Rift_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<RiftHypersabre>(), 1);
                    }
                    if (t.type == ModContent.ItemType<Tenebrous_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<Colossus>(), 1);
                    }
                }

                foreach (Item t in Player.bank4.item)
                {
                    if (t.type == ModContent.ItemType<Rift_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<RiftHypersabre>(), 1);
                    }
                    if (t.type == ModContent.ItemType<Tenebrous_Katana>())
                    {
                        t.TurnToAir();
                        Player.QuickSpawnItem(Player.GetSource_None(), ModContent.ItemType<Colossus>(), 1);
                    }
                }
            }
        }
    }
}
