using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml;
using System.IO;
using Microsoft.Win32;

namespace Расчет_процессов_газоотвода
{
    public class CalcPressureInsideTankerViewModel : INotifyPropertyChanged
    {
        private СalcPressureInsideTanker _сalcPressureInsideTanker;
        public СalcPressureInsideTanker PressureInsideTanker
        {
            get { return _сalcPressureInsideTanker; }
            set
            {
                _сalcPressureInsideTanker = value;
                OnPropertyChanged();
            }
        }
        public CalcPressureInsideTankerViewModel()
        {
            PressureInsideTanker = new СalcPressureInsideTanker();
            PressureInsideTanker.Pn = 101325;
            PressureInsideTanker.Pk = 101325;
            PressureInsideTanker.Jn = 1;
            PressureInsideTanker.F = 10000;
            PressureInsideTanker.Mgvs = 60;
            PressureInsideTanker.T = 293;
            PressureInsideTanker.ksi = 0.5;
            PressureInsideTanker.lambda = 0.012;
            PressureInsideTanker.L = 1500;
            PressureInsideTanker.D = 0.8;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand EnterCommand => new RelayCommand(Enter);
        public ICommand SaveCommand => new RelayCommand(Save);
        public ICommand FindPointsCommand => new RelayCommand(FindPoints);

        public string InformationLoadingVolume { get; set; }

        private System.Windows.Controls.Page graphPage;

        public System.Windows.Controls.Page GraphPage
        {
            get { return graphPage; }
            set
            {
                if (graphPage != value)
                {
                    graphPage = value;
                    OnPropertyChanged(nameof(GraphPage));
                }
            }
        }

        private System.Windows.Controls.Page graphPageKp;

        public System.Windows.Controls.Page GraphPageKp
        {
            get { return graphPageKp; }
            set
            {
                if (graphPageKp != value)
                {
                    graphPageKp = value;
                    OnPropertyChanged(nameof(GraphPageKp));
                }
            }
        }
        private System.Windows.Controls.Page graphPageQgvs;

        public System.Windows.Controls.Page GraphPageQgvs
        {
            get { return graphPageQgvs; }
            set
            {
                if (graphPageQgvs != value)
                {
                    graphPageQgvs = value;
                    OnPropertyChanged(nameof(GraphPageQgvs));
                }
            }
        }

        private System.Windows.Controls.Page graphTankerLoading;

        public System.Windows.Controls.Page GraphTankerLoading
        {
            get { return graphTankerLoading; }
            set
            {
                if (graphTankerLoading != value)
                {
                    graphTankerLoading = value;
                    OnPropertyChanged(nameof(GraphTankerLoading));
                }
            }
        }

        public void ClearFrame()
        {
            GraphPage = new System.Windows.Controls.Page();
            GraphTankerLoading = new System.Windows.Controls.Page();
            GraphPageQgvs = new System.Windows.Controls.Page();
            GraphPageKp = new System.Windows.Controls.Page();
        }
        Point[] GetLoadingSchedule(Point[] p)
        {

            Point[] res = new Point[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                if(p[i].Y < 10)
                {
                    p[i].Y = 10;
                }
                res[i] = new Point(p[i].X, p[i].Y);
            }
            return res;
        }
        private void Enter()
        {
            ClearFrame();

            Point[] points = TankerLoading.Points();
            //Point[] points = new Point[] { new Point(0,0), new Point(0.5, 2000), new Point(1.5, 2000), new Point(2, 10000), new Point(11, 10000), new Point(12.2, 6000), new Point(13, 6000), new Point(13.5, 2000), new Point(14, 2000), new Point(14.5, 0) };


            if (points.Length < 2)
            {
                MessageBox.Show("Необходимо ввести данные о точках для построения графика", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            GraphTankerLoading = new Graph("ГРАФИК ПОГРУЗКИ", points, "Время t, ч", "Объемный расход Qн, м3/ч");

            PressureInsideTanker.LoadingSchedule = GetLoadingSchedule(points);


            PressureInsideTanker.GetP();


            // Создание нового графика и установка его в Frame
            GraphPage = new Graph("ИЗМЕНЕНИЕ ДАВЛЕНИЯ ВНУТРИ ТАНКЕРА", PressureInsideTanker.Pressure.ToArray(), "Время t, ч", "Давление, Па");


            GraphPageKp = new Graph("Коэффициент превышения", PressureInsideTanker.Kp.ToArray(), "Время t, ч", "Кп");
            GraphPageQgvs = new Graph("Расход газовоздушной смеси \n в трубопроводе газовой фазы", PressureInsideTanker.Qgvs.ToArray(), "Время t, ч", "Qгвс");

            InformationLoadingVolume = PressureInsideTanker.V.ToString();
            OnPropertyChanged(nameof(InformationLoadingVolume));
        }

        public string findT { get; set; }
        public string findQn { get; set; }
        public string findKp { get; set; }
        public string findQgvs { get; set; }
        public string findP { get; set; }
        private void FindPoints()
        {
            if (PressureInsideTanker.LoadingSchedule.Length < 2)
            {
                MessageBox.Show("Необходимо произвести расчет", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PressureInsideTanker.LoadingSchedule[0].X > double.Parse(findT))
            {
                MessageBox.Show("Введенное число должно быть больше начального времени налива ", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (PressureInsideTanker.LoadingSchedule[PressureInsideTanker.LoadingSchedule.Length-1].X < double.Parse(findT))
            {
                MessageBox.Show("Введенное число должно быть меньше конечного времени налива", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            double t = double.Parse(findT);
            findQn = Math.Round(FindPoint.Find(t, PressureInsideTanker.LoadingSchedule), 3).ToString();
            findKp = Math.Round(FindPoint.Find(t, PressureInsideTanker.Kp.ToArray()), 3).ToString();
            findQgvs = Math.Round(FindPoint.Find(t, PressureInsideTanker.Qgvs.ToArray()), 3).ToString();
            findP = Math.Round(FindPoint.Find(t, PressureInsideTanker.Pressure.ToArray()), 3).ToString();


            OnPropertyChanged(nameof(findQn));
            OnPropertyChanged(nameof(findKp));
            OnPropertyChanged(nameof(findQgvs));
            OnPropertyChanged(nameof(findP));
        }
        private void Save()
        {
            if (PressureInsideTanker.LoadingSchedule.Length < 2)
            {
                MessageBox.Show("Необходимо произвести расчет", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файлы Excel (*.xlsx)|*.xlsx";
            saveFileDialog.Title = "Выберите место для сохранения файла Excel";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    SaveToExcel(PressureInsideTanker.LoadingSchedule, PressureInsideTanker.Pressure.ToArray(), PressureInsideTanker.Kp.ToArray(), PressureInsideTanker.Qgvs.ToArray(), saveFileDialog.FileName);
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SaveToExcel(Point[] points, Point[] P, Point[] Kp, Point[] Qgvs, string filePath)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                // Добавьте новый лист в пакет
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Лист1");

                // Добавьте данные в ячейки
                worksheet.Cells["A1"].Value = "Время";
                worksheet.Cells["B1"].Value = "Давление";
                worksheet.Cells["C1"].Value = "Расход";
                worksheet.Cells["D1"].Value = "Коэффициент К";


                int i = 2;
                double k = points[0].X;
                do
                {
                    worksheet.Cells["A" + i.ToString()].Value = k;
                    worksheet.Cells["B" + i.ToString()].Value = Math.Round(FindPoint.Find(k, P), 3);
                    worksheet.Cells["C" + i.ToString()].Value = Math.Round(FindPoint.Find(k, Qgvs), 3);
                    worksheet.Cells["D" + i.ToString()].Value = Math.Round(FindPoint.Find(k, Kp), 3);
                    i++;
                    k += 0.25;
                } while (k < points[points.Length - 1].X);

                    // Сохраните пакет в файл
                    File.WriteAllBytes(filePath, package.GetAsByteArray());
            }

        }
    }
}
