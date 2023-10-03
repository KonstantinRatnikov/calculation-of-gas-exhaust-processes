using System.Collections.Generic;
using System.Windows.Controls;

namespace Расчет_процессов_газоотвода
{
    public static class TankerLoading
    {
        public static List<TextBox> textBoxT = new List<TextBox>();
        public static List<TextBox> textBoxQ = new List<TextBox>();

        public static Point[] Points()
        {
            Point[] res = new Point[textBoxT.Count];
            double[] t = new double[textBoxT.Count];
            double[] q = new double[textBoxQ.Count];

            for (int i = 0; i < t.Length; i++)
            {
                if (double.TryParse(textBoxT[i].Text, out t[i]) && double.TryParse(textBoxQ[i].Text, out q[i]))
                {
                    // Преобразование прошло успешно
                    Point point = new Point(t[i], q[i]);
                    res[i] = point;
                }
                else
                {
                    return new Point[0];
                }
            }
            return res;
        }

        public static double LoadingVolume ()
        {
            Point[] points = Points();
            double area = 0.0;

            for (int i = 1; i < points.Length; i++)
            {
                // Ширина трапеции
                double width = points[i].X - points[i - 1].X;

                // Средняя высота трапеции (среднее значение y)
                double averageHeight = (points[i].Y + points[i - 1].Y) / 2.0;

                // Площадь текущей трапеции и добавление ее к общей площади
                area += width * averageHeight;
            }
            return area;
        }

    }
}
