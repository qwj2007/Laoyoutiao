using ExcelImageImportToDB;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Drawing;
using System.Text;


public class ExcelImportService
{
    private readonly IWebHostEnvironment _env;
    private readonly DatabaseHelper _dbHelper;

    public ExcelImportService(IWebHostEnvironment env, DatabaseHelper dbHelper)
    {
        _env = env;
        _dbHelper = dbHelper;        
        ExcelPackage.License.SetNonCommercialPersonal("gddd");


    }

    /// <summary>
    /// 导入Excel文件，文本字段和图片
    /// </summary>
    /// <param name="fileStream">Excel文件流</param>
    /// <returns>导入成功的记录数</returns>
    public async Task<int> ImportExcelAsync(Stream fileStream)
    {
        var users = new List<User>();
        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0]; // 第一个工作表

        // 1. 确定列索引（通过第一行表头）
        int userNameCol = -1, nicknameCol = -1, avatarCol = -1, addressCol = -1;
        var headerRow = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns];
        foreach (var cell in headerRow)
        {
            switch (cell.Text.Trim())
            {
                case "用户名": userNameCol = cell.Start.Column; break;
                case "昵称": nicknameCol = cell.Start.Column; break;
                case "头像": avatarCol = cell.Start.Column; break;
                case "住址": addressCol = cell.Start.Column; break;
            }
        }

        if (userNameCol == -1 || nicknameCol == -1 || addressCol == -1)
            throw new Exception("Excel表头缺少必要列：用户名、昵称、住址");

        // 2. 构建图片位置映射 (行号 -> 图片字节数组)
        var pictureMap = new Dictionary<int, byte[]>();
        foreach (var drawing in worksheet.Drawings)
        {
            if (drawing is ExcelPicture excelPic && excelPic.From.Row > 0)
            {
                int rowIndex = excelPic.From.Row;
                int colIndex = excelPic.From.Column;
                if (avatarCol != -1 && colIndex == avatarCol)
                {
                    byte[] imageBytes = null;
                    var excelImage = excelPic.Image;
                    if (excelImage != null)
                    {
                        // 尝试通过反射读取可能存在的 ImageBytes 属性（某些 EPPlus 版本可能有）
                        var pi = excelImage.GetType().GetProperty("ImageBytes");
                        if (pi != null)
                        {
                            imageBytes = pi.GetValue(excelImage) as byte[];
                        }
                        else
                        {
                            // 如果不存在 ImageBytes，则尝试读取 Image 属性并保存为 PNG
                            var imgProp = excelImage.GetType().GetProperty("Image");
                            if (imgProp != null)
                            {
                                var drawingImage = imgProp.GetValue(excelImage) as System.Drawing.Image;
                                if (drawingImage != null)
                                {
                                    using var ms = new MemoryStream();
                                    drawingImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                    imageBytes = ms.ToArray();
                                }
                            }
                        }
                    }

                    if (imageBytes != null)
                    {
                        pictureMap[rowIndex] = imageBytes;
                    }
                }
            }
        }

        // 3. 遍历数据行（从第2行开始）
        int startRow = 2;
        int totalRows = worksheet.Dimension.Rows;
        for (int row = startRow; row <= totalRows; row++)
        {
            var userName = worksheet.Cells[row, userNameCol].Text?.Trim();
            if (string.IsNullOrEmpty(userName)) continue; // 用户名为空则跳过整行

            var nickname = worksheet.Cells[row, nicknameCol].Text?.Trim();
            var address = worksheet.Cells[row, addressCol].Text?.Trim();

            string avatarPath = null;
            // 检查当前行是否有图片
            if (pictureMap.TryGetValue(row, out var imageBytes))
            {
                // 保存图片到文件系统，文件名使用 用户名_时间戳.png
                string ext = GetImageExtension(imageBytes);
                string fileName = $"{userName}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                string filePath = Path.Combine(uploadsFolder, fileName);
                await File.WriteAllBytesAsync(filePath, imageBytes);
                avatarPath = $"/uploads/avatars/{fileName}"; // 相对路径用于访问
            }

            var user = new User
            {
                UserName = userName,
                Nickname = nickname,
                AvatarPath = avatarPath,
                Address = address
            };
            users.Add(user);
        }

        // 4. 批量保存到数据库
        int insertedCount = 0;
        foreach (var user in users)
        {
            await _dbHelper.InsertUserAsync(user);
            insertedCount++;
        }

        return insertedCount;
    }

    /// <summary>
    /// 根据文件头判断图片扩展名
    /// </summary>
    private string GetImageExtension(byte[] imageBytes)
    {
        if (imageBytes.Length < 8) return ".png";
        // PNG
        if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
            return ".png";
        // JPEG
        if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
            return ".jpg";
        // GIF
        if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
            return ".gif";
        // BMP
        if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
            return ".bmp";
        return ".png";
    }
}