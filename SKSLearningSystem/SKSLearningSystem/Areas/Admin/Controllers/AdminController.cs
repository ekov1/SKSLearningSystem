﻿using SKSLearningSystem.Areas.Admin.Models;
using SKSLearningSystem.Areas.Admin.Services;
using SKSLearningSystem.Data;
using System.Linq;
using SKSLearningSystem.Data.Models;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SKSLearningSystem.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminServices services;
        private readonly IGridServices gridServices;
        private readonly ApplicationUserManager userManager;
        private readonly LearningSystemDbContext context;

        public AdminController(IAdminServices services, ApplicationUserManager userManager, LearningSystemDbContext context
            , IGridServices gridServices)

        {
            this.services = services;
            this.userManager = userManager;
            this.context = context;
            this.gridServices = gridServices;
            this.userManager = userManager;
        }     

        [HttpGet]
        public ActionResult UploadCourse()
        {
            return this.View();
        }

        // Assign Course Methods Start
        [HttpGet]
        public ActionResult AssignCourse()
        {
            var assignCourseViewModel = this.services.GetUsersAndCoursesFromDB();

            return this.View(assignCourseViewModel);
        }

        public ActionResult ConfirmAssignment(AssignCourseViewModel assignCourseViewModel)
        {
            return View(assignCourseViewModel);
        }

        [HttpPost]
        public ActionResult SubmitAssignments(AssignCourseViewModel assignCourseViewModel)
        {
            this.services.SaveAssignedCoursesToDb(assignCourseViewModel);

            return RedirectToAction("AssignCourse");
        }
        // end

        [HttpGet]
        public ActionResult DeAssignCourse()
        {
            var courses = this.context
                .Courses
                .Select(c => new CourseViewModel()
                {
                    Name = c.Name,
                    Id = c.Id
                })
                .ToList();

            var assignCourseViewModel = new AssignCourseViewModel()
            {
                Courses = courses
            };

            return this.View(assignCourseViewModel);
        }

        public ActionResult EditUsers(AssignCourseViewModel assignCourseViewModel)
        {
            var coureseId = assignCourseViewModel.Courses.First().Id;
            var users = context.CourseStates.Where(cs => cs.CourseId == coureseId).Select(x => x.UserId).ToList();

            return View(assignCourseViewModel);
        }

        [HttpPost]
        public ActionResult UploadCourse(UploadCourseViewModel model)
        {
            var IsValid = this.services.ValidateInputFiles(model);

            if (IsValid)
            {

                var course = this.services.ReadCourseFromJSON(model.CourseFile);
                var images = this.services.ReadImagesFromFiles(model.Photos);

                course.Images = images;
                this.services.SaveCourseToDB(course);
            }
            else
            {
                this.ModelState.AddModelError("file", "You can upload only json, png or jpg files.");
            }

            return this.View();
        }


        public ActionResult MonitorUsersProgress()
        {
            return this.View();
        }

        public ActionResult AssignRoles()
        {
            var users = this.userManager
                .Users
                .Select(u => new UserViewModel()
                {
                    UserName = u.UserName,
                    Id = u.Id
                }).ToList();

            var assignCourseViewModel = new AssignCourseViewModel()
            {
                Users = users
            };

            return View(assignCourseViewModel);
        }

        public async Task<ActionResult> MakeAdmin(AssignCourseViewModel assignCourseViewModel)
        {
            var userIds = assignCourseViewModel.Users.Select(y => y.Id).ToArray();
            var users = context.Users.Where(x => userIds.Contains(x.Id)).ToList();

            for (int i = 0; i < users.Count; i++)
            {
                await this.userManager.AddToRoleAsync(users[i].Id, "Admin");
            }

            return RedirectToAction("AssignRoles");
        }

            public ActionResult GetJSON(bool _search, int rows, int page, string filters)
            {
                if (_search == false)
                {
                    return Json(this.gridServices.SearchFalseResult(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(this.gridServices.SearchResultTrue(filters), JsonRequestBehavior.AllowGet);
                }

            }
        }
    }