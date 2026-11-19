using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using DbAccessaLayer.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework.Constraints;
using SolarXNUnit.SqLite.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolarXNUnit.SqLite.Infrastructure
{
    public class CategoryRepositoryTest:SqliteTestBase
    {
        private CategoryRepository categoryRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private IFormFile CreateFakeImage(string fileName)
        {
            var content = "Dummy image";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            return new FormFile(stream, 0, stream.Length, "Data", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };
        }

        [SetUp]
        public void SetUp()
        {
            var mock = new Mock<IWebHostEnvironment>();
            mock.Setup(x => x.WebRootPath).Returns(Path.GetTempPath());
            categoryRepository =new CategoryRepository(_db,mock.Object);

        }

        //GetAllCategories
        [Test]
        public async Task GetAllCategories_Success()
        {
            
            var result=await categoryRepository.GetAllCategories();
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void GetAllCategories_ThrowException()
        {
            categoryRepository = new CategoryRepository(null,webHostEnvironment);
            Assert.ThrowsAsync<NullReferenceException>(async() => await categoryRepository.GetAllCategories());  
        }

        //GetCategoryListById
        [Test]
        public async Task GetCategoryListById_Success()
        {
            int id = 1;
            var result=await categoryRepository.GetCategoryListById(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CategoryId);
        }

        [Test]
        public void GetCategoryListById_ThrowException()
        {
            var id = 90;
            var ex=Assert.ThrowsAsync<Exception>(async()=>await categoryRepository.GetCategoryListById(id));
            Assert.That(ex.Message, Does.Contain($"no existing CategoryId {id} was found "));
        }

       //Create Category
       [Test]
       public async Task CreateCategory_Success()
        {
            var dto = new CategoryDto
            {
                Name = "light panel",
                Slug = "light panel 1",
                Description = "used for light power generation",
                ParentId = null,
                Image = CreateFakeImage("category.png"),
                
            };

            var result= await categoryRepository.CreateCategory(dto);
            _db.SaveChanges();
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Slug);
        }

        
        [Test]
        public void CreateCategory_ThrowException_SlugExists()
        {
            var dto = new CategoryDto
            {
                Name = "solar child-1",
                Description = "used for light power generation",
                ParentId = null,

            };

            Assert.ThrowsAsync<DataExistsException>(async () => await categoryRepository.CreateCategory(dto));
        }

        [Test]

        public void CreateCategory_ThrowException_ParentIdNotExist()
        {
            var dto = new CategoryDto
            {
                Name = "solar bi panel",
                Description = "used for light power generation",
                ParentId = 5,

            };

            Assert.ThrowsAsync<DataExistsException>(async () => await categoryRepository.CreateCategory(dto));
        }



        //Update Category
        [Test]
        public void UpdateCategory_Faliure_InvalidId()
        {
            int id = 9;
            var dto = new CreateUpdateCategoryDto
            {
                Name = "solar n panels",
                Description = "more reliable"
            };

           Assert.ThrowsAsync<NotFoundException>(async () => await categoryRepository.UpdateCategory(id, dto));
        }

        [Test]
        public void SlugExists_UpdateCategory_Faliure()
        {
            int id = 1;
            var dto = new CreateUpdateCategoryDto
            {
                Name = "solar child-1",
                Description = "more reliable"
            };

            Assert.ThrowsAsync<DataExistsException>(async () => await categoryRepository.UpdateCategory(id, dto));
        }

        [Test]
        public async Task UpdateCategory_Success()
        {
            int id = 1;
            var dto = new CreateUpdateCategoryDto
            {
                Name = "solar child",
                Description = "more reliable"
            };

            var result=await categoryRepository.UpdateCategory(id,dto);
            Assert.Multiple(() =>
            {
                Assert.That(result.Name, Is.Not.Empty);
                Assert.That(result.ParentId, Is.EqualTo(null));
            });
        }


        //DeleteCategory
        [Test]
        public void DeleteCategory_Faliure_InvalidId()
        {
            int id = 7;
            Assert.ThrowsAsync<NotFoundException>(async()=>await  categoryRepository.DeleteCategory(id));
        }

        [Test]
        public async Task DeleteCategory_Success()
        {
            int id = 1;
            var result=await categoryRepository.DeleteCategory(id);

            Assert.Multiple(() => {
                Assert.IsNotEmpty(result);
            });
        }


        //DeleteCategory
        [Test]
        public void DeleteCategory_Faliure_Id()
        {
            var dto = new SoftDeleteCategoryDto { CategoryId = 3, Reason = "Low production" };
            Assert.ThrowsAsync<InActiveModeExceptions>(async()=>await categoryRepository.SoftDeleteCategory(dto));

        }

        [Test]
        public async Task DeleteCategory_SuccessCase()
        {
            var dto = new SoftDeleteCategoryDto { CategoryId = 2, Reason = "Low production" };
            int id = dto.CategoryId;
            await categoryRepository.SoftDeleteCategory(dto);
            var updatedCategory=await _db.Categories.FirstOrDefaultAsync(x=>x.CategoryId==id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedCategory!.IsActive, Is.EqualTo(false));
            });
        }

        //RestoreCategory
        [Test]
        public void RestoreCategory_Failure_Id()
        {
            int id = 8;
            Assert.ThrowsAsync<DataExistsException>(async()=>await categoryRepository.RestoreCategory(id));
        }

        [Test]
        public async Task RestoreCategorySuccess()
        {
            int id = 3;
            await categoryRepository.RestoreCategory(id);

            var updatedCtegory=await _db.Categories.FirstOrDefaultAsync(x=>x.CategoryId == id);
            Assert.Multiple(() =>
            {
                Assert.That(updatedCtegory!.IsActive, Is.EqualTo(true));
            });
        }

        //UpdateParentCategory
        [Test]
        public void UpdateCategory_Faliure_Id()
        {
            int id = 8;
            var dto = new CreateUpdateCategoryDto
            {
                Name = "solar child",
                Description = "more reliable",
                Image = CreateFakeImage("category.png")
            };
            Assert.ThrowsAsync<NotFoundException>(async () => await categoryRepository.UpdateParentCategory(id, dto));
        }


        [Test]
        public void UpdateParentCategory_NullExeception()
        {
            int id = 2;
            Assert.ThrowsAsync<ArgumentNullException>(async () => await categoryRepository.UpdateParentCategory(id, null));
        }

        [Test]
        public async Task UpdateParentCategory_Success()
        {
            int id = 1;
            var dto = new CreateUpdateCategoryDto
            {
                Name = "solar child",
                Description = "more reliable",
                Image = CreateFakeImage("category.png")
            };

            await categoryRepository.UpdateParentCategory(id,dto);

            var updatedCategory=await _db.Categories.FirstAsync(x=>x.CategoryId==id);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(updatedCategory.UpdatedAt);
                Assert.That(updatedCategory.IsActive, Is.EqualTo(true));
            });
        }


        //DeleteParentCategory
        [Test]
        public void DeleteParentCategory_Falure()
        {
            var dto = new SoftDeleteCategoryDto { CategoryId = 2, Reason = "Low production" };
            Assert.ThrowsAsync<NotFoundException>(async()=>await  categoryRepository.DeleteParentCategory(dto));
        }

        [Test]
        public void DeleteParentCategory_Failure_Active()
        {
            var dto = new SoftDeleteCategoryDto { CategoryId = 1, Reason = "Low production" };
            Assert.ThrowsAsync<DataExistsException>(async () => await categoryRepository.DeleteParentCategory(dto));
        }

        [Test]
        public async Task DeleteParentCategory_Success()
        {
            var dto = new SoftDeleteCategoryDto { CategoryId = 4, Reason = "Low production" };
            await categoryRepository.DeleteParentCategory(dto);

            var updatedCategory= await _db.Categories.FirstAsync( x=>x.CategoryId==dto.CategoryId);
            Assert.That(updatedCategory.IsActive,Is.EqualTo(false));
        }


        //RestoreParentCategory
        [Test]
        public void RestoreParentCategory_FalureId()
        {
            int id = 9;
            Assert.ThrowsAsync<NotFoundException>(async () => await categoryRepository.RestoreParentCategory(id));
        }

        [Test]
        public async Task RestoreParentCategory_Success()
        {
            int id = 3;
            await categoryRepository.RestoreParentCategory(id);

            var updatedCategory=await _db.Categories.FirstAsync(x=>x.CategoryId==id);
            Assert.That(updatedCategory.IsActive, Is.EqualTo(true));
        }
    }
}
