namespace BusinessLayer.Interface
{
    public interface IRabbitMQProducer
    {
        void PublishMessage<T>(T message);
    }
}
