using X.PagedList;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Manage_CLB_HTSV.Extensions
{
    public static class PagedListExtensions
    {
        public static IHtmlContent PagedListPager<T>(this IHtmlHelper htmlHelper, IPagedList<T> list, 
            Func<int, string> generatePageUrl, 
            string containerDivClasses = "pagination-container",
            string ulElementClasses = "pagination",
            string liElementClasses = "page-item",
            string activeClass = "active",
            string disabledClass = "disabled")
        {
            if (list == null || list.PageCount <= 1)
                return new HtmlString("");

            var div = new TagBuilder("div");
            div.AddCssClass(containerDivClasses);

            var ul = new TagBuilder("ul");
            ul.AddCssClass(ulElementClasses);

            // Previous button
            var prevLi = new TagBuilder("li");
            prevLi.AddCssClass(liElementClasses);
            if (!list.HasPreviousPage)
            {
                prevLi.AddCssClass(disabledClass);
                var prevSpan = new TagBuilder("span");
                prevSpan.InnerHtml.Append("Previous");
                prevLi.InnerHtml.AppendHtml(prevSpan);
            }
            else
            {
                var prevA = new TagBuilder("a");
                prevA.Attributes["href"] = generatePageUrl(list.PageNumber - 1);
                prevA.InnerHtml.Append("Previous");
                prevLi.InnerHtml.AppendHtml(prevA);
            }
            ul.InnerHtml.AppendHtml(prevLi);

            // Page numbers
            for (int i = 1; i <= list.PageCount; i++)
            {
                var li = new TagBuilder("li");
                li.AddCssClass(liElementClasses);
                
                if (i == list.PageNumber)
                {
                    li.AddCssClass(activeClass);
                    var span = new TagBuilder("span");
                    span.InnerHtml.Append(i.ToString());
                    li.InnerHtml.AppendHtml(span);
                }
                else
                {
                    var a = new TagBuilder("a");
                    a.Attributes["href"] = generatePageUrl(i);
                    a.InnerHtml.Append(i.ToString());
                    li.InnerHtml.AppendHtml(a);
                }
                ul.InnerHtml.AppendHtml(li);
            }

            // Next button
            var nextLi = new TagBuilder("li");
            nextLi.AddCssClass(liElementClasses);
            if (!list.HasNextPage)
            {
                nextLi.AddCssClass(disabledClass);
                var nextSpan = new TagBuilder("span");
                nextSpan.InnerHtml.Append("Next");
                nextLi.InnerHtml.AppendHtml(nextSpan);
            }
            else
            {
                var nextA = new TagBuilder("a");
                nextA.Attributes["href"] = generatePageUrl(list.PageNumber + 1);
                nextA.InnerHtml.Append("Next");
                nextLi.InnerHtml.AppendHtml(nextA);
            }
            ul.InnerHtml.AppendHtml(nextLi);

            div.InnerHtml.AppendHtml(ul);
            return new HtmlString(div.ToString());
        }
    }
}