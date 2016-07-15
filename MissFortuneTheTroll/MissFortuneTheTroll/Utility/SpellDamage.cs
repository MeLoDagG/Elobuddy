using EloBuddy;
using EloBuddy.SDK;

namespace MissFortuneTheTroll.Utility
{
    public static class SpellDamage
    {
        public static float GetTotalDamage(AIHeroClient target)
        {
            
            var damage = Program.Player.GetAutoAttackDamage(target);

            
            if (Program.Q.IsReady())
            {
                damage += Program.Q.GetRealDamage(target);
            }

            
            if (Program.W.IsReady())
            {
                damage += Program.W.GetRealDamage(target);
            }

            
            if (Program.E.IsReady())
            {
                damage += Program.E.GetRealDamage(target);
            }

            
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
            
            var spellLevel = Program.Player.Spellbook.GetSpell(slot).Level;
            const DamageType damageType = DamageType.Physical;
            float damage = 0;

            
            if (spellLevel == 0)
            {
                return 0;
            }
            spellLevel--;

            switch (slot)
            {
                case SpellSlot.Q:
                    
                    damage = new float[] { 20, 35, 50, 65, 80 }[spellLevel] + 0.85f * Program.Player.TotalAttackDamage + 0.35f * Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.W:
                    // 
                    damage = new float[] { 0, 0, 0, 0, 0 }[spellLevel] + 0.0f * Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.E:
                   
                    damage = new float[] { 90, 145, 200, 255, 310 }[spellLevel] + 0.8f * Program.Player.TotalMagicalDamage;
                    break;

                case SpellSlot.R:
                   
                    damage = new float[] { (0.35f * Program.Player.FlatPhysicalDamageMod + 0.2f * Program.Player.TotalMagicalDamage)*12  ,
                        (0.35f * Program.Player.FlatPhysicalDamageMod + 0.2f * Program.Player.TotalMagicalDamage) * 14,
                        (0.35f * Program.Player.FlatPhysicalDamageMod + 0.2f * Program.Player.TotalMagicalDamage) * 16}
                    [spellLevel];
                    break;
            }

            if (damage <= 0)
            {
                return 0;
            }

            return Program.Player.CalculateDamageOnUnit(target, damageType, damage) - 10;
        }
    }
}
