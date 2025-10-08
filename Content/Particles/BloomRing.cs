using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class BloomRingSharp : BasePRT
    {

        public int MaxLifetime => 120;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            Scale = 0.01f;
        }


        public override void AI()
        {
            Velocity *= 0.99f;
            Rotation += Velocity.ToRotation();

            // Smooth lifetime-based scale growth:
            float t = LifetimeCompletion; // 0 → 1 across particle lifetime
            float startScale = 0.01f;
            float endScale = Scale * 1.25f; // 3x initial scale
            float currentScale = MathHelper.Lerp(startScale, endScale, t);
            Scale = currentScale;

            // Optional fading near the end of life
            if (t > 0.8f)
                Color *= 0.9f;

            // Kill particle when its lifetime runs out
            if (Lifetime <= 0)
                Kill();
        }


        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }

    internal class BloomRingZap : BasePRT
    {

        public int MaxLifetime => 120;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
        
            Scale = 0.01f;
        }


        public override void AI()
        {
            Velocity *= 0.99f;
            Rotation += Velocity.ToRotation();

            if (Main.rand.NextBool(3))
            {
                PRTLoader.NewParticle(DTUtils.ElectricArcs[Main.rand.Next(DTUtils.ElectricArcs.Length)], Main.rand.NextVector2FromRectangle(Frame), Vector2.Zero, Color, 0.4f);
            }

            // Smooth lifetime-based scale growth:
            float t = LifetimeCompletion; // 0 → 1 across particle lifetime
            float startScale = 0.01f;
            float endScale = Scale * 1.25f; // 3x initial scale
            float currentScale = MathHelper.SmoothStep(startScale, endScale, t);
            Scale = currentScale;

            // Optional fading near the end of life
            if (t > 0.8f)
                Color *= 0.9f;

            // Kill particle when its lifetime runs out
            if (Lifetime <= 0)
                Kill();
        }


        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
    
    internal class BloomRing : BasePRT
    {

        // The Texture property doesn't need to be overridden, as BasePRT has an automatic loading mechanism.
        // It automatically loads a .png file with the same name in the same directory.
        // This is similar to how ModProjectile works.
        // So, let's prepare a .png file called "ExamplePRT", which is an image with the same name as the class.
        // public override string Texture => base.Texture;

        // Override this function, it will be called once when the particle is generated.
        // PRT entities are independent instances, so the settings in this function
        // can also be applied to each instance individually, similar to ModProjectile.SetDefaults.
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
            // PRTDrawMode determines which rendering mode the instance will be batched into.
            // This sets the color blending mode for the particle's rendering.
            // Here, we set it to additive blending mode. The effect brought by this field is real-time,
            // and it will batch all PRT instances in each draw call.
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; // Lifetime of 220 to 360 ticks.
            //Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi); // Random rotation angle.
            Scale = 0.01f; // Random scale between 0.5 and 1.5.

        }

        public override void AI()
        {
            Velocity *= 0.99f;
            Rotation += Velocity.ToRotation();

            // Adjust the rotation according to the movement direction.
            //Rotation += Main.rand.NextFloat(-0.1f, 0.1f);

            Scale += 0.1f;


            //// Relative position change
            Position += Main.LocalPlayer.velocity;


            // Apply a fading effect near the end of its life.
            if (LifetimeCompletion > 0.9f)
            {
                Color *= 0.9f;
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }

    
}