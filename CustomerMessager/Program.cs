using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMessager
{
    public class Program
    {
        public static void Main(string[] args)
        {
         

            Publisher publisher = new Publisher();
            publisher.publishMessage("HelloWord");
            Subscriber subscriber1 = new Subscriber();
            Subscriber subscriber2 = new Subscriber();
            subscriber1.Subscribe(publisher);
            subscriber2.Subscribe(publisher);
        }
    }
}
