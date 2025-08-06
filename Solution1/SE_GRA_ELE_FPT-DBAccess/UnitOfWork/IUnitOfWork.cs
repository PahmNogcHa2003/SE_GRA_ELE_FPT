
namespace SE_GRA_ELE_FPT_DBAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveAsync();
      
        //IUserRepository User { get; }
    }
}
