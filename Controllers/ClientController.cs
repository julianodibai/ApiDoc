
using System.Net.Mime;
using ApiDoc.Entities;
using ApiDoc.Mock;
using ApiDoc.Services.Filters;
using ApiDoc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Bogus.DataSets.Name;

namespace ApiDoc.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IFilters _filters;
        private static ClientMock mockClients;
        private readonly ILogger<Filters> _logger;

        public ClientController(ILogger<Filters> logger, IFilters filters)
        {
            _logger = logger;
            _filters = filters;

            if (mockClients == null)
            {
                _logger.LogInformation("Creating mock data");
                mockClients = new ClientMock();
            }
        }

        [HttpGet]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<GetClient>> GetAll()
        {
            var clientsResponse = _filters.NoFilter();
            if (clientsResponse == null)
                return NoContent();

            return Ok(clientsResponse);
        }

        [HttpGet("{id}")]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetClient> GetById([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest();

            var clientResponse = _filters.FilterById(id);
            if (clientResponse == null)
                return NotFound();

            return Ok(clientResponse);
        }

        [HttpGet("search")]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetClient>> GetByFilter(
            [FromQuery] string name = null,
            [FromQuery] Gender? gender = null)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!string.IsNullOrEmpty(name) && gender == null)
            {
                var clientsResponse = _filters.FilterByName(name);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }

            else if (string.IsNullOrEmpty(name) && gender != null)
            {
                var clientsResponse = _filters.FilterByGender(gender);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }

            else if (!string.IsNullOrEmpty(name) && gender != null)
            {
                var clientsResponse = _filters.FilterByNameAndGender(name, gender);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }
            else
                return BadRequest();
        }
        
        [HttpPost]
        //[Consumes(MediaTypeNames.Application.Json)]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<int> Post([FromBody] CreateClient createClient)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var nextId = mockClients.Data.Max(c => c.Id) + 1;

            var client = new Client(
                id: nextId,
                name: createClient.Name,
                email: createClient.Email,
                gender: createClient.Gender,
                phone: createClient.Phone);

            mockClients.Add(client);

            return CreatedAtAction(nameof(Post), new { Id = client.Id });
        }

        [HttpPut("{id}")]
        //[Consumes(MediaTypeNames.Application.Json)]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> Put(
                                    [FromRoute] int id,
                                    [FromBody] UpdateClient updateClient)
        {
            if (!ModelState.IsValid || id != updateClient.Id)
                return BadRequest();

            var client = mockClients.GetById(id);
            if (client == null)
                return NotFound();

            client.Name = updateClient.Name;
            client.Email = updateClient.Email;
            client.Gender = updateClient.Gender;
            client.Phone = updateClient.Phone;

            mockClients.Update(client);

            return Ok(new { Id = client.Id });
        }

        [HttpPatch("{id}")]
        //[Produces(MediaTypeNames.Application.Json)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> Patch(
                                    [FromRoute] int id,
                                    [FromQuery] bool enabled)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var client = mockClients.GetById(id);

            if (client == null)
                return NotFound();

            client.Enabled = enabled;

            mockClients.Update(client);

            return Ok(new { Id = client.Id });
        }

        [HttpDelete("{id}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var client = mockClients.GetById(id);
            if (client == null)
                return NotFound();

            mockClients.Remove(id);

            return NoContent();
        }

    }
}