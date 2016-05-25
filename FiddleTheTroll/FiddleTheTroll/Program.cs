using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using FiddleTheTroll.Utility;
using Activator = FiddleTheTroll.Utility.Activator;

namespace FiddleTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1 25/5/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Targeted Q;
        public static Spell.Targeted W;
        public static Spell.Targeted E;
        public static Spell.Skillshot E1;
        public static Spell.Skillshot R;
        public static bool Out;
        public static int CurrentSkin;
        public static bool Channeling;
        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Hero != Champion.FiddleSticks)
            {
                return;
            }
            Chat.Print(
                "<font color=\"#6ce510\" >MeLoDag Presents </font><font color=\"#fffffff\" >Fiddlesticks </font><font color=\"#6ce510\" >Kappa Kippo</font>");
            FiddleTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 575);
            W = new Spell.Targeted(SpellSlot.W, 575);
            E = new Spell.Targeted(SpellSlot.E, 750);
            R = new Spell.Skillshot(SpellSlot.R, 800, SkillShotType.Circular);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (FiddleTheTrollMeNu.Nodraw()) return;

            {
                if (FiddleTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.LawnGreen, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (FiddleTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.LawnGreen, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (FiddleTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.LawnGreen, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (FiddleTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.LawnGreen, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    FiddleTheTrollMeNu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = FiddleTheTrollMeNu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (FiddleTheTrollMeNu.CheckSkin())
            {
                if (FiddleTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(FiddleTheTrollMeNu.SkinId());
                    CurrentSkin = FiddleTheTrollMeNu.SkinId();
                }
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy) return;

            if (sender.IsValidTarget(Q.Range) && FiddleTheTrollMeNu.GapcloserQ() && Player.Distance(e.End) < 150)
            {
                Q.Cast(sender);
            }
            if (sender.IsValidTarget(E.Range) && FiddleTheTrollMeNu.GapcloserE() && Player.Distance(e.End) < 150)
            {
                E.Cast(sender);
            }
        }

        public static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsEnemy) return;
            if (e.DangerLevel == DangerLevel.High && FiddleTheTrollMeNu.InterupteQ())
            {
                Q.Cast(sender);
            }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= FiddleTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= FiddleTheTrollMeNu.SpellsHealHp()
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
            AutoCc();
            AutoPotions();
            AutoHourglass();
            UseRTarget();
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "Drain")
            {
                Channeling = true;
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Out = true;
                Channeling = true;
            }
        }

        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "Drain")
            {
                Channeling = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Out = false;
            }
        }

        private static
            void AutoHourglass()
        {
            var zhonyas = FiddleTheTrollMeNu.Activator["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = FiddleTheTrollMeNu.Activator["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && Activator.ZhonyaHourglass.IsReady())
            {
                Activator.ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPotions()
        {
            if (FiddleTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= FiddleTheTrollMeNu.SpellsPotionsHp() &&
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
            if (FiddleTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= FiddleTheTrollMeNu.SpellsPotionsM() &&
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
        public static
           void UseRTarget()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target != null &&
                (FiddleTheTrollMeNu.ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && R.IsReady() && target.IsValid &&
                 !Player.HasBuff("FiddlesticksR"))) R.Cast(target.Position);
        }

        private static void KillSteal()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent)
                .Where(
                    a =>
                        !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= E.Range && !a.IsDead && !a.IsZombie &&
                        a.HealthPercent <= 35);
            foreach (
                var target in
                    enemies)
            {
                if (!target.IsValidTarget())
                {
                    return;
                }
                if (FiddleTheTrollMeNu.KillstealE() && E.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.E))
                {
                    Q.Cast(target);
                    Chat.Print("Use E ks", Color.GreenYellow);
                }
            }
        }


        private static
            void AutoCc()
        {
            if (!FiddleTheTrollMeNu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
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
            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.ServerPosition,
                    Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(Q.Range));
            if (count == 0) return;
            if (E.IsReady() && FiddleTheTrollMeNu.LaneE() && Player.ManaPercent > FiddleTheTrollMeNu.LaneMana())
            {
                E.Cast(source);
            }
        }

        private static
            void OnJungle()
        {
            var junleminions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(900));

            if (FiddleTheTrollMeNu.JungleE() && E.IsReady() && junleminions.IsValidTarget(E.Range))
            {
                E.Cast(junleminions);
            }
            if (FiddleTheTrollMeNu.JungleQ() && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
            {
                Q.Cast(junleminions);
            }
            if (FiddleTheTrollMeNu.JungleW() && W.IsReady() && E.IsOnCooldown && Q.IsOnCooldown && junleminions.IsValidTarget(W.Range))
            {
                W.Cast(junleminions);
            }
        }

        private static
            void OnHarrass()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= E.Range);
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in enemies)
                {
                    var useE = FiddleTheTrollMeNu.HarassMeNu["harass.E"
                                                             + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE && Player.ManaPercent > FiddleTheTrollMeNu.HarassQe())
                    {
                        E.Cast(target);
                    }
                }
        }

        private static
            void OnCombo()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= Q.Range);
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in enemies)
                {
                    var useE = FiddleTheTrollMeNu.ComboMenu["combo.E"
                                                            + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE)
                    {
                        E.Cast(target);
                    }
                }
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = FiddleTheTrollMeNu.ComboMenu["combo.Q"
                                                            + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ)
                    {
                        Q.Cast(target);
                    }
                }
            if (FiddleTheTrollMeNu.ComboW() && W.IsReady() && Q.IsOnCooldown && E.IsOnCooldown &&
                target.IsValidTarget(W.Range) && !target.IsInvulnerable)
            {
                W.Cast(target);
            }
         /*   if (FiddleTheTrollMeNu.ComboR() && Player.CountEnemiesInRange(R.Range) >= FiddleTheTrollMeNu.ComboREnemies() &&
                R.IsReady())
            {
                Core.DelayAction(() => R.Cast(target.Position), 250);
            } */
        }
    }
}