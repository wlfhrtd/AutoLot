using AutoLot.Api.Controllers.Base;
using AutoLot.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Services.Logging;


namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")]
    public class MakesController : BaseCrudController<Make, MakesController>
    {
        public MakesController(IMakeRepository makeRepository, IAppLogging<MakesController> logger)
            : base(makeRepository, logger) { }
    }
}
