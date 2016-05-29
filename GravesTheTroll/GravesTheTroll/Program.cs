using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using GravesTheTroll.Utility;
using SharpDX;
using Activator = GravesTheTroll.Utility.Activator;
using Color = System.Drawing.Color;

namespace GravesTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1 22/5/2016";
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
            if (Player.ChampionName != "Graves") return;
            Chat.Print(
                "<font color=\"#990b0b\" >MeLoDag Presents </font><font color=\"#fffffff\" >Graves </font><font color=\"#990b0b\" >Kappa Kippo</font>");
            GravesTheTrollMeNu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 2000, 60);
            W = new Spell.Skillshot(SpellSlot.W, 850, SkillShotType.Circular, 250, 1650, 150);
            E = new Spell.Skillshot(SpellSlot.E, 425, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1000, SkillShotType.Linear, 250, 2100, 100);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Drawing.OnDraw += GameOnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (GravesTheTrollMeNu.Nodraw()) return;

            {
                if (GravesTheTrollMeNu.DrawingsQ())
                {
                    new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (GravesTheTrollMeNu.DrawingsW())
                {
                    new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (GravesTheTrollMeNu.DrawingsE())
                {
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (GravesTheTrollMeNu.DrawingsR())
                {
                    new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                GravesTheTrollMeNu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = GravesTheTrollMeNu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
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
            if (GravesTheTrollMeNu.CheckSkin())
            {
                if (GravesTheTrollMeNu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(GravesTheTrollMeNu.SkinId());
                    CurrentSkin = GravesTheTrollMeNu.SkinId();
                }
            }
        }


        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (GravesTheTrollMeNu.GapcloserW() && sender.IsEnemy &&
                e.End.Distance(Player) < 200)
            {
                W.Cast(e.End);
              
            }


            if (GravesTheTrollMeNu.GapcloserE() && sender.IsEnemy &&
                e.End.Distance(Player) < 200)
            {
                E.Cast(e.End);
               
            }
        }

        private static void Barrier()
        {
            if (Player.IsFacing(Target) && Activator.Barrier.IsReady() &&
                Player.HealthPercent <= GravesTheTrollMeNu.SpellsBarrierHp())
                Activator.Barrier.Cast();
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= GravesTheTrollMeNu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= GravesTheTrollMeNu.SpellsHealHp()
                && Player.CountEnemiesInRange(600) > 0 && Activator.Heal.IsReady())
            {
                Activator.Heal.Cast();
            }
        }

        private static void GameOnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) OnCombo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) OnHarrass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear)) OnLaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)) OnJungle();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) OnFlee();
            KillSteal();
            AutoCc();
            AutoPotions();
        }

        private static
            void AutoPotions()
        {
            if (GravesTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= GravesTheTrollMeNu.SpellsPotionsHp() &&
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
            if (GravesTheTrollMeNu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= GravesTheTrollMeNu.SpellsPotionsM() &&
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

            if (args.Buff.Type == BuffType.Taunt && GravesTheTrollMeNu.Taunt())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && GravesTheTrollMeNu.Stun())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && GravesTheTrollMeNu.Snare())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && GravesTheTrollMeNu.Polymorph())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && GravesTheTrollMeNu.Blind())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && GravesTheTrollMeNu.Fear())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && GravesTheTrollMeNu.Charm())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && GravesTheTrollMeNu.Suppression())
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && GravesTheTrollMeNu.Silence())
            {
                DoQss();
            }
        }

        private static void DoQss()
        {
            if (Activator.Qss.IsOwned() && Activator.Qss.IsReady())
            {
                Core.DelayAction(() => Activator.Qss.Cast(),
                    GravesTheTrollMeNu.Activator["QssDelay"].Cast<Slider>().CurrentValue);
            }

            if (Activator.Mercurial.IsOwned() && Activator.Mercurial.IsReady())
            {
                Core.DelayAction(() => Activator.Mercurial.Cast(),
                    GravesTheTrollMeNu.Activator["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid &&
                E.IsReady() &&
                GravesTheTrollMeNu.ComboMenu["useEcombo"].Cast<ComboBox>().CurrentValue == 0)
            {
                E.Cast(Side(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                Orbwalker.ResetAutoAttack();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && E.IsReady() && target.IsValid &&
                GravesTheTrollMeNu.ComboMenu["useEcombo"].Cast<ComboBox>().CurrentValue == 1)
            {
                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
                Orbwalker.ResetAutoAttack();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && E.IsReady() && target.IsValid &&
                GravesTheTrollMeNu.ComboMenu["useEcombo"].Cast<ComboBox>().CurrentValue == 2)
                if (Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                {
                    EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && E.IsReady() && target.IsValid && GravesTheTrollMeNu.JungleE())
            {
                EloBuddy.Player.CastSpell(SpellSlot.E, Game.CursorPos);
                Orbwalker.ResetAutoAttack();
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

                if (GravesTheTrollMeNu.KillstealQ() && Q.IsReady() &&
                    target.Health + target.AttackShield < SpellDamage.RDamage(target))
                {
                    Q.Cast(target.Position);
                }
                if (GravesTheTrollMeNu.KillstealR() && R.IsReady() &&
                    target.Health + target.AttackShield < Player.GetSpellDamage(target, SpellSlot.R))
                {
                    var predRKs = R.GetPrediction(target);
                    if (predRKs.HitChance >= HitChance.High)
                    {
                        R.Cast(target.Position);
                    }
                }
            }
        }

        private static
            void AutoCc()
        {
            var autoTarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.HasBuffOfType(BuffType.Charm) || x.HasBuffOfType(BuffType.Knockup) ||
                        x.HasBuffOfType(BuffType.Stun) || x.HasBuffOfType(BuffType.Suppression) ||
                        x.HasBuffOfType(BuffType.Snare));
            if (autoTarget != null)
                if (!GravesTheTrollMeNu.ComboMenu["combo.CCQ"].Cast<CheckBox>().CurrentValue)
                {
                    return;
                }
            if (autoTarget != null)
            {
                E.Cast(autoTarget.ServerPosition);
            }
            if (!GravesTheTrollMeNu.ComboMenu["combo.CCW"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }
            if (autoTarget != null)
            {
                W.Cast(autoTarget.ServerPosition);
            }
        }

        private static void OnFlee()
        {
            if (E.IsReady() && GravesTheTrollMeNu.UseEmouse())
            {
                E.Cast(Game.CursorPos);
            }
        }


        private static void OnLaneClear()
        {
            if (Q.IsReady() && GravesTheTrollMeNu.LaneQ() && Player.Mana > GravesTheTrollMeNu.LaneMana())
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(Player) <= Q.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 600)
                            .Count();
                    if (enemyMinionsInRange > GravesTheTrollMeNu.LaneQcount())
                    {
                        Q.Cast(enemyMinion.Position);
                    }
                }
            }
        }


        private static
            void OnJungle()
        {
            if (GravesTheTrollMeNu.JungleQ() && Player.ManaPercent > GravesTheTrollMeNu.JunglMana())
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && minion != null)
                {
                    Q.Cast(minion.Position);
                }

                if (W.IsReady() && GravesTheTrollMeNu.JungleW() && Player.ManaPercent > GravesTheTrollMeNu.JunglMana() &&
                    minion != null)
                {
                    W.Cast(minion.Position);
                }
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

            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = GravesTheTrollMeNu.HarassMeNu["harass.Q"
                                                             + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ && Player.ManaPercent >= GravesTheTrollMeNu.HarassQe())
                    {
                        var predQharass = Q.GetPrediction(target);
                        if (predQharass.HitChance >= HitChance.High)
                        {
                            Q.Cast(predQharass.CastPosition);
                        }
                        else if (predQharass.HitChance >= HitChance.Medium)
                        {
                            Q.Cast(predQharass.CastPosition);
                        }
                        else if (predQharass.HitChance >= HitChance.Immobile)
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
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                foreach (var eenemies in enemies)
                {
                    var useQ = GravesTheTrollMeNu.ComboMenu["combo.q"
                                                            + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useQ)
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
                    if (GravesTheTrollMeNu.ComboW() && W.IsReady() && target.IsValidTarget(W.Range))
                    {
                        var predW = W.GetPrediction(target);
                        if (predW.HitChance >= HitChance.Medium)
                        {
                            W.Cast(predW.CastPosition);
                        }
                    }
                    if (R.IsReady() && GravesTheTrollMeNu.ComboR() &&
                        Player.CountEnemiesInRange(R.Range) >= GravesTheTrollMeNu.ComboREnemies())
                    {
                        R.Cast(target);
                    }
                    if ((ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >=
                         GravesTheTrollMeNu.YoumusEnemies() ||
                         Player.HealthPercent >= GravesTheTrollMeNu.ItemsYoumuShp()) &&
                        Activator.Youmus.IsReady() && GravesTheTrollMeNu.Youmus() && Activator.Youmus.IsOwned())
                    {
                        Activator.Youmus.Cast();
                        return;
                    }
                    if (Player.HealthPercent <= GravesTheTrollMeNu.BilgewaterHp() &&
                        GravesTheTrollMeNu.Bilgewater() &&
                        Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
                    {
                        Activator.Bilgewater.Cast(target);
                        return;
                    }

                    if (Player.HealthPercent <= GravesTheTrollMeNu.BotrkHp() && GravesTheTrollMeNu.Botrk() &&
                        Activator.Botrk.IsReady() &&
                        Activator.Botrk.IsOwned())
                    {
                        Activator.Botrk.Cast(target);
                    }
                }
        }

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
    }
}