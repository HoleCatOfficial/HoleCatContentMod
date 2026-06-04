using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.player.Potion
{
    public class ShadeInfernoRing : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public Asset<Texture2D> RingTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/ShadeInfernoRing");

        public override void SetStaticDefaults()
        {
            
        }
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1600;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var Tex = RingTexture.Value;
            var Orig = RingTexture.Value.Size() / 2;

            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient with { A = 0 }, Projectile.ai[1], Orig, 3f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient with { A = 0 } * 0.5f, Projectile.ai[1] * 0.2f, Orig, 2.7f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(Tex, Projectile.Center - Main.screenPosition, null, ColorLib.TenebrisGradient with { A = 0 }, (Projectile.ai[1] * -1) * 0.8f, Orig, 2.6f, SpriteEffects.None, 0f);
            return false;
        }

        NPC[] AllNPCsInRange(float Radius)
        {
            List<NPC> npcsInRange = new List<NPC>();
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && Vector2.Distance(npc.Center, Projectile.Center) <= Radius)
                {
                    npcsInRange.Add(npc);
                }
            }
            return npcsInRange.ToArray();   
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Projectile.ai[1] += 0.05f;

            Projectile.Center = Owner.Center;

            if (Owner.GetModPlayer<ShadeRingPlayer>().Active)
            {
                Projectile.timeLeft = 1200;
            }

            for (int i = 0; i < AllNPCsInRange(370).Length; i++)
            {
                NPC npc = AllNPCsInRange(370)[i];
                if (npc != null && Projectile.ai[0] % 60 == 0)
                {
                    NPC.HitInfo hit = new NPC.HitInfo() { Damage = 150, Knockback = 1f, HitDirection = npc.Center.X > Projectile.Center.X ? 1 : -1 };
                    npc.StrikeNPC(hit);
                    ShimmeringFlames.ShimmerBurn(npc, false);
                }
            }
        }
    }
}
