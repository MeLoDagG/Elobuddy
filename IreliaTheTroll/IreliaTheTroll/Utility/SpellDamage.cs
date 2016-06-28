using System.Linq;
using EloBuddy;
using EloBuddy.SDK;

namespace IreliaTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {

            var damage = Program.Player.GetAutoAttackDamage(target);
            if (Program.R.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
            if (Program.E.IsReady())
                 damage += Player.Instance.GetSpellDamage(target, SpellSlot.E);
            if (Program.W.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
            if (Program.Q.IsReady())
                damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);

            return damage;
        }

        public static double ExtraWDamage()
        {
            var extra = 0d;
            var buff = ObjectManager.Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
            if (buff != null)
                extra += new double[] {15, 30, 45, 60, 75}[Program.W.Level - 1];

            return extra;
        }

        public static double QDamage(Obj_AI_Base target)
        {
            return Program.Q.IsReady()
                ? ObjectManager.Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new float[] {20, 50, 80, 110, 140}[Program.Q.Level - 1]
                    + 1.2F*ObjectManager.Player.TotalAttackDamage)
                : 0d;
        }

        public static double EDamage(Obj_AI_Base target)
        {
            return Program.E.IsReady()
                ? ObjectManager.Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new float[] {80, 120, 160, 200, 240}[Program.E.Level - 1]
                    + .5f*ObjectManager.Player.TotalMagicalDamage)
                : 0d;
        }

        public static double RDamage(Obj_AI_Base target)
        {
            return Program.R.IsReady()
                ? ObjectManager.Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    (new float[] {80, 120, 160}[Program.R.Level - 1]
                     + .5f*ObjectManager.Player.TotalMagicalDamage
                     + .6f*ObjectManager.Player.FlatPhysicalDamageMod
                        )*Program.Rcount)
                : 0d;
        }
    }
}
