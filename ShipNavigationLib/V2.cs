using System.Diagnostics.CodeAnalysis;

namespace ShipNavigationLib
{
    /// <summary>
    /// Represents 2d point.
    /// </summary>
    public struct V2(double x1, double x2)
    {
        public double x1 = x1, x2 = x2;

        public override string? ToString()
        {
            return $"({x1};{x2})";
        }
    };
}