using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoLot.Dal.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;


namespace AutoLot.Mvc.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly IMakeRepository _makeRepository;


        public MenuViewComponent(IMakeRepository makeRepository)
        {
            _makeRepository = makeRepository;
        }


        /*
         * put templates into any of these:
         * 
         * Views/<controller>/Components/<view_component_name>/<view_name>
           Views/Shared/Components/<view_component_name>/<view_name>
           Pages/Shared/Components/<view_component_name>/<view_name>
         * 
         */
        public IViewComponentResult Invoke()
        {
            var makes = _makeRepository.FindAll().ToList();

            return makes.Any() ? View("MenuView", makes) : new ContentViewComponentResult("Unable to get makes");
        }
    }
}
