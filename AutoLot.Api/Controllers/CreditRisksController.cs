using AutoLot.Api.Controllers.Base;
using AutoLot.Model.Entities;
using AutoLot.Dal.Repository.Interfaces;
using AutoLot.Services.Logging;
using Microsoft.AspNetCore.Mvc;


namespace AutoLot.Api.Controllers
{
    [Route("api/[controller]")]
    public class CreditRisksController : BaseCrudController<CreditRisk, CreditRisksController>
    {
        public CreditRisksController(ICreditRiskRepository creditRiskRepository, IAppLogging<CreditRisksController> logger)
            : base(creditRiskRepository, logger) { }
    }
}
