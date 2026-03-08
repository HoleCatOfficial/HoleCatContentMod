using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class ButterflyofProvidenceSwing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 20;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
           
        }

    }
}