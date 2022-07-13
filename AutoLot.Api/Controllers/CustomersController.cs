using AutoLot.Api.Controllers.Base;
using AutoLot.Model.Entities;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc;


namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")]
    public class CustomersController : BaseCrudController<Customer, CustomersController>
    {
        public CustomersController(ICustomerRepository customerRepository, IAppLogging<CustomersController> logger)
            : base(customerRepository, logger) { }
    }
}
