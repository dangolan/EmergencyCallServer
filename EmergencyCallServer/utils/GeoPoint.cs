namespace EmergencyCallServer.utils
{
    public class GeoPoint
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public GeoPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
