using Xunit;

namespace Labradoratory.Fetch.AddOn.Pagination.Test
{
    public class PageInfo_Tests
    {
        [Fact]
        public void Constructor_NoParams()
        {
            var subject = new PageInfo();
            Assert.Equal(PageInfo.DefaultPage, subject.Page);
            Assert.Equal(PageInfo.DefaultPageSize, subject.PageSize);
        }


        [Fact]
        public void Constructor_Params_PageLessThenOne()
        {
            var expectedPageSize = 20u;
            var subject = new PageInfo(0, expectedPageSize);
            Assert.Equal(PageInfo.DefaultPage, subject.Page);
            Assert.Equal(expectedPageSize, subject.PageSize);
        }


        [Fact]
        public void Constructor_Params_PageSizeLessThenOne()
        {
            var expectedPage = 2u;
            var subject = new PageInfo(expectedPage, 0);
            Assert.Equal(expectedPage, subject.Page);
            Assert.Equal(PageInfo.DefaultPageSize, subject.PageSize);
        }


        [Fact]
        public void Equals_RefSame_True()
        {
            var subject = new PageInfo();
            Assert.True(subject.Equals(subject));
        }


        [Fact]
        public void Equals_ValuesSame_True()
        {
            var page = 4u;
            var pageSize = 200u;
            var subject = new PageInfo(page, pageSize);
            var compare = new PageInfo(page, pageSize);
            Assert.True(subject.Equals(compare));
        }


        [Fact]
        public void Equals_PageDif_False()
        {
            var subject = new PageInfo(2u, 100u);
            var compare = new PageInfo(3u, 100u);
            Assert.False(subject.Equals(compare));
        }


        [Fact]
        public void Equals_PageSizeDif_False()
        {
            var subject = new PageInfo(2u, 100u);
            var compare = new PageInfo(2u, 200u);
            Assert.False(subject.Equals(compare));
        }


        [Fact]
        public void Equals_NotPageInfo_False()
        {
            var subject = new PageInfo(2u, 100u);
            var compare = new object();
            Assert.False(subject.Equals(compare));
        }


        [Fact]
        public void HasCode_Equals_True()
        {
            var subject = new PageInfo(2u, 100u);
            var compare = new PageInfo(2u, 100u);
            Assert.Equal(subject.GetHashCode(), compare.GetHashCode());
        }


        [Fact]
        public void HasCode_Equals_False()
        {
            var subject = new PageInfo(2u, 100u);
            var compare = new PageInfo(2u, 200u);
            Assert.NotEqual(subject.GetHashCode(), compare.GetHashCode());
        }
    }
}
