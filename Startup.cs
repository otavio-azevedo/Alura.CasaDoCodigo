using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CasaDoCodigo.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CasaDoCodigo
{
    public class Startup
    {
        /*
          O ConfigureServices() serve para adicionarmos serviços, por exemplo, o SQL Server, ou o serviço de log. 
         Já o método Configure() é onde o serviço é consumido, ou utilizado. 
      */
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();


            //Configuração/registro para utilização de sessions
            services.AddDistributedMemoryCache();
            services.AddSession();

            //Configuração/registro de acesso ao banco de dados
            string connectionString = Configuration.GetConnectionString("Default");
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString)
            );

            //Configuração/registro do serviço de inicialização do DB
            services.AddTransient<IDataService, DataService>();
            //Configuração/registro para acesso aos repositórios
            services.AddTransient<IProdutoRepository, ProdutoRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<ICadastroRepository, CadastroRepository>();
            services.AddTransient<IItemPedidoRepository, ItemPedidoRepository>();


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

            app.UseSession();

            app.UseMvc(routes =>
            {
                //Define página inicial da aplicação
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Pedido}/{action=Carrossel}/{codigo?}");
            });

            /*
            serviceProvider.GetService<ApplicationContext>()
                .Database
                //.EnsureCreated(); //Garante que o banco de dados está criado e igual ao modelo de dados
                .Migrate(); // Mesma função que o EnsureCreated, porém utiliza migrações, permitindo que outras migrações sejam realizadas futuramente
            */

            serviceProvider.GetService<IDataService>().InicializaDB();

        }
    }
}
