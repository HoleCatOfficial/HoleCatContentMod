using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.NodeBoss.Blessed
{
    public class BlessedNodeDeathProjectile : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.timeLeft = 60;
            Projectile.hostile = true;
        }

        public float LaserWarnOpacity = 0f;
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, Projectile.Center - Main.screenPosition, null, Main.DiscoColor * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 1f, SpriteEffects.None);
            Main.EntitySpriteDraw(DTAssetLib.BlessedNodeLaserTelegraph.Value, Projectile.Center - Main.screenPosition, null, Color.White * LaserWarnOpacity, LaserRotOffset - 12f, DTAssetLib.BlessedNodeLaserTelegraph.Value.Size() / 2, 0.65f, SpriteEffects.None);
            Opus.ReturnToDefaultDrawing(spriteBatch);
        }

        Projectile[] LaserBurstCol;
        float LaserRotOffset = 0f;

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasers"), Projectile.Center);
            LaserBurstCol = Opus.RadialSpreadProjectile(ModContent.ProjectileType<BlessedLaser>(), 6, Projectile.Center, 80, 1, 0.005f, offset: LaserRotOffset);
        }

        public bool Sound1 = false;
        public override void AI()
        {
            if (!Sound1)
            {
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/BlessedNodeLasersCharge"), Projectile.Center);
                Sound1 = true;
            }
            LaserRotOffset += 0.03f;

            if (LaserBurstCol != null)
            {
                if (LaserBurstCol.Length != 0)
                {
                    for (int p = 0; p < LaserBurstCol.Length; p++)
                    {
                        LaserBurstCol[p].ai[1] = LaserRotOffset;
                        LaserBurstCol[p].netUpdate = true;
                    }
                }
            }

            float t = Utilities.Convert01To010((Projectile.timeLeft / 60f));
            LaserWarnOpacity = MathHelper.Lerp(0f, 1f, t);
        }
    }
}
