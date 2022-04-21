using ApiDoc.Mock;
using ApiDoc.ViewModels;
using Bogus.DataSets;

namespace ApiDoc.Services.Filters
{
    public class Filters : IFilters
    {
        private static ClientMock mockClients;
        private readonly ILogger<Filters> _logger;

        public Filters(ILogger<Filters> logger)
        {
            _logger = logger;

            if (mockClients == null)
            {
                _logger.LogInformation("Creating mock data");
                mockClients = new ClientMock();
            }
          
        }

        public IEnumerable<GetClient> NoFilter()
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
        public GetClient FilterById(int id)
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
        public IEnumerable<GetClient> FilterByName(string name)
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
        public IEnumerable<GetClient> FilterByGender(Name.Gender? gender)
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

        public object FilterByNameAndGender(string name, Name.Gender? gender)
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