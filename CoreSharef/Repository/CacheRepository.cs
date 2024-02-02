using CoreSharef.Models;
using MongoDB.Driver;

namespace CoreSharef.Repository;

public class CacheRepository
{
    private readonly IMongoCollection<Cache> _cache;
    
    public CacheRepository(string connectionName, string databaseName)
    {
        var client = new MongoClient(connectionName);
        
        var database = client.GetDatabase(databaseName);
        
        _cache = database.GetCollection<Cache>("Cache");
    }
    
    public void Insert(Cache cache)
    {
        _cache.InsertOne(cache);
    }
    
    public Cache GetCacheByCode(string code)
    {
        return _cache.Find(cache => cache.Code == code).FirstOrDefault();
    }
    
    public bool Delete(string code)
    {
        _cache.DeleteOne(cache => cache.Code == code);
        
        return true;
    }
}

