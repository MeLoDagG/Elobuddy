using EloBuddy;
using EloBuddy.SDK;

namespace CaitlynTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {

            var damage = Program.Player.GetAutoAttackDamage(target);
            if (Program.R.IsReady())
                damage = Program.Player.GetSpellDamage(target, SpellSlot.R);
            if (Program.E.IsReady())
                damage = Program.Player.GetSpellDamage(target, SpellSlot.E);
            if (Program.W.IsReady())
                damage = Program.Player.GetSpellDamage(target, SpellSlot.W);
            if (Program.Q.IsReady())
                damage = Program.Player.GetSpellDamage(target, SpellSlot.Q);

            return damage;
        }
     }
}
       