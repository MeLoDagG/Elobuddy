using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using IreliaTheTroll.Utility;
using Activator = IreliaTheTroll.Utility.Activator;

namespace IreliaTheTroll
{
    public static class Program
    {
        public static string Version = "Version 1 28/6/2016";
        public static AIHeroClient Target = null;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;
        public static Spell.Targeted Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Skillshot R;
        public static int CurrentSkin;
        public static int Rcount;
        public static readonly AIHeroClient Player = ObjectManager.Player;


        internal static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
            Bootstrap.Init(null);
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Hero != Champion.Irelia)
            {
                return;
            }
            Chat.Print("Irelia The Troll Loaded!!", Color.White);
            Chat.Print("Version 1 (28/6/2016)", Color.White);
            Chat.Print("Hafe Fun And Dont Feed!", Color.White);
            IreliaTheTrollMenu.LoadMenu();
            Game.OnTick += GameOnTick;
            Activator.LoadSpells();
            Game.OnUpdate += OnGameUpdate;

            #region Skill

            Q = new Spell.Targeted(SpellSlot.Q, 650);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, 350);
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 0, 1600, 65);

            #endregion

            Gapcloser.OnGapcloser += AntiGapCloser;
            Interrupter.OnInterruptableSpell += Interupt;
            Drawing.OnDraw += GameOnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            DamageIndicator.Initialize(SpellDamage.GetTotalDamage);
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (IreliaTheTrollMenu.Nodraw()) return;

            {
                if (IreliaTheTrollMenu.DrawingsQ())
                {
                    new Circle {Color = Color.White, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (IreliaTheTrollMenu.DrawingsW())
                {
                    new Circle {Color = Color.White, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (IreliaTheTrollMenu.DrawingsE())
                {
                    new Circle {Color = Color.White, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                if (IreliaTheTrollMenu.DrawingsR())
                {
                    new Circle {Color = Color.White, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    IreliaTheTrollMenu.DrawMeNu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = IreliaTheTrollMenu.DrawMeNu["percent"].Cast<CheckBox>().CurrentValue;
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (Activator.Heal != null)
                Heal();
            if (Activator.Ignite != null)
                Ignite();
            if (IreliaTheTrollMenu.CheckSkin())
            {
                if (IreliaTheTrollMenu.SkinId() != CurrentSkin)
                {
                    Player.SetSkinId(IreliaTheTrollMenu.SkinId());
                    CurrentSkin = IreliaTheTrollMenu.SkinId();
                }
            }
        }

        private static void AntiGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsValidTarget(E.Range) && e.End.Distance(Player.Position) <= E.Range
                && Player.HealthPercent <= sender.HealthPercent)
            {
                if (E.IsReady() && IreliaTheTrollMenu.GapcloserE())
                {
                    E.Cast(sender);
                }
            }
        }

        public static void Interupt(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (sender.IsValidTarget(E.Range) && Player.HealthPercent <= sender.HealthPercent)
                if (E.IsReady() && IreliaTheTrollMenu.InterupteE())
                {
                    E.Cast(sender);
                }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Activator.Ignite.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Activator.Ignite.Slot) ||
                autoIgnite != null && autoIgnite.HealthPercent <= IreliaTheTrollMenu.SpellsIgniteFocus())
                Activator.Ignite.Cast(autoIgnite);
        }

        private static void Heal()
        {
            if (Activator.Heal != null && Activator.Heal.IsReady() &&
                Player.HealthPercent <= IreliaTheTrollMenu.SpellsHealHp()
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
            UseRTarget();
            RCount();
        }

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (W.IsReady() && IreliaTheTrollMenu.ComboW())
                {
                    W.Cast();
                }
        }

        private static bool UnderTheirTower(Obj_AI_Base target)
        {
            var tower =
                ObjectManager
                    .Get<Obj_AI_Turret>()
                    .FirstOrDefault(
                        turret =>
                            turret != null && turret.Distance(target) <= 775 && turret.IsValid && turret.Health > 0 &&
                            !turret.IsAlly);

            return tower != null;
        }

        private static
            void AutoPotions()
        {
            if (IreliaTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.HealthPercent <= IreliaTheTrollMenu.SpellsPotionsHp() &&
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
            if (IreliaTheTrollMenu.SpellsPotionsCheck() && !Player.IsInShopRange() &&
                Player.ManaPercent <= IreliaTheTrollMenu.SpellsPotionsM() &&
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
            if (target != null && IreliaTheTrollMenu.ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && R.IsReady() &&
                target.IsValid && !Player.HasBuff("IreliaR")) R.Cast(target.Position);
        }

        private static void RCount()
        {
            if (Rcount == 0 && R.IsReady())
                Rcount = 4;

            if (!R.IsReady() & Rcount != 0)
                Rcount = 0;

            foreach (
                var buff in
                    Player.Buffs.Where(b => b.Name == "ireliatranscendentbladesspell" && b.IsValid))
            {
                Rcount = buff.Count;
            }
        }

        private static void KillSteal()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (IreliaTheTrollMenu.KillstealQ() && Q.IsReady() && SpellDamage.QDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player) <= Q.Range)
                {
                    Q.Cast(enemy);
                }
                if (IreliaTheTrollMenu.KillstealE() && E.IsReady() && SpellDamage.EDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player) <= E.Range)
                {
                    E.Cast(enemy);
                }
                if (IreliaTheTrollMenu.killstealR() && R.IsReady() && SpellDamage.RDamage(enemy) >= enemy.Health &&
                    enemy.Distance(Player) <= R.Range)
                {
                    R.Cast(enemy);
                }
            }
        }

        private static void OnLaneClear()
        {
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(Player) <= Q.Range &&
                            m.Health <= SpellDamage.QDamage(m) + SpellDamage.ExtraWDamage() - 10 &&
                            m.IsValidTarget());
            if (Q.IsReady() && IreliaTheTrollMenu.LaneQ() && Player.ManaPercent > IreliaTheTrollMenu.LaneMana() &&
                qminion != null)
            {
                Q.Cast(qminion);
            }
        }

        private static
            void OnJungle()
        {
            var junleminions =
                EntityManager.MinionsAndMonsters.GetJungleMonsters()
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(900));

            if (IreliaTheTrollMenu.JungleE() && E.IsReady() && junleminions.IsValidTarget(E.Range))
            {
                E.Cast(junleminions);
            }
            if (IreliaTheTrollMenu.JungleQ() && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
            {
                Q.Cast(junleminions);
            }
            if (IreliaTheTrollMenu.JungleW() && W.IsReady() && E.IsOnCooldown && Q.IsOnCooldown &&
                junleminions.IsValidTarget(W.Range))
            {
                W.Cast();
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
                    var useE = IreliaTheTrollMenu.HarassMeNu["harass.E"
                                                             + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                    if (useE && Player.ManaPercent > IreliaTheTrollMenu.HarassQe())
                    {
                        E.Cast(target);

                        if (Q.IsReady() && target.IsValidTarget(Q.Range))
                        {
                            var useQ = IreliaTheTrollMenu.HarassMeNu["harass.Q"
                                                                     + eenemies.ChampionName].Cast<CheckBox>()
                                .CurrentValue;
                            if (useQ && Player.ManaPercent > IreliaTheTrollMenu.HarassQe())
                            {
                                Q.Cast(target);
                            }
                        }
                    }
                }
        }

        private static
            void OnCombo()
        {
            var gctarget = TargetSelector.GetTarget(Q.Range*2.5f, DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (gctarget == null) return;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Position,
                    Q.Range + 350)
                    .Where(
                        m =>
                            m.IsValidTarget()
                            && Prediction.Health.GetPrediction(m, 1000*(int) (m.Distance(Player)/2200))
                            <= SpellDamage.QDamage(m) + SpellDamage.ExtraWDamage())
                    .OrderBy(m => m.Distance(gctarget))
                    .FirstOrDefault();

            if (Q.IsReady())
            {
                if (IreliaTheTrollMenu.ComboQgapclose() && qminion != null &&
                    gctarget.Distance(Player) >= Player.GetAutoAttackRange(gctarget) &&
                    qminion.Distance(gctarget) <= Player.Distance(gctarget) &&
                    qminion.Distance(Player) <= Q.Range)
                {
                    Q.Cast(qminion);
                }
            }
            if (IreliaTheTrollMenu.ComboQ() && !IreliaTheTrollMenu.ComboQlastsec() && target != null &&
                Player.Distance(target) <= IreliaTheTrollMenu.Qminrange())
            {
                if (UnderTheirTower(target))
                    if (target.HealthPercent >=
                        IreliaTheTrollMenu.Qundertower()) return;
                {
                    Q.Cast(target);
                }
            }
            if (IreliaTheTrollMenu.ComboQlastsec() && !IreliaTheTrollMenu.ComboQ() && target != null)
            {
                var buff = Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                if (buff != null &&
                    buff.EndTime - Game.Time <= Player.Distance(target)/2200 + .500 + Player.AttackCastDelay)

                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            IreliaTheTrollMenu.Qundertower()) return;

                    Q.Cast(target);
                }
            }
            if (E.IsReady() && IreliaTheTrollMenu.ComboEstun() && !IreliaTheTrollMenu.ComboE() && target != null &&
                target.Distance(Player) <= E.Range && target.HealthPercent >= Player.HealthPercent &&
                !Player.IsDashing())
            {
                E.Cast(target);
            }
            if (E.IsReady() && IreliaTheTrollMenu.ComboE() && !IreliaTheTrollMenu.ComboEstun() && target != null)
            {
                E.Cast(target);
            }
            if (R.IsReady() && IreliaTheTrollMenu.ComboR() && target != null)
            {
                R.Cast(target);
            }
            if ((ObjectManager.Player.CountEnemiesInRange(ObjectManager.Player.AttackRange) >=
                 IreliaTheTrollMenu.YoumusEnemies() ||
                 Player.HealthPercent >= IreliaTheTrollMenu.ItemsYoumuShp()) &&
                Activator.Youmus.IsReady() && IreliaTheTrollMenu.Youmus() && Activator.Youmus.IsOwned())
            {
                Activator.Youmus.Cast();
                return;
            }
            if (Player.HealthPercent <= IreliaTheTrollMenu.BilgewaterHp() &&
                IreliaTheTrollMenu.Bilgewater() &&
                Activator.Bilgewater.IsReady() && Activator.Bilgewater.IsOwned())
            {
                Activator.Bilgewater.Cast(target);
                return;
            }

            if (Player.HealthPercent <= IreliaTheTrollMenu.BotrkHp() && IreliaTheTrollMenu.Botrk() &&
                Activator.Botrk.IsReady() &&
                Activator.Botrk.IsOwned())
            {
                Activator.Botrk.Cast(target);
            }
        }
    }
}