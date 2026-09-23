
using System.Collections.Generic;
using System.Linq;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class ElementalPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
            DTUtils.NoEquipWith[Type] = (InfectedPendant.ItemsThatInfectedPendantCannotPairWith.ToArray().Concat(PendantofUnity.ItemsThatPendantofUnityCannotPairWith.ToArray())).ToArray();
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public float DMGBonus = 0.375f;
        public static readonly float CritBonus = 1.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs((DMGBonus - 1f).ToString("P1"), CritBonus.ToString("F1") + "%");

       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<ElementalPendantPlayer>().Active = true;
            player.GetDamage(ModContent.GetInstance<ScepterClass>()) += DMGBonus;
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += CritBonus;

            player.GetArmorPenetration<ScepterClass>() += 15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<InfectedPendant>()
                .AddIngredient<PendantofUnity>()
                .AddIngredient<FrigidPendant>()
                .AddIngredient<SmolderingPendant>()
                .AddIngredient<HelicitePendant>()
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class ElementalPendantPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Active)
            {
                if (item.DamageType.CountsAsClass<ScepterClass>())
                {
                    if (Player.altFunctionUse == 2)
                    {
                        if (Main.rand.NextBool())
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1f), ModContent.ProjectileType<TerraBolt>(), damage / 4, 3, Player.whoAmI);
                            }
                        }

                        if (Main.rand.NextBool())
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1f), ModContent.ProjectileType<TerraEater>(), damage / 4, 3, Player.whoAmI);
                            }
                        }
                    }
                }
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        
    }

    public class ElementalPendantGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.DamageType.CountsAsClass<ScepterClass>();
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.ModProjectile is ElementalScepterShot EShot)
            {
                return;
            }

            if (Main.player[projectile.owner].GetModPlayer<ElementalPendantPlayer>().Active)
            {
                target.AddBuff(BuffID.CursedInferno, 300);
                target.AddBuff(BuffID.Ichor, 300);
                target.AddBuff(BuffID.OnFire3, 300);
                target.AddBuff(BuffID.Frostburn2, 300);
                target.AddBuff(ModContent.BuffType<DaylightOverload>(), 300);
            }
        }
    }

}