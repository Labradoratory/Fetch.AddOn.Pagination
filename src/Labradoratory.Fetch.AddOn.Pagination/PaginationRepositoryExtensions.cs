using System;
using System.Threading;
using System.Threading.Tasks;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public static class PaginationRepositoryExtensions
    {
        /// <summary>
        /// Gets the total number of entities available.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The total number of entities available.</returns>
        public static Task<int> CountAsync<TEntity>(this Repository<TEntity> repository, CancellationToken cancellationToken = default) where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
                return sp.CountAsync(cancellationToken);

            throw RepositoryNotExtended<TEntity>();
        }

        /// <summary>
        /// Gets the page of entities.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The page of entities.</returns>
        public static Task<ResultPage<TEntity>> GetPageAsync<TEntity>(this Repository<TEntity> repository, int page, int pageSize, CancellationToken cancellationToken = default) where TEntity : Entity, IPageable
        {
            if (repository is ISupportsPagination<TEntity> sp)
                return sp.GetPageAsync(page, pageSize, cancellationToken);

            throw RepositoryNotExtended<TEntity>();
        }

        private static InvalidOperationException RepositoryNotExtended<TEntity>() where TEntity : IPageable
        {
            return new InvalidOperationException($"Repository does not implement {typeof(ISupportsPagination<TEntity>).Name}");
        }
    }
}
