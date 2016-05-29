using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DotNetBejewelledBot
{
    public class BejeweledColor
    {
        public static List<Color> Collection;

        public static Color Green
        {
            get
            {
                return Color.FromArgb(41, 222, 74);
            }
        }

        public static Color Blue
        {
            get
            {
                return Color.FromArgb(13, 128, 231);
            }
        } 

        public static Color Red
        {
            get
            {
                return Color.FromArgb(240, 25, 54);
            }
        }

        public static Color Yellow
        {
            get
            {
                return Color.FromArgb(242, 216, 28);
            }
        }
        public static Color YellowCoin
        {
            get
            {
                return Color.FromArgb(210, 183, 65);
            }
        }

        public static Color Orange
        {
            get
            {
                return Color.FromArgb(246, 161, 64);
            }
        }

        public static Color White
        {
            get
            {
                return Color.FromArgb(225, 225, 225);
            }
        }

        public static Color Purple
        {
            get
            {
                return Color.FromArgb(205, 37, 214);
            }
        }

        public static Color Black
        {
            get
            {
                return Color.Black;
            }
        }
    }
}
