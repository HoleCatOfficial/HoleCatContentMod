using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike
{
    public class PNoctisSparkFire : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 16;
        }

        WitheringSpark Spark = new WitheringSpark();
        int TargetNPC = -1;

        public override void OnSpawn(IEntitySource source)
        {
            TargetNPC = (int)Projectile.ai[0];
            Spark.PrepareSpark(Projectile.Center, Projectile.velocity * 0.001f, Projectile.velocity.ToRotation(), Color.Orange, 2f, false, 15, SparkDrawMode.AlphaBlend, 4f);
            ParticleEngine.BehindProjectiles.Add(Spark);

            if (TargetNPC >= 0 && TargetNPC < Main.maxNPCs && Main.npc[TargetNPC].active)
            {
                Vector2 Vel = Main.npc[TargetNPC].Center - Projectile.Center;
                Vel.Normalize();
                Projectile.velocity = Vel * 40;
            }
        }

        public override void AI()
        {
            Spark.position = Projectile.Center;
            Spark.rotation = Projectile.velocity.ToRotation();

            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing);
        }
    }

    public class PNoctisSparkIce : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 15;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 16;
        }

        WitheringSpark Spark = new WitheringSpark();
        int TargetNPC = -1;

        public override void OnSpawn(IEntitySource source)
        {
            TargetNPC = (int)Projectile.ai[0];
            Spark.PrepareSpark(Projectile.Center, Projectile.velocity * 0.001f, Projectile.velocity.ToRotation(), Color.SkyBlue, 2f, false, 15, SparkDrawMode.AlphaBlend, 4f);
            ParticleEngine.BehindProjectiles.Add(Spark);

            if (TargetNPC >= 0 && TargetNPC < Main.maxNPCs && Main.npc[TargetNPC].active)
            {
                Vector2 Vel = Main.npc[TargetNPC].Center - Projectile.Center;
                Vel.Normalize();
                Projectile.velocity = Vel * 40;
            }
        }

        public override void AI()
        {
            Spark.position = Projectile.Center;
            Spark.rotation = Projectile.velocity.ToRotation();

            
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.TenebrisSwing);
        }
    }
}
