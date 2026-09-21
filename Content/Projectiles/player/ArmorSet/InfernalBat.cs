using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.RangedItems;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.ArmorSet
{
    public class InfernalBat : ModProjectile
    {
        private Hook GetShaderHook;

        private delegate int orig_GetShaderHook(Projectile projectile);

        private static int ProjShaderDerailment(orig_GetShaderHook orig, Projectile projectile)
        {
            if (projectile.type != ModContent.ProjectileType<InfernalBat>())
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
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;

        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 120;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                texture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            SpriteEffects FX = Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            ArmorShaderData DumbassShader = GameShaders.Armor.GetSecondaryShader(Main.GetProjectileDesiredShader(Projectile), Main.player[Projectile.owner]);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, FX, 0f);
       

            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
           
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
            if (++Projectile.frameCounter >= 5)
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
            if (Owner.HasBuff(ModContent.BuffType<InfernalBatBuff>()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        SoundStyle Shoot = SoundID.Item46;
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

            Color C = new Color(255, 49, 32);

            Lighting.AddLight(Projectile.Center, C.ToVector3());

            Vector2 Ideal = Owner.MountedCenter + new Vector2(20f * Owner.direction, -40f * Owner.gravDir);
            Projectile.SmoothMoveToPoint(Ideal, 20f, 120);

            if (Owner.controlUseItem && !Owner.dead && Owner.HeldItem.DamageType.CountsAsClass(DamageClass.Ranged))
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] % 90 == 0)
                {
                    

                    int Targ = Projectile.AutoTarget();

                    if (Targ != -1)
                    {
                        NPC target = Main.npc[Targ];

                        if (target != null && target.active)
                        {
                            SoundEngine.PlaySound(Shoot, Projectile.Center);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.Center.DirectionTo(target.Center), ModContent.ProjectileType<InfernalBatShot>(), (int)Owner.GetTotalDamage(DamageClass.Ranged).ApplyTo(45), 6f, Owner.whoAmI);
                        }
                    }
                }    
            }
        }

    }

    public class InfernalBatShot : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 15;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

        public override void AI()
        {
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 70, default, 1.6f);
            dust.noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
