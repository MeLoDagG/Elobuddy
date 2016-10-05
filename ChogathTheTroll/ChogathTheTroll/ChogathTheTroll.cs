using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;

namespace ChogathTheTroll
{
    internal class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Active E;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignitee;
        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item ZhonyaHourglass;


        private static Menu _menu,
            _comboMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _activatorMenu;

        private static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }


        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (EloBuddy.Player.Instance.Hero != Champion.Chogath)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, 750, int.MaxValue, 175);
            W = new Spell.Skillshot(SpellSlot.W, 650, SkillShotType.Cone, 250, 1750, 100);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Targeted(SpellSlot.R, 500);
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);
            ZhonyaHourglass = new Item(ItemId.Zhonyas_Hourglass);
            var slot2 = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            if (slot2 != SpellSlot.Unknown)
            {
                Ignitee = new Spell.Targeted(slot2, 600);
            }


            _menu = MainMenu.AddMenu("ChogathThetroll", "ChogathThetroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.AddGroupLabel("Combo Settings");
            _comboMenu.Add("useQCombo", new CheckBox("Use Q"));
            _comboMenu.Add("useWCombo", new CheckBox("Use W"));
            _comboMenu.Add("useRCombo", new CheckBox("Use R"));
            _comboMenu.AddLabel("Prediction  HItchange");
            _comboMenu.Add("predQ", new Slider("Use Q Pred {0}(%)", 70));
            _comboMenu.Add("predW", new Slider("Use W Pred {0}(%)", 70));
            _comboMenu.AddLabel("Higher % ->Higher chance of spell landing on target but takes more time to get casted");
            _comboMenu.AddLabel("Lower % ->Faster casting but lower chance that the spell will land on your target. ");

            _jungleLaneMenu = _menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            _jungleLaneMenu.AddGroupLabel("Lane Clear");
            _jungleLaneMenu.Add("UseQFarm", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("qFarm", new Slider("Cast Q if >= minions hit", 3, 1, 8));
            _jungleLaneMenu.Add("UseWFarm", new CheckBox("Use W"));
            _jungleLaneMenu.Add("wFarm", new Slider("Cast W if >= minions hit", 4, 1, 15));
            _jungleLaneMenu.Add("LaneMana", new Slider("USe Mana spell {0}(%)", 40));
            _jungleLaneMenu.AddLabel("Jungle Clear");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));
            _jungleLaneMenu.Add("JungleMana", new Slider("USe Mana spell {0}(%)", 40));

            _activatorMenu = _menu.AddSubMenu("Activator Settings", "ActivatorSettings");
            _activatorMenu.AddGroupLabel("Auto pot usage");
            _activatorMenu.Add("potion", new CheckBox("Use potions"));
            _activatorMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            _activatorMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            _activatorMenu.AddLabel("Auto Zhonyas Hourglass");
            _activatorMenu.Add("Zhonyas", new CheckBox("Use Zhonyas Hourglass"));
            _activatorMenu.Add("ZhonyasHp", new Slider("Use Zhonyas Hourglass If Your HP%", 20));
            _activatorMenu.AddLabel("Ignite settings:");
            _activatorMenu.Add("spells.Ignite.Focus",
                new Slider("Use Ignite when target HP is lower than {0}(%)", 10, 1));

            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.AddGroupLabel("Ks settings");
            _miscMenu.Add("useRks", new CheckBox("Use R ks"));
            _miscMenu.AddLabel("Interrupter settings");
            _miscMenu.Add("interrupterQ", new CheckBox("Auto Q for Interrupter"));
            _miscMenu.Add("interrupterW", new CheckBox("Auto W for Interrupter"));
            _miscMenu.AddLabel("Auto SKills CC settings");
            _miscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
            _miscMenu.Add("CCW", new CheckBox("Auto W on Enemy CC"));

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.AddGroupLabel("Skin Settings:");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 7));


            _drawMenu = _menu.AddSubMenu("Drawing Settings");
            _drawMenu.AddGroupLabel("Draw Settings:");
            _drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            _drawMenu.Add("drawW", new CheckBox("Draw W Range"));
            _drawMenu.Add("drawR", new CheckBox("Draw R Range"));
            _drawMenu.AddLabel("Damage indicators");
            _drawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            _drawMenu.Add("percent", new CheckBox("Damage percent info"));


            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += GameOnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            DamageIndicator.Initialize(SpellDamage.GetRawDamage);

            Chat.Print(
                "<font color=\"#ca0711\" >MeLoDag Presents </font><font color=\"#ffffff\" >ChogathThetroll </font><font color=\"#ca0711\" >Kappa Kippo</font>");
        }


        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useQint = _miscMenu["interrupterQ"].Cast<CheckBox>().CurrentValue;
            var useWint = _miscMenu["interrupterW"].Cast<CheckBox>().CurrentValue;

            {
                if (useWint)
                {
                    if (sender.IsEnemy && W.IsReady() && sender.Distance(Player) <= W.Range)
                    {
                        W.Cast(sender);
                    }
                    else if (useQint)
                    {
                        if (sender.IsEnemy && Q.IsReady() && sender.Distance(Player) <= Q.Range)
                        {
                            Q.Cast(sender);
                        }
                    }
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                CastR();
                CheckE(true);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                FarmQ();
                FarmW();
                CheckE(true);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                CheckE(true);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                CheckE(false);
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
                CheckE(true);
            }
            Auto();
            Ignite();
            AutoPot();
            CastRKs();
            AutoHourglass();
        }

        private static
            void AutoHourglass()
        {
            var zhonyas = _activatorMenu["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var zhonyasHp = _activatorMenu["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (zhonyas && Player.HealthPercent <= zhonyasHp && ZhonyaHourglass.IsReady())
            {
                ZhonyaHourglass.Cast();
            }
        }

        private static void Ignite()
        {
            var autoIgnite = TargetSelector.GetTarget(Ignitee.Range, DamageType.True);
            if (autoIgnite != null && autoIgnite.Health <= Player.GetSpellDamage(autoIgnite, Ignitee.Slot) ||
                autoIgnite != null &&
                autoIgnite.HealthPercent <= _activatorMenu["spells.Ignite.Focus"].Cast<Slider>().CurrentValue)
                Ignitee.Cast(autoIgnite);
        }

        private static
            void AutoPot()
        {
            if (_activatorMenu["potion"].Cast<CheckBox>().CurrentValue && !EloBuddy.Player.Instance.IsInShopRange() &&
                EloBuddy.Player.Instance.HealthPercent <= _activatorMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
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
            if (EloBuddy.Player.Instance.ManaPercent <= _activatorMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
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
            var wonCc = _miscMenu["CCW"].Cast<CheckBox>().CurrentValue;
            if (qonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(EloBuddy.Player.Instance) < Q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        Q.Cast(enemy);
                    }
                    if (wonCc)
                    {
                        if (enemy.Distance(EloBuddy.Player.Instance) < W.Range &&
                            (enemy.HasBuffOfType(BuffType.Stun)
                             || enemy.HasBuffOfType(BuffType.Snare)
                             || enemy.HasBuffOfType(BuffType.Suppression)
                             || enemy.HasBuffOfType(BuffType.Fear)
                             || enemy.HasBuffOfType(BuffType.Knockup)))
                        {
                            W.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static
            void JungleClear()
        {
            var useWJungle = _jungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = _jungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var junglemana = _jungleLaneMenu["JungleMana"].Cast<Slider>().CurrentValue;
            {
                var junleminions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(a => a.MaxHealth)
                        .FirstOrDefault(a => a.IsValidTarget(900));
                {
                    if (Q.IsReady() && useQJungle && Player.ManaPercent > junglemana && junleminions.IsValidTarget(Q.Range))
                    {
                        Q.Cast(junleminions);
                    }
                    if (W.IsReady() && useWJungle && Player.ManaPercent > junglemana && junleminions.IsValidTarget(W.Range))
                    {
                        W.Cast(junleminions);
                    }
                }
            }
        }

        private static
            void CheckE(bool shouldBeOn)
        {
            if (shouldBeOn)
            {
                if (!Player.HasBuff("VorpalSpikes"))
                {
                    E.Cast();
                }
            }
            else
            {
                if (Player.HasBuff("VorpalSpikes"))
                {
                    E.Cast();
                }
            }
        }

        private static void FarmQ()
        {
            var useQfarm = _jungleLaneMenu["UseQFarm"].Cast<CheckBox>().CurrentValue;
            var LaneMana = _jungleLaneMenu["LaneMana"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && useQfarm && Player.ManaPercent > LaneMana)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(Player) <= Q.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= _jungleLaneMenu["qFarm"].Cast<Slider>().CurrentValue)
                    {
                        Q.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void FarmW()
        {
            var useWfarm = _jungleLaneMenu["UseWFarm"].Cast<CheckBox>().CurrentValue;
            var LaneMana = _jungleLaneMenu["LaneMana"].Cast<Slider>().CurrentValue;

            if (W.IsReady() && useWfarm && Player.ManaPercent > LaneMana)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(Player) <= W.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= _jungleLaneMenu["wFarm"].Cast<Slider>().CurrentValue)
                    {
                        W.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void Combo()
        {
            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var usew = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var predQ = _comboMenu["predQ"].Cast<Slider>().CurrentValue;
            var predW = _comboMenu["predW"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }

            {
                if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    var predq = Q.GetPrediction(target);
                    if (predq.HitChancePercent >= predQ)
                    {
                        Q.Cast(predq.CastPosition);
                    }
                }
                if (usew && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    var predw = W.GetPrediction(target);
                    if (predw.HitChancePercent >= predW)
                    {
                        W.Cast(predw.CastPosition);
                    }
                    else
                    {
                        if (target.IsValidTarget(300))
                        {
                            W.Cast(target.Position);
                        }
                    }
                }
            }
        }

        private static
            void CastR()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (!target.IsValidTarget(R.Range) || target == null)
            {
                return;
            }
            var useRCombo = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            {
                if (R.IsReady() && useRCombo)
                {
                    if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static
            void CastRKs()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (!target.IsValidTarget(R.Range) || target == null)
            {
                return;
            }
            var useRCombo = _miscMenu["useRks"].Cast<CheckBox>().CurrentValue;
            {
                if (R.IsReady() && useRCombo)
                {
                    if (Player.GetSpellDamage(target, SpellSlot.R) > target.Health)
                    {
                        R.Cast(target);
                    }
                }
            }
        }

        private static void GameOnDraw(EventArgs args)
        {
            if (Q.IsReady() && _drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Red, Radius = Q.Range, BorderWidth = 2f}.Draw(Player.Position);
            }

            if (W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Red, Radius = W.Range, BorderWidth = 2f}.Draw(Player.Position);
            }

            if (R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Red, Radius = R.Range, BorderWidth = 2f}.Draw(Player.Position);
            }
            DamageIndicator.HealthbarEnabled =_drawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = _drawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (CheckSkin())
            {
                EloBuddy.Player.SetSkinId(SkinId());
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
    }
}