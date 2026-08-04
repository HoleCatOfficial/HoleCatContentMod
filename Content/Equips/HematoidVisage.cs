using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles.Riftplate;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using DestroyerTest.Rarity.Scepter;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Projectiles.player.Accessory;

namespace DestroyerTest.Content.Equips
{
    public class HematoidVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(4, 5));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Ranged) += 0.08f;
            player.GetModPlayer<HematoidVisagePlayer>().Active = true;
        }
    }

    public class HematoidVisagePlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }

       
    }

    public class HematoidVisageOwnedProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].GetModPlayer<HematoidVisagePlayer>().Active)
            {
                if (Main.rand.NextBool(5))
                {
                    Vector2 v = projectile.velocity;
                    v.Normalize();
                    Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, v * 16f, ModContent.ProjectileType<HematoidBlob>(), projectile.damage / 2, 4f, projectile.owner);
                }
            }
        }
    }
}
