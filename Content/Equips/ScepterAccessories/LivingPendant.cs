
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.player.Accessory;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity.Scepter;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Equips.ScepterAccessories
{
    public class LivingPendant : ModItem
    {
        public override void SetStaticDefaults()
        {
            DTUtils.NoUpgradeStack[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 54;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<LivingPendantPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 9)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class LivingPendantPlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public int Cooldown = 0;
        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;
                }
            }
        }
    }

    public class LivingPendantGlobal : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return lateInstantiation && entity.DamageType.CountsAsClass<ScepterClass>();
        }


        public override void AI(Projectile projectile)
        {
            base.AI(projectile);

            
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && Main.player[projectile.owner].GetModPlayer<LivingPendantPlayer>().Active && Main.player[projectile.owner].GetModPlayer<LivingPendantPlayer>().Cooldown <= 0) 
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal);
                Projectile.NewProjectile(projectile.GetSource_OnHit(target), target.Center, Main.rand.NextVector2CircularEdge(10f, 10f), ModContent.ProjectileType<LivingPendantHeal>(), 0, 0, projectile.owner, ai1: damageDone * 0.05f);
                Main.player[projectile.owner].GetModPlayer<LivingPendantPlayer>().Cooldown = 60;
            }
        }
    }
}