using DestroyerTest.Content.Entity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class NightmareRoseBarrier : BasePRT
    {

        // The Texture property doesn't need to be overridden, as BasePRT has an automatic loading mechanism.
        // It automatically loads a .png file with the same name in the same directory.
        // This is similar to how ModProjectile works.
        // So, let's prepare a .png file called "ExamplePRT", which is an image with the same name as the class.
        // public override string Texture => base.Texture;

        // Override this function, it will be called once when the particle is generated.
        // PRT entities are independent instances, so the settings in this function
        // can also be applied to each instance individually, similar to ModProjectile.SetDefaults.
        public int MaxLifetime => 90000;
        public override void SetProperty()
        {
            // PRTDrawMode determines which rendering mode the instance will be batched into.
            // This sets the color blending mode for the particle's rendering.
            // Here, we set it to additive blending mode. The effect brought by this field is real-time,
            // and it will batch all PRT instances in each draw call.
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; // Lifetime of 220 to 360 ticks.
        }

        public override void AI()
        {
            bool bossFound = false;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.type == ModContent.NPCType<NightmareRoseBoss>())
                {
                    if (npc.ModNPC is NightmareRoseBoss modNPC)
                    {
                        Position = modNPC.NPCHead;
                        bossFound = true;
                        break; // stop only when we actually found it
                    }
                }
            }

            if (bossFound)
            {
                Lifetime += 200;
            }
            if (!bossFound)
                {
                    Scale++;
                    if (Scale > 6)
                        Color *= 0.9f;
                    if (Scale > 12)
                        Kill();
                }

            Rotation -= 0.75f;
        }



        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}