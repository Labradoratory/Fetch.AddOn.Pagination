using System.Threading;
using System.Threading.Tasks;

namespace Labradoratory.Fetch.AddOn.Pagination
{
    public interface ISupportsPagination<TEntity>
    {
        /// <summary>
        /// Gets the total number of entities available.
        /// </summary>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The total number of entities available.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the page of entities.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="cancellationToken">[Optional] The token to monitor for cancellation requests.</param>
        /// <returns>The page of entities.</returns>
        Task<ResultPage<TEntity>> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
