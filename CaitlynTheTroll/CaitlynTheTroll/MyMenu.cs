using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace CaitlynTheTroll
{
    internal static class CaitlynTheTrollMeNu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu;

        public static void LoadMenu()
        {
            MyCaitlynTheTrollPage();
            DrawMeNuPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
        }

        private static void MyCaitlynTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Caitlyn The Troll", "main");
            _myMenu.AddLabel(" Caitlyn The Troll " + Program.Version);
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
            ComboMenu.Add("combo.E",
                new CheckBox("Use E"));
            ComboMenu.Add("combo.w",
                new CheckBox("Use W"));
          ComboMenu.Add("combo.R",
                new CheckBox("Use R"));
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Combo preferences:");
           ComboMenu.Add("comboEQbind",
                new KeyBind("Use E + Q Spells", false, KeyBind.BindTypes.HoldActive, 'Z'));
            ComboMenu.Add("combo.CC",
                new CheckBox("Use E CC"));
            ComboMenu.Add("combo.CCQ",
                new CheckBox("Use Q CC"));
            ComboMenu.Add("combo.CCW",
               new CheckBox("Use W CC"));
            ComboMenu.Add("combo.REnemies",
                new Slider("Min Enemyes for R", 1, 0, 5));
        }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("Lane Clear Settings", "laneclear");
            FarmMeNu.AddGroupLabel("Lane clear settings:");
            FarmMeNu.Add("Lane.Q",
                new CheckBox("Use Q"));
            FarmMeNu.Add("LaneMana",
                new Slider("Min. Mana for Laneclear Spells %", 60));
            FarmMeNu.AddSeparator();
            FarmMeNu.AddGroupLabel("Jungle Settings");
            FarmMeNu.Add("jungle.Q",
                new CheckBox("Use Q"));
        }

        private static void HarassMeNuPage()
        {
            HarassMeNu = _myMenu.AddSubMenu("Harass/Killsteal Settings", "hksettings");
            HarassMeNu.AddGroupLabel("Harass Settings:");
            HarassMeNu.AddSeparator();
            HarassMeNu.AddLabel("Use Q on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                HarassMeNu.Add("Harass.Q" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            HarassMeNu.Add("harass.QE",
                new Slider("Min. Mana for Harass Spells %", 55));
            HarassMeNu.AddSeparator();
            HarassMeNu.AddGroupLabel("KillSteal Settings:");
            HarassMeNu.Add("killsteal.Q",
                new CheckBox("Use Q", false));
        }

        private static void ActivatorPage()
        {
            Activator = _myMenu.AddSubMenu("Activator Settings", "Items");
            Activator.AddGroupLabel("Auto QSS if :");
            Activator.Add("Blind",
                new CheckBox("Blind", false));
            Activator.Add("Charm",
                new CheckBox("Charm"));
            Activator.Add("Fear",
                new CheckBox("Fear"));
            Activator.Add("Polymorph",
                new CheckBox("Polymorph"));
            Activator.Add("Stun",
                new CheckBox("Stun"));
            Activator.Add("Snare",
                new CheckBox("Snare"));
            Activator.Add("Silence",
                new CheckBox("Silence", false));
            Activator.Add("Taunt",
                new CheckBox("Taunt"));
            Activator.Add("Suppression",
                new CheckBox("Suppression"));
            Activator.AddGroupLabel("Items usage:");
            Activator.AddSeparator();
       Activator.Add("bilgewater",
                new CheckBox("Use Bilgewater Cutlass"));
            Activator.Add("bilgewater.HP",
                new Slider("Use Bilgewater Cutlass if hp is lower than {0}(%)", 60));
            Activator.AddSeparator();
            Activator.Add("botrk",
                new CheckBox("Use Blade of The Ruined King"));
            Activator.Add("botrk.HP",
                new Slider("Use Blade of The Ruined King if hp is lower than {0}(%)", 60));
            Activator.AddSeparator();
            Activator.Add("youmus",
                new CheckBox("Use Youmus Ghostblade"));
            Activator.Add("items.Youmuss.HP",
                new Slider("Use Youmuss Ghostblade if hp is lower than {0}(%)", 60, 1));
            Activator.Add("youmus.Enemies",
                new Slider("Use Youmus Ghostblade when there are {0} enemies in range", 3, 1, 5));
            Activator.AddSeparator();
            Activator.AddGroupLabel("Potion Settings");
            Activator.Add("spells.Potions.Check",
                new CheckBox("Use Potions"));
            Activator.Add("spells.Potions.HP",
                new Slider("Use Potions when HP is lower than {0}(%)", 60, 1));
            Activator.Add("spells.Potions.Mana",
                new Slider("Use Potions when Mana is lower than {0}(%)", 60, 1));
            Activator.AddSeparator();
            Activator.AddGroupLabel("Spells settings:");
            Activator.AddGroupLabel("Barrier settings:");
            Activator.Add("spells.Barrier.Hp",
                new Slider("Use Barrier when HP is lower than {0}(%)", 30, 1));
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
            MiscMeNu.AddGroupLabel("Settings for Flee");
            MiscMeNu.Add("useEmouse",
                new KeyBind("Use E To Mouse", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
            MiscMeNu.AddGroupLabel("Anti Gap Closer/Interrupt");
            MiscMeNu.Add("gapcloser.E",
                new CheckBox("Use E GapCloser"));
            MiscMeNu.Add("gapcloser.W",
                new CheckBox("Use W GapCloser"));
            MiscMeNu.Add("interupt.W",
              new CheckBox("Use W Interrupt"));
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

        public static bool ComboE()
        {
            return ComboMenu["combo.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool ComboW()
        {
            return ComboMenu["combo.W"].Cast<CheckBox>().CurrentValue;
        }
        public static float ComboREnemies()
        {
            return ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
        }
        public static bool ComboR()
        {
            return ComboMenu["combo.R"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboEq()
        {
            return ComboMenu["comboEQbind"].Cast<KeyBind>().CurrentValue;
        }
       public static bool LaneQ()
        {
            return FarmMeNu["lane.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static float LaneMana()
        {
            return FarmMeNu["LaneMana"].Cast<Slider>().CurrentValue;
        }

        public static bool JungleQ()
        {
            return FarmMeNu["jungle.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static bool HarassQ()
        {
            return HarassMeNu["harass.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static float HarassQe()
        {
            return HarassMeNu["harass.QE"].Cast<Slider>().CurrentValue;
        }

        public static bool KillstealQ()
        {
            return HarassMeNu["killsteal.Q"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Bilgewater()
        {
            return Activator["bilgewater"].Cast<CheckBox>().CurrentValue;
        }

        public static float BilgewaterHp()
        {
            return Activator["bilgewater.HP"].Cast<Slider>().CurrentValue;
        }

        public static bool Botrk()
        {
            return Activator["botrk"].Cast<CheckBox>().CurrentValue;
        }

        public static float BotrkHp()
        {
            return Activator["botrk.HP"].Cast<Slider>().CurrentValue;
        }

        public static bool Youmus()
        {
            return Activator["youmus"].Cast<CheckBox>().CurrentValue;
        }
        public static float YoumusEnemies()
        {
            return Activator["youmus.Enemies"].Cast<Slider>().CurrentValue;
        }

        public static float ItemsYoumuShp()
        {
            return Activator["items.Youmuss.HP"].Cast<Slider>().CurrentValue;
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

        public static float SpellsBarrierHp()
        {
            return Activator["spells.Barrier.Hp"].Cast<Slider>().CurrentValue;
        }

        public static bool Blind()
        {
            return Activator["Blind"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Charm()
        {
            return Activator["Charm"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Fear()
        {
            return Activator["Fear"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Polymorph()
        {
            return Activator["Polymorph"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Stun()
        {
            return Activator["Stun"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Snare()
        {
            return Activator["Snare"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Silence()
        {
            return Activator["Silence"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Taunt()
        {
            return Activator["Taunt"].Cast<CheckBox>().CurrentValue;
        }

        public static bool Suppression()
        {
            return Activator["Suppression"].Cast<CheckBox>().CurrentValue;
        }

        public static int SkinId()
        {
            return MiscMeNu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool GapcloserE()
        {
            return MiscMeNu["gapcloser.E"].Cast<CheckBox>().CurrentValue;
        }

        public static bool GapcloserW()
        {
            return MiscMeNu["gapcloser.W"].Cast<CheckBox>().CurrentValue;
        }
        public static bool InterupteW()
        {
            return MiscMeNu["interupt.W"].Cast<CheckBox>().CurrentValue;
        }

        public static bool SkinChanger()
        {
            return MiscMeNu["SkinChanger"].Cast<CheckBox>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return MiscMeNu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }

        public static bool UseEmouse()
        {
            return MiscMeNu["useEmouse"].Cast<KeyBind>().CurrentValue;
        }
    }
}