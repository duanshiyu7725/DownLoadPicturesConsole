using System;
using WebApi.Common;

namespace DownLoadPicturesConsole
{
    /*
     * Desc:下载图片
     * Auth:dsy
     * Date:2022-4-7 10:34:48
     */
    class Program
    {
        static void Main(string[] args)
        {
            //当前程序的根目录
            var root = AppDomain.CurrentDomain.BaseDirectory;

            //测试图片地址
            //https://pica.zhimg.com/v2-fcd104234042f6259ab089121e6da0d0_r.jpg?source=1940ef5c

            //调用公共方法下载图片
            var flag = HttpClientHelper.DownloadPicture("https://pica.zhimg.com", "/v2-fcd104234042f6259ab089121e6da0d0_r.jpg?source=1940ef5c", $"{root}DownLoadPicture\\{DateTime.Now.ToString("yyyy-MM-dd")}\\{Guid.NewGuid().ToString()}.jpg");

            Console.ReadLine();
        }
    }
}
