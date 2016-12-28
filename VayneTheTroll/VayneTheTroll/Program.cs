using System;
using System.Collections.Generic;
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
    internal class Vayne
    {
        public static Spell.Ranged Q;
        public static Spell.Targeted E;
        public static Spell.Skillshot E2;
        public static Spell.Active W;
        public static Spell.Active R;
        public static Spell.Active Heal;
        public static AIHeroClient Target;

        public static readonly string[] MobNames =
        {
            "SRU_Red", "SRU_Blue", "SRU_Gromp", "SRU_Murkwolf",
            "SRU_Razorbeak", "SRU_Krug"
        };

        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);

        public static List<Vector2> Points = new List<Vector2>();

        public static bool UltActive()
        {
            return Player.HasBuff("vaynetumblefade") && !UnderEnemyTower((Vector2) Player.Position);
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 1100);
        }

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

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (EloBuddy.Player.Instance.Hero != Champion.Vayne)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, int.MaxValue, SkillShotType.Linear);
            E = new Spell.Targeted(SpellSlot.E, 590);
            E2 = new Spell.Skillshot(SpellSlot.E, 590, SkillShotType.Linear, 250, 1250);
            VayneTheTroll.Condemn.ESpell = new Spell.Skillshot(SpellSlot.E, 590, SkillShotType.Linear, 250, 1250);
            R = new Spell.Active(SpellSlot.R);


            var slot = Player.GetSpellSlotFromName("summonerheal");
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
                "<font color=\"#ef0101\" >MeLoSenpai Presents </font><font color=\"#ffffff\" > VayneTHeTroll </font><font color=\"#ef0101\" >Kappa Kippo</font>");
            Chat.Print("Version 1.5 (28/12/2016)", Color.GreenYellow);
            Chat.Print("Gl and HF also Dont Feed!!", Color.GreenYellow);


            Menu = MainMenu.AddMenu("VayneTheTroll", "VayneTheTroll");
            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Q Settings");
            ComboMenu.Add("useQcombo",
                new ComboBox("Q Logic", 0, "Side", "Cursor", "SmartQ", "SafeQ", "AggroQ", "Burst"));
            ComboMenu.Add("UseQulty", new CheckBox("Auto Q when using R", false));
            ComboMenu.AddLabel("W Settings:");
            ComboMenu.Add("FocusW", new CheckBox("Focus target With Silver Bolt"));
            ComboMenu.AddLabel("E Settings:");
            ComboMenu.Add("UseEks", new CheckBox("Use E Ks"));
            ComboMenu.Add("useEcombo", new ComboBox("E Logic", 0, "VayneTheTroll", "360 Fluxy"));
            ComboMenu.Add("pushDistance", new Slider("Push Distance", 410, 350, 420));
            ComboMenu.Add("condemnPercent", new Slider("HitChange %", 70));
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
            JungleLaneMenu.Add("useQMana", new Slider("Q Mana > %", 75));
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));
            JungleLaneMenu.Add("useQJunglemana", new Slider("Mana > %", 40));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.AddGroupLabel("Gapcloser Settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto Q for Gapcloser", false));
            MiscMenu.AddLabel("Interrupter Settings:");
            MiscMenu.Add("interrupter", new CheckBox("Enable Interrupter Using E"));
            MiscMenu.Add("interrupt.value", new ComboBox("Interrupter DangerLevel", 0, "High", "Medium", "Low"));
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
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health < ", 60));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health < ", 60));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddSeparator();
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));
            ItemMenu.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
            ItemMenu.Add("Stun", new CheckBox("Stun"));
            ItemMenu.Add("Blind", new CheckBox("Blind"));
            ItemMenu.Add("Charm", new CheckBox("Charm"));
            ItemMenu.Add("Suppression", new CheckBox("Suppression"));
            ItemMenu.Add("Polymorph", new CheckBox("Polymorph"));
            ItemMenu.Add("Fear", new CheckBox("Fear"));
            ItemMenu.Add("Taunt", new CheckBox("Taunt"));
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
            //  Orbwalker.OnPostAttack += Orbwalking_AfterAttack1;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interupthighlvl;
            Interrupter.OnInterruptableSpell += Interuptmediumlvl;
            Interrupter.OnInterruptableSpell += Interuptlowlvl;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
            EloBuddy.Player.OnIssueOrder += Player_OnIssueOrder;
        }


        public static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawStun"].Cast<CheckBox>().CurrentValue)
            {
                var t = TargetSelector.GetTarget(E.Range + Q.Range,
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
                    new Circle {Color = Color.Red, Radius = E.Range, BorderWidth = 2f}.Draw(Player.Position);
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

        public static void Interupthighlvl(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue)
            {
                if (MiscMenu["interrupt.value"].Cast<ComboBox>().CurrentValue == 0)
                {
                    if (interruptableSpellEventArgs.DangerLevel == DangerLevel.High && E.IsReady() && sender.IsValidTarget(E.Range))
                     {
                        E.Cast(sender);
                        Chat.Print("use Condemn For Interrupter spell", Color.Chartreuse);
                    }
                }
            }
        }

        public static void Interuptmediumlvl(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue)
            {
                if (MiscMenu["interrupt.value"].Cast<ComboBox>().CurrentValue == 1)
                {
                    if (interruptableSpellEventArgs.DangerLevel == DangerLevel.Medium && E.IsReady() && sender.IsValidTarget(E.Range))
                    {
                        E.Cast(sender);
                        Chat.Print("use Condemn For Interrupter spell", Color.Chartreuse);
                    }
                }
            }
        }

        public static void Interuptlowlvl(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs interruptableSpellEventArgs)
        {
            if (!sender.IsEnemy) return;

            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue)
            {
                if (MiscMenu["interrupt.value"].Cast<ComboBox>().CurrentValue == 2)
                {
                    if (interruptableSpellEventArgs.DangerLevel == DangerLevel.Low && E.IsReady() && sender.IsValidTarget(E.Range))
                    {
                        E.Cast(sender);
                        Chat.Print("use Condemn For Interrupter spell", Color.Chartreuse);
                    }
                }
            }
        }


        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var useQgap = MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue;

            if (useQgap && sender.IsEnemy &&
                e.End.Distance(Player) <= 350)
            {
                EloBuddy.Player.CastSpell(SpellSlot.Q,
                    e.End.Extend(EloBuddy.Player.Instance.Position, e.End.Distance(EloBuddy.Player.Instance) - 325)
                        .To3D());
            }
        }

        public static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.R)
                    if (ComboMenu["UseQulty"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                    {
                        EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
            }
        }

        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && ComboMenu["noaa"].Cast<CheckBox>().CurrentValue
                && (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                &&
                (Player.CountEnemiesInRange(1000f) >=
                 ComboMenu["Noaaslider"].Cast<Slider>().CurrentValue)
                && UltActive() || Player.HasBuffOfType(BuffType.Invisibility)
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
                EloBuddy.Player.SetSkinId(SkinId());
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
                Burst();
                Condemn();
                Condemn360();
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
            KillSteal();
        }

        public static
            void AutoPot()
        {
            if (AutoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !EloBuddy.Player.Instance.IsInShopRange() &&
                EloBuddy.Player.Instance.HealthPercent <= AutoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
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
            if (EloBuddy.Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(EloBuddy.Player.Instance.HasBuff("RegenerationPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  EloBuddy.Player.Instance.HasBuff("ItemCrystalFlask") ||
                  EloBuddy.Player.Instance.HasBuff("ItemDarkCrystalFlask")))
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
                Player.HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
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
                    EloBuddy.Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
                    target.HealthPercent < ItemMenu["useBotrkEnemyHP"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(Cutlass.Id, target);
                }
                if (ItemMenu["useBOTRK"].Cast<CheckBox>().CurrentValue && Item.HasItem(Botrk.Id) &&
                    Item.CanUseItem(Botrk.Id) &&
                    EloBuddy.Player.Instance.HealthPercent < ItemMenu["useBotrkMyHP"].Cast<Slider>().CurrentValue &&
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

        public static void KillSteal()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= E.Range && e.IsValidTarget(1000) && !e.IsInvulnerable))
            {
                if (ComboMenu["UseEks"].Cast<CheckBox>().CurrentValue && E.IsReady() &&
                    Player.GetSpellDamage(enemy, SpellSlot.E) >= enemy.Health)
                {
                    E.Cast(enemy);
                }
            }
        }

        public static
            void Flee()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var fleeQ = FleeMenu["fleeQ"].Cast<CheckBox>().CurrentValue;
            var fleeE = FleeMenu["fleeE"].Cast<CheckBox>().CurrentValue;

            if (fleeE && E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
            if (fleeQ && Q.IsReady())
            {
                EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
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
                        x => x.IsValid && !x.IsDead && x.Position.Distance(Player) < Player.GetAutoAttackRange(x)))
            {
                if (useQJungle && Q.IsReady() && mob != null && mob.IsValidTarget(Q.Range) &&
                    Player.ManaPercent >= usemana)
                {
                    EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                }
                if (useEJungle && E.IsReady() && mob != null && mob.IsValidTarget(E.Range) &&
                    MobNames.Contains(mob.BaseSkinName) &&
                    Player.ManaPercent >= usemana)
                {
                    E.Cast(mob);
                }
            }
        }

        public static
            void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useQMana = JungleLaneMenu["useQMana"].Cast<Slider>().CurrentValue;

            if (Q.IsReady() && useQ && Player.ManaPercent >= useQMana)
            {
                var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    Player.Position, Player.GetAutoAttackRange());
                foreach (var minions in
                    Minions.Where(
                        minions => minions.Health < DamageLibrary.QDamage(minions)))
                {
                    if (minions != null)
                    {
                        EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                }
            }
        }

        public static void FocusW()
        {
            var focusW = ComboMenu["FocusW"].Cast<CheckBox>().CurrentValue;
            var focusWtarget =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    h =>
                        h.ServerPosition.Distance(Player.ServerPosition) < 600 &&
                        h.GetBuffCount("vaynesilvereddebuff") == 1);
            if (focusW && focusWtarget.IsValidTarget())
            {
                Orbwalker.ForcedTarget = focusWtarget;
            }
        }

        public static void Condemn()
        {
            var distance = ComboMenu["pushDistance"].Cast<Slider>().CurrentValue;

            if (ComboMenu["useEcombo"].Cast<ComboBox>().CurrentValue == 0 && E.IsReady())
                foreach (
                    var enemy in
                        from enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(550f))
                        let prediction = E2.GetPrediction(enemy)
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
                    E.Cast(enemy);
                }
        }


        public static void Condemn360()
        {
            var target = TargetSelector.GetTarget((int) Player.GetAutoAttackRange(), DamageType.Physical);
            {
                if (!target.IsValidTarget() || Orbwalker.IsAutoAttacking) return;

                if (E.IsReady() && target.IsValidTarget(E.Range) && target.IsCondemable() &&
                    ComboMenu["useEcombo"].Cast<ComboBox>().CurrentValue == 1)
                {
                    E.Cast(target);
                }
            }
        }

        public static void ComboR()
        {
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var comboR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (comboR && Player.CountEnemiesInRange(Player.AttackRange + 350) >= rCount && R.IsReady()
                && targetR != null)
            {
                R.Cast();
            }
        }

        public static void Burst()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 5 && target.IsValidTarget(600) &&
                target.GetBuffCount("vaynesilvereddebuff") == 2 && !UnderEnemyTower((Vector2) Player.Position))
            {
                EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                Orbwalker.ResetAutoAttack();
            }
        }

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 0 &&
                    !UnderEnemyTower((Vector2) Player.Position))
                {
                    EloBuddy.Player.CastSpell(SpellSlot.Q,
                        Qlogic.Side(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 1 &&
                    !UnderEnemyTower((Vector2) Player.Position))
                {
                    EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 2 &&
                    !UnderEnemyTower((Vector2) Player.Position))
                    if (Player.Position.Extend(Game.CursorPos, 700).CountEnemiesInRange(700) <= 1)
                    {
                        EloBuddy.Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                        Orbwalker.ResetAutoAttack();
                    }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 3 &&
                    !UnderEnemyTower((Vector2) Player.Position))
                {
                    EloBuddy.Player.CastSpell(SpellSlot.Q,
                        Qlogic.DefQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) && target.IsValid)
                if (Q.IsReady() && ComboMenu["useQcombo"].Cast<ComboBox>().CurrentValue == 4 &&
                    !UnderEnemyTower((Vector2) Player.Position))
                {
                    EloBuddy.Player.CastSpell(SpellSlot.Q,
                        Qlogic.AggroQ(Player.Position.To2D(), target.Position.To2D(), 65).To3D());
                    Orbwalker.ResetAutoAttack();
                }
        }
    }
}