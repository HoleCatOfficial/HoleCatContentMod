using BreadLibrary.Common.Whip;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon.SoulBoundWhip
{
    public class SoulBoundWhipProjectile : BaseWhipProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        #region IwhipMOtion
        protected override IWhipMotion CreateMotion()
        {
            return new WhipMotions.FancyWhipMotion();
        }

        protected override void SetupModifiers(ModularWhipController controller)
        {
            //controller.AddModifier(new WhipModifiers.TwirlModifier(4, 12, 0.05f* Projectile.spriteDirection));
            //controller.AddModifier(new WhipModifiers.SmoothSineModifier(6, 30, 8f, 4f, 1f, Direction: Projectile.spriteDirection));
        }
        #endregion
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.aiStyle = -1;
        }

        public override SoundStyle? WhipCrack_SFX => new SoundStyle("DestroyerTest/Assets/Audio/SoulWhipCrack") { MaxInstances = 0, PitchVariance = 0.2f };
        public override void Prepare()
        {
            AddHitEffects(ModContent.BuffType<SoulInferno>(), 600);

            WhipController = new ModularWhipController(CreateMotion());

            SetupModifiers(WhipController);

            Projectile.WhipSettings.Segments = 48;
        }
        public override void AI2()
        {
            
        }


        #region Drawing
        public override float GetWhipWidth(float baseWidth, float t)
        {
            _HeadOffset = new Vector2(0, -_HeadRectangle.Height / 2f);
            _DebugMode = false;
            _ShouldDrawNormal = true;
            _Head_VerticalFrames = 1;
            baseWidth = 2;
            return baseWidth;
        }
        protected override float RenderSpacing => 10f;
        public override float _PrimitiveScrollRate() => -1f;
        public override Color GetWhipColor(float t, float w)
        {
            Projectile.alpha = 0;
            return Color.White;
            //return Color.Lerp(Color.White, Color.Blue, MathF.Sin(Main.GlobalTimeWrappedHourly) * MathF.Cos(t * 10f));
        }


        public float Saturate(float x)
        {
            if (x > 1f)
                return 1f;
            if (x < 0f)
                return 0f;
            return x;
        }

        public string Path = "DestroyerTest/Content/Projectiles/Weapon/Summon/SoulBoundWhip";
        protected override void DrawOverPrimitive(List<Vector2> points)
        {
            _Head_y = (int)(5 * Math.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)));
            Texture2D tex = ModContent.Request<Texture2D>($"{Path}/SoulBoundWhip_MidChain").Value;

            


            float whipLength = Projectile.WhipSettings.Segments;

            float spacingPixels = tex.Width;

            int count = Math.Max(1, (int)(whipLength / spacingPixels));

            Vector2 End = GetPointAlongWhip(points, whipLength);

            //PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), End, Vector2.Zero, ColorLib.Soul3, 1f);

            // shared sliding parameter
            float slide = (MathF.Sin(Main.GlobalTimeWrappedHourly * 1f)) % 1f;

            for (int i = 0; i < count; i++)
            {
                // fixed offset per element
                float offset = i / (float)count;

                // sliding + offset
                float t = (slide + offset) % 1f;

                Vector2 point = GetPointAlongWhip(points, t);

                float rot = GetRotationAlongWhip(points, t);

                Color color = Color.White.MultiplyRGB(Lighting.GetColor(point.ToTileCoordinates()));

                Main.EntitySpriteDraw(tex, point - Main.screenPosition, null, color, rot, tex.Size() / 2, 1f, 0);
            }
        }
        public override bool _PrimitiveIsScrollingTexture => true;
        protected override Texture2D PrimitiveTex => DTAssetLib.Square.Value;

        protected override Texture2D WhipHandle => ModContent.Request<Texture2D>($"{Path}/SoulBoundWhipHilt").Value;
        protected override Texture2D WhipHead => ModContent.Request<Texture2D>($"{Path}/SoulBoundWhipHead").Value;

        #endregion
    }
}
