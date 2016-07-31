using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace AsheTheTroll
{
    class Volley
    {
        private enum VolleyLocations
        {
            Baron,
            Dragon,
         }

        private const float MaxRandomRadius = 15;
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);
        private static readonly Dictionary<GameObjectTeam, Dictionary<VolleyLocations, Vector2>> Locations = new Dictionary<GameObjectTeam, Dictionary<VolleyLocations, Vector2>>
        { 
           {
                GameObjectTeam.Neutral, new Dictionary<VolleyLocations, Vector2>
                {
                    { VolleyLocations.Baron, new Vector2(5007.124f, 10471.45f) },
                    { VolleyLocations.Dragon, new Vector2(9866.148f, 4414.014f) }
                }
            }
        };

        private static readonly Dictionary<VolleyLocations, Func<bool>> EnabledLocations = new Dictionary<VolleyLocations, Func<bool>>
        {
            { VolleyLocations.Baron, () =>  AsheTheTroll.VolleyMenu["Volley.baron"].Cast<CheckBox>().CurrentValue},
            { VolleyLocations.Dragon, () =>  AsheTheTroll.VolleyMenu["Volley.dragon"].Cast<CheckBox>().CurrentValue},
        };

        private static readonly List<Tuple<GameObjectTeam, VolleyLocations>> OpenLocations = new List<Tuple<GameObjectTeam, VolleyLocations>>();
        private static readonly Dictionary<GameObjectTeam, Dictionary<VolleyLocations, Obj_AI_Base>> ActiveVolleys = new Dictionary<GameObjectTeam, Dictionary<VolleyLocations, Obj_AI_Base>>();
        private static Tuple<GameObjectTeam, VolleyLocations> SentLocation { get; set; }

        static Volley()
        {
            if (Game.MapId == GameMapId.SummonersRift)
            {
                
                Game.OnTick += OnTick;
                GameObject.OnCreate += OnCreate;

                
                RecalculateOpenLocations();
            }
        }

        private static void OnTick(EventArgs args)
        {
           if (AsheTheTroll.VolleyMenu["Volley.enable"].Cast<CheckBox>().CurrentValue && AsheTheTroll._e.IsReady() && Player.Instance.ManaPercent >= AsheTheTroll.VolleyMenu["Volley.mana"].Cast<Slider>().CurrentValue && !Player.Instance.IsRecalling())
            {
                if (!AsheTheTroll.VolleyMenu["Volley.noMode"].Cast<CheckBox>().CurrentValue || Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.None)
                {
                    if (OpenLocations.Count > 0 && SentLocation == null)
                    {
                        var closestLocation = OpenLocations.Where(o => Locations[o.Item1][o.Item2].IsInRange(Player.Instance, AsheTheTroll._e.Range - MaxRandomRadius / 2))
                            .OrderByDescending(o => Locations[o.Item1][o.Item2].Distance(Player.Instance, true))
                            .FirstOrDefault();
                        if (closestLocation != null)
                        {
                            var position = Locations[closestLocation.Item1][closestLocation.Item2];
                            var randomized = (new Vector2(position.X - MaxRandomRadius / 2 + Random.NextFloat(0, MaxRandomRadius),
                                position.Y - MaxRandomRadius / 2 + Random.NextFloat(0, MaxRandomRadius))).To3DWorld();
                            SentLocation = closestLocation;
                            AsheTheTroll._e.Cast(randomized);
                            Core.DelayAction(() => SentLocation = null, 2000);
                        }
                    }
                }
            }
        }

        public static void RecalculateOpenLocations()
        {
            OpenLocations.Clear();
            foreach (var location in Locations)
            {
                if (!ActiveVolleys.ContainsKey(location.Key))
                {
                    OpenLocations.AddRange(location.Value.Where(o => EnabledLocations[o.Key]()).Select(loc => new Tuple<GameObjectTeam, VolleyLocations>(location.Key, loc.Key)));
                }
                else
                {
                    OpenLocations.AddRange(from loc in location.Value
                                           where EnabledLocations[loc.Key]() && !ActiveVolleys[location.Key].ContainsKey(loc.Key)
                                           select new Tuple<GameObjectTeam, VolleyLocations>(location.Key, loc.Key));
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (SentLocation == null)
            {
                return;
            }

            var Volley = sender as Obj_AI_Minion;
            if (Volley != null && Volley.IsAlly && Volley.MaxHealth == 2 && Volley.Name == "RobotBuddy")
            {
                Core.DelayAction(() => ValidateVolley(Volley), 1000);
            }
        }

        private static void ValidateVolley(Obj_AI_Base Volley)
        {
            if (Volley.Health == 2 && Volley.GetBuffCount("AsheE") == 1)
            {
                if (!ActiveVolleys.ContainsKey(SentLocation.Item1))
                {
                    ActiveVolleys.Add(SentLocation.Item1, new Dictionary<VolleyLocations, Obj_AI_Base>());
                }
                ActiveVolleys[SentLocation.Item1].Remove(SentLocation.Item2);
                ActiveVolleys[SentLocation.Item1].Add(SentLocation.Item2, Volley);

                SentLocation = null;
                RecalculateOpenLocations();
            }
        }
    }
}
