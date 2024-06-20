using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.Exceptions;
using Laoyoutiao.Util;
using Minio.DataModel.Args;
namespace Laoyoutiao.webapi.Controllers
{
    /// <summary>
    /// 文件操作控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileOperateController : ControllerBase
    {
        private static string bucketName = "laoyoutiao";

        private readonly IMinioClient _client;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client"></param>
        public FileOperateController(IMinioClient client)
        {
            _client = client;
        }
        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("/file/UploadFile")]
        public async Task<dynamic> UploadFile(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            try
            {
                //var args = new BucketExistsArgs().WithBucket(bucketName);
                //var found = await _client.BucketExistsAsync(args).ConfigureAwait(false);
                //Console.WriteLine((found ? "Found" : "Couldn't find ") + "bucket " + bucketName);
                //Console.WriteLine();

                //await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                bool found = await MinioUtil.BucketExistsAsync(_client, bucketName);
                //如果桶不存在则创建桶
                if (!found)
                {
                    await MinioUtil.MakeBucketAsync(_client, bucketName);
                }
                foreach (var formFile in files)
                {
                    string saveFileName = $"{Guid.NewGuid():N}{Path.GetExtension(formFile.FileName)}";//存储的文件名
                    string objectName = $"/{DateTime.Now:yyyy/MM/dd}/{saveFileName}";//文件保存路径
                    if (formFile.Length > 0)
                    {
                        Stream stream = formFile.OpenReadStream();
                        await MinioUtil.PutObjectAsync(_client, bucketName, objectName, stream, formFile.Length, formFile.ContentType);
                    }
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("文件上传错误: {0}", e.Message);
            }
            return Ok(new { count = files.Count, size });
        }

    }
}
