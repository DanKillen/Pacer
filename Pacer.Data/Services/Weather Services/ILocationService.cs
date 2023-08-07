public interface ILocationService
{
    Task<IpStackResponse> GetLocationInfo(string ipAddress);
}
