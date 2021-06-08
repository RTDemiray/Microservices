using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared;
using FreeCourse.Shared.Dtos;
using MapsterMapper;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;

        public CategoryService(IDatabaseSettings databaseSettings,IMapper mapper)
        {
            var client = new MongoClient(databaseSettings.ConnectionStrings);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _categoryCollection.Find(category => true).ToListAsync();
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories),HttpStatusCode.OK);
        }

        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            await _categoryCollection.InsertOneAsync(_mapper.Map<Category>(categoryDto));
            return Response<CategoryDto>.Success(categoryDto,HttpStatusCode.Created);
        }

        public async Task<Response<NoContent>> UpdateAsync(CategoryDto categoryDto)
        {
            var result = await _categoryCollection.FindOneAndReplaceAsync(x=>x.Id == categoryDto.Id,_mapper.Map<Category>(categoryDto));
            if (result == null)
            {
                return Response<NoContent>.Fail("Category not found", HttpStatusCode.NotFound);
            }

            return Response<NoContent>.NoContent();
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _categoryCollection.Find<Category>(x => x.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                return Response<CategoryDto>.Fail("Category not found",HttpStatusCode.NotFound);
            }

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), HttpStatusCode.OK);
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _categoryCollection.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount == 0)
            {
                return Response<NoContent>.Fail("Category not found", HttpStatusCode.NotFound);
            }

            return Response<NoContent>.NoContent();
        }
    }
}