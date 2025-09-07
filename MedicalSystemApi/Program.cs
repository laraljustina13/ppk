using MedicalSystemApi.Data;
using MedicalSystemApi.Interfaces;
using MedicalSystemApi.Repository;
using MedicalSystemApi.Services;
using Microsoft.EntityFrameworkCore;
using Supabase;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




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

builder.Services.AddHttpClient("Supabase", client =>
{
    var supabaseUrl = builder.Configuration["Supabase:Url"];
    var supabaseKey = builder.Configuration["Supabase:Key"];

    client.BaseAddress = new Uri(supabaseUrl);
    client.DefaultRequestHeaders.Add("apikey", supabaseKey);
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", supabaseKey);
});

//  CORS SA SVIM DOZVOLAMA
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", p => p
        .WithOrigins("http://localhost:5173", "https://localhost:5173") // Vite dev
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS MIDDLEWARE - BITAN REDOSLJED!
app.UseCors("frontend");


app.UseAuthorization();
app.MapControllers();


app.Run();