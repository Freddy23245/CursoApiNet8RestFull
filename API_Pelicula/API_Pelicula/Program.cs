using API_Pelicula.Data;
using API_Pelicula.PeliculaMappers;
using API_Pelicula.Repositorio;
using API_Pelicula.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using XAct.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(
    opciones => opciones.UseSqlServer(builder.Configuration.GetConnectionString("Conexion")));

//Soporte par cache
builder.Services.AddResponseCaching();
//Se Agregan los Repositorios
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IPeliculasRepositorio, PeliculasRepositorio>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secreta");
//Agregamos el AutoMapper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));
// aqui se configura la autenticacion
builder.Services.AddAuthentication
    (
        x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    ).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;// no requiere https para metadata en prod eso tiene que ir en true
        x.SaveToken = true;//debe ser almacenado en el contexto de la aplicacion una vez validado
        //establece los parametros de validacion del token
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,//clave de forma de emisor debe ser validada
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
//perfil Global
builder.Services.AddControllers(options =>
{
//cache profile cahce global asi no tener que ponerlo en todos lados
options.CacheProfiles.Add("PorDefecto20Segundos", new CacheProfile(){ Duration = 20 });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Autenticacion JWT usando el esquema Bearer",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
             new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                 },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }

    });
});

builder.Services.AddCors(p => p.AddPolicy("PoliticaCors", build =>
{
    //podes poner dominios que puede usar la api pones
    //un dominio dentro de la comilla, si es mas de uno lo separas por coma
    // si es publico,es decir que cualquier dominio puede consultarlo pones *
    //                          |
    //                          V
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Soporte para Cors
app.UseCors("PoliticaCors");
//Soporte para Auteticacion
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
