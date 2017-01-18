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
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;

namespace Varus_The_Troll
{
    internal class VarusTheTroll
    {
        public static string Version = "Version 1.1 (18/1/2017)";
        public static Spell.Chargeable Q;
        public static Spell.Active W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Active Heal;
        private static Font Thm;
        public static bool IsCharging = false;
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

        private static readonly List<BuffType> DeBuffsList = new List<BuffType>
        {
            BuffType.Blind,
            BuffType.Charm,
            BuffType.Fear,
            BuffType.Knockback,
            BuffType.Knockup,
            BuffType.NearSight,
            BuffType.Poison,
            BuffType.Polymorph,
            BuffType.Silence,
            BuffType.Shred,
            BuffType.Sleep,
            BuffType.Slow,
            BuffType.Snare,
            BuffType.Stun,
            BuffType.Suppression,
            BuffType.Taunt
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

        public static AIHeroClient Player
        {
            get { return ObjectManager.Player; }
        }

        public static SpellSlot Ignite { get; private set; }

        public static float HealthPercent
        {
            get { return Player.Health/Player.MaxHealth*100; }
        }


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (EloBuddy.Player.Instance.Hero != Champion.Varus)
            {
                return;
            }

            Q = new Spell.Chargeable(SpellSlot.Q, 925, 1600, 1250, 0, 1500, 70)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 925, SkillShotType.Circular, 250, 1750, 250)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 250, 1200, 120)
            {
                AllowedCollisionCount = int.MaxValue
            };

