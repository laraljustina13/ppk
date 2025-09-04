using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Repository;
using MedicalSystemApi.Services;
using Microsoft.EntityFrameworkCore;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// NADOPSI postojeæi CORS sa ovim:
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5173",  // Vue dev server
                    "https://localhost:7048", // Backend
                    "http://localhost:7048"   // Backend (HTTP)
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  CORS SA SVIM DOZVOLAMA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLazyLoadingProxies());

// Add Repository Factory and specific repositories
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IExaminationFileRepository, ExaminationFileRepository>();
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

// Add Supabase client
builder.Services.AddScoped(provider =>
{
    var supabaseUrl = builder.Configuration["Supabase:Url"];
    var supabaseKey = builder.Configuration["Supabase:Key"];

    return new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions
    {
        AutoConnectRealtime = true
    });
});

// Add file storage service
builder.Services.AddScoped<IFileStorageService, SupabaseFileStorageService>();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS MIDDLEWARE - BITAN REDOSLJED!
app.UseCors("AllowAll");

app.UseCors("AllowVueDevServer");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// DODATNA CORS SIGURNOST
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
    context.Response.Headers.Append("Access-Control-Allow-Methods", "*");
    await next();
});

app.Run();