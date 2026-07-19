using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace DestroyerTest.Common.Primitives
{
    public struct EmpressTrailDiagonal
    {
        public const int TotalIllusions = 1;

        public const int FramesPerImportantTrail = 60;

        public static VertexStrip _vertexStrip = new VertexStrip();

        public Color ColorStart;

        public Color ColorEnd;

        public void Draw(Projectile proj, List<Vector2> oldPos, List<float> oldRot)
        {
            _ = proj.ai[1];
            MiscShaderData miscShaderData = GameShaders.Misc["EmpressBlade"];
            int num = 1;
            int num2 = 0;
            int num3 = 0;
            float w = 0.6f;
            miscShaderData.UseShaderSpecificData(new Vector4(num, num2, num3, w));
            miscShaderData.Apply();
            _vertexStrip.PrepareStrip(oldPos.ToArray(), oldRot.ToArray(), StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f, oldPos.Count, includeBacksides: true);
            _vertexStrip.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public Color StripColors(float progressOnStrip)
        {
            Color result = Color.Lerp(ColorStart, ColorEnd, Utils.GetLerpValue(0f, 0.7f, progressOnStrip, clamped: true)) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip, clamped: true));
            result.A /= 2;
            return result;
        }

        public float StripWidth(float progressOnStrip)
        {
            return 36f;
        }
    }
}
