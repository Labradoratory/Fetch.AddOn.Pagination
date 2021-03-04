using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.AddOn.Pagination.Test
{
    public class PaginationRepositoryActions_Test
    {
        [Fact]
        public async Task CountAsync_CallsRepository()
        {
            var expectedValue = new Random().Next();

            var mockRepository = new Mock<ISupportsPagination<TestEntity>>(MockBehavior.Strict);
            mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedValue);

            var subject = new PaginationRepositoryActions<TestEntity>(mockRepository.Object, new Uri("http://test.test"));
            var result = await subject.CountAsync(CancellationToken.None);

            Assert.Equal(expectedValue, result);
            mockRepository.Verify(r => r.CountAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_CallsRepository()
        {
            var expectedPage = 123;
            var expectedPageSize = 2;
            var expectedResult = new List<TestEntity> { new TestEntity(), new TestEntity() };

            var baseUriString = "http://test.test";
            var expectedUri = new Uri($"{baseUriString}?page={expectedPage + 1}&pagesize={expectedPageSize}");

            var mockRepository = new Mock<ISupportsPagination<TestEntity>>(MockBehavior.Strict);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync<int, int, CancellationToken, ISupportsPagination<TestEntity> , ResultPage <TestEntity>>((p, ps, ct) => new ResultPage<TestEntity>(p, ps, expectedResult));

            var subject = new PaginationRepositoryActions<TestEntity>(mockRepository.Object, new Uri(baseUriString));
            var result = await subject.GetPageAsync(expectedPage, expectedPageSize, CancellationToken.None);

            Assert.Equal(expectedPage, result.Page);
            Assert.Equal(expectedPageSize, result.PageSize);
            Assert.Equal(expectedResult, result.Results);
            Assert.Equal(expectedUri, result.Next);
            mockRepository.Verify(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetPageAsync_NextNullWhenLastPage()
        {
            var expectedPage = 123;
            var expectedPageSize = 456;
            var expectedResult = new List<TestEntity>();

            var baseUriString = "http://test.test";
            var expectedUri = new Uri($"{baseUriString}?page={expectedPage + 1}&pagesize={expectedPageSize}");

            var mockRepository = new Mock<ISupportsPagination<TestEntity>>(MockBehavior.Strict);
            mockRepository
                .Setup(r => r.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync<int, int, CancellationToken, ISupportsPagination<TestEntity>, ResultPage<TestEntity>>((p, ps, ct) => new ResultPage<TestEntity>(p, ps, expectedResult));

            var subject = new PaginationRepositoryActions<TestEntity>(mockRepository.Object, new Uri(baseUriString));
            var result = await subject.GetPageAsync(expectedPage, expectedPageSize, CancellationToken.None);

            Assert.Equal(null, result.Next);
        }

        public class TestEntity : Entity, IPageable
        {
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
