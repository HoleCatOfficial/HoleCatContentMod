using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Particles.PotionFlowers;
using ReLogic.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Common
{
    public class PotionProfile
    {
        public string Name;
        public int ItemID;
        public int HealAmount;
        public PotionProfile(string name, int itemID, int healAmount)
        {
            Name = name;
            ItemID = itemID;
            HealAmount = healAmount;
        }
    }
    public class PotionFlowerPlayer : ModPlayer
    {
        public static SoundStyle HealSound = new SoundStyle("DestroyerTest/Assets/Audio/PotionFlowerHeal") { MaxInstances = 3, PitchVariance = 0.7f };
        public static void RegisterPotion(PotionProfile potion)
        {
            PotionFlowerPlayer player = ModContent.GetInstance<PotionFlowerPlayer>();
            player.Potions.Add(potion);
        }

        public List<PotionProfile> Potions = new List<PotionProfile>
        {
            new PotionProfile("Mushroom", ItemID.Mushroom, 15),
            new PotionProfile("BottledWater", ItemID.Mushroom, 30),
            new PotionProfile("LesserHealing", ItemID.LesserHealingPotion, 50),
            new PotionProfile("BottledHoney", ItemID.BottledHoney, 80),
            new PotionProfile("Eggnog", ItemID.Eggnog, 80),
            new PotionProfile("Restoration", ItemID.RestorationPotion, 80),
            new PotionProfile("Healing", ItemID.HealingPotion, 100),
            new PotionProfile("HoneyFin", ItemID.Honeyfin, 120),
            new PotionProfile("GreaterHealing", ItemID.GreaterHealingPotion, 150),
            new PotionProfile("SuperHealing", ItemID.SuperHealingPotion, 200),
        };

        bool IsRegisteredPotion(int itemType)
        {
            return Potions.Any(p => p.ItemID == itemType);
        }

        public override bool CanUseItem(Item item)
        {
            if (IsRegisteredPotion(item.type) && Active)
                return false;

            return base.CanUseItem(item);
        }

        private bool TryConsumeBestHealingPotion(Player player)
        {
            PotionProfile bestPotion = null;
            int bestSlot = -1;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.stack <= 0)
                    continue;

                PotionProfile profile = Potions.Find(p => p.ItemID == item.type);
                if (profile == null)
                    continue;

                if (bestPotion == null || profile.HealAmount > bestPotion.HealAmount)
                {
                    bestPotion = profile;
                    bestSlot = i;
                }
            }

            if (bestPotion == null)
                return false;

            Item bestItem = player.inventory[bestSlot];

            bestItem.stack--;
            if (bestItem.stack <= 0)
                bestItem.TurnToAir();

            player.statLife = Math.Min(
                player.statLife + bestPotion.HealAmount,
                player.statLifeMax2
            );

            player.HealEffect(bestPotion.HealAmount);

            return true;
        }



        public bool RadiantRose = false;
        public bool EphemeralSolvent = false;
        public bool Lillies = false;

        public bool Active => RadiantRose || EphemeralSolvent || Lillies;
        public override void ResetEffects()
        {
            RadiantRose = false;
            EphemeralSolvent = false;
            Lillies = false;
        }
        public int UseCooldown = 0;
        public override void PostUpdateMiscEffects()
        {
            if (RadiantRose || EphemeralSolvent || Lillies)
                {
                    if (Player.statLife < Player.statLifeMax2 / 2)
                    {
                        if (Main.rand.NextBool(5))
                        {
                            Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.YellowTorch, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            if (EphemeralSolvent)
                            {
                                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.PinkTorch, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            }
                            if (Lillies)
                            {
                                Dust.NewDust(Player.position, Player.Hitbox.Width, Player.Hitbox.Height, DustID.WhiteTorch, Player.velocity.X * 0.5f, Player.velocity.Y * 0.5f, 0, default, 2.25f);
                            }
                        }
                        if (UseCooldown >= (60 * 45))
                        {
                            if (TryConsumeBestHealingPotion(Player))
                            {
                                SoundEngine.PlaySound(HealSound, Player.Center);
                                if (RadiantRose)
                                {
                                    RadiantRoseParticle FX = new();
                                    FX.Spawn(Player.Center, 1f);
                                    ParticleEngine.BehindProjectiles.Add(FX);
                                }
                                if (EphemeralSolvent)
                                {
                                    EphemeralSolventParticle FX = new();
                                    FX.Spawn(Player.Center, 1f);
                                    ParticleEngine.BehindProjectiles.Add(FX);
                                }
                                if (Lillies)
                                {
                                    LilliesOfImmortalityParticle FX = new();
                                    FX.Spawn(Player.Center, 1f);
                                    ParticleEngine.BehindProjectiles.Add(FX);
                                }

                                UseCooldown = 0;
                            }
                        }
                    }

                    if (UseCooldown < (60 * 45))
                    {
                        UseCooldown++;
                    }
                }
    
        }
    }
}