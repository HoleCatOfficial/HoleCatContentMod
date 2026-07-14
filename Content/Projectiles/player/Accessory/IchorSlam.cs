using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class IchorSlam : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Bursting && Projectile.ManualCanHitFriendly(target);
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(Burst);

            Player Owner = Main.player[Projectile.owner];
            Owner.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 5;
            Owner.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 30;

            ImpactCracks Cracks = new();
            Cracks.Prepare(Projectile.Center, Color.White, 1f);
            ParticleEngine.BehindProjectiles.Add(Cracks);

            //Beeg boy
            SimpleExplosionParticle Burst1 = new SimpleExplosionParticle();
            Burst1.Prepare(Projectile.Center, Vector2.Zero, ColorLib.Ichor, 0.1f, 0.01f, 2.5f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst1);


            for (int d = 0; d < 24; d++)
            {
                Vector2 ran = new Vector2(Main.rand.NextFloat(-9f, 9f), Main.rand.NextFloat(-12f, -2f));

                Dust.NewDustPerfect(Projectile.Center, DustID.IchorTorch, ran, (int)MathHelper.Lerp(255, 0, Main.rand.NextFloat(0.5f, 1f)), default, Main.rand.NextFloat(1f, 2f));
            }

            Rectangle S = Utils.CenteredRectangle(Projectile.Center, new Vector2(48, 48));
            Point ST = S.TopLeft().ToTileCoordinates();
            Point SB = S.BottomRight().ToTileCoordinates();
            Projectile.CreateImpactExplosion(5, Projectile.Center, ref ST, ref SB, 60, out bool Shockwave);

            KnockbackNPCs();
        }

        public SoundStyle Burst = new SoundStyle("DestroyerTest/Assets/Audio/TenebrisTesticleKill");
        public bool Bursting = false;
        public override void AI()
        {

        }

        private void KnockbackNPCs()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.noGravity && npc.Center.Distance(Projectile.Center) < 90f)
                {
                    if (!npc.knockBackResist.Equals(0f))
                    {
                        Vector2 direction = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        npc.velocity += direction * 15f * npc.knockBackResist;
                    }
                }
            }

            foreach (Player plr in Main.player)
            {
                if (plr.active && plr.Center.Distance(Projectile.Center) < 90f)
                {
                    if (!plr.noKnockback)
                    {
                        Vector2 direction = (plr.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        plr.velocity += direction * 15f;
                    }
                }
            }
        }
    }
}