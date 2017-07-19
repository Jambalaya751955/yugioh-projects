using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDBUpdater.Helpers
{
    static class Types
    {
        public static string GetValue(long key)
        {
            if (types_l.ContainsKey(key))
                return types_l[key];
            else
                return null;
        }

        public static List<long> GetKeys(this List<string> types)
        {
            return types_l.Where(pair => types.Contains(pair.Value)).Select(pair => pair.Key).ToList();
        }

        private static Dictionary<long, string> types_l = new Dictionary<long, string>()
        {
            { (long)Math.Pow(2.0, 0.0), "Monster" },
            { (long)Math.Pow(2.0, 1.0), "Spell" },
            { (long)Math.Pow(2.0, 2.0), "Trap" },
            { (long)Math.Pow(2.0, 4.0), "Normal" },
            { (long)Math.Pow(2.0, 5.0), "Effect" },
            { (long)Math.Pow(2.0, 6.0), "Fusion" },
            { (long)Math.Pow(2.0, 7.0), "Ritual" },
            { (long)Math.Pow(2.0, 9.0), "Spirit" },
            { (long)Math.Pow(2.0, 10.0), "Union" },
            { (long)Math.Pow(2.0, 11.0), "Gemini" },
            { (long)Math.Pow(2.0, 12.0), "Tuner" },
            { (long)Math.Pow(2.0, 13.0), "Synchro" },
            { (long)Math.Pow(2.0, 14.0), "Token" },
            { (long)Math.Pow(2.0, 15.0), "Quick-Play" },
            { (long)Math.Pow(2.0, 16.0), "Continuous" },
            { (long)Math.Pow(2.0, 17.0), "Equip" },
            { (long)Math.Pow(2.0, 18.0), "Field" },
            { (long)Math.Pow(2.0, 19.0), "Counter" },
            { (long)Math.Pow(2.0, 20.0), "Flip" },
            { (long)Math.Pow(2.0, 21.0), "Toon" },
            { (long)Math.Pow(2.0, 22.0), "Xyz" },
            { (long)Math.Pow(2.0, 24.0), "Pendulum" },
            { (long)Math.Pow(2.0, 28.0), "Armor" },
            { (long)Math.Pow(2.0, 29.0), "Plus" },
            { (long)Math.Pow(2.0, 30.0), "Minus" },
        };
    }
}
