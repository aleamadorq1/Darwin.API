using System;
using Darwin.API.Dtos;
using Darwin.API.Models;
using Darwin.API.Repositories;

namespace Darwin.API.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto> GetCategoryById(int id);
        Task<CategoryDto> AddCategory(CategoryDto categoryDto);
        Task<CategoryDto> UpdateCategory(CategoryDto categoryDto);
        Task<bool> DeleteCategory(int id);
    }

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryService(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId
            }).ToList();
        }

        public async Task<CategoryDto> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return null;
            }
            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId
            };
        }

        public async Task<CategoryDto> AddCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            var newCategory = await _categoryRepository.AddAsync(category);

            return new CategoryDto
            {
                CategoryId = newCategory.CategoryId,
                CategoryName = newCategory.CategoryName,
                ParentCategoryId = newCategory.ParentCategoryId
            };
        }

        public async Task<CategoryDto> UpdateCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName,
                ParentCategoryId = categoryDto.ParentCategoryId
            };

            var updatedCategory = await _categoryRepository.UpdateAsync(category);

            return new CategoryDto
            {
                CategoryId = updatedCategory.CategoryId,
                CategoryName = updatedCategory.CategoryName,
                ParentCategoryId = updatedCategory.ParentCategoryId
            };
        }

        public async Task<bool> DeleteCategory(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}

