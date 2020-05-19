using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMq.Fast.RabbitMQ;

namespace RabbitMq.Fast
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("请输入角色【0、生产者，1、消费者】：");
            var _cmd = Convert.ToInt32(Console.ReadLine());
            var _option=0;
            List<string> Route = new List<string>();
            string _text;
            switch (_cmd)
            {
                case 0:
                    Console.Write("请选择发布者类型：0、简单队列，1、发布订阅，2、路由模式，3、通配符匹配：");
                    _option = Convert.ToInt32(Console.ReadLine());
                    switch (_option)
                    {
                        case 0:
                            RabbitServer.Productor();
                            break;
                        case 1:
                            RabbitServer.Fanout();
                            break;
                        case 2:
                            //Fast
                            Console.Write("请输入路由名称：");
                             _text = Console.ReadLine();
                            RabbitServer.Direct(_text);
                            break;
                        case 3:
                            //fast.1[,fast.1.2]
                            Console.Write("请输入路由名称：");
                            _text = Console.ReadLine();
                            RabbitServer.Topic(_text);
                            break;
                    }
                    break;
                case 1:
                    Console.Write("请选择消费者类型：0、简单队列，1、发布订阅，2、路由模式，3、通配符匹配：");
                     _option = Convert.ToInt32(Console.ReadLine());
                    string[] _arr;
                    switch (_option)
                    {
                        case 0:
                            Console.Write("请输入是否延迟订阅【0、否，1、是】：");
                            var _bRst = Convert.ToBoolean(Console.ReadLine());
                            RabbitClient.Consumer(_bRst);
                            break;
                        case 1:
                            //Fast
                            Console.Write("请输入队列名称：");
                            _text = Console.ReadLine();
                            RabbitClient.Fanout(false,_text);
                            break;
                        case 2:
                            //fast、Fast
                            Console.Write("请输入队列名称、路由名称【队列与路由用、分隔，多个路由用,分隔】：");
                            _text = Console.ReadLine();
                            _arr = _text.Split('、');
                            RabbitClient.Direct(false,_arr[0],_arr[1].Split(','));
                            break;
                        case 3:
                            //TopicEx、fast、fast.*[,fast.#]=>可以默认一个
                            Console.Write("请输入交换机名称、队列名称、路由名称【（交换机、队列、路由）用、分隔，多个路由用,分隔】：");
                            _text = Console.ReadLine();
                            _arr = _text.Split('、');
                            RabbitClient.Topic(false, _arr[0],_arr[1], _arr[2].Split(','));
                            break;
                    }
                    
                    break;
            }
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
