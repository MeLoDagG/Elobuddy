using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

namespace EliseTheTroll
{
    internal class Program
    {
        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item ZhonyaHourglass;
        public static string[] JungleMobsNames;

        public static string[] SrJungleMobsNames =
        {
            "SRU_Dragon_Air", "SRU_Dragon_Earth", "SRU_Dragon_Fire",
            "SRU_Dragon_Water", "SRU_Dragon_Elder", "SRU_Baron", "SRU_Gromp", "SRU_Krug", "SRU_Razorbeak", "Sru_Crab",
            "SRU_Murkwolf", "SRU_Blue", "SRU_Red", "SRU_RiftHerald"
        };


        private static Menu _menu,
            _comboMenu,
            _laneMenu,
            _jungleMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _activatorMenu,
            _harassMenu;


        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static bool Humanoid
        {
            get
            {
                return _Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseHumanQ"
                       || _Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseHumanW"
                       || _Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseHumanE";
            }
        }

        private static bool Spiderman
        {
            get
            {
                return _Player.Spellbook.GetSpell(SpellSlot.Q).Name == "EliseSpiderQCast"
                       || _Player.Spellbook.GetSpell(SpellSlot.W).Name == "EliseSpiderW"
                       || _Player.Spellbook.GetSpell(SpellSlot.E).Name == "EliseSpiderEInitial";
            }
        }

