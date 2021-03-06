﻿using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SKSLearningSystem.Areas.Admin.Controllers;
using SKSLearningSystem.Areas.Admin.Services;
using SKSLearningSystem.Data.Models;
using SKSLearningSystem.Models.ViewModels.AdminViewModels;
using SKSLearningSystem.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.FluentMVCTesting;

namespace SKSLearningSystem.Tests.Web.Areas.Admin.Controllers.AdminControllerTests
{
    [TestClass]
    public class AssignDepToCoursePost_Should
    {
        [TestMethod]
        public void ReturnDefaultPartialViewWithModel()
        {
            //Arrange
            var userStore = new Mock<IUserStore<User>>();
            var adminServicesMock = new Mock<IAdminServices>();
            var gridServicesMock = new Mock<IGridServices>();
            var dbServicesMock = new Mock<IDBServices>();
            var depName = "somedepname";
            var coursename = "somecoursename";

            var applicationUserManagerMock = new Mock<ApplicationUserManager>(userStore.Object);

            var controller = new AdminController(adminServicesMock.Object, applicationUserManagerMock.Object, gridServicesMock.Object,
                dbServicesMock.Object);

            //Act & Assert
            controller
                .WithCallTo(x => x.AssignDepToCourse(depName,coursename))
                .ShouldRenderPartialView("ConfirmDepToCourse")
                .WithModel<DepToCourseViewModel>(x=>x.CourseName==coursename&&x.Department==depName);
        }
    }
}
