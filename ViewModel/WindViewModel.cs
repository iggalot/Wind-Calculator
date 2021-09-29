using ASCE7_10Library;

namespace WindCalculator.ViewModel
{
    public class WindViewModel
    {
        public WindViewModel(BuildingInfo bldg, WindProvisions wind_prov)
        {
            Bldg = bldg;
            Wind_Prov = wind_prov;
        }

        public BuildingInfo Bldg { get; set; }
        public WindProvisions Wind_Prov { get; set; }
    }
}
