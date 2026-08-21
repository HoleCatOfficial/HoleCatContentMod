using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using OpusLib;


namespace DestroyerTest.Content.Projectiles.Pets
{
    public class DarkPrincePet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 7;
        }
        public SoundStyle TP = DTAssetLib.Impacts.StellarFox;

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = ProjAIStyleID.FloatInFrontPet;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(TP, Projectile.Center);
           
        }

        private void AnimateProjectile()
        {
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.velocity.Length() > 0f)
            {
                Projectile.rotation = 0.05f * Projectile.velocity.X;
            }

            AnimateProjectile();

            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<DarkPrincePetBuff>());
            }

            if (player.HasBuff(ModContent.BuffType<DarkPrincePetBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            KeepUp(2400f, player);
        }

        private void KeepUp(float distTeleport, Player master)
        {
            float dist = Projectile.Distance(master.Center);

            if (dist > distTeleport)
            {
                SoundEngine.PlaySound(TP);
                Projectile.Center = master.Center;
                Projectile.velocity *= 0.1f;
            }
        }
    }
}
