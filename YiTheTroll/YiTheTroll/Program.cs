using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using YiTheTroll.Utility;
using Activator = YiTheTroll.Utility.Activator;

namespace YiTheTroll

{
    public static class Program
    {
        public const float SmiteRange = 570;
        public static string Version = "Version 1 27/6/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Active E;
        public static Spell.Active R;
        public static bool Out;
        public static int CurrentSkin;
        public static bool Healing;
        public static bool Channeling;
        public static List<string> EvadeMenu = new List<string>();


        public static readonly AIHeroClient Player = ObjectManager.Player;


        public static readonly string[] BuffsThatActuallyMakeSenseToSmite =
        {
            "SRU_Red", "SRU_Blue", "SRU_Dragon_Water", "SRU_Dragon_Fire", "SRU_Dragon_Earth", "SRU_Dragon_Air",
            "SRU_Dragon_Elder",
            "SRU_Baron", "SRU_RiftHerald", "TT_Spiderboss"
        };

        public static readonly string[] MonstersNames =
        {
            "SRU_Dragon_Water", "SRU_Dragon_Fire", "SRU_Dragon_Earth", "SRU_Dragon_Air", "SRU_Dragon_Elder", "Sru_Crab",
            "SRU_Baron", "SRU_RiftHerald",
            "SRU_Red", "SRU_Blue", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak",
            "TT_Spiderboss", "TTNGolem", "TTNWolf", "TTNWraith"
        };


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        public static float SmiteDmgMonster(Obj_AI_Base target)
        {
            return Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
        }

        public static float SmiteDmgHero(AIHeroClient target)
        {
            return Player.CalculateDamageOnUnit(target, DamageType.True,
                20.0f + Player.Level*8.0f);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "MasterYi") return;
            Chat.Print("Yi The Troll!!", Color.LawnGreen);
            Chat.Print("Loaded Version 1 (27-6-2016)", Color.LawnGreen);
            Chat.Print("GL And Dont Feed!!", Color.Red);
            YiTheTrollMenu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Active(SpellSlot.R);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (YiTheTrollMenu.Nodraw()) return;

            {
                if (YiTheTrollMenu.DrawingsQ())
                {
                    new Circle {Color = Color.LawnGreen, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (YiTheTrollMenu.DrawingsW())
                {
                    new Circle {Color = Color.LawnGreen, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (YiTheTrollMenu.DrawingsE())
                {
                    new Circle {Color = Color.LawnGreen, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (YiTheTrollMenu.DrawingsR())
                {
                    new Circle {Color = Color.LawnGreen, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Barrier != null)
                Barrier();
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (Activator.Smite != null)
                Smite();

            if (YiTheTrollMenu.CheckSkin())
            {
                if (YiTheTrollMenu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(YiTheTrollMenu.SkinId());
                    CurrentSkin = YiTheTrollMenu.SkinId();
                }
            }
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Meditate")
            {
                Healing = true;
                Channeling = true;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Out = true;
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "Meditate")
            {
                Healing = false;
                Channeling = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Out = false;
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (YiTheTrollMenu.GapcloserQ() && sender.IsEnemy && sender.IsValidTarget(Q.Range) &&
                e.End.Distance(Player) <= 250)
            {
                Q.Cast(e.End);
            }
        }

        private static void Barrier()
        {
            if (Player.IsFacing(Target) && Activator.Barrier.IsReady() &&
                Player.HealthPercent <= YiTheTrollMenu.SpellsBarrierHp())
                Activator.Barrier.Cast();
        }

        private static void Smite()
        {
            var unit =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        a =>
                            MonstersNames.Contains(a.BaseSkinName) &&
                            Player.GetSummonerSpellDamage(a, DamageLibrary.SummonerSpells.Smite) >= a.Health &&
                            YiTheTrollMenu.SmiteMenu[a.BaseSkinName].Cast<CheckBox>().CurrentValue &&
                            Activator.Smite.IsInRange(a))
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault();
            if (unit != null && Activator.Smite.IsReady())
                Activator.Smite.Cast(unit);
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= YiTheTrollMenu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= YiTheTrollMenu.SpellsHealHp()
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
            }
            KillSteal();
            AutoPotions();
            SmartR();
        }

        private static
            void AutoPotions()
        {
            if (YiTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= YiTheTrollMenu.SpellsPotionsHp() &&
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
            if (YiTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= YiTheTrollMenu.SpellsPotionsM() &&
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

            if (args.Buff.Type == BuffType.Taunt && YiTheTrollMenu.Taunt())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && YiTheTrollMenu.Stun())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && YiTheTrollMenu.Snare())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && YiTheTrollMenu.Polymorph())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && YiTheTrollMenu.Blind())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && YiTheTrollMenu.Fear())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && YiTheTrollMenu.Charm())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && YiTheTrollMenu.Suppression())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && YiTheTrollMenu.Silence())
            {
                DoQss();
            }
        }

