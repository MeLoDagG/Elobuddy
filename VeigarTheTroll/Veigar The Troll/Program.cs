using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using Veigar_The_Troll;
using SharpDX;
using Color = System.Drawing.Color;

namespace Veigar_The_Troll
{
    internal class Program
    {
        public static Item HealthPotion;
        public static Item CorruptingPotion;
        public static Item RefillablePotion;
        public static Item TotalBiscuit;
        public static Item HuntersPotion;
        public static Item ZhonyaHourglass;

        private static Menu _menu,
            _comboMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu,
            _autoPotHealMenu,
            _autostuckEnemy;
        
        private static AIHeroClient _target;

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

  
        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Veigar)
            {
                return;
            }
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);
            ZhonyaHourglass = new Item(ItemId.Zhonyas_Hourglass);


            _menu = MainMenu.AddMenu("VeigarTheTroll", "VeigarTheTroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.Add("useQCombo", new CheckBox("Use Q", true));
            _comboMenu.Add("useWCombo", new CheckBox("Use W", true));
            _comboMenu.Add("useECombo", new CheckBox("Use E", true));
            _comboMenu.AddLabel("E logic With Slider Hitchange");
            _comboMenu.Add("useECombopred", new CheckBox("Use E [% hitchange Slider]", false));
            _comboMenu.Add("predE", new Slider("Use E Pred %"));
            _comboMenu.AddLabel("R Logics");
            _comboMenu.Add("useRburst", new CheckBox("Use R Burst", true));
            _comboMenu.Add("useRCombo", new CheckBox("Use R Ks[Old]", true));
            _comboMenu.Add("useRCombobeta", new CheckBox("USe R New Formula [Beta]", false));
            _comboMenu.AddLabel("Ignite Settings");
            _comboMenu.Add("UseIgnite", new CheckBox("Use Ignite If Combo Killable", true));

            _autostuckEnemy = _menu.AddSubMenu("Auto Stuck Enemy", "AutoStuckEnemy");
            _autostuckEnemy.Add("UseQstuck", new CheckBox("Use Q Stuck [Enemy]", false));
            _autostuckEnemy.Add("UseWstuck", new CheckBox("Use W Stuck [Enemy]", false));
            _autostuckEnemy.Add("StuckEnemyMana", new Slider("Use Q W Mana", 70, 0, 100));

            _jungleLaneMenu = _menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            _jungleLaneMenu.AddLabel("Lane Clear");
            _jungleLaneMenu.Add("qFarm", new CheckBox("Cast Q LastHit[ForAllMode]", true));
            _jungleLaneMenu.Add("wwFarm", new CheckBox("Use W",false));
            _jungleLaneMenu.Add("wFarm", new Slider("Cast W if >= minions hit", 4, 1, 15));
            _jungleLaneMenu.AddLabel("Use Auto Q");
            _jungleLaneMenu.Add("qFarmAuto", new CheckBox("Cast Q LastHit[Auto]", true));
            _jungleLaneMenu.Add("qFarmAutoMana", new Slider("Use Q Mana", 60, 0, 100));
            _jungleLaneMenu.AddLabel("Jungle Clear");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));

            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.AddGroupLabel("Auto SKills CC settings");
            _miscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
            _miscMenu.Add("CCW", new CheckBox("Auto W on Enemy CC"));
            _miscMenu.AddGroupLabel("Ks settings");
            _miscMenu.Add("ksQ", new CheckBox("Killsteal Q"));
           // _miscMenu.Add("ksIgnite", new CheckBox("Killsteal Ignite"));
            _miscMenu.AddLabel("Auto Zhonyas Hourglass");
            _miscMenu.Add("Zhonyas", new CheckBox("Use Zhonyas Hourglass"));
            _miscMenu.Add("ZhonyasHp", new Slider("Use Zhonyas Hourglass If Your HP%", 20, 0, 100));
            _miscMenu.AddLabel("Interrupter settings");
            _miscMenu.Add("interuptE", new CheckBox("Auto E for Interrupter"));
            _miscMenu.AddLabel("Gapcloser");
            _miscMenu.Add("gapcloser", new CheckBox("Auto E for Gapcloser"));


            _autoPotHealMenu = _menu.AddSubMenu("Potion", "Potion");
            _autoPotHealMenu.AddGroupLabel("Auto pot usage");
            _autoPotHealMenu.Add("potion", new CheckBox("Use potions"));
            _autoPotHealMenu.Add("potionminHP", new Slider("Minimum Health % to use potion", 40));
            _autoPotHealMenu.Add("potionMinMP", new Slider("Minimum Mana % to use potion", 20));

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer", false));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));

            _drawMenu = _menu.AddSubMenu("Drawing Settings");
            _drawMenu.AddGroupLabel("DrawS kills");
            _drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            _drawMenu.Add("drawW", new CheckBox("Draw W Range"));
            _drawMenu.Add("drawE", new CheckBox("Draw E Range"));
            _drawMenu.Add("drawR", new CheckBox("Draw R Range"));
            _drawMenu.AddLabel("Damage indicators");
            _drawMenu.Add("healthbar", new CheckBox("Healthbar overlay"));
            _drawMenu.Add("percent", new CheckBox("Damage percent info"));


            DamageIndicator.Initialize(ComboDamage);
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapCloser;

            Chat.Print(
                "<font color=\"#6909aa\" >MeLoDag Presents </font><font color=\"#fffffff\" >Veigar </font><font color=\"#6909aa\" >Kappa Kippo</font>");
        }

        public static
           void Gapcloser_OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (_miscMenu["gapcloser"].Cast<CheckBox>().CurrentValue && sender.IsEnemy &&
                e.End.Distance(_Player) <= 750)
            {
                SpellManager.E.Cast(e.End);
            }
        }
        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender,
            Interrupter.InterruptableSpellEventArgs e)
        {
            var useE = _miscMenu["interuptE"].Cast<CheckBox>().CurrentValue;
         

            {
                if (useE)
                {
                    if (sender.IsEnemy && SpellManager.E.IsReady() && sender.Distance(_Player) <= SpellManager.E.Range)
                    {
                        SpellManager.E.Cast(sender);
                        Chat.Print("<font color=\"#fffffff\" > Use E Interrupt</font>");
                    }
                }
            }
        }

        private static
                void Game_OnTick(EventArgs args)
        {
            Orbwalker.ForcedTarget = null;
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Combo();
                    castR();
                    useEpercent();
                    useRburst();
                }
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                FarmQ();
                FarmW();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                FarmQ();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                FarmQ();
            }
            Auto();
            Killsteal();
            AutoPot();
            castR();
            qFarmAuto();
            AutoHourglass();
            AutoStuckEnemy();
        }

        private static void useRburst()
        {
            var useRburst = _comboMenu["useRburst"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;
            {
                if (SpellManager.R.IsReady() && useRburst)
                {
                    SpellManager.R.Cast(target);
                }
            }
        }

        private static
                void AutoStuckEnemy()
        {

            var useQstuck = _autostuckEnemy["UseQstuck"].Cast<CheckBox>().CurrentValue;
            var useWstuck = _autostuckEnemy["UseWstuck"].Cast<CheckBox>().CurrentValue;
            var StuckEnemyMana = _autostuckEnemy["StuckEnemyMana"].Cast<Slider>().CurrentValue;

            {

                var targetq = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);
                if (targetq != null)
                {
                    if (SpellManager.Q.IsReady() && useQstuck && _Player.ManaPercent >= StuckEnemyMana)
                    {
                        var predQ = SpellManager.Q.GetPrediction(targetq);
                        if (predQ.HitChance >= HitChance.High)
                        {
                            SpellManager.Q.Cast(predQ.CastPosition);
                        }
                        else if (predQ.HitChance >= HitChance.Immobile)
                        {
                            SpellManager.Q.Cast(predQ.CastPosition);
                        }
                    }
                }
                       var targetw = TargetSelector.GetTarget(SpellManager.W.Range, DamageType.Magical);
                       if (targetw != null)
                        {
                            if (SpellManager.W.IsReady() && useWstuck && _Player.ManaPercent >= StuckEnemyMana)
                            {
                                var predW = SpellManager.W.GetPrediction(targetw);
                                if (predW.HitChance >= HitChance.High)
                                {
                                    SpellManager.W.Cast(predW.CastPosition);
                                }
                                else if (predW.HitChance >= HitChance.Immobile)
                                {
                                    SpellManager.W.Cast(predW.CastPosition);
                                }
                            }
                        }
                    }
                }
            
        private static
               void AutoHourglass()
        {
            var Zhonyas = _miscMenu["Zhonyas"].Cast<CheckBox>().CurrentValue;
            var ZhonyasHp = _miscMenu["ZhonyasHp"].Cast<Slider>().CurrentValue;

            if (Zhonyas && _Player.HealthPercent <= ZhonyasHp && ZhonyaHourglass.IsReady())
            {
                ZhonyaHourglass.Cast();
                Chat.Print("<font color=\"#fffffff\" > Use Zhonyas <font>");
            }
        }

        private static
            void AutoPot()
        {
            if (_autoPotHealMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() &&
                Player.Instance.HealthPercent <= _autoPotHealMenu["potionminHP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") ||
                  Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") ||
                  Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                {
                    HealthPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                {
                    TotalBiscuit.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                {
                    RefillablePotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                    return;
                }
            }
            if (Player.Instance.ManaPercent <= _autoPotHealMenu["potionMinMP"].Cast<Slider>().CurrentValue &&
                !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") ||
                  Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                {
                    CorruptingPotion.Cast();
                    Chat.Print("<font color=\"#fffffff\" > Use Pot Kappa kippo</font>");
                }
            }
        }

        private static void castR()
        {
            var useR = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var useRCombobeta = _comboMenu["useRCombobeta"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(SpellManager.R.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;
            {
                if (SpellManager.R.IsReady() && useR)
                {
                    if (Rdamageold(target) >= target.Health)
                    {
                        SpellManager.R.Cast(target);
                        Chat.Print("<font color=\"#fffffff\" > Use Ulty Free Kill</font>");
                    }
                    if (SpellManager.R.IsReady() && useRCombobeta)
                    {
                        if (Rdamage(target) >= target.Health)
                        {
                            SpellManager.R.Cast(target);
                            Chat.Print("<font color=\"#fffffff\" > Use new Ulty formula Free Kill</font>");
                        }
                    }
                }
            }
        }


        private static
                void Killsteal()
        {
            var ksQ = _miscMenu["ksQ"].Cast<CheckBox>().CurrentValue;
            var ksIgnite = _miscMenu["ksIgnite"].Cast<CheckBox>().CurrentValue;


            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(_Player) <= SpellManager.Q.Range && e.IsValidTarget() && !e.IsInvulnerable))

            {
                if (ksQ && SpellManager.Q.IsReady() &&
                    Qdamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= SpellManager.Q.Range)
                {
                    SpellManager.Q.Cast(enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Q Free Kill</font>");
                }
             /*   if (ksIgnite && _Player.Distance(enemy) <= 600 &&
                    _Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >= enemy.Health)
                {
                    _Player.Spellbook.CastSpell(SpellManager.Ignite, enemy);
                    Chat.Print("<font color=\"#fffffff\" > Use Ignite Killsteal</font>");
                } */
             }
        }

        private static void useEpercent()
        {

            var useECombopred = _comboMenu["useECombopred"].Cast<CheckBox>().CurrentValue;
            var predE = _comboMenu["predE"].Cast<Slider>().CurrentValue;
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (eTarget != null)

                if (SpellManager.E.IsReady() && useECombopred && eTarget.Distance(_Player) <= 750)
                {
                    var prede = SpellManager.E.GetPrediction(eTarget);
                    if (prede.HitChancePercent >= predE)
                    {
                        SpellManager.E.Cast(prede.CastPosition);
                    }
                }
        }

        private static
            void Combo()
        {

            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useE = _comboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useIgnite = _comboMenu["UseIgnite"].Cast<CheckBox>().CurrentValue;
            var eTarget = TargetSelector.GetTarget(SpellManager.E.Range, DamageType.Magical);
            if (eTarget != null)
            {

                if (SpellManager.E.IsReady() && useE && eTarget.Distance(_Player) <= 750)
                {
                    var ePosition = eTarget.ServerPosition.Extend(_Player.Position, 350);
                    SpellManager.E.Cast(ePosition.To3D());
                }
            }
                var targetq = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);
                if (targetq != null)
                    {
                        if (SpellManager.Q.IsReady() && useQ)
                        {
                            var predQ = SpellManager.Q.GetPrediction(targetq);
                            if (predQ.HitChance >= HitChance.High)
                            {
                                SpellManager.Q.Cast(predQ.CastPosition);
                            }
                            else if (predQ.HitChance >= HitChance.Immobile)
                            {
                                SpellManager.Q.Cast(predQ.CastPosition);
                            }
                            var targetw = TargetSelector.GetTarget(SpellManager.W.Range, DamageType.Magical);
                            if (targetw != null)
                            {
                                if (SpellManager.W.IsReady() && useW)
                                {
                                    var predW = SpellManager.W.GetPrediction(targetw);
                                    if (predW.HitChance >= HitChance.High)
                                    {
                                        SpellManager.W.Cast(predW.CastPosition);
                                    }
                                    else if (predW.HitChance >= HitChance.Immobile)
                                    {
                                        SpellManager.W.Cast(predW.CastPosition);
                                    }
                                    var target = TargetSelector.GetTarget(SpellManager.Q.Range, DamageType.Magical);
                                    if (target != null)
                                    {
                                        if (useIgnite && targetq != null)
                                        {
                                            if (_Player.Distance(target) <= 600 &&
                                                Qdamage(target) >= target.Health)
                                                _Player.Spellbook.CastSpell(SpellManager.Ignite, target);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
        
        public static
            float ComboDamage(Obj_AI_Base hero)
        {
            var result = 0d;

            if (SpellManager.Q.IsReady())
            {
                result += Qdamage(hero);
            }
            if (SpellManager.W.IsReady())
            {
                result += Wdamage(hero);
            }
            if (SpellManager.R.IsReady())
            {
                result += Rdamageold(hero);
            }

            return (float) result;
        }

        public static float Qdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new[] {0, 70, 110, 150, 190, 230}[SpellManager.Q.Level] + 0.6f*_Player.FlatMagicDamageMod));
        }

        public static float Wdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float) (new[] {0, 100, 150, 200, 250, 300}[SpellManager.W.Level] + 0.99f*_Player.FlatMagicDamageMod));
        }

          public static float Rdamageold(Obj_AI_Base target)
          {
              return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                  (new[] {0, 250, 375, 500}[SpellManager.R.Level] +
                   0.99*target.FlatMagicDamageMod +
                   1.0*_Player.FlatMagicDamageMod));

          } 
        public static double Rdamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                (new[] {0, 175, 250, 325}[SpellManager.R.Level] + (new[] {0, 350, 500, 650}[SpellManager.R.Level] +
                                                                   0.88*target.FlatMagicDamageMod +
                                                                   1.5f*target.HealthPercent)));

        } 
      

        private static void Auto()
        {
            var QonCc = _miscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            var WonCc = _miscMenu["CCW"].Cast<CheckBox>().CurrentValue;
            if (QonCc)
            {
                foreach (var enemy in EntityManager.Heroes.Enemies)
                {
                    if (enemy.Distance(Player.Instance) < SpellManager.Q.Range &&
                        (enemy.HasBuffOfType(BuffType.Stun)
                         || enemy.HasBuffOfType(BuffType.Snare)
                         || enemy.HasBuffOfType(BuffType.Suppression)
                         || enemy.HasBuffOfType(BuffType.Fear)
                         || enemy.HasBuffOfType(BuffType.Knockup)))
                    {
                        SpellManager.Q.Cast(enemy);
                    }
                    if (WonCc)
                    {
                        if (enemy.Distance(Player.Instance) < SpellManager.W.Range &&
                            (enemy.HasBuffOfType(BuffType.Stun)
                             || enemy.HasBuffOfType(BuffType.Snare)
                             || enemy.HasBuffOfType(BuffType.Suppression)
                             || enemy.HasBuffOfType(BuffType.Fear)
                             || enemy.HasBuffOfType(BuffType.Knockup)))
                        {
                            SpellManager.W.Cast(enemy);
                        }
                    }
                }
            }
        }

        private static
            void JungleClear()
        {
            var useWJungle = _jungleLaneMenu["useWJungle"].Cast<CheckBox>().CurrentValue;
            var useQJungle = _jungleLaneMenu["useQJungle"].Cast<CheckBox>().CurrentValue;

            if (useQJungle)
            {
                var minion =
                    EntityManager.MinionsAndMonsters.GetJungleMonsters(_Player.ServerPosition, 950f, true)
                        .FirstOrDefault();
                if (SpellManager.Q.IsReady() && useQJungle && minion != null)
                {
                    SpellManager.Q.Cast(minion.Position);
                }

                if (SpellManager.W.IsReady() && useWJungle && minion != null)
                {
                    SpellManager.W.Cast(minion.Position);
                }
            }
        }

        private static void qFarmAuto()
        {
            var qFarmAuto = _jungleLaneMenu["qFarmAuto"].Cast<CheckBox>().CurrentValue;
            var qFarmAutoMana = _jungleLaneMenu["qFarmAutoMana"].Cast<Slider>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, SpellManager.Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= SpellManager.Q.Range &&
                        m.Health <= _Player.GetSpellDamage(m, SpellSlot.Q) - 20 &&
                        m.IsValidTarget());


            if (SpellManager.Q.IsReady() && qFarmAuto && _Player.ManaPercent >= qFarmAutoMana && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                SpellManager.Q.Cast(qminion);
            }
        }
        private static void FarmQ()
        {
            var useQ = _jungleLaneMenu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, SpellManager.Q.Range)
                    .FirstOrDefault(m =>
                        m.Distance(_Player) <= SpellManager.Q.Range &&
                        m.Health <= _Player.GetSpellDamage(m, SpellSlot.Q) - 20 &&
                        m.IsValidTarget());


            if (SpellManager.Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                SpellManager.Q.Cast(qminion);
            }
        }

        private static
            void FarmW()
        {
            var useW = _jungleLaneMenu["wwFarm"].Cast<CheckBox>().CurrentValue;
            if (SpellManager.W.IsReady() && useW)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(_Player) <= SpellManager.W.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= _jungleLaneMenu["wFarm"].Cast<Slider>().CurrentValue && useW)
                    {
                        SpellManager.W.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_target != null && _target.IsValid)
            {
            }

            if (SpellManager.Q.IsReady() && _drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.Q.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.Q.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.W.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager. W.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.E.IsReady() && _drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.E.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.E.Range, Color.DarkOliveGreen);
            }

            if (SpellManager.R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, SpellManager.R.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, SpellManager.R.Range, Color.DarkOliveGreen);
            }

            DamageIndicator.HealthbarEnabled = _drawMenu["healthbar"].Cast<CheckBox>().CurrentValue;
            DamageIndicator.PercentEnabled = _drawMenu["percent"].Cast<CheckBox>().CurrentValue;
        }

        private static
            void OnGameUpdate(EventArgs args)
        {
            if (CheckSkin())
            {
                Player.SetSkinId(SkinId());
            }
        }

        public static int SkinId()
        {
            return _skinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return _skinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }
    }
}

