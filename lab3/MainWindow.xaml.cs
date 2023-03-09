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
using System.ComponentModel;


namespace lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker Back;
        Calculator calculator;

        public MainWindow()
        {
            InitializeComponent();

            Back = new BackgroundWorker();
        }



        private void BtnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (Back.IsBusy)
            {
                if (MessageBox.Show("Прервать вычисление?", "Внимание!",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Question,
                                   MessageBoxResult.Yes,
                                   MessageBoxOptions.ServiceNotification) != MessageBoxResult.Yes)
                {
                    return;
                }
                else Back.CancelAsync();
            }
            else
            {
                ParamStruct param = new ParamStruct();
                try
                {
                    param.X0 = Convert.ToInt32(TbX0.Text);
                    param.XIterCount = Convert.ToInt32(TbIterCount.Text);
                    param.Incr = Convert.ToInt32(TbIncr.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                Back = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };
                Back.DoWork += Back_DoWork;
                Back.ProgressChanged += Back_ProgressChanged;
                Back.RunWorkerCompleted += Back_RunWorkerCompleted;

                calculator = new Calculator(param, Back);
                if (!calculator.GetSuccess())
                {
                    MessageBox.Show(calculator.GetException().Message);
                    return;
                }

                Back.RunWorkerAsync(param);
                BtnCalculate.Content = "Прервать";
                DGResult.HeadersVisibility = DataGridHeadersVisibility.All;

            }

        }



        private void Back_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!calculator.GetSuccess())
                    return;
                calculator.Calculate();

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        private void Back_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            FillTable((ResultStruct[])e.UserState);
        }

        private void Back_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FillTable(calculator.GetResults());
            BtnCalculate.Content = "Рассчитать";

            Exception ex = calculator.GetException();
            if (ex != null)
                MessageBox.Show(ex.Message);
        }


        private void FillTable(ResultStruct[] results)
        {
            if (calculator == null)
                return;

            var t = results.Select(x => new
            {
                X = x.X,
                F1 = x.F1x,
                F2 = x.F2x,
                F3 = x.F3x,
                F4 = x.F4x,
                F = x.Fx()
            });

            DGResult.ItemsSource = t;

            //для переименования колонок
            //DGResult.Columns[0].Header = "Название кадра";
            //DGResult.Columns[1].Header = "Количество кадров";
            //DGResult.Columns[2].Header = "Ошибок нумерации";
            //DGResult.Columns[3].Header = "Ошибок CRC";
        }
    }
}
