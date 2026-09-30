using System.Collections.Generic;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.ConstitutionBoss;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
    public class DivineWellProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));

            Mult = Opus.Sine(1f, 1.1f, 0.3f);


            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Bottom - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, MathHelper.PiOver2, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(Growth * 0.5f, Growth * 2) * Mult, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.MiscSparkle144.Value, Projectile.Bottom - Main.screenPosition, null, ColorLib.Soul with { A = 0 }, 0f, DTAssetLib.MiscSparkle144.Value.Size() / 2, new Vector2(Growth / 2, Growth) * Mult, SpriteEffects.None);

            return false;
        }

        float Mult = 1f;

        float Growth = 0f;

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle(DTAssetLib.AudioFolder.Corpse + "/Summon") { PauseBehavior = PauseBehavior.PauseWithGame });
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            Projectile.velocity = new Vector2(0, -0.1f);

            Growth = MathHelper.Lerp(0f, 3f, Projectile.ai[0] / 120f);

            Projectile.rotation = 0f;

            Lighting.AddLight(Projectile.Center, ColorLib.Soul.ToVector3());

            PointGlowPreMultiplied glow = new();
            glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(0f, -7f), ColorLib.Soul3, 1.25f, 30);
            ParticleEngine.Particles.Add(glow);


            PointGlowPreMultiplied glow2 = new();
            glow2.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), new Vector2(0f, -7f), ColorLib.Soul, 1f, 30);
            ParticleEngine.Particles.Add(glow2);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                PointGlowPreMultiplied glow = new();
                glow.Initialize(Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Main.rand.NextVector2Circular(8f, 8f), ColorLib.Soul, 2f);
                ParticleEngine.Particles.Add(glow);
            }

            BlessingParticle Sigil = new();
            Sigil.Prepare(Projectile.Center, Vector2.Zero, Color.White, 0.5f, 0.02f, 3f, BlendState.Additive);
            ParticleEngine.Particles.Add(Sigil);


            Player player = Main.player[Projectile.owner];

            if (player.whoAmI == Main.myPlayer)
            {
                int type = ModContent.NPCType<WyvernCorpseHead>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }
        }
    }
}
