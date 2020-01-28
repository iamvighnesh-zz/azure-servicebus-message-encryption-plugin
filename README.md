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

    Install-Package ServiceBus.MessageEncryption
<br />

## **Encryption Algorithms**
Here is a list of algorithms supported by the library,

| Algorithm | Supported |
|----------- | ---------- |
| [Rijndael Managed aka Advanced Encryption Standard (AES)](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard) | YES |
<br />

# **Using the library**

## Keys for Encryption and Decryption
In a typical implementation of Rijndael Managed Symmetric algorithm, we need 2 inputs for the algorithm to encrypt and decrypt any payload.

| Key | Description |
| --- | ---------- |
| **Cryptography Key/Passphrase** | - Primary passphrase that defines the key used to encrypt or decrypt the payload <br/> - **Key will always be converted to a 256 bits Base64 string**|
| **Initialization Vector** | - Another key used to randomize the blocks used in the encryption algorithm <br/> - **Key must not exceed 64 bytes of a string**|

## Registering the plugin
The **Message Sender** can be registered with the plugin with the code below,
``` cs
var sender = new MessageSender(connectionString, topicPath, RetryPolicy.Default);
sender.EnableRijndaelManagedEncryption(cryptoKey, initVectorKey);
```

Please note that the `EnableRijndaelManagedEncryption()` can be used on a `TopicClient` too. [Snippet Source](/src/ServiceBus.MessageEncryption.Console/Program.cs#L26-L27) can be found here.

<br />

And the **Message Receiver** can be registered with the plugin with the code below,
``` cs
var receiver = new MessageReceiver(connectionString, subscriptionPath, ReceiveMode.PeekLock, RetryPolicy.Default);
receiver.EnableRijndaelManagedEncryption(cryptoKey, initVectorKey);
```

Please note that the `EnableRijndaelManagedEncryption()` can be used on a `SubscriptionClient` too. [Snippet Source](/src/ServiceBus.MessageEncryption.Console/Program.cs#L30-L31) can be found here.

<br />

## **Build Status**

[![Build Status](https://iamvighnesh.visualstudio.com/GitHub%20Projects/_apis/build/status/ServiceBus.MessageEncryption.Plugin-CI?branchName=master)](https://iamvighnesh.visualstudio.com/GitHub%20Projects/_build/latest?definitionId=6&branchName=master)
