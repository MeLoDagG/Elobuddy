using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Ezreal_The_Troll
{
    public static class Extension
    {
        public static int GetValue(this Menu menu, string id, bool IsSlider = true)
        {
            if (IsSlider)
                return menu[id].Cast<Slider>().CurrentValue;
            return menu[id].Cast<ComboBox>().CurrentValue;
        }

        public static bool Checked(this Menu menu, string id, bool IsCheckBox = true)
        {
            if (IsCheckBox)
                return menu[id].Cast<CheckBox>().CurrentValue;
            return menu[id].Cast<KeyBind>().CurrentValue;
        }

        public static bool Unkillable(this AIHeroClient target)
        {
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "UndyingRage"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "ChronoShift"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "JudicatorIntervention"))
            {
                return true;
            }
            if (target.Buffs.Any(b => b.IsValid() && b.DisplayName == "kindredrnodeathbuff"))
            {
                return true;
            }
            if (target.HasBuffOfType(BuffType.Invulnerability))
            {
                return true;
            }
            return target.IsInvulnerable;
        }

        public static bool IsActive(this Enum Flag)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Flag);
        }

        public static bool IsUnderEnemyTurret(this Vector3 Pos)
        {
            return
                EntityManager.Turrets.AllTurrets.Any(
                    (Obj_AI_Turret turret) =>
                        Player.Instance.Team != turret.Team && Pos.Distance(turret) <= turret.GetAutoAttackRange() &&
                        turret.IsValid);
        }

        public static bool IsUnderEnemyTurret(this Vector2 Pos)
        {
            return Pos.To3DWorld().IsUnderEnemyTurret();
        }
    }
}