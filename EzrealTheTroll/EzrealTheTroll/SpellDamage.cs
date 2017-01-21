using EloBuddy;
using EloBuddy.SDK;

namespace Ezreal_The_Troll
{
    public static class SpellDamage
    {
        internal static float GetRawDamage(Obj_AI_Base target)
        {
            float damage = 0;
            if (target != null)
            {
                if (EzrealTheTroll.Q.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (EzrealTheTroll.W.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.W);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (EzrealTheTroll.R.IsReady())
                {
                    damage += Player.Instance.GetSpellDamage(target, SpellSlot.R);
                    damage += Player.Instance.GetAutoAttackDamage(target);
                }
                if (ObjectManager.Player.CanAttack)
                damage += ObjectManager.Player.GetAutoAttackDamage(target);
            }
            return damage;
        }
        public static float Qdamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 35, 55, 75, 95, 115 }[EzrealTheTroll.Q.Level] + 1.1f * Player.Instance.FlatPhysicalDamageMod + 0.4f * Player.Instance.FlatMagicDamageMod
                    ));
        }
        public static float Wdamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 70, 115, 160, 205, 250 }[EzrealTheTroll.W.Level] + 0.8f * Player.Instance.FlatMagicDamageMod
                    ));
        }
        public static float Edamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 75, 125, 175, 225, 275 }[EzrealTheTroll.E.Level] + 0.5f * Player.Instance.FlatPhysicalDamageMod + 0.75f * Player.Instance.FlatMagicDamageMod
                    ));
        }
        public static float Rdamage(Obj_AI_Base target)
        {
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)(new[] { 0, 350, 500, 650 }[EzrealTheTroll.R.Level] + 1.0f * Player.Instance.FlatPhysicalDamageMod + 0.9f * Player.Instance.FlatMagicDamageMod
                    ));
        }

    }
}