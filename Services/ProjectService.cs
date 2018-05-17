using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using releasenotes.Dtos;
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
            await _projects.ReplaceOneAsync(
                Builders<Project>.Filter.Eq(x => x.Id, model.Id),
                model,
                new UpdateOptions
                {
                    IsUpsert = true
                }
            );
        }

        public async Task<IEnumerable<ProjectSummary>> List()
            => await (_projects
                .Find(FilterDefinition<Project>.Empty)
                .SortByDescending(x => x.Name)
                .Project(x => new ProjectSummary
                {
                    Name = x.Name,
                    Id = x.Id,
                    ReleaseCount = x.Releases.Count,
                    LatestRelease = x.Releases
                        .OrderByDescending(y => y.Date)
                        .Select(y => new ReleaseSummary
                        {
                            Id = y.Id,
                            Date = y.Date,
                            href = ReleaseSummary.GenerateHref(x.Id, y.Id)
                        })
                        .FirstOrDefault()
                }))
                .ToListAsync();

        public async Task<ProjectDetails> Get(string id)
            => await _projects.Find(x => x.Id == id)
                .Project(x => new ProjectDetails
                {
                    Name = x.Name,
                    Id = x.Id,
                    Releases = x.Releases.Select(y => new ReleaseSummary
                    {
                        Id = y.Id,
                        Date = y.Date,
                        href = ReleaseSummary.GenerateHref(x.Id, y.Id)
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync();

        public async Task Delete(string id)
            => await _projects.DeleteOneAsync(x => x.Id == id);

        public async Task PutRelease(string id, Release model)
        {
            var p = await _projects.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (p == null) throw new KeyNotFoundException($"No project with id: {id}");

            var r = p.Releases.FirstOrDefault(x => x.Id == model.Id);

            if (r == null) // Create
            {
                await _projects.UpdateOneAsync(
                    Builders<Project>.Filter.Eq(x => x.Id, id),
                    Builders<Project>.Update.Push(x => x.Releases, model));
            }
            else // Update
            {
                await _projects.UpdateOneAsync(
                    Builders<Project>.Filter
                        .Where(x => x.Id == id && x.Releases.Any(y => y.Id == model.Id)),
                    Builders<Project>.Update.Set(x => x.Releases[-1], model));
            }
        }

        public async Task<Release> GetRelease(string id, string release)
            => (await _projects.Find(x => x.Id == id)
                .FirstOrDefaultAsync())?
                .Releases
                .FirstOrDefault(x => x.Id == release);

        public async Task DeleteRelease(string id, string release)
            => await _projects.UpdateOneAsync(
                Builders<Project>.Filter.Where(x => x.Id == id && x.Releases.Any(y => y.Id == release)),
                Builders<Project>.Update.Unset(x => x.Releases[-1]));
    }
}