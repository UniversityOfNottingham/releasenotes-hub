using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<object> Get()
        {
            return new[] {
                new {
                    Name = "Hello"
                },
                new {
                    Name = "Jon"
                }
            };
        }

        [HttpGet("{id}")]
        public object Get(string id)
        {
            return new
            {
                Name = "Hello"
            };
        }


        // also PUT at slug
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Project model)
        {
            model.Id = id;
            await _projects.Put(model);

            return CreatedAtAction("Get", new { id });
        }

        [HttpPost] // POST auto generates slug, may return conflict
        public void Post([FromBody]string value)
        {
            // TODO generate slug if none provided
        }

        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }

        #endregion
    }
}
