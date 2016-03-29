using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace IreliaTheTroll
{
    internal class Program
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static int rcount;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static Spell.Targeted Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Skillshot R { get; private set; }
        public static SpellSlot Ignite { get; private set; }
        public static Item Youmuu { get; private set; }
        public static Item Cutlass { get; private set; }
        public static Item Blade { get; private set; }
        public static Item Tiamat { get; private set; }
        public static Item Hydra { get; private set; }
        public static Item Qss = new Item(ItemId.Quicksilver_Sash);
        public static Item Simitar = new Item(ItemId.Mercurial_Scimitar);
        public static Item Botrk = new Item(ItemId.Blade_of_the_Ruined_King);

        public static Menu Menu,
            ComboMenu,
            HarassMenu,
            JungleLaneMenu,
            MiscMenu,
            DrawMenu,
            PrediMenu,
            ItemMenu,
            SkinMenu;

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Irelia)
            {
                return;
            }

            Q = new Spell.Targeted(SpellSlot.Q, 650);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Targeted(SpellSlot.E, 350);
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 0, 1600, 65);

            Ignite = ObjectManager.Player.GetSpellSlotFromName("summonerdot");
            Youmuu = new Item(3142, Q.Range);
            Cutlass = new Item(3144, 450f);
            Blade = new Item(3153, 450f);
            Tiamat = new Item(3077, 400f);
            Hydra = new Item(3074, 400f);

            Chat.Print(
                "<font color=\"#ac1616\" >MeLoDag Presents </font><font color=\"#CCFFFF\" >IreLia </font><font color=\"#ac1616\" >Kappa Kippo</font>");

            Menu = MainMenu.AddMenu("IreliaTheTroll", "IreliaTheTroll");
            ComboMenu = Menu.AddSubMenu("Combo Settings", "Combo");
            ComboMenu.AddGroupLabel("Combo");
            ComboMenu.AddGroupLabel("Q settings");
            ComboMenu.Add("combo.q", new CheckBox("Use Q on enemy"));
            ComboMenu.Add("combo.q.minrange", new Slider("Minimum range to Q enemy", 450, 0, 650));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.q.undertower", new Slider("Q enemy under tower only if their health % under", 40));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.q.lastsecond",
                new CheckBox("Use Q to target always before W buff ends (range doesnt matter)"));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.q.gc", new CheckBox("Use Q to gapclose (killable minions)"));
            ComboMenu.AddSeparator(12);
            ComboMenu.AddGroupLabel("W settings");
            ComboMenu.Add("combo.w", new CheckBox("Use W"));
            ComboMenu.AddSeparator(12);
            ComboMenu.AddGroupLabel("E settings");
            ComboMenu.Add("combo.e", new CheckBox("Use E"));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.e.logic", new CheckBox("advanced logic"));
            ComboMenu.AddSeparator(12);
            ComboMenu.AddGroupLabel("R settings");
            ComboMenu.Add("combo.r", new CheckBox("Use R"));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.r.weave", new CheckBox("sheen synergy"));
            ComboMenu.AddLabel(
                "                                                                                                                                                               .",
                4);
            ComboMenu.Add("combo.r.selfactivated", new CheckBox("only if self activated", false));
            ComboMenu.AddSeparator(12);
            ComboMenu.AddGroupLabel("Extra");
            ComboMenu.Add("combo.items", new CheckBox("Use items"));
            ComboMenu.Add("combo.ignite", new CheckBox("Use ignite if combo killable"));

            HarassMenu = Menu.AddSubMenu("Harass Settings", "Harass");

            HarassMenu.AddGroupLabel("harras");
            HarassMenu.AddGroupLabel("Q settings");
            HarassMenu.Add("harass.q", new CheckBox("Use Q on enemy"));
            HarassMenu.Add("harass.q.minrange", new Slider(" Minimum range to Q enemy", 450, 0, 650));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.q.undertower", new Slider("Q enemy under tower only if their health % under", 40));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.q.lastsecond",
                new CheckBox("Use Q to target always before W buff ends (range doesnt matter)"));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.q.gc", new CheckBox("Use Q to gapclose (killable minions)"));
            HarassMenu.AddSeparator(12);
            HarassMenu.AddGroupLabel("W settings");
            HarassMenu.Add("harass.w", new CheckBox("Use W"));
            HarassMenu.AddSeparator(12);
            HarassMenu.AddGroupLabel("E settings");
            HarassMenu.Add("harass.e", new CheckBox("Use E"));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.e.logic", new CheckBox("advanced logic"));
            HarassMenu.AddSeparator(12);
            HarassMenu.AddGroupLabel("R settings");
            HarassMenu.Add("harass.r", new CheckBox("Use R"));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.r.weave", new CheckBox("sheen synergy"));
            HarassMenu.AddLabel(".", 4);
            HarassMenu.Add("harass.r.selfactivated", new CheckBox("only if self activated", false));

            JungleLaneMenu = Menu.AddSubMenu("Lane Clear Settings", "FarmSettings");
            JungleLaneMenu.Add("laneclear.q", new CheckBox("LaneClear Q"));
            JungleLaneMenu.AddSeparator(12);
            JungleLaneMenu.Add("laneclear.mana", new Slider("Mana manager (%)", 40, 1));

            MiscMenu = Menu.AddSubMenu("Misc Settings", "MiscSettings");
            MiscMenu.Add("misc.ks.q", new CheckBox("Killsteal Q"));
            MiscMenu.Add("misc.ks.e", new CheckBox("Killsteal E"));
            MiscMenu.Add("misc.ks.r", new CheckBox("Killsteal R"));
            MiscMenu.Add("misc.ag.e", new CheckBox("Anti-Gapclose E"));
            MiscMenu.Add("misc.interrupt", new CheckBox("Stun interruptable spells"));

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
            SkinMenu.Add("skin.Id", new Slider("Skin", 1, 0, 5));


            DrawMenu = Menu.AddSubMenu("Drawing Settings");
            DrawMenu.Add("drawings.q", new CheckBox("Draw Q"));
            DrawMenu.AddLabel(".", 4);
            DrawMenu.Add("drawings.e", new CheckBox("Draw E"));
            DrawMenu.AddLabel(".", 4);
            DrawMenu.Add("drawings.r", new CheckBox("Draw R"));


            Bootstrap.Init(null);

            Game.OnUpdate += OnGameUpdate;
            Game.OnTick += OnTick;
            Orbwalker.OnPreAttack += OnPreAttack;
            Drawing.OnDraw += OnDraw;
            Obj_AI_Base.OnBuffGain += OnBuffGain;
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Orbwalker.OnPostAttack += (unit, target) =>
            {
                if (ComboMenu["combo.items"].Cast<CheckBox>().CurrentValue && unit.IsMe && target != null &&
                    Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
                {
                    if (Tiamat.IsReady())
                        Tiamat.Cast();

                    if (Hydra.IsReady())
                        Hydra.Cast();
                }
            };
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!MiscMenu["misc.interrupt"].Cast<CheckBox>().CurrentValue) return;
            if (sender.IsValidTarget(E.Range) && _Player.HealthPercent <= sender.HealthPercent)
                if (E.IsReady())
                    E.Cast(sender);
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!MiscMenu["misc.ag.e"].Cast<CheckBox>().CurrentValue) return;
            if (sender.IsValidTarget(E.Range) && e.End.Distance(_Player.Position) <= E.Range
                && _Player.HealthPercent <= sender.HealthPercent)
            {
                if (E.IsReady())
                    E.Cast(sender);
            }
        }



        private static void OnDraw(EventArgs args)
        {
            if (DrawMenu["drawings.q"].Cast<CheckBox>().CurrentValue)
            {
                if (Q.IsReady()) new Circle { Color = Color.Red, Radius = Q.Range }.Draw(_Player.Position);
                else if (Q.IsOnCooldown)
                    new Circle { Color = Color.Gray, Radius = Q.Range }.Draw(_Player.Position);
            }

            if (DrawMenu["drawings.e"].Cast<CheckBox>().CurrentValue)
            {
                if (E.IsReady()) new Circle { Color = Color.Red, Radius = E.Range }.Draw(_Player.Position);
                else if (E.IsOnCooldown)
                    new Circle { Color = Color.Gray, Radius = E.Range }.Draw(_Player.Position);
            }

            if (DrawMenu["drawings.r"].Cast<CheckBox>().CurrentValue)
            {
                if (R.IsReady()) new Circle { Color = Color.Red, Radius = R.Range }.Draw(_Player.Position);
                else if (R.IsOnCooldown)
                    new Circle { Color = Color.Gray, Radius = R.Range }.Draw(_Player.Position);
            }

        }

        private static void OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (ComboMenu["combo.w"].Cast<CheckBox>().CurrentValue &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                target != null && target.IsValidTarget())
                if (W.IsReady())
                    W.Cast();

            if (HarassMenu["harass.w"].Cast<CheckBox>().CurrentValue &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                target != null &&
                target.Type == _Player.Type &&
                target.IsValidTarget())
                if (W.IsReady())
                    W.Cast();
        }

        private static void OnTick(EventArgs args)
        {
            Killsteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                ItemUsage();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {

            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Clear();
            }
            ItemUsage();
            RCount();
        }

        //Gredit GuTenTak
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
                if (type == BuffType.Flee && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
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
                if (type == BuffType.Flee && ItemMenu["Fear"].Cast<CheckBox>().CurrentValue)
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
        private static void DoQSS()
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

        private static void UltQSS()
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
            return SkinMenu["skin.Id"].Cast<Slider>().CurrentValue;
        }

        public static bool CheckSkin()
        {
            return SkinMenu["checkSkin"].Cast<CheckBox>().CurrentValue;
        }




        private static void Clear()
        {

            if (_Player.ManaPercent <= JungleLaneMenu["laneclear.mana"].Cast<Slider>().CurrentValue) return;

            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range)
                    .FirstOrDefault(
                        m =>
                            m.Distance(_Player) <= Q.Range &&
                            m.Health <= QDamage(m) + ExtraWDamage() + SheenDamage(m) - 10 &&
                            m.IsValidTarget());

            if (Q.IsReady() && JungleLaneMenu["laneclear.q"].Cast<CheckBox>().CurrentValue && qminion != null && !Orbwalker.IsAutoAttacking)
            {
                Q.Cast(qminion);
            }
        }

        private static void Killsteal()
        {
            foreach (
                var enemy in
                    EntityManager.Heroes.Enemies.Where(e => e.Distance(_Player) <= R.Range && e.IsValidTarget() && !e.IsInvulnerable))
            {
                if (Q.IsReady() && MiscMenu["misc.ks.q"].Cast<CheckBox>().CurrentValue &&
                    E.IsReady() && MiscMenu["misc.ks.e"].Cast<CheckBox>().CurrentValue &&
                    EDamage(enemy) + QDamage(enemy) + ExtraWDamage() + SheenDamage(enemy) >=
                            enemy.Health)
                {
                    if (enemy.Distance(_Player) <= Q.Range && enemy.Distance(_Player) > E.Range)
                    {
                        Q.Cast(enemy);
                        var enemy1 = enemy;
                        Core.DelayAction(() => E.Cast(enemy1), (int)(1000 * _Player.Distance(enemy) / 2200));
                    }
                    else if (enemy.Distance(_Player) <= Q.Range)
                    {
                        E.Cast(enemy);
                        var enemy1 = enemy;
                        Core.DelayAction(() => Q.Cast(enemy1), 250);
                    }

                }

                if (MiscMenu["misc.ks.q"].Cast<CheckBox>().CurrentValue && Q.IsReady() &&
                    QDamage(enemy) + ExtraWDamage() + SheenDamage(enemy) >= enemy.Health &&
                    enemy.Distance(_Player) <= Q.Range)
                {
                    Q.Cast(enemy);
                    return;
                }

                if (MiscMenu["misc.ks.e"].Cast<CheckBox>().CurrentValue && E.IsReady() &&
                    EDamage(enemy) >= enemy.Health && enemy.Distance(_Player) <= E.Range)
                {
                    E.Cast(enemy);
                    return;
                }

                if (MiscMenu["misc.ks.r"].Cast<CheckBox>().CurrentValue && R.IsReady() &&
                    RDamage(enemy) >= enemy.Health)
                {
                    R.Cast(enemy);
                }

            }
        }

        private static void RCount()
        {
            if (rcount == 0 && R.IsReady())
                rcount = 4;

            if (!R.IsReady() & rcount != 0)
                rcount = 0;

            foreach (
                var buff in
                    _Player.Buffs.Where(b => b.Name == "ireliatranscendentbladesspell" && b.IsValid))
            {
                rcount = buff.Count;
            }
        }

        private static bool UnderTheirTower(Obj_AI_Base target)
        {
            var tower =
                ObjectManager
                    .Get<Obj_AI_Turret>()
                    .FirstOrDefault(turret => turret != null && turret.Distance(target) <= 775 && turret.IsValid && turret.Health > 0 && !turret.IsAlly);

            return tower != null;
        }

        private static void Combo()
        {
            var gctarget = TargetSelector.GetTarget(Q.Range * 2.5f, DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (gctarget == null) return;

            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range + 350)
                    .Where(
                        m =>
                        m.IsValidTarget()
                        && Prediction.Health.GetPrediction(m, 1000 * (int)(m.Distance(_Player) / 2200))
                        <= QDamage(m) + ExtraWDamage() + SheenDamage(m) - 10)
                    .OrderBy(m => m.Distance(gctarget))
                    .FirstOrDefault();

            if (Q.IsReady())
            {
                if (ComboMenu["combo.q.gc"].Cast<CheckBox>().CurrentValue && qminion != null &&
                    gctarget.Distance(_Player) >= _Player.GetAutoAttackRange(gctarget) &&
                    qminion.Distance(gctarget) <= _Player.Distance(gctarget) &&
                    qminion.Distance(_Player) <= Q.Range)
                {
                    Q.Cast(qminion);
                }

                if (ComboMenu["combo.q"].Cast<CheckBox>().CurrentValue && target != null &&
                    target.Distance(_Player) <= Q.Range &&
                    _Player.Distance(target) >=
                    ComboMenu["combo.q.minrange"].Cast<Slider>().CurrentValue)
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            ComboMenu["combo.q.undertower"].Cast<Slider>().CurrentValue) return;

                    if (ComboMenu["combo.w"].Cast<CheckBox>().CurrentValue)
                        W.Cast();

                    if (qminion != null && qminion.Distance(target) <= _Player.Distance(target)) return;

                    Q.Cast(target);
                }

                if (ComboMenu["combo.q"].Cast<CheckBox>().CurrentValue &&
                    ComboMenu["combo.q.lastsecond"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    var buff = _Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (_Player.Distance(target) / 2200 + .500 + _Player.AttackCastDelay) && !Orbwalker.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                ComboMenu["combo.q.undertower"].Cast<Slider>().CurrentValue) return;

                        Q.Cast(target);
                    }
                }
            }

            if (E.IsReady() && ComboMenu["combo.e"].Cast<CheckBox>().CurrentValue && target != null)
            {
                if (ComboMenu["combo.e.logic"].Cast<CheckBox>().CurrentValue &&
                    target.Distance(_Player) <= E.Range)
                {
                    if (target.HealthPercent >= _Player.HealthPercent && !_Player.IsDashing())
                    {
                        E.Cast(target);
                    }
                    else if (!target.IsAttackingPlayer && !_Player.IsDashing() && target.Distance(_Player) >= E.Range * .5)
                    {
                        E.Cast(target);
                    }
                }
                else if (target.Distance(_Player) <= E.Range)
                {
                    E.Cast(target);
                }
            }

            if (R.IsReady() && ComboMenu["combo.r"].Cast<CheckBox>().CurrentValue && !ComboMenu["combo.r.selfactivated"].Cast<CheckBox>().CurrentValue)
            {
                if (ComboMenu["combo.r.weave"].Cast<CheckBox>().CurrentValue)
                {
                    if (Item.HasItem((int)ItemId.Sheen, _Player) && Item.CanUseItem((int)ItemId.Sheen) || Item.HasItem((int)ItemId.Trinity_Force, _Player) && Item.CanUseItem((int)ItemId.Trinity_Force))
                        if (target != null && !_Player.HasBuff("sheen") &&
                            target.Distance(_Player) <= R.Range)
                        {
                            R.Cast(target);
                        }
                }
                else if (!ComboMenu["combo.r.weave"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    R.Cast(target);

                }
            }
            else if (R.IsReady() && ComboMenu["combo.r"].Cast<CheckBox>().CurrentValue && ComboMenu["combo.r.selfactivated"].Cast<CheckBox>().CurrentValue && rcount <= 3)
            {
                if (ComboMenu["combo.r.weave"].Cast<CheckBox>().CurrentValue)
                {
                    if (Item.HasItem((int)ItemId.Sheen, _Player) && Item.CanUseItem((int)ItemId.Sheen) || Item.HasItem((int)ItemId.Trinity_Force, _Player) && Item.CanUseItem((int)ItemId.Trinity_Force))
                        if (target != null && !Player.HasBuff("sheen") &&
                            target.Distance(_Player) <= R.Range)
                        {
                            R.Cast(target);
                        }
                }
                else if (!ComboMenu["combo.r.weave"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    R.Cast(target);

                }
            }

            if (ComboMenu["combo.ignite"].Cast<CheckBox>().CurrentValue && target != null)
            {
                if (_Player.Distance(target) <= 600 && ComboDamage(target) >= target.Health)
                    _Player.Spellbook.CastSpell(Ignite, target);
            }

            if (ComboMenu["combo.items"].Cast<CheckBox>().CurrentValue && target != null)
            {
                if (Youmuu.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Youmuu.Cast();
                }

                if (_Player.Distance(target) <= 450 && Cutlass.IsReady())
                {
                    Cutlass.Cast(target);
                }

                if (_Player.Distance(target) <= 450 && Blade.IsReady())
                {
                    Blade.Cast(target);
                }
            }
        }
        private static void Harass()
        {
            var gctarget = TargetSelector.GetTarget(Q.Range * 2.5f, DamageType.Physical);
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (gctarget == null) return;

            var qminion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, _Player.Position, Q.Range + 350)
                    .Where(
                        m =>
                        m.IsValidTarget()
                        && Prediction.Health.GetPrediction(m, (int)(m.Distance(_Player) / 2200))
                        <= QDamage(m) + ExtraWDamage() + SheenDamage(m) - 10)
                    .OrderBy(m => m.Distance(gctarget))
                    .FirstOrDefault();

            if (Q.IsReady())
            {
                if (HarassMenu["harass.q.gc"].Cast<CheckBox>().CurrentValue && qminion != null &&
                    gctarget.Distance(_Player) >= _Player.GetAutoAttackRange(gctarget) &&
                    qminion.Distance(gctarget) <= _Player.Distance(gctarget) &&
                    qminion.Distance(_Player) <= Q.Range)
                {
                    Q.Cast(qminion);
                }

                if (HarassMenu["harass.q"].Cast<CheckBox>().CurrentValue && target != null &&
                    target.Distance(_Player) <= Q.Range &&
                    _Player.Distance(target) >=
                    HarassMenu["harass.q.minrange"].Cast<Slider>().CurrentValue)
                {
                    if (UnderTheirTower(target))
                        if (target.HealthPercent >=
                            HarassMenu["harass.q.undertower"].Cast<Slider>().CurrentValue) return;

                    if (HarassMenu["harass.w"].Cast<CheckBox>().CurrentValue)
                        W.Cast();

                    if (qminion != null && qminion.Distance(target) <= _Player.Distance(target)) return;

                    Q.Cast(target);
                }

                if (HarassMenu["harass.q"].Cast<CheckBox>().CurrentValue &&
                    HarassMenu["harass.q.lastsecond"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    var buff = _Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
                    if (buff != null && buff.EndTime - Game.Time <= (_Player.Distance(target) / 2200 + .500 + _Player.AttackCastDelay) && !Orbwalker.IsAutoAttacking)
                    {
                        if (UnderTheirTower(target))
                            if (target.HealthPercent >=
                                HarassMenu["harass.q.undertower"].Cast<Slider>().CurrentValue) return;

                        Q.Cast(target);
                    }
                }
            }

            if (E.IsReady() && HarassMenu["harass.e"].Cast<CheckBox>().CurrentValue && target != null)
            {
                if (HarassMenu["harass.e.logic"].Cast<CheckBox>().CurrentValue &&
                    target.Distance(_Player) <= E.Range)
                {
                    if (target.HealthPercent >= _Player.HealthPercent && !_Player.IsDashing())
                    {
                        E.Cast(target);
                    }
                    else if (!target.IsAttackingPlayer && !_Player.IsDashing() && target.Distance(_Player) >= E.Range * .5)
                    {
                        E.Cast(target);
                    }
                }
                else if (target.Distance(_Player) <= E.Range)
                {
                    E.Cast(target);
                }
            }

            if (R.IsReady() && HarassMenu["harass.r"].Cast<CheckBox>().CurrentValue && !HarassMenu["harass.r.selfactivated"].Cast<CheckBox>().CurrentValue)
            {
                if (HarassMenu["harass.r.weave"].Cast<CheckBox>().CurrentValue)
                {
                    if (Item.HasItem((int)ItemId.Sheen, _Player) && Item.CanUseItem((int)ItemId.Sheen) || Item.HasItem((int)ItemId.Trinity_Force, _Player) && Item.CanUseItem((int)ItemId.Trinity_Force))
                        if (target != null && !Player.HasBuff("sheen") &&
                            target.Distance(_Player) <= R.Range)
                        {
                            R.Cast(target);
                        }
                }
                else if (!HarassMenu["harass.r.weave"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    R.Cast(target);
                }
            }
            else if (R.IsReady() && HarassMenu["harass.r"].Cast<CheckBox>().CurrentValue && HarassMenu["harass.r.selfactivated"].Cast<CheckBox>().CurrentValue && rcount <= 3)
            {
                if (HarassMenu["harass.r.weave"].Cast<CheckBox>().CurrentValue)
                {
                    if (Item.HasItem((int)ItemId.Sheen, _Player) && Item.CanUseItem((int)ItemId.Sheen) || Item.HasItem((int)ItemId.Trinity_Force, _Player) && Item.CanUseItem((int)ItemId.Trinity_Force))
                        if (target != null && !_Player.HasBuff("sheen") &&
                            target.Distance(_Player) <= R.Range)
                        {
                            R.Cast(target);
                        }
                }
                else if (!HarassMenu["harass.r.weave"].Cast<CheckBox>().CurrentValue && target != null)
                {
                    R.Cast(target);
                }
            }
        }


        private static void JungleClear()
        {

        }


        public static float ComboDamage(Obj_AI_Base hero)
        {
            var result = 0d;

            if (Q.IsReady())
            {
                result += QDamage(hero) + ExtraWDamage() + SheenDamage(hero);
            }
            if (W.IsReady() || Player.HasBuff("ireliahitenstylecharged"))
            {
                result += (ExtraWDamage() + _Player.CalculateDamageOnUnit(hero, DamageType.Physical, _Player.TotalAttackDamage)) * 3;
            }
            if (E.IsReady())
            {
                result += EDamage(hero);
            }
            if (R.IsReady())
            {
                result += RDamage(hero);
            }

            return (float)result;
        }

        private static double SheenDamage(Obj_AI_Base target)
        {
            var result = 0d;
            foreach (var item in _Player.InventoryItems)
                switch ((int)item.Id)
                {
                    case 3057:
                        if (Item.CanUseItem((int)ItemId.Sheen))
                            result += _Player.CalculateDamageOnUnit(target, DamageType.Physical, _Player.BaseAttackDamage);

                        break;
                    case 3078:
                        if (Item.CanUseItem((int)ItemId.Trinity_Force))
                            result += _Player.CalculateDamageOnUnit(target, DamageType.Physical, _Player.BaseAttackDamage * 2);

                        break;
                }
            return result;
        }

        private static double ExtraWDamage()
        {


            var extra = 0d;
            var buff = _Player.Buffs.FirstOrDefault(b => b.Name == "ireliahitenstylecharged" && b.IsValid);
            if (buff != null)
                extra += new double[] { 15, 30, 45, 60, 75 }[W.Level - 1];

            return extra;
        }
        private static double QDamage(Obj_AI_Base target)
        {
            return Q.IsReady()
                ? _Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    new float[] { 20, 50, 80, 110, 140 }[Q.Level - 1]
                    + _Player.TotalAttackDamage)
                : 0d;
        }

        private static double EDamage(Obj_AI_Base target)
        {
            return E.IsReady()
                ? _Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Magical,
                    new float[] { 80, 120, 160, 200, 240 }[E.Level - 1]
                    + .5f * _Player.TotalMagicalDamage)
                : 0d;
        }

        private static double RDamage(Obj_AI_Base target)
        {
            return R.IsReady()
                ? _Player.CalculateDamageOnUnit(
                    target,
                    DamageType.Physical,
                    (new float[] { 80, 120, 160 }[R.Level - 1]
                    + .5f * _Player.TotalMagicalDamage
                    + .6f * _Player.FlatPhysicalDamageMod
                    ) * rcount)
                : 0d;
        }
    }
}
