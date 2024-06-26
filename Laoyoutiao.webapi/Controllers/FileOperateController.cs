﻿using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.Exceptions;
using Laoyoutiao.Util;
using Minio.DataModel.Args;
using Laoyoutiao.Common;
using System.Drawing;
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
            string url = _client.Config.Endpoint;
            long size = files.Sum(f => f.Length);
            try
            {

                
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
                        url += "/"+bucketName+"/"+objectName;
                    }
                }
                
            }
            catch (MinioException ex)
            {
                throw ex;
            }
            return ResultHelper.Success(new { count = files.Count, size=size,url= url });

        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("/File/DownloadFile")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var memoryStream = new MemoryStream();

            try
            {
                await MinioUtil.StatObjectAsync(_client, bucketName, fileName);
                await MinioUtil.GetObjectAsync(_client, bucketName, fileName,
                                    (stream) =>
                                    {
                                        stream.CopyTo(memoryStream);
                                    });
                memoryStream.Position = 0;

            }
            catch (MinioException e)
            {
                throw new MinioException("下载文件出错");
            }
            return File(memoryStream, GetContentType(fileName));

        }
        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="bucketName">桶名</param>
        /// <param name="objectName">文件路径如：/2024/06/20/2a8452619e0d40298bdfc664288bf28a.jpg</param>
        /// <returns></returns>
        [HttpPost("/File/RemoveFile")]
        public async Task<dynamic> RemoveFile(string bucketName, string objectName)
        {
            bool result = false;
            try
            {
                result= await MinioUtil.RemoveObjectAsync(_client, bucketName, objectName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ResultHelper.Success(result);
        }
        /// <summary>
        /// 删除多个文件
        /// </summary>
        /// <param name="bucketName">桶名</param>
        /// <param name="objectNames">文件路径列表</param>
        /// <returns></returns>
        [HttpPost("/File/RemoveFiles")]
        public async Task<dynamic> RemoveFiles(string bucketName, List<string> objectNames)
        {
            bool result = false;
            try
            {
                result = await MinioUtil.RemoveObjectsAsync(_client, bucketName, objectNames);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ResultHelper.Success(result);
        }
        private static string GetContentType(string fileName)
        {
            if (fileName.Contains(".jpg"))
            {
                return "image/jpg";
            }
            else if (fileName.Contains(".jpeg"))
            {
                return "image/jpeg";
            }
            else if (fileName.Contains(".png"))
            {
                return "image/png";
            }
            else if (fileName.Contains(".gif"))
            {
                return "image/gif";
            }
            else if (fileName.Contains(".pdf"))
            {
                return "application/pdf";
            }
            else
            {
                return "application/octet-stream";
            }
        }
    }
}
