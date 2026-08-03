using System;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips
{
    public class MoltenEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = 400;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.2f;
            player.GetModPlayer<MoltenEmblemPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AvengerEmblem)
                .AddIngredient(ItemID.MagmaStone)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }
    }

    public class MoltenEmblemPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }
    }

    public class MoltenEmblemOwnedProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].GetModPlayer<MoltenEmblemPlayer>().Active && projectile.DamageType == DamageClass.Ranged && Main.rand.NextBool(14))
            {
                SoundEngine.PlaySound(SoundID.Item100);
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<FriendlyGreekFire>(), 12, target.Center, projectile.damage / 2, 2, 3f);
            }
        }
    }
}
