﻿namespace Deepin.Infrastructure.EventBus;
public class RabbitMqOptions
{
    public string HostName { get; set; }
    public string VirtualHost { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string QueueName { get; set; }
}
