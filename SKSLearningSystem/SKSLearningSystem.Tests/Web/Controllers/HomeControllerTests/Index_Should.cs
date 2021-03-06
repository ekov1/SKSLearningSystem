﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SKSLearningSystem.Areas.Admin.Models;
using SKSLearningSystem.Areas.Admin.Services;
using SKSLearningSystem.Controllers;
using SKSLearningSystem.Models.ViewModels;
using SKSLearningSystem.Services;
using SKSLearningSystem.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.FluentMVCTesting;

namespace SKSLearningSystem.Tests.Web.Controllers.HomeControllerTests
{
    [TestClass]
    public class Index_Should
    {
        [TestMethod]
        public void ReturnDefaultViewWithListOfModels()
        {
            //Arrange
            var homeServicesMock = new Mock<IHomeServices>();
            var adminServicesMock = new Mock<IAdminServices>();
            var dbServicesMock = new Mock<IDBServices>();
            List<SingleCourseViewModel> model = new List<SingleCourseViewModel>();

            var controller = new HomeController(homeServicesMock.Object,adminServicesMock.Object,dbServicesMock.Object);


            //Act & Assert
            controller.WithCallTo(x => x.Index()).ShouldRenderDefaultView();
        }
    }
}
