
namespace Расчет_процессов_газоотвода
{
    public static class FindPoint
    {
        public static double Find(double p, Point[] points)
        {
            if (p == 0)
                p = points[0].X;
            for (int i = 1; i < points.Length; i++)
            {
                if (p <= points[i].X)
                {
                    // Линейная интерполяция между двумя ближайшими точками
                    double x1 = points[i - 1].X;
                    double y1 = points[i - 1].Y;
                    double x2 = points[i].X;
                    double y2 = points[i].Y;

                    // Формула линейной интерполяции
                    double interpolatedValue = y1 + (y2 - y1) * (p - x1) / (x2 - x1);

                    return interpolatedValue;
                }
            }
            return points[points.Length - 1].Y;
        }
    }
}
