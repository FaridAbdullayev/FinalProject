using AutoMapper;
using Data;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Service.Profiles;
using System;
using Service.Dtos.Branch;
using HotelProject.Middlware;
using Service.Services.Interfaces;
using Service.Services;
using Data.Repositories.Interfaces;
using Data.Repositories;
using Services.Services;
using Microsoft.AspNetCore.Mvc;
using static Service.Exceptions.ResetException;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Core.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Service.Dtos.Users;
using Quartz;
using Service.PrintJob;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using HotelProject;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {

        var errors = context.ModelState.Where(x => x.Value.Errors.Count > 0)
        .Select(x => new RestExceptionError(x.Key, x.Value.Errors.First().ErrorMessage)).ToList();

        return new BadRequestObjectResult(new { message = "", errors });
    };
});


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireUppercase = false;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo
    //{
    //    Title = "My API",
    //    Version = "v1"
    //});
    c.SwaggerDoc("admin_v1", new OpenApiInfo
    {
        Title = "Admin API",
        Version = "v1"
    });

    c.SwaggerDoc("user_v1", new OpenApiInfo
    {
        Title = "User API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
});


builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddGoogle(options => {
    options.ClientId = builder.Configuration.GetSection("Google:ClientId").Value!;
    options.CallbackPath = "/signin-google";
    options.ClientSecret = builder.Configuration.GetSection("Google:ClientSecret").Value!;
    options.SaveTokens = true;
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.profile");
    options.Scope.Add("https://www.googleapis.com/auth/userinfo.email");
    options.Events.OnRedirectToAuthorizationEndpoint = context => {
        context.HttpContext.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});






builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MapProfile(provider.GetService<IHttpContextAccessor>(), provider.GetService<IWebHostEnvironment>()));
}).CreateMapper());




builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7089")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<BranchCreateDtoValidator>();


builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();

builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();

builder.Services.AddScoped<IOurStaffService,OurStaffService>();
builder.Services.AddScoped<IOurStaffRepository, OurStaffRepository>();


builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<ISliderRepository, SliderRepository>();


builder.Services.AddScoped<IRoomService, Service.Services.RoomService>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<IBedTypeService, BedTypeService>();
builder.Services.AddScoped<IBedTypeRepository, BedTypeRepository>();

builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ISettingRepository, SettingRepository>();

builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IRoomReviewRepository, RoomReviewRepository>();

builder.Services.AddScoped<IExcelService, ExcelService>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddSignalR();

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});


builder.Services.AddQuartz(options =>
{
    var key = JobKey.Create(nameof(PrintJob));
    options.AddJob<PrintJob>(key)
           .AddTrigger(x => x.ForJob(key).WithCronSchedule("0 0 8 * * ?").StartNow());
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
    options.AwaitApplicationStarted = true;
});

//builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/admin_v1/swagger.json", "Admin API v1");
        options.SwaggerEndpoint("/swagger/user_v1/swagger.json", "User API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();


app.UseCors("AllowSpecificOrigin");
app.MapHub<ReservationHub>("/reservationHub");


app.UseMiddleware<ExceptionHandlerMiddleware>();


app.Run();
