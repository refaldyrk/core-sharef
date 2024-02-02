using MongoDB.Driver;
using File = CoreSharef.Models.File;

namespace CoreSharef.Repository;

public class FileRepository
{
    private readonly IMongoCollection<File> _file;

    public FileRepository(string connectionName, string databaseName)
    {
        var client = new MongoClient(connectionName);

        var database = client.GetDatabase("Core");

        _file = database.GetCollection<File>("File");
    }

    public List<File> GetList()
    {
       return _file.Find(file => true).ToList();
    }

    public File GetFileByCode(string code)
    {
        return _file.Find(file => file.Code == code).FirstOrDefault();
    }
    
    public File GetFileByName(string name)
    {
        return _file.Find(file => file.Name == name).FirstOrDefault();
    }
    
    public File Insert(File file)
    {
        _file.InsertOne(file);
        return file;
    }
    
    public Boolean Delete(string code)
    {
        _file.DeleteOne(file => file.Code == code);
        
        return true;
    }
}