        public static float SmiteDmgMonster(Obj_AI_Base target)
        {
            return _Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Smite);
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Elise)
            {
                return;
            }
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);
            ZhonyaHourglass = new Item(ItemId.Zhonyas_Hourglass);


            _menu = MainMenu.AddMenu("EliseTheTroll", "EliseTheTroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.AddGroupLabel("Human Spells");
            _comboMenu.Add("useQCombo", new CheckBox("Use Q", true));
            _comboMenu.Add("useWCombo", new CheckBox("Use W", true));
            _comboMenu.Add("useECombo", new CheckBox("Use E", true));
            _comboMenu.AddLabel("Spider Spells");
            _comboMenu.Add("useQspider", new CheckBox("Use Q ", true));
            _comboMenu.Add("useWspider", new CheckBox("Use W ", true));
            _comboMenu.Add("useEspider", new CheckBox("Use E ", true));
            _comboMenu.AddLabel("Prediction E HItchange");
            _comboMenu.Add("predE", new Slider("Use E Pred %", 70, 0, 100));
            _comboMenu.Add("predW", new Slider("Use W Pred %", 70, 0, 100));
            _comboMenu.AddGroupLabel("Smite Settings");
            _comboMenu.Add("smitecombo", new CheckBox("Use Smite Gank/Combo"));
            _comboMenu.AddGroupLabel("KillSteal Settings");
            _comboMenu.Add("killsteal.Q", new CheckBox("Use Q", false));


            _harassMenu = _menu.AddSubMenu("Harass", "Harass");
            _harassMenu.AddGroupLabel("Human Spells");
            _harassMenu.Add("useQharass", new CheckBox("Use Q", false));
            _harassMenu.Add("useWharass", new CheckBox("Use W", false));
            _harassMenu.Add("useEharass", new CheckBox("Use E", false));
            _harassMenu.AddLabel("Spider Spells");
            _harassMenu.Add("UseQharassSpider", new CheckBox("Use Q ", false));
            _harassMenu.Add("UseWharassSpider", new CheckBox("Use W ", false));
            _harassMenu.AddLabel("Prediction E HItchange");
            _harassMenu.Add("predE", new Slider("Use E Pred %", 70, 0, 100));
            _harassMenu.Add("predW", new Slider("Use W Pred %", 70, 0, 100));
            _harassMenu.AddLabel("Mana Manage");
            _harassMenu.Add("HarassMana", new Slider("Min. Mana for Harass Spells %", 60));


            _laneMenu = _menu.AddSubMenu("Lane Clear", "LaneClear");
            _laneMenu.AddGroupLabel("Jungle Clear Settings");
            _laneMenu.AddLabel("Human Spells");
            _laneMenu.Add("useQLane", new CheckBox("Use Q"));
            _laneMenu.Add("useWLane", new CheckBox("Use W"));
            _laneMenu.Add("useELane", new CheckBox("Use E"));
            _laneMenu.AddLabel("Spider Spells");
            _laneMenu.Add("useQspiderLane", new CheckBox("Use Q "));
            _laneMenu.Add("useWspiderLane", new CheckBox("Use W "));
            _laneMenu.AddLabel("Mana Manage");
            _laneMenu.Add("LaneMana", new Slider("Min. Mana for Lane Spells %", 60));

            _jungleMenu = _menu.AddSubMenu("Jungle Clear", "JungleClear");
            _jungleMenu.AddGroupLabel("Jungle Clear Settings");
            _jungleMenu.AddLabel("Human Spells");
            _jungleMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleMenu.Add("useWJungle", new CheckBox("Use W"));
            _jungleMenu.Add("useEJungle", new CheckBox("Use E"));
            _jungleMenu.AddLabel("Spider Spells");
            _jungleMenu.Add("useQspider", new CheckBox("Use Q "));
            _jungleMenu.Add("useWspider", new CheckBox("Use W "));
            _jungleMenu.AddLabel("Mana Manage");
            _jungleMenu.Add("JungleMana", new Slider("Min. Mana for Jungle Spells %", 25));


            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.AddGroupLabel("Auto SKills CC settings");
            _miscMenu.Add("CCQ", new CheckBox("Auto E on Enemy CC"));
            _miscMenu.AddLabel("Interrupter - Gapclose settings");
            _miscMenu.Add("interuptE", new CheckBox("Auto E for Interrupter"));
            _miscMenu.Add("Gapclose", new CheckBox("Auto E for Gapclose"));
            _miscMenu.AddLabel("Auto R in Base Settings");
            _miscMenu.Add("AutoR", new CheckBox("Use R In base for Speed(SpiderForm)"));

            _activatorMenu = _menu.AddSubMenu("Activator Settings", "ActivatorSettings");
            _activatorMenu.AddGroupLabel("Auto pot usage");
            _activatorMenu.Add("potion", new CheckBox("Use potions"));
            _activatorMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            _activatorMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            _activatorMenu.AddLabel("Auto Zhonyas Hourglass");
            _activatorMenu.Add("Zhonyas", new CheckBox("Use Zhonyas Hourglass"));
            _activatorMenu.Add("ZhonyasHp", new Slider("Use Zhonyas Hourglass If Your HP%", 20, 0, 100));
            _activatorMenu.AddLabel("Smite settings");
            _activatorMenu.Add("vSmiteSRU_Dragon_Elder", new CheckBox("Elder Dragon"));
            _activatorMenu.Add("vSmiteSRU_Dragon_Air", new CheckBox("Air Dragon"));
            _activatorMenu.Add("vSmiteSRU_Dragon_Earth", new CheckBox("Fire Dragon"));
            _activatorMenu.Add("vSmiteSRU_Dragon_Fire", new CheckBox("Earth Dragon"));
            _activatorMenu.Add("vSmiteSRU_Dragon_Water", new CheckBox("Water Dragon"));
            _activatorMenu.Add("SRU_Red", new CheckBox("Smite Red Buff"));
            _activatorMenu.Add("SRU_Blue", new CheckBox("Smite Blue Buff"));
            _activatorMenu.Add("SRU_Baron", new CheckBox("Smite Baron"));
            _activatorMenu.Add("SRU_Gromp", new CheckBox("Smite Gromp"));
            _activatorMenu.Add("SRU_Murkwolf", new CheckBox("Smite Wolf"));
            _activatorMenu.Add("SRU_Razorbeak", new CheckBox("Smite Bird"));
            _activatorMenu.Add("SRU_Krug", new CheckBox("Smite Golem"));
            _activatorMenu.Add("Sru_Crab", new CheckBox("Smite Crab"));
            _activatorMenu.AddGroupLabel("Ignite settings:");
            _activatorMenu.Add("spells.Ignite.Focus",
                new Slider("Use Ignite when target HP is lower than {0}(%)", 10, 1));

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            _drawMenu = _menu.AddSubMenu("Draw  settings", "Draw");
            _drawMenu.AddGroupLabel("Draw Settings:");
            _drawMenu.Add("nodraw",
                new CheckBox("No Display Drawing", false));
            _drawMenu.AddGroupLabel("HumanSpells");
            _drawMenu.Add("draw.Q",
                new CheckBox("Draw Q"));
            _drawMenu.Add("draw.W",
                new CheckBox("Draw W"));
            _drawMenu.Add("draw.E",
                new CheckBox("Draw E"));
            _drawMenu.AddLabel("SpiderSpells");
            _drawMenu.Add("draw.Q1",
                new CheckBox("Draw Q"));
            _drawMenu.Add("draw.W1",
                new CheckBox("Draw W"));
            _drawMenu.Add("draw.E1",
                new CheckBox("Draw E"));
            _drawMenu.Add("draw.R",
                new CheckBox("Draw R"));


            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

            Chat.Print("Elise The Troll!!", Color.Red);
            Chat.Print("Loaded Version 1 (5-9-2016)", Color.Red);
            Chat.Print("GL And Dont Feed!!", Color.Red);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_drawMenu["nodraw"].Cast<CheckBox>().CurrentValue) return;

            {
                if (_drawMenu["draw.Q"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.Q.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.W"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.W.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.E"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.E.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.Q1"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.Q1.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.W1"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.W2.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.E1"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.E2.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
                if (_drawMenu["draw.R"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = SpellManager.R.Range, BorderWidth = 2f}.Draw(
                        _Player.Position);
                }
            }
        }

        private static void Smite()
        {
            var unit =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        a =>
                            SrJungleMobsNames.Contains(a.BaseSkinName) &&
                            _Player.GetSummonerSpellDamage(a, DamageLibrary.SummonerSpells.Smite) >= a.Health &&
                            _activatorMenu[a.BaseSkinName].Cast<CheckBox>().CurrentValue &&
                            SpellManager.Smite.IsInRange(a))
                    .OrderByDescending(a => a.MaxHealth)
                    .FirstOrDefault();
            if (unit != null && SpellManager.Smite.IsReady())
                SpellManager.Smite.Cast(unit);
        }


        private static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (sender == null || !sender.IsEnemy || e.End.Distance(_Player) > SpellManager.E.Range ||
                !_miscMenu["gapclose"].Cast<CheckBox>().CurrentValue || !SpellManager.E.IsReady() || Humanoid) return;

            SpellManager.E.Cast(sender.Position);
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useE = _miscMenu["interuptE"].Cast<CheckBox>().CurrentValue;

            if (sender == null || !sender.IsEnemy) return;

            if (useE && SpellManager.E.IsReady() && Humanoid)
            {
                SpellManager.E.Cast(sender);
            }
        }


        private static
            void OnGameUpdate(EventArgs args)
        {
            if (CheckSkin())
            {
                Player.SetSkinId(SkinId());
            }
        }

        private static int SkinId()
        {
            return _skinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        private static bool CheckSkin()
        {
            return _skinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        private static
            void Game_OnTick(EventArgs args)
        {
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
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
            }
            Auto();
            AutoR();
            AutoPot();
            Smite();
            AutoHourglass();
            Killsteal();
        }


        private static
            void AutoHourglass()
        {
            var zhonyas = _activatorMenu["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = _activatorMenu["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && _Player.HealthPercent <= zhonyasHp && ZhonyaHourglass.IsReady())
            {
                ZhonyaHourglass.Cast();
            }
        }

        private static
            void AutoPot()
        {
            if (_activatorMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= _activatorMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") ||
                  Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= _activatorMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                }
            }
        }

        private static
            void Auto()
        {
            var qonCc = _miscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            if (qonCc && Humanoid)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < SpellManager.Q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        SpellManager.E.Cast(enemy);
                    }
                }
            }
        }


        private static
            void Killsteal()
        {
            var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                (a => a.HealthPercent)
                .Where(
                    a =>
                        !a.IsMe && a.IsValidTarget() && a.Distance(_Player) <= SpellManager.Q.Range && !a.IsDead &&
                        !a.IsZombie &&
                        a.HealthPercent <= 35);
            foreach (
                var target in
                    enemies)
            {
                if (!target.IsValidTarget())
                {
                    return;
                }

                if (_comboMenu["killsteal.Q"].Cast<CheckBox>().CurrentValue && SpellManager.Q.IsReady() &&
                    target.Health + target.AttackShield <
                    _Player.GetSpellDamage(target, SpellSlot.Q))
                {
                    SpellManager.Q.Cast(target);
                }
            }
        }

        private static void LaneClear()
        {
            var useQLane = _laneMenu["useQLane"].Cast<CheckBox>().CurrentValue;
            var useWLane = _laneMenu["useWLane"].Cast<CheckBox>().CurrentValue;
            var useELane = _laneMenu["useELane"].Cast<CheckBox>().CurrentValue;
            var useQspider = _laneMenu["useQspiderLane"].Cast<CheckBox>().CurrentValue;
            var useWspider = _laneMenu["useWspiderLane"].Cast<CheckBox>().CurrentValue;
            var mana = _jungleMenu["LaneMana"].Cast<Slider>().CurrentValue;
            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.ServerPosition,
                    _Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(SpellManager.Q.Range));
            if (count == 0) return;
            if (useQLane && _Player.ManaPercent > mana)
            {
                if (SpellManager.Q.IsReady() && Humanoid && _Player.ManaPercent > mana)
                {
                    SpellManager.Q.Cast(source);
                }
                if (SpellManager.W.IsReady() && Humanoid && _Player.ManaPercent > mana && useWLane)
                {
                    SpellManager.W.Cast(source);
                }
                if (SpellManager.E.IsReady() && useELane && _Player.ManaPercent > mana && Humanoid)
                {
                    SpellManager.E.Cast(source);
                }
                if (SpellManager.R.IsReady() && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() &&
                    !SpellManager.E.IsReady() && Humanoid)
                {
                    SpellManager.R.Cast();
                }
                if (SpellManager.Q1.IsReady() && useQspider && _Player.ManaPercent > mana && Spiderman)
                {
                    SpellManager.Q1.Cast(source);
                }
                if (SpellManager.W2.IsReady() && Spiderman && _Player.ManaPercent > mana && useWspider)
                {
                    SpellManager.W2.Cast();
                }
                if (SpellManager.R.IsReady() && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && Spiderman)

                {
                    SpellManager.R.Cast();
                }
            }
        }

        private static
            void JungleClear()
        {
            var useWJungle = _jungleMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = _jungleMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useEJungle = _jungleMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var useQspider = _jungleMenu["useQspider"].Cast<CheckBox>().CurrentValue;
            var useWspider = _jungleMenu["useWspider"].Cast<CheckBox>().CurrentValue;
            var mana = _jungleMenu["JungleMana"].Cast<Slider>().CurrentValue;

            if (useQJungle && _Player.ManaPercent > mana)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (SpellManager.Q.IsReady() && Humanoid && _Player.ManaPercent > mana && minion != null)
                {
                    SpellManager.Q.Cast(minion);
                }
                if (SpellManager.W.IsReady() && Humanoid && _Player.ManaPercent > mana && useWJungle && minion != null)
                {
                    SpellManager.W.Cast(minion.Position);
                }
                if (SpellManager.E.IsReady() && useEJungle && _Player.ManaPercent > mana && Humanoid && minion != null)
                {
                    SpellManager.E.Cast(minion);
                }
                if (SpellManager.R.IsReady() && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() &&
                    !SpellManager.E.IsReady() && Humanoid && minion != null)
                {
                    SpellManager.R.Cast();
                }
                if (SpellManager.Q1.IsReady() && useQspider && _Player.ManaPercent > mana && Spiderman && minion != null)
                {
                    SpellManager.Q1.Cast(minion);
                }
                if (SpellManager.W2.IsReady() && Spiderman && _Player.ManaPercent > mana && useWspider && minion != null)
                {
                    SpellManager.W2.Cast();
                }
                if (SpellManager.R.IsReady() && !SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && Spiderman &&
                    minion != null)
                {
                    SpellManager.R.Cast();
                }
            }
        }

        private static void Harass()
        {
            var useQ = _harassMenu["useQharass"].Cast<CheckBox>().CurrentValue;
            var useE = _harassMenu["useEharass"].Cast<CheckBox>().CurrentValue;
            var useW = _harassMenu["useWharass"].Cast<CheckBox>().CurrentValue;
            var useQspider = _harassMenu["UseQharassSpider"].Cast<CheckBox>().CurrentValue;
            var useWspider = _harassMenu["UseWharassSpider"].Cast<CheckBox>().CurrentValue;
            var predE = _harassMenu["predE"].Cast<Slider>().CurrentValue;
            var predW = _harassMenu["predW"].Cast<Slider>().CurrentValue;
            var mana = _harassMenu["HarassMana"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (!target.IsValidTarget(SpellManager.E.Range) || target == null)
            {
                return;
            }
            if (Humanoid && _Player.ManaPercent > mana)
            {
                if (useE && SpellManager.E.IsInRange(target) && SpellManager.E.IsReady())
                {
                    var prede = SpellManager.E.GetPrediction(target);
                    if (prede.HitChancePercent >= predE)
                    {
                        SpellManager.E.Cast(prede.CastPosition);
                    }
                }
                if (useQ && SpellManager.Q.IsReady() && target.IsValidTarget(SpellManager.Q.Range))
                {
                    SpellManager.Q.Cast(target);
                }
                if (useW && SpellManager.W.IsReady() && target.IsValidTarget(SpellManager.W.Range))
                {
                    var predw = SpellManager.W.GetPrediction(target);
                    if (predw.HitChancePercent >= predW)
                    {
                        SpellManager.W.Cast(predw.CastPosition);
                    }
                }
                if (target.HasBuff("buffelisecocoon") && SpellManager.Q1.IsReady() &&
                    target.IsValidTarget(SpellManager.Q1.Range))
                {
                    SpellManager.R.Cast();
                }
                if (!SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && !SpellManager.E.IsReady())
                {
                    SpellManager.R.Cast();
                }
                if (SpellManager.R.IsReady() && target.Distance(_Player) >= 600 && target.HealthPercent <= 25)
                {
                    SpellManager.R.Cast();
                }
            }
            if (Spiderman && _Player.ManaPercent > mana)
            {
                if (useQspider && SpellManager.Q1.IsReady())
                {
                    if (target.IsValidTarget(SpellManager.Q1.Range))
                    {
                        SpellManager.Q1.Cast(target);
                    }
                }
                if (useWspider && SpellManager.W2.IsReady())
                {
                    if (target.IsValidTarget(250))
                    {
                        SpellManager.W2.Cast();
                    }
                }
                if (SpellManager.R.IsReady() && !target.IsValidTarget(SpellManager.Q1.Range))

                {
                    SpellManager.R.Cast();
                }

                if (!SpellManager.Q1.IsReady() && !SpellManager.W2.IsReady() && SpellManager.R.IsReady())
                {
                    SpellManager.R.Cast();
                }
            }
        }

        private static void AutoR()
        {
            var autoR = _miscMenu["AutoR"].Cast<CheckBox>().CurrentValue;
            if (autoR && Humanoid)
                if (_Player.IsInShopRange() && SpellManager.R.Name == "EliseR")
                {
                    SpellManager.R.Cast();
                }
        }

        private static
            void Combo()
        {
            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useE = _comboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useQspider = _comboMenu["useQspider"].Cast<CheckBox>().CurrentValue;
            var useWspider = _comboMenu["useWspider"].Cast<CheckBox>().CurrentValue;
            var useEspider = _comboMenu["useEspider"].Cast<CheckBox>().CurrentValue;
            var predE = _comboMenu["predE"].Cast<Slider>().CurrentValue;
            var predW = _comboMenu["predW"].Cast<Slider>().CurrentValue;
            var combosmite = _comboMenu["smitecombo"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (!target.IsValidTarget(SpellManager.E.Range) || target == null)
            {
                return;
            }
            if (Humanoid)
            {
                if (useE && SpellManager.E.IsInRange(target) && SpellManager.E.IsReady())
                {
                    var prede = SpellManager.E.GetPrediction(target);
                    if (prede.HitChancePercent >= predE)
                    {
                        SpellManager.E.Cast(prede.CastPosition);
                    }
                }
                if (useQ && SpellManager.Q.IsReady() && target.IsValidTarget(SpellManager.Q.Range))
                {
                    SpellManager.Q.Cast(target);
                }
                if (useW && SpellManager.W.IsReady() && target.IsValidTarget(SpellManager.W.Range))
                {
                    var predw = SpellManager.W.GetPrediction(target);
                    if (predw.HitChancePercent >= predW)
                    {
                        SpellManager.W.Cast(predw.CastPosition);
                    }
                }
                if (target.HasBuff("buffelisecocoon") && SpellManager.Q1.IsReady() &&
                    target.IsValidTarget(SpellManager.Q1.Range))
                {
                    SpellManager.R.Cast();
                }
                if (!SpellManager.Q.IsReady() && !SpellManager.W.IsReady() && !SpellManager.E.IsReady())
                {
                    SpellManager.R.Cast();
                }
            }
            if (Spiderman)
            {
                if (useQspider && SpellManager.Q1.IsReady())
                {
                    if (target.IsValidTarget(SpellManager.Q1.Range))
                    {
                        SpellManager.Q1.Cast(target);
                    }
                }
                if (useWspider && SpellManager.W2.IsReady())
                {
                    if (target.IsValidTarget(250))
                    {
                        SpellManager.W2.Cast();
                    }
                }
                if (useEspider && target.Distance(_Player) >= 600)
                {
                    SpellManager.E2.Cast(target);
                }
                else if (SpellManager.E2.IsReady() && _Player.CountEnemiesInRange(950) == 1)
                {
                    SpellManager.E2.Cast(target);
                }
                if (SpellManager.R.IsReady() && !target.IsValidTarget(SpellManager.Q1.Range) &&
                    !SpellManager.E2.IsReady())
                {
                    SpellManager.R.Cast();
                }

                if (!SpellManager.Q1.IsReady() && !SpellManager.W2.IsReady() && SpellManager.R.IsReady())
                {
                    SpellManager.R.Cast();
                }
            }
            if (SpellManager.Smite.IsReady() && combosmite)
            {
                SpellManager.Smite.Cast(target);
            }
        }
    }
}