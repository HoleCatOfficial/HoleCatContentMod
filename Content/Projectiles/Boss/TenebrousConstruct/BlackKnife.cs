using BreadLibrary.Core;
using DestroyerTest.Common;
using DestroyerTest.Content.Entities;
using Microsoft.Xna.Framework;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class BlackKnife : ModProjectile
    {
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Type] = 80;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        float ro = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            ro += 0.08f;
            DTTrail.DrawTrail(Main.spriteBatch, DTAssetLib.Streak(14).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, ColorLib.TenebrisGradient * Projectile.Opacity, ro);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, ColorLib.TenebrisGradient));

            return false;
        }

        Vector2 StoredCenter;
        Vector2 StoredPlayerCenter;
        Vector2 EndPoint;
        Vector2 PreEnd;
        float RandomRot = 0f;
        Vector2[] PrePositions;


        public override void OnSpawn(IEntitySource source)
        {
            RandomRot = Main.rand.NextFloat(MathHelper.TwoPi);
            StoredCenter = Projectile.Center;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.type == ModContent.NPCType<Entities.TenebrousConstruct>() && npc.ModNPC is Entities.TenebrousConstruct tenebrousConstruct)
                {
                    StoredPlayerCenter = tenebrousConstruct.Knife_PlayerCenter;
                    PrePositions = Opus.GetEquidistantVectors(8, tenebrousConstruct.Knife_PlayerCenter, 350f);
                    PreEnd = tenebrousConstruct.KnifePositions[(int)Projectile.ai[1]];
                    EndPoint = PrePositions[(int)Projectile.ai[1]];
                }
            }
            
        }

        int timer = 0;
        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();
            timer++;
            float Prog = (float)timer / 90f;

            //Dust.NewDustPerfect(PreEnd, DustID.FireworksRGB, newColor: Color.Red).noGravity = true;


            if (timer <= 90)
            {
                BezierCurve Curve = DTUtils.EasyBezier(StoredCenter, Projectile.rotation.ToRotationVector2(), EndPoint, PreEnd.DirectionTo(EndPoint), 0.3f, 0.75f);
                Projectile.Center = Curve.Evaluate(Prog);
                Projectile.rotation = (Curve.Evaluate(Prog) - Curve.Evaluate(Prog - 0.01f)).ToRotation() + MathHelper.PiOver4;
            }
            else
            {
                if (timer == 91)
                {
                    SoundEngine.PlaySound(DTAssetLib.SwordSounds.MetalSwing with { MaxInstances = 0, PitchVariance = 0.7f });
                    Projectile.velocity = EndPoint.DirectionTo(StoredPlayerCenter) * 40f;
                }
                
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }
        }

        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(SoundID.Item14);
            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.TintableDustLighted, Main.rand.NextVector2Circular(3, 3), 0, ColorLib.TenebrisGradient);
            }
        }
    }
}

