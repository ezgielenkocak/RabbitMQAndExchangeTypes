using System;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace RabbitMQ.Consume2
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new(); //bağlantı oluşturmak için factory classını kullandım
            factory.Uri = new("amqps://wshfrkdl:mQeHhSM2SYagOsk4ttKUSwjiz3j8AnoV@moose.rmq.cloudamqp.com/wshfrkdl"); //servera bağlandım

            using IConnection connection = factory.CreateConnection(); // connection nesnesi oluşturdum ve bağlantıyı aktifleştirdim
            using IModel channel = connection.CreateModel();// kanal açtım channel: connction üstünde işlem yapmamızı sağlar

            //1.ADIM
            channel.ExchangeDeclare(exchange: "direct-exchange-example", type: ExchangeType.Direct); //publisherın aynı isim ve aynı type olacak şekilde exchange tanımlarım.


            //2.ADIM Kuyruk olutşruuruz.
            var queueName=channel.QueueDeclare().QueueName; //routing key'gönderilen mesajı bu queue'ya yönlendiricem.



            //3.ADIM
            channel.QueueBind(
                queue: queueName, 
                exchange: "direct-exchange-example",
                routingKey: "direct-queue-example");

            //receive operation
            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(queue:queueName, autoAck:true, consumer:consumer);

            consumer.Received += (sender, e) =>
             {
                 string message = Encoding.UTF8.GetString(e.Body.Span);
                 Console.WriteLine(message);
             };
            Console.Read();

        }
    }
}
