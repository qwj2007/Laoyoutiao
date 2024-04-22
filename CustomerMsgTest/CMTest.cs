using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMessager
{
    public class CMTest
    {

        public void publisher()
        {
            Publisher publisher = new Publisher();
            publisher.publishMessage("消息发送了。。。。。。");

        }
    }
}
