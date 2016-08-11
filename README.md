## C# client for MainSMS.ru API
[![Build status](https://ci.appveyor.com/api/projects/status/laa9w0noj5yri76f/branch/master?svg=true)](https://ci.appveyor.com/project/kroniak/extensions-mainsms/branch/master)
[![NuGet](https://img.shields.io/nuget/v/MainSMS.svg)](https://www.nuget.org/packages/MainSMS/)
> This repository contains models and service C# class for MainSMS XML API.

[MainSMS.ru](http://MainSMS.ru) is a service to sending text message to the recipients from Russia and nearest countries.

### Install
> The package is compiled for NET45 and .NET Platform Standard 1.6 which include .NET Core and other targets. [Read about it](https://github.com/dotnet/corefx/blob/master/Documentation/architecture/net-platform-standard.md#mapping-the-net-platform-standard-to-platforms).

`PM> Install-Package MainSMS`

### Usage

```c#
// Create client
var client = new MainSmsClient("you project name", "your api key");

// Getting your project balance
var balanse = await client.GetBalanceAsync();

Console.WriteLine(balanse.Status == "success" ? $"balance is {balanse.Balance}" 
	: $"Error is {balanse.ErrorMessage}");

// Getting send price information and right format phone numbers
var price = await client.GetPriceAsync("79214445566", "test messages");

Console.WriteLine(price.Status == "success" ? 
	$"Price is {price.Price}. Total messages count is {price.MessageCount}" 
	: $"Error is {price.ErrorMessage}");

// Sending messages
var sendResult = await client.SendAsync("79214445566,+89214445566", "test message");

Console.WriteLine(sendResult.Status == "success" ? 
	$"Price is {sendResult.Price}. Total messages count is {sendResult.MessageCount}" 
	: $"Error is {sendResult.ErrorMessage}");

if (sendResult.MessageIds.Count > 0) //print receaved message ids.
	sendResult.MessageIds.ToList().ForEach(id => Console.WriteLine($"Message id is {id}")); 

// If you want only test without real sending the messages 
// pass testMode param, MessageIds will be empty
var testSendResult = await client.SendAsync("79214445566,+89214445566", 
	"test messages", testMode: true);

// Getting delivery statuses of the messages
var statuses = await client.GetStatusesAsync(sendResult.MessageIds);

if (statuses.Status=="success") // print message statuses
	if (statuses.Statuses.Any())
		foreach (var status in statuses.Statuses)
			Console.WriteLine(
				$"The status of the message {status.Key} is {status.Value}");
else Console.WriteLine($"Error is {statuses.ErrorMessage}");

// Send a delayed message
var delayedSendResult = await client.SendAsync("79214445566,+89214445566", 
			"test messages", startDateTime: new DateTime(2016,08,12));

// Try to cancel these messages
if (delayedSendResult.Status == "success" && delayedSendResult.MessageIds.Any())
{
	var cancelResult = await client.CancelAsync(delayedSendResult.MessageIds);
	if (cancelResult.Status == "success") // print messages statuses
		if (cancelResult.Statuses.Any())
			foreach (var status in cancelResult.Statuses)
				Console.WriteLine($"The status of the message {status.Key} is {status.Value}");
}
```
