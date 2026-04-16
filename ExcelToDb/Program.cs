using ExcelToDb;

// 配置参数
string excelFile = @"E:\\11.xlsx";
string imageFolder = @"E:\\test\\Images";
string dbConnection = @"Data Source=E:\test\users.db;Version=3;";

var importer = new ExcelImporter(imageFolder, dbConnection);
var result = importer.Import(excelFile);

Console.WriteLine($"导入完成：成功 {result.SuccessCount} 行，失败 {result.FailCount} 行");
if (result.Errors.Any())
{
    Console.WriteLine("错误详情：");
    foreach (var err in result.Errors)
        Console.WriteLine(err);
}