using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Расчет_процессов_газоотвода
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new CalcPressureInsideTankerViewModel();

        }

        private void checkBoxLoadingScheduleUnchecked(object sender, RoutedEventArgs e)
        {
            stackPanelLoadingSchedule.Visibility = Visibility.Collapsed;
        }
        private void checkBoxLoadingScheduleChecked(object sender, RoutedEventArgs e)
        {
            stackPanelLoadingSchedule.Visibility = Visibility.Visible;
        }

        private void checkBoxGasExhaustSystemUnchecked(object sender, RoutedEventArgs e)
        {
            stackPanelGasExhaustSystem.Visibility = Visibility.Collapsed;
        }
        private void checkBoxGasExhaustSystemChecked(object sender, RoutedEventArgs e)
        {
            stackPanelGasExhaustSystem.Visibility = Visibility.Visible;
        }

        private void checkBoxLiquidCargoUnchecked(object sender, RoutedEventArgs e)
        {
            stackPanelLiquidCargo.Visibility = Visibility.Collapsed;
        }
        private void checkBoxLiquidCargoChecked(object sender, RoutedEventArgs e)
        {
            stackPanelLiquidCargo.Visibility = Visibility.Visible;
        }
        private void buttonLoadingScheduleAddPoint_Click(object sender, RoutedEventArgs e)
        {
            TextBlock textBlockT = new();
            textBlockT.Text = "t ";
            // Создаем TextBox для ввода значения t
            TextBox textBoxT = new TextBox();
            textBoxT.Name = "textBoxT";
            textBoxT.Style = (Style)FindResource("TextBoxPointStyle");
            textBoxT.PreviewTextInput += TextBox_PreviewTextInput;
            TankerLoading.textBoxT.Add(textBoxT);

            TextBlock textBlockQ = new();
            textBlockQ.Text = "Q ";
            // Создаем TextBox для ввода значения Q
            TextBox textBoxQ = new TextBox();
            textBoxQ.Name = "textBoxQ";
            textBoxQ.Style = (Style)FindResource("TextBoxPointStyle");
            textBoxQ.PreviewTextInput += TextBox_PreviewTextInput;
            TankerLoading.textBoxQ.Add(textBoxQ);

            Button buttonDel = new();
            buttonDel.Content = "Удалить";
            buttonDel.Click += buttonDel_Click;

            StackPanel stackPanelPoint = new();
            stackPanelPoint.Style = (Style)FindResource("StackPanelinputStyle");
            stackPanelPoint.Orientation = Orientation.Horizontal;

            // Добавляем созданные TextBox в stackPanelPoint
            stackPanelPoint.Children.Add(textBlockT);
            stackPanelPoint.Children.Add(textBoxT);
            stackPanelPoint.Children.Add(textBlockQ);
            stackPanelPoint.Children.Add(textBoxQ);
            stackPanelPoint.Children.Add(buttonDel);

            stackPanelPoints.Children.Add(stackPanelPoint);

        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsPositiveNumber(e.Text))
            {
                e.Handled = true; 
            }
        }
        private bool IsPositiveNumber(string text)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d*\,?\d*$");
        }
        private void buttonDel_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            StackPanel stackPanelPoint = (StackPanel)button.Parent;

            TextBox textBoxToRemove = stackPanelPoint.Children.OfType<TextBox>().FirstOrDefault();

            if (textBoxToRemove != null)
            {
                TankerLoading.textBoxT.Remove(textBoxToRemove);
            }
            TextBox textBoxQToRemove = stackPanelPoint.Children.OfType<TextBox>().LastOrDefault();

            if (textBoxQToRemove != null)
            {
                TankerLoading.textBoxQ.Remove(textBoxQToRemove);
            }
            stackPanelPoints.Children.Remove(stackPanelPoint);
        }

    }
}
