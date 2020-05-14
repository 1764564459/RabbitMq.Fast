①、安装erlang:https://www.erlang.org/downloads => * Windows 64-bit Binary File
②、安装 rabbitmq-server：https://www.rabbitmq.com/install-windows.html
③、Nuget:RabbitMQ.Client
③、RabblitMq添加、查看用户：cd F:\RabbitMQ\rabbitmq_server-3.8.3\sbin 
    rabbitmqctl list_users 查看用户【guest=>默认密码：guest】

④、查看RabbitMq配置：rabbitmqctl status 【默认port：5672】