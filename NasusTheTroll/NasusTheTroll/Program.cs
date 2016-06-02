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

namespace NasusTheTroll
{
    internal class NasusTheTroll
    {
        public static Spell.Active Q;
        public static Spell.Targeted W;
        public static Spell.Skillshot E;
        public static Spell.Active R;
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

        public static string[] MinionNames =
        {
            "SRU_Red",
            "SRU_Blue",
            "SRU_Dragon",
            "SRU_Baron",
            "SRU_Gromp",
            "SRU_Murkwolf",
            "SRU_Razorbeak",
            "SRU_Krug",
            "Sru_Crab"
        };


        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu;


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
            if (Player.Instance.Hero != Champion.Nasus)
            {
                return;
            }

            Q = new Spell.Active(SpellSlot.Q, 150);
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Skillshot(SpellSlot.E, 650, SkillShotType.Circular, 500, 20, 380)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R);


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

            Chat.Print("Nasus The Troll Loaded! Version 1", Color.Gold);
            Chat.Print("Have Fun And Dont Feed Kappa!", Color.Gold);

            Menu = MainMenu.AddMenu("Nasus The Troll", "NasusTheTroll");
            Menu.AddGroupLabel("Nasus The Troll Version 1");
            Menu.AddLabel("Last Update 2/6/2016");
            Menu.AddLabel("Made By MeLoDaG");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("useQCombo", new CheckBox("Use Q"));
            ComboMenu.Add("useWCombo", new CheckBox("Use W"));
            ComboMenu.AddLabel("E settings");
            ComboMenu.Add("useECombo", new CheckBox("Use E"));
            ComboMenu.AddLabel("For Max Range Set Slider pred 10% - 30%");
            ComboMenu.Add("Prediction", new Slider("Use E Pred %", 30, 0, 100));
            ComboMenu.AddLabel("Ulty settings");
            ComboMenu.Add("useRcombo", new CheckBox("Use R"));
            ComboMenu.Add("combo.Hp", new Slider("Use Ulty If Hp", 30, 0, 100));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            HarassMenu.Add("useWHarassMana", new Slider("Min. Mana for Harass %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Jungle Clear Settings", "FarmSettings");
            JungleLaneMenu.AddGroupLabel("Lane Clear");
            JungleLaneMenu.AddLabel("Q stuck work for laneclear lasthit mode!");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q Stuck"));
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
            MiscMenu.AddGroupLabel("Ks Settings");
            MiscMenu.Add("UseQks", new CheckBox("Use Q ks"));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));


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
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static
            void Drawing_OnDraw(EventArgs args)
        {
            if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Gold, Radius = Q.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Gold, Radius = W.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Gold, Radius = E.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }
            if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                new Circle {Color = Color.Gold, Radius = R.Range, BorderWidth = 2f}.Draw(_Player.Position);
            }

            DamageIndicator.HealthbarEnabled = DrawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = DrawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }


        public static void AUtoheal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
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
            if (Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
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
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
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

        public static void Ks()
        {
            var useQks = MiscMenu["UseQks"].Cast<CheckBox>().CurrentValue;
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= 200 && e.IsValidTarget() && !e.IsInvulnerable))
            {
                if (Q.IsReady() && useQks && QDamage(enemy) >= enemy.Health)
                    {
                        Q.Cast();
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
                        .FirstOrDefault(
                            a =>
                                a.Distance(_Player) <= Q.Range && a.Health <= QDamage(a) &&
                                a.IsValidTarget());

                if (useQ && _Player.ManaPercent > junglemana && Q.IsReady() && junleminions.IsValidTarget(Q.Range))
                {
                    Q.Cast();
                }
            }
        }

        public static void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var lanemana = JungleLaneMenu["useWManalane"].Cast<Slider>().CurrentValue;


            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position,
                    Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= 200 && m.Health <= QDamage(m) &&
                        m.IsValidTarget());

            if (Q.IsReady() && useQ && _Player.ManaPercent >= lanemana && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast();
            }
        }


        public static
            void Harass()
        {
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var eharass = HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (!target.IsValidTarget())
            {
                return;
            }
            if (E.IsReady() && eharass && target.IsValidTarget(850) && _Player.ManaPercent >= wmana)
            {
                var prede = (E.GetPrediction(target));
                if (prede.HitChancePercent >= 10)
                {
                    E.Cast(prede.CastPosition);
                }
            }
        }

        public static void Combo()
        {
            var useQ = ComboMenu["useQcombo"].Cast<CheckBox>().CurrentValue;
            var useW = ComboMenu["useWcombo"].Cast<CheckBox>().CurrentValue;
            var useE = ComboMenu["useEcombo"].Cast<CheckBox>().CurrentValue;
            var prediction = ComboMenu["Prediction"].Cast<Slider>().CurrentValue;
            var useR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var hp = ComboMenu["combo.Hp"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(1400, DamageType.Physical);
            if (!target.IsValidTarget(E.Range) || target == null)
            {
                return;
            }
            if (useQ && Q.IsReady() && target.IsValidTarget(250))
            {
                Q.Cast();
            }
            if (useW && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast(target);
            }
            if (useE && E.IsReady() && target.IsValidTarget(850))
            {
                var predE = (E.GetPrediction(target));
                if (predE.HitChancePercent >= prediction)
                {
                    E.Cast(predE.CastPosition);
                }
            }
            if (useR && HealthPercent <= hp && R.IsReady() &&
                target.IsValidTarget(R.Range))
            {
                R.Cast();
            }
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
                damage = QDamage(hero);

            return damage;
        }

        //Gredits KarmaPanda
        private static float QDamage(Obj_AI_Base target)
        {
            return
                Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical,
                    (new float[] {0, 30, 50, 70, 90, 110}[Q.Level] + Player.Instance.FlatPhysicalDamageMod +
                     Player.Instance.GetBuffCount("NasusQStacks"))) +
                Player.Instance.GetAutoAttackDamage(target);
        }

        #endregion
    }
}