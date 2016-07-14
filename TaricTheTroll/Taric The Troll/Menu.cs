using System.Linq;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace Taric_The_Troll
{
    internal static class TarickTheTrollMeNu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, MiscMeNu, AUtoMenu;

        public static void LoadMenu()
        {
            TarickTheTrollPage();
            ComboMenuPage();
            AUtoMenuPage();
            HarassMeNuPage();
            ActivatorPage();
            MiscMeNuPage();
            DrawMeNuPage();
        }

        private static void TarickTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Taric The Troll", "main");
            _myMenu.AddLabel(" Taric The Troll " + Program.Version);
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
            ComboMenu.Add("combo.Q1", new CheckBox("Use Heal If My hp"));
            ComboMenu.Add("MyhpQ", new Slider("Use Auto Heal If My Hp", 40));
            ComboMenu.Add("combo.W", new CheckBox("Use Shield My Self If My Hp"));
            ComboMenu.Add("MyhpW", new Slider("Use Shield My Self If MY Hp", 40));
            ComboMenu.AddLabel("Use Stun On");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                ComboMenu.Add("combo.E" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            ComboMenu.AddLabel("R Settings");
            ComboMenu.Add("combo.R", new CheckBox("Use R"));
            ComboMenu.Add("MyhpR", new Slider("Use R If My Hp", 20));
            ComboMenu.AddSeparator();
            ComboMenu.AddGroupLabel("Combo preferences:");
            ComboMenu.Add("Force.Stun",new KeyBind("Force Stun On Target", false, KeyBind.BindTypes.HoldActive, 'T'));
            ComboMenu.Add("combo.CC",
                new CheckBox("Use E CC"));
        }

        private static void AUtoMenuPage()
        {
            AUtoMenu = _myMenu.AddSubMenu("Auto Heal Shield Ulty", "AutoHealShieldulty");
            AUtoMenu.AddGroupLabel("Auto Settings");
            AUtoMenu.Add("auto.Q", new CheckBox("Use Auto Heal"));
            AUtoMenu.Add("alliehpQ", new Slider("Use Auto Heal If Ally Hp", 50));
            AUtoMenu.Add("AUtomanaQ", new Slider("Mana Auto Heal", 30));
            AUtoMenu.AddLabel("Use Auto Shield On");
           foreach (var allies in EntityManager.Heroes.Allies.Where(i => !i.IsMe))
           {
               AUtoMenu.Add("Autoshield.Champion" + allies.ChampionName, new CheckBox("" + allies.ChampionName));
           }
            AUtoMenu.Add("alliehp", new Slider("Use Auto Shield If Ally hp", 65));
            AUtoMenu.Add("AutomanaW", new Slider("Mana Auto Shield", 30));
            AUtoMenu.AddLabel("Auto R Settings");
            AUtoMenu.Add("Auto.R", new CheckBox("Use Auto R"));
            AUtoMenu.Add("Auto.Rhp",
               new Slider("Use R If Ally ", 35));
        }
        private static void HarassMeNuPage()
        {
            HarassMeNu = _myMenu.AddSubMenu("Harass Settings", "hksettings");
            HarassMeNu.AddGroupLabel("Harass Settings:");
            HarassMeNu.AddSeparator();
            HarassMeNu.AddLabel("Use Stun on");
            foreach (var enemies in EntityManager.Heroes.Enemies.Where(i => !i.IsMe))
            {
                HarassMeNu.Add("Harass.E" + enemies.ChampionName, new CheckBox("" + enemies.ChampionName));
            }
            HarassMeNu.Add("harassmana",
                new Slider("Min. Mana for Harass Spells %", 55));
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
           MiscMeNu.AddGroupLabel("Anti Gap Closer/Interrupt");
            MiscMeNu.Add("gapcloser.E",
                new CheckBox("Use E GapCloser"));
          MiscMeNu.Add("interupt.E",
              new CheckBox("Use E Interrupt"));
            MiscMeNu.AddSeparator();
            MiscMeNu.AddGroupLabel("Skin settings");
            MiscMeNu.Add("checkSkin",
                new CheckBox("Use skin changer:", true));
            MiscMeNu.Add("skin.Id",
                new Slider("Skin Editor", 2, 0, 10));
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
        public static bool ComboQ()
        {
            return ComboMenu["combo.Q1"].Cast<CheckBox>().CurrentValue;
        }
        public static float MyhpQ()
        {
            return ComboMenu["MyhpQ"].Cast<Slider>().CurrentValue;
        }
        public static bool Combor()
        {
            return ComboMenu["combo.r"].Cast<CheckBox>().CurrentValue;
        }
        public static float Comborhp()
        {
            return ComboMenu["MyhpR"].Cast<Slider>().CurrentValue;
        }
        public static bool ForceStun()
        {
            return ComboMenu["Force.Stun"].Cast<KeyBind>().CurrentValue;
        }
        public static bool AutoQ()
        {
            return AUtoMenu["auto.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static float HpallyQ()
        {
            return AUtoMenu["alliehpQ"].Cast<Slider>().CurrentValue;
        }
        public static float Automanaheal()
        {
            return AUtoMenu["AutomanaQ"].Cast<Slider>().CurrentValue;
        }
        public static float AUtomanaShield()
        {
            return AUtoMenu["AutomanaW"].Cast<Slider>().CurrentValue;
        }
        public static float Hpally()
        {
            return AUtoMenu["alliehp"].Cast<Slider>().CurrentValue;
        }
        public static bool ComboW()
        {
            return ComboMenu["combo.W"].Cast<CheckBox>().CurrentValue;
        }
        public static float ShieldMyHp()
        {
            return ComboMenu["MyhpW"].Cast<Slider>().CurrentValue;
        }
        public static float AutoRhp()
        {
            return AUtoMenu["AUto.Rhp"].Cast<Slider>().CurrentValue;
        }
        public static bool AutoR()
        {
            return AUtoMenu["Auto.R"].Cast<CheckBox>().CurrentValue;
        }
        public static float Harassmana()
        {
            return HarassMeNu["harassmana"].Cast<Slider>().CurrentValue;
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
        public static bool InterruptE()
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