using ShipNavigationLib;

namespace Playground
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrajectoryInfo trajectoryInfo = ShipNavigationProblem.TrajectoryShipAndDestination(x => 1.0, 0.0, 2.0, 10, Math.PI / 3, 1000, 1, 0.01, 0.0, 0, 0.02);
            Console.WriteLine(trajectoryInfo.ShipTrajectory.Count);
            foreach (var item in trajectoryInfo.ShipTrajectory)
            {
                Console.WriteLine(item.x1.ToString() + " " + item.x2.ToString());
            }
        }
    }
}
