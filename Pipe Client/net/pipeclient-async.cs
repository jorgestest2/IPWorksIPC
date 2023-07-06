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
using nsoftware.async.IPWorksIPC;
using System.Threading.Tasks;

class Program
{
  private static Pipeclient pipeclient = new Pipeclient();
  private static bool dataInReceived = false;

  static async Task Main(string[] args)
  {
    Console.WriteLine("*****************************************************************\n");
    Console.WriteLine("* This is a demo to show how to use the PipeClient component to *\n");
    Console.WriteLine("* connect to a PipeServer and receive the echoed response.      *\n");
    Console.WriteLine("*****************************************************************\n");

    pipeclient.OnConnected += FireConnected;
    pipeclient.OnDataIn += FireDataIn;
    pipeclient.OnDisconnected += FireDisconnected;
    pipeclient.OnError += FireError;

    try
    {
      Console.Write("Pipe Name: ");
      string servername = Console.ReadLine();
      if (string.IsNullOrWhiteSpace(servername))
        servername = "MyPipeServer";
      pipeclient.PipeName = servername;

      await pipeclient.Connect();
      if (!pipeclient.Connected)
      {
        Console.WriteLine("Error connecting");
        Environment.Exit(0);
      }
      while (true)
      {
        dataInReceived = false;
        Console.WriteLine("\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit \r\n");
        string cmd = Console.ReadLine();
        if (cmd == "1")
        {
          Console.Write("Please enter data to send: ");
          string data = Console.ReadLine();
          await pipeclient.SendText(data);
          Console.WriteLine("Waiting for response...\n");
          while (!dataInReceived)
          {
            await pipeclient.DoEvents();
          }
        }
        else if (cmd == "2")
        {
          break;
        }
        else
        {
          Console.WriteLine("Command not recognized\n");
        }
      }
    }
    catch (IPWorksIPCPipeclientException e)
    {
      Console.WriteLine($"ERROR {e.Message}");
    }
  }

  private static void FireConnected(object sender, EventArgs e)
  {
    Console.WriteLine("Connected");
  }

  private static void FireDataIn(object sender, PipeclientDataInEventArgs e)
  {
    Console.WriteLine($"Received {e.Text} from Pipe Server.\r\n");
    dataInReceived = true;
  }

  private static void FireDisconnected(object sender, EventArgs e)
  {
    Console.WriteLine("Disconnected");
  }

  private static void FireError(object sender, PipeclientErrorEventArgs e)
  {
    Console.WriteLine(e.Description);
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