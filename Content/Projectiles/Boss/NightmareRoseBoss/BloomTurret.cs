using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;

using DestroyerTest.Content.Projectiles.Gores;
using OpusLib.Content.Helpers;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class BloomTurret : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
        }

        public override bool PreDrawExtras()
        {

            var Tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph2");
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, OpusColorUtils.MultiLerp(prog.Inverse(), ColorLib.WretchedColorMap) with { A = 0 }, ToPlayer.ToRotation(), new Vector2(0f, Tex.Height() / 2), new Vector2(4f, 1f), SpriteEffects.None);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(source, Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
        }

        Vector2 ToPlayer => Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center);

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        float prog => Projectile.ai[1] / 90f;
        public override void AI()
        {
            AnimateProjectile();
            Projectile.velocity *= 0.99f;
            Projectile.rotation = ToPlayer.ToRotation();

            Projectile.ai[1]++;

            
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ToPlayer * 18f, ProjectileID.CursedFlameHostile, Projectile.damage, 5f);
        }
    }

    public class BloomTurret2 : ModProjectile
    {
        public override string Texture => "DestroyerTest/Content/Projectiles/Boss/NightmareRoseBoss/BloomTurret";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16 * 500;
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
        }

        public override bool PreDrawExtras()
        {

            var Tex = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/DirectionalTelegraph2");
            Main.EntitySpriteDraw(Tex.Value, Projectile.Center - Main.screenPosition, null, OpusColorUtils.MultiLerp(prog.Inverse(), ColorLib.WretchedColorMap) with { A = 0 }, ToPlayer.ToRotation(), new Vector2(0f, Tex.Height() / 2), new Vector2(4f, 1f), SpriteEffects.None);

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(source, Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
        }

        Vector2 ToPlayer => Projectile.Center.DirectionTo(Main.player[(int)Projectile.ai[0]].Center);

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        float prog => Projectile.ai[1] / 180f;
        public override void AI()
        {
            AnimateProjectile();
            Projectile.velocity *= 0.99f;
            Projectile.rotation = ToPlayer.ToRotation();

            Projectile.ai[1]++;


        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {

            SoundEngine.PlaySound(SoundID.Item100, Projectile.Center);
            for (int u = 0; u < 12; u++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.NextFloat(-6, 6), Main.rand.NextFloat(-6, 6)), ModContent.GoreType<RosePetalGore1>(), 2f);
            }
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ToPlayer * 18f, ProjectileID.CursedFlameHostile, Projectile.damage, 5f);
        }
    }
}