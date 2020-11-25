namespace GokoSite.Data.Models.LoL
{
    using System;

    public class Region
    {
        public Region()
        {
            this.RegionId = Guid.NewGuid().ToString();
        }

        public string RegionId { get; set; }

        public string RegionName { get; set; }

        public int RiotRegionId { get; set; }
    }
}
