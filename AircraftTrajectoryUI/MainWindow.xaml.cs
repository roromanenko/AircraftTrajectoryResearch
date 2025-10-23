using System;
using System.Linq;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using AircraftTrajectoryResearch; // підключаємо твою бібліотеку

namespace AircraftTrajectoryUI
{
    public partial class MainWindow : Window
    {
        private SimulationResult? _result;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RunSimulation_Click(object sender, RoutedEventArgs e)
        {
            double w = double.Parse(WInput.Text);
            double NV = double.Parse(NVInput.Text);
            double Z0 = double.Parse(Z0Input.Text);
            int controlLawNumber = int.Parse(ControlLawInput.Text); 

            // Ініціалізація симуляції (приклад)
            var coeff = new CoefficientsCalculator();
            var parameters = new Params();
            var controlLaw = new CalculateControlLaw();

            var simulation = new Simulation(coeff, parameters, controlLaw);
            _result = simulation.Run(tEnd: 50, dt: 0.1, w: w, NV: NV, controlLawNumber: controlLawNumber, X0: 100, Z0: Z0);

            // показуємо вибір режиму (радіо-кнопки)
            ModeSelector.Visibility = Visibility.Visible;

            if (TableRadio.IsChecked == true)
                TableRadio_Checked(null, null);
            else if (GraphRadio.IsChecked == true)
                GraphRadio_Checked(null, null);
        }

        private void TableRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_result == null) return;

            // приховуємо графік
            TrajectoryPlot.Visibility = Visibility.Collapsed;

            // заповнюємо таблицю
            ResultsTable.ItemsSource = _result.Time
                .Select((t, i) => new
                {
                    Time = t,
                    X = _result.X[i],
                    Z = _result.Z[i],
                    Gamma = _result.Gamma[i],
                    Psi_g = _result.Psi_g[i],
                    G_p = _result.G_p[i]
                })
                .ToList();

            ResultsTable.Visibility = Visibility.Visible;
        }

        private void GraphRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (_result == null) return;

            // приховуємо таблицю
            ResultsTable.Visibility = Visibility.Collapsed;

            // створюємо графік
            var plotModel = new PlotModel { Title = "Траєкторія польоту" };
            var line = new LineSeries { Title = "Z(X)", StrokeThickness = 2 };

            for (int i = 0; i < _result.X.Count; i++)
                line.Points.Add(new DataPoint(_result.X[i], _result.Z[i]));

            plotModel.Series.Add(line);
            TrajectoryPlot.Model = plotModel;
            TrajectoryPlot.Visibility = Visibility.Visible;
        }
    }
}
