using ShipNavigationLib;

namespace Playground
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IList<V2> trajectory = ShipNavigationProblem.Trajectory(x => 1.0, 1.0, 2.0, 10, Math.PI / 3, 1000, 1, 0.01);
            Console.WriteLine(trajectory.Count);
            foreach (var item in trajectory)
            {
                Console.WriteLine(item.x1.ToString() + " " + item.x2.ToString());
            }
        }
    }
}
