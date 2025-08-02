using MonomiPark.SlimeRancher.Regions;

namespace SRLE.Models
{
    public class TeleportModel
    {
        public string Name { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public RegionRegistry.RegionSetId RegionSet { get; set; }
    }
}