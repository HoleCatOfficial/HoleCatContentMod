using System.Collections.Generic;
using DestroyerTest.Content.MeleeWeapons;
using Opus.Content.Helpers;
using Opus.Content.OpusBook;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Lorebooks
{
    public class HexBook : OpusBookItem
    {
        public override string GetBookKey() => "DTHexes";
    }

    public class WelcomeBookLoading : ModSystem
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                OpusBookRegistry.RegisterBook("DTHexes", new Dictionary<int, string>
                {
                    [0] = @"HEXES: Notes and Observations
                            By Joan Curen",

                    [1] = @"A hex differs from a spell in many ways, and from a curse in a
                    fewer, but still some. Hexes always have a caller, and typically are
                    not very advanced as far as Magic is concerned. Curses work like Hexes,
                    but have a propensity to be permanent or leave some other lifelong scar
                    on those afflicted by it. A spell is typically not meant for affliction,
                    as it simply costs too much mana from oneself to sustain such a spell long
                    enough to see its effects on their victim.",

                    [2] = @"I was given multiple materials to study. A hook of unknown origin,
                    an ingot that vibrated intensely, and an ingot that felt as though it were
                    breathing; expanding and contracting visibly. All of these shared the ability
                    to inflict a hex upon those unfortunate enough to be near them at the wrong
                    time.",

                    [3] = @"The first hex I took note of was an aliment relating to healing. When
                    afflicted with what I call the Blood Hex, I was unable to recover lost energy
                    or blood, with that process resuming after the affliction had run its course.
                    Aside from not being able to quickly recover from injuries, this Hex is rather
                    benign compared to what could have been done.",

                    [4] = @"The second hex I encountered was far more frightening. Out of similar
                    creative bankruptcy as before, I called it the Mobility Hex. When afflicted,
                    I found that I was unable to move my body very efficiently. I could still move,
                    but it felt as if I were submerged in chilled tar.",

                    [5] = @"What I found interesting, though, was how these hexes apply in combat.
                    If an enemy is hexxed, the hex will afflict you as well if they can land a shot
                    on you. I observed this in a battle against an etherian wyvern using the hook
                    (which, I must include, was the heaviest weapon I think I've ever wielded.) 
                    The Wyvern swooped in, and I jabbed through it 10 times with the pointed end
                    of the hook, which froze it in place, but left a rude surprise for me when
                    I turned my back."
                });
            }
        }
}
}