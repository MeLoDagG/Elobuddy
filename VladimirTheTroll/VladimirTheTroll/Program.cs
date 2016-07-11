using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using VladimirTheTroll.Utility;
using Activator = VladimirTheTroll.Utility.Activator;

namespace VladimirTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1.4 11/7/2016";
        public static AIHeroClient Target = null;
        public static Spell.Targeted Q;
        public static Spell.Chargeable E;
        public static Spell.Skillshot W;
        public static Spell.Active W1;
        public static Spell.Skillshot R;
        public static int CurrentSkin;

        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }


        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.ChampionName != "Vladimir") return;
            Chat.Print(
                "<font color=\"#d80303\" >MeLoDag Presents </font><font color=\"#ffffff\" > Vladimir </font><font color=\"#d80303\" >Kappa Kippo</font>");
            Chat.Print("Version Loaded 1.4 (11/7/2016)", Color.Red);
            Chat.Print("HF Gl And Dont Feed Kappa!!!", Color.Red);
            VladimirTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Skillshot(SpellSlot.W, 600, SkillShotType.Circular);
            W1 = new Spell.Active(SpellSlot.W, 600);
            E = new Spell.Chargeable(SpellSlot.E, 600, 600, 1250, 0, 1500, 70);
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 150);

            #endregion

            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (VladimirTheTrollMeNu.Nodraw()) return;

            {
                if (VladimirTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (VladimirTheTrollMeNu.DrawingsR())
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
            if (VladimirTheTrollMeNu.CheckSkin())
            {
                if (VladimirTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(VladimirTheTrollMeNu.SkinId());
                    CurrentSkin = VladimirTheTrollMeNu.SkinId();
                }
            }
        }


        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= VladimirTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= VladimirTheTrollMeNu.SpellsHealHp()
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
                UseE();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                OnHarrass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Lasthit();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Lasthit();
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
            AutoHarass();
            AutoHourglass();
        }


        private static
            void AutoHourglass()
        {
            var zhonyas = VladimirTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = VladimirTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (VladimirTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= VladimirTheTrollMeNu.SpellsPotionsHp() &&
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
            if (VladimirTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= VladimirTheTrollMeNu.SpellsPotionsM() &&
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


        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                        if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
                            {
                                if ((args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E ||
                                     args.Slot == SpellSlot.R) && sender.IsEnemy && W.IsReady())
                                {
                                    if (args.SData.TargettingType == SpellDataTargetType.Unit ||
                                        args.SData.TargettingType == SpellDataTargetType.SelfAndUnit ||
                                        args.SData.TargettingType == SpellDataTargetType.Self)
                                    {
                                        if ((args.Target.NetworkId == Player.NetworkId && args.Time < 1.5 ||
                                             args.End.Distance(Player.ServerPosition) <= Player.BoundingRadius*3) &&
                                            VladimirTheTrollMeNu.EvadeMenu[args.SData.Name].Cast<CheckBox>()
                                                .CurrentValue)
                                        {
                                            W1.Cast();
                                        }
                                    }
                                    else if (args.SData.TargettingType == SpellDataTargetType.LocationAoe)
                                    {
                                        var castvector =
                                            new Geometry.Polygon.Circle(args.End, args.SData.CastRadius).IsInside(
                                                Player.ServerPosition);

                                        if (castvector &&
                                            VladimirTheTrollMeNu.EvadeMenu[args.SData.Name].Cast<CheckBox>()
                                                .CurrentValue)
                                        {
                                            W1.Cast();
                                        }
                                    }

                                    else if (args.SData.TargettingType == SpellDataTargetType.Cone)
                                    {
                                        var castvector =
                                            new Geometry.Polygon.Arc(args.Start, args.End, args.SData.CastConeAngle,
                                                args.SData.CastRange)
                                                .IsInside(Player.ServerPosition);

                                        if (castvector &&
                                            VladimirTheTrollMeNu.EvadeMenu[args.SData.Name].Cast<CheckBox>()
                                                .CurrentValue)
                                        {
                                            W1.Cast();
                                        }
                                    }

                                    else if (args.SData.TargettingType == SpellDataTargetType.SelfAoe)
                                    {
                                        var castvector =
                                            new Geometry.Polygon.Circle(sender.ServerPosition, args.SData.CastRadius)
                                                .IsInside(
                                                    Player.ServerPosition);

                                        if (castvector &&
                                            VladimirTheTrollMeNu.EvadeMenu[args.SData.Name].Cast<CheckBox>()
                                                .CurrentValue)
                                        {
                                            W1.Cast();
                                        }
                                    }
                                    else
                                    {
                                        var castvector =
                                            new Geometry.Polygon.Rectangle(args.Start, args.End, args.SData.LineWidth)
                                                .IsInside(
                                                    Player.ServerPosition);

                                        if (castvector &&
                                            VladimirTheTrollMeNu.EvadeMenu[args.SData.Name].Cast<CheckBox>()
                                                .CurrentValue)
                                        {
                                            W1.Cast();
                                        }
                                    }
                                }
                            }
        }

        private static void KillSteal()
        {
            var ksQ = VladimirTheTrollMeNu.HarassMeNu["ksQ"].Cast<CheckBox>().CurrentValue;

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (ksQ && Q.IsReady() &&
                    SpellDamage.QDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player) <= Q.Range)
                {
                    Q.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Q Free Kill<font>");
                }
            }
        }

        private static void Lasthit()
        {
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.Health <= SpellDamage.QDamage(m) &&
                            m.IsValidTarget());
            if (Q.IsReady() && VladimirTheTrollMeNu.LastHitQ() && qminion != null)
            {
                Q.Cast(qminion);
            }
        }

        private static
            void OnJungle()
        {
            var useQ = VladimirTheTrollMeNu.FarmMeNu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useE = VladimirTheTrollMeNu.FarmMeNu["useEJungle"].Cast<CheckBox>().CurrentValue;

            {
                var junleminions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(a => a.MaxHealth)
                        .FirstOrDefault(a => a.IsValidTarget(900));

                if (useQ && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
                {
                    Q.Cast(junleminions);
                }
                if (useE && E.IsReady() && junleminions.IsValidTarget(450))
                {
                    if (E.IsCharging)
                    {
                        E.Cast(Game.CursorPos);
                    }
                    E.StartCharging();
                }
            }
        }

        private static
            void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;
            var autoQharass = VladimirTheTrollMeNu.HarassMeNu["useQAuto"].Cast<CheckBox>().CurrentValue;

            {
                if (Q.IsReady() && autoQharass)
                {
                    Q.Cast(target);
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

            var useQh = VladimirTheTrollMeNu.HarassMeNu["useQHarass"].Cast<CheckBox>().CurrentValue;

            {
                if (Q.IsReady() && useQh)
                {
                    Q.Cast(target);
                }
            }
        }

        private static
            void UseE()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            var useE = VladimirTheTrollMeNu.ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            {
                if (useE && E.IsReady() && target.IsValidTarget(350))
                {
                    if (E.IsCharging)
                    {
                        E.Cast(Game.CursorPos);
                    }
                    E.StartCharging();
                }
            }
        }


        private static
            void OnCombo()
        {
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            var useWcostumHp = VladimirTheTrollMeNu.ComboMenu["useWcostumHP"].Cast<Slider>().CurrentValue;
            var useQ = VladimirTheTrollMeNu.ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = VladimirTheTrollMeNu.ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = VladimirTheTrollMeNu.ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var rCount = VladimirTheTrollMeNu.ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            {
                if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
                if (W.IsReady() && useW && Player.HealthPercent <= useWcostumHp)
                {
                    W.Cast(Game.CursorPos);
                }
                if (R.IsReady() && Player.CountEnemiesInRange(R.Range) >= rCount && useR)
                {
                    R.Cast(target);
                }
            }
        }
    }
}