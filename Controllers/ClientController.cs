using System.Net.Mime;
using ApiDoc.Entities;
using ApiDoc.Services.Mock;
using ApiDoc.ViewModels;
using Bogus.DataSets;
using Microsoft.AspNetCore.Mvc;
using static Bogus.DataSets.Name;

namespace ApiDoc.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClientController : ControllerBase
    {
        private static ClientMock mockClients;
        private readonly ILogger<ClientController> _logger;

        public ClientController(ILogger<ClientController> logger)
        {
            _logger = logger;

            if (mockClients == null)
            {
                _logger.LogInformation("Creating mock data");
                mockClients = new ClientMock();
            }
        }

        /// <summary>
        /// Mostra todos usuários
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<IEnumerable<GetClient>> GetAll()
        {
            var clientsResponse = NoFilter();
            if (clientsResponse == null)
                return NoContent();

            return Ok(clientsResponse);
        }

        /// <summary>
        /// Mostra o usuário por id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<GetClient> GetById([FromRoute] int id)
        {
            if (id <= 0)
                return BadRequest();

            var clientResponse = FilterById(id);
            if (clientResponse == null)
                return NotFound();

            return Ok(clientResponse);
        }

        /// <summary>
        /// Mostra todos usários por busca
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        [HttpGet("search")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<GetClient>> GetByFilter(
            [FromQuery] string name = null,
            [FromQuery] Gender? gender = null)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!string.IsNullOrEmpty(name) && gender == null)
            {
                var clientsResponse = FilterByName(name);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }

            else if (string.IsNullOrEmpty(name) && gender != null)
            {
                var clientsResponse = FilterByGender(gender);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }

            else if (!string.IsNullOrEmpty(name) && gender != null)
            {
                var clientsResponse = FilterByNameAndGender(name, gender);
                if (clientsResponse == null)
                    NotFound();

                return Ok(clientsResponse);
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Cria usuário
        /// </summary>
        /// <param name="createClient"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Atualiza usuário
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateClient"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deleta usuário'
        /// </summary>
        /// <param name="id">Client id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        private IEnumerable<GetClient> NoFilter()
        {
            return from client in mockClients.Data
                   select new GetClient(
                       id: client.Id,
                       name: client.Name,
                       email: client.Email,
                       gender: client.Gender,
                       phone: client.Phone,
                       enabled: client.Enabled
                   );
        }
        private GetClient FilterById(int id)
        {
            return (from client in mockClients.Data
                    where client.Id.Equals(id)
                    select new GetClient(
                        id: client.Id,
                        name: client.Name,
                        email: client.Email,
                        gender: client.Gender,
                        phone: client.Phone,
                        enabled: client.Enabled
                    )).FirstOrDefault();
        }
        private IEnumerable<GetClient> FilterByName(string name)
        {
            return from client in mockClients.Data
                   where client.Name.Contains(name)
                   select new GetClient(
                       id: client.Id,
                       name: client.Name,
                       email: client.Email,
                       gender: client.Gender,
                       phone: client.Phone,
                       enabled: client.Enabled
                   );
        }
        private IEnumerable<GetClient> FilterByGender(Name.Gender? gender)
        {
            return from client in mockClients.Data
                   where client.Gender.Equals(gender)
                   select new GetClient(
                       id: client.Id,
                       name: client.Name,
                       email: client.Email,
                       gender: client.Gender,
                       phone: client.Phone,
                       enabled: client.Enabled
                   );
        }

        private object FilterByNameAndGender(string name, Name.Gender? gender)
        {
            return from client in mockClients.Data
                   where client.Name.Contains(name) && client.Gender.Equals(gender)
                   select new GetClient(
                       id: client.Id,
                       name: client.Name,
                       email: client.Email,
                       gender: client.Gender,
                       phone: client.Phone,
                       enabled: client.Enabled
                   );
        }
    }
}