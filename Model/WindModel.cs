using ASCE7_10Library;

namespace WindCalculator.Model
{
    public class WindModel
    {
        public WindProvisions EAST_CaseA { get; set; }
        public WindProvisions EAST_CaseB { get; set; }
        public WindProvisions WEST_CaseA { get; set; }
        public WindProvisions WEST_CaseB { get; set; }
        public WindProvisions NORTH_CaseA { get; set; }
        public WindProvisions NORTH_CaseB { get; set; }
        public WindProvisions SOUTH_CaseA { get; set; }
        public WindProvisions SOUTH_CaseB { get; set; }
        public WindModel(double speed, BuildingInfo bldg,  WindOrientations orientations, ExposureCategories exposure, double gust_factor)
        {
//            EAST_CaseA = new WindProvisions()
        }
    }
}
