using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Magic;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Melee
{
	public class DistendedPikeProjectile : BaseSpearProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            MinExtension = 0.6f;
            MaxExtension = 80f;

            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.ArmorPenetration = 17;
            JabSound = DTAssetLib.SwordSounds.MediumHeavySwing with { PitchVariance = 0.4f };
        }

        public float ShineOpacity = 0f;
        Color SC;
        Vector2 DPos;
        public override void PostDraw(Color lightColor)
        {
            Texture2D Highlight = ModContent.Request<Texture2D>("DestroyerTest/Content/Extras/DistendedPikeHighlight").Value;

            DPos = Projectile.Center + (new Vector2(50, -50) * Projectile.scale).RotatedBy(Projectile.rotation);
            Main.EntitySpriteDraw(Highlight, DPos - Main.screenPosition, null, SC with { A = 0 } * ShineOpacity, Projectile.rotation, Highlight.Size() / 2, 1f * Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void ExtraEffects()
        {
            MaxExtension = 80f * Projectile.scale;

            ShineOpacity = MathHelper.Lerp(0, 1, Utilities.Convert01To010(progress));
            SC = OpusColorUtils.MultiLerp(Utilities.Convert01To010(progress), ColorLib.IchorCrystalColorMap);


            Dust D = Dust.NewDustPerfect(Tip, DustID.IchorTorch, Vector2.Zero, 50, (Color)default with { A = 0 }, 1f);
            D.noGravity = true;
        }

        public override void AtFullExtension()
        {
            Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 12, ModContent.ProjectileType<IchorNodeCrystalFriendly>(), Projectile.damage / 4, 5, Owner.whoAmI)].ArmorPenetration = Projectile.ArmorPenetration;
        }
    }
}