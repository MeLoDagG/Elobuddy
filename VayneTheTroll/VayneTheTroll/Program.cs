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

namespace VayneTheTroll
{
    internal class VayneTheTroll
    {
        public static Spell.Ranged _q;
        public static Spell.Targeted _e;
        public static Spell.Skillshot _e2;
        public static Spell.Active _r;
        public static Spell.Active Heal;

        private static readonly string[] MobNames =
        {
            "SRU_Red", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf",
            "SRU_Razorbeak", "SRU_Krug"
        };

        private static Item HealthPotion;
        private static Item CorruptingPotion;
        private static Item RefillablePotion;
        private static Item TotalBiscuit;
        private static Item HuntersPotion;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static DangerLevel Dangerlvl;


        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu,
            FleeMenu;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Vayne)
            {
                return;
            }

            _q = new Spell.Skillshot(SpellSlot.Q, int.MaxValue, SkillShotType.Linear);
            _e = new Spell.Targeted(SpellSlot.E, 590);
            _e2 = new Spell.Skillshot(SpellSlot.E, 590, SkillShotType.Linear, 250, 1250);
            _r = new Spell.Active(SpellSlot.R);


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

            Chat.Print(
                "<font color=\"#ef0101\" >MeLoDag Presents </font><font color=\"#ffffff\" > VayneTHeTroll </font><font color=\"#ef0101\" >Kappa Kippo</font>");
            Chat.Print("Version 1.3 (14/7/2016)", Color.AntiqueWhite);
            Chat.Print("Gl and HF also Dont Feed!!", Color.AntiqueWhite);


            Menu = MainMenu.AddMenu("VayneTheTroll", "VayneTheTroll");
            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Q Settings");
            ComboMenu.Add("useQcombo", new ComboBox("Q Logic", 0, "Side", "Cursor", "SmartQ", "SafeQ", "AggroQ"));
            ComboMenu.Add("UseQulty", new CheckBox("Auto Q when using R", false));
            ComboMenu.AddLabel("W Settings:");
            ComboMenu.Add("FocusW", new CheckBox("Focus target With Silver Bolt"));
            ComboMenu.AddLabel("E Settings:");
            ComboMenu.AddLabel("Use E on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                ComboMenu.Add("useEcombo" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            ComboMenu.Add("pushDistance", new Slider("Push Distance", 410, 350, 420));
            ComboMenu.AddLabel("Use R Settings");
            ComboMenu.Add("useRCombo", new CheckBox("Use R"));
            ComboMenu.Add("Rcount", new Slider("R when enemies >= ", 2, 1, 5));
            ComboMenu.Add("noaa", new CheckBox("No AA If active Ulty "));
            ComboMenu.Add("Noaaslider", new Slider("No AA when enemy in range ", 2, 1, 5));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddLabel("SoonTM");
            //  HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            //   HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            //   HarassMenu.Add("useEHarassMana", new Slider("E Mana > %", 70, 0, 100));
            //  HarassMenu.Add("useQHarassMana", new Slider("Q Mana > %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q[LastHit]"));
            JungleLaneMenu.Add("useQMana", new Slider("Q Mana > %", 75, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));
            JungleLaneMenu.Add("useQJunglemana", new Slider("Mana > %", 40, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.AddGroupLabel("Gapcloser Settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto Q for Gapcloser"));
            MiscMenu.AddGroupLabel("Interrupter Settings & Dangerlvl");
            MiscMenu.Add("interrupter", new CheckBox("Auto E for Interrupter"));
            MiscMenu.Add("Dangerlvl", new ComboBox("Min DangerLevel to interrupt", 2, "Low", "Medium", "High"));
            MiscMenu.Add("delayinter", new Slider("Use Interrupter Delay(ms)", 50));


            //     MiscMenu.Add("UseQks", new CheckBox("Use Q ks"));

            AutoPotHealMenu = Menu.AddSubMenu("Potion & Heal", "Potion & Heal");
            AutoPotHealMenu.AddGroupLabel("Auto pot usage");
            AutoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            AutoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            AutoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));
            AutoPotHealMenu.AddGroupLabel("AUto Heal Usage");
            AutoPotHealMenu.Add("UseHeal", new CheckBox("Use Heal"));
            AutoPotHealMenu.Add("useHealHP", new Slider("Minimum Health % to use Heal", 20));

