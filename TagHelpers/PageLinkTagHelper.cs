using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using FinalToDoList.ViewModels;

namespace FinalToDoList.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public PageViewModel PageModel { get; set; }
        public string PageAction { get; set; }
        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "div";

            TagBuilder tag = new TagBuilder("ul");
            tag.AddCssClass("pagination");

            if (PageModel.HasPreviousPage)
            {
                TagBuilder prevItem = CreateTag(PageModel.PageNumber - 1, urlHelper);
                tag.InnerHtml.AppendHtml(prevItem);
            }

            TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);
            tag.InnerHtml.AppendHtml(currentItem);

            if (PageModel.HasNextPage)
            {
                TagBuilder nextItem = CreateTag(PageModel.PageNumber + 1, urlHelper);
                tag.InnerHtml.AppendHtml(nextItem);
            }
            output.Content.AppendHtml(tag);
        }
        private TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new TagBuilder("li");
            TagBuilder link = new TagBuilder("a");

            if (pageNumber == this.PageModel.PageNumber)
                item.AddCssClass("active");
            else
                link.Attributes["href"] = urlHelper.Action(PageAction, new
                {
                    page = pageNumber
                });

            item.AddCssClass("page-item");
            item.AddCssClass("page-link");

            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);

            return item;
        }
    }
}
