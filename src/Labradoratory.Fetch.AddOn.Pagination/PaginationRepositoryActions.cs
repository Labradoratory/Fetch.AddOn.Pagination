using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public class PaginationRepositoryActions<TEntity> where TEntity : Entity, IPageable
    {
        private readonly ISupportsPagination<TEntity> _repository;
        private readonly Uri _baseUri;

        public PaginationRepositoryActions(ISupportsPagination<TEntity> repository, Uri baseUri)
        {
            _repository = repository;
            _baseUri = baseUri;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _repository.CountAsync(cancellationToken);
        }

        public Task<ResultPageWithNext<TEntity>> GetPageAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default)
        {
            var page = 0;
            if (httpRequest.Query.TryGetValue("page", out var values) && int.TryParse(values.First(), out var value))
                page = value;

            var pageSize = 100;
            if (httpRequest.Query.TryGetValue("pagesize", out values) && int.TryParse(values.First(), out value))
                pageSize = value;

            return GetPageAsync(page, pageSize, cancellationToken);
        }

        public async Task<ResultPageWithNext<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetPageAsync(page, pageSize, cancellationToken);
            return result.GetWithNext(_baseUri);
        }
    }
}
