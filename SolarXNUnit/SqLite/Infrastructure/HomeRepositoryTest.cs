using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.Entities;
using DbAccessaLayer.Infrastructure;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class HomeRepositoryTest : SqliteTestBase
    {
        private HomeRepository homeRepository;

        [SetUp]
        public void SetUp()
        {
            homeRepository = new HomeRepository(_db);
        }

        //GetHomeCategories
        [Test]
        public async Task GetAllCategories_Success()
        {

            var result = await homeRepository.GetHomeCategories();
            Assert.Multiple(() =>
            {
                Assert.IsNotNull(result);

            });
        }

        [Test]
        public void GetAllCategories_ThrowException()
        {
            homeRepository = new HomeRepository(null);
            Assert.ThrowsAsync<NullReferenceException>(async () => await homeRepository.GetHomeCategories());
        }

        //GetHomePackages
        [Test]
        public async Task GetHomePackes_Success()
        {
            var result = await homeRepository.GetHomePackage();
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result);
                Assert.That(result.Count(), Is.EqualTo(2));
                Assert.That(result[0].Children.SelectMany(x => x.ProductsDto).Count(), Is.EqualTo(1));
            });
        }

        [Test]
        public async Task GetHomePackage_Failure_ThrowException()
        {
            _db.Categories.RemoveRange(_db.Categories);
            await _db.SaveChangesAsync();
            Assert.ThrowsAsync<NotFoundException>(async () => await homeRepository.GetHomePackage());
        }

        //GetProductBySlug

        [Test]
        public void GetProductBySlug_Failure_ThrowException()
        {
            string slug = "fake slug";
            Assert.ThrowsAsync<NotFoundException>(async () => await homeRepository.GetProductBySlug(slug));
        }

        [Test]
        public async Task GetProductBySlug_Success()
        {
            string slug = "solar-panels";
            var result = await homeRepository.GetProductBySlug(slug);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result.Children);
                Assert.That(result.Children.Count(), Is.EqualTo(1));
                Assert.That(result.Children.SelectMany(x => x.ProductsDto).Count(), Is.EqualTo(1));

            });
        }

        [Test]
        public async Task GetProductsBySlug_Success_Senario_NoSubCategories()
        {
            string slug = "solarx";
            var result = await homeRepository.GetProductBySlug(slug);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result.ProductsDto);
            });
        }

        [Test]
        public void ViewProducts_Failure_Case()
        {
            int productId = 77;
            Assert.ThrowsAsync<NotFoundException>(async () => await homeRepository.ViewProductDetails(productId));
        }

        [Test]
        public async Task ViewProducts_Success()
        {
            int productId = 1;
            var result = await homeRepository.ViewProductDetails(productId);
            Assert.Multiple(() =>
            {
                Assert.IsNotEmpty(result.Brand);
                Assert.That(result.productReviews.Count(), Is.EqualTo(0));
            });
        }

        //ProductAnalytics
        [Test]
        public async Task GetProductAnalytics_ShouldReturnCorrectData()
        {
            var result = await homeRepository.GetProductAnalytics(3);

            Assert.Multiple(() =>
            {
                Assert.That(result.ProductId,Is.EqualTo(3));
                Assert.That(result.PowerGenerated, Is.EqualTo(500));
                Assert.That(result.Unit, Is.EqualTo("W"));
                Assert.That(result.Quantity, Is.EqualTo(1));
            });
        }


        [Test]
        public async Task GetProductAnalytics_ShouldConvertToKW()
        {
           var dto= new SolarPerformance
            {
                UserId = "LMN456",
                PanelId = 2,
                Power = 50000,
                Generated_at = TimeUtility.NowIST(),
                IsActive = true
            };
            await _db.SolarPerformances.AddAsync(dto);

            var result = await homeRepository.GetProductAnalytics(3);
            dynamic data = result;

            Assert.That((string)data.Unit, Is.EqualTo("W"));
        }

        [Test]
        public async Task GetProductAnalytics_ShouldConvertToMW()
        {
            var dto = new SolarPerformance
            {
                UserId = "LMN456",
                PanelId = 2,
                Power = 2_000_000,
                Generated_at = TimeUtility.NowIST(),
                IsActive = true
            };
            await _db.SolarPerformances.AddAsync(dto);
            await _db.SaveChangesAsync();

            var result = await homeRepository.GetProductAnalytics(3);
            dynamic data = result;

            Assert.That((string)data.Unit, Is.EqualTo("MW"));
        }

    }
}
