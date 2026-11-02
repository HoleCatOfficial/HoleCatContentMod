using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles.ParentClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

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