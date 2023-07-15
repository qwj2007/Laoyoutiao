namespace Laoyoutiao.webapi.NotifactionHandler;

using System.Threading;
using System.Threading.Tasks;
using Laoyoutiao.webapi.Notifaction;
using MediatR;
/// <summary>
/// 
/// </summary>
public class BodyNotifactionHandler : NotificationHandler<BodyNotification>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="notification"></param>
    protected override void Handle(BodyNotification notification)
    {
       Console.WriteLine("接收到消息："+notification.body+":时间："+DateTime.Now);
    }
}
