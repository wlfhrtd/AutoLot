using AutoLot.Mvc.Controllers;
using AutoLot.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace AutoLot.Mvc.TagHelpers.Base
{
    public abstract class ItemLinkTagHelperBase : TagHelper
    {
        protected readonly IUrlHelper _urlHelper;


        protected ItemLinkTagHelperBase(IActionContextAccessor contextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _urlHelper = urlHelperFactory.GetUrlHelper(contextAccessor.ActionContext);
        }


        public int? ItemId { get; set; }


        protected void BuildContent(
            TagHelperOutput output,
            string actionName,
            string className,
            string displayText,
            string fontAwesomeName)
        {
            output.TagName = "a"; // replace <item-list> with <a> tag

            var target = (ItemId.HasValue)
                ? _urlHelper.Action(actionName, nameof(CarsController).CutController(), new { id = ItemId })
                : _urlHelper.Action(actionName, nameof(CarsController).CutController());

            output.Attributes.SetAttribute("href", target);
            output.Attributes.Add("class", className);
            output.Content.AppendHtml($@"{displayText} <i class=""fas fa-{fontAwesomeName}""></i>");
        }
    }
}
