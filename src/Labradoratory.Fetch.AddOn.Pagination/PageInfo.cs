using System;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    /// <summary>
    /// Represents a page number and size.
    /// </summary>
    public class PageInfo
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 100;

        public PageInfo(int? page = null, int? pageSize = null)
        {
            if(page == null || page == 0)
                Page = DefaultPage;
            else
                Page = page.Value;

            if(pageSize == null || pageSize == 0)
                PageSize = DefaultPageSize;
            else
                PageSize = pageSize.Value;
        }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        public int PageSize { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            if (!(obj is PageInfo pi))
                return false;

            return Page == pi.Page && PageSize == pi.PageSize;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Page, PageSize);
        }
    }
}
