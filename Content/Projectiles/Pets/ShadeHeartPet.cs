using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Pets
{
    public class ShadeHeartPet : ModProjectile
    {
        private Hook GetShaderHook;

        private delegate int orig_GetShaderHook(Projectile projectile);

        private static int ProjShaderDerailment(orig_GetShaderHook orig, Projectile projectile)
        {
            if (projectile.type != ModContent.ProjectileType<ShadeHeartPet>())
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
            Main.projFrames[Projectile.type] = 9;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = ProjAIStyleID.FloatBehindPet;
   
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowtexture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ArmorShaderData DumbassShader = GameShaders.Armor.GetSecondaryShader(Main.GetProjectileDesiredShader(Projectile), Main.player[Projectile.owner]);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, origin, Projectile.scale, FX, 0f);
            Main.EntitySpriteDraw(glowtexture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, FX, 0f);

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { Pitch = -0.5f, PitchVariance = 0.1f }, Projectile.Center);
            Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 5, Projectile.Center, 170, ColorLib.TenebrisMagenta, 1f);
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
            if (Owner.HasBuff(ModContent.BuffType<ShadeHeartPetBuff>()))
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

            Color C = OpusColorUtils.Pastel(ColorLib.TenebrisMagenta, 0.55f);

            if (Main.rand.NextBool(60))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, Main.rand.NextFloat(-0.001f, 0.001f), Main.rand.NextFloat(-0.005f, 0.005f), 90, ColorLib.TenebrisMagenta, Main.rand.NextFloat(0.2f, 1.7f));
                dust.velocity *= 0.08f;
            }    

            if (LightAmount > 0.005f)
            {
                LightAmount -= 0.0003f;
            }
            else
            {
                LightAmount = 0.005f;
            }

            if (Projectile.frame == 2)
            {
                if (DTConfig.instance.MinionExtrasToggle)
                {
                    SoundEngine.PlaySound(Heartbeat, Projectile.Center);
                }
                Opus.RadialSpreadDustRandom(DustID.TintableDustLighted, 3, Projectile.Center, 200, ColorLib.TenebrisMagenta * 0.5f, 0.4f, 0.5f);
                LightAmount = 0.01f;
            }

            
            
            Vector3 LightColor = new Vector3(C.R * LightAmount, C.G * LightAmount, C.B * LightAmount);
            Lighting.AddLight(Projectile.Center, LightColor);
        }

    }
}
