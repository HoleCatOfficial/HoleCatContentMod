using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Magic;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using System.Text;
using ReLogic.Content;
using OpusLib;

namespace DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss
{
    public class DarkOrb : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
        }

        float R1 = 0f;
        float R2 = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            R1 += 0.08f;
            R2 += 0.01f;
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, -R1, DTAssetLib.AuraRing.Value.Size() / 2, 1.4f * Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient, -R2, DTAssetLib.AuraRing.Value.Size() / 2, 1.4f * Projectile.scale, SpriteEffects.None, 0f);


            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, Projectile.Center - Main.screenPosition, null, Color.Black, R1, DTAssetLib.AuraRing.Value.Size() / 2, 1.4f * Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.AuraRing.Value, Projectile.Center - Main.screenPosition, null, Color.Black, R2, DTAssetLib.AuraRing.Value.Size() / 2, 1.4f * Projectile.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.BloomRingSharp.Value, Projectile.Center - Main.screenPosition, null, Color.Black, 0f, DTAssetLib.BloomRingSharp.Value.Size() / 2, 0.07f * Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


        public int rad = 70;
        public int revCooldown = 0;
        Entities.NightmareRoseBoss Rose = null;
        public override void AI()
        {
            
            //To visualize radius. ignore.
            //Opus.RingDustOutwardRandomDir(DustID.Torch, 10, Projectile.Center, rad, 0, default, 0, 1f);

            if (revCooldown > 0)
            {
                revCooldown--;
            }

            if (Projectile.timeLeft < 60)
            {
                if (Projectile.scale > 0)
                {
                    Projectile.scale -= 0.02f;
                }
            }
            

            if (Rose == null)
            {
                foreach(NPC T in Main.npc)
                {
                    if (T.active && T.ModNPC is Entities.NightmareRoseBoss R)
                    {
                        Rose = R;
                    }
                }
            }
            else
            {
                float Dsq = (Rose.BorderRad - rad) * (Rose.BorderRad - rad);
                if (Projectile.Center.DistanceSQ(Rose.NPCHead) >= Dsq)
                {
                    SoundEngine.PlaySound(DTAssetLib.Impacts.MagicBeep with { MaxInstances = 0, PitchVariance = 1f }, Projectile.Center);
                    Vector2 normal = Vector2.Normalize(Projectile.Center - Rose.NPCHead);
                    Vector2 vel = Projectile.velocity;

                    // reflect: v - 2*(v·n)*n
                    Vector2 reflected = vel - 2f * Vector2.Dot(vel, normal) * normal;

                    // add slight randomness AFTER reflection
                    reflected = reflected.RotatedByRandom(0.3f);

                    Projectile.velocity = reflected;
                }
            }
        }

    
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Enfeebled>(), 240);
        }
    }
}

