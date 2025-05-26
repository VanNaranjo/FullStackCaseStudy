using CaseStudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;
using CaseStudy.DAL.DAO;

namespace CaseStudy.DAL.DAO
{
    public class ProductDAO
    {
        private readonly AppDbContext _db;
        public ProductDAO(AppDbContext ctx)
        {
            _db = ctx;
        }
        public async Task<List<Product>> GetAllByBrand(int id)
        {
            return await _db.Products!.Where(item => item.Brand!.Id == id).ToListAsync();
        }
    }
}
