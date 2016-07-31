using EloBuddy;
using EloBuddy.SDK;

namespace AsheTheTroll
{
    class DmgLibrary
    {
        public static
            int WDamage(Obj_AI_Base target)
        {
            return
                (int)
                    (new[] { 10, 60, 110, 160, 210 }[AsheTheTroll._w.Level - 1] +
                     1.4 * (AsheTheTroll._Player.TotalAttackDamage));
        }

        public static float RDamage(Obj_AI_Base target)
        {

            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 250, 425, 600 }[AsheTheTroll._r.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);

        }
    }
}