            var slot = Player.GetSpellSlotFromName("summonerheal");
            if (slot != SpellSlot.Unknown)
            {
                Heal = new Spell.Active(slot, 600);
            }
            Thm = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 32,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });
            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);

            Chat.Print(
                "<font color=\"#580dd9\" >MeloSenpai Presents </font><font color=\"#ffffff\" > VarusTheTroll </font><font color=\"#580dd9\" >Kappa Kippo</font>");

            Chat.Print("Hf Gl enjoy!!",Color.Aqua
                );


            Menu = MainMenu.AddMenu("Varus The Troll", "VarusTheTroll");
            Menu.AddLabel(" Varus The Troll " + Version);
            Menu.AddLabel(" Made by MeloSenpai");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Q Settings");
            ComboMenu.Add("Qlogic", new ComboBox("Q Logic ", 0, "Always", "If 3 Stacks"));
            ComboMenu.AddLabel("E Settings");
            ComboMenu.Add("ELogic", new ComboBox("E Logic ", 0, "Normal", "After AA"));
            ComboMenu.AddLabel("R Settings:");
            ComboMenu.Add("useRCombo", new CheckBox("Use R", false));
            ComboMenu.Add("Rlogic", new ComboBox("Ulty Logic ", 0, "EnemyHp", "HitCountEnemys"));
            ComboMenu.Add("Hp", new Slider("Use R Enemy Health {0}(%)", 45, 0, 100));
            ComboMenu.Add("Rcount", new Slider("If Ulty Hit {0} Enemy ", 2, 1, 5));
            ComboMenu.Add("rpred", new Slider("Select Ulty {0}(%) Hitchance", 70, 0, 100));
            ComboMenu.AddLabel("Use R Range Settigs For all Logic:");
            ComboMenu.Add("useRRange", new Slider("Use Ulty Max Range", 1800, 500, 2000));
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Combo preferences:");
            ComboMenu.Add("useWComboFocus", new CheckBox("Focus Target W"));
            ComboMenu.Add("ForceR",
                new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            ComboMenu.Add("combo.ignite", new CheckBox("Use Ignite If Combo Killable"));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMenu.Add("useEHarass", new CheckBox("Use E"));
            HarassMenu.Add("useEHarassMana", new Slider("E Mana > %", 70));
            HarassMenu.Add("useQHarassMana", new Slider("Q Mana > %", 70));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useEFarm", new CheckBox("Use E"));
            JungleLaneMenu.Add("LaneMana", new Slider("Mana > %", 70));
            JungleLaneMenu.AddSeparator();
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useEJungle", new CheckBox("Use E"));
            JungleLaneMenu.Add("JungleMana", new Slider("E Mana > %", 70));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.AddGroupLabel("Gap Close/Interrupt Settings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto E for Gapcloser"));
            MiscMenu.Add("interrupter", new CheckBox("Auto R for Interrupter"));
            MiscMenu.AddLabel("Auto Skills On CC Enemy");
            MiscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
            MiscMenu.AddLabel("KillSteal Settings");
            MiscMenu.Add("UseRKs", new CheckBox("Use R Ks"));

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
            foreach (var debuff in DeBuffsList)
            {
                ItemMenu.Add(debuff.ToString(), new CheckBox(debuff.ToString()));
            }
            ItemMenu.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));


            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 5));

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawRange", new CheckBox("Draw Q Range"));
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw R Range"));
            DrawMenu.AddLabel("Damage indicators");
            DrawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            DrawMenu.Add("percent", new CheckBox("Damage percent info"));
            DrawMenu.Add("howaa", new CheckBox("How Many AA"));
            DrawMenu.Add("Rkill", new CheckBox("R kill "));


            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
            DamageIndicator.Initialize(ComboDamage);
        }


        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            if (MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.DangerLevel == DangerLevel.High && sender.IsValidTarget(650))
            {
                R.Cast(sender);
            }
        }

        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(Player) < 200)
            {
                E.Cast(e.End);
            }
        }

        public static
            void OnGameUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                ItemUsage();
                ComboR();
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

        public static void Game_OnTick(EventArgs args)
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
            if (EloBuddy.Player.Instance.ManaPercent <= AutoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
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

        public static
            void ItemUsage()
        {
            var target = TargetSelector.GetTarget(550, DamageType.Physical);


            if (ItemMenu["useYoumu"].Cast<CheckBox>().CurrentValue && Youmuu.IsOwned() && Youmuu.IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(1800) >= 1)
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
            {
                if (sender == null || args.Buff == null || !sender.IsMe)
                    return;

                var type = args.Buff.Type;

                if (!DeBuffsList.Contains(type))
                    return;

                if (!ItemMenu[type.ToString()].Cast<CheckBox>().CurrentValue)
                    return;

                if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 0)
                {
                    DoQss();
                    return;
                }

                if (ItemMenu["Qssmode"].Cast<ComboBox>().CurrentValue == 1 &&
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
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

        public static
            void UseIgnite()
        {
            var useIgnite = ComboMenu["combo.ignite"].Cast<CheckBox>().CurrentValue;
            var targetIgnite = TargetSelector.GetTarget(Player.AttackRange, DamageType.Physical);

            if (useIgnite && targetIgnite != null)
            {
                if (Player.Distance(targetIgnite) <= 600 && ComboDamage(targetIgnite) >= targetIgnite.Health)
                    Player.Spellbook.CastSpell(Ignite, targetIgnite);
            }
        }

        public static void Auto()
        {
            var eonCc = MiscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            if (eonCc)
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
                        if (Q.IsCharging)
                        {
                            Q.Cast(enemy);
                            return;
                        }
                        Q.StartCharging();
                        return;
                    }
                }
            }
        }

        public static void Ks()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) <= Q.Range && e.IsValidTarget() && !e.IsInvulnerable))
            {
                if (MiscMenu["UseRKs"].Cast<CheckBox>().CurrentValue && R.IsReady() &&
                    RDamage(enemy) >= enemy.Health)
                {
                    R.Cast(enemy.Position);
                }
            }
        }


        public static
            void WaveClear()
        {
            var useW = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useE = JungleLaneMenu["useEFarm"].Cast<CheckBox>().CurrentValue;
            var laneMana = JungleLaneMenu["LaneMana"].Cast<Slider>().CurrentValue;


            if (Orbwalker.IsAutoAttacking) return;

            if (useW && Player.ManaPercent > laneMana)
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        t =>
                            t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable &&
                            t.IsInRange(EloBuddy.Player.Instance.Position, Q.Range));
                foreach (var m in minions)
                {
                    if (
                        Q.GetPrediction(m)
                            .CollisionObjects.Where(t => t.IsEnemy && !t.IsDead && t.IsValid && !t.IsInvulnerable)
                            .Count() >= 0)
                    {
                        if (Q.IsCharging)
                        {
                            Q.Cast(m.Position);
                        }
                        else
                        {
                            Q.StartCharging();
                        }

                        if (useE && Player.ManaPercent > laneMana)
                        {
                            E.Cast(m.Position);
                        }
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useQJungle = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var useEJungle = JungleLaneMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var jungleMana = JungleLaneMenu["LaneMana"].Cast<Slider>().CurrentValue;
            var minion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.ServerPosition, 950f, true)
                    .FirstOrDefault();

            if (useQJungle && Player.ManaPercent > jungleMana && minion != null)
            {
                if (Q.IsCharging)
                {
                    Q.Cast(minion.Position);
                }
                else
                {
                    Q.StartCharging();
                }
                if (useEJungle && Player.ManaPercent > jungleMana && minion != null)
                {
                    E.Cast(minion.Position);
                }
            }
        }

        public static
            void Harass()
        {
            var targetE = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(1200, DamageType.Physical);
            var emana = HarassMenu["useEHarassMana"].Cast<Slider>().CurrentValue;
            var qmana = HarassMenu["useQHarassMana"].Cast<Slider>().CurrentValue;

            Orbwalker.ForcedTarget = null;

            if (Orbwalker.IsAutoAttacking) return;

            if (targetE != null)
            {
                if (HarassMenu["useEHarass"].Cast<CheckBox>().CurrentValue && E.IsReady() &&
                    targetE.Distance(Player) > Player.AttackRange &&
                    targetE.IsValidTarget(E.Range) && Player.ManaPercent > emana)
                {
                    E.Cast(targetE);
                }
            }

            if (targetQ != null)
            {
                if (HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                    targetQ.Distance(Player) > Player.AttackRange && targetQ.IsValidTarget(800)
                    && EloBuddy.Player.Instance.ManaPercent > qmana)
                {
                    if (Q.IsCharging)
                    {
                        Q.Cast(targetQ);
                    }
                    else
                    {
                        Q.StartCharging();
                    }
                }
            }
        }

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable ||
                        !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                        return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;
                if (ComboMenu["ELogic"].Cast<ComboBox>().CurrentValue == 1)
                {
                    if (E.IsReady())
                    {
                        E.Cast(enemy);
                    }
                }
            }
        }

        public static void Combo()

        {
            var wTarget =
                EntityManager.Heroes.Enemies.Find(
                    x => x.HasBuff("varuswdebuff") && x.IsValidTarget(Player.CastRange));
            var target = TargetSelector.GetTarget(Q.MaximumRange, DamageType.Physical);
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
            }
            if (target == null || !target.IsValidTarget())
            {
                return;
            }
            if (wTarget != null && ComboMenu["useWComboFocus"].Cast<CheckBox>().CurrentValue)
            {
                Orbwalker.ForcedTarget = wTarget;
            }
            if (ComboMenu["ELogic"].Cast<ComboBox>().CurrentValue == 0 && E.CanCast(target))
            {
                var prediction = E.GetPrediction(target);
                if (prediction.HitChance >= HitChance.Medium)
                {
                    E.Cast(target);
                }
            }
            if (ComboMenu["QLogic"].Cast<ComboBox>().CurrentValue == 0 && Q.CanCast(target))
            {
                var prediction = Q.GetPrediction(target);
                if (prediction.HitChance >= Q.MinimumHitChance)
                {
                    if (Q.IsCharging)
                    {
                        Q.Cast(prediction.CastPosition);
                        return;
                    }
                    Q.StartCharging();
                    return;
                }
            }
            if (ComboMenu["QLogic"].Cast<ComboBox>().CurrentValue == 1 && Q.CanCast(target))
            {
                if (target.GetBuffCount("varuswdebuff") == 3)
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChance >= Q.MinimumHitChance)
                    {
                        if (Q.IsCharging)
                        {
                            Q.Cast(prediction.CastPosition);
                            return;
                        }
                        Q.StartCharging();
                    }
                }
            }
        }

        public static
            void ComboR()
        {
            var distance = ComboMenu["useRRange"].Cast<Slider>().CurrentValue;
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var rpred = ComboMenu["rpred"].Cast<Slider>().CurrentValue;
            var useR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var hp = ComboMenu["Hp"].Cast<Slider>().CurrentValue;
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (targetR == null || !targetR.IsValidTarget()) return;
            if (ComboMenu["Rlogic"].Cast<ComboBox>().CurrentValue == 0 && useR)
            {
                if (R.IsReady() && targetR.Distance(Player) <= distance && targetR.HealthPercent <= hp &&
                    !targetR.IsUnderEnemyturret())
                {
                    var predR = R.GetPrediction(targetR);
                    if (predR.HitChancePercent >= rpred)
                    {
                        R.Cast(predR.CastPosition);
                    }
                }
            }
            if (ComboMenu["Rlogic"].Cast<ComboBox>().CurrentValue == 1 && useR && targetR.Distance(Player) <= distance &&
                !targetR.IsUnderEnemyturret())
            {
                {
                    R.CastIfItWillHit(rCount, rpred);
                }
            }
        }

        public static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target != null &&
                (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && R.IsReady() && target.IsValid &&
                 !EloBuddy.Player.HasBuff("VarusR"))) R.Cast(target.Position);
        }

        public static void Drawing_OnDraw(EventArgs args)
        {


            {
                if (DrawMenu["drawRange"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady()) new Circle {Color = Color.Purple, Radius = Q.Range}.Draw(Player.Position);
                    else if (Q.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = Q.Range}.Draw(Player.Position);
                }

                if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                {
                    if (E.IsReady()) new Circle {Color = Color.Purple, Radius = E.Range}.Draw(Player.Position);
                    else if (W.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = E.Range}.Draw(Player.Position);
                }

                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady()) new Circle {Color = Color.Purple, Radius = R.Range}.Draw(Player.Position);
                    else if (R.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = R.Range}.Draw(Player.Position);
                }
                DamageIndicator.HealthbarEnabled =
                    DrawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = DrawMenu["percent"].Cast<CheckBox>().CurrentValue;
            }
            if (DrawMenu["howaa"].Cast<CheckBox>().CurrentValue)
            {
                // double temp = 0;
                foreach (
                    var noob in
                        ObjectManager.Get<AIHeroClient>().Where(x => x.IsVisible && x.IsEnemy && x.IsValid))
                {
                    var dmg = Player.GetAutoAttackDamage(noob);

                    var howmanyaa = noob.Health/dmg;
                    if (howmanyaa >= 10)
                    {
                        Drawing.DrawText(noob.HPBarPosition.X, noob.HPBarPosition.Y - 44, Color.Yellow,
                            "" + "  How Many AA: " + string.Format("{0:0.00}", howmanyaa));
                    }
                    if (howmanyaa < 8)
                    {
                        Drawing.DrawText(noob.HPBarPosition.X, noob.HPBarPosition.Y - 44, Color.LawnGreen,
                            "" + "  How Many AA: " + string.Format("{0:0.00}", howmanyaa));
                    }
                }
            }
            foreach (
                var noob in
                    ObjectManager.Get<AIHeroClient>().Where(x => x.IsVisible && x.IsEnemy && x.IsValid))
                if (DrawMenu["Rkill"].Cast<CheckBox>().CurrentValue && R.IsReady())
                {
                    var ft = Drawing.WorldToScreen(noob.Position);
                    if (noob.IsValidTarget(R.Range) &&
                        Player.GetSpellDamage(noob, SpellSlot.R) > noob.Health + noob.AttackShield)
                    {
                        DrawFont(Thm, "Use R  Killable " + noob.ChampionName, ft[0] - 140,
                            ft[1] + 80, SharpDX.Color.LawnGreen);
                    }
                }
        }

        public static void DrawFont(Font vFont, string vText, float vPosX, float vPosY, ColorBGRA vColor)
        {
            vFont.DrawText(null, vText, (int) vPosX, (int) vPosY, vColor);
        }

        #region dmg calc

        public static float ComboDamage(AIHeroClient target)
        {
            float damage = 0;
            if (target != null)
            {
                if (Q.IsReady())
                {
                    damage += Player.GetSpellDamage(target, SpellSlot.Q);
                    damage += Player.GetAutoAttackDamage(target);
                }
                if (E.IsReady())
                {
                    damage += Player.GetSpellDamage(target, SpellSlot.E);
                    damage += Player.GetAutoAttackDamage(target);
                }
                if (W.IsReady())
                {
                    damage += Player.GetSpellDamage(target, SpellSlot.W);
                    damage += Player.GetAutoAttackDamage(target);
                }
                if (R.IsReady())
                {
                    damage += Player.GetSpellDamage(target, SpellSlot.R);
                    damage += Player.GetAutoAttackDamage(target);
                }
                if (ObjectManager.Player.CanAttack)
                    damage += ObjectManager.Player.GetAutoAttackDamage(target);
            }
            return damage;
        }

        public static double EDamage(Obj_AI_Base target)
        {
            return E.IsReady()
                ? Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new float[] {80, 120, 160, 200, 240}[E.Level - 1]
                    + .5f*Player.TotalMagicalDamage)
                : 0d;
        }

        public static double QDamage(Obj_AI_Base target)
        {
            return Q.IsReady()
                ? Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new float[] {70, 125, 180, 23}[Q.Level - 1]
                    + 1.2F*Player.TotalAttackDamage)
                : 0d;
        }

        public static float RDamage(Obj_AI_Base target)
        {
            if (!EloBuddy.Player.GetSpell(SpellSlot.R).IsLearned) return 0;
            return EloBuddy.Player.Instance.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) new double[] {100, 175, 250}[R.Level - 1] + 1*EloBuddy.Player.Instance.FlatMagicDamageMod);
        }

        #endregion dmg calc
    }
}