[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SKSLearningSystem.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(SKSLearningSystem.App_Start.NinjectWebCommon), "Stop")]

namespace SKSLearningSystem.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Microsoft.AspNet.Identity.Owin;
    using SKSLearningSystem.Data;
  
    
    using SKSLearningSystem.Areas.Admin.Models;
    using SKSLearningSystem.Areas.Admin.Services;
    using SKSLearningSystem.Services.CourseServices;
    using SKSLearningSystem.Services;
    using SKSLearningSystem.Services.Contracts;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ApplicationUserManager>().ToMethod(_ =>
            HttpContext
            .Current
            .GetOwinContext()
            .GetUserManager<ApplicationUserManager>());

            kernel.Bind<LearningSystemDbContext>().ToMethod(_ =>
            HttpContext
            .Current
            .GetOwinContext()
            .Get<LearningSystemDbContext>());

            kernel.Bind<IAdminServices>().To<AdminServices>().InRequestScope();


            kernel.Bind<IGridServices>().To<GridServices>().InRequestScope();

            kernel.Bind<ICourseService>().To<CourseService>().InRequestScope();

            kernel.Bind<IHomeServices>().To<HomeServices>().InRequestScope();

            kernel.Bind<IDBServices>().To<DBServices>().InRequestScope();
        }        
    }
}
