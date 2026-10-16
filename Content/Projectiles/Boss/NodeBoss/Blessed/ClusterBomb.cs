using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class ClusterBomb : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => false;

        bool IHomingProjectile.TracksPlayers => true;

        float IHomingProjectile.HomingTurnSpeed => 3f;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.04f;

        float IHomingProjectile.HomingMaxAccel => 16f;

        float IHomingProjectile.DetectRadius => 1600;

        bool IHomingProjectile.CanHome => Projectile.ai[1].InBounds(90f, 30f);

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<BlessedNodeMB>();
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }


        public override void AI()
        {
            Projectile.rotation = 0.1f * Math.Sign(Projectile.velocity.X);

            if (Projectile.ai[0] > 0)
            {
                Projectile.ai[1]++;
            }
            else
            {
                Projectile.timeLeft = 120;
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {

            if (Projectile.timeLeft < 2)
            {
                hitbox = Utils.CenteredRectangle(Projectile.Center, new Vector2(220, 220));
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<LightInferno>(), 300);
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath with { MaxInstances = 0, PitchVariance = 0.5f }, Projectile.Center);

            BloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 0.1f, 0.01f, 0.8f, BlendState.Additive);
            ParticleEngine.Particles.Add(Ring);



            int Gore1 = Mod.Find<ModGore>("BlessedShard1").Type;
            int Gore2 = Mod.Find<ModGore>("BlessedShard2").Type;
            int Gore3 = Mod.Find<ModGore>("BlessedShard3").Type;

            var entitySource = Projectile.GetSource_Death();
            DTOptimizationsConfig optcfg = ModContent.GetInstance<DTOptimizationsConfig>();
            if (optcfg.OptimizeGame == false)
            {
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore1);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore2);
                Gore.NewGore(entitySource, Projectile.position, new Vector2(Main.rand.Next(-4, 4), Main.rand.Next(-4, 4)), Gore3);
            }
        }

    }
}
