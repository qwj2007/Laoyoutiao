using CustomerMessager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMsgTest
{
    public class CMTest2
    {
        public void Subcriber() {
            Publisher publisher = new Publisher();
            publisher.publishMessage("sdfdsfdsfdskfjsdkljfsdkjfksdljfdskljfdskl");
            Subscriber subscriber1 = new Subscriber();
            Subscriber subcriber2 = new Subscriber();
            subscriber1.Subscribe(publisher);
            subcriber2.Subscribe(publisher);
            
            //publisher.publishMessage("sdfdsfdsfdskfjsdkljfsdkjfksdljfdskljfdskl");
        }
    }
}
