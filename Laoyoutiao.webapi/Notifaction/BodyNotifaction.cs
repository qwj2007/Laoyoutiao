namespace Laoyoutiao.webapi.Notifaction;
using MediatR;

//public class BodyNotificaction:INotification
// {
//     public string body{get;set;}
// }
public record BodyNotification(string body):INotification;
