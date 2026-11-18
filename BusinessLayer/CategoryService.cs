using ApplicationLayer.Interfaces;
using CoreModels.DTO_S;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CategoryService
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            return await this.unitOfWork.category.GetAllCategories();
        }

        public async Task<CategoryDto> GetCategoryListById(int id)
        {
            return await this.unitOfWork.category.GetCategoryListById(id);
        }

        public async Task<CreateUpdateCategoryDto> CreateCategory(CategoryDto categoryDto)
        {
           return await this.unitOfWork.category.CreateCategory(categoryDto);
        }

        public async Task<CreateUpdateCategoryDto> UpdateCategory(int id, CreateUpdateCategoryDto dto)
        {
            return await this.unitOfWork.category.UpdateCategory(id, dto);
        }

        public async Task<string> DeleteCategory(int id)
        {
            return await unitOfWork.category.DeleteCategory(id);
        }

        //public async Task<List<CategoryDto>> GetCategoryTree()
        //{
        //    return await unitOfWork.category.GetCategoryTree();
        //}

        public async Task SoftDeleteCategory(SoftDeleteCategoryDto dto)
        {
            await this.unitOfWork.category.SoftDeleteCategory(dto);
        }

        public async Task RestoreCategory(int categoryId)
        {
            await this.unitOfWork.category.RestoreCategory(categoryId);
        }

        public async Task UpdateParentCategory(int id,CreateUpdateCategoryDto dto)
        {
            await this.unitOfWork.category.UpdateParentCategory(id, dto);   
        }

        public async Task SoftDeleteParentCategory(SoftDeleteCategoryDto dto)
        {
            await this.unitOfWork.category.DeleteParentCategory(dto);
        }

        public async Task RestoreParentCategory(int categoryId)
        {
            await this.unitOfWork.category.RestoreParentCategory(categoryId);   
        }
    }
}
