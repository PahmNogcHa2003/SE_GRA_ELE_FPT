
using SE_GRA_ELE_FPT_API.Services.Interfaces;
using SE_GRA_ELE_FPT_DBAccess.UnitOfWork;
using SE_GRA_ELE_FPT_Utilities;

namespace SE_GRA_ELE_FPT_API.Services.Bases
{
    public abstract class BaseService : DisposableObject, IBaseService
    {
        protected IConfiguration _configuration;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IUnitOfWork UnitOfWork { get; set; }

        private BaseService()
        {

        }

        protected BaseService(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }
        protected BaseService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.UnitOfWork = unitOfWork;

            this._httpContextAccessor = httpContextAccessor;
        }
        protected BaseService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            UnitOfWork = unitOfWork;

            this._configuration = configuration;
        }

        #region Dispose
        private bool _disposed;

        protected override void Dispose(bool isDisposing)
        {
            if (!this._disposed)
            {
                if (isDisposing)
                {
                    UnitOfWork = null;
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
