using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using Modes = EloBuddy.SDK.Orbwalker.ActiveModes;

namespace Ezreal_The_Troll
{
    internal class Elogic
    {
        public static Vector3[] BestEPos
        {
            get
            {
                var AARange = Player.Instance.GetAutoAttackRange(Nearest);
                var TargetPosition = new Vector2();
                if (Nearest != null)
                    TargetPosition = EzrealTheTroll.EPath.CurrentValue
                        ? Prediction.Position.PredictUnitPosition(Nearest, 500)
                        : Nearest.Position.To2D();
                var Path = Intersection_Of_2Circle(Player.Instance.Position.To2D(), EzrealTheTroll.E.Range,
                    TargetPosition, AARange);
                if (Path.Count() > 0)
                {
                    return new[] {Path.First().To3DWorld(), Path.Last().To3DWorld()};
                }
                return new Vector3[] {};
            }
        }

        public static int Get_Style_Of_E
        {
            get
            {
                if (Modes.Flee.IsActive())
                {
                    return 4;
                }
                return EzrealTheTroll.EMenu.GetValue("E", false);
            }
        }

        public static AIHeroClient Nearest
        {
            get
            {
                return
                    EntityManager.Heroes.Enemies.Where(x => x.IsValid && !x.IsDead)
                        .OrderBy(x => x.Distance(Player.Instance))
                        .FirstOrDefault();
            }
        }

        public static Vector3 GetPos()
        {
            var AARange = Player.Instance.GetAutoAttackRange(Nearest);
            var TargetPosition = new Vector2();
            if (Nearest != null)
                TargetPosition = EzrealTheTroll.EPath.CurrentValue
                    ? Prediction.Position.PredictUnitPosition(Nearest, 200)
                    : Nearest.Position.To2D();
            var Location = new Vector3();
            {
                switch (Get_Style_Of_E)
                {
                    case 1:
                    {
                        var Range = EzrealTheTroll.ERange.CurrentValue;
                        var Path = Intersection_Of_2Circle(Player.Instance.Position.To2D(), EzrealTheTroll.E.Range,
                            TargetPosition, AARange - Range);
                        if (Path.Count() > 0)
                        {
                            if (Path.Count(x => IsNotDangerPosition(x)) == 2)
                            {
                                Location =
                                    Path.OrderByDescending(x => Get_Rate_Position(x))
                                        .OrderBy(x => x.Distance(Game.CursorPos))
                                        .FirstOrDefault()
                                        .To3DWorld();
                            }
                            else if (Path.Count(x => IsNotDangerPosition(x)) == 0)
                            {
                                var Position =
                                    Path.OrderByDescending(x => Get_Rate_Position(x))
                                        .OrderBy(x => x.Distance(Game.CursorPos))
                                        .FirstOrDefault()
                                        .To3DWorld();
                                var target = TargetSelector.GetTarget(EntityManager.Heroes.Enemies.Where(t => t != null
                                                                                                              &&
                                                                                                              !t.IsDead
                                                                                                              &&
                                                                                                              t
                                                                                                                  .IsValidTarget
                                                                                                                  ()
                                                                                                              &&
                                                                                                              Position
                                                                                                                  .IsInRange
                                                                                                                  (t,
                                                                                                                      Player
                                                                                                                          .Instance
                                                                                                                          .GetAutoAttackRange
                                                                                                                          (t))
                                                                                                              &&
                                                                                                              t.Health <=
                                                                                                              SpellDamage
                                                                                                                  .Edamage
                                                                                                                  (t)
                                                                                                              &&
                                                                                                              !t
                                                                                                                  .Unkillable
                                                                                                                  ()),
                                    DamageType.Physical);
                                if (target != null)
                                {
                                    Location = Position;
                                }
                                else if (EzrealTheTroll.ECorrect.CurrentValue)
                                {
                                    var Loc = Path.OrderBy(x => x.Distance(Game.CursorPos)).FirstOrDefault().To3DWorld();
                                    Location = CorrectToBetter(Loc);
                                }
                                else
                                {
                                    Location = Vector3.Zero;
                                }
                            }
                            else
                            {
                                Location =
                                    Path.OrderBy(x => Get_Rate_Position(x))
                                        .FirstOrDefault(x => IsNotDangerPosition(x))
                                        .To3DWorld();
                            }
                        }
                        else
                        {
                            if (Player.Instance.Distance(TargetPosition) >= AARange + EzrealTheTroll.E.Range)
                            {
                                Location = Vector3.Zero;
                            }
                            if (Player.Instance.Distance(TargetPosition) <= AARange - EzrealTheTroll.E.Range &&
                                EzrealTheTroll.EKite.CurrentValue)
                            {
                                Location =
                                    Nearest.Position.Extend(Player.Instance,
                                        Nearest.Distance(Player.Instance) + EzrealTheTroll.E.Range).To3DWorld();
                            }
                        }
                    }
                        break;
                    case 2:
                    {
                        if (Nearest.Distance(Player.Instance) <= AARange + EzrealTheTroll.E.Range)
                            Location =
                                Player.Instance.ServerPosition.Extend(Game.CursorPos, EzrealTheTroll.E.Range)
                                    .To3DWorld();
                    }
                        break;
                    case 3:
                    {
                        if (Nearest.Distance(Player.Instance) <= AARange + EzrealTheTroll.E.Range)
                        {
                            if (Nearest.Distance(Player.Instance) <= AARange - EzrealTheTroll.E.Range &&
                                EzrealTheTroll.EKite.CurrentValue)
                            {
                                Location =
                                    Nearest.Position.Extend(Player.Instance,
                                        Nearest.Distance(Player.Instance) + EzrealTheTroll.E.Range).To3DWorld();
                            }
                            else
                            {
                                var Pos =
                                    Player.Instance.ServerPosition.Extend(Game.CursorPos, EzrealTheTroll.E.Range)
                                        .To3DWorld();
                                var GrassObject =
                                    ObjectManager.Get<GameObject>()
                                        .Where(
                                            x =>
                                                EzrealTheTroll.E.IsInRange(x.Position) &&
                                                x.Type.ToString() == "GrassObject")
                                        .OrderBy(x => x.Distance(Nearest))
                                        .FirstOrDefault();
                                var target = TargetSelector.GetTarget(EntityManager.Heroes.Enemies.Where(t => t != null
                                                                                                              &&
                                                                                                              !t.IsDead
                                                                                                              &&
                                                                                                              t
                                                                                                                  .IsValidTarget
                                                                                                                  ()
                                                                                                              &&
                                                                                                              Pos
                                                                                                                  .IsInRange
                                                                                                                  (t,
                                                                                                                      Player
                                                                                                                          .Instance
                                                                                                                          .GetAutoAttackRange
                                                                                                                          (t))
                                                                                                              &&
                                                                                                              t.Health <=
                                                                                                              SpellDamage
                                                                                                                  .Edamage
                                                                                                                  (t)
                                                                                                              &&
                                                                                                              !t
                                                                                                                  .Unkillable
                                                                                                                  ()),
                                    DamageType.Physical);
                                if (IsNotDangerPosition(Pos) || target != null)
                                {
                                    Location = Pos;
                                }
                                else if (EzrealTheTroll.ECorrect.CurrentValue)
                                {
                                    if (GrassObject != null && EzrealTheTroll.EGrass.CurrentValue)
                                    {
                                        Location = GrassObject.Position;
                                    }
                                    else
                                    {
                                        Location = CorrectToBetter(Pos);
                                    }
                                }
                            }
                        }
                    }
                        break;
                    case 4:
                    {
                        Location =
                            Player.Instance.ServerPosition.Extend(Game.CursorPos, EzrealTheTroll.E.Range).To3DWorld();
                    }
                        break;
                }
            }
            return Location;
        }

