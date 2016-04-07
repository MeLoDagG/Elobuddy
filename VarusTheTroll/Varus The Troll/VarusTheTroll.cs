using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;

namespace VarusTheTroll
{
    internal class VarusTheTroll
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static Spell.Chargeable _q;
        private static Spell.Active _w;
        private static Spell.Skillshot _e;
        private static Spell.Skillshot _r;
        public static Spell.Active Heal;
        public static SpellSlot Ignite { get; private set; }

        public static float HealthPercent
        {
            get { return _Player.Health/_Player.MaxHealth*100; }
        }

        public static bool isCharging = false;
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


        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            PrediMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu;





        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Varus)
            {
                return;
            }

            _q = new Spell.Chargeable(SpellSlot.Q, 925, 1600, 1250, 0, 1500, 70)
            {
                AllowedCollisionCount = int.MaxValue
            };
            _w = new Spell.Active(SpellSlot.W);
            _e = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 250, 1750, 250)
            {
                AllowedCollisionCount = int.MaxValue
            };
            _r = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 250, 1200, 120)
            {
                AllowedCollisionCount = int.MaxValue
            };

            var slot = _Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);

            Chat.Print(
                "<font color=\"#580dd9\" >MeLoDag Presents </font><font color=\"#ffffff\" > VarusTheTroll </font><font color=\"#580dd9\" >Kappa Kippo</font>");


            Menu = MainMenu.AddMenu("VarusTheTroll", "VarusTheTroll");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.Add("useQComboAlways", new CheckBox("Use Q[Always]"));
            ComboMenu.Add("useQCombo", new CheckBox("Use Q[StackCount]"));
            ComboMenu.Add("StackCount", new Slider("Q when stacks >= ", 3, 1, 3));
            ComboMenu.Add("useECombo", new CheckBox("Use E"));
            ComboMenu.Add("useWComboFocus", new CheckBox("Focus Target W"));
            ComboMenu.Add("useRCombo", new CheckBox("Use R"));
            ComboMenu.Add("Rcount", new Slider("R when enemies >= ", 1, 1, 5));
            ComboMenu.AddSeparator();
            ComboMenu.Add("useRComboFinisher", new CheckBox("Use R [FinisherMode]"));
            ComboMenu.Add("ForceR",
                new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            ComboMenu.Add("combo.ignite", new CheckBox("Use Ignite If Combo Killable"));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            HarassMenu.Add("useEHarassMana", new Slider("E Mana > %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useEFarm", new CheckBox("Use E"));
            JungleLaneMenu.AddSeparator();
            //     JungleLaneMenu.AddLabel("Jungle Clear");
            //     JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            //    JungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));
            //    JungleLaneMenu.Add("useWMana", new Slider("E Mana > %", 70, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto E for Gapcloser"));
            MiscMenu.Add("interrupter", new CheckBox("Auto R for Interrupter"));
            MiscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
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
            ItemMenu.AddGroupLabel("Qqs Utly");
            ItemMenu.Add("ZedUlt", new CheckBox("Zed R", true));
            ItemMenu.Add("VladUlt", new CheckBox("Vladimir R", true));
            ItemMenu.Add("FizzUlt", new CheckBox("Fizz R", true));
            ItemMenu.Add("MordUlt", new CheckBox("Mordekaiser R", true));
            ItemMenu.Add("PoppyUlt", new CheckBox("Poppy R", true));
            ItemMenu.Add("QssUltDelay", new Slider("Use QSS Delay(ms) for Ult", 250, 0, 1000));

            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 5));

            PrediMenu = Menu.AddSubMenu("Prediction Settings", "_PrediMenuettings");
            var style = PrediMenu.Add("style", new Slider("Min Prediction", 1, 0, 2));
            style.OnValueChange += delegate
            {
                style.DisplayName = "Min Prediction: " + new[] {"Low", "Medium", "High"}[style.CurrentValue];
            };
            style.DisplayName = "Min Prediction: " + new[] {"Low", "Medium", "High"}[style.CurrentValue];

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawRange", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));

            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;

        }



        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.DangerLevel == EloBuddy.SDK.Enumerations.DangerLevel.High && sender.IsValidTarget(850))
            {
                _r.Cast(sender);
            }
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                ItemUsage();
                comboR();
                UseIgnite();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }


            Ks();
            Auto();
            UseRTarget();
            AutoPot();
        }

        private static
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

        //Gredit GuTenTak
        public static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical); // 550 = Botrk.Range


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

        private static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
        {
            if (!sender.IsMe) return;
            var type = args.Buff.Type;
            var duration = args.Buff.EndTime - Game.Time;
            var Name = args.Buff.Name.ToLower();

            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Fear && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ItemMenu["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ItemMenu["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ItemMenu["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ItemMenu["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ItemMenu["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
            if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 1 &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (type == BuffType.Taunt && ItemMenu["Taunt"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Stun && ItemMenu["Stun"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Snare && ItemMenu["Snare"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Polymorph && ItemMenu["Polymorph"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Blind && ItemMenu["Blind"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Fear && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Charm && ItemMenu["Charm"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Suppression && ItemMenu["Suppression"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (type == BuffType.Silence && ItemMenu["Silence"].Cast<CheckBox>().CurrentValue)
                {
                    DoQSS();
                }
                if (Name == "zedrdeathmark" && ItemMenu["ZedUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "vladimirhemoplague" && ItemMenu["VladUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "fizzmarinerdoom" && ItemMenu["FizzUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "mordekaiserchildrenofthegrave" && ItemMenu["MordUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
                if (Name == "poppydiplomaticimmunity" && ItemMenu["PoppyUlt"].Cast<CheckBox>().CurrentValue)
                {
                    UltQSS();
                }
            }
        }

        private static void DoQSS()
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

        private static void UltQSS()
        {
            if (ItemMenu["useQSS"].Cast<CheckBox>().CurrentValue && Qss.IsOwned() && Qss.IsReady())
            {
                Core.DelayAction(() => Qss.Cast(), ItemMenu["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
            if (Simitar.IsOwned() && Simitar.IsReady())
            {
                Core.DelayAction(() => Simitar.Cast(), ItemMenu["QssUltDelay"].Cast<Slider>().CurrentValue);
            }
        }

        private static void Combo()
        {

            var wTarget =
                EntityManager.Heroes.Enemies.Find(
                    x => x.HasBuff("varuswdebuff") && x.IsValidTarget(_Player.CastRange));
            var target = TargetSelector.GetTarget(_q.MaximumRange, DamageType.Physical);


            if (target == null || !target.IsValidTarget())
            {
                return;
            }
            if (wTarget != null && ComboMenu["useWComboFocus"].Cast<CheckBox>().CurrentValue) 

                  {
               TargetSelector.GetTarget(_w.Range, DamageType.Magical);
               }
        
            var stackCount = ComboMenu["StackCount"].Cast<Slider>().CurrentValue;
            var comboQ = ComboMenu["useQcombo"].Cast<CheckBox>().CurrentValue;
            var comboQalways = ComboMenu["useQComboAlways"].Cast<CheckBox>().CurrentValue;
            var comboE = ComboMenu["useEcombo"].Cast<CheckBox>().CurrentValue;
            



            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
            }

            if (comboE && _e.IsReady())
            {
                _e.Cast(target);
            }

            if (comboQalways && _q.IsReady() && target != null)

            {
                if (_q.IsCharging)
                {
                    _q.Cast(target);
                    return;
                }
                else
                {
                    _q.StartCharging();
                    return;
                }
            }
            var qpred = _q.GetPrediction(target);
            if (comboQ)
            {
                if (target.GetBuffCount("varuswdebuff") >= stackCount)
                {
                    if (_q.IsCharging && qpred.HitChance >= HitChance.Medium)
                    {
                        _q.Cast(qpred.CastPosition);
                    }
                    else
                    {
                        _q.StartCharging();
                    }
                }
            }
        }
    

    public static
            void UseIgnite()
         {
            var useIgnite = ComboMenu["combo.ignite"].Cast<CheckBox>().CurrentValue;
            var targetIgnite =TargetSelector.GetTarget(_Player.AttackRange, DamageType.Physical); 

            if (useIgnite && targetIgnite != null)
            {
                if (_Player.Distance(targetIgnite) <= 600 && QDamage(targetIgnite) >= targetIgnite.Health)
                    _Player.Spellbook.CastSpell(Ignite, targetIgnite);
            }
        }



        public static void comboR()
        {
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var comboR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var TargetR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            if (comboR && _Player.CountEnemiesInRange(_r.Range) >= rCount && _r.IsReady()
                && TargetR != null && _r.GetPrediction(TargetR).HitChance >= HitChance.Medium) 
            {
                _r.Cast(_r.GetPrediction(TargetR).CastPosition);
            }
        }



        public static
          void UseRTarget()
        {
            var target = TargetSelector.GetTarget(_r.Range, DamageType.Magical);
            if (target != null &&
                (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && _r.IsReady() && target.IsValid &&
                 !Player.HasBuff("VarusR"))) _r.Cast(target.Position);
        }


        private static void Game_OnTick(EventArgs args)
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
            var eonCc = MiscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            if (eonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < _q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        if (_q.IsCharging)
                        {
                            _q.Cast(enemy);
                            return;
                        }
                        else
                        {
                            _q.StartCharging();
                            return;
                        }
                    }
                }
            }
        }

        public static void JungleClear()
        {
        }

        public static
            void WaveClear()
        {
            var useW = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useE = JungleLaneMenu["useEFarm"].Cast<CheckBox>().CurrentValue;


            if (Orbwalker.IsAutoAttacking) return;

            if (useW)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t =>
                            t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                            t.IsInRange(Player.Instance.Position, _q.Range));
                foreach (var m in minions)
                {
                    if (
                        _q.GetPrediction(m)
                            .CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable)
                            .Count() >= 0)
                    {
                        _q.Cast(m);
                        break;
                    }
                }
            }
        }



        public static
            void Harass()
        {
            var targetE = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(_q.MaximumRange, DamageType.Physical);
            var Emana = HarassMenu["useEHarassMana"].Cast<Slider>().CurrentValue;

            Orbwalker.ForcedTarget = null;

            if (Orbwalker.IsAutoAttacking) return;

            if (targetE != null)
            {

                if (HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue && _e.IsReady() &&
                    targetE.Distance(_Player) > _Player.AttackRange &&
                    targetE.IsValidTarget(_e.Range) && Player.Instance.ManaPercent > Emana)
                {
                    _e.Cast(targetE);
                }
            }

            if (targetQ != null)
            {
                if (HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue)
                {
                    if (targetQ.Distance(_Player) <= Player.Instance.AttackRange)
                    {
                        if (_q.IsCharging)
                        {
                            _q.Cast(targetQ);
                            return;
                        }
                        else
                        {
                            _q.StartCharging();
                            return;
                        }
                    }
                }
            }
        }

        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) < 200)
            {
                _e.Cast(e.End);
            }

        }

        public static void Ks()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= _q.Range && e.IsValidTarget() && !e.IsInvulnerable))
              { 
              if (ComboMenu["useRComboFinisher"].Cast<CheckBox>().CurrentValue && _r.IsReady() &&
                    RDamage(enemy) >= enemy.Health)
                {
                    _r.Cast(enemy);
                }

            }
        }

        public static float QDamage(Obj_AI_Base target)
        {
            if (!Player.GetSpell(SpellSlot.Q).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) new double[] {70, 125, 180, 23}[_r.Level - 1] + 1*Player.Instance.FlatPhysicalDamageMod);
        }

        public static float RDamage(Obj_AI_Base target)
        {

            if (!Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)new double[] { 100 , 175 , 250 }[_r.Level - 1] + 1 * Player.Instance.FlatMagicDamageMod);

        }

   
        private static void Drawing_OnDraw(EventArgs args)
        {


            {
                if (DrawMenu["drawRange"].Cast<CheckBox>().CurrentValue)
                {
                    if (_q.IsReady()) new Circle { Color = Color.Purple, Radius = _q.Range }.Draw(_Player.Position);
                    else if (_q.IsOnCooldown)
                        new Circle { Color = Color.Gray, Radius = _q.Range }.Draw(_Player.Position);
                }

                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    if (_e.IsReady()) new Circle { Color = Color.Purple, Radius = _e.Range }.Draw(_Player.Position);
                    else if (_w.IsOnCooldown)
                        new Circle { Color = Color.Gray, Radius = _e.Range }.Draw(_Player.Position);
                }

                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (_r.IsReady()) new Circle { Color = Color.Purple, Radius = _r.Range }.Draw(_Player.Position);
                    else if (_r.IsOnCooldown)
                        new Circle { Color = Color.Gray, Radius = _r.Range }.Draw(_Player.Position);
                }
            }
        }
    }
}






