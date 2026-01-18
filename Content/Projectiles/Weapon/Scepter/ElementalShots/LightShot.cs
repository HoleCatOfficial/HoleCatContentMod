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
using DestroyerTest.Content.Dusts;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter.ElementalShots
{
	public class LightShot : ElementalScepterShot
	{
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            TrailType = 10;
        }

        public override void SetDefaults()
        {
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

			TrailColor = Main.DiscoColor;
			DustColor = Color.White;
			TravelDust = DustID.FireworksRGB;
			KillDust = DustID.FireworksRGB;
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;

            DetectionRad = 1200;
        }

        public override bool PreAI()
        {
            TrailColor = Main.DiscoColor;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.ShimmerArrow, new ParticleOrchestraSettings { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = target.Center});
            ParticleOrchestrator.RequestParticleSpawn(true, ParticleOrchestraType.RainbowRodHit, new ParticleOrchestraSettings { IndexOfPlayerWhoInvokedThis = (byte)Projectile.owner, PositionInWorld = target.Center});
        }
    }
}