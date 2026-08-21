using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles.ParentClasses;

using InnoVault.Trails;
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

    public class UnionSwing : BaseBroadswordProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 54;
            Projectile.height = 88;
            Projectile.extraUpdates = 5;
            SweepColor = Color.Goldenrod;
            SweepHighlightColor = Color.YellowGreen;
            UsesDefaultSweepFX = true;
            UsesFireSweepFX = true;
            SwingSpeed = 0.17f;
            ScaleMult = 1.36f;
            SweepScale = 1.6f;

            

            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.HeavySwing;

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            npc.AddBuff(ModContent.BuffType<Defilement>(), 600);
            SoundEngine.PlaySound(DTAssetLib.Impacts.FlameImpact with { MaxInstances = 0, PitchVariance = 0.4f }, npc.Center);

        }

        private bool Ending;

        public int S = 0;
        public override void OnStartSwing()
        {


            Vector2 toMouse = Main.MouseWorld - Projectile.Center;
            toMouse.Normalize();

            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (toMouse.RotatedBy(Main.rand.NextFloat(0.1f, 1.1f) * LastSwing) * (15 * Owner.GetTotalAttackSpeed(DamageClass.Melee))), ModContent.ProjectileType<UnionFireball>(), Projectile.damage, 5, Projectile.owner);
            }
        }

        public Vector2 swordTip;
        public Line SwordLine;

        int t = 0;
        public override void ExtraEffects()
        {
            float s = Projectile.scale;

            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
        }
    }
}