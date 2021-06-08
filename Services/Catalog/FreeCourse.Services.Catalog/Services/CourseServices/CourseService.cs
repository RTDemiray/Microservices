using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services.CategoryServices;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MapsterMapper;
using MassTransit;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services.CourseServices
{
    public class CourseService : ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public CourseService(IDatabaseSettings databaseSettings, IMapper mapper, ICategoryService categoryService, IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionStrings);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Shared.Response<List<CourseDto>>> GetAllAsync()
        {
            var courses = await _courseCollection.Find(category => true).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstOrDefaultAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return Shared.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), HttpStatusCode.OK);
        }

        public async Task<Shared.Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Shared.Response<CourseDto>.Fail("Course not found", HttpStatusCode.NotFound);
            }

            course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstOrDefaultAsync();
            return Shared.Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), HttpStatusCode.OK);
        }

        public async Task<Shared.Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find(x => x.UserId == userId).ToListAsync();
            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find(x => x.Id == course.Id).FirstOrDefaultAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return Shared.Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), HttpStatusCode.OK);
        }

        public async Task<Shared.Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);
            newCourse.CreatedTime = DateTime.Now;
            newCourse.Category = await _categoryCollection.Find(x => x.Id == newCourse.CategoryId).FirstOrDefaultAsync();
            await _courseCollection.InsertOneAsync(newCourse);
            return Shared.Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse),HttpStatusCode.Created);
        }

        public async Task<Shared.Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);
            var result = await _courseCollection.ReplaceOneAsync(x=>x.Id == courseUpdateDto.Id,updateCourse);
            if (result == null)
            {
                return Shared.Response<NoContent>.Fail("Course not found", HttpStatusCode.NotFound);
            }
            
            await _publishEndpoint.Publish<BasketCourseNameChangedEvent>(new BasketCourseNameChangedEvent{UserId = updateCourse.UserId,CourseId = updateCourse.Id,UpdatedName = updateCourse.Name});
            await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent{CourseId = updateCourse.Id,UpdatedName = updateCourse.Name});
            
            return Shared.Response<NoContent>.NoContent();
        }

        public async Task<Shared.Response<NoContent>> DeleteAsync(string id)
        {
            var categoryCourse = await _courseCollection.Find(x=>x.Id==id).FirstOrDefaultAsync();
            var deleteCourseResult = await _courseCollection.DeleteOneAsync(x => x.Id == id);
            var deleteCategoryResult = await _categoryCollection.DeleteOneAsync(x => x.Id == categoryCourse.CategoryId);
            
            if (deleteCourseResult.DeletedCount == 0)
            {
                return Shared.Response<NoContent>.Fail("Course not found", HttpStatusCode.NotFound);
            }
            if (deleteCategoryResult.DeletedCount == 0)
            {
                return Shared.Response<NoContent>.Fail("No categories found in the course", HttpStatusCode.NotFound);
            }
            
            return Shared.Response<NoContent>.NoContent();
        }
    }
}