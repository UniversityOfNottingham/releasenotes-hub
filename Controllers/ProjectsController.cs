using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using releasenotes.Dtos;
using releasenotes.Models;
using releasenotes.Services;

namespace releasenotes.Controllers
{
    [Route("api/[controller]")]
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projects;

        public ProjectsController(ProjectService projects)
        {
            _projects = projects;
        }

        #region Projects

        [HttpGet]
        public async Task<IEnumerable<ProjectSummary>> List()
            => await _projects.List();

        [HttpGet("{id}")]
        public async Task<ProjectDetails> Get(string id)
            => await _projects.Get(id);

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Project model)
        {
            model.Id = id;
            await _projects.Put(model);

            return CreatedAtAction("Get", new { id });
        }

        // TODO [HttpPost] // POST auto generates slug, may return conflict
        // public void Post([FromBody]string value)
        // {
        //     // TODO generate slug if none provided
        // }

        // TODO [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }

        #endregion

        #region Releases

        [HttpPut("{id}/{release}")]
        public async Task<IActionResult> PutRelease(string id, string release, [FromBody] Release model)
        {
            model.Id = release;
            await _projects.PutRelease(id, model);

            return CreatedAtAction("GetRelease", new { id, release });
        }

        [HttpGet("{id}/{release}")]
        public async Task<Release> GetRelease(string id, string release)
            => await _projects.GetRelease(id, release);

        // TODO Delete

        // TODO download notes as file? or let the frontend app do that?
        // would be easy in .NET

        #endregion
    }
}
