using CaseStudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;

namespace CaseStudy.DAL.DAO
{
    public class BrandDAO
    {
        private readonly AppDbContext _db;
        public BrandDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Brand>> GetAll()
        {
            return await _db.Brands!.ToListAsync();
        }
    }
}
