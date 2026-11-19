using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.CustomMessages;
using CoreModels.DTO_S;
using CoreModels.Entities;
using DbAccessaLayer.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbAccessaLayer.Infrastructure
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        public CategoryRepository(ApplicationDbContext db,IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _env = webHostEnvironment;
        }


        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            try
            {
                return await _db.Set<Category>().Where(x => x.ParentCategoryId == null)
                    .Select(x => new CategoryDto
                    {
                        CategoryId = x.CategoryId,
                        Name = x.CategoryName,
                        Slug = x.Slug,
                        IsActive = x.IsActive,
                        Description = x.Description,
                        DeletedReason = x.DeletedReason,
                        ImageUrl=x.ImageUrl,

                        Children = x.SubCategories.Select(c => new CategoryDto
                        {
                            CategoryId = c.CategoryId,
                            Name = c.CategoryName,
                            IsActive = c.IsActive,
                            DeletedReason = c.DeletedReason,
                            Description = c.Description
                        }).ToList()

                    }).ToListAsync();
                    
                

            }
            catch(Exception )
            {
                throw;
            }
        }

        public async Task<CategoryDto> GetCategoryListById(int id)
        {
            try
            {
                var existCategoryId=await _db.Categories.Include(x=>x.SubCategories).FirstOrDefaultAsync(x=>x.CategoryId== id &&x.IsActive);
                if (existCategoryId == null)
                {
                    throw new Exception($"no existing CategoryId {id} was found ");
                }
                else
                {
                    return new CategoryDto
                    {
                        CategoryId = existCategoryId.CategoryId,
                        Name = existCategoryId.CategoryName,
                        Slug = existCategoryId.Slug,
                        Description=existCategoryId.Description,
                        ParentId=existCategoryId.ParentCategoryId,
                        ImageUrl=existCategoryId.ImageUrl,
                        
                    };

                }

            }
            catch(Exception)
            {
                throw;
            }
        }

        private static string GenerateSlug(string name)
        {
            return Regex.Replace(name.ToLower().Trim(), @"[^a-z0-9]+","-").Trim('-');
        }


        public async Task<CreateUpdateCategoryDto> CreateCategory(CategoryDto categoryDto)
        {
            try
            {
                int? parentId = categoryDto.ParentId == 0 ? null : categoryDto.ParentId;

                var slug = GenerateSlug(categoryDto.Name);

                var slugExists = await _db.Categories
                    .AnyAsync(x => x.Slug == slug && x.IsActive);

                if (slugExists)
                    throw new DataExistsException(MessageHelper.Get("Categories", "Slug"));

                if (parentId != null)
                {
                    var parentExists = await _db.Categories
                        .AnyAsync(x => x.CategoryId == parentId && x.IsActive);

                    if (!parentExists)
                        throw new DataExistsException(MessageHelper.Get("Categories", "Parent"));
                }

                string? imageUrl = null;

           
                if (parentId == null)
                {
                    if (categoryDto.Image == null)
                        throw new Exception("Parent category must have image");

                    imageUrl = await ImageHelper.SaveCategoryImage(categoryDto.Image, _env);
                }
                else
                {
                   
                    if (categoryDto.Image != null)
                        throw new Exception("Subcategories should not have images");
                }

                var details = new Category
                {
                    CategoryName = categoryDto.Name,
                    Slug = slug,
                    Description = categoryDto.Description,
                    ParentCategoryId = parentId,
                    ImageUrl = imageUrl, 
                    CreatedAt = TimeUtility.NowIST()
                };

                await _db.Categories.AddAsync(details);
                await _db.SaveChangesAsync();

                return new CreateUpdateCategoryDto
                {
                    Name = details.CategoryName,
                    Slug = slug,
                    Description = details.Description,
                    ParentId = details.ParentCategoryId
                };
            }
            catch
            {
                throw;
            }
        }

        public async Task<CreateUpdateCategoryDto> UpdateCategory(int id,CreateUpdateCategoryDto dto)
        {
            try
            {
                var existId = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId==id);
                if (existId==null)
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                else
                {
                   var slug= GenerateSlug(dto.Name);
                    var slugExists = await _db.Categories.AnyAsync(x => x.Slug == slug && x.CategoryId != id && x.IsActive);

                    if (slugExists)
                        throw new DataExistsException(MessageHelper.Get("Categories", "Slug"));
                    existId.CategoryName=dto.Name.Trim();
                   existId.Slug=slug;
                   existId.Description= dto.Description;
                   existId.ParentCategoryId = dto.ParentId;
                   existId.IsActive=true;
                   existId.UpdatedAt = TimeUtility.NowIST();

                   await _db.SaveChangesAsync();

                    return new CreateUpdateCategoryDto
                    {
                        ParentId = dto.ParentId,
                        Name = dto.Name,
                        Slug = slug,
                        Description = dto.Description
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteCategory(int id)
        {
            try
            {
                var existId= await _db.Categories.FindAsync(id);
                if (existId == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                else
                {
                    existId.IsActive=false;
                    existId.UpdatedAt = TimeUtility.NowIST();
                    await _db.SaveChangesAsync();
                    return $"Successfully decativated if having id {id}";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<List<CategoryDto>> GetCategoryTree()
        //{
        //    try
        //    {
        //        //get flat data
        //        var categories = await _db.Categories.Where(x => x.IsActive).Select(x => new CategoryDto
        //        {
        //            CategoryId = x.CategoryId,
        //            Name = x.CategoryName,
        //            Slug = x.Slug,
        //            ParentId = x.ParentCategoryId,

        //        }).ToListAsync();

        //        //dictonary for fast look up
        //        var lookup = categories.ToDictionary(x => x.CategoryId);   //id as key

        //        //build tree
        //        List<CategoryDto> roots = new();

        //        foreach(var category in categories)
        //        {
        //            if (category.ParentId == null)
        //            {
        //                roots.Add(category);  //it looks like Residential (Children = [])

        //            }
        //            else if (lookup.ContainsKey(category.ParentId.Value))
        //            {
        //                lookup[category.ParentId.Value].Children.Add(category);  //returns [1] like Residential CategoryDto
        //                                                                         //after becomes Residential.Children.Add(Monocrystalline)
        //            }
        //        }
        //        return roots;
        //    }
        //    catch(Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task SoftDeleteCategory(SoftDeleteCategoryDto dto)
        {
            try
            {
                var category = await _db.Categories.FirstOrDefaultAsync(x => x.CategoryId == dto.CategoryId && x.IsActive);

                if (category == null)
                {
                    throw new InActiveModeExceptions(MessageHelper.Get("Categories", "Mode"));
                }
                else
                {
                    category.DeletedReason = dto.Reason;
                    category.DeletedAt = TimeUtility.NowIST();
                    category.IsActive = false;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RestoreCategory(int id)
        {
            try
            {
                var existCategory=await _db.Categories.FirstOrDefaultAsync(x=>x.CategoryId== id&&!x.IsActive);
                if(existCategory == null)
                {
                    throw new DataExistsException(MessageHelper.Get("Categories", "Active"));
                }
                else
                {
                    existCategory.IsActive = true;
                    existCategory.UpdatedAt= TimeUtility.NowIST();
                    existCategory.DeletedReason = null;
                    existCategory.DeletedAt = null;
                    await _db.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateParentCategory(int categoryId, CreateUpdateCategoryDto dto)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(MessageHelper.Get("Categories", "fill"));
                }
                var slug =  GenerateSlug(dto.Name.Trim());
                var existCategory=await _db.Categories.FirstOrDefaultAsync(x=>x.CategoryId== categoryId&&x.IsActive&&x.ParentCategoryId==null);
                if (existCategory == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                var imageUrl = await ImageHelper.SaveCategoryImage(dto.Image, _env);
                if (imageUrl == null)
                {
                    throw new Exception("Image is required");
                }
                else
                {
                    existCategory.CategoryName = dto.Name.Trim();
                    existCategory.Slug= slug;
                    existCategory.IsActive = true;
                    existCategory.ImageUrl = imageUrl;
                    existCategory.UpdatedAt = TimeUtility.NowIST(); 
                    
                    await _db.SaveChangesAsync();

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteParentCategory(SoftDeleteCategoryDto dto)
        {
            try
            {
                var category=await _db.Categories.Include(x=>x.SubCategories).FirstOrDefaultAsync(x=>x.CategoryId==dto.CategoryId&&
                x.ParentCategoryId==null&&x.IsActive);
                if(category == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                else
                {
                    if (category.SubCategories.Any(x => x.IsActive))
                    {
                        throw new DataExistsException(MessageHelper.Get("Categories", "DeactiveP"));
                    }
                    else
                    {
                        //category.SubCategories.Clear();
                        category.IsActive= false;
                        category.DeletedReason=dto.Reason.Trim();
                        category.DeletedAt = TimeUtility.NowIST();
                        await _db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task RestoreParentCategory(int categoryId)
        {
            try
            {
                var category=await _db.Categories.FirstOrDefaultAsync(x=>x.CategoryId==categoryId&&x.ParentCategoryId==null&&!x.IsActive);
                if(category == null)
                {
                    throw new NotFoundException(MessageHelper.Get("Categories", "NotFoundCategories"));
                }
                else
                {
                    category.IsActive = true;
                    category.UpdatedAt = TimeUtility.NowIST();
                    category.DeletedReason = null;
                    category.DeletedAt = null;
                    await _db.SaveChangesAsync();   
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
