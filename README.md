# Azure ServicesBus Message Encryption

## Table of Content
  * [What is this library?](#what-is-this-library)
  * [Download the library](#download-the-library)
  * [Encryption Algorithms](#encryption-algorithms)
  * [Using the library](#using-the-library)
  * [Build Status](#build-status)



<br />

## **What is this library?**
This library is all about a plugin to [Azure ServiceBus Messaging Library](https://github.com/Azure/azure-service-bus-dotnet/) that enables you to apply message payload encryption. This will help protect a sensitive message payload by encrypting and decrypting the service bus message. 

This library provides an Azure ServiceBus Plugin that allows you to,

* **Encrypt the message payload** right after the `Send` is invoked on the message publisher and just before sending the message on topic.
* **Decrypt the message payload** right after the message is received by the subscriber and before the message is handed over to the subscriber function.

## **Download the library**
The package can be download from [this link](https://www.nuget.org/packages/ServiceBus.MessageEncryption/).

And here is the command for the package manager to install the package:

    Install-Package ServiceBus.MessageEncryption -Version 1.0.0
<br />

## **Encryption Algorithms**
Here is a list of algorithms supported by the library,

| Algorithm | Supported |
|----------- | ---------- |
| [Triple Data Encryption Standard (TDES)](https://en.wikipedia.org/wiki/Triple_DES)| YES |
| [Advanced Encryption Standard (AES)](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard) | YES |
<br />

## **Using the library**

### Registering the plugin
The **Message Sender** can be registered with the plugin with the code below,
``` cs
var sender = new MessageSender(connectionString, topicPath, RetryPolicy.Default);
sender.RegisterMessageEncryptionPlugin(encryptionKey);
```

Please note that the `RegisterMessageEncryptionPlugin()` can be used on a `TopicClient` too. [Snippet Source](/src/ServiceBus.MessageEncryption.Console/Program.cs#L24-L25) can be found here.

<br />

And the **Message Receiver** can be registered with the plugin with the code below,
``` cs
var receiver = new MessageReceiver(connectionString, subscriptionPath ReceiveMode.PeekLock, RetryPolicy.Default);
receiver.RegisterMessageEncryptionPlugin(encryptionKey);
```

Please note that the `RegisterMessageEncryptionPlugin()` can be used on a `SubscriptionClient` too. [Snippet Source](/src/ServiceBus.MessageEncryption.Console/Program.cs#L33-L34) can be found here.

<br />

## **Build Status**

[![Build Status](https://iamvighnesh.visualstudio.com/GitHub%20Projects/_apis/build/status/ServiceBus.MessageEncryption.Plugin-CI?branchName=master)](https://iamvighnesh.visualstudio.com/GitHub%20Projects/_build/latest?definitionId=6&branchName=master)
