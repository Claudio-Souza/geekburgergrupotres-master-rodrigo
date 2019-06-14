using AutoMapper;
using GeekBurger.Ingredients.Api.Services;
using GeekBurger.Ingredients.Api.Subscribers;
using GeekBurger.Ingredients.DataLayer;
using GeekBurger.Ingredients.DataLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Net.Http;

namespace GeekBurger.Ingredients.Api
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
            var serviceBusSettings = Configuration.GetSection(nameof(ServiceBusSettings))
                .Get<ServiceBusSettings>();

            var mongoUri = Configuration.GetConnectionString("mongo");
            var mongoClient = new MongoClient(mongoUri);
            services.AddSingleton<IMongoClient>(mongoClient);




            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>(factory => new UnitOfWork(mongoClient.GetDatabase("GeekBurgerIngredients")));
            services.AddScoped<IIngredientRepository, IngredientRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IMergedProductsRepository, MergedProductsRepository>();
            services.AddSingleton<IProductService, ProductService>(factory =>
            {
                var mapper = factory.GetRequiredService<IMapper>();
                var httpClient = factory.GetRequiredService<HttpClient>();
                var mergeService = factory.GetRequiredService<IMergeService>();
                var productServiceUri = Configuration.GetValue<string>("productService");

                return new ProductService(productServiceUri, httpClient, mapper, mergeService);
            });
            services.AddSingleton<IMergeService, MergeService>();
            services.AddSingleton(factory =>
            { 
                var mapper = factory.GetRequiredService<IMapper>();
                var mergeService = factory.GetRequiredService<IMergeService>();
                var unitOfWork = factory.GetRequiredService<IUnitOfWork>();

                var queue = new QueueClient(serviceBusSettings.ServiceBusConnectionString, serviceBusSettings.LabelImageAddedQueueName);

                return new LabelImageAddedSubscriber(mapper, mergeService, queue, unitOfWork);
            });
            services.AddSingleton(factory => 
            {
                var mapper = factory.GetRequiredService<IMapper>();
                var mergeService = factory.GetRequiredService<IMergeService>();
                var unitOfWork = factory.GetRequiredService<IUnitOfWork>();

                var queue = new SubscriptionClient(serviceBusSettings.ServiceBusConnectionString, serviceBusSettings.TopicName, serviceBusSettings.SubscriptionName);

                return new ProductChangedSubscriber(mapper, mergeService, queue, unitOfWork);
            });
            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, ProductService>(
                factory =>
                {
                    var mapper = factory.GetRequiredService<IMapper>();
                    var httpClient = factory.GetRequiredService<HttpClient>();
                    var mergeService = factory.GetRequiredService<IMergeService>();
                    var productServiceUri = Configuration.GetValue<string>("productService");

                    return new ProductService(productServiceUri, httpClient, mapper, mergeService);
                });

            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "Geekburger Ingredients" });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwaggerUI(opts =>
            {
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "Geekburger Ingredients");
            });

            app.UseSwagger();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
