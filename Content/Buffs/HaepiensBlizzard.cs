using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Buffs
{
    public class HaepiensBlizzard : ModBuff
    {
        private const float SpreadRadius = 100f; // Radius in pixels for buff spreading

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(NPC target, ref int buffIndex) {
            if (target.TryGetGlobalNPC<HBTarget>(out var modNPC)) {
                modNPC.lifeRegenDebuff = true;
            }

            // Spread the buff to nearby NPCs
            foreach (NPC npc in Main.npc) {
                if (npc.active && !npc.friendly && npc.whoAmI != target.whoAmI && npc.Distance(target.Center) < SpreadRadius) {
                    npc.AddBuff(Type, target.buffTime[buffIndex]); // Apply buff with same duration
                }
            }
        }
    }

    public class HBTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool lifeRegenDebuff;

        public override void ResetEffects(NPC npc) 
        {
            lifeRegenDebuff = false;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if(lifeRegenDebuff)
            {
                Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
                spriteBatch.Draw(DTAssetLib.HaepienCircleBottom.Value, npc.Center - Main.screenPosition, null, Color.Aquamarine, Rot, DTAssetLib.HaepienCircleBottom.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
                Opus.ReturnToDefaultDrawing(spriteBatch);
                spriteBatch.Draw(DTAssetLib.HaepienCircleTop.Value, npc.Center - Main.screenPosition, null, Color.White, Rot, DTAssetLib.HaepienCircleTop.Value.Size() / 2, 0.5f, SpriteEffects.None, 0);
            }
        }

        public float Rot = 0f;
        public float RotSpeed = 0.04f;
        public override void AI(NPC npc)
        {
            if (lifeRegenDebuff)
            {
                Rot += RotSpeed;
                if (!npc.boss)
                {
                    npc.velocity *= 0.6f;
                }


                Vector2[] pt = Opus.GetEquidistantOrbitVectors(6, npc.Center, RotSpeed, 25);

                foreach(Vector2 p in pt)
                {
                    Dust.NewDustPerfect(p, ModContent.DustType<ColorableNeonDust>(), Vector2.Zero, 0, Color.Aquamarine, 1f);
                }
                //Dust.NewDustDirect(npc.position, npc.width, npc.height, ModContent.DustType<ColorableNeonDust>(), (Main.rand.NextFloat(-0.2f, 0.2f) + npc.velocity.X), (Main.rand.NextFloat(-0.2f, 0.2f) + npc.velocity.Y), 0, Color.Aquamarine, 1f);
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            if (lifeRegenDebuff) 
            {
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;

                npc.lifeRegen -= 48;
            }
        }
    }
}
