using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace IreliaTheTroll
{
    internal static class IreliaTheTrollMenu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu;

        public static void LoadMenu()
        {
            IreliaTheTrollPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
            DrawMeNuPage();
        }

        private static void IreliaTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Irelia The Troll", "main");
            _myMenu.AddLabel(" Irelia The Troll " + Program.Version);
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
            ComboMenu.AddLabel("Q settings:");
            ComboMenu.AddLabel("Check only 1 logic for work use Q Or use Q to target");
            ComboMenu.Add("combo.q", new CheckBox("Use Q"));
            ComboMenu.Add("combo.q.lastsecond", new CheckBox("Use Q to target always before W buff ends (range doesnt matter)",false));
            ComboMenu.Add("combo.qgap", new CheckBox("Use Q on minions to gapclose"));
            ComboMenu.Add("combo.q.minrange", new Slider("Minimum range to Q enemy", 450, 0, 650));
            ComboMenu.Add("combo.q.undertower", new Slider("Q enemy under tower only if their health % under", 40));
            ComboMenu.AddLabel("W settings:");
            ComboMenu.Add("combo.w", new CheckBox("Use W"));
            ComboMenu.AddLabel("E settings:");
            ComboMenu.AddLabel("Check only 1 logic for work use E Or use E if will stun");
            ComboMenu.Add("combo.e", new CheckBox("Use E",false));
            ComboMenu.Add("combo.estun",new CheckBox("Use Only E if will stun"));
            ComboMenu.AddLabel("R settings;");
            ComboMenu.Add("combo.R", new CheckBox("Use R"));
            ComboMenu.Add("ForceR",
              new KeyBind("Force R On Target Selector", false, KeyBind.BindTypes.HoldActive, "T".ToCharArray()[0]));
          }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("Lane Jungle Settings", "laneclear");
            FarmMeNu.AddGroupLabel("Lane clear settings:");
            FarmMeNu.Add("Lane.Q",
               new CheckBox("Use Q"));
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
            HarassMeNu.AddLabel("Use Q on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                HarassMeNu.Add("Harass.Q" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }

            HarassMeNu.AddLabel("Use E on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                HarassMeNu.Add("Harass.E" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            HarassMeNu.Add("harass.QE",
                new Slider("Min. Mana for Harass Spells %", 55));
            HarassMeNu.AddSeparator();
            HarassMeNu.AddGroupLabel("KillSteal Settings:");
            HarassMeNu.Add("killsteal.Q",new CheckBox("Use Q"));
            HarassMeNu.Add("killsteal.E", new CheckBox("Use E"));
            HarassMeNu.Add("killsteal.R", new CheckBox("Use R"));
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
            MiscMeNu.AddGroupLabel("Settings GapClose & Interrupt");
            MiscMeNu.Add("gapcloser.E",
                new CheckBox("Use E GapCloser"));
            MiscMeNu.Add("interupt.E",
              new CheckBox("Use E Interrupt"));
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
          public static float ComboREnemies()
         {
             return ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
         }
         public static bool ComboR()
         {
             return ComboMenu["combo.R"].Cast<CheckBox>().CurrentValue;
         } 
        public static bool ComboQ()
        {
            return ComboMenu["combo.q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboQgapclose()
        {
            return ComboMenu["combo.qgap"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboEstun()
        {
            return ComboMenu["combo.estun"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboE()
        {
            return ComboMenu["combo.E"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboW()
        {
            return ComboMenu["combo.W"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboQlastsec()
        {
            return ComboMenu["combo.q.lastsecond"].Cast<CheckBox>().CurrentValue;
        }
        public static float Qminrange()
        {
            return ComboMenu["combo.q.minrange"].Cast<Slider>().CurrentValue;
        }
        public static float Qundertower()
        {
            return ComboMenu["combo.q.undertower"].Cast<Slider>().CurrentValue;
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
        public static bool KillstealQ()
        {
            return HarassMeNu["killsteal.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool KillstealE()
        {
            return HarassMeNu["killsteal.E"].Cast<CheckBox>().CurrentValue;
        }
        public static bool killstealR()
        {
            return HarassMeNu["killsteal.R"].Cast<CheckBox>().CurrentValue;
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

        public static bool InterupteE()
        {
            return MiscMeNu["interupt.E"].Cast<CheckBox>().CurrentValue;
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