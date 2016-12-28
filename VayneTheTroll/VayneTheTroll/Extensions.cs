using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;

namespace VayneTheTroll
{
    public static partial class Extensions
    {

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static bool IsAfterAttack;

        public static bool IsBeforeAttack;


        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 950);
        }

        public static bool UnderAllyTurret_Ex(this Vector3 position)
        {
            return ObjectManager.Get<Obj_AI_Turret>().Any(t => t.IsAlly && !t.IsDead);
        }

        public static IEnumerable<AIHeroClient> MeleeEnemiesTowardsMe
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.FindAll(
                        m => m.IsMelee && m.Distance(Extensions._Player) <= _Player.GetAutoAttackRange(m)
                             &&
                             (m.ServerPosition.To2D() + (m.BoundingRadius + 25f)*m.Direction.To2D().Perpendicular())
                                 .Distance(Extensions._Player.ServerPosition.To2D()) <=
                             m.ServerPosition.Distance(Extensions._Player.ServerPosition)
                             && m.IsValidTarget(1200, false));
            }
        }

        public static IEnumerable<AIHeroClient> EnemiesClose
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.Where(
                        m =>
                            m.Distance(ObjectManager.Player, true) <= Math.Pow(1000, 2) && m.IsValidTarget(1500, false) &&
                            m.CountEnemiesInRange(m.IsMelee ? m.AttackRange*1.5f : m.AttackRange + 20*1.5f) > 0);
            }
        }

        public static List<AIHeroClient> GetLhEnemiesNear(this Vector3 position, float range, float healthpercent)
        {
            return
                EntityManager.Heroes.Enemies.Where(
                    hero => hero.IsValidTarget(range, true, position) && hero.HealthPercent <= healthpercent).ToList();
        }
    }
}

          