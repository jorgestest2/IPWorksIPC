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
using System.Threading.Tasks;
using System;
using nsoftware.async.IPWorksIPC;


class Program
{
  // The main method of the program. It is marked as async because it will use asynchronous operations.
  static async Task Main(string[] args)
  {
    // Creating an instance of the PipeExec component from the IPWorks IPC library
    Pipeexec pipeexec = new Pipeexec();

    // Print out some introduction about this program to console
    Console.WriteLine("*****************************************************************");
    Console.WriteLine("* This demo shows how to use the Process class to launch a *");
    Console.WriteLine("* process then send and receive data to and from the process. *");
    Console.WriteLine("*****************************************************************");

    // Define strings for process path and arguments
    string processPath;
    string processArgs;

    // Check the platform the program is running on
    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
      // Prompt the user to enter the process name
      Console.Write("Process [ls]: ");
      processPath = Console.ReadLine();
      // If no process name is entered, use 'ls' as default
      if (string.IsNullOrEmpty(processPath)) pipeexec.ProcessFileName = "ls";

      // Prompt the user to enter the process arguments
      Console.Write("Process Args [-la]: ");
      processArgs = Console.ReadLine();
      // If no process arguments are entered, use '-la' as default
      if (string.IsNullOrEmpty(processArgs)) pipeexec.ProcessArgs = "-la";
    }
    else
    {
      // For non-Unix platforms, use similar approach but with different defaults
      Console.Write("Process Path [C:\\Windows\\System32\\cmd.exe]: ");
      processPath = Console.ReadLine();
      if (string.IsNullOrEmpty(processPath)) pipeexec.ProcessFileName = "C:\\Windows\\System32\\cmd.exe";

      Console.Write("Process Args [/Q]: ");
      processArgs = Console.ReadLine();
      if (string.IsNullOrEmpty(processArgs)) pipeexec.ProcessArgs = "/Q";
    }

    // Prompt the user to start the process
    Console.Write("Press enter to start process. Enter 'exit' to exit.");
    Console.ReadLine();

    // Attach an event handler to the OnStdout event. This handler will print the standard output to the console.
    pipeexec.OnStdout += (sender, e) =>
    {
      Console.Write(e.Text);

    };

    // Attach an event handler to the OnStderr event. This handler will print the standard error to the console.
    pipeexec.OnStderr += (sender, e) =>
    {
      Console.WriteLine(e.Text);
    };

    // Start the process asynchronously
    await pipeexec.StartProcess();

    // Infinite loop that keeps the program running until the user enters 'exit'
    while (true)
    {
      string input = Console.ReadLine();
      if (input != "exit")
      {
        // Send user input to the process
        await pipeexec.SendLine(input);
      }
      else
      {
        // If the user enters 'exit', break the loop
        break;
      }

    }

    // Stop the process asynchronously
    await pipeexec.StopProcess();

    // Notify the user that the program is exiting and wait for a key press
    Console.WriteLine("Exiting... (press enter)");
    Console.ReadLine();
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