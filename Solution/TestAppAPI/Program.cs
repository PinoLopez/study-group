using TestApp;          // For StudyGroup, User, Subject
using TestAppAPI;       // For IStudyGroupRepository, StudyGroupRepository

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your repository
builder.Services.AddSingleton<IStudyGroupRepository, StudyGroupRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();