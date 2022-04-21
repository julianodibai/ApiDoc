using ApiDoc.ViewModels;
using static Bogus.DataSets.Name;

namespace ApiDoc.Services.Filters
{
    public interface IFilters
    {
        IEnumerable<GetClient> NoFilter();
        GetClient FilterById(int id);
        IEnumerable<GetClient> FilterByName(string name);
        IEnumerable<GetClient> FilterByGender(Gender? gender);
        object FilterByNameAndGender(string name, Gender? gender);
    }
}