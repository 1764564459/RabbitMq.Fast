using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMq.Fast.RabbitMQ
{
    public class RabbitClient
    {
        /// <summary>
        ///  简单队列消费者
        /// </summary>
        /// <param name="IsDelay">开启网络延迟</param>
        public static void Consumer(bool IsDelay)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1",
                Port=5672
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            //告诉Rabbit每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息【实现能者多劳（谁处理快谁处理）】
            channel.BasicQos(0, 1, false);

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                //添加处理延迟
                if (IsDelay)
                    Task.Delay(10 * 1000).Wait();
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"收到消息： {message}");
                //确认该消息已被消费【multiple:false(消费者断开，重新投递)】
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为AutoAck:fase手动应答消息【避免消费者断开，存在未处理队列】
            channel.BasicConsume("Rabbit", false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }

        /// <summary>
        ///  Exchange交换机发布订阅模式【通知所有交换机为FanoutEx消费者】
        /// </summary>
        /// <param name="IsDelay">开启网络延迟</param>
        /// <param name="QueueName">队列名称【每个消费者队列名称不同】</param>
        public static void Fanout(bool IsDelay,string QueueName)
        {
            string _exchange = "FanoutEx";

            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(_exchange, type: "fanout");

            //告诉Rabbit每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息【实现能者多劳（谁处理快谁处理）】
            channel.BasicQos(0, 1, false);

            //声明队列
            channel.QueueDeclare(QueueName,false,false,false);

            //队列与交换机绑定
            channel.QueueBind(QueueName, _exchange, "");

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                //添加处理延迟
                if (IsDelay)
                    Task.Delay(10 * 1000).Wait();
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"收到消息： {message}");
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为AutoAck:fase手动应答消息【避免】
            channel.BasicConsume(QueueName, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }


        /// <summary>
        /// Exchange交换机路由匹配【消费者根据路由名称订阅发布信息】
        /// </summary>
        /// <param name="IsDelay">开启网络延迟</param>
        /// <param name="QueueName">队列名称【每个消费者队列名称不同】</param>
        /// <param name="RouteKey">路由名称【指定订阅发布者】</param>
        public static void Direct(bool IsDelay,string QueueName,string[] RouteKey)
        {
            string _exchange = "DirectEx";//交换机名称
            //string _queueName = "Direct";//队列名称


            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(_exchange, type: "direct");

            //告诉Rabbit每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息【实现能者多劳（谁处理快谁处理）】
            channel.BasicQos(0, 1, false);

            //声明队列
            channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            //将队列与交换机进行绑定
            foreach (var item in RouteKey)
            {
                //绑定路由
                channel.QueueBind(QueueName, _exchange, item);
            }

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                //添加处理延迟
                if (IsDelay)
                    Task.Delay(10 * 1000).Wait();
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"收到消息： {message}");
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为AutoAck:fase手动应答消息【避免】
            channel.BasicConsume(QueueName, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }

        /// <summary>
        /// 通配符模式
        /// </summary>
        /// <param name="IsDelay">开启网络延迟</param>
        /// <param name="ExChange">交换器名称【使用：TopicEx】</param>
        /// <param name="QueueName">队列名称【每个消费者队列名称不同,使用：Fast】</param>
        /// <param name="RouteKey">路由名称【指定订阅发布者，使用：fast.#、*（如 ”fast.*“匹配的规则以topic1开始并且"."后只有一段语句的路由  例：“fast.aaa”，“fast.bb”）】</param>
        public static void Topic(bool IsDelay,string ExChange,string QueueName,string [] RouteKey)
        {
            //交换机
            //string _exchange = "TopicEx";

            //string _queueName = "Topic";

            ConnectionFactory factory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(ExChange, type: "topic");

            //声明队列
            channel.QueueDeclare(QueueName, false, false, false);

            //告诉Rabbit每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息【实现能者多劳（谁处理快谁处理）】
            channel.BasicQos(0, 1, false);

            //将队列与交换机进行绑定
            foreach (var item in RouteKey)
            {
                //绑定路由
                channel.QueueBind(QueueName, ExChange, item);
            }

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                //添加处理延迟
                if (IsDelay)
                    Task.Delay(10 * 1000).Wait();
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"收到消息： {message}");
                //确认该消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };
            //启动消费者 设置为AutoAck:fase手动应答消息【避免】
            channel.BasicConsume(QueueName, false, consumer);
            Console.WriteLine("消费者已启动");
            Console.ReadKey();
            channel.Dispose();
            connection.Close();
        }
    }
}
