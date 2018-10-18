using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AssetsDB.DB;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NJsonSchema;

using NSwag.AspNetCore;


namespace AssetsDB
{
   public class Startup
   {
      public Startup( IConfiguration configuration )
      {
         Configuration = configuration;

         // custom config..
         AssetsDBManager.DBConnectionString = Configuration["DBConnectionString"];

      }

      public IConfiguration Configuration { get; }

      // This method gets called by the runtime. Use this method to add services to the container.
      public void ConfigureServices( IServiceCollection services )
      {
         services.AddMvc().SetCompatibilityVersion( CompatibilityVersion.Version_2_1 );
         // Register the Swagger services
         services.AddSwagger();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure( IApplicationBuilder app, IHostingEnvironment env )
      {
         //if ( env.IsDevelopment() ) {
         //   app.UseDeveloperExceptionPage();
         //}
         
         // Register the Swagger generator and the Swagger UI middlewares
         app.UseSwaggerUi3WithApiExplorer( settings =>
         {
            settings.PostProcess = document =>
            {
               document.Info.Version = "v1";
               document.Info.Title = "Assets DB";
               document.Info.Description = "A simple ASP.NET Core web API";
               //document.Info.TermsOfService = "None";
               //document.Info.Contact = new NSwag.SwaggerContact {
               //   Name = "Shayne Boyer",
               //   Email = string.Empty,
               //   Url = "https://twitter.com/spboyer"
               //};
               //document.Info.License = new NSwag.SwaggerLicense {
               //   Name = "Use under LICX",
               //   Url = "https://example.com/license"
               //};
            };

            settings.GeneratorSettings.DefaultPropertyNameHandling =
               PropertyNameHandling.CamelCase;
         } );

         app.UseMvc();
      }
   }
}
