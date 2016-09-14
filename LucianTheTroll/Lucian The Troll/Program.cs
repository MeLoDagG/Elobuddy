using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Lucian_The_Troll.Utility;
using SharpDX;
using Activator = Lucian_The_Troll.Utility.Activator;
using Color = System.Drawing.Color;

namespace Lucian_The_Troll

{
    public static class Program
    {
        public static string Version = "Version 1.1.1 (14/9/2016)";
        public static AIHeroClient Target = null;
        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static int CurrentSkin;
        public static readonly AIHeroClient Player = ObjectManager.Player;

        public static bool HasPassive()
        {
            return ObjectManager.Player.HasBuff("lucianpassivebuff");
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 1100);
        }

        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "Lucian") return;
            Chat.Print("Lucian The Troll Loaded! Version 1.1.1 (14/9/2016)", Color.DeepSkyBlue);
            Chat.Print("Have Fun And Dont Feed Kappa!", Color.DeepSkyBlue);
            LucianTheTrollMenu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 675);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, int.MaxValue, 65);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Linear, 250, 1600, 80);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 250, 1200, 70);

            #endregion

            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Obj_AI_Base.OnSpellCast += OnProcessSpellCast;
            //   Obj_AI_Base.OnBuffLose += OnBuffLose;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetRawDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (LucianTheTrollMenu.Nodraw()) return;

            {
                if (LucianTheTrollMenu.DrawingsQ())
                {
                    new Circle {Color = Color.DeepSkyBlue, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (LucianTheTrollMenu.DrawingsQ1())
                {
                    new Circle {Color = Color.DeepSkyBlue, Radius = Q1.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (LucianTheTrollMenu.DrawingsW())
                {
                    new Circle {Color = Color.DeepSkyBlue, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (LucianTheTrollMenu.DrawingsE())
                {
                    new Circle {Color = Color.DeepSkyBlue, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (LucianTheTrollMenu.DrawingsR())
                {
                    new Circle {Color = Color.DeepSkyBlue, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();

            if (LucianTheTrollMenu.CheckSkin())
            {
                if (LucianTheTrollMenu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(LucianTheTrollMenu.SkinId());
                    CurrentSkin = LucianTheTrollMenu.SkinId();
                }
            }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= LucianTheTrollMenu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= LucianTheTrollMenu.SpellsHealHp()
                && Player.CountEnemiesInRange(600) > 0 && Activator.Heal.IsReady())
            {
                Activator.Heal.Cast();
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                OnCombo();
                ItemUsage();
                CastR();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarrass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                OnLaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            KillSteal();
            AutoPotions();
            UseRTarget();
            AutoHarass();
        }

        private static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)

                if (LucianTheTrollMenu.Youmus() && Activator.Youmuu.IsOwned() && Activator.Youmuu.IsReady())
                {
                    if (ObjectManager.Player.CountEnemiesInRange(1800) >= LucianTheTrollMenu.YoumusEnemies())
                    {
                        Activator.Youmuu.Cast();
                    }
                }
            if (Player.HealthPercent <= LucianTheTrollMenu.BilgewaterHp() &&
                LucianTheTrollMenu.Bilgewater() &&
                Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
            {
                Activator.Bilgewater.Cast(target);
                return;
            }
            if (Player.HealthPercent <= LucianTheTrollMenu.BotrkHp() && LucianTheTrollMenu.Botrk() &&
                Activator.Botrk.IsReady() &&
                Activator.Botrk.IsOwned())
            {
                Activator.Botrk.Cast(target);
            }
        }

        private static
            void AutoPotions()
        {
            if (LucianTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= LucianTheTrollMenu.SpellsPotionsHp() &&
                !(Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlask") ||
                  Player.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Activator.HuntersPot.IsReady() && Activator.HuntersPot.IsOwned())
                {
                    Activator.HuntersPot.Cast();
                }
                if (Activator.CorruptPot.IsReady() && Activator.CorruptPot.IsOwned())
                {
                    Activator.CorruptPot.Cast();
                }
                if (Activator.Biscuit.IsReady() && Activator.Biscuit.IsOwned())
                {
                    Activator.Biscuit.Cast();
                }
                if (Activator.HpPot.IsReady() && Activator.HpPot.IsOwned())
                {
                    Activator.HpPot.Cast();
                }
                if (Activator.RefillPot.IsReady() && Activator.RefillPot.IsOwned())
                {
                    Activator.RefillPot.Cast();
                }
            }
            if (LucianTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= LucianTheTrollMenu.SpellsPotionsM() &&
                !(Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlask") ||
                  Player.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Activator.CorruptPot.IsReady() && Activator.CorruptPot.IsOwned())
                {
                    Activator.CorruptPot.Cast();
                }
            }
        }

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.Buff.Type == BuffType.Taunt && LucianTheTrollMenu.Taunt())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && LucianTheTrollMenu.Stun())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && LucianTheTrollMenu.Snare())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && LucianTheTrollMenu.Polymorph())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && LucianTheTrollMenu.Blind())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && LucianTheTrollMenu.Fear())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && LucianTheTrollMenu.Charm())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && LucianTheTrollMenu.Suppression())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && LucianTheTrollMenu.Silence())
            {
                DoQss();
            }
        }

        private static void DoQss()
        {
            if (Activator.Qss.IsOwned() && Activator.Qss.IsReady())
            {
                Core.DelayAction(() => Activator.Qss.Cast(),
                    LucianTheTrollMenu.Activator["delay"].Cast<Slider>().CurrentValue);
            }

            if (Activator.Mercurial.IsOwned() && Activator.Mercurial.IsReady())
            {
                Core.DelayAction(() => Activator.Mercurial.Cast(),
                    LucianTheTrollMenu.Activator["delay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void Flee()
        {
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (LucianTheTrollMenu.Fleew() && W.IsReady() && targetW.IsValidTarget(W.Range))
            {
                W.Cast(targetW);
            }
            if (LucianTheTrollMenu.Fleee() && E.IsReady())
            {
                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
        }

        private static void KillSteal()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= Q.Range && e.IsValidTarget(1000) && !e.IsInvulnerable))
            {
                if (LucianTheTrollMenu.KillstealQ() && Q.IsReady() &&
                    SpellDamage.Qdamage(enemy) >= enemy.Health && enemy.Distance(Player) >= 550)
                {
                    Q.Cast(enemy);
                }
                if (LucianTheTrollMenu.KillstealW() && W.IsReady() &&
                    SpellDamage.Wdamage(enemy) >= enemy.Health && enemy.Distance(Player) >= 550)
                {
                    var predW = W.GetPrediction(enemy);
                    if (predW.HitChance == HitChance.High)
                    {
                        W.Cast(enemy.Position);
                    }
                }
            }
        }

        private static
            void OnLaneClear()
        {
            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition,
                    Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(Q.Range));
            if (count == 0) return;
            if (LucianTheTrollMenu.LaneQ() && Player.ManaPercent > LucianTheTrollMenu.LaneMana() && Q.IsReady() &&
                !HasPassive())
            {
                Q.Cast(source);
            }
            if (LucianTheTrollMenu.LaneW() && Player.ManaPercent > LucianTheTrollMenu.LaneMana() && W.IsReady() &&
                !E.IsReady() && !HasPassive())
            {
                W.Cast(source);
            }
            if (LucianTheTrollMenu.LaneE() && Player.ManaPercent > LucianTheTrollMenu.LaneMana() && E.IsReady() &&
                !HasPassive())
            {
                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
        }

        private static
            void OnJungle()
        {
            var junleminions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(900));
            if (LucianTheTrollMenu.JungleE() && Player.ManaPercent > LucianTheTrollMenu.Junglemana() && E.IsReady() &&
                junleminions.IsValidTarget(E.Range) &&
                !HasPassive())
            {
                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
            if (LucianTheTrollMenu.JungleQ() && Player.ManaPercent > LucianTheTrollMenu.Junglemana() && Q.IsReady() &&
                !E.IsReady() && !W.IsReady() && junleminions.IsValidTarget(Q.Range) &&
                !HasPassive())
            {
                Core.DelayAction(() => Q.Cast(junleminions), 350);
            }
            if (LucianTheTrollMenu.JungleW() && Player.ManaPercent > LucianTheTrollMenu.Junglemana() && W.IsReady() &&
                !E.IsReady() && junleminions.IsValidTarget(W.Range) &&
                !HasPassive())
            {
                Core.DelayAction(() => W.Cast(junleminions), 350);
            }
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (LucianTheTrollMenu.AutoQHarass() && Q.IsReady() && target.IsValidTarget(Q1.Range) &&
                Player.ManaPercent >= LucianTheTrollMenu.AutoHarassMana())
            {
                CastExtendedQ();
            }
        }

        private static void OnHarrass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady() && LucianTheTrollMenu.HarassQ() && target.IsValidTarget(Q1.Range) &&
                Player.ManaPercent >= LucianTheTrollMenu.HarassMana())
            {
                CastExtendedQ();
                Q.Cast(target);
            }
            if (W.IsReady() && target.IsValidTarget(W.Range) && LucianTheTrollMenu.HarassW() &&
                Player.ManaPercent >= LucianTheTrollMenu.HarassMana())
            {
                var predW = W.GetPrediction(target);
                if (predW.HitChance >= HitChance.High)
                {
                    W.Cast(predW.CastPosition);
                }
            }
        }

        //Gredit D4mnedN00B
        private static void CastExtendedQ()
        {
            var target = TargetSelector.SelectedTarget != null &&
                         TargetSelector.SelectedTarget.Distance(EloBuddy.Player.Instance) < 2000
                ? TargetSelector.SelectedTarget
                : TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (!target.IsValidTarget(Q1.Range))
                return;
            var predPos = Q1.GetPrediction(target);
            var minions =
                EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                    m => m.Distance(EloBuddy.Player.Instance) <= Q1.Range);
            var champs = EntityManager.Heroes.Enemies.Where(m => m.Distance(EloBuddy.Player.Instance) <= Q1.Range);
            var monsters =
                EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(EloBuddy.Player.Instance) <= Q1.Range);
            {
                foreach (var minion in from minion in minions
                    let polygon = new Geometry.Polygon.Rectangle(
                        (Vector2) EloBuddy.Player.Instance.ServerPosition,
                        EloBuddy.Player.Instance.ServerPosition.Extend(minion.ServerPosition, Q1.Range), 65f)
                    where polygon.IsInside(predPos.CastPosition)
                    select minion)
                {
                    Q.Cast(minion);
                }

                foreach (var champ in from champ in champs
                    let polygon = new Geometry.Polygon.Rectangle(
                        (Vector2) EloBuddy.Player.Instance.ServerPosition,
                        EloBuddy.Player.Instance.ServerPosition.Extend(champ.ServerPosition, Q1.Range), 65f)
                    where polygon.IsInside(predPos.CastPosition)
                    select champ)
                {
                    Q.Cast(champ);
                }

                foreach (var monster in from monster in monsters
                    let polygon = new Geometry.Polygon.Rectangle(
                        (Vector2) EloBuddy.Player.Instance.ServerPosition,
                        EloBuddy.Player.Instance.ServerPosition.Extend(monster.ServerPosition, Q1.Range), 65f)
                    where polygon.IsInside(predPos.CastPosition)
                    select monster)
                {
                    Q.Cast(monster);
                }
            }
        }

        private static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target != null && LucianTheTrollMenu.ForceR() && R.IsReady() && target.IsValid &&
                !Player.HasBuff("lucianr")) R.Cast(target.Position);
        }

        private static
            void CastR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (!target.IsValidTarget(R.Range) || target == null)
            {
                return;
            }
            if (LucianTheTrollMenu.ComboRhp() && target.HealthPercent <= LucianTheTrollMenu.Hpslider() &&
                Player.CountEnemiesInRange(R.Range) == LucianTheTrollMenu.MinenemyR() &&
                R.IsReady() && target.IsValidTarget(R.Range))
            {
                R.Cast(target.Position);
            }

            if (LucianTheTrollMenu.KillstealR() && R.IsReady() && target.HealthPercent <= 30 &&
                Player.CountEnemiesInRange(R.Range) == 1 && target.IsValidTarget(R.Range))
            {
                R.Cast(target.Position);
            }
        }

        #region Side

        public static
            Vector2 Side(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4;
            result = Vector2.Add(result, point1);
            return result;
        }

        #endregion side

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsDead || !sender.IsMe) return;
            switch (args.Slot)
            {
                case SpellSlot.Q:
                    Orbwalker.ResetAutoAttack();
                    break;
                case SpellSlot.E:
                    Orbwalker.ResetAutoAttack();
                    break;
            }
        }

        private static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable ||
                    !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;

            var enemy = target as AIHeroClient;
            if (enemy == null)
                return;
            if (LucianTheTrollMenu.AArange())
            {
                if (!E.IsReady() && Q.IsReady())
                {
                    Q.Cast(enemy);
                }
                if (!E.IsReady() && W.IsReady())
                {
                    W.Cast(enemy.Position);
                    // Core.DelayAction(() => W.Cast(enemy), 300);
                }
            }
        }

        private static
            void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && Orbwalker.IsAutoAttacking)
            {
                var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
                if (!target.IsValidTarget(Q1.Range) || target == null)
                {
                    return;
                }
                {
                    if (LucianTheTrollMenu.ComboEStart())
                    {
                        if (LucianTheTrollMenu.Eside())
                        {
                            if (E.IsReady() && target.IsValidTarget(Q1.Range) &&
                                !UnderEnemyTower((Vector2) Player.Position) &&
                                !HasPassive())
                            {
                                E.Cast(Side(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                            }
                        }
                        if (LucianTheTrollMenu.Ecursor())
                        {
                            if (E.IsReady() && target.IsValidTarget(Q1.Range) &&
                                !UnderEnemyTower((Vector2) Player.Position) &&
                                !HasPassive())
                            {
                                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
                            }
                        }
                        if (LucianTheTrollMenu.Eauto())
                        {
                            if (Game.CursorPos.Distance(Player.Position) > Player.AttackRange + Player.BoundingRadius*2 &&
                                !Player.Position.Extend(Game.CursorPos, E.Range).IsUnderTurret() && !HasPassive())
                            {
                                E.Cast(Player.Position.Extend(Game.CursorPos, E.Range).To3D());
                            }
                            else
                            {
                                E.Cast(Side(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                            }
                        }
                    }
                    {
                        if (LucianTheTrollMenu.Smooth())
                        {
                            if (!E.IsReady() && Q.IsReady() && Player.Distance(target.Position) < Q.Range &&
                                !HasPassive() && !Player.IsDashing())
                            {
                                   Q.Cast(target);
                              //  Core.DelayAction(() => Q.Cast(target), 1);
                            }
                            if (!E.IsReady() && W.IsReady() && Player.Distance(target.Position) < W.Range &&
                                !Player.IsDashing() && !HasPassive())
                            {
                                var predW = W.GetPrediction(target);
                                if (predW.HitChance >= HitChance.High)
                                {
                                    W.Cast(predW.CastPosition);
                                    //  Core.DelayAction(() => W.Cast(predW.UnitPosition), 350);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static
            void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (!target.IsValidTarget(Q1.Range) || target == null)
            {
                return;
            }
            {
                if (LucianTheTrollMenu.Fast())
                {
                    if (!E.IsReady() && Q.IsReady() && Player.Distance(target.Position) < Q.Range && !HasPassive() &&
                        !Player.IsDashing())
                    {
                      //  Q.Cast(target);
                         Core.DelayAction(() => Q.Cast(target), 1);
                    }
                    if (!E.IsReady() && W.IsReady() &&
                        Player.Distance(target.Position) < W.Range &&
                        !Player.IsDashing() && !HasPassive())
                    {
                        var predW = W.GetPrediction(target);
                        if (predW.HitChance >= HitChance.High)
                        {
                            //   W.Cast(predW.CastPosition);
                            Core.DelayAction(() => W.Cast(predW.CastPosition), 350);
                        }
                        else
                        {
                            if (target.IsValidTarget(300))
                            {
                                Core.DelayAction(() => W.Cast(target.Position), 350);
                            }
                        }
                    }
                }
            }
        }
    }
}