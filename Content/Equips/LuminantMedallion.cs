
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;

namespace DestroyerTest.Content.Equips
{
    public class LuminantMedallion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.maxStack = 1;
            Item.value = 100;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LuminantMedallionPlayer>().Active = true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofLight, 20)
                .AddIngredient(ItemID.SoulofNight, 20)
                .AddIngredient(ItemID.GoldCoin)
            .Register();
        }
    }

    public class LuminantMedallionPlayer : ModPlayer
    {
        public bool Active;
        public override void ResetEffects()
        {
            Active = false;
        }
    }

    public class LuminantMedallionOwnedProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.player[projectile.owner].TryGetModPlayer<LuminantMedallionPlayer>(out LuminantMedallionPlayer player) && player.Active)
            {
                if ((projectile.DamageType == DamageClass.Summon && projectile.DamageType != DamageClass.Generic) && Main.rand.NextBool((int)(16 * (1 + (0.1f * Main.player[projectile.owner].numMinions)))) && projectile.type != ProjectileID.StardustGuardian)
                {
                    DTUtils.InfectedScepter_RingSpreadProjectileAlternating(ModContent.ProjectileType<SoulOfLight_Projectile>(), ModContent.ProjectileType<SoulOfNight_Projectile>(), 6, projectile.Center, 40f, 40, 3, 10);
                }
            }
        }
    }
}