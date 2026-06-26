using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DestroyerTest.Content.Particles
{
    public class DamnationParticle : BaseParticle<DamnationParticle>
    {
        public Vector2 position;
        public Vector2 velocity = Vector2.Zero;

        public float Opacity = 1.0f;
        float Rotation = 0f;
        public float scale = 0f;
        int LifeTime = 0;
        int MaxLifetime = 120;
        Asset<Texture2D> Tex = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/DamnationParticle");

        public static DamnationParticle Create(Vector2 Position, Vector2 Velocity, float Scale, int LifeTime, PixelLayer Layer)
        {
            DamnationParticle P = new();
            P.position = Position;
            P.velocity = Velocity;
            P.scale = Scale;
            P.MaxLifetime = LifeTime;
            P.PixelLayer = Layer;
            ParticleEngine.BehindProjectiles.Add(P, Layer);
            return P;
        }

        Color C;
        public override void Update(ref ParticleRendererSettings settings)
        {
            LifeTime++;
            float Progress = (float)LifeTime / (float)MaxLifetime;

            C = OpusColorUtils.MultiLerp(Progress, ColorLib.WretchedColorMap);

            position += velocity;
            Rotation = velocity.ToRotation() + MathHelper.PiOver4;

            if (Progress >= 1)
            {
                ShouldBeRemovedFromRenderer = true;
            }
    
        }

        public override void Draw(ref ParticleRendererSettings settings, SpriteBatch spritebatch)
        {
            Opus.StartSpriteBatchPixelated(spritebatch, BlendState.AlphaBlend, SpriteSortMode.Immediate); 
            spritebatch.Draw(Tex.Value, position - Main.screenPosition, null, C with { A = 0 }, Rotation, Tex.Size() / 2, scale, SpriteEffects.None, 0f);
            spritebatch.ResetToDefault();
        }
    }
}
