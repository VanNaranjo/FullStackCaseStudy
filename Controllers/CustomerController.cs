using CaseStudy.DAL;
using CaseStudy.DAL.DAO;
using CaseStudy.DAL.DomainClasses;
using CaseStudy.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _ctx;
        readonly IConfiguration configuration;
        public CustomerController(AppDbContext context, IConfiguration config)
        {
            _ctx = context;
            this.configuration = config;
        }


        [HttpPost]
        [Route("Register")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<CustomerHelper>> Register(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? already = await dao.GetByEmail(helper.Email);
            if (already == null)
            {
                HashSalt hashSalt = GenerateSaltedHash(64, helper.Password!);
                helper.Password = ""; // don’t need the string anymore
                Customer dbCustomer = new()
                {
                    Firstname = helper.Firstname!,
                    Lastname = helper.Lastname!,
                    Email = helper.Email!,
                    Hash = hashSalt.Hash!,
                    Salt = hashSalt.Salt!
                };
                dbCustomer = await dao.Register(dbCustomer);
                if (dbCustomer.Id > 0)
                    helper.Token = "Customer registered";
                else
                    helper.Token = "Customer registration failed";
            }
            else
            {
                helper.Token = "Customer registration failed - email already in use";
            }
            return helper;
        }
        private static HashSalt GenerateSaltedHash(int size, string password)
        {
            var saltBytes = new byte[size];
            var provider = RandomNumberGenerator.Create();
            // Fills an array of bytes with a cryptographically strong sequence of random nonzero values.
            provider.GetNonZeroBytes(saltBytes);
            var salt = Convert.ToBase64String(saltBytes);
            // a password, salt, and iteration count, then generates a binary key
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 10000);
            var hashPassword = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            HashSalt hashSalt = new() { Hash = hashPassword, Salt = salt };
            return hashSalt;
        }

        [HttpPost]
        [Route("Login")]
        [Produces("application/json")]
        [AllowAnonymous]

        public async Task<ActionResult<CustomerHelper>> Login(CustomerHelper helper)
        {
            CustomerDAO dao = new(_ctx!);
            Customer? Customer = await dao.GetByEmail(helper.Email);
            if (Customer != null)
            {
                if (VerifyPassword(helper.Password, Customer.Hash!, Customer.Salt!))
                {
                    helper.Password = "";
                    var appSettings = configuration.GetSection("AppSettings").GetValue<string>("Secret");
                    // authentication successful so generate jwt token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(appSettings);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                    {
                         new Claim(ClaimTypes.Name, Customer.Id.ToString())
                     }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    string returnToken = tokenHandler.WriteToken(token);
                    helper.Token = returnToken;
                }
                else
                {
                    helper.Token = "Customername or password invalid - login failed";
                }
            }
            else
            {
                helper.Token = "Invalid Customer/Password";
            }
            return helper;
        }
        public static bool VerifyPassword(string? enteredPassword, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(enteredPassword!, saltBytes, 10000);
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == storedHash;
        }

    }

    public class HashSalt
    {
        public string? Hash { get; set; }
        public string? Salt { get; set; }
    }
}
