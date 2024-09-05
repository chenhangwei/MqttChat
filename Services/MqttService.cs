using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Diagnostics;
using MQTTnet.Protocol;
using System.Text.Json;
using Chat.Models;
using System.Collections.ObjectModel;
using Chat.ViewModels;
namespace Chat.Services
{
    public class MqttService
    {
        public ObservableCollection<ChatMsg> ChatMsgs;
        User User;
        ChatMsg ChatMsg;
        private string? broker; // MQTT代理服务器地址
        private int port; // MQTT代理服务器端口
        private List<X509Certificate2> certlist; // 证书列表，包含CA证书和客户端证书
        private X509Certificate2 caCert; // CA证书
        private X509Certificate2 clientCert;    // 客户端证书
        public IMqttClient _client; // MQTT客户端实例
        private MqttClientOptions _options; // MQTT连接选项
        private async Task Init(User user)
        {
            try
            {
                // 创建随机数生成器
                Random random = new Random();
                // 从安全存储中获取代理服务器地址
                broker = await SecureStorage.GetAsync("Host");
                // 从安全存储中获取端口号并转换为整型
                int.TryParse(await SecureStorage.GetAsync("Port"), out port);
                // 生成随机客户端ID
                var clientid = $"chat-{random.Next(0, 99)}";
                var certAsByte = await ReadCertificateAsByteArray();
                caCert = new X509Certificate2(certAsByte);
                clientCert = new X509Certificate2(certAsByte);
                certlist = new List<X509Certificate2>();
                // 将证书添加到证书列表
                certlist.Add(caCert);
                certlist.Add(clientCert);
                // 创建MQTT客户端实例
                _client = new MqttFactory().CreateMqttClient();
                // 构建MQTT客户端连接选项
                _options = new MqttClientOptionsBuilder()
                .WithTcpServer(broker, port) // 设置TCP服务器地址和端口
                    .WithClientId(clientid) // 设置客户端ID
                    .WithCleanSession(true) // 设置为清洁会话
                    .WithCredentials(user.Phone, user.Password) // 设置认证凭据
                    .WithTlsOptions(
                        o => o
                        .WithClientCertificates(certlist)
                        .WithCertificateValidationHandler(_ => true)) // 配置TLS选项
                    .Build(); // 构建最终的连接选项
            }
            catch (Exception)
            {
                throw;
            }
        }    // 初始化方法，设置MQTT客户端配置并初始化客户端
        public async Task ConnetMqttAsync(User user,string topic, CancellationToken shutdownToken)
        {
            try
            {
                    User = user;
                    await Init(User);
                    // 尝试连接到MQTT服务器
                    MqttClientConnectResult result = await _client.ConnectAsync(_options);
                    // 根据连接结果返回布尔值
                    if (result.ResultCode == MqttClientConnectResultCode.Success)
                    {
                        Subscribe(topic, shutdownToken);
                    }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }
        public void Subscribe(string topic, CancellationToken shutdownToken)
        {
            try
            {
                // 创建MQTT工厂实例
                var mqttFactory = new MqttFactory();
                // 构建订阅选项
                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter($"{topic}", MqttQualityOfServiceLevel.AtLeastOnce) // 设置主题过滤器
                    .Build();
                // 发起订阅请求
                Task<MqttClientSubscribeResult> response = _client.SubscribeAsync(mqttSubscribeOptions, shutdownToken);
                // 启动并发处理并禁用自动确认
                ConcurrentProcessingDisableAutoAcknowledge(shutdownToken, _client);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task PublishAsync(string topic, ChatMsg chatmsg, string sendMsg)
        {
            try
            {
                if (_client.IsConnected)
                {
                    ChatMsg.Users = User;
                    ChatMsg.ReceiveDateTime = DateTime.Now;
                    ChatMsg.ReceiveMsg = sendMsg;
                    string payload = JsonSerializer.Serialize(ChatMsg);
                    if (payload is not null)
                    {
                        //构建消息
                        var message = new MqttApplicationMessageBuilder()
                                .WithTopic(topic) // 设置主题
                            .WithPayload(payload) // 设置负载
                                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce) // 设置QoS级别
                                .Build();
                        // 发布消息
                        await _client.PublishAsync(message);
                        //ChatMsgs.Add(ChatMsgx);
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出错误信息
                Console.WriteLine($"Error publishing: {ex.Message}");
            }
        }
        private void ConcurrentProcessingDisableAutoAcknowledge(CancellationToken shutdownToken, IMqttClient mqttClient)
        {
            /*
             * 这个示例展示了如何实现并发处理并且不让消息自动确认，
             * 以获得MQTT规范所能提供的至少一次（at-least-once）体验。
             */
            // 当接收到应用程序消息时触发的事件处理器
            mqttClient.ApplicationMessageReceivedAsync += ea =>
            {
                ea.AutoAcknowledge = false; // 禁用自动确认功能。
                // 异步任务用于处理传入的消息
                async Task ProcessAsync()
                {
                    // 在这里执行您的工作！例如解析消息的有效载荷、执行一些业务逻辑等
                    await Task.Delay(1000, shutdownToken); // 模拟处理延迟，并支持取消令牌。
                    string josn = ea.ApplicationMessage.ConvertPayloadToString();
                    //ChatMsg chatMsg = JsonSerializer.DeserializeAsync<ChatMsg>(josn);
                    var options1 = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true, // 使属性名称大小写不敏感
                                                            // 
                                                            // Converters = { new IsoDateTimeConverter() }
                    };
                    ChatMsg msg = new ChatMsg();
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                        {
                            await writer.WriteAsync(josn);
                            await writer.FlushAsync();
                            stream.Position = 0;
                            try
                            {
                                msg = await JsonSerializer.DeserializeAsync<ChatMsg>(stream, options1);
                                if (MainThread.IsMainThread)
                                {
                                    ChatMsgs.Add(msg);
                                }
                                else
                                {
                                    MainThread.BeginInvokeOnMainThread(() => { 
                                        ChatMsgs.Add(msg);
                                    });
                                }
                            }
                            catch (OperationCanceledException e)
                            {
                                Debug.WriteLine(e.ToString());
                            }
                        }
                    }
                    // 在成功处理消息后确认消息
                    await ea.AcknowledgeAsync(shutdownToken);
                    // 警告：如果处理失败并非瞬时故障，那么在每次客户端重启时都会重试该消息
                    //       因为MQTT没有NACK包来通知代理处理失败
                    //
                    // 可选方案：使用如Polly这样的框架创建重试策略：https://github.com/App-vNext/Polly#retry
                }
                // 启动一个单独的任务来处理消息。
                // 这允许消息并发地进行处理。
                _ = Task.Run(ProcessAsync, shutdownToken);
                // 返回一个已完成的任务，表示事件处理器已经开始处理。
                return Task.CompletedTask;
            };
        }
        public async Task<byte[]> ReadCertificateAsByteArray()
        {
            try
            {
                // Get the stream of the certificate from the Assets folder
                using (var stream = await FileSystem.OpenAppPackageFileAsync("Resources/emqxsl-ca.crt"))
                {
                    if (stream != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            // Copy the stream to a MemoryStream
                            stream.CopyTo(memoryStream);
                            // Convert the MemoryStream to byte[]
                            return memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Certificate file not found in the Assets folder.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading certificate: {ex.Message}");
            }
            return null;
        }
        public MqttService()
        {
            ChatMsgs = new ObservableCollection<ChatMsg>();
            User = new User();
            ChatMsg = new ChatMsg();
        }
    }
}
