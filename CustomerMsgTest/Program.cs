using CustomerMessager;

Publisher publisher = new Publisher();
Subscriber subscriber1 = new Subscriber();
Subscriber subscriber2 = new Subscriber();
subscriber1.Subscribe(publisher);
subscriber2.Subscribe(publisher);
publisher.publishMessage("HelloWord");
