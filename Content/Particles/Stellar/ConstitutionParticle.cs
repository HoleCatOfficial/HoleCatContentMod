using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using DestroyerTest.Common;
 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Stellar
{
    public class ConstitutionParticle : BaseParticle<ConstitutionParticle>
    {
        public int lifetime = 0;
        public int MaxLifetime = 120;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float scale;

        public void Initialize(Vector2 Position, Vector2 Velocity, float Scale, int Lifetime)
        {
            this.position = Position;
            this.velocity = Velocity;
            this.scale = Scale;
            this.lifetime = Lifetime;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {

            lifetime++;

            float LifetimeCompletion = (float)lifetime / MaxLifetime;

            position += velocity;

            color = DTColorUtils.MultiLerp(LifetimeCompletion, ColorLib.StellarFireColormap);

            if (LifetimeCompletion > 0.5f)
            {
                color *= 0.95f;
                scale *= 0.95f;
            }

            if (lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveProjectiles;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Stellar/ConstitutionParticle").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spriteBatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 }, 0f, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spriteBatch);
        }
    }
}