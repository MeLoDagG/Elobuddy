using EloBuddy;
using EloBuddy.SDK;

namespace taliyahTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            // Auto attack
            var damage = Program.Player.GetAutoAttackDamage(target);

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


            return damage;
        }

        public static float GetRealDamage(this Spell.SpellBase spell, Obj_AI_Base target)
        {
            return spell.Slot.GetRealDamage(target);
        }

        public static float GetRealDamage(this SpellSlot slot, Obj_AI_Base target)
        {
            // Helpers
            var spellLevel = Program.Player.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Magical;
            float damage = 0;

            // Validate spell level
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:

                    damage = new float[] {60, 80, 100, 120, 140}[spellLevel] + 0.4f*Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.W:

                    damage = new float[] {60, 80, 100, 120, 140}[spellLevel] + 0.4f*Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.E:

                    damage = new float[] {80, 105, 130, 155, 180}[spellLevel] + 0.4f*Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.R:

                    damage = new float[] {0, 0, 0}[spellLevel] + 0.0f*Program.Player.TotalMagicalDamage;
                    break;
            }

            if (damage <= 0)
            {
                return 0;
            }

            return Program.Player.CalculateDamageOnUnit(target, damageType, damage) - 10;
        }

        public static float Qdamage(Obj_AI_Base target)
        {
            return
                (float)
                    ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical,
                        new int[] {60, 80, 100, 120, 140}[Program.Q.Level] +
                        ObjectManager.Player.TotalMagicalDamage*0.4f);
        }
    }
}
