using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace VayneTheTroll
{
    internal class Qlogic
    {

        public static
            Vector2 Side(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4;
            result = Vector2.Add(result, point1);
            return result;
        }

        public static
            Vector2 DefQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/-50.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/4;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/4;
            result = Vector2.Add(result, point1);
            return result;
        }


        public static
            Vector2 AggroQ(Vector2 point1, Vector2 point2, double angle)
        {
            angle *= Math.PI/300;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0);
            result.X = (float) (temp.X*Math.Cos(angle) - temp.Y*Math.Sin(angle))/50;
            result.Y = (float) (temp.X*Math.Sin(angle) + temp.Y*Math.Cos(angle))/50;
            result = Vector2.Add(result, point1);
            return result;
        }
    }
}