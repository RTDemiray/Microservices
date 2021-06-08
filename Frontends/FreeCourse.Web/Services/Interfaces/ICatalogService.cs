using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeCourse.Web.Models;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CourseViewModel>> GetAllCourse();
        Task<List<CategoryViewModel>> GetAllCategory();
        Task<List<CourseViewModel>> GetAllCourseByUserId(string userId);
        Task<CourseViewModel> GetByCourseId(string courseId);
        Task<bool> CreateCourse(CourseCreateInput courseCreateInput);
        Task<bool> UpdateCourse(CourseUpdateInput courseUpdateInput);
        Task<bool> DeleteCourse(string courseId);
    }
}