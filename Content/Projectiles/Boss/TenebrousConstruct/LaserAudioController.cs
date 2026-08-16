using BreadLibrary.Core;
using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Graphics.Pixelation;
using BreadLibrary.Core.Graphics.Spritebatch;
using BreadLibrary.Core.Utilities;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct
{
    public class LaserAudioController : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 1200;
        }

        SlotId LoopSlot;
        public SoundStyle Loop = DTAssetLib.LoopedSounds.HateLaser with
        {
            MaxInstances = 0,
            IsLooped = true,
            PauseBehavior = PauseBehavior.PauseWithGame
        };

        public override void AI()
        {
            float Prog = ((float)Projectile.timeLeft / 600f).Inverse();

            if (!SoundEngine.TryGetActiveSound(LoopSlot, out var activeSound))
            {
                var tracker = new ProjectileAudioTracker(Projectile);
                LoopSlot = SoundEngine.PlaySound(Loop, Projectile.Center, soundInstance => {
                    //soundInstance.Position = Projectile.Center;
                    return tracker.IsActiveAndInGame();
                });

            }
            else
            {
                activeSound.Position = Main.screenPosition + new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
                activeSound.Pitch = MathHelper.Lerp(0f, -0.1f, Prog);

                if (Projectile.timeLeft < 60)
                {
                    activeSound.Volume -= 0.04f;
                }
            }
        }
    }
}
