using CoreModels.DTO_S;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();

        Task<CategoryDto> GetCategoryListById(int id);

        Task<CreateUpdateCategoryDto> CreateCategory(CategoryDto categoryDto);

        Task<CreateUpdateCategoryDto> UpdateCategory(int id,CreateUpdateCategoryDto dto);

        Task<string> DeleteCategory(int id);

        Task SoftDeleteCategory(SoftDeleteCategoryDto dto);

        Task RestoreCategory(int id);

        //Task<List<CategoryDto>> GetCategoryTree();



        Task UpdateParentCategory(int categoryId, CreateUpdateCategoryDto dto);

        Task DeleteParentCategory(SoftDeleteCategoryDto dto);

        Task RestoreParentCategory(int categoryId);
    }
}
