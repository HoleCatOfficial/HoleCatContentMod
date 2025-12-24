using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using Terraria.DataStructures;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Scepter;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Magic;

namespace DestroyerTest.Content.Projectiles.Weapon.Scepter
{
    public class RiftScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = ColorLib.Rift;
            WidthDim = 34;
            HeightDim = 34;
            DustType = ModContent.DustType<RiftDust>();
            base.SetDefaults();
        }

 

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HeliouricShock>(), 600);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
           
            base.OnTileCollide(oldVelocity);

            return false;
        }

    }
}

