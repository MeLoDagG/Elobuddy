using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using MissFortuneTheTroll.Utility;
using SharpDX;
using Activator = MissFortuneTheTroll.Utility.Activator;
using Color = System.Drawing.Color;

namespace MissFortuneTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1.1 (16/7/2016)";
        public static AIHeroClient castOn = null;
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static bool Channeling ;
        public static bool Out ;
        public static int CurrentSkin;
        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Hero != Champion.MissFortune)
            {
                return;
            }
            Chat.Print("Miss Fortune The Troll Loaded!", Color.Red);
            Chat.Print("Version 1.2 (20.7.2016!", Color.Red);
            Chat.Print("Gl HF And dont feed kappa", Color.Red);
            MissFortuneTheTrollMenu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 650);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Circular, 250, 2000, 50);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Circular, 500, int.MaxValue, 200);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Cone, 0, int.MaxValue);
            R.ConeAngleDegrees = 30;

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Obj_AI_Base.OnProcessSpellCast += OnSpellCast;
            Obj_AI_Base.OnBuffLose += OnBuffLose;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
         //   Obj_AI_Base.OnBuffGain += OnBuffGain1;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (MissFortuneTheTrollMenu.Nodraw()) return;

            {
                if (MissFortuneTheTrollMenu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (MissFortuneTheTrollMenu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (MissFortuneTheTrollMenu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (MissFortuneTheTrollMenu.DrawingsR())
                {
                    new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    MissFortuneTheTrollMenu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled =
                    MissFortuneTheTrollMenu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (MissFortuneTheTrollMenu.CheckSkin())
            {
                if (MissFortuneTheTrollMenu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(MissFortuneTheTrollMenu.SkinId());
                    CurrentSkin = MissFortuneTheTrollMenu.SkinId();
                }
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsEnemy) return;

            if (sender.IsValidTarget(E.Range) && MissFortuneTheTrollMenu.GapcloserE() && Player.Distance(e.End) < 150)
            {
                E.Cast(sender.Position);
            }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= MissFortuneTheTrollMenu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= MissFortuneTheTrollMenu.SpellsHealHp()
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
                Flee();
            }
            KillSteal();
            AutoCc();
            AutoPotions();
         //   AutoHarass();

        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "MissFortuneBulletSound")
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                Out = true;
                Channeling = true;
            }
        }

        //  MissFortuneBulletTime
        private static void OnBuffLose(Obj_AI_Base sender, Obj_AI_BaseBuffLoseEventArgs args)
        {
            if (sender.IsMe && args.Buff.DisplayName == "MissFortuneBulletSound")
            {
                Channeling = false;
                Orbwalker.DisableAttacking = false;
                Orbwalker.DisableMovement = false;
                Out = false;
            }
        }

        private static
            void AutoPotions()
        {
            if (MissFortuneTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= MissFortuneTheTrollMenu.SpellsPotionsHp() &&
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
            if (MissFortuneTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= MissFortuneTheTrollMenu.SpellsPotionsM() &&
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

            if (args.Buff.Type == BuffType.Taunt && MissFortuneTheTrollMenu.Taunt())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && MissFortuneTheTrollMenu.Stun())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && MissFortuneTheTrollMenu.Snare())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && MissFortuneTheTrollMenu.Polymorph())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && MissFortuneTheTrollMenu.Blind())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && MissFortuneTheTrollMenu.Fear())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && MissFortuneTheTrollMenu.Charm())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && MissFortuneTheTrollMenu.Suppression())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && MissFortuneTheTrollMenu.Silence())
            {
                DoQss();
            }
        }

        private static void DoQss()
        {
            if (Activator.Qss.IsOwned() && Activator.Qss.IsReady())
            {
                Core.DelayAction(() => Activator.Qss.Cast(),
                    MissFortuneTheTrollMenu.Activator["delay"].Cast<Slider>().CurrentValue);
            }

            if (Activator.Mercurial.IsOwned() && Activator.Mercurial.IsReady())
            {
                Core.DelayAction(() => Activator.Mercurial.Cast(),
                    MissFortuneTheTrollMenu.Activator["delay"].Cast<Slider>().CurrentValue);
            }
        }

      private static void Flee()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (MissFortuneTheTrollMenu.Fleew() && W.IsReady())
            {
                W.Cast();
            }
            if (MissFortuneTheTrollMenu.Fleee() && E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
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
                if (MissFortuneTheTrollMenu.KillstealQ() && Q.IsReady() &&
                    target.Health + target.AttackShield <
                    Player.GetSpellDamage(target, SpellSlot.Q))
                {
                    Q.Cast(target);
                }
            }
        }


        private static
            void AutoCc()
        {
            if (!MissFortuneTheTrollMenu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
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
                E.Cast(autoTarget.ServerPosition);
            }
        }
       
        public static void castQ(bool NotkillOnly, bool killMinionOnly)
        {
            castQ(NotkillOnly, killMinionOnly, false);
        }

        public static void castQ(bool NotkillOnly, bool killMinionOnly, bool onChampsOnly)
        {
            foreach (var killable in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(EloBuddy.Player.Instance, 1000) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && (e.Health < DamageLibrary.GetSpellDamage(EloBuddy.Player.Instance, e, SpellSlot.Q) || NotkillOnly)))
            {
                var killablePosition = Prediction.Position.PredictUnitPosition(killable, 250).To3D();
                int i = -1;
                for (int j = 0; j < EntityManager.Heroes.Enemies.Count; j++)
                {
                    if (killable.NetworkId == EntityManager.Heroes.Enemies[j].NetworkId)
                        i = j;
                }
                if (i == -1 || !MissFortuneTheTrollMenu.ComboQ())
                    continue;

                bool buff = false;
                foreach (var b in killable.Buffs)
                {
                    if (b.Name == "missfortunepassivestack")
                        buff = true;
                }
                foreach (var t in EntityManager.Heroes.Enemies.Where(e => e.IsInRange(EloBuddy.Player.Instance, Q.Range) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && Prediction.Position.PredictUnitPosition(e, 250).To3D().Distance(killable) < 500))
                {
                    Vector3 meToTarget = Prediction.Position.PredictUnitPosition(t, 250).To3D() - Prediction.Position.PredictUnitPosition(EloBuddy.Player.Instance, 250).To3D();
                    if (meToTarget.AngleBetween(killablePosition) < 0.6981 && buff && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        Q.Cast(t);
                        return;
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.349066 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.349066 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.6981 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.6981 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 1.9 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 1.9 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                }
                if (onChampsOnly)
                    return;
                foreach (var t in EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(EloBuddy.Player.Instance, Q.Range) && Prediction.Position.PredictUnitPosition(e, 250).To3D().Distance(killable) < 500 && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && (!killMinionOnly || e.Health < EloBuddy.Player.Instance.GetSpellDamage(e, SpellSlot.Q))).OrderBy(t => t.Health))
                {
                    Vector3 meToTarget = Prediction.Position.PredictUnitPosition(t, 250).To3D() - Prediction.Position.PredictUnitPosition(EloBuddy.Player.Instance, 250).To3D();
                    if (meToTarget.AngleBetween(killablePosition) < 0.6981 && buff && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        Q.Cast(t);
                        return;
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.349066 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.349066 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 0.6981 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 0.6981 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
                    else if (meToTarget.AngleBetween(killablePosition) < 1.9 && meToTarget.AngleBetween(killablePosition) > 0)
                    {
                        int m = EntityManager.MinionsAndMonsters.CombinedAttackable.Where(e => e.IsInRange(t, 500) && !e.IsDead && !e.IsInvulnerable && e.IsTargetable && !e.IsZombie && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(Prediction.Position.PredictUnitPosition(e, 250).To3D()) < 1.9 && Prediction.Position.PredictUnitPosition(t, 250).To3D().AngleBetween(killablePosition) > 0).Count();
                        if (m == 0)
                        {
                            Q.Cast(t);
                            return;
                        }
                    }
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
            if (Q.IsReady() && MissFortuneTheTrollMenu.LaneQ() &&
                Player.ManaPercent > MissFortuneTheTrollMenu.LaneMana())
            {
                Q.Cast(source);
            }
            if (W.IsReady() && MissFortuneTheTrollMenu.LaneW() &&
                Player.ManaPercent > MissFortuneTheTrollMenu.LaneMana())
            {
                W.Cast();
            }

            if (E.IsReady() && MissFortuneTheTrollMenu.LaneE() &&
                Player.ManaPercent > MissFortuneTheTrollMenu.LaneMana())
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

            if (MissFortuneTheTrollMenu.JungleE() && E.IsReady() && junleminions.IsValidTarget(E.Range))
            {
                E.Cast(junleminions);
            }
            if (MissFortuneTheTrollMenu.JungleQ() && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
            {
                Q.Cast(junleminions);
            }
            if (MissFortuneTheTrollMenu.JungleW() && W.IsReady() && E.IsOnCooldown && Q.IsOnCooldown &&
                junleminions.IsValidTarget(W.Range))
            {
                W.Cast();
            }
        }

        private static void AutoHarass()
        {
            var target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Q.IsReady() && target.IsValidTarget(Q1.Range) && MissFortuneTheTrollMenu.AutoQextendharass() &&
                Player.ManaPercent > MissFortuneTheTrollMenu.AutoHarassmana())
            {
                castQ(true, true);
            }
            else
            {
                castQ(true, false);
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
            if (Q.IsReady() && target.IsValidTarget(Q1.Range) && MissFortuneTheTrollMenu.UseQextendharass() &&
                Player.ManaPercent > MissFortuneTheTrollMenu.HarassQe())
            {
                castQ(true, true);
            }
            else
            {
               castQ(true, false);
            }
        if ( E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in enemies)
                {
                    var useE = MissFortuneTheTrollMenu.HarassMeNu["harass.E"
                                                                  + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE && Player.ManaPercent > MissFortuneTheTrollMenu.HarassQe())
                    {
                        E.Cast(target);
                    }
                }
        }
     

        private static
            void OnCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (!target.IsValidTarget(Q.Range) || target == null && Channeling)
            {
                return;
            }
            if (MissFortuneTheTrollMenu.ComboW() && W.IsReady() && target.IsValidTarget(W.Range) &&
                !target.IsInvulnerable &&
                target.Position.CountEnemiesInRange(800) >= MissFortuneTheTrollMenu.Combowenemies())
            {
                W.Cast();
            }
            if (E.IsReady() && target.IsValidTarget(E.Range) && MissFortuneTheTrollMenu.ComboE())
            {
                E.Cast(target);
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range) && MissFortuneTheTrollMenu.ComboQ())
            {
                Q.Cast(target);
            }
            if (Q.IsReady() && target.IsValidTarget(Q1.Range) && MissFortuneTheTrollMenu.ComboQextend())
            {
                castQ(true, true);
            }
            else
            {
                castQ(true, false);
            }
            if (MissFortuneTheTrollMenu.ComboR() &&
                Player.CountEnemiesInRange(1000) >= MissFortuneTheTrollMenu.ComboREnemies() &&
                R.IsReady() && target.IsValidTarget(1000))
            {
                var predR = R.GetPrediction(target);
                if (predR.HitChance >= HitChance.Medium)
                {
                    R.Cast(predR.CastPosition);
                }
            }
            var autoTarget =
              EntityManager.Heroes.Enemies.FirstOrDefault(
                  x =>
                      x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup) ||
                      x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression) ||
                      x.HasBuffOfType(BuffType.Slow) ||
                      x.HasBuffOfType(BuffType.Snare));
            if (MissFortuneTheTrollMenu.ComboRcc() && R.IsReady() && autoTarget != null)
            {
                R.Cast(autoTarget.ServerPosition);
            }
             if ((ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >=
                 MissFortuneTheTrollMenu.YoumusEnemies() ||
                 Player.HealthPercent >= MissFortuneTheTrollMenu.ItemsYoumuShp()) &&
                Activator.Youmuu.IsReady() && MissFortuneTheTrollMenu.Youmus() && Activator.Youmuu.IsOwned())
            {
                Activator.Youmuu.Cast();
                return;
            }
            if (Player.HealthPercent <= MissFortuneTheTrollMenu.BilgewaterHp() && MissFortuneTheTrollMenu.Bilgewater() &&
                Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
            {
                Activator.Bilgewater.Cast(target);
                return;
            }

            if (Player.HealthPercent <= MissFortuneTheTrollMenu.BotrkHp() && MissFortuneTheTrollMenu.Botrk() &&
                Activator.Botrk.IsReady() &&
                Activator.Botrk.IsOwned())
            {
                Activator.Botrk.Cast(target);
            }
        }
    }
}