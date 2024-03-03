namespace ShipNavigationLib
{
    /// <summary>
    /// Class - container of trajectory related values.
    /// </summary>
    public class TrajectoryInfo
    {
        private readonly IList<V2> shipTrajectory;
        private readonly IList<V2> destinationTrajectory;

        private readonly double tau;
        private readonly double totalTime;

        public TrajectoryInfo(IList<V2> shipTrajectory, IList<V2> destinationTrajectory,
                              double tau, double totalTime)
        {
            this.destinationTrajectory = destinationTrajectory;
            this.shipTrajectory = shipTrajectory;
            this.tau = tau;
            this.totalTime = totalTime;
        }

        public IList<V2> ShipTrajectory => shipTrajectory;
        public IList<V2> DestinationTrajectory => destinationTrajectory;
        public V2 ShipStart => shipTrajectory.First();
        public V2 ShipEnd => shipTrajectory.Last();
        public V2 DestinationStart => destinationTrajectory.First();
        public V2 DestinationEnd => destinationTrajectory.Last();

        public double Tau => tau;

        public double TotalTime => totalTime;
    }
}