            ItemMenu = Menu.AddSubMenu("Item Settings", "ItemMenuettings");
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60, 1, 100));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60, 1, 100));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddSeparator();
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));
            ItemMenu.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
            ItemMenu.Add("Stun", new CheckBox("Stun", true));
            ItemMenu.Add("Blind", new CheckBox("Blind", true));
            ItemMenu.Add("Charm", new CheckBox("Charm", true));
            ItemMenu.Add("Suppression", new CheckBox("Suppression", true));
            ItemMenu.Add("Polymorph", new CheckBox("Polymorph", true));
            ItemMenu.Add("Fear", new CheckBox("Fear", true));
            ItemMenu.Add("Taunt", new CheckBox("Taunt", true));
            ItemMenu.Add("Silence", new CheckBox("Silence", false));
            ItemMenu.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));

            FleeMenu = Menu.AddSubMenu("Flee Settings", "FleeSettings");
            FleeMenu.Add("fleeQ", new CheckBox("Use Q"));
            FleeMenu.Add("fleeE", new CheckBox("Use E"));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            StringList(SkinMenu, "skin.Id", "Skin",
                new[]
                {
                    "Default", "Vindicator", "Aristocrat ", "Dragonslayer ", "Heartseeker", "SKT T1", "Arclight",
                    "DragonSlayer Chaos", "DragonSlayer Curse", "DragonSlayer Element"
                },
                0);

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawStun", new CheckBox("Draw Stun Pos"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));


            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawStun"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(_e.Range + _q.Range,
                    DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var color = Color.Red;
                    for (var i = 1; i < 8; i++)
                    {
                        var targetBehind = t.Position +
                                           Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position)*i*50;

                        if (!targetBehind.IsWall())
                        {
                            color = Color.Aqua;
                        }
                        else
                        {
                            color = Color.Red;
                        }
                    }

                    var tt = t.Position + Vector3.Normalize(t.ServerPosition - ObjectManager.Player.Position)*8*50;

                    var startpos = t.Position;
                    var endpos = tt;
                    var endpos1 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(45*(float) Math.PI/180).To3D()*
                                  t.BoundingRadius;
                    var endpos2 = tt +
                                  (startpos - endpos).To2D().Normalized().Rotated(-45*(float) Math.PI/180).To3D()*
                                  t.BoundingRadius;

                    var width = 2;

                    var x = new Geometry.Polygon.Line(startpos, endpos);
                    {
                        x.Draw(color, width);
                    }

                    var y = new Geometry.Polygon.Line(endpos, endpos1);
                    {
                        y.Draw(color, width);
                    }

                    var z = new Geometry.Polygon.Line(endpos, endpos2);
                    {
                        z.Draw(color, width);
                    }
                }
                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    new Circle {Color = Color.Red, Radius = _e.Range, BorderWidth = 2f}.Draw(_Player.Position);
                }
            }
        }

        public static void StringList(Menu menu, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = menu.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange +=
                delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
                {
                    sender.DisplayName = displayName + ": " + values[args.NewValue];
                };
        }

        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useEint = MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue;

            if (sender.IsMe && sender.IsAlly && useEint)
            {
                return;
            }

            switch (MiscMenu["Dangerlvl"].Cast<ComboBox>().CurrentValue)
            {
                case 0:
                    Dangerlvl = DangerLevel.Low;
                    break;
                case 1:
                    Dangerlvl = DangerLevel.Medium;
                    break;
                case 2:
                    Dangerlvl = DangerLevel.High;
                    break;
                default:
                    Dangerlvl = DangerLevel.High;
                    break;
            }

            if (_e.CanCast(sender) && sender.IsValidTarget(_e.Range) && e.DangerLevel == Dangerlvl)
            {
                var delay = MiscMenu["delayinter"].Cast<Slider>().CurrentValue;
                Core.DelayAction(() => _e.Cast(sender), delay);
            }
        }

        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var useQgap = MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue;

            if (useQgap && sender.IsEnemy &&
                e.End.Distance(_Player) <= 350)
            {
                Player.CastSpell(SpellSlot.Q, sender.Position - 450);
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.R)
                    if (ComboMenu["UseQulty"].Cast<CheckBox>().CurrentValue && _q.IsReady())
                    {
                        Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
            }
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && ComboMenu["noaa"].Cast<CheckBox>().CurrentValue
                && (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                &&
                (_Player.CountEnemiesInRange(1000f) >=
                 ComboMenu["Noaaslider"].Cast<Slider>().CurrentValue)
                && UltActive() || _Player.HasBuffOfType(BuffType.Invisibility)
                && Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                args.Process = false;
            }
        }

        public static
            void Game_OnTick(EventArgs args)
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
                UseHeal();
                ItemUsage();
                ComboR();
                Condemn();
                FocusW();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                //  Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }
            AutoPot();
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
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
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
                    Chat.Print("<font color=\"#ffffff\" > USe Pot </font>");
                }
            }
        }

        public static
            void UseHeal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                _Player.HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Chat.Print("<font color=\"#ffffff\" > USe Heal Noob </font>");
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
            var type = args.Buff.Type;

            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Fear && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
            }
            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 1 &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Fear && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQss();
                }
            }
        }

        public static
            void DoQss()
        {
            if (ItemMenu["useQSS"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady() &&
                ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Qss.Cast(), ItemMenu["QssDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady() && ObjectManager.Player.CountEnemiesInRange(1800) > 0)
            {
                Core.DelayAction(() => Simitar.Cast(), ItemMenu["QssDelay"].Cast<Slider>().CurrentValue);
            }
        }

        public static void Flee()
        {
            var target = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var fleeQ = FleeMenu["fleeQ"].Cast<CheckBox>().CurrentValue;
            var fleeE = FleeMenu["fleeE"].Cast<CheckBox>().CurrentValue;

            if (fleeE && _e.IsReady() && target.IsValidTarget(_e.Range))
            {
                _e.Cast(target);
            }
            if (fleeQ && _q.IsReady())
            {
                Player.CastSpell(SpellSlot.Q, Game.CursorPos);
            }
        }

        public static
            void JungleClear()
        {
            var useEJungle = JungleLaneMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var usemana = JungleLaneMenu["useQJunglemana"].Cast<Slider>().CurrentValue;
            foreach (
                var mob in
                    EntityManager.MinionsAndMonsters.Monsters.Where(
                        x => x.IsValid && !x.IsDead && x.Position.Distance(_Player) < _Player.GetAutoAttackRange(x)))
            {
                if (useQJungle && _q.IsReady() && mob != null && mob.IsValidTarget(_q.Range) &&
                    _Player.ManaPercent >= usemana)
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
                if (useEJungle && _e.IsReady() && mob != null && mob.IsValidTarget(_e.Range) &&
                    MobNames.Contains(mob.BaseSkinName) &&
                    _Player.ManaPercent >= usemana)
                {
                    _e.Cast(mob);
                }
            }
        }

        public static
            void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useQMana = JungleLaneMenu["useQMana"].Cast<Slider>().CurrentValue;

            if (_q.IsReady() && useQ && _Player.ManaPercent >= useQMana)
            {
                var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    _Player.Position, _Player.GetAutoAttackRange());
                foreach (var minions in
                    Minions.Where(
                        minions => minions.Health < QDamage(minions)))
                {
                    if (minions != null)
                    {
                        Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                }
            }
        }

        private static void FocusW()
        {
            var focusW = ComboMenu["FocusW"].Cast<CheckBox>().CurrentValue;
            var focusWtarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    h =>
                        h.ServerPosition.Distance(_Player.ServerPosition) < 600 &&
                        h.GetBuffCount("vaynesilvereddebuff") == 1);
            if (focusW && focusWtarget.IsValidTarget())
            {
                Orbwalker.ForcedTarget = focusWtarget;
            }
        }

        public static void Condemn()
        {
            {
                var enemies = EntityManager.Heroes.Enemies.OrderByDescending
                    (a => a.HealthPercent).Where(a => !a.IsMe && a.IsValidTarget() && a.Distance(_Player) <= _e.Range);
                var target = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
                var distance = ComboMenu["pushDistance"].Cast<Slider>().CurrentValue;
                if (!target.IsValidTarget(_e.Range) || target == null)
                {
                    return;
                }
                if (_e.IsReady() && target.IsValidTarget(_e.Range))
                    foreach (var eenemies in enemies)
                    {
                        var useE = ComboMenu["useEcombo"
                                             + eenemies.ChampionName].Cast<CheckBox>().CurrentValue;
                        if (useE)
                        {
                            foreach (
                                var enemy in
                                    from enemy in
                                        ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(550f))
                                    let prediction = _e2.GetPrediction(enemy)
                                    where NavMesh.GetCollisionFlags(
                                        prediction.UnitPosition.To2D()
                                            .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                                -distance)
                                            .To3D())
                                        .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                            prediction.UnitPosition.To2D()
                                                .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                                    -(distance/2))
                                                .To3D())
                                            .HasFlag(CollisionFlags.Wall)
                                    select enemy)
                            {
                                _e.Cast(enemy);
                            }
                        }
                    }
            }
        }

        public static void ComboR()
        {
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var comboR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var targetR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            if (comboR && _Player.CountEnemiesInRange(_Player.AttackRange + 350) >= rCount && _r.IsReady()
                && targetR != null)
            {
                _r.Cast();
            }
        }

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 0)
                {
                    Player.CastSpell(SpellSlot.Q, Side(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 1)
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 2)
                    if (_Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                    {
                        Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                        Orbwalker.ResetAutoAttack();
                    }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 3)
                {
                    Player.CastSpell(SpellSlot.Q, DefQ(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 4)
                {
                    Player.CastSpell(SpellSlot.Q, AggroQ(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (_q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 5)
                {
                    Player.CastSpell(SpellSlot.Q, AggroQ(_Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
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

        public static
            Vector2 DefQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/-50.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4;
            result = Vector2.Add(result, point1);
            return result;
        }

        public static
            Vector2 AggroQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/300;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/50;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/50;
            result = Vector2.Add(result, point1);
            return result;
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    new[] {0.3, 0.35, 0.4, 0.45, 0.5}[
                        _Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1]*
                _Player.TotalAttackDamage);
        }


        public static bool UltActive()
        {
            return _Player.HasBuff("vaynetumblefade") && !UnderEnemyTower((Vector2) _Player.Position);
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 950);
        }

        /*   public static
            void Harass()
        {
            var targetE = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var Emana = HarassMenu["useEHarassMana"].Cast<Slider>().CurrentValue;
            var Qmana = HarassMenu["useQHarassMana"].Cast<Slider>().CurrentValue;
            
       }*/
    }
}