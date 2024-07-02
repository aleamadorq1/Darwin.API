using Darwin.API.Models;
using Darwin.API.Repositories;
using Darwin.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:3000") // Change this to your React app's URL
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AlphaDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings =>
           {
               warnings.Default(WarningBehavior.Ignore)
                       .Log(CoreEventId.FirstWithoutOrderByAndFilterWarning, CoreEventId.RowLimitingOperationWithoutOrderByWarning)
                       .Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning);
           });
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IModuleMaterialsRepository, ModuleMaterialsRepository>();
builder.Services.AddScoped<IModulesLaborRepository, ModulesLaborRepository>();
builder.Services.AddScoped<IModulesCompositeRepository, ModulesCompositeRepository>();
builder.Services.AddScoped<IProjectMaterialsRepository, ProjectMaterialsRepository>();
builder.Services.AddScoped<IProjectLaborRepository, ProjectLaborRepository>();

builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILaborService, LaborService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<IModuleMaterialsService, ModuleMaterialsService>();
builder.Services.AddScoped<IModulesLaborService, ModulesLaborService>();
builder.Services.AddScoped<IModulesCompositeService, ModulesCompositeService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectDetailsService, ProjectDetailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // Ensure this is added before UseAuthorization and MapControllers

app.UseAuthorization();

// Use the CORS policy
app.UseCors();

app.MapControllers();

app.Run();
