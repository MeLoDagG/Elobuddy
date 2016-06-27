using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace YiTheTroll
{
    internal static class YiTheTrollMenu
    {
        private static Menu _myMenu;
        public static Menu ComboMenu, DrawMeNu, HarassMeNu, Activator, FarmMeNu, MiscMeNu, EvadeMenu, SmiteMenu;

        public static readonly List<string> DodgeSpells = new List<string>()
        {
            "SorakaQ",
            "SorakaE",
            "TahmKenchW",
            "TahmKenchQ",
            "Bushwhack",
            "ForcePulse",
            "KarthusFallenOne",
            "KarthusWallOfPain",
            "KarthusLayWasteA1",
            "KarmaWMantra",
            "KarmaQMissileMantra",
            "KarmaSpiritBind",
            "KarmaQ",
            "JinxW",
            "JinxE",
            "JarvanIVGoldenAegis",
            "HowlingGaleSpell",
            "SowTheWind",
            "ReapTheWhirlwind",
            "IllaoiE",
            "HeimerdingerUltWDummySpell",
            "HeimerdingerUltEDummySpell",
            "HeimerdingerW",
            "HeimerdingerE",
            "HecarimUlt",
            "HecarimRampAttack",
            "GravesQLineSpell",
            "GravesQLineMis",
            "GravesClusterShot",
            "GravesSmokeGrenade",
            "GangplankR",
            "GalioIdolOfDurand",
            "GalioResoluteSmite",
            "FioraE",
            "EvelynnR",
            "EliseHumanE",
            "EkkoR",
            "EkkoW",
            "EkkoQ",
            "DravenDoubleShot",
            "InfectedCleaverMissileCast",
            "DariusExecute",
            "DariusAxeGrabCone",
            "DariusNoxianTacticsONH",
            "DariusCleave",
            "PhosphorusBomb",
            "MissileBarrage",
            "BraumQ",
            "BrandFissure",
            "BardR",
            "BardQ",
            "AatroxQ",
            "AatroxE",
            "AzirE",
            "AzirEWrapper",
            "AzirQWrapper",
            "AzirQ",
            "AzirR",
            "Pulverize",
            "AhriSeduce",
            "CurseoftheSadMummy",
            "InfernalGuardian",
            "Incinerate",
            "Volley",
            "EnchantedCrystalArrow",
            "BraumRWrapper",
            "CassiopeiaPetrifyingGaze",
            "FeralScream",
            "Rupture",
            "EzrealEssenceFlux",
            "EzrealMysticShot",
            "EzrealTrueshotBarrage",
            "FizzMarinerDoom",
            "GnarW",
            "GnarBigQMissile",
            "GnarQ",
            "GnarR",
            "GragasQ",
            "GragasE",
            "GragasR",
            "RiftWalk",
            "LeblancSlideM",
            "LeblancSlide",
            "LeonaSolarFlare",
            "UFSlash",
            "LuxMaliceCannon",
            "LuxLightStrikeKugel",
            "LuxLightBinding",
            "yasuoq3w",
            "VelkozE",
            "VeigarEventHorizon",
            "VeigarDarkMatter",
            "VarusR",
            "ThreshQ",
            "ThreshE",
            "ThreshRPenta",
            "SonaQ",
            "SonaR",
            "ShenShadowDash",
            "SejuaniGlacialPrisonCast",
            "RivenMartyr",
            "JavelinToss",
            "NautilusSplashZone",
            "NautilusAnchorDrag",
            "NamiR",
            "NamiQ",
            "DarkBindingMissile",
            "StaticField",
            "RocketGrab",
            "RocketGrabMissile",
            "timebombenemybuff",
            "NocturneUnspeakableHorror",
            "SyndraQ",
            "SyndraE",
            "SyndraR",
            "VayneCondemn",
            "Dazzle",
            "Overload",
            "AbsoluteZero",
            "IceBlast",
            "LeblancChaosOrb",
            "JudicatorReckoning",
            "KatarinaQ",
            "NullLance",
            "Crowstorm",
            "FiddlesticksDarkWind",
            "BrandWildfire",
            "Disintegrate",
            "FlashFrost",
            "Frostbite",
            "AkaliMota",
            "InfiniteDuress",
            "PantheonW",
            "blindingdart",
            "JayceToTheSkies",
            "IreliaEquilibriumStrike",
            "maokaiunstablegrowth",
            "nautilusgandline",
            "runeprison",
            "WildCards",
            "BlueCardAttack",
            "RedCardAttack",
            "GoldCardAttack",
            "AkaliShadowDance",
            "Headbutt",
            "PowerFist",
            "BrandConflagration",
            "CaitlynYordleTrap",
            "CaitlynAceintheHole",
            "CassiopeiaNoxiousBlast",
            "CassiopeiaMiasma",
            "CassiopeiaTwinFang",
            "Feast",
            "DianaArc",
            "DianaTeleport",
            "EliseHumanQ",
            "EvelynnE",
            "Terrify",
            "FizzPiercingStrike",
            "Parley",
            "GarenQAttack",
            "GarenR",
            "IreliaGatotsu",
            "IreliaEquilibriumStrike",
            "SowTheWind",
            "JarvanIVCataclysm",
            "JaxLeapStrike",
            "JaxEmpowerTwo",
            "JaxCounterStrike",
            "JayceThunderingBlow",
            "KarmaSpiritBind",
            "NetherBlade",
            "KatarinaR",
            "JudicatorRighteousFury",
            "KennenBringTheLight",
            "LeblancChaosOrbM",
            "BlindMonkRKick",
            "LeonaZenithBlade",
            "LeonaShieldOfDaybreak",
            "LissandraW",
            "LissandraQ",
            "LissandraR",
            "LuluQ",
            "LuluW",
            "LuluE",
            "LuluR",
            "SeismicShard",
            "AlZaharMaleficVisions",
            "AlZaharNetherGrasp",
            "MaokaiUnstableGrowth",
            "MordekaiserMaceOfSpades",
            "MordekaiserChildrenOfTheGrave",
            "SoulShackles",
            "NamiW",
            "NasusW",
            "NautilusGrandLine",
            "Takedown",
            "NocturneParanoia",
            "PoppyDevastatingBlow",
            "PoppyHeroicCharge",
            "QuinnE",
            "PuncturingTaunt",
            "RenektonPreExecute",
            "SpellFlux",
            "SejuaniWintersClaw",
            "TwoShivPoisen",
            "Fling",
            "SkarnerImpale",
            "SonaHymnofValor",
            "SwainTorment",
            "SwainDecrepify",
            "BlindingDart",
            "OrianaIzunaCommand",
            "OrianaDetonateCommand",
            "DetonatingShot",
            "BusterShot",
            "TrundleTrollSmash",
            "TrundlePain",
            "MockingShout",
            "Expunge",
            "UdyrBearStance",
            "UrgotHeatseekingLineMissile",
            "UrgotSwap2",
            "VeigarBalefulStrike",
            "VeigarPrimordialBurst",
            "ViR",
            "ViktorPowerTransfer",
            "VladimirTransfusion",
            "VolibearQ",
            "HungeringStrike",
            "XenZhaoComboTarget",
            "XenZhaoSweep",
            "YasuoQ3W",
            "YasuoQ3Mis",
            "YasuoQ3",
            "YasuoRKnockUpComboW"
        };

        public static void LoadMenu()
        {
            MyYiTheTrollPage();
            ComboMenuPage();
            FarmMeNuPage();
            HarassMeNuPage();
            ActivatorPage();
            MyEvadeMenu();
            MySmitePage();
            MiscMeNuPage();
            DrawMeNuPage();
        }

        private static void MyYiTheTrollPage()
        {
            _myMenu = MainMenu.AddMenu("Yi The Troll", "main");
            _myMenu.AddLabel(" Yi The Troll " + Program.Version);
            _myMenu.AddLabel(" Made by MeLoDag");
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
            ComboMenu.AddLabel("E Settings:");
            ComboMenu.Add("combo.E",
                new CheckBox("Use E"));
            ComboMenu.AddLabel("W Settings:");
            ComboMenu.Add("combo.w", new CheckBox("Use W",false));
            ComboMenu.Add("hpw", new Slider("Use W if hp %", 35, 0, 100));
            ComboMenu.Add("ResetAA", new CheckBox("Use W AAreset"));
            ComboMenu.AddLabel("R Settings:");
            ComboMenu.Add("combo.R", new CheckBox("Use Smart R",false));
            ComboMenu.Add("combo.R1", new CheckBox("Use R"));
            ComboMenu.Add("combo.REnemies", new Slider("Use R if Enemy in range", 3, 0, 5));
            ComboMenu.AddGroupLabel("Smite Settings:");
            ComboMenu.Add("smitecombo", new CheckBox("Use Smite Gank/Combo"));
        }


        private static void FarmMeNuPage()
        {
            FarmMeNu = _myMenu.AddSubMenu("Lane Clear Settings", "laneclear");
            FarmMeNu.AddGroupLabel("Lane clear settings:");
            FarmMeNu.Add("Lane.Q",
                new CheckBox("Use Q"));
            FarmMeNu.Add("Lane.E",
                new CheckBox("Use E"));
            FarmMeNu.Add("LaneMana",
                new Slider("Min. Mana for Laneclear Spells %", 60));
            FarmMeNu.AddSeparator();
            FarmMeNu.AddGroupLabel("Jungle Settings");
            FarmMeNu.Add("jungle.Q",
                new CheckBox("Use Q"));
            FarmMeNu.Add("jungle.E",
               new CheckBox("Use E"));
            FarmMeNu.Add("jungle.W",
             new CheckBox("Use W"));
            FarmMeNu.Add("JungleMana",
                new Slider("Min. Mana for Jungle Spells %", 15));
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
            HarassMeNu.AddSeparator();
            HarassMeNu.Add("harass.QE",
                new Slider("Min. Mana for Harass Spells %", 55));
            HarassMeNu.AddSeparator();
            HarassMeNu.AddGroupLabel("KillSteal Settings:");
            HarassMeNu.Add("killsteal.Q",
                new CheckBox("Use Q", false));
            HarassMeNu.Add("killsteal.Smite",
               new CheckBox("Use Smite", false));
        }
        private static void MyEvadeMenu()
        {
            EvadeMenu = _myMenu.AddSubMenu("Evade Settings", "EvadeSettings");
            EvadeMenu.AddGroupLabel("Q/W Evade Option");

            foreach (AIHeroClient hero in EntityManager.Heroes.Enemies)
            {
                EvadeMenu.AddGroupLabel(hero.BaseSkinName);
                {
                    foreach (SpellDataInst spell in hero.Spellbook.Spells)
                    {
                        if (DodgeSpells.Any(el => el == spell.SData.Name))
                        {
                            EvadeMenu.Add(spell.Name, new Slider(hero.BaseSkinName + " : " + spell.Slot.ToString() + " : " + spell.Name, 3, 0, 3));
                            EvadeMenu.AddLabel(spell.Name);
                        }
                    }
                }

                EvadeMenu.AddSeparator();
            }
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
        private static void MySmitePage()
        {
            SmiteMenu = _myMenu.AddSubMenu("Smite Settings", "Smite");
            SmiteMenu.AddGroupLabel("Smite settings");
            SmiteMenu.Add("vSmiteSRU_Dragon_Elder", new CheckBox("Elder Dragon"));
            SmiteMenu.Add("vSmiteSRU_Dragon_Air", new CheckBox("Air Dragon"));
            SmiteMenu.Add("vSmiteSRU_Dragon_Earth", new CheckBox("Fire Dragon"));
            SmiteMenu.Add("vSmiteSRU_Dragon_Fire", new CheckBox("Earth Dragon"));
            SmiteMenu.Add("vSmiteSRU_Dragon_Water", new CheckBox("Water Dragon"));
            SmiteMenu.Add("SRU_Red", new CheckBox("Smite Red Buff"));
            SmiteMenu.Add("SRU_Blue", new CheckBox("Smite Blue Buff"));
            SmiteMenu.Add("SRU_Baron", new CheckBox("Smite Baron"));
            SmiteMenu.Add("SRU_Gromp", new CheckBox("Smite Gromp"));
            SmiteMenu.Add("SRU_Murkwolf", new CheckBox("Smite Wolf"));
            SmiteMenu.Add("SRU_Razorbeak", new CheckBox("Smite Bird"));
            SmiteMenu.Add("SRU_Krug", new CheckBox("Smite Golem"));
            SmiteMenu.Add("Sru_Crab", new CheckBox("Smite Crab"));
        }
        private static void MiscMeNuPage()
        {
            MiscMeNu = _myMenu.AddSubMenu("Misc Menu", "othermenu");
            MiscMeNu.AddGroupLabel("Anti Gap Closer/Interrupt");
            MiscMeNu.Add("gapcloser.Q",
                new CheckBox("Use Q GapCloser"));
            MiscMeNu.AddSeparator();
            MiscMeNu.AddGroupLabel("Skin settings");
            MiscMeNu.Add("checkSkin",
                new CheckBox("Use skin changer:", false));
            MiscMeNu.Add("skin.Id",
                new Slider("Skin Editor", 5, 0, 10));
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


        public static
                bool Nodraw()
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
        public static float Hpw()
        {
            return ComboMenu["hpw"].Cast<Slider>().CurrentValue;
        }
        public static float ComboREnemies()
        {
            return ComboMenu["combo.REnemies"].Cast<Slider>().CurrentValue;
        }
        public static bool ComboR()
        {
            return ComboMenu["combo.R"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ComboR1()
        {
            return ComboMenu["combo.R1"].Cast<CheckBox>().CurrentValue;
        }
        public static bool ResetAa()
        {
            return ComboMenu["ResetAA"].Cast<CheckBox>().CurrentValue;
        }
        public static bool Combosmite()
        {
            return ComboMenu["smitecombo"].Cast<CheckBox>().CurrentValue;
        }
        public static bool LaneQ()
        {
            return FarmMeNu["lane.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool LaneE()
        {
            return FarmMeNu["lane.E"].Cast<CheckBox>().CurrentValue;
        }
        public static float LaneMana()
        {
            return FarmMeNu["LaneMana"].Cast<Slider>().CurrentValue;
        }
        public static float Junglemana()
        {
            return FarmMeNu["Junglemana"].Cast<Slider>().CurrentValue;
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
        public static bool HarassW()
        {
            return HarassMeNu["UseWharass"].Cast<CheckBox>().CurrentValue;
        }
        public static float HarassQe()
        {
            return HarassMeNu["harass.QE"].Cast<Slider>().CurrentValue;
        }
        public static bool KillstealQ()
        {
            return HarassMeNu["killsteal.Q"].Cast<CheckBox>().CurrentValue;
        }
        public static bool KillstealSmite()
        {
            return HarassMeNu["killsteal.Smite"].Cast<CheckBox>().CurrentValue;
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

        public static bool GapcloserQ()
        {
            return MiscMeNu["gapcloser.Q"].Cast<CheckBox>().CurrentValue;
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