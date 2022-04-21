using System.Reflection;
using ApiDoc.Services.Filters;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                });

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddRouting(options => options.LowercaseUrls = true);

static string GetPathOfXmlFromAssembly() => Path.Combine(AppContext.BaseDirectory, 
                                                            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API with Documentation", Version = "v1" });
                options.IncludeXmlComments(GetPathOfXmlFromAssembly());
            });

builder.Services.AddScoped<IFilters, Filters>();

var app = builder.Build();

app.UseSwagger();
    
app.UseSwaggerUI(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API width Documentation V1");
    // s.RoutePrefix = string.Empty;
});

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
