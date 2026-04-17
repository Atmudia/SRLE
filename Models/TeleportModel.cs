using MonomiPark.SlimeRancher.Regions;

namespace SRLE.Models
{
    public class TeleportModel
    {
        public string Name { get; set; }
        public BuildObjectData.Vector3Save Position { get; set; }
        public BuildObjectData.Vector3Save Rotation { get; set; }
        public RegionRegistry.RegionSetId RegionSet { get; set; }
    }
}