using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
 
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{

    public class SunSaberSwing : BaseBroadswordProjectile
    {
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 118;
            Projectile.height = 118;
            SweepColor = Color.DarkOrange;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");

            SwingSpeed = 0.17f;
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.ColdSword with { Pitch = -0.4f, PitchVariance = 0.2f};

        public override void ExtraEffects()
        {
            SparkEdge(Main.player[Projectile.owner], 1f, Color.PaleGoldenrod);
        }

        public override void OnStartSwing()
        {
            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (toMouse * (7 * Owner.GetTotalAttackSpeed(DamageClass.Melee))), ModContent.ProjectileType<ComaceraticSlash>(), Projectile.damage, 5, Projectile.owner);
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, Vector2.Zero, ModContent.ProjectileType<SunExplosion>(), Projectile.damage / 2, 10, Owner.whoAmI);

            npc.AddBuff(ModContent.BuffType<ComaceraticBurn>(), 600);
        }
    }
}