using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DestroyerTest.Content.Entities;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Common;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Microsoft.Xna.Framework.Audio;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace DestroyerTest.Content.Projectiles
{
    public class ScholarShield : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20; // Will be reset to stay alive
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public SoundStyle Deflect = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldHit", 3) with { PitchVariance = 0.3f };
        public SoundStyle Spawn = new SoundStyle("DestroyerTest/Assets/Audio/Scholar/ShieldActivate", 3) with { PitchVariance = 0.3f };

        private Asset<Texture2D> ShieldTex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/BloomRingSharp_FullScale");
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            DTUtils Utility = new DTUtils();

            Utility.StartSpriteBatchWithBlending(sb, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(ShieldTex.Value, Projectile.Center - Main.screenPosition, null, ColorLib.JavelinEnergy, Projectile.rotation, ShieldTex.Value.Size() / 2, ShieldTexScale, SpriteEffects.None, 0);
            Utility.ReturnToDefaultDrawing(sb);
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(Spawn, Projectile.Center);
        }

        public float ShieldTexScale = 0.00001f;
        public override void AI()
        {
            if (ShieldTexScale < 0.5f)
            {
                ShieldTexScale += 0.005f;
            }

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && (!proj.friendly || proj.hostile) && proj.Distance(Projectile.Center) <= (ShieldTex.Value.Width * ShieldTexScale) / 2)
                {
                    SoundEngine.PlaySound(Deflect, proj.Center);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), proj.Center, Vector2.Zero, Color.White, 1f);
                    proj.Kill();
                }
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 Edge = Main.rand.NextVector2CircularEdge((ShieldTex.Value.Width * ShieldTexScale) / 2, (ShieldTex.Value.Width * ShieldTexScale) / 2);
                for(int i = 0; i < 3; i++)
                {
                    Dust.NewDustPerfect(Edge, DustID.TintableDustLighted, Vector2.Zero, 0, ColorLib.JavelinEnergy, 2);
                }
            }

            int scholarNpcType = ModContent.NPCType<Scholar>();
            NPC scholar = null;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == scholarNpcType)
                {
                    scholar = Main.npc[i];
                    break;
                }
            }

            if (scholar == null)
            {
                return;
            }

            Projectile.Center = scholar.Center;
            Projectile.timeLeft = 20;


        }
    }
}