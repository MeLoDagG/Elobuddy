using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace EliseTheTroll
{
    public static class SpellManager
    {
        public static Spell.Targeted Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
        public static Spell.Targeted Q1;
        public static Spell.Active W2;
        public static Spell.Targeted E2;
        public static  Spell.Targeted Ignite { get; private set; }
        public static Spell.Targeted Smite;

        static SpellManager()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Linear, 250, 1000, 100);
            E = new Spell.Skillshot(SpellSlot.E, 1075, SkillShotType.Linear, 250, 1300, 55) { AllowedCollisionCount = 0 };
            R = new Spell.Active(SpellSlot.R);
            Q1 = new Spell.Targeted(SpellSlot.Q, 950);
            W2 = new Spell.Active(SpellSlot.W);
            E2 = new Spell.Targeted(SpellSlot.E, 750);
            var smite = Player.Spells.FirstOrDefault(s => s.SData.Name.ToLower().Contains("smite"));
            if (smite != null)
                Smite = new Spell.Targeted(smite.Slot, 570);

            var slot2 = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            if (slot2 != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot2, 600);
            }
        }

    public static void Initialize()
        {
        }
    }
}