using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;

namespace Veigar_The_Troll
{
    internal class Program
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }


        public static Spell.Skillshot Q;
        public static Spell.Skillshot E;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        public static SpellSlot Ignite { get; private set; }
        private static Menu _menu,
            _comboMenu,
            _jungleLaneMenu,
            _miscMenu,
            _drawMenu,
            _skinMenu;

        private static AIHeroClient _target;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }


        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Veigar)
            {
                return;
            }

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 2000, 70) {AllowedCollisionCount = 1};
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular, 1350, int.MaxValue, 225);
            E = new Spell.Skillshot(SpellSlot.E, 500, SkillShotType.Circular, 700, int.MaxValue, 80)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Targeted(SpellSlot.R, 650);

            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");

            _menu = MainMenu.AddMenu("VeigarTheTroll", "VeigarTheTroll");
            _comboMenu = _menu.AddSubMenu("Combo", "Combo");
            _comboMenu.Add("useQCombo", new CheckBox("Use Q"));
            _comboMenu.Add("useWCombo", new CheckBox("Use W"));
            _comboMenu.Add("useECombo", new CheckBox("Use E"));
            _comboMenu.Add("useRCombo", new CheckBox("Use R"));
            _comboMenu.Add("combo.ignite", new CheckBox("Use ignite if combo killable"));
          
            
            _jungleLaneMenu = _menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            _jungleLaneMenu.AddSeparator(12);
            _jungleLaneMenu.AddLabel("Lane Clear");
            _jungleLaneMenu.Add("qFarm", new CheckBox("Cast Q LastHit[ForAllMode]"));
            _jungleLaneMenu.Add("wwFarm", new CheckBox("Use W"));
            _jungleLaneMenu.Add("wFarm", new Slider("Cast W if >= minions hit", 4, 1, 15));
            _jungleLaneMenu.AddSeparator(12);
            _jungleLaneMenu.AddLabel("Jungle Clear");
            _jungleLaneMenu.Add("useQJungle", new CheckBox("Use Q"));
            _jungleLaneMenu.Add("useWJungle", new CheckBox("Use W"));



            _miscMenu = _menu.AddSubMenu("Misc Settings", "MiscSettings");
            _miscMenu.Add("CCQ", new CheckBox("Auto Q on Enemy CC"));
            _miscMenu.Add("CCW", new CheckBox("Auto W on Enemy CC"));
       

            _skinMenu = _menu.AddSubMenu("Skin Changer", "SkinChanger");
            _skinMenu.Add("checkSkin", new CheckBox("Use Skin Changer"));
            _skinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 8));


            _drawMenu = _menu.AddSubMenu("Drawing Settings");
            _drawMenu.Add("drawQ", new CheckBox("Draw Q Range"));
            _drawMenu.Add("drawW", new CheckBox("Draw W Range"));
            _drawMenu.Add("drawE", new CheckBox("Draw E Range"));
            _drawMenu.Add("drawR", new CheckBox("Draw R Range"));
            
           
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnTick += Game_OnTick;
            Game.OnUpdate += OnGameUpdate;

            Chat.Print(
                "<font color=\"#6909aa\" >MeLoDag Presents </font><font color=\"#4dd5ea\" >VeiGar </font><font color=\"#6909aa\" >Kappa Kippo</font>");
        }

        
        private static void Game_OnTick(EventArgs args)
        {
            if (_Player.IsDead)
                return;


            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    Combo();
                   
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
        }



        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null || !target.IsValidTarget()) return;

            Orbwalker.ForcedTarget = target;

            var useE = _comboMenu["useECombo"].Cast<CheckBox>().CurrentValue;
            var useQ = _comboMenu["useQCombo"].Cast<CheckBox>().CurrentValue;
            var useW = _comboMenu["useWCombo"].Cast<CheckBox>().CurrentValue;
            var useR = _comboMenu["useRCombo"].Cast<CheckBox>().CurrentValue;
            var useIgnite = _comboMenu["combo.ignite"].Cast<CheckBox>().CurrentValue;

            {
                if (E.IsReady() && useE)
                {
                    var predE = E.GetPrediction(target);
                    if (predE.HitChance >= HitChance.High)
                    {
                        E.Cast(predE.CastPosition);
                    }
                 else if (predE.HitChance >= HitChance.Immobile)
                   {
                        E.Cast(predE.CastPosition);
                   }
                }
            }

            if (Q.IsReady() && useQ)
            {
                var predQ = Q.GetPrediction(target);
                if (predQ.HitChance >= HitChance.High)
                {
                    Q.Cast(predQ.CastPosition);
                }
            }

            if (W.IsReady() && useW)
            {
                var predW = W.GetPrediction(target);
                if (predW.HitChance >= HitChance.Immobile)
                {
                    W.Cast(predW.CastPosition);
                }
                else if (predW.HitChance >= HitChance.High)
                {
                    W.Cast(predW.CastPosition);
                }
            }


            if (R.IsReady() && useR)
            {
                if (RDamage(target) >= target.Health)
                {
                    R.Cast(target);
                }
            }
            if (useIgnite && target != null)
            {
                if (_Player.Distance(target) <= 600 && RDamage(target) >= target.Health)
                    _Player.Spellbook.CastSpell(Ignite, target);
            }
        }

        public static float RDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                (new[] { 0, 250, 375, 500 }[R.Level] +
                0.8 * target.FlatMagicDamageMod +
                1.0 * _Player.FlatMagicDamageMod));

        }

        public static float QDamage(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical, (float)
                (new[] { 0, 80, 125, 170, 215, 260 }[Q.Level] +
                 0.6 * _Player.FlatMagicDamageMod));
        }

      
        private static void Auto()
        {
            var QonCc = _miscMenu["CCQ"].Cast<CheckBox>().CurrentValue;
            var WonCc = _miscMenu["CCW"].Cast<CheckBox>().CurrentValue;
            if (QonCc)
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
                    if (WonCc)
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
                if (Q.IsReady() && useQJungle && minion != null)
                {
                    Q.Cast(minion.Position);
                }

                if (W.IsReady() && useWJungle && minion != null)
                {
                    W.Cast(minion.Position);
                }
            }
        }

       


        private static void FarmQ()
        {
          var useQ = _jungleLaneMenu["qFarm"].Cast<CheckBox>().CurrentValue;
            var qminion =
                 EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range)
                     .FirstOrDefault(
                         m =>
                             m.Distance(_Player) <= Q.Range &&
                              m.Health <= QDamage(m) - 20 &&
                             m.IsValidTarget());

            if (Q.IsReady() && useQ && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }
        private static
            void FarmW()
        {
            var useW = _jungleLaneMenu["wwFarm"].Cast<CheckBox>().CurrentValue;
            if (W.IsReady() && useW)
            {
                foreach (
                    var enemyMinion in
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => x.IsEnemy && x.Distance(_Player) <= W.Range))
                {
                    var enemyMinionsInRange =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(x => x.IsEnemy && x.Distance(enemyMinion) <= 185)
                            .Count();
                    if (enemyMinionsInRange >= _jungleLaneMenu["wFarm"].Cast<Slider>().CurrentValue && useW)
                    {
                        W.Cast(enemyMinion);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (_target != null && _target.IsValid)
            {

            }

            if (Q.IsReady() && _drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Q.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawQ"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, Q.Range, Color.DarkOliveGreen);
            }

            if (W.IsReady() && _drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, W.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawW"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, W.Range, Color.DarkOliveGreen);
            }

            if (E.IsReady() && _drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, E.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawE"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, E.Range, Color.DarkOliveGreen);
            }

            if (R.IsReady() && _drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, R.Range, Color.Purple);
            }
            else
            {
                if (_drawMenu["drawR"].Cast<CheckBox>().CurrentValue)
                    Drawing.DrawCircle(_Player.Position, R.Range, Color.DarkOliveGreen);
            }
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
