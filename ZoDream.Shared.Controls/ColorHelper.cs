using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZoDream.Shared.Controls
{
    public static class ColorHelper
    {
        public static Color FromRGBA(byte r, byte g, byte b, byte a)
        {
            return Color.FromArgb(a, r, g, b);
        }

        public static Color FromRGBA(byte r, byte g, byte b, double a)
        {
            return FromRGBA(r, g, b, Convert.ToByte(a * 255));
        }

        public static Color FromRGB(byte r, byte g, byte b)
        {
            return Color.FromRgb(r, g, b);
        }

        /// <summary>
        /// 值越小颜色越深
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static double Deep(Color color)
        {
            return color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
        }

        public static Color From(string val)
        {
            return From(val, Colors.Black);
        }

        public static Color From(string val, Color def)
        {
            val = val.Trim();
            if (val.Length == 0)
            {
                return def;
            }
            if (val[0] == '#')
            {
                return FromHtml(val, def);
            }
            if (!val.Contains(','))
            {
                if (Regex.IsMatch(val, @"^[\da-fA-F]+$"))
                {
                    return FromHtml("#" + val, def);
                }
                return FromHtml(val, def);
            }
            var ms = Regex.Matches(val, @"[\d\.]+");
            return ms.Count switch
            {
                3 => FromRGB(Convert.ToByte(ms[0].Value), Convert.ToByte(ms[1].Value),
                                        Convert.ToByte(ms[2].Value)),//255,255,255
                4 => FromRGBA(Convert.ToByte(ms[0].Value), Convert.ToByte(ms[1].Value),
                                        Convert.ToByte(ms[2].Value),
                                        ms[3].Value.StartsWith('.') || ms[3].Value.StartsWith("0.") ?
                                        Convert.ToDouble(ms[3].Value) :
                                        Convert.ToByte(ms[3].Value)),//0,0,0,0.1
                _ => def,
            };
        }

        private static Color FromHtml(string val, Color def)
        {
            try
            {
                var res = ColorConverter.ConvertFromString(val);
                return res is null ? def : (Color)res;
            }
            catch (Exception)
            {
                return def;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns>#argb</returns>
        public static string To(Color color)
        {
            return color.ToString();
        }
    }
}
