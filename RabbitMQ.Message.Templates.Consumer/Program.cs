using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Message.Templates.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new();
            factory.Uri=new("amqps://wshfrkdl:mQeHhSM2SYagOsk4ttKUSwjiz3j8AnoV@moose.rmq.cloudamqp.com/wshfrkdl");

            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();


            #region P2P (Point-to-Point) Tasarımı
            //string queName = "example-p2p-queue";
            //channel.QueueDeclare(
            //    queue: queName,
            //    durable: false, //kalıcılık göstermedik
            //    exclusive: false,
            //    autoDelete: false
            //    );

            //EventingBasicConsumer consumer = new(channel);
            //channel.BasicConsume(
            //    queue:queName,
            //    autoAck:false,
            //    consumer:consumer
            //    );

            //consumer.Received += (sender, e) =>
            // {
            //     Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
            // };
            #endregion

            #region  Publish/Subscribe(Pub-Sub) Tasarımı
            //string exchangeName = "example-pub-sub-exchange";

            //channel.ExchangeDeclare(
            //    exchange: exchangeName,
            //    type: ExchangeType.Fanout
            //    );

            //string queName = channel.QueueDeclare().QueueName;
            //channel.QueueBind(
            //    queue:queName,
            //    exchange:exchangeName,
            //    routingKey:string.Empty
            //    );

            //EventingBasicConsumer consumer = new(channel);
            //channel.BasicConsume(
            //    queue:queName,
            //    autoAck:false,
            //    consumer:consumer
            //    );
            //consumer.Received += (sender, e) =>
            // {
            //     Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
            // };
            #endregion

            #region Work-Queue(İş Kuyruğu) Tasarımı
            //string queName = "example-work-queue";

            //channel.QueueDeclare(
            //   queue: queName,
            //   durable: false,
            //   exclusive: false,
            //   autoDelete: false
            //   );

            //EventingBasicConsumer consumer = new(channel);
            //channel.BasicConsume(
            //    queue:queName,
            //    autoAck:true,
            //    consumer:consumer
            //    );

            //channel.BasicQos(
            //    prefetchCount:1 ,//her bir consumer 1 mesaj işleyecek
            //    prefetchSize:0, //her consumer totalde sınırsız mesaj alabilecek
            //    global:false     
            //    );

            //consumer.Received += (sender, e) =>
            // {
            //     Console.WriteLine(Encoding.UTF8.GetString(e.Body.Span));
            // };
            #endregion

            #region Request-Response Tasarımı
            string requestQueName = "example-request-response-queue";

            channel.QueueDeclare(
                queue: requestQueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(
                queue:requestQueName,
                autoAck:true,
                consumer:consumer
                );

            consumer.Received += (sender, e) =>
             {
                string message= Encoding.UTF8.GetString(e.Body.Span);
                 Console.WriteLine(message);

                 byte[] responseMessage = Encoding.UTF8.GetBytes($"İşlem tamamlandı. {message}");
                 IBasicProperties properties=  channel.CreateBasicProperties();
                 properties.CorrelationId = e.BasicProperties.CorrelationId;
                 channel.BasicPublish(
                     exchange:string.Empty,
                     routingKey:e.BasicProperties.ReplyTo,
                     basicProperties:properties,
                     body:responseMessage
                     );

             };

            #endregion
            Console.Read();

        }
    }
}
