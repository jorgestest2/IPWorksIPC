/*
 * IPWorks IPC 2022 C++ Edition - Sample Project
 *
 * This sample project demonstrates the usage of IPWorks IPC in a 
 * simple, straightforward way. This is not intended to be a complete 
 * application. Error handling and other checks are simplified for clarity.
 *
 * Copyright (c) 2023 /n software inc. www.nsoftware.com
 */

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>
#include "../../include/ipworksipc.h"
#define LINE_LEN 80

bool promptReceived;

class MyPipeExec : public PipeExec
{
public:

	int FireStdout(PipeExecStdoutEventParams *e)
	{
		printf(e->Text);
		if (e->Text[e->lenText - 1] == '>') //Assume if the last char is '>' that it's a shell prompt
			promptReceived = true;
		return 0;
	}

	int FireStderr(PipeExecStderrEventParams *e)
	{
		printf(e->Text);
		return 0;
	}


};

int main(int argc, char* argv[])
{
	MyPipeExec pipeExec;
	char		buffer[LINE_LEN + 1];

	printf("*****************************************************************\n");
	printf("* This demo shows how to use the PipeExec component to launch a *\n");
	printf("* process then send and receive data to and from the process.   *\n");
	printf("*****************************************************************\n");

#ifdef __linux
	printf("Process [ls]: ");
	fgets(buffer, LINE_LEN, stdin);
	buffer[strlen(buffer) - 1] = '\0';
	if (strlen(buffer) == 0) strcat(buffer, "ls");
#else
	printf("Process Path [C:\\Windows\\System32\\cmd.exe]: ");
	fgets(buffer, LINE_LEN, stdin);
	buffer[strlen(buffer) - 1] = '\0';
	if (strlen(buffer) == 0) strcat(buffer, "C:\\Windows\\System32\\cmd.exe");
#endif

	pipeExec.SetProcessFileName(buffer);

#ifdef __linux
	printf("Process Args [-la]: ");
	fgets(buffer, LINE_LEN, stdin);
	buffer[strlen(buffer) - 1] = '\0';
	if (strlen(buffer) == 0) strcat(buffer, "-la");
#else
	printf("Process Args [/Q]: ");
	fgets(buffer, LINE_LEN, stdin);
	buffer[strlen(buffer) - 1] = '\0';
	if (strlen(buffer) == 0) strcat(buffer, "/Q");
#endif
	pipeExec.SetProcessArgs(buffer);

	printf("Press enter to start process. Enter 'exit' or 'Ctl+C' to exit.\n");
	getchar();

	int ret_code = pipeExec.StartProcess();

	if (ret_code)
	{
		printf("Error starting process: %i - %s\n", ret_code, pipeExec.GetLastError());
		goto done;
	}

	while (true)
	{
		if (promptReceived)
		{
			promptReceived = false;
			fgets(buffer, LINE_LEN, stdin);
			buffer[strlen(buffer) - 1] = '\0';
			if (strcmp(buffer, "exit") != 0)
			{
				strcat(buffer, "\r\n");
				pipeExec.SetStdin(buffer, strlen(buffer));
			}
			else
			{
				goto done;
			}
		}
		pipeExec.DoEvents();
	}

done:
	pipeExec.StopProcess();
	printf("Exiting... (press enter)\n");
	getchar();

	return 0;
}




