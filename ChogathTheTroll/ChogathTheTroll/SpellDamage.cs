using EloBuddy;
using EloBuddy.SDK;

namespace ChogathTheTroll
{
    public static class SpellDamage
    {
        internal static float GetRawDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (target != null)
            {
                if (Program.Q.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (Program.W.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (Program.R.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
            }
            return damage;
        }
    }
}