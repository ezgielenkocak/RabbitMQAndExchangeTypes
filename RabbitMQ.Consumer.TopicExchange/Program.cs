using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.TopicExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new();
            factory.Uri=new("amqps://wshfrkdl:mQeHhSM2SYagOsk4ttKUSwjiz3j8AnoV@moose.rmq.cloudamqp.com/wshfrkdl");

            using IConnection connection = factory.CreateConnection();
            using IModel channnel = connection.CreateModel();

            channnel.ExchangeDeclare(
                exchange: "topic-exchange-example",
                type:ExchangeType.Topic
                );
            Console.WriteLine("Dinlenecek topic formatını yazın:");
            string topic = Console.ReadLine(); //routing key

            string queName=channnel.QueueDeclare().QueueName;

            channnel.QueueBind(queName, "topic-exchange-example",topic); //que ile exchange'i bind ettik


            //Bundan sonra routingKey:topic formatına uygun gelen mesajları bu consumerla yakalayıp receive edicez
            EventingBasicConsumer consumer = new(channnel);
            channnel.BasicConsume(queue:queName, autoAck: true, consumer);

            //mesajlar geldikçe tüketicez- mesaj geldikçe bu event tetiklenecek.
            consumer.Received += (sender, e)=>
            {
                string message = Encoding.UTF8.GetString(e.Body.Span);
                Console.WriteLine(message);
            };
            Console.Read();
        }
    }
}
