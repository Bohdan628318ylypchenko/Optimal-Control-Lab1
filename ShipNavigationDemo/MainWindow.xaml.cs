using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ScottPlot;
using ShipNavigationLib;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ShipNavigationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _runCount = 0;

        private static readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void TrajectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            Func<double, double> f;
            double s0, v, l, fi, epsilon, vDest, aMin, aMax;
            long N, K;
            try
            {
                f = CreateFunctionXFromString(FunctionTextBox.Text);
                s0 = double.Parse(S0TextBox.Text);
                v = double.Parse(VShipTextBox.Text);
                l = double.Parse(LTextBox.Text);
                fi = double.Parse(FiTextBox.Text);
                epsilon = double.Parse(EpsilonTextBox.Text);
                N = long.Parse(NTextBox.Text);
                K = long.Parse(KTextBox.Text);

                TrajectoryInfo result;

                if ((bool)StreamOnDestinationCheckBox.IsChecked)
                {
                    vDest = double.Parse(VDestTextBox.Text);
                    aMin = double.Parse(AMinTextBox.Text);
                    aMax = double.Parse(AMaxTextBox.Text);
                    if (aMax < aMin)
                    {
                        throw new Exception();
                    }
                    result = ShipNavigationProblem.TrajectoryShipAndDestination(f, s0, v, l, fi,
                                                                                N, K, epsilon,
                                                                                vDest, aMin, aMax);
                }
                else
                {
                    result = ShipNavigationProblem.TrajectoryShip(f, s0, v, l, fi,
                                                                  N, K, epsilon);
                }

                Color shipColor = _randomColor();
                Color destinationColor = _randomColor();
                double[] sX1 = result.ShipTrajectory.Select(v => v.x1).ToArray();
                double[] sX2 = result.ShipTrajectory.Select(v => v.x2).ToArray();
                Plot.Plot.Add.Scatter(sX1, sX2, shipColor);
                double[] dX1 = result.DestinationTrajectory.Select(v => v.x1).ToArray();
                double[] dX2 = result.DestinationTrajectory.Select(v => v.x2).ToArray();
                Plot.Plot.Add.Scatter(dX1, dX2, destinationColor);
                Plot.Plot.Add.HorizontalLine(0);
                Plot.Plot.Add.VerticalLine(0);
                LegendItem shipLegend = new()
                {
                    LineColor = shipColor,
                    LineWidth = 2,
                    Label = $"""Ship #{_runCount}"""
                };
                LegendItem destinationLegend = new()
                {
                    LineColor = destinationColor,
                    LineWidth = 2,
                    Label = $"""Destination #{_runCount}"""
                };
                Plot.Plot.Legend.IsVisible = true;
                Plot.Plot.Legend.ManualItems.AddRange(new[] { shipLegend, destinationLegend });
                Plot.Refresh();

                RunTextBox.Text += $"""
                                    ==========> Run {_runCount} ================
                                    input parameters:
                                        f       = {FunctionTextBox.Text}
                                        s0      = {S0TextBox.Text}
                                        vShip   = {VShipTextBox.Text}
                                        l       = {LTextBox.Text}
                                        fi      = {FiTextBox.Text}
                                        epsilon = {EpsilonTextBox.Text}
                                        N       = {NTextBox.Text}
                                        K       = {KTextBox.Text}
                                        vDest   = {VDestTextBox.Text}
                                        aMin    = {AMinTextBox.Text}
                                        aMax    = {AMaxTextBox.Text}
                                    trajectory:
                                        ship trajectory start        = {result.ShipStart}
                                        ship trajectory end          = {result.ShipEnd}
                                        destination trajectory start = {result.DestinationStart}
                                        destination trajectory end   = {result.DestinationEnd}
                                        tau                          = {result.Tau}
                                        total time                   = {result.TotalTime}
                                    ============================================
                                    """;
                RunTextBox.Text += "\n\n";

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"""==========> Run {_runCount} ================""");
                stringBuilder.Append('\n');
                foreach(V2 v2 in result.ShipTrajectory)
                {
                    stringBuilder.Append(v2);
                    stringBuilder.Append('\n');
                }
                stringBuilder.Append("============================================\n\n");
                TrajectoryTextBox.Text += stringBuilder.ToString();

                _runCount++;
            }
            catch
            {
                MessageBox.Show("Something went wrong, check input.");
            }

            Mouse.OverrideCursor = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Plot.Plot.Clear();
            Plot.Plot.Legend.ManualItems.Clear();
            Plot.Refresh();
            RunTextBox.Text = TrajectoryTextBox.Text = "";
        }

        private Func<double, double> CreateFunctionXFromString(string fSrc)
        {
            var str = "(x) => " + fSrc;
            var options = ScriptOptions.Default.AddImports(new string[] { "System" });
            return CSharpScript.EvaluateAsync<Func<double, double>>(str, options).Result;
        }

        private void StreamOnDestinationCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AMinTextBox.IsEnabled = AMaxTextBox.IsEnabled = VDestTextBox.IsEnabled = true;
        }

        private void StreamOnDestinationCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AMinTextBox.IsEnabled = AMaxTextBox.IsEnabled = VDestTextBox.IsEnabled = false;
        }

        private Color _randomColor()
        {
            return Color.FromARGB((uint)(8128 + _random.Next()));
        }
    }
}