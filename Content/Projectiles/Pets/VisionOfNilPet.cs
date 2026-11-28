using System.Reflection;
using BreadLibrary.Core.Utilities;
using CalamityEntropy.Content.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Pets
{
    public class VisionOfNilPet : ModProjectile
    {
        private Hook GetShaderHook;

        private delegate int orig_GetShaderHook(Projectile projectile);

        private static int ProjShaderDerailment(orig_GetShaderHook orig, Projectile projectile)
        {
            if (projectile.type != ModContent.ProjectileType<VisionOfNilPet>())
            {
                return orig(projectile);
            }
            else
            {
                return Main.player[projectile.owner].cMinion;
            }
        }


        public override void Load()
        {
            MethodInfo method = typeof(Main).GetMethod(nameof(Main.GetProjectileDesiredShader));
            GetShaderHook = new Hook(method, ProjShaderDerailment);

        }

        public override void Unload()
        {
            GetShaderHook?.Dispose();
            GetShaderHook = null;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;

        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = ProjAIStyleID.FloatBehindPet;

        }

        float r = 0;
        Vector2 off = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D swirl = ModContent.Request<Texture2D>(DTAssetLib.ParticlePath + "/Swirl_alpha").Value;

            Texture2D fade = ModContent.Request<Texture2D>(DTAssetLib.ExtrasPath + "/Aura1").Value;

            r -= 0.04f;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.UseBlendState(BlendState.NonPremultiplied);

            ArmorShaderData DumbassShader = GameShaders.Armor.GetShaderFromItemId(ItemID.TwilightDye);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }

            Main.EntitySpriteDraw(fade, Projectile.Center - Main.screenPosition, null, Color.Black * 0.8f, r * -1.6f, fade.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.EntitySpriteDraw(fade, Projectile.Center - Main.screenPosition, null, Color.Black * 0.8f, -r, fade.Size() / 2, 0.9f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.EntitySpriteDraw(fade, Projectile.Center - Main.screenPosition, null, Color.Black, r, fade.Size() / 2, 0.8f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);


            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Black, r + 0.01f, swirl.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Black, r + 0.01f, swirl.Size() / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Black, r, swirl.Size() / 2, 0.8f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);
            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Indigo, r * 1.3f, swirl.Size() / 2, 0.8f * Projectile.scale, SpriteEffects.FlipVertically, 0f);

            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Black, r * 2.2f, swirl.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);

            Main.EntitySpriteDraw(swirl, Projectile.Center - Main.screenPosition, null, Color.Purple, r * 2f, swirl.Size() / 2, 0.5f * Projectile.scale, SpriteEffects.FlipVertically, 0f);

            Main.EntitySpriteDraw(DTAssetLib.FeatheredCircle.Value, Projectile.Center - Main.screenPosition, null, Color.Black, r, DTAssetLib.FeatheredCircle.Value.Size() / 2, 0.8f * Projectile.scale, SpriteEffects.FlipHorizontally, 0f);


            Main.spriteBatch.ResetToDefault();

            /*
            ArmorShaderData DumbassShader = GameShaders.Armor.GetSecondaryShader(Main.GetProjectileDesiredShader(Projectile), Main.player[Projectile.owner]);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }
            */


            for (int i = 0; i < 4; i++)
            {
                Main.EntitySpriteDraw(texture, (Projectile.Center + new Vector2(0, -10).RotatedBy(Projectile.rotation) + new Vector2(8, 0).RotatedBy(r + (MathHelper.PiOver2 * i))) + off - Main.screenPosition, frame, Color.Indigo with { A = 0 }, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            }

            Main.EntitySpriteDraw(texture, (Projectile.Center + new Vector2(0, -10).RotatedBy(Projectile.rotation)) + off - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, FX, 0f);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.EnergyBounce with { Pitch = -0.3f, PitchVariance = 0.1f }, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                var p = PRTLoader.NewParticle<PRT_Void>(Projectile.Center, Main.rand.NextVector2Circular(9f, 9f), Color.White, 1f);
                p.Opacity = 0.7f;
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 30)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        Player Owner => Main.player[Projectile.owner];
        public bool CheckActive()
        {
            if (Owner.HasBuff(ModContent.BuffType<VoidHeartBuff>()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        SoundStyle Heartbeat = new SoundStyle("DestroyerTest/Assets/Audio/ShadeHeartHeartbeat") { MaxInstances = 2, Pitch = 0.5f, PitchVariance = 0.3f };
        float LightAmount = 0f;
        public override void AI()
        {
            AnimateProjectile();

            if (CheckActive())
            {
                Projectile.timeLeft = 120;
            }
            else
            {
                Projectile.Kill();
            }

            Color C = OpusColorUtils.Pastel(Color.Indigo, 0.55f);

            if (Main.rand.NextBool(4))
            {
                Vector2 outer = Projectile.Center + Main.rand.NextVector2Circular(300, 300);
                Color col = Main.rand.NextBool() ? Color.Indigo : Color.Purple;
                Dust d = Dust.NewDustPerfect(outer, DustID.FireworksRGB, outer.DirectionTo(Projectile.Center).RotatedBy(0.5f) * (4f + (0.005f * outer.Distance(Projectile.Center))), 0, col);
                d.noGravity = true;
            }

            if (Main.rand.NextBool(8))
            {
                Vector2 outer = Projectile.Center + Main.rand.NextVector2Circular(300, 300);
                var p = PRTLoader.NewParticle<PRT_Void>(outer, outer.DirectionTo(Projectile.Center).RotatedBy(0.5f) * (3f + (0.005f * outer.Distance(Projectile.Center))), Color.White, 1f);
                p.Opacity = 0.5f;
            }

            if (Main.rand.NextBool(20))
            {
                off = new Vector2(Main.rand.NextFloat(-9f, 9f));
                if (DTConfig.instance.MinionExtrasToggle)
                {
                    SoundEngine.PlaySound(DTAssetLib.Charge.WoodyTick with { Pitch = 1.7f, PitchVariance = 0.1f }, Projectile.Center);
                }
            }
            else
            {
                off = Vector2.Zero;
            }

            //var p = PRTLoader.NewParticle<PRT_Void>(Projectile.Center, Vector2.Zero, Color.White, 1f);
            //p.Opacity = 0.7f;

            if (LightAmount > 0.005f)
            {
                LightAmount -= 0.0003f;
            }
            else
            {
                LightAmount = 0.005f;
            }

            if (Projectile.frame == 5)
            {
                if (DTConfig.instance.MinionExtrasToggle)
                {
                    //SoundEngine.PlaySound(Heartbeat with { Pitch = 2, Volume = 0.2f }, Projectile.Center);
                    //SoundEngine.PlaySound(Heartbeat with { Volume = 0.2f }, Projectile.Center);
                    //SoundEngine.PlaySound(DTAssetLib.Charge.Anvil with { Pitch = -2.5f, PitchVariance = 0.1f, Volume = 0.5f }, Projectile.Center);
                }
                LightAmount = 0.01f;
            }



            Vector3 LightColor = new Vector3(C.R * LightAmount, C.G * LightAmount, C.B * LightAmount);
            Lighting.AddLight(Projectile.Center, LightColor);
        }

    }
}
