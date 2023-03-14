using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.FanoutExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new();
            factory.Uri=new("amqps://wshfrkdl:mQeHhSM2SYagOsk4ttKUSwjiz3j8AnoV@moose.rmq.cloudamqp.com/wshfrkdl");
            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            //exchange oluştur
            channel.ExchangeDeclare(
                exchange: "fanout-exchange-example",
                type:ExchangeType.Fanout
                );

            Console.WriteLine("Kuyruk adını giriniz: ");
            string queName = Console.ReadLine();

            //kuyruk oluştur
            channel.QueueDeclare(
                queue: queName,
                exclusive:false
                );

            //kuyruğu bind et
            channel.QueueBind(queName, "fanout-exchange-example", string.Empty);



            //Mesajları oku
            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(queName, true, consumer);


            //gelen mesajı receive eventi ile yakala
            consumer.Received += (sender, e) =>
             {
                 string message = Encoding.UTF8.GetString(e.Body.Span);
                 Console.WriteLine(message);
             };
            Console.Read();
        }
    }
}
