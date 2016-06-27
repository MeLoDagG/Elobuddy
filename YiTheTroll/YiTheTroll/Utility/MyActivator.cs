using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace YiTheTroll.Utility
{
    internal static class Activator
    {
        public static Spell.Targeted Ignite;
        public static Spell.Targeted Smite;

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
            Ignite = new Spell.Targeted(ObjectManager.Player.GetSpellSlotFromName("summonerdot"), 600);
            if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner1).Name.Contains("barrier"))
                Barrier = new Spell.Active(SpellSlot.Summoner1);
            else if (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Summoner2).Name.Contains("barrier"))
                Barrier = new Spell.Active(SpellSlot.Summoner2);
            var slot = ObjectManager.Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            var smite = Player.Spells.FirstOrDefault(s => s.SData.Name.ToLower().Contains("smite"));
            if (smite != null)
            Smite = new Spell.Targeted(smite.Slot, 570);
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