using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles.fire;
using DestroyerTest.Content.Resources;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    [AutoloadEquip(EquipType.HandsOn)]
    public class ShimmeringGauntlet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoReuseAllWeapons = true;
            player.GetDamage(DamageClass.Melee) += 0.22f;
            if (player.TryGetModPlayer<ShimmeringGauntletPlayer>(out var G))
            {
                G.Active = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FireGauntlet)
                .AddIngredient<ShimmeringShards>(22)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class ShimmeringGauntletPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            if (Active)
            {
                scale = 1.19f;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
                if (Player.HandPosition != null)
                {
                    Vector2 Handpos = (Vector2)Player.HandPosition;

                    if (Main.rand.NextBool(5))
                    {
                        Fire fire = new Fire();
                        fire.PrepareFire(Handpos, Vector2.Zero, Main.rand.Next(1, 3), 0.08f, ColorLib.TenebrisGradient * 0.8f, 0.5f, 40, FireDrawMode.Additive);
                        ParticleEngine.ShaderParticles.Add(fire);
                    }
                }
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (item.DamageType == DamageClass.Melee && Active)
            {
                ShimmeringFlames.ShimmerBurn(target);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (proj.DamageType == DamageClass.Melee && Active)
            {
                ShimmeringFlames.ShimmerBurn(target);
            }
        }
    }
}
