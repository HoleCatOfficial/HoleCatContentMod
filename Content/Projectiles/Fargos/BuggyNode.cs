using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Projectiles.ParentClasses;
using System.Collections.Generic;
using DestroyerTest.Content.Fargos.EternityDrops;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace DestroyerTest.Content.Projectiles.Fargos
{
    public class BuggyNode : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Default;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ArmorShaderData DumbassShader = GameShaders.Armor.GetSecondaryShader(Main.GetProjectileDesiredShader(Projectile), Main.player[Projectile.owner]);
            if (DumbassShader != null)
            {
                DumbassShader.Apply(Projectile);
            }

            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            int target = Projectile.AutoTarget();

            bool ShouldBeActive = Main.player[Projectile.owner].GetModPlayer<BuggyPlayer>().Active && !Main.player[Projectile.owner].dead;

            if (ShouldBeActive)
            {
                Projectile.timeLeft = 2;

                if (target != -1)
                {
                    NPC targetNPC = Main.npc[target];
                    if (Main.GameUpdateCount % 240 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.Center);
                        Vector2 velocitynormal = targetNPC.velocity;
                        Vector2 targ = targetNPC.Center + (velocitynormal * 2f);
                        Vector2 dir = Projectile.Center.DirectionTo(targ) * 10f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, dir, ModContent.ProjectileType<BuggyNodeSpark>(), (int)Main.player[Projectile.owner].GetTotalDamage(DamageClass.Generic).ApplyTo(70), 10, Projectile.owner);
                    }
                }
            }
        }
    }
}
