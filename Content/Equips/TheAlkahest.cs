using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
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
    public class TheAlkahest : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<CelestialRarity>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.autoReuseAllWeapons = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<DTTrueMeleeClass>() += 0.12f;
            player.buffImmune[ModContent.BuffType<FusionBurn>()] = true;
            if (player.TryGetModPlayer<AlkahestPlayer>(out var G))
            {
                G.Active = true;
            }
        }
    }

    public class AlkahestPlayer : ModPlayer
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
                scale += 0.11f;
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
                target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
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
                target.AddBuff(ModContent.BuffType<FusionBurn>(), 300);
            }
        }
    }
}
