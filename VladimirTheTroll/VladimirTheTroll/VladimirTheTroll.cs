using System;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace VladimirTheTroll
{
    internal class Vladimir
    {
        public static Spell.Targeted Q;
        public static Spell.Active E;
        public static Spell.Active W;
        public static Spell.Skillshot R;
        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;

        public static Menu _menu,
            _comboMenu,
            _HarassMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _autoPotHealMenu;


        public static AIHeroClient _target;
        public static SpellSlot Ignite { get; private set; }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Vladimir)
            {
                return;
            }

            Q = new Spell.Targeted(SpellSlot.Q, 600);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 610);
            R = new Spell.Skillshot(SpellSlot.R, 700, SkillShotType.Circular, 250, 1200, 150);

            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);


            _menu = MainMenu.AddMenu("VladimirTheTroll", "VladimirTheTroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.Add("ComboMode", new ComboBox(" ", 0, "E->Q->W->R", "Burst"));
            _comboMenu.Add("useQCombo", new CheckBox("Use Q"));
            _comboMenu.Add("useWCombo", new CheckBox("Use W"));
            _comboMenu.Add("useECombo", new CheckBox("Use E"));
            _comboMenu.Add("useRCombo", new CheckBox("Use R"));
            _comboMenu.Add("Rcount", new Slider("Use R Only If Die >= ", 2, 1, 5));
            _comboMenu.Add("UseIgnite", new CheckBox("Use ignite if combo killable"));

            _HarassMenu = _menu.AddSubMenu("Harass", "Harass");
            _HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            _HarassMenu.Add("useEHarass", new CheckBox("Use E"));

            _jungleLaneMenu = _menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            _jungleLaneMenu.AddSeparator(12);
            _jungleLaneMenu.AddLabel("Lane Clear");
            _jungleLaneMenu.Add("qFarm", new CheckBox("Cast Q LastHit[ForAllMode]"));
            _jungleLaneMenu.Add("FarmE", new CheckBox("Use E"));
            _jungleLaneMenu.Add("FarmEmana", new Slider("Cast E if >= minions hit", 4, 1, 15));
            _jungleLaneMenu.AddSeparator(12);
            _jungleLaneMenu.AddLabel("Jungle Clear");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));

            _autoPotHealMenu = _menu.AddSubMenu("Potion", "Potion");
            _autoPotHealMenu.AddGroupLabel("Auto pot usage");
            _autoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            _autoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            _autoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));

            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.Add("ksQ", new CheckBox("Killsteal Q"));

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 7));


            _drawMenu = _menu.AddSubMenu("Drawing Settings");
            _drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            _drawMenu.Add("drawW", new CheckBox("Draw W Range"));
            _drawMenu.Add("drawE", new CheckBox("Draw E Range"));
            _drawMenu.Add("drawR", new CheckBox("Draw R Range"));


            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;


            Chat.Print(
                "<font color=\"#d80303\" >MeLoDag Presents </font><font color=\"#ffffff\" > Vladimir </font><font color=\"#d80303\" >Kappa Kippo</font>");
        }


        private static void Game_OnTick(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Combo();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    FarmQ();
                    FarmE();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    FarmQ();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    JungleClear();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    Harass();
                    FarmQ();
                }
                AutoPot();
                Killsteal();
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


        private static void Killsteal()
        {
            var ksQ = _miscMenu["ksQ"].Cast<CheckBox>().CurrentValue;

            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (ksQ && Q.IsReady() &&
                    QDamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= Q.Range)
                {
                    Q.Cast(enemy);
                }
            }
        }

        private static
            void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var useE = _comboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var rCount = _comboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var useIgnite = _comboMenu["UseIgnite"].Cast<CheckBox>().CurrentValue;
            {
                if (_comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue == 0)

                    if (E.IsReady() && useE)
                    {
                        E.Cast();
                    }
                if (Q.IsReady() && useQ)
                {
                    Q.Cast(target);
                }

                if (W.IsReady() && useW)
                {
                    W.Cast();
                }

                if (R.IsReady() && _Player.CountEnemiesInRange(R.Range) >= rCount && useR)
                {
                    if (RDamage(target) >= target.Health)
                    {
                        R.Cast(target);
                    }
                }
                if (useIgnite && target != null)
                {
                    if (_Player.Distance(target) <= 600 && QDamage(target) >= target.Health)
                        _Player.Spellbook.CastSpell(Ignite, target);
                }
            }

            if (_comboMenu["ComboMode"].Cast<ComboBox>().CurrentValue == 1)

                if (R.IsReady() && useR)
                {
                    R.Cast(target);
                }
            if (E.IsReady() && useE)
            {
                E.Cast();
            }
            if (Q.IsReady() && useQ)
            {
                Q.Cast(target);
            }

            if (W.IsReady() && useW)
            {
                W.Cast();
            }
            if (useIgnite && target != null)
            {
                if (_Player.Distance(target) <= 600 && QDamage(target) >= target.Health)
                    _Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        private static
            void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var useEH = _HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue;
            var useQh = _HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;

            {
                if (E.IsReady() && useEH)
                {
                    E.Cast();
                }
                if (Q.IsReady() && useQh)
                {
                    Q.Cast(target);
                }
            }
        }

        public static
            float QDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] {0, 90, 125, 160, 195, 230}[Q.Level] + (0.6f*_Player.FlatMagicDamageMod)));
        }


        public static float RDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] {0, 168, 280, 392}[R.Level] + (0.78f*_Player.FlatMagicDamageMod)));
        }


        private static
            void JungleClear()
        {
            var useEJungle = _jungleLaneMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = _jungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (Q.IsReady() && useQJungle && minion != null)
                {
                    Q.Cast(minion);
                }

                if (E.IsReady() && useEJungle && minion != null)
                {
                    E.Cast();
                }
            }
        }


        private static void FarmQ()
        {
            var useQ = _jungleLaneMenu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(_Player) <= Q.Range &&
                            m.Health <= QDamage(m) - 20 &&
                            m.IsValidTarget());

            if (Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }

        private static
            void FarmE()
        {
            var FarmE = _jungleLaneMenu["FarmE"].Cast<CheckBox>().CurrentValue;
            var FarmEmana = _jungleLaneMenu["FarmEmana"].Cast<Slider>().CurrentValue;
            if (E.IsReady() && FarmE)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(_Player) <= E.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= FarmEmana && FarmE)
                    {
                        E.Cast();
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


            if (W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, W.Range, Color.Red);
            }


            if (E.IsReady() && _drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, E.Range, Color.Red);
            }
            else if (R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, R.Range, Color.Red);
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