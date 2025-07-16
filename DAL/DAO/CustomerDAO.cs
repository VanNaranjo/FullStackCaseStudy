using CaseStudy.DAL.DomainClasses;
using CaseStudy.DAL;
using Microsoft.EntityFrameworkCore;

namespace CaseStudy.DAL.DAO
{
    public class CustomerDAO
    {
        private readonly AppDbContext _db;
        public CustomerDAO(AppDbContext _ctx)
        {
            _db = _ctx;
        }
        public async Task<Customer> Register(Customer Customer)
        {
            await _db.Customers!.AddAsync(Customer);
            await _db.SaveChangesAsync();
            return Customer;
        }
        public async Task<Customer?> GetByEmail(string? email)
        {
            Customer? Customer = await _db.Customers!.FirstOrDefaultAsync(u => u.Email == email);
            return Customer;
        }

    }
}
