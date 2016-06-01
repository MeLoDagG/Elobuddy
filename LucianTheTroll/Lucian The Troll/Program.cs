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
using Color = System.Drawing.Color;

namespace Lucian_The_Troll
{
    internal class LucianTHeTroll
    {
        public static Spell.Targeted _q;
        public static Spell.Skillshot _q1;
        public static Spell.Skillshot _w;
        public static Spell.Skillshot _e;
        public static Spell.Skillshot _r;
        public static Spell.Active Heal;

        public static bool HasPassive()
        {
            return ObjectManager.Player.HasBuff("LucianPassiveBuff");
        }

        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static readonly Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static readonly Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static readonly Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static readonly Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static readonly Item Simitar = new Item(ItemId.Mercurial_Scimitar);


        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu,
            FleeMenu,
            Humanizer;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public static float HealthPercent
        {
            get { return _Player.Health/_Player.MaxHealth*100; }
        }

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Lucian)
            {
                return;
            }


            _q = new Spell.Targeted(SpellSlot.Q, 675);
            _q1 = new Spell.Skillshot(SpellSlot.Q, 1140, SkillShotType.Linear, 350, int.MaxValue, 75);
            _w = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 100);
            _e = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            _r = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 500, 2800, 110);

            var slot = _Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);

            Chat.Print("Lucian The Troll Loaded! Version 1.4", Color.DeepSkyBlue);
            Chat.Print("Have Fun And Dont Feed Kappa!", Color.DeepSkyBlue);

            Menu = MainMenu.AddMenu("Lucian The Troll", "LucianTheTroll");
            Menu.AddGroupLabel("Lucian The Troll Version 1.4");
            Menu.AddLabel("Last Update 1/6/2016");
            Menu.AddLabel("Made By MeLoDaG");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.AddLabel("Combo Logic");
            ComboMenu.Add("ComboLogic",
                new ComboBox(" ", 0, "AA > Q > AA > E > AA > W", "Q > AA > W > AA > E", "E > AA > Q > AA > W"));
            ComboMenu.AddLabel("W Settings For Normal Logic");
            ComboMenu.Add("useWrange", new Slider("Min Range Use W", 500, 0, 1000));
            ComboMenu.AddLabel("E Settings");
            ComboMenu.Add("useEcombo", new CheckBox("Use E"));
            ComboMenu.Add("ELogic", new ComboBox(" ", 0, "Side", "Cursor"));
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("UseRcomboHp", new CheckBox("Use R"));
            ComboMenu.Add("Hp", new Slider("Use R Enemy Health %", 45, 0, 100));
            ComboMenu.Add("combo.REnemies", new Slider("Min Enemyes for R", 1, 1, 5));
            ComboMenu.Add("ForceR",
                new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));

         /*   Humanizer = Menu.AddSubMenu("Humanizer Settings", "Humanizer");
            Humanizer.AddGroupLabel("Humanizer Settings");
            Humanizer.AddLabel("For Better And smoothest 250");
            Humanizer.AddLabel("For Faster 0");
            Humanizer.Add("Humanizer", new Slider("Humanizer", 0, 0, 1000)); */

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q normal - Exted - Test"));
            HarassMenu.Add("useWHarass", new CheckBox("Use W"));
            HarassMenu.Add("useWHarassMana", new Slider("Min. Mana for Harass %", 70, 0, 100));
            HarassMenu.AddLabel("AutoHarass");
            HarassMenu.Add("autoQHarass", new CheckBox("Auto Q Exted  Harass", false));
            HarassMenu.Add("autoQHarassMana", new Slider("Min. Mana for Auto Harass%", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Jungle Clear Settings", "FarmSettings");
            JungleLaneMenu.AddGroupLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWFarm", new CheckBox("Use W"));
            JungleLaneMenu.Add("useEFarm", new CheckBox("Use E"));
            JungleLaneMenu.Add("useWManalane", new Slider("Min. Mana for Laneclear Spells %", 70, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));
            JungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));
            JungleLaneMenu.Add("useWMana", new Slider("Min. Mana for Jungleclear Spells %", 70, 0, 100));

            ItemMenu = Menu.AddSubMenu("Item Settings", "ItemMenuettings");
            ItemMenu.AddGroupLabel("Botrk Settings");
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60, 1, 100));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60, 1, 100));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddGroupLabel("Auto QSS if :");
            ItemMenu.Add("Blind",
                new CheckBox("Blind", false));
            ItemMenu.Add("Charm",
                new CheckBox("Charm"));
            ItemMenu.Add("Fear",
                new CheckBox("Fear"));
            ItemMenu.Add("Polymorph",
                new CheckBox("Polymorph"));
            ItemMenu.Add("Stun",
                new CheckBox("Stun"));
            ItemMenu.Add("Snare",
                new CheckBox("Snare"));
            ItemMenu.Add("Silence",
                new CheckBox("Silence", false));
            ItemMenu.Add("Taunt",
                new CheckBox("Taunt"));
            ItemMenu.Add("Suppression",
                new CheckBox("Suppression"));

            AutoPotHealMenu = Menu.AddSubMenu("Potion & HeaL", "Potion & HeaL");
            AutoPotHealMenu.AddGroupLabel("Auto pot usage");
            AutoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            AutoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            AutoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            AutoPotHealMenu.AddGroupLabel("AUto Heal Usage");
            AutoPotHealMenu.Add("UseHeal", new CheckBox("Use Heal"));
            AutoPotHealMenu.Add("useHealHP", new Slider("Minimum Health % to use Heal", 20));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.AddGroupLabel("Gapcloser  settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto W for Gapcloser"));
            MiscMenu.AddGroupLabel("Ks Settings");
            MiscMenu.Add("UseQks", new CheckBox("Use Q ks"));
            MiscMenu.Add("UseWks", new CheckBox("Use W ks"));
            MiscMenu.Add("UseRks", new CheckBox("Use R ks"));
            MiscMenu.Add("UseRksRange", new Slider("Use Ulty Max Range[KS]", 1000, 500, 1400));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            FleeMenu = Menu.AddSubMenu("Flee Settings", "FleeSettings");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("FleeE", new CheckBox("Use E"));
            FleeMenu.Add("FleeW", new CheckBox("Use W"));

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.AddGroupLabel("Draw Settings");
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawQ.1", new CheckBox("Draw Q Extend Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawMenu.AddLabel("Damage indicators");
            DrawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            DrawMenu.Add("percent", new CheckBox("Damage percent info"));

            DamageIndicator.Initialize(ComboDamage);
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Drawing.OnDraw += Drawing_OnDraw;


        }

        public static
            void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = _q.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawQ.1"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = _q1.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = _w.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = _e.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = _r.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }

            DamageIndicator.HealthbarEnabled = DrawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = DrawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }

        public static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) >= 900)
            {
                _w.Cast(e.End);
            }
        }

        public static void AUtoheal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Chat.Print("Use Heal Noob!", Color.DeepSkyBlue);
            }
        }

        public static
            void AutoPot()
        {
            if (AutoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= AutoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") ||
                  Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    Chat.Print("Use Pot NooB!", Color.DeepSkyBlue);
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("Use Pot NooB!", Color.DeepSkyBlue);
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("Use Pot NooB!", Color.DeepSkyBlue);
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("Use Pot NooB!", Color.DeepSkyBlue);
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("Use Pot NooB!", Color.DeepSkyBlue);
                }
            }
        }

        public static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical);


            if (ItemMenu["useYoumu"].Cast<CheckBox>().CurrentValue && Youmuu.IsOwned() && Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(1500) == 1)
                {
                    Youmuu.Cast();
                }
            }
            if (target != null)
            {
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Cutlass.Id) &&
                    Item.CanUseItem(Cutlass.Id) &&
                    Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Cutlass.Id, target);
                }
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Botrk.Id) &&
                    Item.CanUseItem(Botrk.Id) &&
                    Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Botrk.Cast(target);
                }
            }
        }

        public static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.Buff.Type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Flee && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
            if (args.Buff.Type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
            {
                DoQss();
            }
        }

        public static void DoQss()
        {
            if (Qss.IsOwned() && Qss.IsReady())
            {
                Qss.Cast();
            }

            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Simitar.Cast();
            }
        }

        public static void Game_OnTick(EventArgs args)
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

        public static
            void OnGameUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                ItemUsage();
                AUtoheal();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
                HarassW();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                Ks();
                AutoHarass();
                UseRTarget();
                AutoPot();
            }
        }


        public static void Flee()
        {
            var targetW = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            var fleeW = FleeMenu["FleeW"].Cast<CheckBox>().CurrentValue;
            var fleeE = FleeMenu["FleeE"].Cast<CheckBox>().CurrentValue;

            if (fleeW && _w.IsReady() && targetW.IsValidTarget(_w.Range))
            {
                _w.Cast(targetW);
            }
            if (fleeE && _e.IsReady())
            {
                Player.CastSpell(SpellSlot.E, Game.CursorPos);
            }
        }

        public static void Ks()
        {
            var distance = MiscMenu["UseRksRange"].Cast<Slider>().CurrentValue;
            var useQks = MiscMenu["UseQks"].Cast<CheckBox>().CurrentValue;
            var useWks = MiscMenu["UseWks"].Cast<CheckBox>().CurrentValue;
            var useRks = MiscMenu["UseRks"].Cast<CheckBox>().CurrentValue;
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= _q.Range && e.IsValidTarget() && !e.IsInvulnerable))
            {
                if (useQks && _q.IsReady() &&
                    Qdamage(enemy) >= enemy.Health && enemy.Distance(_Player) <= _q.Range)
                {
                    _q.Cast(enemy);
                }
                if (useWks && _w.IsReady() &&
                    Wdamage(enemy) >= enemy.Health && enemy.Distance(_Player) <= _w.Range)
                {
                    _w.Cast(enemy);
                }
                if (useRks && _r.IsReady() && RDamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= distance)
                {
                    _r.Cast(enemy.Position);
                }
            }
        }

        public static void JungleClear()
        {
            var useQ = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useW = JungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useE = JungleLaneMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var junglemana = JungleLaneMenu["useWMana"].Cast<Slider>().CurrentValue;

            {
                var junleminions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(a => a.MaxHealth)
                        .FirstOrDefault(a => a.IsValidTarget(900));

                if (useQ && _Player.ManaPercent > junglemana && _q.IsReady() && junleminions.IsValidTarget(_q.Range) &&
                    !_Player.IsDashing() && !HasPassive())
                {
                    Core.DelayAction(() => _q.Cast(junleminions), 300);
                }
                if (useW && _Player.ManaPercent > junglemana && _w.IsReady() && junleminions.IsValidTarget(_w.Range) &&
                    !_Player.IsDashing() && !HasPassive())
                {
                    Core.DelayAction(() => _w.Cast(junleminions), 300);
                }
                if (useE && _Player.ManaPercent > junglemana && _e.IsReady() && junleminions.IsValidTarget(_e.Range) &&
                    !_Player.IsDashing() && !HasPassive())
                {
                    Core.DelayAction(() => _e.Cast(Game.CursorPos), 300);
                }
            }
        }

        public static void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useW = JungleLaneMenu["useWFarm"].Cast<CheckBox>().CurrentValue;
            var useE = JungleLaneMenu["useEFarm"].Cast<CheckBox>().CurrentValue;
            var lanemana = JungleLaneMenu["useWManalane"].Cast<Slider>().CurrentValue;


            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.ServerPosition,
                    _Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(_q.Range));
            if (count == 0) return;
            if (useQ && _Player.ManaPercent > lanemana && _q.IsReady() && !_Player.IsDashing() && !HasPassive())
            {
                Core.DelayAction(() => _q.Cast(source), 300);
            }
            if (useW && _Player.ManaPercent > lanemana && _w.IsReady() && !_Player.IsDashing() && !HasPassive())
            {
                Core.DelayAction(() => _w.Cast(source), 300);
            }
            if (useE && _Player.ManaPercent > lanemana && _e.IsReady() && !_Player.IsDashing() && !HasPassive())
            {
                Core.DelayAction(() => _e.Cast(Game.CursorPos), 300);
            }
        }

        public static void AutoHarass()
        {
            var autoQmana = HarassMenu["autoQHarassMana"].Cast<Slider>().CurrentValue;
            var autoQharass = HarassMenu["autoQHarass"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(_q1.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (_q.IsReady() && autoQharass && target.IsValidTarget(_q1.Range) && _Player.ManaPercent >= autoQmana)
            {
                CastExtendedQ();
            }
        }

        public static
            void Harass()
        {
            var qharass = HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(_q1.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (_q.IsReady() && qharass && target.IsValidTarget(_q1.Range) && _Player.ManaPercent >= wmana)
            {
                CastExtendedQ();
                _q.Cast(target);
            }
        }

        public static
            void HarassW()
        {
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var wharass = HarassMenu["useWHarass"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(_w.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (_w.IsReady() && target.IsValidTarget(_w.Range) && wharass && _Player.ManaPercent >= wmana)
            {
                var predW = _w.GetPrediction(target);
                if (predW.HitChance >= HitChance.High)
                {
                    _w.Cast(predW.CastPosition);
                }
            }
        }
        //Gredit D4mnedN00B
        public static void CastExtendedQ()
        {
          var target = TargetSelector.SelectedTarget != null &&
                             TargetSelector.SelectedTarget.Distance(Player.Instance) < 2000
                    ? TargetSelector.SelectedTarget
                    : TargetSelector.GetTarget(_q1.Range, DamageType.Physical);

                if (!target.IsValidTarget(_q1.Range))
                    return;
                var predPos = _q1.GetPrediction(target);
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(Player.Instance) <= _q1.Range);
                var champs = EntityManager.Heroes.Enemies.Where(m => m.Distance(Player.Instance) <= _q1.Range);
                var monsters =
                    EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(Player.Instance) <= _q1.Range);
                {
                    foreach (var minion in from minion in minions
                        let polygon = new Geometry.Polygon.Rectangle(
                            (Vector2) Player.Instance.ServerPosition,
                            Player.Instance.ServerPosition.Extend(minion.ServerPosition, _q1.Range), 65f)
                        where polygon.IsInside(predPos.CastPosition)
                        select minion)
                    {
                        _q.Cast(minion);
                    }

                    foreach (var champ in from champ in champs
                        let polygon = new Geometry.Polygon.Rectangle(
                            (Vector2) Player.Instance.ServerPosition,
                            Player.Instance.ServerPosition.Extend(champ.ServerPosition, _q1.Range), 65f)
                        where polygon.IsInside(predPos.CastPosition)
                        select champ)
                    {
                        _q.Cast(champ);
                    }

                    foreach (var monster in from monster in monsters
                        let polygon = new Geometry.Polygon.Rectangle(
                            (Vector2) Player.Instance.ServerPosition,
                            Player.Instance.ServerPosition.Extend(monster.ServerPosition, _q1.Range), 65f)
                        where polygon.IsInside(predPos.CastPosition)
                        select monster)
                    {
                        _q.Cast(monster);
                    }
                }
            }
         
        public static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (target != null &&
                (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && _r.IsReady() && target.IsValid &&
                 !Player.HasBuff("lucianr"))) _r.Cast(target.Position);
        }

        public static void Combo()
        {
            var comboLogic = ComboMenu["ComboLogic"].Cast<ComboBox>().CurrentValue;
            var useWrange = ComboMenu["useWrange"].Cast<Slider>().CurrentValue;
            var logice = ComboMenu["ELogic"].Cast<ComboBox>().CurrentValue;
            var usee = ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var userhp = ComboMenu["UseRcomboHp"].Cast<CheckBox>().CurrentValue;
            var enemyhp = ComboMenu["hp"].Cast<Slider>().CurrentValue;
            var useRminPl = ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
        //    var humanizer = Humanizer["Humanizer"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(_q.Range) || target == null)
            {
                return;
            }
            if (comboLogic == 1)
            {
                if (_q.IsReady() && target.IsValidTarget(_q.Range) && !HasPassive())
                {
                     _q.Cast(target);
                    Orbwalker.ResetAutoAttack();
                }
                if (logice == 0 && usee && _e.IsReady() &&
                    target.IsValidTarget(450) && !HasPassive())
                {
                    _e.Cast(Side(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
                if (logice == 1 && usee && _e.IsReady() && !HasPassive())
                {
                    Player.CastSpell(SpellSlot.E, Game.CursorPos);
                    Orbwalker.ResetAutoAttack();
                }
                if (_w.IsReady() && target.Distance(_Player) <= useWrange && !HasPassive())
                {
                    var predW = _w.GetPrediction(target);
                    if (predW.HitChance <= HitChance.High)
                    {
                       _w.Cast(predW.CastPosition);
                       Orbwalker.ResetAutoAttack();
                    }
                }
                if (userhp && target.HealthPercent <= enemyhp && _Player.CountEnemiesInRange(_r.Range) == useRminPl &&
                   _r.IsReady() && target.IsValidTarget(_r.Range))
                {
                    _r.Cast(target.Position);
                }
            }
            if (comboLogic == 2)
            {
                if (logice == 0 && usee && _e.IsReady() && target.IsValidTarget(_q1.Range) && !HasPassive())
                {
                    _e.Cast(Side(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                   // Core.DelayAction(() => _e.Cast(Side(_Player.Position.To2D(), target.Position.To2D(), 65).To3D()), humanizer);
                    Orbwalker.ResetAutoAttack();
                }
                if (logice == 1 && usee && _e.IsReady() && target.IsValidTarget(_q1.Range) && !HasPassive())
                {
                    Player.CastSpell(SpellSlot.E, Game.CursorPos);
                  //  Core.DelayAction(() => Player.CastSpell(SpellSlot.E, Game.CursorPos), humanizer);
                    Orbwalker.ResetAutoAttack();
                }
                if (_q.IsReady() && target.IsValidTarget(550) && !HasPassive())
                {
                    _q.Cast(target);
                    Orbwalker.ResetAutoAttack();
                    //   Core.DelayAction(() => _q.Cast(target), humanizer);
                }
                if (_w.IsReady() && target.Distance(_Player) <= useWrange && !HasPassive())
                {
                    var predW = _w.GetPrediction(target);
                    if (predW.HitChance <= HitChance.High)
                    {
                        _w.Cast(predW.CastPosition);
                        Orbwalker.ResetAutoAttack();
                        //  Core.DelayAction(() => _w.Cast(predW.CastPosition), humanizer);
                    }
                }
                if (userhp && target.HealthPercent <= enemyhp && _Player.CountEnemiesInRange(_r.Range) == useRminPl &&
                        _r.IsReady() && target.IsValidTarget(_r.Range))
                    {
                        _r.Cast(target.Position);
                    }
                }
            }
        

        public static
            void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (ComboMenu["ComboLogic"].Cast<ComboBox>().CurrentValue == 0 && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable || !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                    return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;

                var logice = ComboMenu["ELogic"].Cast<ComboBox>().CurrentValue;
                var usee = ComboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
         
                if (_q.IsReady())
                {
                    Core.DelayAction(() => _q.Cast(enemy), 0);
                    Orbwalker.ResetAutoAttack();
                }
                if (logice == 0 && usee && _e.IsReady())
                {
                    Core.DelayAction(() => _e.Cast(Side(_Player.Position.To2D(), target.Position.To2D(), 65).To3D()),0);
                    Orbwalker.ResetAutoAttack();
                }
                if (logice == 1 && usee && _e.IsReady())
                {
                    Core.DelayAction(() => Player.CastSpell(SpellSlot.E, Game.CursorPos), 0);
                    Orbwalker.ResetAutoAttack();
                }
                if (_w.IsReady())
                {
                    var predW = _w.GetPrediction(enemy);
                    if (predW.HitChance <= HitChance.High)
                    {
                        Core.DelayAction(() => _w.Cast(predW.CastPosition), 300);
                        Orbwalker.ResetAutoAttack();
                    }
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

        #region dmg

        public static float ComboDamage(Obj_AI_Base hero)
        {
            var damage = _Player.GetAutoAttackDamage(hero);
            if (_r.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.R);
            if (_e.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.E);
            if (_w.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.W);
            if (_q.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.Q);

            return damage;
        }

        public static float Qdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new[] {0, 80, 110, 140, 170, 200}[_q.Level] +
                     (new[] {0, 0.6, 0.75, 0.9, 1.05, 1.2}[_q.Level]*_Player.FlatPhysicalDamageMod
                         )));
        }

        public static float Wdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 60, 100, 140, 180, 220}[_w.Level] + 0.9f*_Player.FlatMagicDamageMod);
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                new[] {0, 20, 35, 50}[_r.Level] + 0.2f*_Player.FlatPhysicalDamageMod +
                0.1f*_Player.FlatMagicDamageMod);
        }

        #endregion
    }
}