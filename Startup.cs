using System;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CasaDoCodigo
{
    public class Startup
    {
        /*
          ConfigureServices() serve para adicionarmos serviços, por exemplo, o SQL Server, ou o serviço de log. 
          Configure() é onde o serviço é consumido, ou utilizado. 
      */
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }

        public Startup(ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            _loggerFactory = loggerFactory;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddMvc();

            //Configuração/registro para utilização de sessions
            services.AddDistributedMemoryCache();
            services.AddSession();

            //Configuração/registro de acesso ao banco de dados
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            //Configuração/registro do serviço de inicialização do DB
            services.AddTransient<IDataService, DataService>();
            
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            //Configuração/registro sessionhelper
            services.AddTransient<IHttpHelper, HttpHelper>();

            //Configuração/registro para acesso aos repositórios
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            //Necessário para utilizar o identitiy (middleware) no pipeline das requisições
            app.UseAuthentication();


            app.UseSession();

            app.UseMvc(routes =>
            {
                //Define página inicial da aplicação
                routes.MapRoute(
                    name: "default",
                    //template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
                    template: "{controller=Pedido}/{action=BuscaProdutos}/{codigo?}");
            });

            #region EnsureCreated/Migrate

            /*
            serviceProvider.GetService<ApplicationContext>()
                .Database
                //.EnsureCreated(); //Garante que o banco de dados está criado e igual ao modelo de dados
                .Migrate(); // Mesma função que o EnsureCreated, porém utiliza migrações, permitindo que outras migrações sejam realizadas futuramente
            */
            #endregion

            var dataService = serviceProvider.GetRequiredService<IDataService>();
            dataService.InicializaDBAsync(serviceProvider).Wait();

        }
    }
}
