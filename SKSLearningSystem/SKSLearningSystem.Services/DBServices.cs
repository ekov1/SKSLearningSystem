﻿using SKSLearningSystem.Areas.Admin.Models;
using SKSLearningSystem.Data;
using SKSLearningSystem.Data.Models;
using SKSLearningSystem.Models.ViewModels;
using SKSLearningSystem.Models.ViewModels.AdminViewModels;
using SKSLearningSystem.Services.Contracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace SKSLearningSystem.Services
{
    public class DBServices : IDBServices
    {
        private LearningSystemDbContext context;

        public DBServices(LearningSystemDbContext context)
        {
            this.context = context;
        }

        public Course GetCoursesFromDB(int courseId)
        {
            return this.context.Courses.First(c => c.Id == courseId);
        }


        //alt st
        public Course GetCoursesFromDBByName(string courseName)
        {
            return this.context.Courses.First(c => c.Name == courseName);
        }

        public CourseState GetStateFromDB(int courStateId)
        {
            return this.context.CourseStates.First(c => c.Id == courStateId);
        }
        public IList<UserViewModel> GetUserViewModels()
        {
            return this.context.Users.Select(x => new UserViewModel()
            {
                UserName = x.UserName,
                Id = x.Id
            }).ToList();
        }
        public async Task SaveAssignementsForDepartment(DepToCourseViewModel model)
        {
            var users = this.context.Users
                .Where(x => x.Department == model.Department).ToList();
            var course = this.context.Courses
                .First(x => x.Name == model.CourseName);

            for (int i = 0; i < users.Count; i++)
            {
                if (await this.context.CourseStates
                    .AnyAsync(x => x.CourseId == course.Id && users[i].Id == x.UserId) == false)
                {
                    this.context.CourseStates.Add(new CourseState()
                    {
                        User = users[i],
                        Course = course,
                        DueDate = model.DueDate,
                        Mandatory = model.Mandatory,
                        Grade = model.Grade,
                        State = "Pending"
                    });
                }
            }
            await this.context.SaveChangesAsync();
        }

        public async Task SaveAssignementsToDb(int courseId, IList<UserViewModel> users)
        {
            var userIds = users.Select(x => x.Id).ToArray();
            var course = this.context.Courses.First(x => x.Id == courseId);
            var usersFromDB = this.context.Users.Where(x => userIds.Contains(x.Id)).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (await this.context.CourseStates
                    .AnyAsync(x => x.CourseId == course.Id && users[i].Id == x.UserId) == false)
                {
                    this.context.CourseStates.Add(new CourseState()
                    {
                        User = usersFromDB.First(x => x.Id == users[i].Id),
                        Course = course,
                        DueDate = users[i].DueDate,
                        Mandatory = users[i].Mandatory,
                        State = "Pending"
                    });
                }

            }
            await this.context.SaveChangesAsync();
        }

        public async Task SaveAssignementsToDb(CourseState state)
        {
            if (await this.context.CourseStates
                .AnyAsync(x => x.CourseId == state.CourseId && state.UserId == x.UserId) == false)
            {
                this.context.CourseStates.Add(state);
                await this.context.SaveChangesAsync();
            }
        }
        //alt end
        public List<Image> GetImages(int courseId)
        {
            var images = this.context.Images.Where(i => i.CourseId == courseId);

            return images.ToList();
        }

        public string GetCourseName(int courseId)
        {
            // Get the assigned from admin course to user
            var assignedCourse = this.context.Courses.First(c => c.Id == courseId);
            var courseName = assignedCourse.Name;

            return courseName;
        }

        public async Task<Image> GetImageByID(int id)
        {
            return await this.context.Images.FirstAsync(x => x.Id == id);
        }

        public IEnumerable<User> GetUsersFromDB(string[] userId)
        {
            return this.context.Users.Where(x => userId.Contains(x.Id));
        }

        public AssignCourseViewModel GetUsersAndCoursesFromDB()
        {
            var users = this.context
               .Users
               .Select(u => new UserViewModel()
               {
                   UserName = u.UserName,
                   Id = u.Id
               }).ToList();

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
                Courses = courses,
                Users = users
            };

            return assignCourseViewModel;
        }

        //public List<SingleCourseViewModel> GetCoursesFromDb()
        //{
        //    var courses = context.Courses.Take(10).Select(x => new SingleCourseViewModel()
        //    {
        //        CourseId = x.Id,
        //        CourseStateId = x.Id,
        //        CourseName = x.Name,
        //        Descrtiption = x.Description,
        //        CourseImageId = x.Images.FirstOrDefault().Id
        //    }).ToList();
        //    return courses;
        //}

        public DeasignCourseViewModel GetCourseStates(DeasignCourseViewModel deasignCourseViewModel)
        {
            deasignCourseViewModel.CourseStates = context.CourseStates
                 .Select(x => new CourseStateRowViewModel()
                 {
                     Coursename = x.Course.Name,
                     Username = x.User.UserName,
                     Checked = true
                 }).ToList();
            return deasignCourseViewModel;
        }

        public void DeleteCourseStates(DeasignCourseViewModel deasignCourseViewModel)
        {
            var courseStatesIDs = deasignCourseViewModel.CourseStates.Select(x => x.Checked == false);

            for (int i = 0; i < courseStatesIDs.Count(); i++)
            {
                context.CourseStates.Remove(courseStatesIDs[i].id);
            }
            
        }
    }
}
