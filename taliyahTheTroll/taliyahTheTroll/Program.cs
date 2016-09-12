using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using taliyahTheTroll.Utility;
using Activator = taliyahTheTroll.Utility.Activator;
using Color = System.Drawing.Color;

namespace taliyahTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1.2 12/9/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static bool Out = false;
        public static int CurrentSkin;


        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "Taliyah") return;
            Chat.Print(
                "<font color=\"#d80303\" >MeLoDag Presents </font><font color=\"#fffffff\" >Taliyah </font><font color=\"#d80303\" >Kappa Kippo</font>");
            Chat.Print("Version 1.2 12 / 9 / 2016", Color.Chartreuse);
            TalliyahTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 2000, 60);
            {
                Q.AllowedCollisionCount = 0;
            }
            W = new Spell.Skillshot(SpellSlot.W, 800, SkillShotType.Circular, 250, int.MaxValue, 180);
            E = new Spell.Skillshot(SpellSlot.E, 700, SkillShotType.Cone);
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear);


            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }
        

        private static
            void GameOnDraw(EventArgs args)
        {
            if (TalliyahTheTrollMeNu.Nodraw()) return;

            {
                if (TalliyahTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TalliyahTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TalliyahTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TalliyahTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    TalliyahTheTrollMeNu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = TalliyahTheTrollMeNu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (TalliyahTheTrollMeNu.CheckSkin())
            {
                if (TalliyahTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(TalliyahTheTrollMeNu.SkinId());
                    CurrentSkin = TalliyahTheTrollMeNu.SkinId();
                }
            }
        }

        private static
            void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!e.Sender.IsValidTarget() || !TalliyahTheTrollMeNu.GapcloserE() || e.Sender.Type != Player.Type ||
                !e.Sender.IsEnemy || e.Sender.IsAlly)
            {
                return;
            }

            E.Cast(e.Sender.ServerPosition);
        }

        public static void Interupt(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (interruptableSpellEventArgs.DangerLevel >= DangerLevel.High && !TalliyahTheTrollMeNu.InterupteW() &&
                W.IsReady())
            {
                W.Cast(sender.Position);
            }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= TalliyahTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= TalliyahTheTrollMeNu.SpellsHealHp()
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
                KillSteal();
            AutoCc();
            AutoPotions();
            AutoHourglass();
        }

        private static
            void AutoHourglass()
        {
            var zhonyas = TalliyahTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = TalliyahTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (TalliyahTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= TalliyahTheTrollMeNu.SpellsPotionsHp() &&
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
            if (TalliyahTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= TalliyahTheTrollMeNu.SpellsPotionsM() &&
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

                if (TalliyahTheTrollMeNu.KillstealQ() && Q.IsReady() &&
                    target.Health <= 20 &&
                    target.Distance(Player) <= Q.Range)
                {
                    Q.Cast(target.Position);
                    Chat.Print("Use Q ks");
                }
            }
        }


        private static
            void AutoCc()
        {
            if (!TalliyahTheTrollMeNu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            var autoTarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup) ||
                        x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression) ||
                        x.HasBuffOfType(BuffType.Slow) ||
                        x.HasBuffOfType(BuffType.Snare));
            if (autoTarget != null)
            {
                Q.Cast(autoTarget.ServerPosition);
            }
        }

        private static void OnLaneClear()
        {
            if (Q.IsReady() && TalliyahTheTrollMeNu.LaneQ() && Player.ManaPercent > TalliyahTheTrollMeNu.LaneMana())
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t =>
                            t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                            t.IsInRange(Player.Position, Q.Range));
                foreach (var m in minions)
                {
                    if (
                        Q.GetPrediction(m)
                            .CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable)
                            .Count() >= 0)
                    {
                        Q.Cast(m);
                        break;
                    }
                }
            }
        }

        private static
            void OnJungle()
        {
            if (TalliyahTheTrollMeNu.JungleQ())
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Position, Q.Range)
                        .Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                if (minions.Count() > 0)
                {
                    Q.Cast(minions.First());
                }
            }
        }

        private static
            void OnHarrass()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = TalliyahTheTrollMeNu.HarassMeNu["harass.Q"
                                                               + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ && Player.ManaPercent >= TalliyahTheTrollMeNu.HarassQe())
                    {
                        var predQharass = Q.GetPrediction(target);
                        if (predQharass.HitChance >= HitChance.High)
                        {
                            Q.Cast(predQharass.CastPosition);
                        }
                    }
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
          if (TalliyahTheTrollMeNu.ComboW() && W.IsReady() && target.IsValidTarget(W.Range) && !target.IsInvulnerable)
            {
                var pred = W.GetPrediction(target);
                if (pred.HitChancePercent >= TalliyahTheTrollMeNu.Predw())
                {
                    W.Cast(pred.UnitPosition);
                 //  Core.DelayAction(() => W.Cast(pred.UnitPosition), 50);
                   Core.DelayAction(() => W.Cast(Player.ServerPosition), 150);
                }
            }
            if (E.IsReady() && target.IsValidTarget(600) && TalliyahTheTrollMeNu.ComboE())
            {
                var predE = E.GetPrediction(target);
                if (predE.HitChancePercent >= TalliyahTheTrollMeNu.Prede())
                {
                    E.Cast(predE.UnitPosition);
                }
                else
                {
                    if (target.IsValidTarget(300))
                    {
                        E.Cast(target);
                    }
                }
            }
            if (TalliyahTheTrollMeNu.UseQonly5() && Q.IsReady() && !Player.HasBuff("Taliyah_Base_Q_aoe_bright.troy") &&
                target.IsValidTarget(Q.Range) &&
                !target.IsInvulnerable)
            {
                var predQ = Q.GetPrediction(target);
                if (predQ.HitChancePercent >= TalliyahTheTrollMeNu.Predq())
                {
                    Q.Cast(predQ.UnitPosition);
                }
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = TalliyahTheTrollMeNu.ComboMenu["combo.q"
                                                              + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ)
                    {
                        var predQ = Q.GetPrediction(target);
                        if (predQ.HitChancePercent >= TalliyahTheTrollMeNu.Predq())
                        {
                            Q.Cast(predQ.UnitPosition);
                        }
                    }
                }
        }
    }
}