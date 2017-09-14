using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace Lib.UI
{
    public class ColorAlternater
    {
        private double k;
        private SortedList<int, SolidColorBrush> brushes;
        private bool _parity;
        public ColorAlternater(double k = 0.96)
        {
            this.k = k;
            brushes = new SortedList<int, SolidColorBrush>();
        }
        public Color AlternateColor(Color color)
        {
            var newColor = new Color();
            double k1 = _parity ? k : 1;
            newColor.R = (byte)(color.R * k1);
            newColor.G = (byte)(color.G * k1);
            newColor.B = (byte)(color.B * k1);
            newColor.A = color.A;
            _parity = !_parity;
            return newColor;
        }

        public SolidColorBrush AlternateBrush(Color color)
        {
            var aColor = AlternateColor(color);
            int index = BitConverter.ToInt32(new [] {aColor.A, aColor.B, aColor.G, aColor.R}, 0);
            if (!brushes.ContainsKey(index))
                brushes.Add(index, new SolidColorBrush(aColor));
            return brushes[index];
        }
    }
}
