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

namespace PoppyTheTroll
{
    internal class PoppyTheTroll
    {
        public static Spell.Skillshot Q;
        public static Spell.Active W;
        public static Spell.Targeted E;
        public static Spell.Skillshot E2;
        public static Spell.Chargeable R;
        public static Spell.Active Heal;

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
            if (Player.Instance.Hero != Champion.Poppy)
            {
                return;
            }


            Q = new Spell.Skillshot(SpellSlot.Q, 430, SkillShotType.Linear, 250, null, 100);
            Q.AllowedCollisionCount = int.MaxValue;
            W = new Spell.Active(SpellSlot.W, 400);
            E = new Spell.Targeted(SpellSlot.E, 425);
            E2 = new Spell.Skillshot(SpellSlot.E, 525, SkillShotType.Linear, 250, 1250);
            R = new Spell.Chargeable(SpellSlot.R, 500, 1200, 4000, 250, int.MaxValue, 90);

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

            Chat.Print("Poppy The Troll Loaded! Version 1", Color.DeepSkyBlue);
            Chat.Print("Have Fun And Dont Feed Kappa!", Color.DeepSkyBlue);

            Menu = MainMenu.AddMenu("Poppy The Troll", "PoppyTheTroll");
            Menu.AddGroupLabel("Poppy The Troll Version 1");
            Menu.AddLabel("Last Update 31/5/2016");
            Menu.AddLabel("Made By MeLoDaG");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("useWCombo", new CheckBox("Use W"));
            ComboMenu.Add("useECombo", new CheckBox("Use E"));
            ComboMenu.Add("useRcombo", new CheckBox("Use R"));
            ComboMenu.Add("combo.REnemies", new Slider("Min Enemyes for R", 1, 1, 5));
            
            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useWHarassMana", new Slider("Min. Mana for Harass %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Jungle Clear Settings", "FarmSettings");
            JungleLaneMenu.AddGroupLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useWManalane", new Slider("Min. Mana for Laneclear Spells %", 70, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
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
            MiscMenu.AddGroupLabel("Gapcloser/Interrupt  settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto W for Gapcloser"));
            MiscMenu.Add("gapcloserE", new CheckBox("Auto E for Gapcloser"));
            MiscMenu.Add("InterruptE", new CheckBox("Auto E for Interrupt"));
            MiscMenu.Add("interruptR", new CheckBox("Auto R for Interrupt"));
            MiscMenu.AddGroupLabel("Ks Settings");
            MiscMenu.Add("UseQks", new CheckBox("Use Q ks"));
        
            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            FleeMenu = Menu.AddSubMenu("Flee Settings", "FleeSettings");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("FleeW", new CheckBox("Use W"));

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.AddGroupLabel("Draw Settings");
            DrawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw W Range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawMenu.AddLabel("Damage indicators");
            DrawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            DrawMenu.Add("percent", new CheckBox("Damage percent info"));

