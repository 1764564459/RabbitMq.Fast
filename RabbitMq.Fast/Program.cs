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
            Console.Write("�������ɫ��0�������ߣ�1�������ߡ���");
            var _cmd = Convert.ToInt32(Console.ReadLine());
            var _option=0;
            List<string> Route = new List<string>();
            string _text;
            switch (_cmd)
            {
                case 0:
                    Console.Write("��ѡ�񷢲������ͣ�0���򵥶��У�1���������ģ�2��·��ģʽ��3��ͨ���ƥ�䣺");
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
                            Console.Write("������·�����ƣ�");
                             _text = Console.ReadLine();
                            RabbitServer.Direct(_text);
                            break;
                        case 3:
                            //fast.1[,fast.1.2]
                            Console.Write("������·�����ƣ�");
                            _text = Console.ReadLine();
                            RabbitServer.Topic(_text);
                            break;
                    }
                    break;
                case 1:
                    Console.Write("��ѡ�����������ͣ�0���򵥶��У�1���������ģ�2��·��ģʽ��3��ͨ���ƥ�䣺");
                     _option = Convert.ToInt32(Console.ReadLine());
                    string[] _arr;
                    switch (_option)
                    {
                        case 0:
                            Console.Write("�������Ƿ��ӳٶ��ġ�0����1���ǡ���");
                            var _bRst = Convert.ToBoolean(Console.ReadLine());
                            RabbitClient.Consumer(_bRst);
                            break;
                        case 1:
                            //Fast
                            Console.Write("������������ƣ�");
                            _text = Console.ReadLine();
                            RabbitClient.Fanout(false,_text);
                            break;
                        case 2:
                            //fast��Fast
                            Console.Write("������������ơ�·�����ơ�������·���á��ָ������·����,�ָ�����");
                            _text = Console.ReadLine();
                            _arr = _text.Split('��');
                            RabbitClient.Direct(false,_arr[0],_arr[1].Split(','));
                            break;
                        case 3:
                            //TopicEx��fast��fast.*[,fast.#]=>����Ĭ��һ��
                            Console.Write("�����뽻�������ơ��������ơ�·�����ơ��������������С�·�ɣ��á��ָ������·����,�ָ�����");
                            _text = Console.ReadLine();
                            _arr = _text.Split('��');
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
