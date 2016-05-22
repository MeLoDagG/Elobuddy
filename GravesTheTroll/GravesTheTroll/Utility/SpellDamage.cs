using EloBuddy;
using EloBuddy.SDK;

namespace GravesTheTroll.Utility
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
        public static float RDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 250, 400, 550 }[Program.R.Level] + 1.4 * ObjectManager.Player.FlatPhysicalDamageMod));
        }
    }
}
       