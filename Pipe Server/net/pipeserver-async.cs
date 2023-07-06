/*
 * IPWorks IPC 2022 .NET Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks IPC in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */

using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using nsoftware.async.IPWorksIPC;

class PipeServerSampleProject
{
  // Define a private static variable for the PipeServer
  private static Pipeserver pipeServer;

  // The main method is async to support non-blocking operations
  static async Task Main()
  {
    // Initialize the pipeServer
    pipeServer = new Pipeserver();

    // Subscribe to events using custom methods defined below
    pipeServer.OnConnected += FireConnected;
    pipeServer.OnDataIn += FireDataIn;
    pipeServer.OnDisconnected += FireDisconnected;
    pipeServer.OnError += FireError;
    pipeServer.OnReadyToSend += FireReadyToSend;

    // Print basic information about the demo
    Console.WriteLine("*****************************************************************");
    Console.WriteLine("* This demo shows how to use the PipeServer component to accept *");
    Console.WriteLine("* connections from a PipeClient.                                *");
    Console.WriteLine("*****************************************************************");

    try
    {
      // Prompt for the pipe name with a default value
      Console.Write("Pipe Name [MyPipeServer]: ");
      var serverName = Console.ReadLine();
      pipeServer.PipeName = string.IsNullOrEmpty(serverName) ? "MyPipeServer" : serverName;

      // Begin listening for connections
      await pipeServer.StartListening();

      Console.WriteLine($"{pipeServer.PipeName} Listening");

      // Continuously process events until the server stops listening
      while (pipeServer.Listening)
      {
        await pipeServer.DoEvents();
      }
    }
    // Catch and handle any exceptions thrown by the IPWorksIPC library
    catch (IPWorksIPCException ex)
    {
      Console.WriteLine($"ERROR {ex.Message}");
    }
  }

  // Event handler for when a client connects
  private static void FireConnected(object s, PipeserverConnectedEventArgs e)
  {
    Console.WriteLine($"PipeClient {e.ConnectionId} has connected.\r\n");
  }

  // Event handler for when data is received
  private static void FireDataIn(object s, PipeserverDataInEventArgs e)
  {
    var data = e.Text;
    Console.WriteLine($"Echoing '{data}' back to client.\r\n");

    // Echo the received data back to the client
    pipeServer.SendLine(e.ConnectionId, e.Text);
  }

  // Event handler for when a client disconnects
  private static void FireDisconnected(object s, PipeserverDisconnectedEventArgs e)
  {
    Console.WriteLine($"PipeClient {e.ConnectionId} has disconnected.\r\n");
  }

  // Event handler for when an error occurs
  private static void FireError(object s, PipeserverErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
  }

  // Event handler for when a client is ready to send data
  private static void FireReadyToSend(object s, PipeserverReadyToSendEventArgs e)
  {
    Console.WriteLine($"Client {e.ConnectionId} is ready to send");
  }
}


class ConsoleDemo
{
  public static Dictionary<string, string> ParseArgs(string[] args)
  {
    Dictionary<string, string> dict = new Dictionary<string, string>();

    for (int i = 0; i < args.Length; i++)
    {
      // If it starts with a "/" check the next argument.
      // If the next argument does NOT start with a "/" then this is paired, and the next argument is the value.
      // Otherwise, the next argument starts with a "/" and the current argument is a switch.

      // If it doesn't start with a "/" then it's not paired and we assume it's a standalone argument.

      if (args[i].StartsWith("/"))
      {
        // Either a paired argument or a switch.
        if (i + 1 < args.Length && !args[i + 1].StartsWith("/"))
        {
          // Paired argument.
          dict.Add(args[i].TrimStart('/'), args[i + 1]);
          // Skip the value in the next iteration.
          i++;
        }
        else
        {
          // Switch, no value.
          dict.Add(args[i].TrimStart('/'), "");
        }
      }
      else
      {
        // Standalone argument. The argument is the value, use the index as a key.
        dict.Add(i.ToString(), args[i]);
      }
    }
    return dict;
  }
}