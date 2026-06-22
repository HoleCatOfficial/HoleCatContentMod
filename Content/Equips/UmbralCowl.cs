using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class UmbralCowl : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<RiftRarity1>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<UmbralCowlPlayer>().Active = true;

            player.GetDamage(DamageClass.Magic) += 0.19f;
            player.GetCritChance(DamageClass.Magic) += 8f;

        }
    }

    public class UmbralCowlPlayer : ModPlayer
    {
        public bool Active = false;
       

        public override void ResetEffects()
        {
            Active = false;

        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (Active)
            {
                mult = 0.85f;
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
            {
                if (proj.DamageType == DamageClass.Magic)
                {
                    if (Main.rand.NextBool(3))
                    {
                        target.AddBuff(ModContent.BuffType<HeliouricShock>(), 300);
                    }
                }
            }
        }
    }
}
