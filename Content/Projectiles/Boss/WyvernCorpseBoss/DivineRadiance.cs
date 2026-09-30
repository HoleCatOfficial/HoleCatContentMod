using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using FargowiltasSouls.Content.Patreon.DanielTheRobot;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss
{
    public class DivineRadiance : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/player/Accessory/ProvidenceRadiance";

        public override void SetStaticDefaults()
        {
            DTUtils.OwnedByBossNPC[Type] = ModContent.NPCType<WyvernCorpseHead>();
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 100;
        }

        public override bool CanHitPlayer(Player target)
        {


            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = TextureAssets.Projectile[Type];

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul3 with { A = 0 } * 0.15f, Rot, Tex.Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul2 with { A = 0 } * 0.25f, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.65f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, -Rot * 1.5f, Tex.Value.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None, 0f);

            return false;
        }

        public BindingRing Parent;

        public float rad = 100;

        float Rot = 0f;

        public override void AI()
        {
            Projectile.scale = 2f;
            float spd = Opus.Sine(0.07f, 0.16f, 0.01f);
            Rot += spd;


            if (Parent != null)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(10f, 10f), 0f, ColorLib.Soul, 1f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);

                float Dsq = ((Parent.Projectile.ai[1] - rad) + 5) * ((Parent.Projectile.ai[1] - rad) + 5);
                if (Projectile.Center.DistanceSQ(Parent.Projectile.Center) >= Dsq)
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.HeavyCrit with { MaxInstances = 0, PitchVariance = 1f }, Projectile.Center);
                    Vector2 normal = Vector2.Normalize(Projectile.Center - Parent.Projectile.Center);
                    Vector2 vel = Projectile.velocity;

                    // reflect: v - 2*(v·n)*n
                    Vector2 reflected = vel - 2f * Vector2.Dot(vel, normal) * normal;

                    // add slight randomness AFTER reflection
                    reflected = reflected.RotatedByRandom(0.2f);

                    Projectile.velocity = reflected;

                    for (int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, reflected.RotatedByRandom(1f), ModContent.ProjectileType<SoulSpit>(), Projectile.damage / 2, 4);
                    }
                }
            }

            if (Projectile.timeLeft == 1)
            {
                Projectile.Resize(200, 200);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<SoulInferno>(), 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Utilities.CircularHitboxCollision(Projectile.Center, rad, targetHitbox);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.DarkShatter, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                HeatseekerSilohSpark spark = new();
                spark.PrepareSpark(Projectile.Center, Main.rand.NextVector2Circular(10f, 10f), 0f, ColorLib.Soul, 1f, false, 60, SparkDrawMode.Additive, 2f);
                ParticleEngine.Particles.Add(spark);
            }

            Opus.RadialSpreadProjectile(ModContent.ProjectileType<SoulSpit>(), 9, Projectile.Center, (int)(Projectile.damage * 0.5f), 6, 6);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<SoulSpit>(), 12, Projectile.Center, (int)(Projectile.damage * 0.5f), 6, 10);
            Opus.RadialSpreadProjectile(ModContent.ProjectileType<SoulSpit>(), 20, Projectile.Center, (int)(Projectile.damage * 0.5f), 6, 14);
        }
    }
}
