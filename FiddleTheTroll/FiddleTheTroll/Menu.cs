using System.Linq;
using System.Security.Cryptography.X509Certificates;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace FiddleTheTroll
{
    internal static class FiddleTheTrollMeNu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu;

        public static void LoadMenu()
        {
            FiddleTheTrollPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
            DrawMeNuPage();
        }

        private static void FiddleTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Fiddle The Troll", "main");
            _myMenu.AddLabel(" Fiddle The Troll " + Program.Version);
            _myMenu.AddLabel(" Made by MeLoDag");
        }

        private static void DrawMeNuPage()
        {
            DrawMeNu = _myMenu.AddSubMenu("Draw  settings", "Draw");
            DrawMeNu.AddGroupLabel("Draw Settings:");
            DrawMeNu.Add("nodraw",
                new CheckBox("No Display Drawing", false));
          DrawMeNu.AddSeparator();
            DrawMeNu.Add("draw.Q",
                new CheckBox("Draw Q"));
            DrawMeNu.Add("draw.W",
                new CheckBox("Draw W"));
            DrawMeNu.Add("draw.E",
                new CheckBox("Draw E"));
            DrawMeNu.Add("draw.R",
                new CheckBox("Draw R"));
            DrawMeNu.AddLabel("Damage indicators");
            DrawMeNu.Add("healthbar", new CheckBox("Healthbar overlay"));
            DrawMeNu.Add("percent", new CheckBox("Damage percent info"));
        }

        private static void ComboMenuPage()
        {
            ComboMenu = _myMenu.AddSubMenu("Combo settings", "Combo");
            ComboMenu.AddGroupLabel("Combo settings:");
            ComboMenu.AddLabel("Use Q  Spell on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                ComboMenu.Add("combo.Q" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            ComboMenu.AddSeparator();
            ComboMenu.AddLabel("Use E  Spell on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                ComboMenu.Add("combo.E" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            ComboMenu.AddSeparator();
            ComboMenu.Add("combo.w",
                new CheckBox("Use W"));
            ComboMenu.AddSeparator();
           /*  ComboMenu.Add("combo.R",
              new CheckBox("Use R"));
        ComboMenu.Add("combo.REnemies",
                 new Slider("Min Enemyes for R", 1, 0, 5)); */
            ComboMenu.AddSeparator(); 
            ComboMenu.AddGroupLabel("Combo preferences:");
            ComboMenu.Add("ForceR",
              new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
             ComboMenu.Add("combo.CCQ",
                new CheckBox("Use Q CC"));
          }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("Lane Jungle Settings", "laneclear");
            FarmMeNu.AddGroupLabel("Lane clear settings:");
            FarmMeNu.Add("Lane.E",
               new CheckBox("Use E"));
            FarmMeNu.Add("LaneMana",
                new Slider("Min. Mana for Laneclear Spells %", 60));
            FarmMeNu.AddSeparator();
            FarmMeNu.AddGroupLabel("Jungle Settings");
            FarmMeNu.Add("jungle.Q",
                new CheckBox("Use Q"));
            FarmMeNu.Add("jungle.W",
              new CheckBox("Use W"));
            FarmMeNu.Add("jungle.E",
              new CheckBox("Use E"));
        }

        private static void HarassMeNuPage()
        {
            HarassMeNu = _myMenu.AddSubMenu("Harass/Killsteal Settings", "hksettings");
            HarassMeNu.AddGroupLabel("Harass Settings:");
            HarassMeNu.AddSeparator();
            HarassMeNu.AddLabel("Use E on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                HarassMeNu.Add("Harass.E" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            HarassMeNu.Add("harass.QE",
                new Slider("Min. Mana for Harass Spells %", 55));
            HarassMeNu.AddSeparator();
            HarassMeNu.AddGroupLabel("KillSteal Settings:");
            HarassMeNu.Add("killsteal.E",
                new CheckBox("Use E", false));
        }

        private static void ActivatorPage()
        {
            Activator = _myMenu.AddSubMenu("Activator Settings", "Items");
            Activator.AddLabel("Zhonyas Settings");
            Activator.Add("Zhonyas", new CheckBox("Use Zhonyas Hourglass"));
            Activator.Add("ZhonyasHp", new Slider("Use Zhonyas Hourglass If Your HP%", 20, 0, 100));
            Activator.AddGroupLabel("Potion Settings");
            Activator.Add("spells.Potions.Check",
                new CheckBox("Use Potions"));
            Activator.Add("spells.Potions.HP",
                new Slider("Use Potions when HP is lower than {0}(%)", 60, 1));
            Activator.Add("spells.Potions.Mana",
                new Slider("Use Potions when Mana is lower than {0}(%)", 60, 1));
            Activator.AddSeparator();
            Activator.AddGroupLabel("Spells settings:");
            Activator.AddGroupLabel("Heal settings:");
            Activator.Add("spells.Heal.Hp",
                new Slider("Use Heal when HP is lower than {0}(%)", 30, 1));
            Activator.AddGroupLabel("Ignite settings:");
            Activator.Add("spells.Ignite.Focus",
                new Slider("Use Ignite when target HP is lower than {0}(%)", 10, 1));
        }

        private static void MiscMeNuPage()
        {
            MiscMeNu = _myMenu.AddSubMenu("Misc Menu", "othermenu");
            MiscMeNu.AddGroupLabel("Settings GapClose & Interrupt");
            MiscMeNu.Add("gapcloser.Q",
               new CheckBox("Use Q GapCloser"));
            MiscMeNu.Add("gapcloser.E",
                new CheckBox("Use E GapCloser"));
            MiscMeNu.Add("interupt.Q",
              new CheckBox("Use Q Interrupt"));
            MiscMeNu.AddSeparator();
            MiscMeNu.AddGroupLabel("Skin settings");
            MiscMeNu.Add("checkSkin",
                new CheckBox("Use skin changer:", false));
            MiscMeNu.Add("skin.Id",
                new Slider("Skin Editor", 5, 0, 10));
        }

        public static bool Nodraw()
        {
            return DrawMeNu["nodraw"].Cast<CheckBox>().CurrentValue;
        }
        
        public static bool DrawingsQ()
        {
            return DrawMeNu["draw.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsW()
        {
            return DrawMeNu["draw.W"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsE()
        {
            return DrawMeNu["draw.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsR()
        {
            return DrawMeNu["draw.R"].Cast<CheckBox>().CurrentValue;
        }

        public static bool DrawingsT()
        {
            return DrawMeNu["draw.T"].Cast<CheckBox>().CurrentValue;
        }
       /*  public static float ComboREnemies()
        {
            return ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
        }
        public static bool ComboR()
        {
            return ComboMenu["combo.R"].Cast<CheckBox>().CurrentValue;
        } */
        public static bool ComboE()
        {
            return ComboMenu["combo.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool ComboW()
        {
            return ComboMenu["combo.W"].Cast<CheckBox>().CurrentValue;
        }

        public static bool UseQonly5()
        {
            return ComboMenu["use.onlyq5"].Cast<CheckBox>().CurrentValue;
        } 
        public static bool LaneE()
        {
            return FarmMeNu["lane.E"].Cast<CheckBox>().CurrentValue;
        }
       public static float LaneMana()
        {
            return FarmMeNu["LaneMana"].Cast<Slider>().CurrentValue;
        }
        public static bool JungleQ()
        {
            return FarmMeNu["jungle.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool JungleE()
        {
            return FarmMeNu["jungle.E"].Cast<CheckBox>().CurrentValue;
        }
        public static bool JungleW()
        {
            return FarmMeNu["jungle.W"].Cast<CheckBox>().CurrentValue;
        }
       public static bool HarassQ()
        {
            return HarassMeNu["harass.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static float HarassQe()
        {
            return HarassMeNu["harass.QE"].Cast<Slider>().CurrentValue;
        }

        public static bool KillstealE()
        {
            return HarassMeNu["killsteal.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool SpellsPotionsCheck()
        {
            return Activator["spells.Potions.Check"].Cast<CheckBox>().CurrentValue;
            
        }
        public static float SpellsPotionsHp()
        {
            return Activator["spells.Potions.HP"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsPotionsM()
        {
            return Activator["spells.Potions.Mana"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsHealHp()
        {
            return Activator["spells.Heal.HP"].Cast<Slider>().CurrentValue;
        }

        public static float SpellsIgniteFocus()
        {
            return Activator["spells.Ignite.Focus"].Cast<Slider>().CurrentValue;
        }
        public static int SkinId()
        {
            return MiscMeNu["skin.Id"].Cast<Slider>().CurrentValue;
        }
       public static bool GapcloserQ()
        {
            return MiscMeNu["gapcloser.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool GapcloserE()
        {
            return MiscMeNu["gapcloser.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool InterupteQ()
        {
            return MiscMeNu["interupt.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static bool SkinChanger()
        {
            return MiscMeNu["SkinChanger"].Cast<CheckBox>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return MiscMeNu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }
    }
}