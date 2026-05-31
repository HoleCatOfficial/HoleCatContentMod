using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using InnoVault.PRT;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace DestroyerTest.Content.Particles
{

    public class ElectricArc : BaseParticle<ElectricArc>
    {
        public float MaxLifetime = 60;
        public float Lifetime = 0;

        Vector2 position = Vector2.Zero;
        Color color = Color.White;
        float opacity = 1f;
        float _opacity = 1f;
        float rotation = 0f;
        public float scale = 1f;
        int variant = Main.rand.Next(1, 4);

        public void Create(Vector2 Position, Color Color)
        {
            this.position = Position;
            this.color = Color;
            this.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            this.variant = Main.rand.Next(1, 4);
        }

        public void Create(Vector2 Position, Color Color, float Opacity)
        {
            this.position = Position;
            this.color = Color;
            this.opacity = _opacity = Opacity;
            this.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            this.variant = Main.rand.Next(1, 4);
        }

        public void Create(Vector2 Position, Color Color, float Opacity, float Scale)
        {
            this.position = Position;
            this.color = Color;
            this.opacity = _opacity = Opacity;
            this.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            this.scale = Scale;
            this.variant = Main.rand.Next(1, 4);
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            Lifetime++;

            float Prog = Lifetime / MaxLifetime;

            opacity = MathHelper.Lerp(_opacity, 0, Prog);

            if (Lifetime > MaxLifetime)
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }

        public override PixelLayer DefaultPixelLayer => PixelLayer.AboveNPCs;

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"DestroyerTest/Content/Particles/PreMultiplied/ElectricArc{variant}").Value;
            Vector2 origin = texture.Size() / 2f;

            Opus.StartSpriteBatchWithBlending(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate);

            spritebatch.Draw(texture, position - Main.screenPosition, null, color with { A = 0 } * opacity, rotation, origin, scale, SpriteEffects.None, 0f);

            Opus.ReturnToDefaultDrawing(spritebatch);
        }
    }
    
}