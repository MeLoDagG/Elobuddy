using EloBuddy;
using EloBuddy.SDK;

namespace FiddleTheTroll.Utility
{
    internal static class Activator
    {
        public static Spell.Targeted Ignite { get; private set; }
        public static Item ZhonyaHourglass { get; private set; }

        public static Item
            CorruptPot,
            HuntersPot,
            RefillPot,
            Biscuit,
            HpPot;


        public static Spell.Active Heal, Barrier;

        public static void LoadSpells()
        {
            var slot2 = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            if (slot2 != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot2, 600);
            }
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("barrier"))
                Barrier = new Spell.Active(SpellSlot.Summoner1);
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("barrier"))
                Barrier = new Spell.Active(SpellSlot.Summoner2);
            var slot = ObjectManager.Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            ZhonyaHourglass = new Item(ItemId.Zhonyas_Hourglass);
            HpPot = new Item(2003);
            Biscuit = new Item(2010);
            RefillPot = new Item(2031);
            HuntersPot = new Item(2032);
            CorruptPot = new Item(2033);
        }
    }
}