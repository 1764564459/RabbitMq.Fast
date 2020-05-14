using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMq.Fast.RabbitMQ
{
    public class RabbitServer
    {

        /// <summary>
        /// 简单队列生产者
        /// </summary>
        public static void Productor()
        {
            //连接工厂
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName= "guest",
                Password= "guest",
                HostName="127.0.0.1"
            };

            //创建连接
            var connect = factory.CreateConnection();
            //创建管道
            var channel= connect.CreateModel();
            //声明队列
            channel.QueueDeclare("Rabbit", false, false, false);//durable 缓存
            string input;
            do
            {
                Console.Write("请输入发布消息：");
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish("", "Rabbit", null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connect.Close();
        }

        /// <summary>
        /// Exchange发布订阅模式
        /// </summary>
        public static void Fanout()
        {
            //交换机名称
            string _exchange = "FanoutEx";

            //连接工厂
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connect = factory.CreateConnection();
            //创建管道
            var channel = connect.CreateModel();

            //声明发布订阅交换机
            channel.ExchangeDeclare(_exchange, type: "fanout");

            //声明队列
            //channel.QueueDeclare(_queueName, false, false, false);//durable 缓存
            string input;
            do
            {
                Console.Write("请输入发布消息：");
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息【使用交换机】
                channel.BasicPublish(_exchange, "", null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connect.Close();
        }

        /// <summary>
        /// Direct Exchange 路由模式【消费者根据路由名称订阅发布信息】
        /// </summary>
        public static void Direct(string RouteKey)
        {
            //交换机名称
            string _exchange = "DirectEx";

            //连接工厂
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connect = factory.CreateConnection();
            //创建管道
            var channel = connect.CreateModel();

            //声明发布订阅交换机
            channel.ExchangeDeclare(_exchange, type: "direct");

            //声明队列
            //channel.QueueDeclare("Rabbit", false, false, false);//durable 缓存
            string input;
            do
            {
                Console.Write("请输入发布消息：");
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息【使用交换机】
                channel.BasicPublish(_exchange, RouteKey, null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connect.Close();
        }

        /// <summary>
        /// 通配符模式【根据路由通配匹配】
        /// </summary>
        /// <param name="RoutKey">路由名称【使用：fast.1、fast.1.2】</param>
        public static void Topic(string RoutKey)
        {
            //交换机名称
            string _exchange = "TopicEx";

            //路由
            string _routeKey = RoutKey;//"Topic.*";

            //连接工厂
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = "127.0.0.1"
            };

            //创建连接
            var connect = factory.CreateConnection();
            //创建管道
            var channel = connect.CreateModel();

            //声明发布订阅交换机
            channel.ExchangeDeclare(_exchange, type: "topic");

            //声明队列
            //channel.QueueDeclare("Rabbit", false, false, false);//durable 缓存
            string input;
            do
            {
                Console.Write("请输入发布消息：");
                input = Console.ReadLine();

                var sendBytes = Encoding.UTF8.GetBytes(input);
                //发布消息【使用交换机】
                channel.BasicPublish(_exchange, _routeKey, null, sendBytes);

            } while (input.Trim().ToLower() != "exit");
            channel.Close();
            connect.Close();
        }
    }
}
