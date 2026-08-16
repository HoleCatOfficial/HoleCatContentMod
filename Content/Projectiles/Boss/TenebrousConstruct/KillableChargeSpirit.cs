using System;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.VampireBoss;
using DestroyerTest.Content.Projectiles.Boss.WyvernCorpseBoss;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class KillableChargeSpirit : ModNPC
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ProjectileNPC[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 200;
            NPCID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 48;
            NPC.takenDamageMultiplier = 0.25f;
            NPC.lifeMax = 300;

            NPC.HitSound = SoundID.Item24;
            NPC.DeathSound = SoundID.Item62;
            NPC.noTileCollide = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return false;

            }

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                float progress = i / (float)NPC.oldPos.Length;
                float scale = MathHelper.Lerp(NPC.scale, 0.0005f, progress);
                float Opacity = MathHelper.Lerp(1f, 0f, progress);

                Main.EntitySpriteDraw(
                    DTAssetLib.TinyBloom.Value,
                    NPC.OldCenter()[i] - Main.screenPosition,
                    null,
                    OpusColorUtils.Darken(ColorLib.TenebrisGradient, 0.6f) with { A = 0 } * Opacity,
                    NPC.rotation,
                    DTAssetLib.TinyBloom.Value.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0
                );
            }

            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, NPC.Center - screenPos, null, ColorLib.TenebrisGradient with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(DTAssetLib.TinyBloom.Value, NPC.Center - screenPos, null, Color.White with { A = 0 }, 0f, DTAssetLib.TinyBloom.Value.Size() / 2, NPC.scale * 0.4f, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            NPC.noTileCollide = true;
            NPC parent = Main.npc[(int)NPC.ai[0]];

            if (parent != null && parent.whoAmI != -1)
            {
                NPC.velocity = NPC.Center.DirectionTo(parent.Center) * 10f;
                if (NPC.Center.Distance(parent.Center) < 10)
                {
                    if (parent.ModNPC is Entities.TenebrousConstruct construct)
                    {
                        construct.Consumed++;
                        SoundEngine.PlaySound(DTAssetLib.Impacts.SpiritOfJusticeParry with { Pitch = MathHelper.Lerp(-1f, 1.2f, construct.ConsumptionProgress) });
                        NPC.StrikeInstantKill();
                    }
                }
            }
        }

        public override void OnKill()
        {
            for (int i = 0; i < 10; i++)
            {
                float scale = Main.rand.NextFloat(0.5f, 2f);

                Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(10, 10), 0, ColorLib.TenebrisGradient, scale);
                dust.noGravity = true;
            }

        }
    }
}
