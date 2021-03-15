using System;
using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public class ResultPageWithNext<TEntity> : ResultPage<TEntity>
    {
        public ResultPageWithNext(PageInfo pageInfo, IEnumerable<TEntity> results, Uri baseUri)
            : this(pageInfo.Page, pageInfo.PageSize, results, baseUri)
        { }

        public ResultPageWithNext(uint page, uint pageSize, IEnumerable<TEntity> results, Uri baseUri)
            : base(page, pageSize, results)
        {
            var query = System.Web.HttpUtility.ParseQueryString("");
            query["page"] = (page + 1).ToString();
            query["pagesize"] = pageSize.ToString();
            
            // If number of results is less than the 
            if (results.Count() < pageSize)
                return;

            var builder = new UriBuilder(baseUri);
            builder.Query = query.ToString();
            Next = new Uri(builder.ToString());
        }

        public Uri Next { get; }
    }
}
