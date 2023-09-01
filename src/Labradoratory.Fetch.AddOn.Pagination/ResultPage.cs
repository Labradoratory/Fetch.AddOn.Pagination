using System;
using System.Collections.Generic;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public class ResultPage<TEntity> : PageInfo
    {
        public ResultPage(PageInfo pageInfo, IEnumerable<TEntity> results)
            : this(pageInfo.Page, pageInfo.PageSize, results)
        {}

        public ResultPage(int page, int pageSize, IEnumerable<TEntity> results)
            : base(page, pageSize)
        {
            Results = results;
        }

        public IEnumerable<TEntity> Results { get; }

        public ResultPageWithNext<TEntity> GetWithNext(Uri baseUri)
        {
            return new ResultPageWithNext<TEntity>(Page, PageSize, Results, baseUri);
        }
    }
}
