namespace Laoyoutiao.webapi.Notifaction;
using MediatR;

//public class BodyNotificaction:INotification
// {
//     public string body{get;set;}
// }
/// <summary>
/// 
/// </summary>
/// <param name="body"></param>
public record BodyNotification(string body):INotification;
