using EloBuddy;
using EloBuddy.SDK;

namespace Lucian_The_Troll.Utility
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
                    damage += LucianPassive();
                }
                if (Program.W.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                    damage += LucianPassive();
                }
                if (Program.E.IsReady())
                {
                    damage += Player.Instance.GetAutoAttackDamage(target);
                    damage += LucianPassive();
                }
            }
            return damage;
        }

        public static float LucianPassive()
        {
            if (ObjectManager.Player.Level >= 1 && ObjectManager.Player.Level < 7)
            {
                return ObjectManager.Player.TotalAttackDamage*0.3f;
            }
            if (ObjectManager.Player.Level >= 7 && ObjectManager.Player.Level < 13)
            {
                return ObjectManager.Player.TotalAttackDamage*0.4f;
            }
            if (ObjectManager.Player.Level >= 13 && ObjectManager.Player.Level < 16)
            {
                return ObjectManager.Player.TotalAttackDamage*0.5f;
            }
            if (ObjectManager.Player.Level >= 16)
            {
                return ObjectManager.Player.TotalAttackDamage*0.6f;
            }
            return 0;
        }

        public static float Qdamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new[] {0, 80, 115, 150, 185, 220}[Program.Q.Level] +
                     new[] {0, 0.6, 0.7, 0.8, 0.9, 1.0}[Program.Q.Level]*ObjectManager.Player.FlatPhysicalDamageMod));
        }

        public static float Wdamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 60, 100, 140, 180, 220}[Program.W.Level] + 0.9f*ObjectManager.Player.FlatMagicDamageMod);
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 20, 35, 50}[Program.R.Level] + 0.2f*ObjectManager.Player.FlatPhysicalDamageMod +
                0.1f*ObjectManager.Player.FlatMagicDamageMod);
        }
    }
}