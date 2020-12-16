using System.Threading.Tasks;

namespace Weekday.Data.Interfaces
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync();
    }
}
