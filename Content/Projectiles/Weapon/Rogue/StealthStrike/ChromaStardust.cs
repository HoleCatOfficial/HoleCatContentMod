using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Rogue.StealthStrike
{
    public class ChromaStardust : ModProjectile, IHomingProjectile
    {
        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 30;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingMaxAccel => 40f;

        float IHomingProjectile.DetectRadius => 1400;

        bool IHomingProjectile.CanHome => WaitTimer > 60;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public int WaitTimer = 0;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<DTRogueClass>();
            Projectile.tileCollide = false;
            Projectile.ArmorPenetration = 8;
        }


        List<SpriteEffects> oldfx = new List<SpriteEffects>();
        public SpriteEffects[] OldFX = new SpriteEffects[30];

        List<float> oldoffset = new List<float>();
        public float[] OldOffsets = new float[30];
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> Tex = TextureAssets.Projectile[Type];
            SpriteEffects FX = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotationoffset = Projectile.direction < 0 ? MathHelper.PiOver2 : 0f;


            if (OldFX != null && OldOffsets != null)
            {
                Projectile.DrawDirectionalAfterimages(Main.spriteBatch, Tex.Value, Color.White, OldFX, OldOffsets, 1f, true, false);
            }
            Vector2 Pos = Projectile.Center;

            Main.EntitySpriteDraw(Tex.Value, Pos - Main.screenPosition, null, Color.White, Projectile.rotation + rotationoffset, Tex.Value.Size() / 2, Projectile.scale, FX, 0f);

            return false;
        }

        public override void AI()
        {
            WaitTimer++;

            oldfx.Add(Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            oldoffset.Add(Projectile.direction < 0 ? MathHelper.PiOver4 + MathHelper.PiOver2 : MathHelper.PiOver4);

            if (oldfx.Count > 30)
            {
                oldfx.RemoveAt(30);
            }
            if (oldoffset.Count > 30)
            {
                oldoffset.RemoveAt(30);
            }

            OldOffsets = oldoffset.ToArray();
            OldFX = oldfx.ToArray();

            if (WaitTimer < 60)
            {
                Projectile.velocity *= 0.95f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Dust Trail = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Vector2.Zero, 0, ColorLib.Stardust, 1f);
            Trail.noGravity = true;

            Projectile.ResetExcessTrailPoints();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
