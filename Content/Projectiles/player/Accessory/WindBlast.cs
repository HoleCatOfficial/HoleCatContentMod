using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Accessory
{
    public class WindBlast : ModProjectile
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

       
            //Beeg boy
            SimpleExplosionParticle Burst1 = new SimpleExplosionParticle();
            Burst1.Prepare(Projectile.Center, Vector2.Zero, Color.SkyBlue * 0.5f, 0.1f, 0.01f, 2.5f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst1);

            SimpleExplosionParticle Burst2 = new SimpleExplosionParticle();
            Burst2.Prepare(Projectile.Center, Vector2.Zero, Color.SkyBlue * 0.15f, 0.1f, 0.01f, 1.8f, BlendState.Additive);
            ParticleEngine.BehindProjectiles.Add(Burst2);

            
        }

        public SoundStyle Burst = DTAssetLib.Impacts.WindHit;
        public bool Bursting = false;
        public override void AI()
        {
            KnockbackNPCs();
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

                    Vector2 direction = (plr.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    plr.velocity += direction * 15f;

                }
            }
        }
    }
}
