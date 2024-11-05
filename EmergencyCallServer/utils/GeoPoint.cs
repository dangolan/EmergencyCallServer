namespace EmergencyCallServer.utils
{
    public class GeoPoint
    {
        public decimal Latitude { get; }
        public decimal Longitude { get; }

        public GeoPoint(decimal latitude, decimal longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
