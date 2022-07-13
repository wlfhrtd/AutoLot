using AutoLot.Api.Controllers.Base;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Model.Entities;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc;


namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : BaseCrudController<Order, OrdersController>
    {
        public OrdersController(IOrderRepository orderRepository, IAppLogging<OrdersController> logger)
            : base(orderRepository, logger) { }
    }
}
