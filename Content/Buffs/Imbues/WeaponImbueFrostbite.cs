using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DestroyerTest.Common;

namespace DestroyerTest.Content.Buffs.Imbues
{
    public class WeaponImbueFrostbite : BaseImbueBuff
    {
        public override WeaponImbuePlayer.Imbues Imbue => WeaponImbuePlayer.Imbues.FrostBite;
    }
}
