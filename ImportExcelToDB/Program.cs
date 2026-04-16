using ExcelImageImportToDB;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<DatabaseHelper>();       // 单例或作用域均可
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();   // 使上传的头像可通过URL访问
app.MapControllers();
app.Run();
