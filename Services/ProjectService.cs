using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using releasenotes.Models;

namespace releasenotes.Services
{
    public class ProjectService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Project> _projects;

        public ProjectService(
            MongoClient mongo,
            IConfiguration config)
        {
            _config = config;
            _projects = mongo
                .GetDatabase(config["DatabaseName"])
                .GetCollection<Project>("projects");
        }

        public async Task Put(Project model)
        {
            // TODO non-exception error handling?
            await _projects.ReplaceOneAsync(
                Builders<Project>.Filter.Eq(x => x.Id, model.Id),
                model,
                new UpdateOptions {
                    IsUpsert = true
                }
            );
        }

        public async Task<Project> Get(string id)
            => await _projects.Find(x => x.Id == id)
                .FirstOrDefaultAsync();
    }
}