using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebApi.Common
{
    /*
     * Desc:http请求数据公共方法
     * Auth:dsy
     * Date:2022-4-7 10:32:12
     */
    public class HttpClientHelper
    {
        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetResponse(string url, out string statusCode)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
              new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        public static string RestfulGet(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            // Get response
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());
                // Console application output
                return reader.ReadToEnd();
            }
        }

        public static T GetResponse<T>(string url)
           where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(
               new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = httpClient.GetAsync(url).Result;

            T result = default(T);

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }
            return result;
        }

        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData, out string statusCode)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";

            HttpClient httpClient = new HttpClient();
            //httpClient..setParameter(HttpMethodParams.HTTP_CONTENT_CHARSET, "utf-8");

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }

            return null;
        }

        /// <summary>
        /// 发起post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url">url</param>
        /// <param name="postData">post数据</param>
        /// <returns></returns>
        public static T PostResponse<T>(string url, string postData)
            where T : class, new()
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();

            T result = default(T);

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                string s = t.Result;

                result = JsonConvert.DeserializeObject<T>(s);
            }
            return result;
        }

        /// <summary>
        /// 反序列化Xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T XmlDeserialize<T>(string xmlString)
            where T : class, new()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xmlString))
                {
                    return (T)ser.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("XmlDeserialize发生异常：xmlString:" + xmlString + "异常信息：" + ex.Message);
            }

        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="token"></param>
        /// <param name="appId"></param>
        /// <param name="serviceURL"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData, string token, string appId, string serviceURL, out string statusCode)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";

            httpContent.Headers.Add("token", token);
            httpContent.Headers.Add("appId", appId);
            httpContent.Headers.Add("serviceURL", serviceURL);


            HttpClient httpClient = new HttpClient();
            //httpClient..setParameter(HttpMethodParams.HTTP_CONTENT_CHARSET, "utf-8");

            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            statusCode = response.StatusCode.ToString();
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }

            return null;
        }

        /// <summary>
        /// 修改API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string ClientPatchResponse(string url, string postData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "PATCH";

            byte[] btBodys = Encoding.UTF8.GetBytes(postData);
            httpWebRequest.ContentLength = btBodys.Length;
            httpWebRequest.GetRequestStream().Write(btBodys, 0, btBodys.Length);

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            var streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string responseContent = streamReader.ReadToEnd();

            httpWebResponse.Close();
            streamReader.Close();
            httpWebRequest.Abort();
            httpWebResponse.Close();

            return responseContent;
        }

        /// <summary>
        /// 创建API
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string ClientPostResponse(string url, string postData)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "utf-8" };
            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// 删除API
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool ClientDeleteResponse(string url)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// 修改或者更改API    
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string ClientPutResponse(string url, string postData)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "utf-8" };

            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// 检索API
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ClientGetResponse(string url)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            var httpClient = new HttpClient();
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            return null;
        }

        /// <summary>
        /// 上传文件请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ClientUploadResponse(string url, MultipartFormDataContent content, IFormFile file)
        {
            using (HttpClient client = new HttpClient())
            {
                //添加字符串参数，参数名为qq
                //content.Add(new StringContent("123456"), "qq");
                string fileName = file.FileName;

                var byteFile = GetBytesByFormFile(file);
                //添加文件参数，参数名为files，文件名为123.png
                content.Add(new ByteArrayContent(byteFile), "file", fileName);

                var requestUri = url;
                var result = client.PostAsync(requestUri, content).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        /// <summary>
        /// 获取文件字节
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetBytesByFormFile(IFormFile file)
        {
            //流
            var stream = file.OpenReadStream();
            //字节
            var bytes = new byte[stream.Length];
            //开始读取字节
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 保存文件到项目路径
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool SaveFile(IFormFileCollection files)
        {
            foreach (var file in files)
            {
                //文件名
                string fileName = file.FileName;
                //扩展名
                var extName = fileName.Substring(fileName.IndexOf("."));

                var bytes = GetBytesByFormFile(file);
                //项目路径
                string path = Environment.CurrentDirectory;
                //路径是否存在
                if (!Directory.Exists(path))
                {
                    //创建目录
                    Directory.CreateDirectory(path);
                }
                //文件目录
                string filePath = path + "\\" + (Guid.NewGuid().ToString() + extName);
                //文件流
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    //开始保存文件流
                    fs.Write(bytes);
                    fs.Close();
                }
            }
            return true;
        }

        #region 下载文件

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="reqPath"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static bool DownloadPicture(string uri,string reqPath,string savePath = "",int timeOut = -1)
        {
            //请求路径
            string picUrl = uri + reqPath;

            //根目录
            string root = AppDomain.CurrentDomain.BaseDirectory;

            //保存路径
            if (string.IsNullOrEmpty(savePath))
            {
                savePath = root + "DownLoadPictures\\" + reqPath;
            }

            string directoryPath = savePath.Substring(0, savePath.LastIndexOf("\\") + 1);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            catch
            {

            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }

        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }

        /// <summary>
        /// 根据路径下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="imageStrCookie"></param>
        /// <returns></returns>
        public Image GetImage(string url, out string imageStrCookie)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://pic.cnblogs.com/avatar/1465512/20200617142308.png");
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            imageStrCookie = "";
            if (response.Headers.HasKeys() && null != response.Headers["Set-Cookie"])
            {
                imageStrCookie = response.Headers.Get("Set-Cookie");
            }
            return Image.FromStream(response.GetResponseStream());

        }

        #endregion

    }
}
