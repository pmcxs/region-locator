namespace RegionLocator.Core
{
    public interface IRegionsLookupHandler
    {
        Region GetRegion(double lon, double lat);
    }
}