using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Shared.Service;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICatalogService _catalogService;
        private readonly ISharedIdentityService _sharedIdentityService;
        private readonly IMapper _mapper;

        public CoursesController(ICatalogService catalogService, ISharedIdentityService sharedIdentityService, IMapper mapper)
        {
            _catalogService = catalogService;
            _sharedIdentityService = sharedIdentityService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _catalogService.GetAllCourseByUserId(_sharedIdentityService.GetUserId));
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _catalogService.GetAllCategory();
            ViewBag.categoryList = new SelectList(categories, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateInput courseCreateInput)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _catalogService.GetAllCategory();
                ViewBag.categoryList = new SelectList(categories, "Id", "Name");
                return View();
            }

            courseCreateInput.UserId = _sharedIdentityService.GetUserId;
            
            var response = await _catalogService.CreateCourse(courseCreateInput);

            if (response)
            {
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Update(string id)
        {
            var course = await _catalogService.GetByCourseId(id);
            var categories = await _catalogService.GetAllCategory();
            ViewBag.categoryList = ViewBag.categoryList = new SelectList(categories, "Id", "Name");

            if (course == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var courseUpdateInput = _mapper.Map<CourseUpdateInput>(course);

            return View(courseUpdateInput);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CourseUpdateInput courseUpdateInput)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _catalogService.GetAllCategory();
                ViewBag.categoryList = new SelectList(categories, "Id", "Name");
                return View(courseUpdateInput);
            }
            
            var response = await _catalogService.UpdateCourse(courseUpdateInput);

            if (response)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(courseUpdateInput);
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _catalogService.DeleteCourse(id);
            
            return RedirectToAction(nameof(Index));
        }
    }
}