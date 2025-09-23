using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_GRA_ELE_FPT_DBAccess.Repositories.RepositoryBase
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
