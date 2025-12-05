namespace RedisSample.Controllers
{
    public interface IRedisCacheProvider : IDistributedCacheProvider
    {

    }
}



public interface IDistributedCacheProvider : ICacheProvider
{

}

//$env:
//ASPNETCORE_URLS="http://localhost:5002;https://localhost:5003"
//    .\RedisSample.exe

public interface ICacheProvider
{
    Task<T> GetAsync<T> ( string key );
    Task SetAsync<T> ( string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null );

    Task RemoveAsync ( string key );

    T Get<T> ( string key );
    void Set<T> ( string key, T value, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpiration = null );
    void Remove ( string key );

}