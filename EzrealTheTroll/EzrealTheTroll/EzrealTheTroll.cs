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

namespace Ezreal_The_Troll
{
    internal class EzrealTheTroll
    {
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Spell.Active Heal;

        private static Font Thm;
        private static Item HealthPotion;
        private static Item CorruptingPotion;
        private static Item RefillablePotion;
        private static Item TotalBiscuit;
        private static Item HuntersPotion;
        public static Item Youmuu = new Item(ItemId.Youmuus_Ghostblade);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);
        public static Item Cutlass = new Item(ItemId.Bilgewater_Cutlass);
        public static Item Tear = new Item(ItemId.Tear_of_the_Goddess);
        public static Item Manamune = new Item(ItemId.Manamune);
        public static Item Archangel = new Item(ItemId.Archangels_Staff);
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

        public static CheckBox ESafe, EWall, ECorrect, EPath, EKite, EGap, EGrass;
        public static Slider ERange;

        public static Menu Menu,
            ComboMenu,
            PredictionMenu,
            EMenu,
            HarassMenu,
            JungleLaneMenu,
            KillsecureMenu,
            DrawMenu,
            ItemMenu,
            SkinMenu,
            AutoPotHealMenu,
            FleEMenu;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        public static float HealthPercent
        {
            get { return _Player.Health/_Player.MaxHealth*100; }
        }

        public static bool UnderEnemyTower(Vector2 pos)
        {
            return EntityManager.Turrets.Enemies.Where(a => a.Health > 0 && !a.IsDead).Any(a => a.Distance(pos) < 950);
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Ezreal)
            {
                return;
            }


            LoadMenu();
            CorrectTheMenu();

