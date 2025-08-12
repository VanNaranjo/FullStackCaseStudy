using CaseStudy.DAL.DomainClasses;
using CaseStudy.Helpers;
using CaseStudy.DAL;
using CaseStudy.DAL.DAO;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;

namespace CaseStudy.DAL.DAO
{
    public class OrderDAO
    {
        private readonly AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<int> AddOrder(int customerid, OrderSelectionHelper[] selections)
        {
            int orderId = -1;
            using (var _trans = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    Order order = new();
                    order.CustomerId = customerid;
                    order.OrderDate = System.DateTime.Now;
                    order.OrderAmount = 0;

                    foreach(OrderSelectionHelper selection in selections)
                    {
                        order.OrderAmount += selection.Item!.MSRP * selection.Qty;
                    }

                    await _db.Orders!.AddAsync(order);
                    await _db.SaveChangesAsync();

                    foreach (OrderSelectionHelper selection in selections)
                    {
                        OrderLineItem lineItem = new();
                        lineItem.OrderId = order.Id;
                        lineItem.ProductId = selection.Item!.Id;
                        lineItem.QtyOrdered = selection.Qty;
                        
                        if(lineItem.QtyOrdered < selection.Item.QtyOnHand)
                        {
                            selection.Item.QtyOnHand -= selection.Qty;
                            lineItem.QtySold = selection.Qty;
                            lineItem.QtyOrdered = selection.Qty;
                            lineItem.QtyBackOrdered = 0;
                        }
                        else if (selection.Qty > selection.Item.QtyOnHand)
                        {
                            int backOrderQty = selection.Qty - selection.Item.QtyOnHand;

                            selection.Item.QtyOnHand = 0;
                            selection.Item.QtyOnBackOrder += backOrderQty;

                            lineItem.QtySold = selection.Item.QtyOnHand;
                            lineItem.QtyOrdered = selection.Qty;
                            lineItem.QtyBackOrdered = backOrderQty;
                        }
                        lineItem.SellingPrice = Convert.ToDecimal(selection.Item!.MSRP) * lineItem.QtySold;

                        await _db.LineItems!.AddAsync(lineItem);
                        await _db.SaveChangesAsync();
                    }

                    await _trans.CommitAsync();
                    orderId = order.Id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await _trans.RollbackAsync();
                }
            }
            return orderId;
        }

        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders!.Where(order => order.CustomerId == id).ToListAsync<Order>();
        }

        public async Task<List<OrderDetailsHelper>> GetOrderDetails(int tid, string email)
        {
            Customer? customer = _db.Customers!.FirstOrDefault(customer => customer.Email == email);
            List<OrderDetailsHelper> allDetails = new();
            // LINQ way of doing INNER JOINS
            var results = from t in _db.Orders
                          join ti in _db.LineItems! on t.Id equals ti.OrderId
                          join mi in _db.Products! on ti.ProductId equals mi.Id
                          where (t.CustomerId == customer!.Id && t.Id == tid)
                          select new OrderDetailsHelper
                          {
                              ProductName = mi.ProductName,
                              QtySold = ti.QtySold,
                              QtyOrdered = ti.QtyOrdered,
                              QtyBackOrdered = ti.QtyBackOrdered,
                              SellingPrice = ti.SellingPrice
                          };
            allDetails = await results.ToListAsync();
            return allDetails;
        }
    }
}
