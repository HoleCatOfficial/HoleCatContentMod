using System;
using System.Configuration;
using DestroyerTest.Content.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Tiles;
using InnoVault.PRT;
using DestroyerTest.Common;
using InnoVault;
using DestroyerTest.Content.Projectiles;
using System.Security.Authentication.ExtendedProtection;

namespace DestroyerTest.Content.Equips.NightSet
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Head value here will result in TML expecting a X_Head.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Head)]
    public class NightHood : ModItem
    {
        public int ParticleSpawnTimer = 0;
        public override void SetStaticDefaults()
        {
            // If your head equipment should draw hair while drawn, use one of the following:

        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 22; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ModContent.RarityType<WineRarity>(); // The rarity of the item
            Item.defense = 8; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
        }

        // IsArmorSet determines what armor pieces are needed for the setbonus to take effect
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<NightBodyArmor>() && legs.type == ModContent.ItemType<NightLegArmor>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            if (player.TryGetModPlayer<NightPlayer>(out NightPlayer Night))
            {
                Night.Active = true;
            }
            ScepterClassStats.Range += 6;
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlinesForbidden = true; // or whatever action you're trying to trigger
        }

    }

    public class NightPlayer : ModPlayer
    {
        public bool Active = false;
        public bool Cooldown = false;
        public int CooldownTime = 360;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            
            if (Cooldown)
            {
                if (CooldownTime > 0)
                {
                    CooldownTime--;
                }

                if (CooldownTime <= 0)
                {
                    Cooldown = false;
                    CooldownTime = 360;
                }
            }
            
        }
    }
    
    public class NightSetTeleGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool IsAThrownScepter = false;
        public bool IsScepterClassButNotThrown = false;
        public bool CanTele = false;
        public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/Impacts/HellWeaponImpact");

        public override void SetDefaults(Projectile entity)
        {
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && entity.Name.Contains("Thrown"))
            {
                IsAThrownScepter = true;
            }
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && !entity.Name.Contains("Thrown"))
            {
                IsScepterClassButNotThrown = true;
            }
        }

        public override void AI(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            Opus.Opus opus = new Opus.Opus();
            if (IsAThrownScepter)
            {
                if (projectile.Distance(player.Center) < 900 && player.TryGetModPlayer<NightPlayer>(out NightPlayer pl))
                {
                    if (pl.Active && !pl.Cooldown)
                    {
                        CanTele = true;
                    }

                    if (CanTele)
                    {
                        int Rad = 200;
                        for (int c = 0; c < 4; c++)
                        {
                            Vector2 RingPos = Main.rand.NextVector2CircularEdge(Rad, Rad);
                            Dust.NewDustPerfect(RingPos, DustID.DemonTorch, projectile.velocity, 100, default, 2f);
                        }
                        
                        if (player.controlDown)
                        {
                            player.Center = projectile.Center;
                            SoundEngine.PlaySound(Tele, player.Center);
                            opus.RadialSpreadProjectile(ModContent.ProjectileType<SoulOfNight_Projectile>(), 8, player.Center, 15, 3, 6);
                            pl.Cooldown = true;
                            CanTele = false;
                        }   
                    }
                }
            }
        }
    }
}
