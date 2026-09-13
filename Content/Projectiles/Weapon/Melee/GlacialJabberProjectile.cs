using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.SwordLineage;
using Terraria.Utilities.Terraria.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using OpusLib;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using BreadLibrary.Core.Utilities;
using OpusLib.Content.Helpers;
using DestroyerTest.Content.Dusts;
using OpusLib.Content.Particles;
using System;
using BreadLibrary.Core.Graphics.Pixelation;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
    public class GlacialJabberProjectile : BaseSpearProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            MinExtension = 0.6f;
            MaxExtension = 80f;
            Projectile.DamageType = ModContent.GetInstance<DTTrueMeleeClass>();
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            //ExtraLength = 110f;
            JabSound = DTAssetLib.SwordSounds.Woosh with { PitchVariance = 0.6f };
        }


        public override void DrawUnder()
        {
      
        }

        public override void ExtraEffects()
        {
            MaxExtension = 80f * Projectile.scale;

            if (!Main.dedServ)
            {

            }

     
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            /*
            Rectangle ExtHitbox = Utils.CenteredRectangle(DPos, new Vector2(120, 120));
            if (ExtHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            */
            return base.Colliding(projHitbox, targetHitbox);
        }

        int LodgeCooldown = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            SoundEngine.PlaySound(DTAssetLib.Impacts.FleshHit with { PitchVariance = 0.2f });

        }
    }
}