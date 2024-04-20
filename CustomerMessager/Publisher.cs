namespace CustomerMessager
{
    /// <summary>
    /// 用委托自定义发布订阅
    /// </summary>
    public class Publisher
    {
        public delegate void MeaaageEventHandler(object sender,MessageEventArgs e);
        public event MeaaageEventHandler OnMessagePublished;
        public void publishMessage(string message)
        {
            MessageEventArgs args = new MessageEventArgs(message);
            OnMessagePublished?.Invoke(this, args);

        }
    }
}
