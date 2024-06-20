using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Minio.DataModel.Result;
using Minio.DataModel;
using Minio.DataModel.Notification;
using Minio.DataModel.Encryption;
namespace Laoyoutiao.Util
{
    /// <summary>
    /// Minio工具类
    /// </summary>
    public class MinioUtil
    {
        #region 操作存储桶

        /// <summary>创建存储桶
        /// 创建存储桶
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="loc">可选参数</param>
        /// <returns></returns>
        public async static Task<bool> MakeBucketAsync(IMinioClient minio, string bucketName, string loc = "us-east-1")
        {
            bool flag = false;
            try
            {
                var args = new BucketExistsArgs().WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(args);
                if (found)
                {
                    throw new Exception(string.Format("存储桶[{0}]已存在", bucketName));
                }
                else
                {
                    await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName).WithLocation(loc));
                    flag = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>列出所有的存储桶
        /// 列出所有的存储桶
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <returns></returns>
        public async static Task<Tuple<bool, ListAllMyBucketsResult>> ListBuckets(IMinioClient minio)
        {
            bool flag = false;
            var list = new ListAllMyBucketsResult();
            try
            {
                list = await minio.ListBucketsAsync();
                flag = true;
                //foreach (var bucket in list.Buckets)
                //{
                //    Console.WriteLine($"{bucket.Name} {bucket.CreationDateDateTime}");
                //}
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, list);
        }

        /// <summary>检查存储桶是否存在
        /// 检查存储桶是否存在
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async static Task<bool> BucketExistsAsync(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            try
            {
                var args = new BucketExistsArgs()
                .WithBucket(bucketName);
                flag = await minio.BucketExistsAsync(args).ConfigureAwait(false);
                //flag = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>删除一个存储桶
        /// 删除一个存储桶
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async static Task<bool> RemoveBucket(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var args = new RemoveBucketArgs().WithBucket(bucketName);
                    await minio.RemoveBucketAsync(args);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>列出存储桶里的对象
        /// 列出存储桶里的对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="prefix">对象的前缀</param>
        /// <param name="recursive">true代表递归查找，false代表类似文件夹查找，以'/'分隔，不查子文件夹</param>
        public static Tuple<bool, IObservable<Item>> ListObjects(IMinioClient minio, string bucketName, string prefix = null, bool recursive = true)
        {
            bool flag = false;
            IObservable<Item> observable = null;
            try
            {
                var found = minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found.Result)
                {
                    var args = new ListObjectsArgs().WithBucket(bucketName).WithPrefix(prefix).WithRecursive(recursive);

                    observable = minio.ListObjectsAsync(args);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
                //IDisposable subscription = observable.Subscribe(
                //    item => Console.WriteLine($"Object: {item.Key}"),
                //    ex => Console.WriteLine($"OnError: {ex}"),
                //    () => Console.WriteLine($"Listed all objects in bucket {bucketName}\n"));

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, observable);
        }

        /// <summary>列出存储桶中未完整上传的对象
        /// 列出存储桶中未完整上传的对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="prefix">对象的前缀</param>
        /// <param name="recursive">true代表递归查找，false代表类似文件夹查找，以'/'分隔，不查子文件夹</param>
        /// <returns></returns>
        public static Tuple<bool, IObservable<Upload>> ListIncompleteUploads(IMinioClient minio, string bucketName, string prefix = null, bool recursive = true)
        {
            bool flag = false;
            IObservable<Upload> observable = null;
            try
            {
                var found = minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found.Result)
                {

                    var args = new ListIncompleteUploadsArgs().WithBucket(bucketName).WithPrefix(prefix).WithRecursive(recursive);
                    observable = minio.ListIncompleteUploads(args);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
                //IDisposable subscription = observable.Subscribe(
                //    item => Console.WriteLine($"OnNext: {item.Key}"),
                //    ex => Console.WriteLine($"OnError: {ex.Message}"),
                //    () => Console.WriteLine($"Listed the pending uploads to bucket {bucketName}"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, observable);
        }

        #endregion

        #region 存储桶策略

        /// <summary>
        /// 获取存储桶或者对象前缀的访问权限
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async static Task<Tuple<bool, string>> GetPolicy(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            string policyJson = string.Empty;
            try
            {
                var found = minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found.Result)
                {
                    var arg = new GetPolicyArgs().WithBucket(bucketName);

                    policyJson = await minio.GetPolicyAsync(arg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, policyJson);
        }

        /// <summary>
        /// 针对存储桶和对象前缀设置访问策略
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        public async static Task<bool> SetPolicy(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    string policyJson = $@"{{""Version"":""2012-10-17"",""Statement"":[{{""Action"":[""s3:GetBucketLocation""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:ListBucket""],""Condition"":{{""StringEquals"":{{""s3:prefix"":[""foo"",""prefix/""]}}}},""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}""],""Sid"":""""}},{{""Action"":[""s3:GetObject""],""Effect"":""Allow"",""Principal"":{{""AWS"":[""*""]}},""Resource"":[""arn:aws:s3:::{bucketName}/foo*"",""arn:aws:s3:::{bucketName}/prefix/*""],""Sid"":""""}}]}}";
                    var arg = new SetPolicyArgs().WithBucket(bucketName).WithPolicy(policyJson);
                    await minio.SetPolicyAsync(arg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        #endregion

        #region 存储桶通知

        /// <summary>
        /// 获取存储桶的通知配置
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        private async static Task<Tuple<bool, string>> GetBucketNotification(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            string Ret = string.Empty;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var arg = new GetBucketNotificationsArgs().WithBucket(bucketName);
                    BucketNotification notifications = await minio.GetBucketNotificationsAsync(arg);
                    Ret = notifications.ToXML();
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, Ret);
        }

        /// <summary>
        /// 给存储桶设置通知
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        private async static Task<bool> SetBucketNotification(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    BucketNotification notification = new BucketNotification();
                    Arn topicArn = new Arn("aws", "sns", "us-west-1", "412334153608", "topicminio");

                    TopicConfig topicConfiguration = new TopicConfig(topicArn);
                    List<EventType> events = new List<EventType>() { EventType.ObjectCreatedPut, EventType.ObjectCreatedCopy };
                    topicConfiguration.AddEvents(events);
                    topicConfiguration.AddFilterPrefix("images");
                    topicConfiguration.AddFilterSuffix("jpg");
                    notification.AddTopic(topicConfiguration);

                    //QueueConfig queueConfiguration = new QueueConfig("arn:aws:sqs:us-west-1:482314153608:testminioqueue1");
                    //queueConfiguration.AddEvents(new List<EventType>() { EventType.ObjectCreatedCompleteMultipartUpload });
                    //notification.AddQueue(queueConfiguration);
                    var args = new SetBucketNotificationsArgs().WithBucket(bucketName);
                    await minio.SetBucketNotificationsAsync(args);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 删除存储桶上所有配置的通知
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <returns></returns>
        private async static Task<bool> RemoveAllBucketNotifications(IMinioClient minio, string bucketName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    await minio.RemoveAllBucketNotificationsAsync(new RemoveAllBucketNotificationsArgs().WithBucket(bucketName));
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        #endregion

        #region 操作文件对象

        /// <summary>
        /// 从桶下载文件到本地
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="fileName">本地路径</param>
        /// <param name="sse"></param>
        /// <returns></returns>
        public async static Task<bool> FGetObject(IMinioClient minio, string bucketName, string objectName, string fileName, IServerSideEncryption sse = null)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    var arg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFile(fileName).WithServerSideEncryption(sse);
                    await minio.GetObjectAsync(arg).ConfigureAwait(false);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 上传本地文件至存储桶
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="fileName">本地路径</param>
        /// <returns></returns>
        public async static Task<bool> FPutObject(IMinioClient minio, string bucketName, string objectName, string fileName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var arg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFileName(fileName).WithContentType("application/octet-stream");
                    await minio.PutObjectAsync(arg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        #endregion

        #region Presigned操作

        /// <summary>生成一个给HTTP GET请求用的presigned URL。浏览器/移动端的客户端可以用这个URL进行下载，即使其所在的存储桶是私有的。这个presigned URL可以设置一个失效时间，默认值是7天。
        /// 生成一个给HTTP GET请求用的presigned URL。浏览器/移动端的客户端可以用这个URL进行下载，即使其所在的存储桶是私有的。这个presigned URL可以设置一个失效时间，默认值是7天。
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="expiresInt">失效时间（以秒为单位），默认是7天，不得大于七天</param>
        /// <param name="reqParams">额外的响应头信息，支持response-expires、response-content-type、response-cache-control、response-content-disposition</param>
        /// <returns></returns>
        public async static Task<Tuple<bool, string>> PresignedGetObject(IMinioClient minio, string bucketName, string objectName, int expiresInt = 1000)
        {
            bool flag = false;
            string Ret = string.Empty;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var reqParams = new Dictionary<string, string> { { "response-content-type", "application/json" } };
                    var arg = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiresInt).WithHeaders(reqParams);
                    string presignedUrl = await minio.PresignedGetObjectAsync(arg);
                    Ret = presignedUrl;
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, Ret);
        }

        /// <summary>生成一个给HTTP PUT请求用的presigned URL。浏览器/移动端的客户端可以用这个URL进行上传，即使其所在的存储桶是私有的。这个presigned URL可以设置一个失效时间，默认值是7天。
        /// 生成一个给HTTP PUT请求用的presigned URL。浏览器/移动端的客户端可以用这个URL进行上传，即使其所在的存储桶是私有的。这个presigned URL可以设置一个失效时间，默认值是7天。
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="expiresInt">失效时间（以秒为单位），默认是7天，不得大于七天</param>
        /// <returns></returns>
        public async static Task<Tuple<bool, string>> PresignedPutObject(IMinioClient minio, string bucketName, string objectName, int expiresInt = 1000)
        {
            bool flag = false;
            string Ret = string.Empty;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var arg = new PresignedPutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiresInt);
                    string presignedUrl = await minio.PresignedPutObjectAsync(arg);
                    Ret = presignedUrl;
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return Tuple.Create(flag, Ret);
        }

        /// <summary>
        /// 允许给POST请求的presigned URL设置策略，比如接收对象上传的存储桶名称的策略，key名称前缀，过期策略。
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="PostPolicy">对象的post策略</param>
        /// <returns></returns>
        public async static Task<Tuple<bool, Uri, IDictionary<string, string>>> PresignedPostPolicy(IMinioClient minio)
        {
            bool flag = false;

            //Tuple<Uri, Dictionary<string, string>> tdic = null;
            //Task(Uri, Dictionary<string, string>) dic = null;
            try
            {
                PostPolicy form = new PostPolicy();
                DateTime expiration = DateTime.UtcNow;
                form.SetExpires(expiration.AddDays(10));
                form.SetKey("my-objectname");
                form.SetBucket("my-bucketname");
                var args = new PresignedPostPolicyArgs().WithPolicy(form);

                var tuple = await minio.PresignedPostPolicyAsync(form).ConfigureAwait(false);
                flag = true;
                return Tuple.Create(flag, tuple.Item1, tuple.Item2);
                //dic = await minio.PresignedPostPolicyAsync(form).ConfigureAwait(false);
                //var tuple = await minio.PresignedPostPolicyAsync(form).ConfigureAwait(false);
                //tdic = tuple;


                //string curlCommand = "curl -X POST ";
                //foreach (KeyValuePair<string, string> pair in tuple.Item2)
                //{
                //    curlCommand = curlCommand + $" -F {pair.Key}={pair.Value}";
                //}
                //curlCommand = curlCommand + " -F file=@/etc/bashrc " + tuple.Item1; // https://s3.amazonaws.com/my-bucketname";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            //return Tuple.Create(flag, tdic.Item1, tdic.Item2);
        }

        #endregion

        #region 操作对象

        /// <summary>返回对象数据的流
        /// 返回对象数据的流
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="callback">处理流的回调函数</param>
        /// <returns></returns>
        public async static Task<bool> GetObjectAsync(IMinioClient minio, string bucketName, string objectName, Action<Stream> callback)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var argstat = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
                    await minio.StatObjectAsync(argstat);
                    var arsg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithCallbackStream(callback);
                    await minio.GetObjectAsync(arsg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 下载对象指定区域的字节数组做为流。offset和length都必须传
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="offset">offset 是起始字节的位置</param>
        /// <param name="length">length是要读取的长度</param>
        /// <param name="callback">处理流的回调函数</param>
        /// <returns></returns>
        public async static Task<bool> GetObjectAsync(IMinioClient minio, string bucketName, string objectName, long offset, long length, Action<Stream> callback)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var argstat = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
                    await minio.StatObjectAsync(argstat);
                    var arsg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithOffsetAndLength(offset, length).WithCallbackStream(callback);
                    await minio.GetObjectAsync(arsg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 下载并将文件保存到本地文件系统
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="fileName">本地文件路径</param>
        /// <returns></returns>
        public async static Task<bool> GetObjectAsync(IMinioClient minio, string bucketName, string objectName, string fileName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    var argstat = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
                    await minio.StatObjectAsync(argstat);
                    var arsg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFile(fileName);
                    await minio.GetObjectAsync(arsg);


                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>通过文件上传到对象中
        /// 通过文件上传到对象中
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="filePath">要上传的本地文件名</param>
        /// <param name="contentType">文件的Content type，默认是"application/octet-stream"</param>
        /// <param name="metaData">元数据头信息的Dictionary对象，默认是null</param>
        /// <returns></returns>
        public async static Task<bool> PutObjectAsync(IMinioClient minio, string bucketName, string objectName, string filePath, string contentType = "application/octet-stream", Dictionary<string, string> metaData = null)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var arg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFileName(filePath).WithContentType(contentType).WithHeaders(metaData);
                    await minio.PutObjectAsync(arg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 通过Stream上传对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <param name="data">要上传的Stream对象</param>
        /// <param name="size">流的大小</param>
        /// <param name="contentType">文件的Content type，默认是"application/octet-stream"</param>
        /// <param name="metaData">元数据头信息的Dictionary对象，默认是null</param>
        /// <returns></returns>
        public async static Task<bool> PutObjectAsync(IMinioClient minio, string bucketName, string objectName, Stream data, long size, string contentType = "application/octet-stream", Dictionary<string, string> metaData = null)
        {
           
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    //byte[] bs = File.ReadAllBytes(fileName);
                    //System.IO.MemoryStream filestream = new System.IO.MemoryStream(bs);
                    var arg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithContentType(contentType).WithHeaders(metaData).WithStreamData(data).WithObjectSize(size);
                    await minio.PutObjectAsync(arg);


                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>获取对象的元数据
        /// 获取对象的元数据
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async static Task<bool> StatObjectAsync(IMinioClient minio, string bucketName, string bucketObject)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    ObjectStat statObject = await minio.StatObjectAsync(new StatObjectArgs().WithBucket(bucketName).WithObject(bucketObject));
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 从objectName指定的对象中将数据拷贝到destObjectName指定的对象
        /// </summary>
        /// <param name="minio"></param>
        /// <param name="fromBucketName">源存储桶名称</param>
        /// <param name="fromObjectName">源存储桶中的源对象名称</param>
        /// <param name="destBucketName">目标存储桶名称</param>
        /// <param name="destObjectName">要创建的目标对象名称,如果为空，默认为源对象名称</param>
        /// <param name="copyConditions">拷贝操作的一些条件Map</param>
        /// <param name="sseSrc"></param>
        /// <param name="sseDest"></param>
        /// <returns></returns>
        public async static Task<bool> CopyObject(IMinioClient minio, string fromBucketName, string fromObjectName, string destBucketName, string destObjectName, CopyConditions copyConditions = null, IServerSideEncryption sseSrc = null, IServerSideEncryption sseDest = null)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(fromBucketName));
                if (!found)
                {
                    throw new Exception(string.Format("源存储桶[{0}]不存在", fromBucketName));
                }
                bool foundtwo = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(destBucketName));
                if (!foundtwo)
                {
                    throw new Exception(string.Format("目标存储桶[{0}]不存在", destBucketName));
                }

                Console.WriteLine("Running example for API: CopyObjectAsync");
                var metaData = new Dictionary<string, string>
                    (StringComparer.Ordinal) { { "Test-Metadata", "Test  Test" } };
                // Optionally pass copy conditions
                var cpSrcArgs = new CopySourceObjectArgs()
                    .WithBucket(fromBucketName)
                    .WithObject(fromObjectName)
                    .WithServerSideEncryption(sseSrc);
                var args = new CopyObjectArgs()
                    .WithBucket(destBucketName)
                    .WithObject(destObjectName)
                    .WithCopyObjectSource(cpSrcArgs)
                    .WithServerSideEncryption(sseDest);
                await minio.CopyObjectAsync(args).ConfigureAwait(false);

                //var args = new CopyObjectArgs().WithBucket(fromBucketName).WithObject(fromObjectName).WithBucket(destBucketName).WithObject(destObjectName).WithServerSideEncryption(sseSrc).WithServerSideEncryption(sseSrc);
                //await minio.CopyObjectAsync(args);
                flag = true;
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async static Task<bool> RemoveObjectAsync(IMinioClient minio, string bucketName, string objectName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {

                    await minio.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName));
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 删除多个对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectsList">含有多个对象名称的IEnumerable</param>
        /// <returns></returns>
        public static async Task<bool> RemoveObjectsAsync(IMinioClient minio, string bucketName, List<string> objectsList)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    if (objectsList != null)
                    {
                        var args = new RemoveObjectsArgs().WithBucket(bucketName).WithObjects(objectsList);
                        await minio.RemoveObjectsAsync(args).ConfigureAwait(false);
                        flag = true;
                        //IDisposable objectsSubscription = objectsOservable.Subscribe(
                        //    objDeleteError => Console.WriteLine($"Object: {objDeleteError.Key}"),
                        //        ex => Console.WriteLine($"OnError: {ex}"),
                        //        () =>
                        //        {
                        //            Console.WriteLine($"Removed objects in list from {bucketName}\n");
                        //        });
                        //return;
                    }
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }

        /// <summary>
        /// 删除一个未完整上传的对象
        /// </summary>
        /// <param name="minio">连接实例</param>
        /// <param name="bucketName">存储桶名称</param>
        /// <param name="objectName">存储桶里的对象名称</param>
        /// <returns></returns>
        public async static Task<bool> RemoveIncompleteUpload(IMinioClient minio, string bucketName, string objectName)
        {
            bool flag = false;
            try
            {
                bool found = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
                if (found)
                {
                    var arg = new RemoveIncompleteUploadArgs().WithBucket(bucketName).WithObject(objectName);
                    await minio.RemoveIncompleteUploadAsync(arg);
                    flag = true;
                }
                else
                {
                    throw new Exception(string.Format("存储桶[{0}]不存在", bucketName));
                }
            }
            catch (MinioException e)
            {
                throw new Exception(e.Message);
            }
            return flag;
        }
        #endregion
    }
}