using System;
using System.Collections.Generic;
using System.Linq;

namespace Расчет_процессов_газоотвода
{
    public class СalcPressureInsideTanker
    {
        public double Pn { get; set; }
        public double Pk { get; set; }
        public double tn { get; set; }
        public double Jn { get; set; }
        public double F { get; set; }
        public double Mgvs { get; set; }
        public double T { get; set; }
        public double ksi { get; set; }//ξ
        public double lambda { get; set; } //λ
        public double L { get; set; }
        public double D { get; set; }
        public double V { get; set; }

        public Point[] LoadingSchedule = Array.Empty<Point>();

        public double R;
        public double S;
        public double theta; //θ
        public double psi;//ψ


        double count = 0;

        public List<Point> Pressure = new List<Point>();
        public List<Point> Kp = new List<Point>();
        public List<Point> Qgvs = new List<Point>();



        public СalcPressureInsideTanker()
        {
        }
        double GetQn(double time)
        {
            for (int i = 1; i < LoadingSchedule.Length; i++)
            {
                if (time <= LoadingSchedule[i].X)
                {
                    // Линейная интерполяция между двумя ближайшими точками
                    double x1 = LoadingSchedule[i - 1].X;
                    double y1 = LoadingSchedule[i - 1].Y;
                    double x2 = LoadingSchedule[i].X;
                    double y2 = LoadingSchedule[i].Y;

                    // Формула линейной интерполяции
                    double interpolatedValue = y1 + (y2 - y1) * (time - x1) / (x2 - x1);

                    return interpolatedValue;
                }
            }
            return LoadingSchedule[LoadingSchedule.Length - 1].Y;
        }

        double GetTheta(double t)
        {
            return (GetQn(t) * Math.Sqrt(lambda * L / D)) / (S * Math.Sqrt(R * T));
        }

        double GetPsi(double t)
        {
            return Jn * F * R * T / (GetQn(t) * Pk);
        }
        double DifferentialEquation(double tau, double y)
        {

            theta = GetTheta(tau * tn) / 3600;
            psi = GetPsi(tau * tn);

            if (y < 1)
            {
                return 0;
            }
            return (y + psi * (1 - (1 - ksi) * tau * tau) - Math.Sqrt(y * y - 1) / theta) / (1 - tau);
        }

        public void GetP()
        {

            Pressure = new List<Point>();
            Kp = new List<Point>();
            Qgvs = new List<Point>();

            V = TankerLoading.LoadingVolume();
            R = 8314 / Mgvs;
            S = (Math.PI * D * D) / 4;
            tn = LoadingSchedule.Last().X;
            double y0 = Pn / Pk;      // начальное значение y
            double tau0 = 0.0001;    // начальное значение параметра tau
            double tauMax = 0.9999;
            double h = 0.01;// начальный шаг
            double epsilon = 1e-6; // Задайте желаемую точность
            double tau = tau0;
            double y = y0;

            bool flag = false;

            count = 0;
            while (tau <= tauMax)
            {
                double k1 = h * DifferentialEquation(tau, y);
                double k2 = h * DifferentialEquation(tau + h / 4, y + k1 / 4);
                double k3 = h * DifferentialEquation(tau + 3 * h / 8, y + 3 * k1 / 32 + 9 * k2 / 32);
                double k4 = h * DifferentialEquation(tau + 12 * h / 13, y + 1932 * k1 / 2197 - 7200 * k2 / 2197 + 7296 * k3 / 2197);
                double k5 = h * DifferentialEquation(tau + h, y + 439 * k1 / 216 - 8 * k2 + 3680 * k3 / 513 - 845 * k4 / 4104);
                double k6 = h * DifferentialEquation(tau + h / 2, y - 8 * k1 / 27 + 2 * k2 - 3544 * k3 / 2565 + 1859 * k4 / 4104 - 11 * k5 / 40);

                // Расчет новых значений
                double y1 = y + 25 * k1 / 216 + 1408 * k3 / 2565 + 2197 * k4 / 4104 - k5 / 5;
                double y2 = y + 16 * k1 / 135 + 6656 * k3 / 12825 + 28561 * k4 / 56430 - 9 * k5 / 50 + 2 * k6 / 55;

                // Пересчет шага h
                double delta = Math.Abs(y2 - y1);
                double deltaMax = epsilon * Math.Max(Math.Abs(y1), 1.0);

                if (delta < deltaMax)
                {
                    // Шаг принят
                    y = y2;
                    tau += h;
                    if(tau > tauMax && !flag)
                    {
                        flag = true;
                        tau = tauMax;
                    }
                    Pressure.Add(new Point(tau * tn, y * Pk));

                    double kp = Math.Sqrt(y * y - 1) / (GetTheta(tau * tn)/3600);
                    double q = kp * GetQn(tau * tn);
                    Kp.Add(new Point(tau * tn, kp));
                    Qgvs.Add(new Point(tau * tn, q));
                }

                // Вычисляем новый шаг h
                double rho = 0.84 * Math.Pow(deltaMax / delta, 1.0 / 4.0);
                if (rho > 4.0)
                {
                    h *= 4.0;
                }
                else if (rho < 0.25)
                {
                    h *= 0.25;
                }
                else
                {
                    h *= rho;
                }

                count++;
                if(count > 10 / epsilon)
                {
                    break;
                }
            }
        }

    }
}
