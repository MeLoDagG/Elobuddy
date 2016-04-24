using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace ChogathTheTroll
{
    internal class Program
    {
        public static Spell.Skillshot Q;
        public static Spell.Active E;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        private static Item HealthPotion;
        private static Item CorruptingPotion;
        private static Item RefillablePotion;
        private static Item TotalBiscuit;
        private static Item HuntersPotion;


        private static Menu _menu,
            _comboMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _autoPotHealMenu;

        private static AIHeroClient _target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static SpellSlot Ignite { get; private set; }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Chogath)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, (int) .625f, int.MaxValue, (int) 250f);
            E = new Spell.Active(SpellSlot.E);
            W = new Spell.Skillshot(SpellSlot.W, 650, SkillShotType.Cone, (int) .25f, int.MaxValue, (int) (30*0.5));
            R = new Spell.Targeted(SpellSlot.R, 250);
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);


            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            _menu = MainMenu.AddMenu("ChogathThetroll", "ChogathThetroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.Add("useQCombo", new CheckBox("Use Q"));
            _comboMenu.Add("useWCombo", new CheckBox("Use W"));
            _comboMenu.Add("useRCombo", new CheckBox("Use R"));
            _comboMenu.Add("useIgnite", new CheckBox("Use Ignite [ks]"));

            _jungleLaneMenu = _menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            _jungleLaneMenu.AddLabel("Lane Clear");
            _jungleLaneMenu.Add("UseQFarm", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("qFarm", new Slider("Cast Q if >= minions hit", 3, 1, 8));
            _jungleLaneMenu.Add("UseWFarm", new CheckBox("Use W"));
            _jungleLaneMenu.Add("wFarm", new Slider("Cast W if >= minions hit", 4, 1, 15));
            _jungleLaneMenu.AddSeparator();
            _jungleLaneMenu.AddLabel("Jungle Clear");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));

            _autoPotHealMenu = _menu.AddSubMenu("Potion", "Potion");
            _autoPotHealMenu.AddGroupLabel("Auto pot usage");
            _autoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            _autoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            _autoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));

            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.AddGroupLabel("Ks settings");
            _miscMenu.Add("useRks", new CheckBox("Use R ks"));
            _miscMenu.AddGroupLabel("Interrupter settings");
            _miscMenu.Add("interrupterQ", new CheckBox("Auto Q for Interrupter"));
            _miscMenu.Add("interrupterW", new CheckBox("Auto W for Interrupter"));
            _miscMenu.AddGroupLabel("Auto SKills CC settings");
            _miscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
            _miscMenu.Add("CCW", new CheckBox("Auto W on Enemy CC"));

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 7));


            _drawMenu = _menu.AddSubMenu("Drawing Settings");
            _drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            _drawMenu.Add("drawW", new CheckBox("Draw W Range"));
            _drawMenu.Add("drawR", new CheckBox("Draw R Range"));


            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;

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
                    if (sender.IsEnemy && W.IsReady() && sender.Distance(_Player) <= W.Range)
                    {
                        W.Cast(sender);
                    }
                    else if (useQint)
                    {
                        if (sender.IsEnemy && Q.IsReady() && sender.Distance(_Player) <= Q.Range)
                        {
                            Q.Cast(sender);
                        }
                    }
                }
            }
        }

        private static void Game_OnTick(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;


            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    CastQ();
                    CastW();
                    CastR();
                    CheckE(true);
                }
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
            UseIgnite();
            AutoPot();
            CastRKs();
        }

        private static void UseIgnite()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var useIgnite = _comboMenu["useIgnite"].Cast<CheckBox>().CurrentValue;

            if (useIgnite && target != null)
            {
                if (_Player.GetSummonerSpellDamage(target, DamageLibrary.SummonerSpells.Ignite) > target.Health)
                    _Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private static
            void AutoPot()
        {
            if (_autoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= _autoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
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
            if (Player.Instance.ManaPercent <= _autoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
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
            var QonCc = _miscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            var WonCc = _miscMenu["CCW"].Cast<CheckBox>().CurrentValue;
            if (QonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < Q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        Q.Cast(enemy);
                    }
                    if (WonCc)
                    {
                        if (enemy.Distance(Player.Instance) < W.Range &&
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

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && useQJungle && minion != null)
                {
                    Q.Cast(minion.Position);
                }

                if (W.IsReady() && useWJungle && minion != null)
                {
                    W.Cast(minion.Position);
                }
            }
        }

           private static
            void CheckE(bool shouldBeOn)
        {
            if (shouldBeOn)
            {
                if (!_Player.HasBuff("VorpalSpikes"))
                {
                    E.Cast();
                }
            }
            else
            {
                if (_Player.HasBuff("VorpalSpikes"))
                {
                    E.Cast();
                }
            }
        }
        
        private static void FarmQ()
        {
            var useQfarm = _jungleLaneMenu["UseQFarm"].Cast<CheckBox>().CurrentValue;

            if (Q.IsReady() && useQfarm)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(_Player) <= Q.Range))
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

            if (W.IsReady() && useWfarm)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(_Player) <= W.Range))
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

        private static void CastQ()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;

            {
                if (Q.IsReady() && useQ)
                {
                    var Predq = Q.GetPrediction(target).CastPosition.Extend(target.ServerPosition, float.MaxValue);
                    {
                        Q.Cast(Predq.To3D());
                    }
                }
            }
        }

        private static
            void CastW()
        {
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);

            if (targetW == null || !targetW.IsValidTarget()) return;

            Orbwalker.ForcedTarget = targetW;
            var useWCombo = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            {
                if (W.IsReady() && useWCombo)
                {
                    W.Cast(targetW);
                }
            }
        }


        private static
            void CastR()
        {
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (targetR == null || !targetR.IsValidTarget()) return;

            Orbwalker.ForcedTarget = targetR;

            var useRCombo = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            {
                if (R.IsReady() && useRCombo)
                {
                    if (_Player.GetSpellDamage(targetR, SpellSlot.R, 0) > targetR.Health)
                    {
                        R.Cast(targetR);
                    }
                }
            }
        }


        private static
            void CastRKs()
        {
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.Physical);
            if (targetR == null) return;
            var useRCombo = _miscMenu["useRks"].Cast<CheckBox>().CurrentValue;
            {
                if (R.IsReady() && useRCombo)
                {
                    if (_Player.GetSpellDamage(targetR, SpellSlot.R, 0) > targetR.Health)
                    {
                        R.Cast(targetR);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_target != null && _target.IsValid)
            {
            }

            if (Q.IsReady() && _drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Q.Range, Color.Red);
            }
            else
            {
                if (_drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, Q.Range, Color.DarkOliveGreen);
            }

            if (W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, W.Range, Color.Red);
            }
            else
            {
                if (_drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, W.Range, Color.DarkOliveGreen);
            }

            if (R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, R.Range, Color.Red);
            }
            else
            {
                if (_drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, R.Range, Color.DarkOliveGreen);
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

        public static int SkinId()
        {
            return _skinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return _skinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }
    }
}