        private static void DoQss()
        {
            if (Activator.Qss.IsOwned() && Activator.Qss.IsReady())
            {
                Activator.Qss.Cast();
            }

            if (Activator.Mercurial.IsOwned() && Activator.Mercurial.IsReady())
            {
                Activator.Mercurial.Cast();
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && EvadeMenu.Any(el => el == args.SData.Name) &&
                Player.Distance(sender) <= args.SData.CastRange)
            {
                if (Q.IsReady() &&
                    (YiTheTrollMenu.EvadeMenu[args.SData.Name].Cast<Slider>().CurrentValue == 1 ||
                     YiTheTrollMenu.EvadeMenu[args.SData.Name].Cast<Slider>().CurrentValue == 3))
                {
                    if (args.SData.Name == "JaxCounterStrike")
                    {
                        Core.DelayAction(() => Q.Cast(Target), 2000 - Game.Ping - 100);
                        return;
                    }

                    if (args.SData.Name == "KarthusFallenOne")
                    {
                        Core.DelayAction(() => Q.Cast(Target), 3000 - Game.Ping - 100);
                        return;
                    }

                    if (args.SData.Name == "ZedR" && args.Target.IsMe)
                    {
                        Core.DelayAction(() => Q.Cast(Target), 750 - Game.Ping - 100);
                        return;
                    }

                    if (args.SData.Name == "SoulShackles")
                    {
                        Core.DelayAction(() => Q.Cast(Target), 3000 - Game.Ping - 100);
                        return;
                    }

                    if (args.SData.Name == "AbsoluteZero")
                    {
                        Core.DelayAction(() => Q.Cast(Target), 3000 - Game.Ping - 100);
                        return;
                    }

                    if (args.SData.Name == "NocturneUnspeakableHorror" && args.Target.IsMe)
                    {
                        Core.DelayAction(() => Q.Cast(Target), 2000 - Game.Ping - 100);
                        return;
                    }

                    Core.DelayAction(delegate
                    {
                        if (Target != null && Target.IsValidTarget(Q.Range)) Q.Cast(Target);
                    }, (int) args.SData.SpellCastTime - Game.Ping - 100);

                    Core.DelayAction(delegate
                    {
                        if (sender.IsValidTarget(Q.Range)) Q.Cast(sender);
                    }, (int) args.SData.SpellCastTime - Game.Ping - 50);
                }

                else if (W.IsReady() && Player.IsFacing(sender) &&
                         YiTheTrollMenu.EvadeMenu[args.SData.Name].Cast<Slider>().CurrentValue > 1 &&
                         (args.Target != null && args.Target.IsMe ||
                          new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth).IsInside(Player) ||
                          new Geometry.Polygon.Circle(args.End, args.SData.CastRadius).IsInside(Player)))
                {
                    var delay =
                        (int)
                            (Player.Distance(sender)/((args.SData.MissileMaxSpeed + args.SData.MissileMinSpeed)/2)*
                             1000) - 150 + (int) args.SData.SpellCastTime;

                    if (args.SData.Name != "ZedR" && args.SData.Name != "NocturneUnpeakableHorror")
                    {
                        Core.DelayAction(() => W.Cast(), delay);
                        if (Target != null && Target.IsValidTarget())
                            Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, Target),
                                delay + 100);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent)
                .Where(
                    a =>
                        !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range && !a.IsDead && !a.IsZombie &&
                        a.HealthPercent <= 35);
            foreach (
                var target in
                    enemies)
            {
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (YiTheTrollMenu.KillstealQ() && Q.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.Q))
                {
                    Q.Cast(target);
                }
                if (target.IsValidTarget(570) && target.Health < SmiteDmgHero(target) && YiTheTrollMenu.KillstealSmite() &&
                    Activator.Smite.IsReady())
                {
                    Activator.Smite.Cast(target);
                }
            }
        }

        private static void OnLaneClear()
        {
            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition,
                    Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(Q.Range));
            if (count == 0) return;
            if (YiTheTrollMenu.LaneQ() && Q.IsReady() && Player.ManaPercent > YiTheTrollMenu.LaneMana())
            {
                Q.Cast(source);
            }
            if (YiTheTrollMenu.LaneE() && E.IsReady() && Player.ManaPercent > YiTheTrollMenu.LaneMana())
            {
                E.Cast();
            }
        }

        private static
            void OnJungle()
        {
            var junleminions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(900));

            if (YiTheTrollMenu.JungleQ() && Q.IsReady() && Player.ManaPercent > YiTheTrollMenu.Junglemana() &&
                junleminions.IsValidTarget(Q.Range))
            {
                Q.Cast(junleminions);
            }
            if (YiTheTrollMenu.JungleE() && E.IsReady() && Player.ManaPercent > YiTheTrollMenu.Junglemana() &&
                junleminions.IsValidTarget(E.Range))
            {
                E.Cast();
            }
            if (YiTheTrollMenu.JungleW() && W.IsReady() && Player.ManaPercent > YiTheTrollMenu.Junglemana() &&
                Player.HealthPercent < 50 &&
                junleminions.IsValidTarget(Q.Range))
            {
                W.Cast();
            }
        }

        private static void OnHarrass()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady() && target.IsValidTarget(700))
                foreach (var eenemies in enemies)
                {
                    var useQ = YiTheTrollMenu.HarassMeNu["harass.Q"
                                                         + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ && Player.ManaPercent >= YiTheTrollMenu.HarassQe())
                    {
                        Q.Cast(target);
                    }
                }
        }

        private static
            void SmartR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (!target.IsValidTarget(R.Range) || target == null)
            {
                return;
            }
            if (R.IsReady() && YiTheTrollMenu.ComboR() && Player.CountEnemiesInRange(1000) == 1 &&
                target.HealthPercent <= 45 && !target.IsInvulnerable)
            {
                R.Cast();
            }
        }

        private static
            void OnCombo()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (E.IsReady() && target.IsValidTarget(Q.Range) && YiTheTrollMenu.ComboE())
            {
                E.Cast();
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = YiTheTrollMenu.ComboMenu["combo.q"
                                                        + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ)
                    {
                        Q.Cast(target);
                    }
                }
            if (YiTheTrollMenu.ComboW() && Player.HealthPercent < YiTheTrollMenu.Hpw() && W.IsReady() &&
                target.IsValidTarget(W.Range) && !target.IsInvulnerable)
            {
                W.Cast();
            }
            if (YiTheTrollMenu.ResetAa() && W.IsReady() && target.IsValidTarget(250) && !target.IsInvulnerable)
            {
                W.Cast();
                Orbwalker.ResetAutoAttack();
            }
            if (R.IsReady() && YiTheTrollMenu.ComboR1() &&
                Player.CountEnemiesInRange(1000) == YiTheTrollMenu.ComboREnemies() && !target.IsInvulnerable)
            {
                R.Cast();
            }
            if ((ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >=
                 YiTheTrollMenu.YoumusEnemies() ||
                 Player.HealthPercent >= YiTheTrollMenu.ItemsYoumuShp()) &&
                Activator.Youmus.IsReady() && YiTheTrollMenu.Youmus() && Activator.Youmus.IsOwned())
            {
                Activator.Youmus.Cast();
                return;
            }
            if (Player.HealthPercent <= YiTheTrollMenu.BilgewaterHp() &&
                YiTheTrollMenu.Bilgewater() &&
                Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
            {
                Activator.Bilgewater.Cast(target);
                return;
            }
            if (Activator.Smite.IsReady() && YiTheTrollMenu.Combosmite())
            {
                Activator.Smite.Cast(target);
            }
            if (Player.HealthPercent <= YiTheTrollMenu.BotrkHp() && YiTheTrollMenu.Botrk() &&
                Activator.Botrk.IsReady() &&
                Activator.Botrk.IsOwned())
            {
                Activator.Botrk.Cast(target);
            }
        }
    }
}