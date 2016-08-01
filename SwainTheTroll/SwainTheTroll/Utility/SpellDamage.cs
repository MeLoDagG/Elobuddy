using EloBuddy;
using EloBuddy.SDK;

namespace SwainTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Player.Instance.GetAutoAttackDamage(target);

            // Q
            if (Program.Q.IsReady())
            {
                damage += Program.Q.GetRealDamage(target);
            }

            // W
            if (Program.W.IsReady())
            {
                damage += Program.W.GetRealDamage(target);
            }

            // E
            if (Program.E.IsReady())
            {
                damage += Program.E.GetRealDamage(target);
            }

            // R
            if (Program.R.IsReady())
            {
                damage += Program.R.GetRealDamage(target);
            }

            return damage;
        }

        public static float GetRealDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetRealDamage(target);
        }

        public static float GetRealDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            
            var spellLevel = Player.Instance.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Magical;
            float damage = 0;

            
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:
                    damage = new float[] { 25, 40, 55, 70, 85 }[spellLevel] + 0.3f * Player.Instance.TotalMagicalDamage * 3;
                    break;

                case SpellSlot.W:
                    damage = new float[] { 80, 120, 160, 200, 240 }[spellLevel] + 0.7f * Player.Instance.TotalMagicalDamage;
                    break;

                case SpellSlot.E:
                    damage = new float[] { 75, 115, 155, 195, 235 }[spellLevel] + 0.8f * Player.Instance.TotalMagicalDamage * 4;
                    break;

                case SpellSlot.R:
                    damage = new float[] { 50, 70, 90 }[spellLevel] + 0.2f * Player.Instance.TotalMagicalDamage;
                    break;
            }

            
            if (damage <= 0)
            {
                return 0;
            }

            
            return Player.Instance.CalculateDamageOnUnit(target, damageType, damage) - 20;
        }
    }
}
