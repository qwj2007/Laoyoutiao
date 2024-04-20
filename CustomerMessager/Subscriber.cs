using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMessager
{
    public class Subscriber
    {
        //处理接受到的消息
        public void OnMessageReceived(object sender,MessageEventArgs e) 
        {
            Console.WriteLine($"Received message:{e.Message}");
    }
        public void Subscribe(Publisher publisher) {
            publisher.OnMessagePublished += OnMessageReceived;
        } 
    }
}
