using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using releasenotes.Dtos;
using releasenotes.Models;
using releasenotes.Services;
using Slugify;

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
        public async Task<Models.Project> Get(string id)
            => await _projects.Get(id);

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Dtos.Project dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            await _projects.Put(id, dto);

            return CreatedAtAction("Get", new { id });
        }

        [HttpPost] // POST auto generates slug, may return conflict
        public async Task<IActionResult> Post([FromBody] Dtos.Project dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            
            var id = new SlugHelper().GenerateSlug(dto.Name);

            // check if id exists
            if(await _projects.Get(id) != null) return StatusCode(409,
                $"A Project with id: {id} already exists. " +
                "Specify an explicit id to create a new Project, " +
                "or PUT a payload at the id to update the existing Project");

            await _projects.Put(id, dto);

            return CreatedAtAction("Get", new { id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _projects.Delete(id);
            return NoContent();
        }

        #endregion

        #region Releases

        [HttpPut("{id}/{release}")]
        public async Task<IActionResult> PutRelease(string id, string release, [FromBody] Release dto)
        {
            await _projects.PutRelease(id, release, dto);

            return CreatedAtAction("GetRelease", new { id, release });
        }

        [HttpGet("{id}/{release}")]
        public async Task<Entities.Release> GetRelease(string id, string release)
            => await _projects.GetRelease(id, release);

        [HttpDelete("{id}/{release}")]
        public async Task<IActionResult> DeleteRelease(string id, string release)
        {
            await _projects.DeleteRelease(id, release);
            return NoContent();
        }

        // TODO download notes as file? or let the frontend app do that?
        // would be easy in .NET

        #endregion
    }
}