        public static Vector3 CorrectToBetter(Vector3 Pos)
        {
            var pos = Pos.To2D();
            var Corrected = new Vector3();
            var ERange = EzrealTheTroll.E.Range;
            var PlayerPos = Player.Instance.ServerPosition.To2D();
            var Path = Intersection_Of_2Circle(PlayerPos, ERange, pos, ERange);
            if (Path.Count() > 0)
            {
                switch (Path.Count(x => IsNotDangerPosition(x)))
                {
                    case 0:
                    {
                        Corrected = Vector3.Zero;
                    }
                        break;
                    case 1:
                    {
                        Corrected = Path.FirstOrDefault(x => IsNotDangerPosition(x)).To3DWorld();
                    }
                        break;
                    case 2:
                    {
                        Corrected =
                            Path.OrderByDescending(x => Get_Rate_Position(x))
                                .FirstOrDefault(x => x.IsInRange(Nearest, Player.Instance.GetAutoAttackRange(Nearest)))
                                .To3DWorld();
                    }
                        break;
                }
            }
            else
            {
                Corrected = Vector3.Zero;
            }
            return Corrected;
        }

        public static Vector2[] Intersection_Of_2Circle(Vector2 center1, float radius1, Vector2 center2, float radius2)
        {
            var Distance = center1.Distance(center2);
            if (Distance > radius1 + radius2 || (Distance <= Math.Abs(radius1 - radius2)))
            {
                return new Vector2[] {};
            }

            var A = (radius1*radius1 - radius2*radius2 + Distance*Distance)/(2*Distance);
            var H = (float) Math.Sqrt(radius1*radius1 - A*A);
            var Direction = (center2 - center1).Normalized();
            var PA = center1 + A*Direction;
            var Loc1 = PA + H*Direction.Perpendicular();
            var Loc2 = PA - H*Direction.Perpendicular();
            return new[] {Loc1, Loc2};
        }

        public static bool IsNotDangerPosition(Vector3 Pos)
        {
            if (EzrealTheTroll.ESafe.CurrentValue)
            {
                if (EzrealTheTroll.EWall.CurrentValue)
                {
                    var Fragment = EzrealTheTroll.E.Range/9;
                    for (var i = 1; i <= 9; i++)
                    {
                        if (Player.Instance.Position.Extend(Pos, i*Fragment).IsWall())
                            return false;
                    }
                }
                if (Pos.IsUnderEnemyTurret())
                {
                    return false;
                }
                if (Pos.CountEnemiesInRange(1000) > Pos.CountAlliesInRange(1000))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public static bool IsNotDangerPosition(Vector2 Pos)
        {
            return IsNotDangerPosition(Pos.To3DWorld());
        }

        public static int Get_Rate_Position(Vector2 Pos)
        {
            var Rate = 0;
            if (NavMesh.IsWallOfGrass(Pos.To3DWorld(), Player.Instance.BoundingRadius))
            {
                Rate += 3;
            }
            if (Pos.CountAlliesInRange(1000) > Pos.CountEnemiesInRange(1000))
            {
                Rate += Pos.CountAlliesInRange(1000) - Pos.CountEnemiesInRange(1000);
            }
            return Rate;
        }
    }
}