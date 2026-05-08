using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;

namespace DestroyerTest.Content.Particles.Orchestrated
{
    public class BlackDiamondParticle : BaseParticle<BlackDiamondParticle>
    {
        public Vector2 position;
        public float rotation;

        public bool Spawned = false;

        public void Initiate(Vector2 Position, float Rotation)
        {
            this.position = Position;
            this.rotation = Rotation;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (!Spawned)
            {
                Spark Up = new Spark();
                Spark Down = new Spark();
                Spark Left = new Spark();
                Spark Right = new Spark();

                Spark UpLeft = new Spark();
                Spark UpRight = new Spark();
                Spark DownLeft = new Spark();
                Spark DownRight = new Spark();

                Up.PrepareSpark(position, new Vector2(0, -2).RotatedBy(rotation), 0f, ColorLib.TenebrisBlue, 1f, false, 40, SparkDrawMode.Additive);
                Down.PrepareSpark(position, new Vector2(0, 2).RotatedBy(rotation), 0f, ColorLib.TenebrisBlue, 1f, false, 40, SparkDrawMode.Additive);

                Left.PrepareSpark(position, new Vector2(-2f, 0).RotatedBy(rotation), 0f, ColorLib.TenebrisBlue, 1f, false, 40, SparkDrawMode.Additive);
                Right.PrepareSpark(position, new Vector2(2f, 0).RotatedBy(rotation), 0f, ColorLib.TenebrisBlue, 1f, false, 40, SparkDrawMode.Additive);

                UpLeft.PrepareSpark(position, new Vector2(-0.3f, -0.3f).RotatedBy(rotation), 0f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                UpRight.PrepareSpark(position, new Vector2(0.3f, -0.3f).RotatedBy(rotation), 0f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                DownLeft.PrepareSpark(position, new Vector2(-0.3f, 0.3f).RotatedBy(rotation), 0f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                DownRight.PrepareSpark(position, new Vector2(0.3f, 0.3f).RotatedBy(rotation), 0f, DTColorUtils.Pastel(ColorLib.TenebrisBlue, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);

                ParticleEngine.BehindProjectiles.Add(Up);
                ParticleEngine.BehindProjectiles.Add(Down);
                ParticleEngine.BehindProjectiles.Add(Left);
                ParticleEngine.BehindProjectiles.Add(Right);
                ParticleEngine.BehindProjectiles.Add(UpLeft);
                ParticleEngine.BehindProjectiles.Add(UpRight);
                ParticleEngine.BehindProjectiles.Add(DownLeft);
                ParticleEngine.BehindProjectiles.Add(DownRight);
                Spawned = true;
            }
            else
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }
}