            DamageIndicator.Initialize(ComboDamage);
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static
            void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = Q.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = W.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = E.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.DeepSkyBlue, Radius = R.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }

            DamageIndicator.HealthbarEnabled = DrawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = DrawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }

        public static void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) <= 600)
            {
                W.Cast();
            }
            if (MiscMenu["gapcloserE"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) < E.Range)
            {
                E.Cast(e.Sender);
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useE = MiscMenu["interruptE"].Cast<CheckBox>().CurrentValue;
            var useR = MiscMenu["interruptR"].Cast<CheckBox>().CurrentValue;

            {
                if (useE)
                {
                    if (sender.IsEnemy && E.IsReady() && e.DangerLevel == DangerLevel.High &&
                        sender.Distance(_Player) <= E.Range)
                    {
                        E.Cast(sender);
                    }
                    else if (useR)
                    {
                        if (sender.IsEnemy && R.IsReady() && e.DangerLevel == DangerLevel.High &&
                            sender.Distance(_Player) <= R.Range)
                        {
                            if (R.IsCharging)
                            {
                                R.Cast(sender);
                                return;
                            }
                            R.StartCharging();
                        }
                    }
                }
            }
        }

        public static void AUtoheal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use HeaL Kappa kippo</font>");
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
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
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
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
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
                AutoPot();
            }
        }


        public static void Flee()
        {
            var fleeW = FleeMenu["FleeW"].Cast<CheckBox>().CurrentValue;
            if (fleeW && W.IsReady())
            {
                W.Cast();
            }
        }

        public static void Ks()
        {
            var useQks = MiscMenu["UseQks"].Cast<CheckBox>().CurrentValue;
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))
            {
                if (Q.IsReady() && useQks)
                {
                    if (_Player.GetSpellDamage(enemy, SpellSlot.Q) > enemy.Health)
                    {
                        var predQ = (Q.GetPrediction(enemy));
                        if (predQ.HitChance >= HitChance.High)
                        {
                            Q.Cast(predQ.CastPosition);
                        }
                    }
                }
            }
        }


        public static
            void JungleClear()
        {
            var useQ = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var junglemana = JungleLaneMenu["useWMana"].Cast<Slider>().CurrentValue;

            {
                var junleminions =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters()
                        .OrderByDescending(a => a.MaxHealth)
                        .FirstOrDefault(a => a.IsValidTarget(900));

                if (useQ && _Player.ManaPercent > junglemana && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
                {
                    Q.Cast(junleminions);
                }
            }
        }

        public static void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var lanemana = JungleLaneMenu["useWManalane"].Cast<Slider>().CurrentValue;


            var count =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.ServerPosition,
                    _Player.AttackRange, false).Count();
            var source =
                EntityManager.MinionsAndMonsters.GetLaneMinions()
                    .OrderBy(a => a.MaxHealth)
                    .FirstOrDefault(a => a.IsValidTarget(Q.Range));
            if (count == 0) return;
            if (useQ && _Player.ManaPercent > lanemana && Q.IsReady())
            {
                Q.Cast(source);
            }
        }

        public static
            void Harass()
        {
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var qharass = HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }

            if (Q.IsReady() && qharass && target.IsValidTarget(Q.Range) && _Player.ManaPercent >= wmana)
            {
                var predQ = (Q.GetPrediction(target));
                if (predQ.HitChance >= HitChance.High)
                {
                    Q.Cast(predQ.CastPosition);
                }
            }
        }
        
        public static void Combo()
        {
            var useQ = ComboMenu["useQcombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useWcombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["useEcombo"].Cast<CheckBox>().CurrentValue;
            var useR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var rEnemies = ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(1400, DamageType.Magical);
            if (!target.IsValidTarget(Q.Range) || target == null)
            {
                return;
            }
            if (useE && E.IsReady() && target.IsValidTarget(E.Range))
            {
                if (Wall(target))
                {
                    E.Cast(target);
                }
            }
            if (useQ && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                var predQ = (Q.GetPrediction(target));
                if (predQ.HitChance >= HitChance.High)
                {
                    Q.Cast(predQ.CastPosition);
                }
            }
            if (useW && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }
            if (useR && _Player.CountEnemiesInRange(R.Range) == rEnemies && R.IsReady() && target.IsValidTarget(R.Range))
            {
                var predR = (R.GetPrediction(target));
                if (predR.HitChance >= HitChance.High)
                {
                    if (R.IsCharging)
                    {
                        R.Cast(target.Position);
                        return;
                    }
                    R.StartCharging();
                }
            }
        }

        private static bool Wall(Obj_AI_Base target)
        {
            var distance = target.BoundingRadius + target.ServerPosition.Extend(Player.Instance.ServerPosition, -355);

            if (distance.IsWall())
            {
                return true;
            }

            return false;
        }

        #region dmg

        public static float ComboDamage(Obj_AI_Base hero)
        {
            var damage = _Player.GetAutoAttackDamage(hero);
            if (R.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.R);
            if (E.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.E);
            if (W.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.W);
            if (Q.IsReady())
                damage = _Player.GetSpellDamage(hero, SpellSlot.Q);

            return damage;
        }

        #endregion
    }
}