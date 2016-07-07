using EloBuddy;
using EloBuddy.SDK;

namespace VladimirTheTroll.Utility
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

        public static
            float QDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] {0, 80, 100, 120, 140, 160}[Program.Q.Level] +
                 (0.45f*ObjectManager.Player.FlatMagicDamageMod)));
        }
    
        public static float RDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] {0, 150, 250, 350}[Program.R.Level] + (0.7f*ObjectManager.Player.FlatMagicDamageMod)));
        }
    }
}