using BreadLibrary.Core.Graphics.Particles;
using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NightmareRoseBoss;
using DestroyerTest.Content.SummonItems;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles.Weapon.Summon
{
    public class BlackKnifeMinion : SwordMinionTemplate
    {
        public SoundStyle Tele = new SoundStyle("DestroyerTest/Assets/Audio/BlackKnifeTP") with {  };
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.minionSlots = 1;
            ThemeColor = Color.White;
            TintColor = Color.White;
            IdleDustType = DustID.WhiteTorch;
            DashDustType = DustID.WhiteTorch;
            TeleDustType = DustID.WhiteTorch;
            TeleSound = new SoundStyle("DestroyerTest/Assets/Audio/BlackKnifeTP");
            DashSound = DTAssetLib.SwordSounds.HellSword with { Pitch = 0.7f, PitchVariance = 0.2f, Volume = 0.8f };
            AfterImageColorless = true;
            AfterImageTinted = false;
            AfterImage = true;
            DefaultDraw = false;
            TickSpeed = 6;
            UsesParticleOrchestratorOnTele = false;
            TeleDist = 2000;
            Range = 2000;
            Style = IdleStyle.Defensive;
            ActiveBuff = ModContent.BuffType<BlackKnifeBuff>();
            UsesGroup = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(DTUtils.CenteredDraw(Projectile, Color.White));
            return false;
        }
    }
}
