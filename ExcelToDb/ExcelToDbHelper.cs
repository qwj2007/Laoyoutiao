using Microsoft.VisualBasic;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToDb
{
    // 用户实体
    public class UserInfo
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string AvatarPath { get; set; }   // 图片保存路径
        public string Address { get; set; }
    }

    // 导入结果
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    public class ExcelImporter
    {
        private readonly string _imageSaveFolder;
        private readonly string _dbConnectionString;

        /// <summary>
        /// 构造导入器
        /// </summary>
        /// <param name="imageSaveFolder">图片保存的文件夹路径（必须存在）</param>
        /// <param name="dbConnectionString">数据库连接字符串</param>
        public ExcelImporter(string imageSaveFolder, string dbConnectionString)
        {
            _imageSaveFolder = imageSaveFolder;
            _dbConnectionString = dbConnectionString;
            // 确保图片保存目录存在
            if (!Directory.Exists(_imageSaveFolder))
                Directory.CreateDirectory(_imageSaveFolder);
        }

        /// <summary>
        /// 导入 Excel 文件
        /// </summary>
        /// <param name="excelFilePath">Excel 文件路径</param>
        /// <returns>导入结果</returns>
        public ImportResult Import(string excelFilePath)
        {
            var result = new ImportResult();
            IWorkbook workbook = null;
            try
            {
                // 1. 根据扩展名创建工作簿
                using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
                {
                    if (excelFilePath.EndsWith(".xlsx"))
                        workbook = new XSSFWorkbook(fs);
                    else if (excelFilePath.EndsWith(".xls"))
                        workbook = new HSSFWorkbook(fs);
                    else
                        throw new NotSupportedException("仅支持 .xls 和 .xlsx 格式");
                }

                ISheet sheet = workbook.GetSheetAt(0);
                IRow headerRow = sheet.GetRow(0);
                if (headerRow == null)
                    throw new Exception("Excel 文件为空或无表头");

                // 2. 获取表头列索引
                int userNameCol = -1, nickNameCol = -1, avatarCol = -1, addressCol = -1;
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    var cell = headerRow.GetCell(i);
                    if (cell != null)
                    {
                        string headerText = cell.ToString().Trim();
                        if (headerText.Contains("用户名")) userNameCol = i;
                        else if (headerText.Contains("昵称")) nickNameCol = i;
                        else if (headerText.Contains("头像")) avatarCol = i;
                        else if (headerText.Contains("住址")) addressCol = i;
                    }
                }

                // 检查必要列是否存在
                if (userNameCol == -1 || nickNameCol == -1 || avatarCol == -1 || addressCol == -1)
                    throw new Exception("Excel 表头缺少必要的列（用户名、昵称、头像、住址）");

                // 3. 提取工作表中的图片映射 (行索引, 列索引) -> 图片字节数组
                var imageMap = ExtractImageMap(workbook, sheet);

                // 4. 创建数据库表（如果不存在）
                CreateDatabaseTable();

                // 5. 遍历数据行
                for (int rowIdx = 1; rowIdx <= sheet.LastRowNum; rowIdx++)
                {
                    IRow dataRow = sheet.GetRow(rowIdx);
                    if (dataRow == null) continue;

                    try
                    {
                        // 读取文本字段
                        string userName = GetCellString(dataRow.GetCell(userNameCol));
                        string nickName = GetCellString(dataRow.GetCell(nickNameCol));
                        string address = GetCellString(dataRow.GetCell(addressCol));

                        // 简单非空校验
                        if (string.IsNullOrWhiteSpace(userName))
                            throw new Exception("用户名为空");

                        // 获取头像图片
                        string avatarPath = null;
                        if (imageMap.TryGetValue((rowIdx, avatarCol), out byte[] imageBytes))
                        {
                            avatarPath = SaveImage(imageBytes);
                        }
                        else
                        {
                            // 如果没有图片，根据需求可跳过或赋默认值
                            // 此处视为可选，不报错
                        }

                        // 保存到数据库
                        var user = new UserInfo
                        {
                            UserName = userName,
                            NickName = nickName,
                            AvatarPath = avatarPath,
                            Address = address
                        };
                        InsertUser(user);

                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailCount++;
                        result.Errors.Add($"第 {rowIdx + 1} 行导入失败：{ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"导入过程发生严重错误：{ex.Message}");
            }
            finally
            {
                workbook?.Close();
            }
            return result;
        }

        /// <summary>
        /// 从 Excel 中提取所有图片及其所在单元格位置（左上角锚点行列）
        /// </summary>
        private Dictionary<(int row, int col), byte[]> ExtractImageMap(IWorkbook workbook, ISheet sheet)
        {
            var imageMap = new Dictionary<(int, int), byte[]>();

            if (sheet is XSSFSheet xssfSheet)
            {
                var drawing = xssfSheet.DrawingPatriarch as XSSFDrawing;
                if (drawing != null)
                {
                    var shapes = drawing.GetShapes();
                    foreach (var shape in shapes)
                    {
                        if (shape is XSSFPicture picture)
                        {
                            var picData = picture.PictureData?.Data;
                            if (picData == null) continue;

                            int row = -1, col = -1;
                            try
                            {
                                // 可能在某些 NPOI 版本抛出 NotImplementedException
                                var anchor = picture.Anchor;
                                if (anchor is XSSFClientAnchor ca)
                                {
                                    row = ca.Row1;
                                    col = ca.Col1;
                                }
                            }
                            catch (NotImplementedException)
                            {
                                // 降级：不映射位置，或者后续通过 CT_Drawing 解析再映射
                            }

                            if (row >= 0 && col >= 0)
                                imageMap[(row, col)] = picData;
                            else
                                imageMap[(-1, -1)] = picData; // 或把无位置信息的图片另存到列表
                        }
                    }
                }
            }
            else if (sheet is HSSFSheet hssfSheet)
            {
                // HSSF（.xls）处理方式
                var patriarch = hssfSheet.DrawingPatriarch as HSSFPatriarch;
                if (patriarch != null)
                {
                    var shapes = patriarch.Children;
                    foreach (var shape in shapes)
                    {
                        if (shape is HSSFPicture picture)
                        {
                            var anchor = picture.Anchor as HSSFClientAnchor;
                            if (anchor != null)
                            {
                                int row = anchor.Row1;    // 图片左上角所在行
                                int col = anchor.Col1;    // 图片左上角所在列
                                var pictureData = picture.PictureData;
                                if (pictureData != null)
                                {
                                    imageMap[(row, col)] = pictureData.Data;
                                }
                            }
                        }
                    }
                }
            }
            return imageMap;
        }

        /// <summary>
        /// 根据图片字节数组保存到文件系统，返回保存路径
        /// </summary>
        private string SaveImage(byte[] imageBytes)
        {
            // 检测图片格式（简单通过文件头判断，此处默认 png）
            string extension = ".png";
            if (imageBytes.Length >= 2 && imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                extension = ".jpg";
            else if (imageBytes.Length >= 4 && imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                extension = ".png";
            else if (imageBytes.Length >= 2 && imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                extension = ".bmp";

            string fileName = Guid.NewGuid().ToString() + extension;
            string fullPath = Path.Combine(_imageSaveFolder, fileName);
            File.WriteAllBytes(fullPath, imageBytes);
            return fullPath; // 返回绝对路径，也可改为相对路径
        }

        /// <summary>
        /// 安全获取单元格字符串值
        /// </summary>
        private string GetCellString(ICell cell)
        {
            if (cell == null) return null;
            switch (cell.CellType)
            {
                case CellType.String: return cell.StringCellValue?.Trim();
                case CellType.Numeric: return cell.NumericCellValue.ToString();
                case CellType.Boolean: return cell.BooleanCellValue.ToString();
                case CellType.Formula: return cell.StringCellValue?.Trim(); // 先尝试取值
                default: return null;
            }
        }

        /// <summary>
        /// 创建 SQLite 用户表（如果不存在）
        /// </summary>
        private void CreateDatabaseTable()
        {
            using (var conn = new SQLiteConnection(_dbConnectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserName TEXT NOT NULL,
                        NickName TEXT,
                        AvatarPath TEXT,
                        Address TEXT
                    )";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 插入用户记录到数据库
        /// </summary>
        private void InsertUser(UserInfo user)
        {
            using (var conn = new SQLiteConnection(_dbConnectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Users (UserName, NickName, AvatarPath, Address)
                               VALUES (@UserName, @NickName, @AvatarPath, @Address)";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", user.UserName);
                    cmd.Parameters.AddWithValue("@NickName", user.NickName ?? "");
                    cmd.Parameters.AddWithValue("@AvatarPath", user.AvatarPath ?? "");
                    cmd.Parameters.AddWithValue("@Address", user.Address ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

    // 控制台示例调用
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        // 配置参数
    //        string excelFile = @"C:\test\users.xlsx";
    //        string imageFolder = @"C:\test\Images";
    //        string dbConnection = @"Data Source=C:\test\users.db;Version=3;";

    //        var importer = new ExcelImporter(imageFolder, dbConnection);
    //        var result = importer.Import(excelFile);

    //        Console.WriteLine($"导入完成：成功 {result.SuccessCount} 行，失败 {result.FailCount} 行");
    //        if (result.Errors.Any())
    //        {
    //            Console.WriteLine("错误详情：");
    //            foreach (var err in result.Errors)
    //                Console.WriteLine(err);
    //        }
    //    }
    //}
}
