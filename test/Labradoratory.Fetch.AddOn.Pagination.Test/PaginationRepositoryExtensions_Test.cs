using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.Pagination.Test
{
    public class PaginationRepositoryExtensions_Test
    {
        [Fact]
        public async Task CountAsync_CallsRepository()
        {
            var expectedValue = new Random().Next();

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository.Setup(r => r.CountAsync(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedValue);

            var subject = mockRepository.Object;
            
            var result = await subject.CountAsync(cancellationToken: CancellationToken.None);

            Assert.Equal(expectedValue, result);
            mockRepository.Verify(r => r.CountAsync(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CountAsync_AppliesFilter()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var items = new List<TestEntity>
            {
                new TestEntity { IntValue = 1 },
                new TestEntity { IntValue = 2 },
                new TestEntity { IntValue = 3 },
                new TestEntity { IntValue = 4 },
                new TestEntity { IntValue = 5 },
                new TestEntity { IntValue = 6 }
            }.AsQueryable();

            var filter = new Func<IQueryable<TestEntity>, IQueryable<TestEntity>>(query => query.Where(t => t.IntValue > 3));
            var expectedCount = filter(items).Count();

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.CountAsync(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken>((f, ct) => Task.FromResult(f(items).Count()));

            var subject = mockRepository.Object;

            var result = await subject.CountAsync(filter, CancellationToken.None);

            Assert.Equal(expectedCount, result);
            mockRepository.Verify(r => r.CountAsync(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_CallsRepository()
        {
            var expectedPage = 46;
            var expectedPageSize = 5;
            var expectedResult = new List<TestEntity> { new TestEntity(), new TestEntity() };

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken>((p, ps, f, ct) => Task.FromResult(new ResultPage<TestEntity>(p, ps, expectedResult)));

            var subject = mockRepository.Object;
            var result = await subject.GetPageAsync(expectedPage, expectedPageSize, cancellationToken: CancellationToken.None);

            Assert.Equal(expectedPage, result.Page);
            Assert.Equal(expectedPageSize, result.PageSize);
            Assert.Equal(expectedResult, result.Results);
            mockRepository.Verify(r => r.GetPageAsync(It.Is<int>(v => v == expectedPage), It.Is<int>(v => v == expectedPageSize), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_AppliesFilter()
        {
            var expectedPage = 46;
            var expectedPageSize = 5;
            var items = new List<TestEntity> { new TestEntity { IntValue = 1 }, new TestEntity { IntValue = 2 } }.AsQueryable();
            var filter = new Func<IQueryable<TestEntity>, IQueryable<TestEntity>>(query => query.Where(t => t.IntValue == 1));
            var expectedResult = filter(items);

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken>((p, ps, f, ct) => Task.FromResult(new ResultPage<TestEntity>(p, ps, f(items))));

            var subject = mockRepository.Object;
            var result = await subject.GetPageAsync(expectedPage, expectedPageSize, filter, CancellationToken.None);

            Assert.Equal(expectedPage, result.Page);
            Assert.Equal(expectedPageSize, result.PageSize);
            Assert.Equal(expectedResult, result.Results);
            mockRepository.Verify(r => r.GetPageAsync(It.Is<int>(v => v == expectedPage), It.Is<int>(v => v == expectedPageSize), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageWithNextAsync_CallsRepository()
        {
            var expectedPage = 123;
            var expectedPageSize = 2;
            var expectedResult = new List<TestEntity> { new TestEntity(), new TestEntity() };

            var baseUriString = "http://test.test:111";
            var baseUri = new Uri(baseUriString);
            var expectedUri = new Uri($"{baseUriString}?page={expectedPage + 1}&pagesize={expectedPageSize}");

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken>((p, ps, f, ct) => Task.FromResult(new ResultPage<TestEntity>(p, ps, expectedResult)));

            var subject = mockRepository.Object;
            var result = await subject.GetPageWithNextAsync(expectedPage, expectedPageSize, baseUri, cancellationToken: CancellationToken.None);

            Assert.Equal(expectedPage, result.Page);
            Assert.Equal(expectedPageSize, result.PageSize);
            Assert.Equal(expectedResult, result.Results);
            Assert.Equal(expectedUri, result.Next);
            mockRepository.Verify(r => r.GetPageAsync(It.Is<int>(v => v == expectedPage), It.Is<int>(v => v == expectedPageSize), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageWithNextAsync_NextNullWhenLastPage()
        {
            var expectedPage = 123;
            var expectedPageSize = 456;
            var expectedResult = new List<TestEntity>();

            var baseUriString = "http://test.test";
            var baseUri = new Uri(baseUriString);

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken>((p, ps, f, ct) => Task.FromResult(new ResultPage<TestEntity>(p, ps, expectedResult)));

            var subject = mockRepository.Object;
            var result = await subject.GetPageWithNextAsync(expectedPage, expectedPageSize, baseUri, cancellationToken: CancellationToken.None);

            Assert.Null(result.Next);
        }

        [Fact]
        public async Task GetPageWithNextAsync_HttpRequest_CallsRepository()
        {
            var expectedPage = 123;
            var expectedPageSize = 2;
            var expectedResult = new List<TestEntity> { new TestEntity(), new TestEntity() };

            var baseUriString = "http://test.test:111";
            var baseUri = new Uri(baseUriString);
            var expectedUri = new Uri($"{baseUriString}?page={expectedPage + 1}&pagesize={expectedPageSize}");

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);

            var mockRepository = new Mock<TestRepository>(MockBehavior.Strict, mockProcessorProvider.Object);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()))
                .Returns<int, int, Func<IQueryable<TestEntity>, IQueryable<TestEntity>>, CancellationToken > ((p, ps, f, ct) => Task.FromResult(new ResultPage<TestEntity>(p, ps, expectedResult)));

            var mockRequest = new Mock<HttpRequest>(MockBehavior.Strict);
            mockRequest.SetupGet(r => r.Query).Returns(new QueryCollection(new Dictionary<string, StringValues>
            {
                { "page", new StringValues(expectedPage.ToString()) },
                { "pagesize", new StringValues(expectedPageSize.ToString()) }
            }));
            mockRequest.SetupGet(r => r.Host).Returns(new HostString("test.test:111"));
            mockRequest.SetupGet(r => r.PathBase).Returns(new PathString(string.Empty));
            mockRequest.SetupGet(r => r.Scheme).Returns("http");

            var subject = mockRepository.Object;
            var result = await subject.GetPageWithNextAsync(mockRequest.Object, cancellationToken: CancellationToken.None);

            Assert.Equal(expectedPage, result.Page);
            Assert.Equal(expectedPageSize, result.PageSize);
            Assert.Equal(expectedResult, result.Results);
            Assert.Equal(expectedUri, result.Next);
            mockRepository.Verify(r => r.GetPageAsync(It.Is<int>(v => v == expectedPage), It.Is<int>(v => v == expectedPageSize), It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }        

        public class TestRepository : Repository<TestEntity>, ISupportsPagination<TestEntity>
        {
            protected TestRepository(Processors.IProcessorProvider processorProvider)
                : base(new ProcessorPipeline(processorProvider))
            {}

            public virtual Task<int> CountAsync(Func<IQueryable<TestEntity>, IQueryable<TestEntity>> filter = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public virtual Task<ResultPage<TestEntity>> GetPageAsync(int page, int pageSize, Func<IQueryable<TestEntity>, IQueryable<TestEntity>> filter = null, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public override Task<TestEntity> FindAsync(object[] keys, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public override IQueryable<TestEntity> Get()
            {
                throw new NotImplementedException();
            }

            public override IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<TestEntity>, IQueryable<TResult>> query)
            {
                throw new NotImplementedException();
            }

            protected override Task ExecuteAddAsync(TestEntity entity, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            protected override Task ExecuteDeleteAsync(TestEntity entity, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            protected override Task<ChangeSet> ExecuteUpdateAsync(TestEntity entity, ChangeSet changes, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }

        public class TestEntity : Entity, IPageable
        {
            public int IntValue { get; set; }

            public override object[] DecodeKeys(string encodedKeys)
            {
                throw new NotImplementedException();
            }

            public override string EncodeKeys()
            {
                throw new NotImplementedException();
            }

            public override object[] GetKeys()
            {
                throw new NotImplementedException();
            }
        }
    }
}
