namespace ShipNavigationLib
{
    /// <summary>
    /// Ship navigation problem implementation.
    /// </summary>
    public static class ShipNavigationProblem
    {
        /// <summary>
        /// Calculates ship trajectory.
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
        ///     Ship trajectory as List of V2 values.
        ///     First vector is (0, 0).
        /// </returns>
        public static IList<V2> Trajectory(Func<double, double> f, double s0, double v,
                                           double l, double fi,
                                           long N, long K, double epsilon)
        {
            List<V2> result = new(100);

            double tau = l / (v * N);
            double vtau = v * tau;
            double vtau2 = vtau * vtau;

            V2 destination = new(l * Math.Cos(fi), l * Math.Sin(fi));
            V2 p = new(0.0, 0.0);
            V2 u;

            result.Add(p);
            for (long i = 0; i < N + K && !_IsArrived(p, destination, epsilon); i++)
            {
                double lg =
                    Math.Sqrt(
                        Math.Pow(destination.x1 - p.x1 - s0 * f(p.x2) * tau, 2.0) +
                        Math.Pow(destination.x2 - p.x2, 2.0)
                    ) * vtau - vtau2;

                u.x1 = ((destination.x1 - p.x1 - s0 * f(p.x2) * tau) * vtau) / (lg + vtau2);
                u.x2 = ((destination.x2 - p.x2) * vtau) / (lg + vtau2);
 
                p.x1 = p.x1 + (s0 * f(p.x2) + v * u.x1) * tau;
                p.x2 = p.x2 + u.x2 * vtau;

                result.Add(p);
            }
            
            return result;
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
