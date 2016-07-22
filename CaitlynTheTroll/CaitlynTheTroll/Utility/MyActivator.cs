using EloBuddy;
using EloBuddy.SDK;

namespace CaitlynTheTroll.Utility
{
    internal static class Activator
    {
        public static Spell.Targeted Ignite;

        public static Item Youmus,
            Botrk,
            Bilgewater,
            CorruptPot,
            HuntersPot,
            RefillPot,
            Biscuit,
            HpPot,
            Qss,
            Mercurial;

        public static Spell.Active Heal, Barrier;

        public static void LoadSpells()
        {
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("dot"))
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 580);
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("dot"))
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 580);
            var slot = ObjectManager.Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            Youmus = new Item((int) ItemId.Youmuus_Ghostblade);
            Botrk = new Item((int) ItemId.Blade_of_the_Ruined_King);
            Bilgewater = new Item((int) ItemId.Bilgewater_Cutlass);
            Qss = new Item((int) ItemId.Quicksilver_Sash);
            Mercurial = new Item((int) ItemId.Mercurial_Scimitar);
            HpPot = new Item(2003);
            Biscuit = new Item(2010);
            RefillPot = new Item(2031);
            HuntersPot = new Item(2032);
            CorruptPot = new Item(2033);
        }
    }
}