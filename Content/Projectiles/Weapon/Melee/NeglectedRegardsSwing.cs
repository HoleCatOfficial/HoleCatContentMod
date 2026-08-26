using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

    public class NeglectedRegardsSwing : BaseBroadswordProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 78;
            Projectile.height = 78;
            SweepColor = new Color(20, 0 ,66);
            SweepHighlightColor = new Color(44, 7, 128);
            UsesDefaultSweepFX = true;
            SweepScale = 1.5f;
            SwingSpeed = 0.3f;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.Woosh with { MaxInstances = 0, Pitch = 0.4f, PitchVariance = 0.2f };

        public override void OnStartSwing()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 Vec = Owner.DirectionTo(Main.MouseWorld).RotatedByRandom(0.8f);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, Vec * 8f, ModContent.ProjectileType<NeglectedRegardsPaper>(), Projectile.damage / 2, 3, Owner.whoAmI);
            }
        }

        public Vector2 swordTip;
        public Line SwordLine;
        public override void ExtraEffects()
        {
            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            Player player = Main.player[Projectile.owner];

            SwordLine = new Line(player.Center, swordTip);
            Vector2[] pt = SwordLine.GetPointsAlongLine(30);

            
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}