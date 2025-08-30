using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Magic
{
    public class InfectedGrimoire : ModItem
    {
        public override void SetDefaults()
        {

            // DefaultToStaff handles setting various Item values that magic staff weapons use.
            // Hover over DefaultToStaff in Visual Studio to read the documentation!
            // Shoot a black bolt, also known as the projectile shot from the onyx blaster.
            Item.DefaultToStaff(ProjectileID.PurificationPowder, 20, 10, 8);
            Item.width = 34;
            Item.height = 38;
            Item.UseSound = new SoundStyle("DestroyerTest/Assets/Audio/GoliathPhantomHit") with
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 100,
            };

            // A special method that sets the damage, knockback, and bonus critical strike chance.
            // This weapon has a crit of 32% which is added to the players default crit chance of 4%
            Item.SetWeaponValues(32, 9, 5);

            Item.SetShopValues(ItemRarityColor.LightRed4, 02500);
        }

        public override void UseItemFrame(Player player)
        {
            base.UseItemFrame(player);
            Color PRTcolor;
            if (Main.rand.NextBool(2))
            {
                PRTcolor = ColorLib.Ichor;
            }
            else
            {
                PRTcolor = ColorLib.CursedFlames;
            }
            PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp2>(), Item.Center, Vector2.Zero, PRTcolor, 1f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float RotationOffset1 = Main.rand.NextFloat(-0.35f, 0.36f);
            float RotationOffset2 = Main.rand.NextFloat(-0.35f, 0.36f);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(RotationOffset1), ModContent.ProjectileType<CursedNodeCrystalFriendly>(), damage, knockback);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(RotationOffset2), ModContent.ProjectileType<IchorNodeCrystalFriendly>(), damage, knockback);
            return false;
        }
	}
}