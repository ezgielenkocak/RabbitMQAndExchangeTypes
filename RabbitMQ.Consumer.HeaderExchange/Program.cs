using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Consumer.HeaderExchange
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new();
            factory.Uri=new("amqps://wshfrkdl:mQeHhSM2SYagOsk4ttKUSwjiz3j8AnoV@moose.rmq.cloudamqp.com/wshfrkdl");

            using IConnection connection = factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("header-exchange-example", type: ExchangeType.Headers);

            Console.WriteLine("Header valuesını giriniz");
            var value = Console.ReadLine();

            //kuyruk oluşturdum adını aldım
            var queName=channel.QueueDeclare().QueueName;

            //oluşturduğumuz kuyrukla exchangei bind edicez
            channel.QueueBind(
                queue:queName,
                exchange: "header-exchange-example",
                routingKey:string.Empty,
                new Dictionary<string, object>
                 {
                    ["x-match"]="all", //defaılt olarak any gelir
                    ["no"]=value //publisher ve consumerdaki key adları aynı olmalı eşleşme için
                 }
                );

            EventingBasicConsumer consumer = new(channel);
            channel.BasicConsume(
                queue: queName,
                autoAck: true,
                consumer: consumer
                );

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.Span);
                Console.WriteLine(message);
            };
            Console.Read();
        }
    }
}
