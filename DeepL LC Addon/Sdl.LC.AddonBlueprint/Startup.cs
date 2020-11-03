using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sdl.Community.DeeplAddon.DAL;
using Sdl.Community.DeeplAddon.Exceptions;
using Sdl.Community.DeeplAddon.Infrastructure;
using Sdl.Community.DeeplAddon.Interfaces;
using Sdl.Community.DeeplAddon.Models;
using Sdl.Community.DeeplAddon.Services;
using System.Net;
using System.Text.Json;


namespace Sdl.Community.DeeplAddon
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

            services.AddSingleton<IDatabaseContext, DatabaseContext>();
			services.AddSingleton<IHealthReporter, HealthReporter>();
			services.AddSingleton<IRepository, Repository>();
			services.AddSingleton<ITenantProvider, TenantProvider>();
			services.AddSingleton<IDescriptorService, DescriptorService>();
			services.AddSingleton<IAccountService, AccountService>();
			services.AddSingleton<ILanguageService, LanguagesService>();
			services.AddSingleton<ITranslationService, TranslationService>();

			services.Configure<AddonDescriptorModel>(Configuration.GetSection(
										AddonDescriptorModel.Descriptor));

			// Configure Basic Authentication 
			services.AddAuthentication("TenantAuthentication")
				.AddScheme<AuthenticationSchemeOptions, AddonAuthenticationHandler>("TenantAuthentication", null);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Use((context, next) =>
			{
				context.Request.EnableBuffering();
				return next();
			});

			app.UseHttpsRedirection();

			app.UseExceptionHandler(appError =>
			{
				appError.Run(async context =>
				{
					context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
					context.Response.ContentType = "application/json";

					var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (contextFeature != null)
					{
						string message;

						if (contextFeature.Error is AddonException)
						{
							var exception = (AddonException)contextFeature.Error;

							context.Response.StatusCode = (int)exception.StatusCode;
							message = JsonSerializer.Serialize(new
							{
								message = exception.Message,
								errorCode = exception.ErrorCode,
								details = exception.ExceptionDetails
							});
						}
						else
						{
							message = JsonSerializer.Serialize(new
							{
								errorCode = ErrorCodes.InternalError,
								message = contextFeature.Error.Message,
							});
						}

						await context.Response.WriteAsync(message).ConfigureAwait(false);
					}
				});
			});

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});


		}
	}
}
