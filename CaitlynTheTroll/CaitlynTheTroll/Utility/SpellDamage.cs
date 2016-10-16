using EloBuddy;
using EloBuddy.SDK;

namespace CaitlynTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {

            float damage = 0;
            if (target != null)
            {
                if (Program.Q.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (Program.E.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.E);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                 }
                if (Program.R.IsReady())
                {
                    damage += Player.Instance.GetAutoAttackDamage(target);
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
                }
            }
            return damage;
        }
    }
}
       