            #region skills load

            Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 2000, 60);
            Q.AllowedCollisionCount = 0;
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 250, 1600, 80);
            W.AllowedCollisionCount = int.MaxValue;
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            E.AllowedCollisionCount = int.MaxValue;
            R = new Spell.Skillshot(SpellSlot.R, 3000, SkillShotType.Linear, 1000, 2000, 160);
            R.AllowedCollisionCount = int.MaxValue;

            #endregion skills load

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
            Teleport.OnTeleport += Teleport_OnTeleport;
            Thm = new Font(Drawing.Direct3DDevice,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 32,
                    Weight = FontWeight.Bold,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.ClearType
                });
            Chat.Print(
                "<font color=\"#ffff1a\" >MeLoSenpai Presents </font><font color=\"#ffffff\" > Ezreal The Troll </font>");
            Chat.Print("Version 1 Loaded 21/1/2017", Color.Yellow);
            Chat.Print("Gl And HF", Color.Aqua);


            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Game.OnTick += StackTear;
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Orbwalker.OnUnkillableMinion += On_Unkillable_Minion;
            DamageIndicator.Initialize(SpellDamage.GetRawDamage);
        }

        public static
            void LoadMenu
            ()
        {
            Menu = MainMenu.AddMenu("Ezrealthetroll", "Ezrealthetroll");
            Menu.AddGroupLabel("EzreaL the Troll!");
            Menu.AddGroupLabel("Made by MeLoSenpai!");
            Menu.AddGroupLabel("Version 1 Hf Gl and dont troll!");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo Settigns:");
            ComboMenu.Add("Qlogic", new ComboBox("Mystic Shot Logic ", 0, "Normal", "After AA"));
            ComboMenu.Add("Wlogic", new ComboBox("Essence Flux Logic ", 0, "Normal", "After AA"));
            ComboMenu.AddLabel("R Settings:");
            ComboMenu.Add("useRCombo", new CheckBox("Use Trueshot Barrage", false));
            ComboMenu.Add("Rlogic", new ComboBox("Trueshot Barrage", 0, "EnemyHp", "HitCountEnemys"));
            ComboMenu.Add("Hp", new Slider("Use Trueshot Barrage if Enemy Health {0}(%)", 45, 0, 100));
            ComboMenu.Add("Rcount", new Slider("If Trueshot Barrage Hit {0} Enemy ", 2, 1, 5));
            ComboMenu.AddLabel("Use Trueshot Barrage Range Settigs For all Logic:");
            ComboMenu.Add("useRRange", new Slider("Use Trueshot Barrage Max Range", 1800, 500, 2000));
            ComboMenu.AddGroupLabel("Combo preferences:");
            ComboMenu.Add("E", new CheckBox("Use Arcane Shift"));
            ComboMenu.Add("CCQ", new CheckBox("Auto Mystic Shot on Enemy CC"));
            ComboMenu.Add("CCW", new CheckBox("Auto Essence Flux on Enemy CC"));
            ComboMenu.Add("ForceR",
                new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "H".ToCharArray()[0]));

            EMenu = Menu.AddSubMenu("Arcane Shift");
            EMenu.AddGroupLabel("Arcane Shift Settings");
            ESafe = EMenu.Add("Esafe", new CheckBox("Safe Point Check E"));
            EWall = EMenu.Add("Ewall", new CheckBox("Check Wall"));
            ECorrect = EMenu.Add("Ecorrect", new CheckBox("Allow Correct to better"));
            ERange = EMenu.Add("Erange", new Slider("Range eliminate {0}", 35, 0, 75));
            EMenu.Add("label",
                new Label("The more value, the more easier for next a.a, the more dangerous, recommended 20~40"));
            EPath = EMenu.Add("Epath", new CheckBox("Anti Enemy Path"));
            EKite = EMenu.Add("Ekite", new CheckBox("Try to kite champ"));
            EGap = EMenu.Add("Etogap", new CheckBox("Use Arcane Shift to Gapclose taget"));
            EGrass = EMenu.Add("Egrass", new CheckBox("Try Arcane Shift to Grass"));
            EMenu.AddSeparator();
            EMenu.AddGroupLabel("Arcane Shift Style Settings");
            var EStyle = EMenu.Add("E",
                new ComboBox("Arcane Shift Logic", 1, "Don't use E", "Auto Adc Kappa", "CursorPos", "Cursor (Smart)"));
            EStyle.OnValueChange += delegate(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                switch (args.NewValue)
                {
                    case 1:
                    {
                        ESafe.IsVisible = true;
                        ESafe.CurrentValue = true;
                        EWall.IsVisible = true;
                        EWall.CurrentValue = true;
                        ECorrect.IsVisible = true;
                        ERange.IsVisible = true;
                        EMenu["label"].IsVisible = true;
                        EPath.CurrentValue = true;
                        EPath.IsVisible = true;
                        EKite.CurrentValue = true;
                        EKite.IsVisible = true;
                        EGap.IsVisible = true;
                        EGrass.IsVisible = false;
                    }
                        break;
                    case 2:
                    {
                        ESafe.IsVisible = false;
                        EWall.IsVisible = false;
                        ECorrect.IsVisible = false;
                        ERange.IsVisible = false;
                        EMenu["label"].IsVisible = false;
                        EPath.IsVisible = false;
                        EKite.IsVisible = false;
                        EGap.IsVisible = false;
                        EGrass.IsVisible = false;
                    }
                        break;
                    case 3:
                    {
                        ESafe.CurrentValue = true;
                        EWall.CurrentValue = false;
                        ECorrect.IsVisible = true;
                        ECorrect.CurrentValue = true;
                        ERange.IsVisible = false;
                        EMenu["label"].IsVisible = false;
                        EPath.IsVisible = false;
                        EKite.IsVisible = true;
                        EGap.IsVisible = true;
                        EGrass.IsVisible = true;
                    }
                        break;
                }
            };


            PredictionMenu = Menu.AddSubMenu("Prediction Settings", "Prediction");
            PredictionMenu.AddGroupLabel("Prediction Settings:");
            PredictionMenu.Add("Qpred", new Slider("Select Mystic Shot {0}(%) Hitchance", 80, 0, 100));
            PredictionMenu.Add("Wpred", new Slider("Select Essence Flux {0}(%) Hitchance", 80, 0, 100));
            PredictionMenu.Add("rpred", new Slider("Select Trueshot Barrage {0}(%) Hitchance", 80, 0, 100));
            PredictionMenu.AddLabel("Work Only For Normal Q/W And All R Logic");
            PredictionMenu.AddLabel(
                "Higher % ->Higher chance of spell landing on target but takes more time to get casted");
            PredictionMenu.AddLabel(
                "Lower % ->Faster casting but lower chance that the spell will land on your target. ");

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddGroupLabel("Harass Settings:");
            HarassMenu.Add("useQHarass", new CheckBox("Use Mystic Shot"));
            HarassMenu.Add("useWHarass", new CheckBox("Use Essence Flux"));
            HarassMenu.Add("useWHarassMana", new Slider("Min Mana {0}(%)", 70, 0, 100));
            HarassMenu.AddLabel("AutoHarass Settings:");
            HarassMenu.Add("autoQHarass", new CheckBox("Auto Mystic Shot for Harass", false));
            HarassMenu.Add("autoWHarass", new CheckBox("Auto Essence Flux for Harass", false));
            HarassMenu.Add("autoWHarassMana", new Slider("Min Mana {0}(%)", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddGroupLabel("Lane Clear Settings:");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Mystic Shot Last Hit"));
            JungleLaneMenu.Add("useQManalane", new Slider("Min Mana {0}(%)", 70, 0, 100));
            JungleLaneMenu.Add("UseQUnkillableMinion", new CheckBox("Use Mystic Shot Unkillable Minion"));
            JungleLaneMenu.Add("Unkillablemana", new Slider("Min Mana {0}(%)", 70, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear Settings:");
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Mystic Shot"));
            JungleLaneMenu.Add("useQMana", new Slider("Min Mana {0}(%)", 70, 0, 100));

            FleEMenu = Menu.AddSubMenu("Flee Settings", "FleEMenu");
            FleEMenu.Add("FleeQ", new CheckBox("Use W"));

            KillsecureMenu = Menu.AddSubMenu("KillSecure Settings", "MiscSettings");
            KillsecureMenu.AddLabel("KillSecure Settings:");
            KillsecureMenu.Add("UseQks", new CheckBox("Use Mystic Shot ks"));
            KillsecureMenu.Add("UseWks", new CheckBox("Use Essence Flux ks"));
            KillsecureMenu.Add("UseRks", new CheckBox("Use Trueshot Barrage ks"));


            AutoPotHealMenu = Menu.AddSubMenu("Potion & HeaL", "Potion & HeaL");
            AutoPotHealMenu.AddGroupLabel("Auto pot usage");
            AutoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            AutoPotHealMenu.Add("potionminHP", new Slider("Minimum Health {0}(%) to use potion", 40));
            AutoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana {0}(%) to use potion", 20));
            AutoPotHealMenu.AddLabel("AUto Heal Usage");
            AutoPotHealMenu.Add("UseHeal", new CheckBox("Use Heal"));
            AutoPotHealMenu.Add("useHealHP", new Slider("Minimum Health {0}(%) to use Heal", 20));

            ItemMenu = Menu.AddSubMenu("Item Settings", "ItemMenuettings");
            ItemMenu.AddGroupLabel("Item Settings");
            ItemMenu.Add("StackTear", new CheckBox("Auto stack tear in Base", true));
            ItemMenu.AddLabel("Botrk settings");
            ItemMenu.Add("useBOTRK", new CheckBox("Use BOTRK"));
            ItemMenu.Add("useBotrkMyHP", new Slider("My Health {0}(%) ", 60, 1, 100));
            ItemMenu.Add("useBotrkEnemyHP", new Slider("Enemy Health {0}(%) ", 60, 1, 100));
            ItemMenu.Add("useYoumu", new CheckBox("Use Youmu"));
            ItemMenu.AddLabel("QQs Settings");
            ItemMenu.Add("useQSS", new CheckBox("Use QSS"));
            ItemMenu.Add("Qssmode", new ComboBox(" ", 0, "Auto", "Combo"));
            foreach (var debuff in DeBuffsList)
            {
                ItemMenu.Add(debuff.ToString(), new CheckBox(debuff.ToString()));
            }

            ItemMenu.Add("QssDelay", new Slider("Use QSS Delay(ms)", 250, 0, 1000));


            SkinMenu = Menu.AddSubMenu("Skin Changer", "SkinChanger");
            SkinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 9));

            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawQ", new CheckBox("Draw Mystic Shot Range"));
            DrawMenu.Add("drawW", new CheckBox("Draw Essence Flux Range"));
            DrawMenu.Add("drawR", new CheckBox("Draw Trueshot Barrage Range"));
            DrawMenu.AddLabel("Recall Tracker");
            DrawMenu.Add("draw.Recall", new CheckBox("Chat Print"));
            DrawMenu.AddLabel("Damage indicators");
            DrawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            DrawMenu.Add("percent", new CheckBox("Damage percent info"));
            DrawMenu.Add("howaa", new CheckBox("How Many AA"));
            DrawMenu.Add("Rkill", new CheckBox("Trueshot Barrage kill "));
        }


        public static
            void CorrectTheMenu
            ()
        {
            switch (EMenu.GetValue("E", false))
            {
                case 1:
                {
                    ESafe.IsVisible = true;
                    ESafe.CurrentValue = true;
                    EWall.IsVisible = true;
                    EWall.CurrentValue = true;
                    ECorrect.IsVisible = true;
                    ERange.IsVisible = true;
                    EMenu["label"].IsVisible = true;
                    EPath.CurrentValue = true;
                    EPath.IsVisible = true;
                    EKite.CurrentValue = true;
                    EKite.IsVisible = true;
                    EGap.IsVisible = true;
                    EGrass.IsVisible = false;
                }
                    break;
                case 0:
                case 2:
                {
                    ESafe.IsVisible = false;
                    EWall.IsVisible = false;
                    ECorrect.IsVisible = false;
                    ERange.IsVisible = false;
                    EMenu["label"].IsVisible = false;
                    EPath.IsVisible = false;
                    EKite.IsVisible = false;
                    EGap.IsVisible = false;
                    EGrass.IsVisible = false;
                }
                    break;
                case 3:
                {
                    ESafe.CurrentValue = true;
                    EWall.CurrentValue = false;
                    ECorrect.IsVisible = true;
                    ECorrect.CurrentValue = true;
                    ERange.IsVisible = false;
                    EMenu["label"].IsVisible = false;
                    EPath.IsVisible = false;
                    EKite.IsVisible = true;
                    EGap.IsVisible = true;
                    EGrass.IsVisible = true;
                }
                    break;
            }
        }


        private static void Drawing_OnDraw(EventArgs args)
        {
            {
                if (DrawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                {
                    if (Q.IsReady()) new Circle {Color = Color.Yellow, Radius = Q.Range}.Draw(_Player.Position);
                    else if (Q.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = Q.Range}.Draw(_Player.Position);
                }

                if (DrawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                {
                    if (W.IsReady()) new Circle {Color = Color.Yellow, Radius = W.Range}.Draw(_Player.Position);
                    else if (W.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = W.Range}.Draw(_Player.Position);
                }

                if (DrawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                {
                    if (R.IsReady()) new Circle {Color = Color.Yellow, Radius = R.Range}.Draw(_Player.Position);
                    else if (R.IsOnCooldown)
                        new Circle {Color = Color.Gray, Radius = R.Range}.Draw(_Player.Position);
                }
                DamageIndicator.HealthbarEnabled = DrawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
                DamageIndicator.PercentEnabled = DrawMenu["percent"].Cast<CheckBox>().CurrentValue;
            }
            if (DrawMenu["howaa"].Cast<CheckBox>().CurrentValue)
            {
                // double temp = 0;
                foreach (
                    var noob in
                        ObjectManager.Get<AIHeroClient>().Where(x => x.IsVisible && x.IsEnemy && x.IsValid))
                {
                    var dmg = _Player.GetAutoAttackDamage(noob);

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
                        Player.Instance.GetSpellDamage(noob, SpellSlot.R) > noob.Health + noob.AttackShield)
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


        private static string FormatTime(double time)
        {
            var t = TimeSpan.FromSeconds(time);
            return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }

        private static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender.Team == _Player.Team || !DrawMenu["draw.Recall"].Cast<CheckBox>().CurrentValue) return;

            if (args.Status == TeleportStatus.Start)
            {
                Chat.Print("<font color='#ffffff'>[" + FormatTime(Game.Time) + "]</font> " + sender.BaseSkinName +
                           " has <font color='#00ff66'>started</font> recall.");
            }

            if (args.Status == TeleportStatus.Abort)
            {
                Chat.Print("<font color='#ffffff'>[" + FormatTime(Game.Time) + "]</font> " + sender.BaseSkinName +
                           " has <font color='#ff0000'>aborted</font> recall.");
            }

            if (args.Status == TeleportStatus.Finish)
            {
                Chat.Print("<font color='#ffffff'>[" + FormatTime(Game.Time) + "]</font> " + sender.BaseSkinName +
                           " has <font color='#fdff00'>finished</font> recall.");
            }
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

        private static
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
            Ks();
            Auto();
            AutoHaras();
            UseRTarget();
            AutoPot();
        }

        private static void AUtoheal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
            }
        }

        public static void StackTear(EventArgs args)
        {
            if (ItemMenu["StackTear"].Cast<CheckBox>().CurrentValue && Player.Instance.IsInShopRange() &&
                (Tear.IsOwned() || Manamune.IsOwned() || Archangel.IsOwned()))
            {
                Q.Cast(Game.CursorPos);
                W.Cast(Game.CursorPos);
            }
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

        private static void DoQss()
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
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var fleeQ = FleEMenu["FleeQ"].Cast<CheckBox>().CurrentValue;

            if (fleeQ && W.IsReady() && targetW.IsValidTarget(W.Range))
            {
                W.Cast(targetW);
            }
        }

        public static void Auto()
        {
            var eonCcW = ComboMenu["CCW"].Cast<CheckBox>().CurrentValue;
            var eonCcQ = ComboMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            if (eonCcW)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
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
                if (eonCcQ)
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
                    }
                }
            }
        }

        public static
            void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    if (target == null || !(target is AIHeroClient) || target.IsDead || target.IsInvulnerable ||
                        !target.IsEnemy || target.IsPhysicalImmune || target.IsZombie)
                        return;

                var enemy = target as AIHeroClient;
                if (enemy == null)
                    return;
                if (ComboMenu["Qlogic"].Cast<ComboBox>().CurrentValue == 1)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(enemy);
                    }
                    if (ComboMenu["Wlogic"].Cast<ComboBox>().CurrentValue == 1)
                    {
                        if (W.IsReady())
                        {
                            W.Cast(enemy);
                        }
                    }
                }
            }
        }

        public static void Ks()
        {
            var usewks = KillsecureMenu["UseWks"].Cast<CheckBox>().CurrentValue;
            var useQks = KillsecureMenu["UseQks"].Cast<CheckBox>().CurrentValue;
            var useRks = KillsecureMenu["UseRks"].Cast<CheckBox>().CurrentValue;
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        x => x.Distance(_Player) <= W.Range && x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie
                             && !x.HasBuff("JudicatorIntervention") //kayle R
                             && !x.HasBuff("zhonyasringshield") //zhonya
                             && !x.HasBuff("VladimirSanguinePool") //vladimir W
                             && !x.HasBuff("ChronoShift") //zilean R
                             && !x.HasBuff("mordekaisercotgself") //mordekaiser R
                             && !x.HasBuff("UndyingRage") //tryndamere R
                             && !x.HasBuff("sionpassivezombie") //sion Passive
                             && !x.HasBuff("KarthusDeathDefiedBuff") //karthus passive
                             && !x.HasBuff("kogmawicathiansurprise"))) //kog'maw passive
            {
                if (usewks && W.IsReady() &&
                    SpellDamage.Wdamage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 650)

                {
                    var prediction = W.GetPrediction(enemy);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        W.Cast(prediction.CastPosition);
                    }
                }
                if (useQks && Q.IsReady() &&
                    SpellDamage.Qdamage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 650)

                {
                    var prediction = Q.GetPrediction(enemy);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                }
                if (useRks && R.IsReady() &&
                    SpellDamage.Rdamage(enemy) >= enemy.Health && enemy.Distance(_Player) >= 750)

                {
                    var prediction = R.GetPrediction(enemy);
                    if (prediction.HitChance >= HitChance.High)
                    {
                        R.Cast(prediction.CastPosition);
                    }
                }
            }
        }

        public static void JungleClear()
        {
            var useq = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var mana = JungleLaneMenu["useQMana"].Cast<Slider>().CurrentValue;

            if (Orbwalker.IsAutoAttacking) return;
            {
                if (useq && Player.Instance.ManaPercent > mana)
                {
                    var minions =
                        EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range)
                            .Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                    if (minions.Count() > 0)
                    {
                        Q.Cast(minions.First());
                    }
                }
            }
        }

        public static
            void WaveClear()
        {
            var useq = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var mana = JungleLaneMenu["useQManalane"].Cast<Slider>().CurrentValue;


            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position,
                    Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= Q.Range &&
                        m.Health <= _Player.GetSpellDamage(m, SpellSlot.Q) - 20 &&
                        m.IsValidTarget());


            if (Q.IsReady() && useq && qminion != null && mana > Player.Instance.ManaPercent)
            {
                Q.Cast(qminion);
            }
        }
    

    public static void On_Unkillable_Minion(Obj_AI_Base unit, Orbwalker.UnkillableMinionArgs args)
        {
            if (unit == null
                || Orbwalker.ActiveModes.Combo.IsActive()) return;
            if (args.RemainingHealth <= SpellDamage.Qdamage(unit) && Q.IsReady() &&
                JungleLaneMenu["UseQUnkillableMinion"].Cast<CheckBox>().CurrentValue && JungleLaneMenu["Unkillablemana"].Cast<Slider>().CurrentValue > Player.Instance.ManaPercent)
            {
                if (Q.IsInRange(unit))
                {
                    Q.Cast(unit);
                }
            }
        }

        public static
            void AutoHaras()
        {
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (HarassMenu["autoWHarass"].Cast<CheckBox>().CurrentValue &&
                W.IsReady() && targetW.IsValidTarget(W.Range) &&
                Player.Instance.ManaPercent > HarassMenu["autoWHarassMana"].Cast<Slider>().CurrentValue)
            {
                W.Cast(targetW);
            }
            if (HarassMenu["autoQHarass"].Cast<CheckBox>().CurrentValue &&
                Q.IsReady() && targetW.IsValidTarget(Q.Range) &&
                Player.Instance.ManaPercent > HarassMenu["autoWHarassMana"].Cast<Slider>().CurrentValue)
            {
                Q.Cast(targetQ);
            }
        }

        public static
            void Harass()
        {
            var targetW = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var wmana = HarassMenu["useWHarassMana"].Cast<Slider>().CurrentValue;
            var wharass = HarassMenu["useWHarass"].Cast<CheckBox>().CurrentValue;
            var useQharass = HarassMenu["useQHarass"].Cast<CheckBox>().CurrentValue;

            if (Orbwalker.IsAutoAttacking) return;

            if (targetW != null)
            {
                if (wharass && W.IsReady() &&
                    target.Distance(_Player) > _Player.AttackRange &&
                    targetW.IsValidTarget(W.Range) && Player.Instance.ManaPercent > wmana)
                {
                    W.Cast(targetW);
                }
            }

            if (target != null)
            {
                if (useQharass && Q.IsReady())
                {
                    if (target.Distance(_Player) <= Player.Instance.AttackRange)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static
            void Combo()
        {
            var distance = ComboMenu["useRRange"].Cast<Slider>().CurrentValue;
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var rpred = PredictionMenu["rpred"].Cast<Slider>().CurrentValue;
            var useR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var hp = ComboMenu["Hp"].Cast<Slider>().CurrentValue;
            var wpred = PredictionMenu["Wpred"].Cast<Slider>().CurrentValue;
            var qpred = PredictionMenu["Qpred"].Cast<Slider>().CurrentValue;
            var targetR = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            if (ComboMenu["Rlogic"].Cast<ComboBox>().CurrentValue == 0 && useR)
            {
                if (R.IsReady() && targetR.Distance(_Player) <= distance && target.HealthPercent <= hp &&
                    !target.IsUnderEnemyturret())
                {
                    var prediction = R.GetPrediction(target);
                    if (prediction.HitChancePercent >= rpred)
                    {
                        R.Cast(prediction.CastPosition);
                    }
                }
            }
            if (ComboMenu["Rlogic"].Cast<ComboBox>().CurrentValue == 1 && useR && targetR.Distance(_Player) <= distance &&
                !target.IsUnderEnemyturret())
            {
                {
                    R.CastIfItWillHit(rCount, rpred);
                }
            }

            if (ComboMenu["Qlogic"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (Q.IsReady() && Q.CanCast(target))
                {
                    var prediction = Q.GetPrediction(target);
                    if (prediction.HitChancePercent >= qpred)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                    else
                    {
                        if (target.IsValidTarget(150))
                        {
                            Q.Cast(target);
                        }
                    }
                }
            }
            if (ComboMenu["Wlogic"].Cast<ComboBox>().CurrentValue == 0)
            {
                if (W.IsReady() && W.CanCast(target))
                {
                    var prediction = W.GetPrediction(target);
                    if (prediction.HitChancePercent >= wpred)
                    {
                        W.Cast(prediction.CastPosition);
                    }
                    else
                    {
                        if (target.IsValidTarget(150))
                        {
                            W.Cast(target);
                        }
                    }
                }
            }
            if (EMenu.GetValue("E", false) > 0 && ComboMenu.Checked("E") && E.IsReady())
            {
                if (Elogic.GetPos() != new Vector3() || Elogic.GetPos() != Vector3.Zero)
                {
                    E.Cast(Elogic.GetPos());
                }
            }
        }

        public static
            void UseRTarget()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target != null &&
                (ComboMenu["ForceR"].Cast<KeyBind>().CurrentValue && R.IsReady() && target.IsValid &&
                 !Player.HasBuff("EzrealR"))) R.Cast(target.Position);
        }
    }
}