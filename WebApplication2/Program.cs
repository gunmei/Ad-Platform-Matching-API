using WebApplication2.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(static options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication2 API", Version = "v1" });

    // 👇 Важно: разрешаем работу с формами и файлами
    options.OperationFilter<FileUploadOperationFilter>();
});  // Подключаем Swagger

builder.Services.AddSingleton<AdPlatformService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
