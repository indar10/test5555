export class MapConstants {
  public static readonly DEFAULT_MAP_NAME = "Location A";
  public static readonly DEFAULT_COUNTRY = "US";
  public static readonly DEFAULT_US_LAT = 37.0902;
  public static readonly DEFAULT_US_LNG = -95.7129;

  public static toMiles(radInMeters: number) {
    return (radInMeters * 0.000621371192).toFixed(2);
  }

  public static toMeters(radInMiles: number) {
    return (radInMiles / 0.000621371192).toFixed(2);
  }
}
