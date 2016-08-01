using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using SwainTheTroll.Utility;
using Activator = SwainTheTroll.Utility.Activator;
using Color = System.Drawing.Color;

namespace SwainTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1 1/8/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted E;
        public static Spell.Active R;
        public static bool Out = false;
        public static int CurrentSkin;
        public static bool RavenForm;

        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }


        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "Swain") return;
            Chat.Print("Swain The Troll Loaded!!", Color.Gold);
            Chat.Print("Version 1 (1/8/2016", Color.Gold);
            Chat.Print("HF And Dont Troll And Feed Kappa!!", Color.Gold);

            SwainTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Skillshot(SpellSlot.Q, 700, SkillShotType.Circular, 250, 1250, 325);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 500, 1250, 125);
            E = new Spell.Targeted(SpellSlot.E, 625);
            R = new Spell.Active(SpellSlot.R, 700);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += GameOnDraw;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (SwainTheTrollMeNu.Nodraw()) return;

            {
                if (SwainTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Gold, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (SwainTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Gold, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (SwainTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Gold, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (SwainTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.Gold, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    SwainTheTrollMeNu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = SwainTheTrollMeNu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (SwainTheTrollMeNu.CheckSkin())
            {
                if (SwainTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(SwainTheTrollMeNu.SkinId());
                    CurrentSkin = SwainTheTrollMeNu.SkinId();
                }
            }
        }


        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= SwainTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= SwainTheTrollMeNu.SpellsHealHp()
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
                Farmerama();
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

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = true;
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = false;
        }

        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var value = SwainTheTrollMeNu.MiscMeNu["interrupt.value"].Cast<ComboBox>().CurrentValue;
            var danger = value == 0
                ? DangerLevel.High
                : value == 1 ? DangerLevel.Medium : value == 2 ? DangerLevel.Low : DangerLevel.High;
            if (sender.IsEnemy
                && SwainTheTrollMeNu.MiscMeNu["interrupter"].Cast<CheckBox>().CurrentValue
                && sender.IsValidTarget(W.Range)
                && e.DangerLevel == danger)
            {
                W.Cast(sender);
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!e.Sender.IsValidTarget() || !SwainTheTrollMeNu.MiscMeNu["gapcloser.W"].Cast<CheckBox>().CurrentValue ||
                e.Sender.Type != Player.Type ||
                !e.Sender.IsEnemy || e.Sender.IsAlly)
            {
                return;
            }
            W.Cast(e.Sender.ServerPosition);
        }


        private static
            void AutoHourglass()
        {
            var zhonyas = SwainTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = SwainTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (SwainTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= SwainTheTrollMeNu.SpellsPotionsHp() &&
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
            if (SwainTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= SwainTheTrollMeNu.SpellsPotionsM() &&
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
            var ksE = SwainTheTrollMeNu.HarassMeNu["ksE"].Cast<CheckBox>().CurrentValue;
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
                if (ksE && E.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.E))

                {
                    E.Cast(target);
                }

            }
        }

        private static
            void AutoCc()
        {
            if (!SwainTheTrollMeNu.ComboMenu["combo.CC"].Cast<CheckBox>().CurrentValue)
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
                W.Cast(autoTarget.ServerPosition);
            }
            if (!SwainTheTrollMeNu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            if (autoTarget != null)
            {
                Q.Cast(autoTarget.ServerPosition);
            }
        }


        private static void Farmerama()
        {
            var qFarmAlways = SwainTheTrollMeNu.FarmMeNu["qFarmAlways"].Cast<CheckBox>().CurrentValue;
            var wFarmAlways = SwainTheTrollMeNu.FarmMeNu["wFarm"].Cast<CheckBox>().CurrentValue;
            var eFarmAlways = SwainTheTrollMeNu.FarmMeNu["EFarm"].Cast<CheckBox>().CurrentValue;
            var lenamana = SwainTheTrollMeNu.FarmMeNu["LaneMana"].Cast<Slider>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.IsValidTarget());

            if (Q.IsReady() && qFarmAlways && Player.ManaPercent > lenamana && qminion != null &&
                !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
            if (W.IsReady() && wFarmAlways && Player.ManaPercent > lenamana && qminion != null &&
                !Orbwalker.IsAutoAttacking)
            {
                W.Cast(qminion);
            }
            if (E.IsReady() && eFarmAlways && Player.ManaPercent > lenamana && qminion != null &&
                !Orbwalker.IsAutoAttacking)
            {
                E.Cast(qminion);
            }
        }

        private static
            void OnJungle()
        {
            var useQJungle = SwainTheTrollMeNu.FarmMeNu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useWJungle = SwainTheTrollMeNu.FarmMeNu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useEJungle = SwainTheTrollMeNu.FarmMeNu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var junglemana = SwainTheTrollMeNu.FarmMeNu["JungleMana"].Cast<Slider>().CurrentValue;

            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && useQJungle && Player.ManaPercent > junglemana && minion != null)
                {
                    Q.Cast(minion);
                }
                if (W.IsReady() && useWJungle && Player.ManaPercent > junglemana && minion != null)
                {
                    W.Cast(minion);
                }
                if (E.IsReady() && useEJungle && Player.ManaPercent > junglemana && minion != null)
                {
                    E.Cast(minion);
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

            var useEh = SwainTheTrollMeNu.HarassMeNu["useEHarass"].Cast<CheckBox>().CurrentValue;
            var useQh = SwainTheTrollMeNu.HarassMeNu["useQharass"].Cast<CheckBox>().CurrentValue;
            var Harassmana = SwainTheTrollMeNu.HarassMeNu["HarassMana"].Cast<Slider>().CurrentValue;

            {
                if (E.IsReady() && Player.ManaPercent > Harassmana && useEh)
                {
                    E.Cast(target);
                }
                if (Q.IsReady() && Player.ManaPercent > Harassmana && useQh)
                {
                    var predQ = Q.GetPrediction(target);
                    if (predQ.HitChance >= HitChance.Medium)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                    else if (predQ.HitChance >= HitChance.Immobile)
                    {
                        Q.Cast(predQ.CastPosition);
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
            if (R.IsReady() && SwainTheTrollMeNu.ComboMenu["Rlogic"].Cast<CheckBox>().CurrentValue &&
                Player.CountEnemiesInRange(1000) == 1 &&
                target.HealthPercent <= 80 && !RavenForm)
            {
                R.Cast();
            }
            else if (RavenForm && target.HealthPercent <= 5)
            {
                R.Cast();
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
            var useQ = SwainTheTrollMeNu.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = SwainTheTrollMeNu.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = SwainTheTrollMeNu.ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var cancelR = SwainTheTrollMeNu.ComboMenu["useRCombo1"].Cast<CheckBox>().CurrentValue;
            var rCount = SwainTheTrollMeNu.ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var useE = SwainTheTrollMeNu.ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            {
                if (useE && E.IsReady() && target.IsValidTarget(E.Range) && !target.IsInvulnerable)
                {
                    E.Cast(target);
                }
                if (W.IsReady() && useW && target.IsValidTarget(W.Range) && !target.IsInvulnerable)
                {
                    var predw = W.GetPrediction(target);
                    if (predw.HitChance >= HitChance.High)
                    {
                        W.Cast(predw.CastPosition);
                    }
                }
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range) && !target.IsInvulnerable)
                {
                    var predQ = Q.GetPrediction(target);
                    if (predQ.HitChance >= HitChance.High)
                    {
                        Q.Cast(predQ.CastPosition);
                    }
                }
                if (R.IsReady() && Player.CountEnemiesInRange(1200) >= rCount && useR && !RavenForm)
                {
                    R.Cast();
                }
                if (cancelR && RavenForm && target.HealthPercent <= 5)
                {
                   {
                        R.Cast();
                    }
                }
            }
        }
    }
}