using System;
using System.Collections.Generic;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public class ResultPage<TEntity>
    {
        public ResultPage(int page, int pageSize, IEnumerable<TEntity> results)
        {
            Page = page;
            PageSize = pageSize;
            Results = results;
        }

        public int Page { get; }
        public int PageSize { get; }

        public IEnumerable<TEntity> Results { get; }

        public ResultPageWithNext<TEntity> GetWithNext(Uri baseUri)
        {
            return new ResultPageWithNext<TEntity>(Page, PageSize, Results, baseUri);
        }
    }
}
