namespace ShipNavigationLib
{
    /// <summary>
    /// Ship navigation problem implementation.
    /// </summary>
    public static class ShipNavigationProblem
    {
        private static readonly int _INIT_LIST_CAPACITY = 100;

        private static readonly Random _random = new Random();

        /// <summary>
        /// Calculates ship trajectory.
        /// Only ship is affected by stream.
        /// Start point is (0, 0).
        /// </summary>
        /// <param name="f"> Speed change function: s = s0 * f(x2). </param>
        /// <param name="s0"> Initial stream speed. </param>
        /// <param name="v"> Constant module of ship speed. </param>
        /// <param name="l"> Distance between start point and end point. </param>
        /// <param name="fi"> Angle between x1 axis and end point. </param>
        /// <param name="N"> Discretization parameter: tau = (l / v) / N. </param>
        /// <param name="K"> Additional iteration count: i < (N + K) && distance(x, dest) > e. </param>
        /// <param name="epsilon"> Arrival criteria: i < (N + K) && distance(x, dest) > e. </param>
        /// <returns>
        ///     Ship trajectory as TrajectoryInfo instance.
        /// </returns>
        public static TrajectoryInfo TrajectoryShip(Func<double, double> f, double s0, double v,
                                                    double l, double fi,
                                                    long N, long K, double epsilon)
        {
            List<V2> shipTrajectory = new(_INIT_LIST_CAPACITY);

            (double tau, double vtau, double vtau2) = _InitializeTauVtauVtau2(l, v, N);

            V2 destination = _V2FromPolar(l, fi);
            V2 p = new(0.0, 0.0);
            V2 u;

            shipTrajectory.Add(p);
            for (long i = 0; i < N + K && !_IsArrived(p, destination, epsilon); i++)
            {
                double langrandian =
                    Math.Sqrt(
                        Math.Pow(destination.x1 - p.x1 - s0 * f(p.x2) * tau, 2.0) +
                        Math.Pow(destination.x2 - p.x2, 2.0)
                    ) * vtau - vtau2;

                u.x1 = ((destination.x1 - p.x1 - s0 * f(p.x2) * tau) * vtau) / (langrandian + vtau2);
                u.x2 = ((destination.x2 - p.x2) * vtau) / (langrandian + vtau2);
 
                p.x1 = p.x1 + (s0 * f(p.x2) + v * u.x1) * tau;
                p.x2 = p.x2 + u.x2 * vtau;

                shipTrajectory.Add(p);
            }
            
            return new TrajectoryInfo(shipTrajectory, new List<V2> { destination, destination },
                                      tau, shipTrajectory.Count * tau);
        }

        /// <summary>
        /// Calculates ship trajectory.
        /// Both ship and destination are affected by stream.
        /// Destination is moving with speed of constant module,
        ///     direction is generated randomly in [aMin, aMax] for each time tick.
        /// Start point is (0, 0).
        /// </summary>
        /// <param name="f"> Speed change function: s = s0 * f(x2). </param>
        /// <param name="s0"> Initial stream speed. </param>
        /// <param name="vShip"> Constant module of ship speed. </param>
        /// <param name="l"> Distance between start point and end point. </param>
        /// <param name="fi"> Angle between x1 axis and end point. </param>
        /// <param name="N"> Discretization parameter: tau = (l / v) / N. </param>
        /// <param name="K"> Additional iteration count: i < (N + K) && distance(x, dest) > e. </param>
        /// <param name="epsilon"> Arrival criteria: i < (N + K) && distance(x, dest) > e. </param>
        /// <param name="vDestination"/> Constant module of destination speed. </param>
        /// <param name="aMin"/> Minimum angle between x1 axis and destination movement vector. </param>
        /// <param name="aMax"/> Maximum angle between x1 axis and destination movement vector. </param>
        /// <returns>
        ///     Ship trajectory and Destination trajectory as TrajectoryInfo instance.
        /// </returns>
        public static TrajectoryInfo TrajectoryShipAndDestination(Func<double, double> f, double s0, double vShip,
                                                                  double l, double fi,
                                                                  long N, long K, double epsilon,
                                                                  double vDestination, double aMin, double aMax)
        {
            List<V2> shipTrajectory = new(_INIT_LIST_CAPACITY);
            List<V2> destinationTrajectory = new(_INIT_LIST_CAPACITY);

            (double tau, double vtau, double vtau2) = _InitializeTauVtauVtau2(l, vShip, N);

            double a;
            double aRange = aMax - aMin;

            V2 destination = _V2FromPolar(l, fi);
            V2 p = new(0.0, 0.0);
            V2 u;

            shipTrajectory.Add(p);
            destinationTrajectory.Add(destination);

            for (long i = 0; i < N + K && !_IsArrived(p, destination, epsilon); i++)
            {
                a = aMin + _random.NextDouble() * aRange;
                destination.x1 = destination.x1 + (s0 * f(destination.x2) + vDestination * Math.Cos(a)) * tau;
                destination.x2 = destination.x2 + vDestination * Math.Sin(a) * tau;

                double langrandian =
                    Math.Sqrt(
                        Math.Pow(destination.x1 - p.x1 - s0 * f(p.x2) * tau, 2.0) +
                        Math.Pow(destination.x2 - p.x2, 2.0)
                    ) * vtau - vtau2;

                u.x1 = ((destination.x1 - p.x1 - s0 * f(p.x2) * tau) * vtau) / (langrandian + vtau2);
                u.x2 = ((destination.x2 - p.x2) * vtau) / (langrandian + vtau2);
 
                p.x1 = p.x1 + (s0 * f(p.x2) + vShip * u.x1) * tau;
                p.x2 = p.x2 + u.x2 * vtau;

                shipTrajectory.Add(p);
                destinationTrajectory.Add(destination);
            }

            return new TrajectoryInfo(shipTrajectory, destinationTrajectory,
                                      tau, shipTrajectory.Count * tau);
        }

        private static (double, double, double) _InitializeTauVtauVtau2(double l, double v, double N)
        {
            double tau = l / (v * N);
            double vtau = v * tau;
            double vtau2 = vtau * vtau;

            return (tau, vtau, vtau2);
        }

        private static V2 _V2FromPolar(double l, double fi)
        {
            return new V2(l * Math.Cos(fi), l * Math.Sin(fi));
        }

        /// <summary>
        /// Checks if distance between 2 points less or equal epsilon.
        /// </summary>
        /// <param name="a"> 1st point. </param>
        /// <param name="b"> 2nd point. </param>
        /// <param name="epsilon"> Accuracy parameter. </param>
        /// <returns> True if dist(a, b) <= epsilon, False otherwise. </returns>
        private static bool _IsArrived(V2 a, V2 b, double epsilon)
        {
            return Math.Sqrt(Math.Pow(b.x1 - a.x1, 2.0) + Math.Pow(b.x2 - a.x2, 2.0)) <= epsilon;
        }
    }
}
