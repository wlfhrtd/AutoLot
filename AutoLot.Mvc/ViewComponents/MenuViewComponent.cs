using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoLot.Dal.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using AutoLot.Services.ApiWrapper;


namespace AutoLot.Mvc.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IApiServiceWrapper _serviceWrapper;


        public MenuViewComponent(IApiServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }


        /*
         * put templates into any of these:
         * 
         * Views/<controller>/Components/<view_component_name>/<view_name>
           Views/Shared/Components/<view_component_name>/<view_name>
           Pages/Shared/Components/<view_component_name>/<view_name>
         * 
         */

        /*
        public IViewComponentResult Invoke()
        {
            var makes = _serviceWrapper.GetMakesAsync();
            return makes.Any() ? View("MenuView", makes) : new ContentViewComponentResult("Unable to get makes");
        }
        */

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var makes = await _serviceWrapper.GetMakesAsync();
            return makes == null ? new ContentViewComponentResult("Unable to get makes") : View("MenuView", makes);
        }
    }
}
