using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public class GargantuaParticle : BaseParticle<GargantuaParticle>
    {

        public Vector2 position;

        public bool Spawned = false;

        public void Initiate(Vector2 Position)
        {
            this.position = Position;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (!Spawned)
            {
                StarParticle Star = new StarParticle();
                Star.Initialize(position, Vector2.Zero, Color.White, 1.5f);
                ParticleEngine.ShaderParticles.Add(Star);



                Spark Up = new Spark();
                Spark Down = new Spark();
                Spark Left = new Spark();
                Spark Right = new Spark();

                Up.PrepareSpark(position, new Vector2(0, -8), 0f, Color.Red, 1f, false, 40, SparkDrawMode.Additive);
                Down.PrepareSpark(position, new Vector2(0, 8), 0f, Color.Red, 1f, false, 40, SparkDrawMode.Additive);

                Left.PrepareSpark(position, new Vector2(-4f, 0), 0f, Color.Red, 0.75f, false, 40, SparkDrawMode.Additive);
                Right.PrepareSpark(position, new Vector2(4f, 0), 0f, Color.Red, 0.75f, false, 40, SparkDrawMode.Additive);

                ParticleEngine.BehindProjectiles.Add(Up);
                ParticleEngine.BehindProjectiles.Add(Down);
                ParticleEngine.BehindProjectiles.Add(Left);
                ParticleEngine.BehindProjectiles.Add(Right);
                Spawned = true;
            }
            else
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }
}