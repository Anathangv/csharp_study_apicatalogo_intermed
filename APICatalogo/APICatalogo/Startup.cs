using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repositories.Implementacoes;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Servicos;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
//using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

namespace APICatalogo
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
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection"); //obtera string de connex�o

            services.AddDbContext<AppDbContext>(options => //registrando contexto como servi�o
            options.UseMySql(mySqlConnection, //definindo provedor do bd 
                             ServerVersion.AutoDetect(mySqlConnection))); //detectando a vers�o do servido

            services.AddControllers()
                    .AddNewtonsoftJson(opt =>
                    {
                        opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });

            //registrar o servi�o, transiente indica que a instancia do servi�o sera criada cada vez que for solicitada
            services.AddTransient<IMeuServico, MeuServico>();

            //registrar o unitOfWork, para cada vez que precisar de uma imlementa��o dessa interface vai dar uma instancia de unitOfWork
            //addScoped - cada request vai criar um novo escopo de servi�o separado, o tempo de vida desse servi�o vai liberar e descartar
            //quando a sua vida �til terminar
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APICatalogo", Version = "v1" });
            });

            //configurar o autommaper e registra como servi�o na startup
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });//criando uma nova configura��o

            IMapper mapper = mappingConfig.CreateMapper(); //habilita o mapeamento
            services.AddSingleton(mapper);//regista como servi�o como singleton: so vai ter uma instancia desse servi�o no projeto
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APICatalogo v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
