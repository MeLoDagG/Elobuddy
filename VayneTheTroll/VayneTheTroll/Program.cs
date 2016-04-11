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

namespace VayneTheTroll
{
    internal class VayneTheTroll
    {
        public static Spell.Ranged _q;
        public static Spell.Targeted _e;
        public static Spell.Skillshot _e2;
        public static Spell.Active _r;
        public static Spell.Active Heal;

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


        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        public static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Vayne)
            {
                return;
            }

            _q = new Spell.Skillshot(SpellSlot.Q, int.MaxValue, SkillShotType.Linear);
            _e = new Spell.Targeted(SpellSlot.E, 590);
            _e2 = new Spell.Skillshot(SpellSlot.E, 590, SkillShotType.Linear, 250, 1250);
            _r = new Spell.Active(SpellSlot.R);


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

            Chat.Print(
                "<font color=\"#ef0101\" >MeLoDag Presents </font><font color=\"#ffffff\" > VayneTHeTroll </font><font color=\"#ef0101\" >Kappa Kippo</font>");


            Menu = MainMenu.AddMenu("VayneTheTroll", "VayneTheTroll");

            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.Add("useQCombo", new CheckBox("Use Q[To Mouse]"));
            ComboMenu.AddGroupLabel("Use E Settings");
            ComboMenu.AddSeparator(14);
            ComboMenu.Add("useECombo", new CheckBox("Use E"));
            ComboMenu.Add("pushDistance", new Slider("Push Distance", 410, 350, 420));
            ComboMenu.AddGroupLabel("Use R Settings");
            ComboMenu.AddSeparator(14);
            ComboMenu.Add("useRCombo", new CheckBox("Use R"));
            ComboMenu.Add("Rcount", new Slider("R when enemies >= ", 2, 1, 5));


            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");
            HarassMenu.AddLabel("SoonTM");
            //  HarassMenu.Add("useQHarass", new CheckBox("Use Q"));
         //   HarassMenu.Add("useEHarass", new CheckBox("Use E"));
         //   HarassMenu.Add("useEHarassMana", new Slider("E Mana > %", 70, 0, 100));
         //  HarassMenu.Add("useQHarassMana", new Slider("Q Mana > %", 70, 0, 100));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.AddLabel("Lane Clear");
            JungleLaneMenu.Add("useQFarm", new CheckBox("Use Q[LastHit]"));
            JungleLaneMenu.Add("useQMana", new Slider("Q Mana > %", 75, 0, 100));
            JungleLaneMenu.AddLabel("Jungle Clear");
            JungleLaneMenu.AddSeparator(14);
            JungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            JungleLaneMenu.Add("useQJunglemana", new Slider("Mana > %", 40, 0, 100));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.Add("gapcloser", new CheckBox("Auto Q for Gapcloser"));
            MiscMenu.Add("interrupter", new CheckBox("Auto E for Interrupter"));
         
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
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 9));



            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawE", new CheckBox("Draw E Range"));


            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Orbwalker.OnPostAttack += OnAfterAttack;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }


        public static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useEint = MiscMenu["interrupter"].Cast<CheckBox>().CurrentValue;

            {
                if (useEint)
                {
                    if (sender.IsEnemy && _e.IsReady() && sender.Distance(_Player) <= _e.Range)
                    {
                        _e.Cast(sender);
                    }
                }
            }
        }

        public static
            void OnGameUpdate(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                UseHeal();
                ItemUsage();
                comboR();
                Condemn();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
              //  Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                WaveClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                flee();
            }

            AutoPot();
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

        //Gredit GuTenTak
        public static void OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs args)
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

        public static void DoQSS()
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

        public static void UltQSS()
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

        public static void OnAfterAttack(AttackableUnit target, EventArgs args)
        {
            var targetq = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var comboQ = ComboMenu["useQcombo"].Cast<CheckBox>().CurrentValue;

            if (target != null && (!target.IsValid || target.IsDead))
                return;

            var orbwalkermode = Orbwalker.ActiveModesFlags;

            if (orbwalkermode == Orbwalker.ActiveModes.Combo)
            {
                if (comboQ && _q.IsReady() &&
                    _Player.Distance(targetq.Position) < (_Player.GetAutoAttackRange() + _q.Range))
                {
                    if (target != null) _q.Cast(target.Position);
                    {
                        Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                }
            }
        }


        public static
            void UseHeal()
        {
            if (Heal != null && AutoPotHealMenu["UseHeal"].Cast<CheckBox>().CurrentValue && Heal.IsReady() &&
                _Player.HealthPercent <= AutoPotHealMenu["useHealHP"].Cast<Slider>().CurrentValue
                && _Player.CountEnemiesInRange(600) > 0 && Heal.IsReady())
            {
                Heal.Cast();
            }
        }


        public static void Condemn()
        {
            var comboE = ComboMenu["useEcombo"].Cast<CheckBox>().CurrentValue;
            var distance = ComboMenu["pushDistance"].Cast<Slider>().CurrentValue;
          //  if (!_e.IsReady()) return;

            if (comboE && _e.IsReady())
                foreach (
                    var enemy in
                        from enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsValidTarget(550f))
                        let prediction = _e2.GetPrediction(enemy)
                        where NavMesh.GetCollisionFlags(
                            prediction.UnitPosition.To2D()
                                .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                    -distance)
                                .To3D())
                            .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                prediction.UnitPosition.To2D()
                                    .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                        -(distance/2))
                                    .To3D())
                                .HasFlag(CollisionFlags.Wall)
                        select enemy)
                {
                    _e.Cast(enemy);
                }
        }
        
        public static void comboR()
        {
            var rCount = ComboMenu["Rcount"].Cast<Slider>().CurrentValue;
            var comboR = ComboMenu["useRcombo"].Cast<CheckBox>().CurrentValue;
            var TargetR = TargetSelector.GetTarget(_r.Range, DamageType.Magical);

            if (comboR && _Player.CountEnemiesInRange(_Player.AttackRange) >= rCount && _r.IsReady()
                && TargetR != null)
            {
                _r.Cast();
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
            void JungleClear()
        {

           // var useEJungle = JungleLaneMenu["useEJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = JungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;
            var usemana = JungleLaneMenu["useQJunglemana"].Cast<Slider>().CurrentValue;

            Obj_AI_Base jungleMobs =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.Position, _q.Range).FirstOrDefault();
            {
                if (useQJungle && _q.IsReady() && jungleMobs != null && jungleMobs.IsValidTarget(_q.Range) &&
                    _Player.ManaPercent >= usemana)
                {
                    Player.CastSpell(SpellSlot.Q, Game.CursorPos);

                }
            }
        }


        public static
            void WaveClear()
        {
            var useQ = JungleLaneMenu["useQFarm"].Cast<CheckBox>().CurrentValue;
            var useQMana = JungleLaneMenu["useQMana"].Cast<Slider>().CurrentValue;

            if (_q.IsReady() && useQ && _Player.ManaPercent >= useQMana)
            {
                var Minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                    _Player.Position, _Player.GetAutoAttackRange());
                foreach (var minions in
                    Minions.Where(
                        minions => minions.Health < QDamage(minions)))
                {
                    if (minions != null)
                    {
                        Player.CastSpell(SpellSlot.Q, Game.CursorPos);
                    }
                }
            }
        }


        private static double QDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Physical,
                (float)
                    (new[] {0.3, 0.35, 0.4, 0.45, 0.5}[
                        _Player.Spellbook.GetSpell(SpellSlot.Q).Level - 1])*
                (_Player.TotalAttackDamage));
        }

     /*   public static
            void Harass()
        {
            var targetE = TargetSelector.GetTarget(_e.Range, DamageType.Physical);
            var targetQ = TargetSelector.GetTarget(_q.Range, DamageType.Physical);
            var Emana = HarassMenu["useEHarassMana"].Cast<Slider>().CurrentValue;
            var Qmana = HarassMenu["useQHarassMana"].Cast<Slider>().CurrentValue;
            
       }*/

        public static
            void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var useQgap = MiscMenu["gapcloser"].Cast<CheckBox>().CurrentValue;

            if (useQgap && sender.IsEnemy &&
                e.End.Distance(_Player) < 600)
            {
                _q.Cast(e.End);
            }
        }

        public static void flee()
        {

        }

        public static void Drawing_OnDraw(EventArgs args)
        {

            if (DrawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                if (_e2.IsReady()) new Circle {Color = Color.Red, Radius = _e2.Range}.Draw(_Player.Position);
                else if (_e2.IsOnCooldown)
                    new Circle {Color = Color.Gray, Radius = _e2.Range}.Draw(_Player.Position);
            }
        }
    }
}
