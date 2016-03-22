using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace AsheTheTroll
{
    internal class AsheTheTroll
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static Spell.Active _q;
        private static Spell.Skillshot _w;
        private static Spell.Skillshot _e;
        private static Spell.Skillshot _r;
        private static Item _healthpot;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);

        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            PrediMenu,
            ItemMenu,
            SkinMenu;



        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Ashe)
            {
                return;
            }

            _healthpot = new Item(2003, 0);
            _q = new Spell.Active(SpellSlot.Q);
            _w = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Linear, 0, int.MaxValue, 60);
            _w.AllowedCollisionCount = 0;
            _e = new Spell.Skillshot(SpellSlot.E, 15000, SkillShotType.Linear, 0, int.MaxValue, 0);
            _r = new Spell.Skillshot(SpellSlot.R, 15000, SkillShotType.Linear, 500, 1000, 250);
            _r.AllowedCollisionCount = int.MaxValue;

            Chat.Print(
                "<font color=\"#4dd5ea\" >MeLoDag Presents </font><font color=\"#4dd5ea\" >AsheTheToLL </font><font color=\"#4dd5ea\" >Kappa Kippo</font>");


            Menu = MainMenu.AddMenu("AsheTheTroll", "AsheTheTroll");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "_ComboMenu");
            ComboMenu.Add("useQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("useWCombo", new CheckBox("Use W"));
            ComboMenu.Add("useRCombo", new CheckBox("Use R"));
            ComboMenu.AddSeparator();
            ComboMenu.AddSeparator();
            ComboMenu.Add("useRComboFinisher", new CheckBox("Use R[FinisherMode]"));
            ComboMenu.Add("ForceR",
               new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            ComboMenu.Add("useRComboRange", new Slider("R Max Range ", 1000, 500, 2000));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "_HarassMenu");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));

            HarassMenu.Add("useWHarass", new CheckBox("Use W"));
            HarassMenu.Add("useWHarassMana", new Slider("W Mana > %", 70, 0, 100));
            HarassMenu.AddSeparator();
            HarassMenu.AddLabel("Auto Harass");
            HarassMenu.Add("autoWHarass", new CheckBox("Auto W for Harass", false));
            HarassMenu.Add("autoWHarassMana", new Slider("W Mana > %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWFarm", new CheckBox("Use W"));
            JungleLaneMenu.AddSeparator();
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));
            JungleLaneMenu.Add("useWMana", new Slider("W Mana > %", 70, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto W for Gapcloser"));
            MiscMenu.Add("interrupter", new CheckBox("Auto R for Interrupter"));
            MiscMenu.Add("CCE", new CheckBox("Auto W on Enemy CC"));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            ItemMenu = Menu.AddSubMenu("Item Settings", "_ItemMenuettings");
            ItemMenu.Add("useHP", new CheckBox("Use Health Potion"));
            ItemMenu.Add("useHPV", new Slider("HP < %", 40, 0, 100));
            ItemMenu.AddSeparator();
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60, 1, 100));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60, 1, 100));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));

            PrediMenu = Menu.AddSubMenu("Prediction Settings", "_PrediMenuettings");
            var style = PrediMenu.Add("style", new Slider("Min Prediction", 1, 0, 2));
            style.OnValueChange += delegate
            {
                style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[style.CurrentValue];
            };
            style.DisplayName = "Min Prediction: " + new[] { "Low", "Medium", "High" }[style.CurrentValue];

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawRange", new CheckBox("Draw AA Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));

            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }



        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.DangerLevel == DangerLevel.High && sender.IsValidTarget(900))
            {
                _r.Cast(sender);
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            var hPpot = ItemMenu["useHP"].Cast<CheckBox>().CurrentValue;
            var hPv = ItemMenu["useHPv"].Cast<Slider>().CurrentValue;
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            Auto();
            Ks();
            AutoW();
            if (hPpot && _Player.HealthPercent < hPv)
            {
                if (Item.HasItem(_healthpot.Id) && Item.CanUseItem(_healthpot.Id) &&
                    !_Player.HasBuff("RegenerationPotion"))
                {
                    _healthpot.Cast();
                }
            }
        }

        private static void Combo()
        {

            var target = TargetSelector.GetTarget(_Player.AttackRange, DamageType.Physical);
            var useYoumu = ItemMenu["useYoumu"].Cast<CheckBox>().CurrentValue;
            var useMahvolmus = ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue;
            var useMahvolmusEV = ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue;
            var useMahvolmusHPV = ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue;

            Orbwalker.ForcedTarget = null;


            if (Orbwalker.IsAutoAttacking) return;

            if (useMahvolmus && Item.HasItem(3153) && Item.CanUseItem(3153) && Item.HasItem(3144) &&
                Item.CanUseItem(3144) && target.HealthPercent < useMahvolmusEV &&
                _Player.HealthPercent < useMahvolmusHPV)
                Item.UseItem(3153, target);
            Item.UseItem(3144, target);

            if (useYoumu && Item.HasItem(3142) && Item.CanUseItem(3142))
                Item.UseItem(3142);

            if ((ComboMenu["useWCombo"].Cast<CheckBox>().CurrentValue) && _w.IsReady())
            {
                if (target != null)
                {
                    var wpred = _w.GetPrediction(target);
                    var Predslider = PrediMenu["style"].Cast<Slider>().CurrentValue;
                    if (wpred.HitChancePercent >= Predslider)
                    {
                        _w.Cast(wpred.CastPosition);
                    }
                }
            }

            if (ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue && _q.IsReady())
            {
                if (Player.Instance.CountEnemiesInRange(700) > 0)
                {
                    foreach (var b in Player.Instance.Buffs)
                        if (b.Name == "asheqcastready")
                        {
                            _q.Cast();
                        }
                }
            }
            if (target != null)
            {
                if (ComboMenu["useRCombo"].Cast<CheckBox>().CurrentValue && _r.IsReady() &&
                    (Player.Instance.CountEnemiesInRange(600) == 0 || Player.Instance.HealthPercent < 25))
                {
                    var Rpred = _r.GetPrediction(target);
                    // var target = TargetSelector.GetTarget(1500, DamageType.Magical);
                    var Predslider = PrediMenu["style"].Cast<Slider>().CurrentValue;
                    if (Rpred.HitChancePercent >= Predslider)
                    {
                        _r.Cast(Rpred.CastPosition);
                    }
                }
            }
        }

        public static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (target != null && (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && _r.IsReady() && target.IsValid && !Player.HasBuff("AsheR"))) _r.Cast(target.Position);
        }


        private static
            void OnGameUpdate(EventArgs args)
        {
            if (CheckSkin())
            {
                Player.SetSkinId(SkinId());
            }
        }

        public static int SkinId()
        {
            return SkinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return SkinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        public static void Auto()
        {
            var useQss = ItemMenu["useQSS"].Cast<CheckBox>().CurrentValue;
            var eonCc = MiscMenu["CCE"].Cast<CheckBox>().CurrentValue;
            if (eonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < _w.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        _w.Cast(enemy);
                    }
                }
            }
            if (_Player.HasBuffOfType(BuffType.Fear) || _Player.HasBuffOfType(BuffType.Stun) ||
                _Player.HasBuffOfType(BuffType.Taunt) || _Player.HasBuffOfType(BuffType.Polymorph))
            {

                if (useQss && Item.HasItem(3140) && Item.CanUseItem(3140))
                    Item.UseItem(3140);

                if (useQss && Item.HasItem(3139) && Item.CanUseItem(3139))
                    Item.UseItem(3139);
            }
        }

        public static void AutoW()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            if (HarassMenu["autoWHarass"].Cast<CheckBox>().CurrentValue &&
              _w.IsReady() && targetW.IsValidTarget(_w.Range) &&
              _Player.ManaPercent > HarassMenu["autoWHarassMana"].Cast<Slider>().CurrentValue)
            {
                _w.Cast(targetW);
            }
        }

        public static void Flee()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            var wPred = _w.GetPrediction(targetW);

            if (wPred.HitChance >= HitChance.Medium && _w.IsReady() && targetW.IsValidTarget(_w.Range))
            {
                _w.Cast(targetW);
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useW = JungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;

            if (Orbwalker.IsAutoAttacking) return;
            if (useQ)
            {
                foreach (var b in Player.Instance.Buffs)
                    if (b.Name == "asheqcastready")
                    {
                        _q.Cast();
                    }

                if (useW)
                {
                    var minions = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, _w.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                    if (minions.Count() > 0)
                    {
                        _w.Cast(minions.First());
                    }
                }
            }
        }

        public static void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useW = JungleLaneMenu["useWFarm"].Cast<CheckBox>().CurrentValue;

            if (Orbwalker.IsAutoAttacking) return;
            if (useQ)
            {
                foreach (var b in Player.Instance.Buffs)
                    if (b.Name == "asheqcastready")
                    {
                        _q.Cast();
                    }
            }
            if (useW)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t =>
                            t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                            t.IsInRange(Player.Instance.Position, _w.Range));
                foreach (var m in minions)
                {
                    if (
                        _w.GetPrediction(m)
                            .CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable)
                            .Count() >= 0)
                    {
                        _w.Cast(m);
                        break;
                    }
                }
            }
        }


        public static
            void Harass()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            var target = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;

            Orbwalker.ForcedTarget = null;

            if (Orbwalker.IsAutoAttacking) return;

            if (targetW != null)
            {

                if (HarassMenu["useWHarass"].Cast<CheckBox>().CurrentValue && _w.IsReady() &&
                    target.Distance(_Player) > _Player.AttackRange &&
                    targetW.IsValidTarget(_w.Range) && _Player.ManaPercent > wmana)
                {
                    _w.Cast(targetW);
                }
            }

            if (target != null)
            {
                if (HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue)
                {
                    if (ComboMenu["useQCombo"].Cast<CheckBox>().CurrentValue)
                    {
                        if (target.Distance(_Player) <= _Player.AttackRange)
                        {
                            _q.Cast();
                        }
                    }
                }
            }
        }


        public static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) < 200)
            {
                _w.Cast(e.End);
            }
        }

        public static void Ks()
        {

            var distance = ComboMenu["useRComboRange"].Cast<Slider>().CurrentValue;
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (ComboMenu["useRComboFinisher"].Cast<CheckBox>().CurrentValue && _r.IsReady() &&
                    enemy.Distance(_Player) <= distance &&
                    RDamage(enemy) >= enemy.Health && !enemy.IsZombie && !enemy.IsDead)
                {
                    _r.Cast(enemy);
                }
            }
        }

        public static int WDamage(Obj_AI_Base target)
        {
            return
                (int)
                    (new[] { 10, 60, 110, 160, 210 }[_w.Level - 1] +
                     1.4 * (_Player.TotalAttackDamage));
        }

        public static float RDamage(Obj_AI_Base target)
        {

            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 250, 425, 600 }[_r.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);

        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawRange"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(_q.IsReady() ? Color.Aqua : Color.Aqua, _q.Range, _Player.Position);
            }

            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(_w.IsReady() ? Color.Aqua : Color.Aqua, _w.Range, _Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(_r.IsReady() ? Color.Aqua : Color.Aqua, _r.Range, _Player.Position);
            }
        }
    }

}