using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using ZyraTheTroll.Utility;
using Activator = ZyraTheTroll.Utility.Activator;
using Color = System.Drawing.Color;

namespace ZyraTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1.1 31/7/2016";
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
            if (Player.ChampionName != "Zyra") return;
            Chat.Print(
                "<font color=\"#d80303\" >MeLoDag Presents </font><font color=\"#ffffff\" > Zyra </font><font color=\"#d80303\" >Kappa Kippo</font>");
            ZyraTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Skillshot(SpellSlot.Q, 800, SkillShotType.Cone);
            W = new Spell.Skillshot(SpellSlot.W, 820, SkillShotType.Circular, 500, int.MaxValue, 80);
            E = new Spell.Skillshot(SpellSlot.E, 1100, SkillShotType.Linear, 250, 1150, 80);
            {
                
                E.AllowedCollisionCount = 0;
            }
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 150);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (ZyraTheTrollMeNu.Nodraw()) return;

            {
                if (ZyraTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (ZyraTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (ZyraTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (ZyraTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
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
            if (ZyraTheTrollMeNu.CheckSkin())
            {
                if (ZyraTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(ZyraTheTrollMeNu.SkinId());
                    CurrentSkin = ZyraTheTrollMeNu.SkinId();
                }
            }
        }


        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= ZyraTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= ZyraTheTrollMeNu.SpellsHealHp()
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
                SmartR();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarrass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                FarmQ();
                FarmQAlways();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                OnJungle();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            KillSteal();
            AutoCc();
            AutoPotions();
            AutoHourglass();
        }

        public static void Interupt(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (interruptableSpellEventArgs.DangerLevel >= DangerLevel.High &&
                !ZyraTheTrollMeNu.MiscMeNu["interupt.E"].Cast<CheckBox>().CurrentValue &&
                E.IsReady())
            {
                E.Cast(sender.Position);
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!e.Sender.IsValidTarget() || !ZyraTheTrollMeNu.MiscMeNu["gapcloser.E"].Cast<CheckBox>().CurrentValue ||
                e.Sender.Type != Player.Type ||
                !e.Sender.IsEnemy || e.Sender.IsAlly)
            {
                return;
            }
            E.Cast(e.Sender.ServerPosition);
        }


        private static
            void AutoHourglass()
        {
            var zhonyas = ZyraTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = ZyraTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (ZyraTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= ZyraTheTrollMeNu.SpellsPotionsHp() &&
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
            if (ZyraTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= ZyraTheTrollMeNu.SpellsPotionsM() &&
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
            var ksQ = ZyraTheTrollMeNu.HarassMeNu["ksQ"].Cast<CheckBox>().CurrentValue;
            var ksE = ZyraTheTrollMeNu.HarassMeNu["ksE"].Cast<CheckBox>().CurrentValue;
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

                if (ksQ && Q.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.Q))

                {
                    Q.Cast(target.Position);
                }

                if (ksE && E.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.E))

                {
                    E.Cast(target.Position);
                }

            }
        }

        private static
            void AutoCc()
        {
            if (!ZyraTheTrollMeNu.ComboMenu["combo.CC"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            var autoTarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup) ||
                        x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression) ||
                        x.HasBuffOfType(BuffType.Snare));
            if (autoTarget != null)
            {
                E.Cast(autoTarget.ServerPosition);
            }
            if (!ZyraTheTrollMeNu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            if (autoTarget != null)
            {
                Q.Cast(autoTarget.ServerPosition);
            }
        }

        private static
            void FarmQ()
        {
            var useQ = ZyraTheTrollMeNu.FarmMeNu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.Health <= SpellDamage.QDamage(m) - 20 &&
                            m.IsValidTarget());

            if (Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }

        private static void FarmQAlways()
        {
            var qFarmAlways = ZyraTheTrollMeNu.FarmMeNu["qFarmAlways"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.IsValidTarget());

            if (Q.IsReady() && qFarmAlways && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }
        
        private static
            void OnJungle()
        {
            var useQJungle = ZyraTheTrollMeNu.FarmMeNu["useQJungle"].Cast<CheckBox>().CurrentValue;

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && useQJungle && minion != null)
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void OnHarrass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            var useEh = ZyraTheTrollMeNu.HarassMeNu["useEHarass"].Cast<CheckBox>().CurrentValue;
            var useQh = ZyraTheTrollMeNu.HarassMeNu["useQharass"].Cast<CheckBox>().CurrentValue;
            {
                if (E.IsReady() && useEh)
                {
                    var prede = E.GetPrediction(target);
                    if (prede.HitChance >= HitChance.Medium)
                    {
                        E.Cast(prede.CastPosition);
                    }
                    else if (prede.HitChance >= HitChance.Immobile)
                    {
                        E.Cast(prede.CastPosition);
                    }
                    if (Q.IsReady() && useQh)
                    {
                        var predQ = Q.GetPrediction(target);
                        if (predQ.HitChance >= HitChance.Medium)
                        {
                            Q.Cast(prede.CastPosition);
                        }
                        else if (predQ.HitChance >= HitChance.Immobile)
                        {
                            Q.Cast(predQ.CastPosition);
                        }
                    }
                }
            }
        }

        private static
            void SmartR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (!target.IsValidTarget(R.Range) || target == null)
            {
                return;
            }
            if (R.IsReady() && ZyraTheTrollMeNu.ComboMenu["Rlogic"].Cast<CheckBox>().CurrentValue && Player.CountEnemiesInRange(1000) == 1 &&
                target.HealthPercent <= 45 && !target.IsInvulnerable)
            {
                R.Cast(target);
            }
        }

        private static
            void OnCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            var useQ = ZyraTheTrollMeNu.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = ZyraTheTrollMeNu.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = ZyraTheTrollMeNu.ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var rCount = ZyraTheTrollMeNu.ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var useE = ZyraTheTrollMeNu.ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
           {
                if (useE && E.IsReady() && target.IsValidTarget(900) && !target.IsInvulnerable)
                {
                    var prede = E.GetPrediction(target);
                    if (prede.HitChance >= HitChance.High)
                    {
                        E.Cast(prede.CastPosition);
                    }
                }
                if (W.IsReady() && useW && target.IsValidTarget(800) && !target.IsInvulnerable)
                {
                    var predw = W.GetPrediction(target);
                    if (predw.HitChance >= HitChance.High)
                    {
                        W.Cast(predw.CastPosition);
                    }
                }
                if (useQ && Q.IsReady() && target.IsValidTarget(700) && !target.IsInvulnerable)
                {
                    var predQ = Q.GetPrediction(target);
                    if (predQ.HitChance >= HitChance.High)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }
                if (R.IsReady() && Player.CountEnemiesInRange(1200) >= rCount && useR)
                {
                    var predR = R.GetPrediction(target);
                    if (predR.HitChance >= HitChance.High)
                    {
                        R.Cast(predR.CastPosition);
                    }
                }
             }
        }
    }
}