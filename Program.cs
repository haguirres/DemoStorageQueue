using System;
using System.Diagnostics.Contracts;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace DemoStorageQueue{
    class Program{
        static string storageConnectionString = "";
        static string queueName = "sample-queue";
        static QueueClient client = default!;
        static int numMessages = 250;
        static async Task SendMessages(QueueClient client){
            await client.CreateIfNotExistsAsync();
            if(await client.ExistsAsync() == true){
                for (int i = 1; i <= numMessages; i++)
                {
                    await client.SendMessageAsync($"Mensaje no {i} de {numMessages}");
                    System.Console.WriteLine($"Enviando mensaje {i} de {numMessages}");
                }
            }
        }

        static async Task ReceiveMessages(QueueClient client){
            for (int i = 1; i <= numMessages; i++)
            {
                var message = await client.ReceiveMessageAsync();
                System.Console.WriteLine($"Procesando: {message.Value.Body}");
                await client.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
            }
        }

        static async Task Main(){

            client = new QueueClient(storageConnectionString, queueName);
            await SendMessages(client);
            System.Console.WriteLine("Presione una tecla para leer los mensajes");
            System.Console.ReadKey();
            await ReceiveMessages(client);         
        } 

    }
}
