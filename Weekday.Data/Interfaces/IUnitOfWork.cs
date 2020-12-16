namespace Weekday.Data.Interfaces
{
    public interface IUnitOfWork
    {
        INewsRepository News { get; }

        int SaveChanges();
    }
}
