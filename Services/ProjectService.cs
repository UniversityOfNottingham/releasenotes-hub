using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using releasenotes.Models;

namespace releasenotes.Services
{
    public class ProjectService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Entities.Project> _projects;

        public ProjectService(
            IMapper mapper,
            MongoClient mongo,
            IConfiguration config)
        {
            _mapper = mapper;
            _config = config;
            _projects = mongo
                .GetDatabase(config["DatabaseName"])
                .GetCollection<Entities.Project>("projects");
        }

        public async Task Put(string id, Dtos.Project dto)
            => await _projects.ReplaceOneAsync(
                Builders<Entities.Project>.Filter.Eq(x => x.Id, id),
                _mapper.Map<Entities.Project>(dto, opts =>
                    opts.AfterMap((src, dest) => ((Entities.Project)dest).Id = id)),
                new UpdateOptions
                {
                    IsUpsert = true
                }
            );

        public async Task<IEnumerable<ProjectSummary>> List()
            => await (_projects
                .Find(FilterDefinition<Entities.Project>.Empty)
                .SortBy(x => x.Name)
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

        public async Task<Models.Project> Get(string id)
            => await _projects.Find(x => x.Id == id)
                .Project(x => new Models.Project
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

        public async Task PutRelease(string id, string release, Dtos.Release dto)
        {
            var p = await _projects.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (p == null) throw new KeyNotFoundException($"No project with id: {id}");

            var r = p.Releases.FirstOrDefault(x => x.Id == release);

            var entity = _mapper.Map<Entities.Release>(dto, opts =>
                opts.AfterMap((src, dest) => ((Entities.Release)dest).Id = release));

            if (r == null) // Create
            {
                await _projects.UpdateOneAsync(
                    Builders<Entities.Project>.Filter.Eq(x => x.Id, id),
                    Builders<Entities.Project>.Update.Push(x => x.Releases, entity));
            }
            else // Update
            {
                await _projects.UpdateOneAsync(
                    Builders<Entities.Project>.Filter
                        .Where(x => x.Id == id && x.Releases.Any(y => y.Id == entity.Id)),
                    Builders<Entities.Project>.Update.Set(x => x.Releases[-1], entity));
            }
        }

        public async Task<Entities.Release> GetRelease(string id, string release)
            => (await _projects.Find(x => x.Id == id)
                .FirstOrDefaultAsync())?
                .Releases
                .FirstOrDefault(x => x.Id == release);

        public async Task DeleteRelease(string id, string release)
            => await _projects.UpdateOneAsync(
                Builders<Entities.Project>.Filter.Where(x => x.Id == id && x.Releases.Any(y => y.Id == release)),
                Builders<Entities.Project>.Update.Unset(x => x.Releases[-1]));
    }
}