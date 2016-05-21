using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Taric_The_Troll.Utility;
using Activator = Taric_The_Troll.Utility.Activator;

namespace Taric_The_Troll
{
    public static class Program
    {
        public static string Version = "Version 1 21/5/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Active Q;
        public static Spell.Targeted W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
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
            if (Player.ChampionName != "Taric") return;
            Chat.Print(
                "<font color=\"#ac4cdc\" >MeLoDag Presents </font><font color=\"#fffffff\" >Taric </font><font color=\"#ac4cdc\" >Kappa Kippo</font>");
            TarickTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Active(SpellSlot.Q, 350);
            W = new Spell.Targeted(SpellSlot.W, 800);
            E = new Spell.Skillshot(SpellSlot.E, 580, SkillShotType.Linear, 250, int.MaxValue, 140);
            R = new Spell.Active(SpellSlot.R, 400);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Drawing.OnDraw += GameOnDraw;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (TarickTheTrollMeNu.Nodraw()) return;

            {
                if (TarickTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.DeepPink, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TarickTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.DeepPink, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TarickTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.DeepPink, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (TarickTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.DeepPink, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
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
            if (TarickTheTrollMeNu.CheckSkin())
            {
                if (TarickTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(TarickTheTrollMeNu.SkinId());
                    CurrentSkin = TarickTheTrollMeNu.SkinId();
                }
            }
        }


        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!e.Sender.IsValidTarget() || !TarickTheTrollMeNu.GapcloserE() || e.Sender.Type != Player.Type ||
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

            if (interruptableSpellEventArgs.DangerLevel >= DangerLevel.High && !TarickTheTrollMeNu.InterruptE() &&
                E.IsReady())
            {
                E.Cast(sender.Position);
            }
        }

        private static void Barrier()
        {
            if (Player.IsFacing(Target) && Activator.Barrier.IsReady() &&
                Player.HealthPercent <= TarickTheTrollMeNu.SpellsBarrierHp())
                Activator.Barrier.Cast();
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= TarickTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= TarickTheTrollMeNu.SpellsHealHp()
                && Player.CountEnemiesInRange(600) > 0 && Activator.Heal.IsReady())
            {
                Activator.Heal.Cast();
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) OnCombo();
            StunE();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) OnHarrass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) ForceStun();
            AutoCc();
            AutoPotions();
            AutoShieldHp();
        }

        private static void ForceStun()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target != null &&
                (TarickTheTrollMeNu.ForceStun() && E.IsReady() &&
                 target.IsValidTarget(E.Range)))
             {
                E.Cast(target.Position);
            }
        }

        private static
            void AutoShieldHp()
        {
            var allies =
                EntityManager.Heroes.Allies.OrderBy(a => a.Health).FirstOrDefault(a => W.IsInRange(a) && !a.IsMe);
            if (allies != null)

                if (Q.IsReady() && TarickTheTrollMeNu.AutoQ() && allies.IsValidTarget(Q.Range) &&
                    allies.HealthPercent <= TarickTheTrollMeNu.HpallyQ() &&
                    Player.ManaPercent >= TarickTheTrollMeNu.Automanaheal())
                {
                    Q.Cast();
                }
            if (W.IsReady())
            {
                var usew = allies != null && allies.IsValidTarget(W.Range) &&
                           TarickTheTrollMeNu.AUtoMenu["Autoshield.Champion"
                                                       + allies.ChampionName].Cast<CheckBox>().CurrentValue;
                if (usew && allies.HealthPercent <= TarickTheTrollMeNu.Hpally() &&
                    Player.ManaPercent >= TarickTheTrollMeNu.AUtomanaShield())
                {
                    W.Cast(allies);
                }
                if (allies != null && (R.IsReady() && TarickTheTrollMeNu.AutoR() && allies.IsValidTarget(R.Range) &&
                                       allies.HealthPercent <= TarickTheTrollMeNu.AutoRhp()))
                {
                    R.Cast();
                }
            }
        }


        private static
            void AutoPotions()
        {
            if (TarickTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= TarickTheTrollMeNu.SpellsPotionsHp() &&
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
            if (TarickTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= TarickTheTrollMeNu.SpellsPotionsM() &&
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

            if (args.Buff.Type == BuffType.Taunt && TarickTheTrollMeNu.Taunt())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && TarickTheTrollMeNu.Stun())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && TarickTheTrollMeNu.Snare())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && TarickTheTrollMeNu.Polymorph())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && TarickTheTrollMeNu.Blind())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && TarickTheTrollMeNu.Fear())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && TarickTheTrollMeNu.Charm())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && TarickTheTrollMeNu.Suppression())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && TarickTheTrollMeNu.Silence())
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


        private static
            void AutoCc()
        {
            if (!TarickTheTrollMeNu.ComboMenu["combo.CC"].Cast<CheckBox>().CurrentValue)
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
        }

        private static void OnHarrass()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= E.Range);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
                foreach (var eenemies in enemies)
                {
                    var useE = TarickTheTrollMeNu.HarassMeNu["harass.E"
                                                             + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE && Player.ManaPercent >= TarickTheTrollMeNu.Harassmana())
                    {
                        var predeharass = E.GetPrediction(target);
                        if (predeharass.HitChance >= HitChance.High)
                        {
                            E.Cast(predeharass.CastPosition);
                        }
                    }
                }
        }


        private static
            void OnCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady() && TarickTheTrollMeNu.ComboQ() &&
                Player.HealthPercent <= TarickTheTrollMeNu.MyhpQ())
            {
                Q.Cast();
            }
            if (W.IsReady() && TarickTheTrollMeNu.ComboW() &&
                Player.HealthPercent <= TarickTheTrollMeNu.ShieldMyHp())
            {
                W.Cast(Player);
            }
            if (R.IsReady() && TarickTheTrollMeNu.Combor() &&
                Player.HealthPercent <= TarickTheTrollMeNu.Comborhp())
            {
                R.Cast();
            }
            if ((ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >=
                 TarickTheTrollMeNu.YoumusEnemies() ||
                 Player.HealthPercent >= TarickTheTrollMeNu.ItemsYoumuShp()) &&
                Activator.Youmus.IsReady() && TarickTheTrollMeNu.Youmus() &&
                Activator.Youmus.IsOwned())
            {
                Activator.Youmus.Cast();
                return;
            }
            if (Player.HealthPercent <= TarickTheTrollMeNu.BilgewaterHp() &&
                TarickTheTrollMeNu.Bilgewater() &&
                Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
            {
                Activator.Bilgewater.Cast(target);
                return;
            }

            if (Player.HealthPercent <= TarickTheTrollMeNu.BotrkHp() && TarickTheTrollMeNu.Botrk() &&
                Activator.Botrk.IsReady() &&
                Activator.Botrk.IsOwned())
            {
                Activator.Botrk.Cast(target);
            }
        }

        private static
            void StunE()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(Player) <= E.Range);
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (E.IsReady() && target.IsValidTarget(650))
                foreach (var eenemies in enemies)
                {
                    var useE = TarickTheTrollMeNu.ComboMenu["combo.E"
                                                            + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE && Player.ManaPercent >= TarickTheTrollMeNu.Harassmana())
                    {
                        var predE = E.GetPrediction(target);
                        if (predE.HitChance >= HitChance.Medium)
                        {
                            E.Cast(predE.CastPosition);
                        }
                        else if (predE.HitChance >= HitChance.Dashing)
                        {
                            E.Cast(predE.CastPosition);
                        }
                        else if (predE.HitChance >= HitChance.Immobile)
                        {
                            E.Cast(predE.CastPosition);
                        }
                    }
                }
        }
    }
}