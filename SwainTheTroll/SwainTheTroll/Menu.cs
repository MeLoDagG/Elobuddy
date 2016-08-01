using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace SwainTheTroll
{
    internal static class SwainTheTrollMeNu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu;

        public static void LoadMenu()
        {
            SwainTheTrollPage();
            DrawMeNuPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
        }

        private static void SwainTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Swain  The Troll", "main");
            _myMenu.AddLabel(" Swain The Troll " + Program.Version);
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
            ComboMenu.Add("useQCombo", new CheckBox("Use Q Combo"));
            ComboMenu.Add("useECombo", new CheckBox("Use E Combo"));
            ComboMenu.Add("useWCombo", new CheckBox("Use W Combo"));
            ComboMenu.AddLabel("R Settings:");
            ComboMenu.Add("useRCombo", new CheckBox("Use R Combo"));
            ComboMenu.Add("useRCombo1", new CheckBox("Auto Cancel Ulty"));
            ComboMenu.Add("Rcount", new Slider("Use R If Hit Enemy ", 3, 1, 5));
            ComboMenu.Add("Rlogic", new CheckBox("Smart R 1vs1"));
            ComboMenu.AddLabel("Combo preferences:");
            ComboMenu.Add("combo.CC",
                new CheckBox("Use W CC"));
            ComboMenu.Add("combo.CCQ",
                new CheckBox("Use Q CC"));
           }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("Farm Settings", "FarmSettings");
            FarmMeNu.AddGroupLabel("Lane Clear Settings");
            FarmMeNu.Add("qFarmAlways", new CheckBox("Cast Q"));
            FarmMeNu.Add("wFarm", new CheckBox("Cast W"));
            FarmMeNu.Add("eFarm", new CheckBox("Cast E"));
            FarmMeNu.Add("LaneMana", new Slider("Min Mana % For spells", 70, 0, 100));
            FarmMeNu.AddLabel("Jungle Clear Settings");
            FarmMeNu.Add("useQJungle", new CheckBox("Use Q"));
            FarmMeNu.Add("useWJungle", new CheckBox("Use W"));
            FarmMeNu.Add("useEJungle", new CheckBox("Use E"));
            FarmMeNu.Add("JungleMana", new Slider("Min Mana % For spells", 70, 0, 100));
        }

        private static void HarassMeNuPage()
        {
            HarassMeNu = _myMenu.AddSubMenu("Harass", "Harass");
            HarassMeNu.AddGroupLabel("Harass Setttings");
            HarassMeNu.Add("useQHarass", new CheckBox("Use Q"));
            HarassMeNu.Add("useEHarass", new CheckBox("Use E"));
            HarassMeNu.Add("HarassMana", new Slider("Min Mana % For spells", 70, 0, 100));
            HarassMeNu.AddLabel("KillSteal Settings:");
            HarassMeNu.Add("ksE",
                new CheckBox("Use E", false));
        }

        private static void ActivatorPage()
        {
            Activator = _myMenu.AddSubMenu("Activator Settings", "Items");
            Activator.AddGroupLabel("Zhonyas Settings");
            Activator.Add("Zhonyas", new CheckBox("Use Zhonyas Hourglass"));
            Activator.Add("ZhonyasHp", new Slider("Use Zhonyas Hourglass If Your HP%", 20, 0, 100));
            Activator.AddLabel("Potion Settings");
            Activator.Add("spells.Potions.Check",
                new CheckBox("Use Potions"));
            Activator.Add("spells.Potions.HP",
                new Slider("Use Potions when HP is lower than {0}(%)", 60, 1));
            Activator.Add("spells.Potions.Mana",
                new Slider("Use Potions when Mana is lower than {0}(%)", 60, 1));
            Activator.AddLabel("Heal settings:");
            Activator.Add("spells.Heal.Hp",
                new Slider("Use Heal when HP is lower than {0}(%)", 30, 1));
            Activator.AddLabel("Ignite settings:");
            Activator.Add("spells.Ignite.Focus",
                new Slider("Use Ignite when target HP is lower than {0}(%)", 10, 1));
        }

        private static void MiscMeNuPage()
        {
            MiscMeNu = _myMenu.AddSubMenu("Misc Menu", "othermenu");
            MiscMeNu.AddGroupLabel("Anti Gap Closer/Interrupt");
            MiscMeNu.Add("gapcloser.W",new CheckBox("Use W GapCloser"));
            MiscMeNu.AddLabel("Interrupter Settings:");
            MiscMeNu.Add("interrupter", new CheckBox("Enable Interrupter Using W"));
            MiscMeNu.Add("interrupt.value", new ComboBox("Interrupter DangerLevel", 0, "High", "Medium", "Low"));
            MiscMeNu.AddLabel("Skin settings");
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