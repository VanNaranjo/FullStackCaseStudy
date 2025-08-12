using CaseStudy.DAL;
using CaseStudy.DAL.DAO;
using CaseStudy.DAL.DomainClasses;
using CaseStudy.Helpers;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly AppDbContext? _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal;
            try
            {
                CustomerDAO cDao = new(_ctx!);
                Customer? orderOwner = await cDao.GetByEmail(helper.Email);
                OrderDAO oDao = new(_ctx!);
                int orderId = await oDao.AddOrder(orderOwner!.Id, helper.Selections!);
                retVal = orderId > 0 ? "Order " + orderId + " saved!" : "Order not saved";
            }
            catch (Exception ex)
            {
                retVal = "Order not saved " + ex.Message;
            }
            return retVal;
        }

        [Route("{email}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> orders;
            CustomerDAO uDao = new(_ctx!);
            Customer? orderOwner = await uDao.GetByEmail(email);
            OrderDAO tDao = new(_ctx!);
            orders = await tDao.GetAll(orderOwner!.Id);
            return orders;
        }

        [Route("{orderid}/{email}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetOrderDetails(int orderid, string email)
        {
            OrderDAO dao = new(_ctx!);
            return await dao.GetOrderDetails(orderid, email);
        }
    }
}
