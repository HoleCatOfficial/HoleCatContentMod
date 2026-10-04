using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;

using Microsoft.Xna.Framework;
using OpusLib.Content.Particles;
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
    public class VoidGauntlet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 100;
            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                if (DTCrossMod.CallOfVoidMod.TryFind("VoidPurple", out ModRarity voidPurple))
                {
                    Item.rare = voidPurple.Type;
                }
            }
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoReuseAllWeapons = true;
            player.GetDamage(DamageClass.Melee) += 0.27f;
            player.GetDamage<DTTrueMeleeClass>() += 0.17f;
            if (player.TryGetModPlayer<VoidGauntletPlayer>(out var G))
            {
                G.Active = true;
            }
        }

        public override void AddRecipes()
        {
            if (DTCrossMod.CallOfVoidIsLoaded)
            {
                if (DTCrossMod.CallOfVoidMod.TryFind("VoidBar", out ModItem VoidBar) && DTCrossMod.CallOfVoidMod.TryFind("VoidWellTile", out ModTile voidWell))
                {
                    CreateRecipe()
                        .AddIngredient<ShimmeringGauntlet>()
                        .AddIngredient(VoidBar.Type, 10)
                        .AddTile(voidWell.Type)
                        .Register();
                }
            }
        }
    }

    public class VoidGauntletPlayer : ModPlayer
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
                scale = 1.22f;
            }
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {
               
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (item.DamageType.CountsAsClass(DamageClass.Melee) && Active)
            {
                DTCrossMod.CallofVoid_AddVoidTouch(target, 120, 1, maxLevel: 100);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (proj.DamageType.CountsAsClass(DamageClass.Melee) && Active)
            {
                DTCrossMod.CallofVoid_AddVoidTouch(target, 120, 1, maxLevel: 100);
            }
        }
    }
}
