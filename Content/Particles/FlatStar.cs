using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles
{
    internal class FlatStar : BasePRT
    {
        public int MaxLifetime => 20;

        private float _targetRotation;
        private float _startRotation;

        // remember the user-supplied starting scale
        private float _baseScale;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;

            // don’t override Scale if the spawner set it.
            // just record it so our curve is relative.
            _baseScale = Scale;

            // random spin ±90°–180°
            float min = MathHelper.ToRadians(90f);
            float max = MathHelper.ToRadians(180f);
            float randomAbs = Main.rand.NextFloat(min, max);
            _targetRotation = Main.rand.NextBool() ? randomAbs : -randomAbs;
            _startRotation = Rotation;
        }

        public override void AI()
        {
            float t = LifetimeCompletion;

            // rotate from start to target
            Rotation = MathHelper.Lerp(_startRotation, _targetRotation, t);

            // pulse up to double the starting scale, then back
            if (t < 0.5f)
                Scale = MathHelper.Lerp(_baseScale, _baseScale * 2f, t * 2f);
            else
                Scale = MathHelper.Lerp(_baseScale * 2f, _baseScale, (t - 0.5f) * 2f);

            // fade near the end
            if (t > 0.8f)
                Color *= 0.9f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }
}
