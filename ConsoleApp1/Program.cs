namespace ConsoleApp1;

using System;
using VSGitBlame.Core;

public class Program
{
    public static void Main(string[] args)
    {
        string output = """
                1b94c76f1a667666bf9510eb2aa21f3b8bdbcc27 1 1 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1711598984
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1711598984
                committer-tz +0100
                summary log whatsapp history chat
                previous 42a8b6f1fc74e81c25d0838a6dc6d74979252072 IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	ï»¿using Amazon.DynamoDBv2.DataModel;
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 2 2 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1710939205
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1710939205
                committer-tz +0100
                summary refine work queue logs
                previous 2541bcc0ff48fa4ce2532b35d8e9ca5a818869ab IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	using IziFinEngagementWalletAPI.Core.DTOs.Services;
                28e20ff7f35d11dc274f825127c5386f9cf447a2 3 3 2
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1716308467
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1716308501
                committer-tz +0100
                summary more prod updates
                previous 91948782d943bc4ba633f3d72bb9ba0cc5e0c60f IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	using IziFinEngagementWalletAPI.Core.Models;
                28e20ff7f35d11dc274f825127c5386f9cf447a2 4 4
                	using IziFinEngagementWalletAPI.Data;
                780826503955b6c96d72a9516e328bd0f5746937 2 5 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1699430316
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1699430316
                committer-tz +0100
                summary optimise mimic whatsapp process
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	using IziFinEngagementWalletAPI.Domain.Services;
                35e6951109c2de580cc1c08170397d501f96eeaa 3 6 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1701165903
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1701165903
                committer-tz +0100
                summary fix mimic
                previous 9f6fbad5bff658e06ab0c4aaf46e1afb4804f5d7 IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	using Microsoft.Extensions.DependencyInjection;
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 5 7 1
                	using Microsoft.Extensions.Logging;
                780826503955b6c96d72a9516e328bd0f5746937 3 8 5

                780826503955b6c96d72a9516e328bd0f5746937 4 9
                	namespace IziFinEngagementWalletAPI.Domain.WorkQueue;
                780826503955b6c96d72a9516e328bd0f5746937 5 10

                780826503955b6c96d72a9516e328bd0f5746937 6 11
                	public class MimicWorkQueue : WorkQueue<TwilioMessagingRequest>
                780826503955b6c96d72a9516e328bd0f5746937 7 12
                	{
                35e6951109c2de580cc1c08170397d501f96eeaa 9 13 1
                	    private readonly IServiceProvider _serviceProvider;
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 12 14 1
                	    private readonly ILogger<MimicWorkQueue> _logger;
                780826503955b6c96d72a9516e328bd0f5746937 10 15 1

                1b94c76f1a667666bf9510eb2aa21f3b8bdbcc27 14 16 1
                	    public MimicWorkQueue(
                20ad9700ee5fc795d403c2a59848cada11212c9c 17 17 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1716309208
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1716309208
                committer-tz +0100
                summary final fix
                previous 28e20ff7f35d11dc274f825127c5386f9cf447a2 IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	        IServiceProvider serviceProvider, ILogger<MimicWorkQueue> logger)
                780826503955b6c96d72a9516e328bd0f5746937 12 18 1
                	    {
                35e6951109c2de580cc1c08170397d501f96eeaa 13 19 1
                	        _serviceProvider = serviceProvider;
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 17 20 1
                	        _logger = logger;
                780826503955b6c96d72a9516e328bd0f5746937 15 21 4
                	    }
                780826503955b6c96d72a9516e328bd0f5746937 16 22

                780826503955b6c96d72a9516e328bd0f5746937 17 23
                	    public override async Task ProcessItem(TwilioMessagingRequest item)
                780826503955b6c96d72a9516e328bd0f5746937 18 24
                	    {
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 22 25 2
                	        _logger.LogInformation("Mimic Work Queue processing started");
                0818d41b515d5e5a1a79c5705b32fc05fa7e243c 23 26

                35e6951109c2de580cc1c08170397d501f96eeaa 18 27 3
                	        using var scope = _serviceProvider.CreateScope();
                35e6951109c2de580cc1c08170397d501f96eeaa 19 28
                	        var mimicService = scope.ServiceProvider.GetRequiredService<MimicService>();
                35e6951109c2de580cc1c08170397d501f96eeaa 20 29
                	        var twilioService = scope.ServiceProvider.GetRequiredService<TwilioService>();
                20ad9700ee5fc795d403c2a59848cada11212c9c 34 30 1
                	        var dbContext  = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                1b94c76f1a667666bf9510eb2aa21f3b8bdbcc27 28 31 1
                	        var dynamoDBContext = scope.ServiceProvider.GetRequiredService<IDynamoDBContext>();
                35e6951109c2de580cc1c08170397d501f96eeaa 21 32 1

                780826503955b6c96d72a9516e328bd0f5746937 19 33 1
                	        string phoneNumber = item.From.Replace("whatsapp:", "");
                780826503955b6c96d72a9516e328bd0f5746937 21 34 1

                13a29612f3111ec5265c96d09d12270f60d6881a 39 35 1
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1726011481
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1726011481
                committer-tz +0100
                summary more updates
                previous 98ba0a61a2fa796549d18edc615f8f174c3caf36 IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	        var response = await mimicService.SendUserMessage(phoneNumber, item.Body);
                b9d0b296d6eec324dc5526f1ea6a03adc4f0309e 36 36 3
                author Emmanuel Adebiyi
                author-mail <smartemma03@gmail.com>
                author-time 1726051842
                author-tz +0100
                committer Emmanuel Adebiyi
                committer-mail <smartemma03@gmail.com>
                committer-time 1726051842
                committer-tz +0100
                summary mimic finalisations
                previous 13a29612f3111ec5265c96d09d12270f60d6881a IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                filename IziFinEngagementWalletAPI.Domain/WorkQueue/MimicWorkQueue.cs
                	        await mimicService.SendBotResponseAsync(phoneNumber, response);
                b9d0b296d6eec324dc5526f1ea6a03adc4f0309e 37 37

                b9d0b296d6eec324dc5526f1ea6a03adc4f0309e 38 38
                	        _logger.LogInformation("Mimic Work Queue processing complete");       
                780826503955b6c96d72a9516e328bd0f5746937 23 39 2
                	    }
                780826503955b6c96d72a9516e328bd0f5746937 24 40
                	}
                """;

        var fileBlameInfo = new FileBlameInfo(output);

        int i = 1;
        var commitInfo = fileBlameInfo.GetAt(i);

        while(commitInfo != null)
        {
            Console.WriteLine($"Line {i}: {commitInfo.AuthorName}");
            commitInfo = fileBlameInfo.GetAt(++i);
        }
    }
}