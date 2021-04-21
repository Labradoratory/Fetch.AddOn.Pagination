using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public static class PaginationRepositoryExtensions
    {
        /// <summary>
        /// Gets the total number of entities available.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="filter">[Optional] A query filter to apply when counting.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The total number of entities available.</returns>
        public static Task<int> CountAsync<TEntity>(
            this Repository<TEntity> repository,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null,
            CancellationToken cancellationToken = default)
                where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
                return sp.CountAsync(filter, cancellationToken);

            throw RepositoryNotExtended<TEntity>();
        }

        /// <summary>
        /// Gets the page of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="pageInfo">Info defining the page number and size.</param>
        /// <param name="filter">[Optional] A query filter to apply when counting.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The page of entities.</returns>
        public static Task<ResultPage<TEntity>> GetPageAsync<TEntity>(
            this Repository<TEntity> repository,
            PageInfo pageInfo,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null,
            CancellationToken cancellationToken = default)
                where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
                return sp.GetPageAsync(pageInfo, filter, cancellationToken);

            throw RepositoryNotExtended<TEntity>();
        }

        /// <summary>
        /// Gets the page of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filter">[Optional] A query filter to apply when counting.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The page of entities.</returns>
        public static async Task<ResultPageWithNext<TEntity>> GetPageWithNextAsync<TEntity>(
            this Repository<TEntity> repository,
            PageInfo pageInfo,
            Uri baseUri,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null,
            CancellationToken cancellationToken = default)
                where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
            {
                var result = await sp.GetPageAsync(pageInfo, filter, cancellationToken);
                return result.GetWithNext(baseUri);
            }

            throw RepositoryNotExtended<TEntity>();
        }

        /// <summary>
        /// Gets the page of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="filter">[Optional] A query filter to apply when counting.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The page of entities.</returns>
        public static async Task<ResultPageWithNext<TEntity>> GetPageWithNextAsync<TEntity>(
            this Repository<TEntity> repository,
            HttpRequest httpRequest,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> filter = null,
            CancellationToken cancellationToken = default)
                where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
            {
                int? page = null;
                if (httpRequest.Query.TryGetValue("page", out var values) && int.TryParse(values.First(), out var value))
                    page = value;

                int? pageSize = null;
                if (httpRequest.Query.TryGetValue("pagesize", out values) && int.TryParse(values.First(), out value))
                    pageSize = value;

                var result = await sp.GetPageAsync(new PageInfo(page, pageSize), filter, cancellationToken);
                return result.GetWithNext(httpRequest.GetBaseUri());
            }

            throw RepositoryNotExtended<TEntity>();
        }

        private static InvalidOperationException RepositoryNotExtended<TEntity>() where TEntity : IPageable
        {
            return new InvalidOperationException($"Repository does not implement {typeof(ISupportsPagination<TEntity>).Name}");
        }
    }
}
