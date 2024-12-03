using InternFselV2.Entities;
using InternFselV2.Repositories.IRepositories;

namespace InternFselV2.Repositories.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(InternV2DbContext internV2DbContext) : base(internV2DbContext)
        {
        }
    }
}
