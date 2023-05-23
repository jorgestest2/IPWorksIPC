/*
 * IPWorks IPC 2022 C++ Edition - Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

#include <stdio.h>
#include <stdlib.h>
#include <ctype.h>
#include <string.h>
#include "../../include/ipworksipc.h"
#define LINE_LEN 80

bool dataInReceived;

class MyPipeClient : public PipeClient
{
public:
	int FireDisconnected(PipeClientDisconnectedEventParams *e)
	{
		printf("Disconnected");
		return 0;
	}

	int FireConnected(PipeClientConnectedEventParams *e)
	{
		printf("Connected");
		return 0;
	}

	int FireDataIn(PipeClientDataInEventParams *e)
	{
		printf("Received '%s' \r\n", e->Text);
		dataInReceived = true;
		return 0;
	}

};

int main(int argc, char* argv[])
{
	MyPipeClient pipeclient; 

	printf("*****************************************************************\n");
	printf("* This is a demo to show hot to use the PipeClient component to *\n");
	printf("* connect to a PipeServer and receive the echoed response.      *\n");
	printf("*****************************************************************\n");

	printf("Pipe Name (MyPipeServer): ");
	char pipename[LINE_LEN];
	fgets(pipename, LINE_LEN, stdin);
	pipename[strlen(pipename) - 1] = '\0';
	if(strlen(pipename) == 0) strcat(pipename,"MyPipeServer");

	pipeclient.SetEOL("\r\n", 2);

	pipeclient.SetPipeName(pipename);

	int ret_code = pipeclient.Connect();

	if(ret_code)
	{
		printf("Error connecting: %i - %s\n", ret_code, pipeclient.GetLastError());
		goto done;
	}

	char command[LINE_LEN];
	while(true)
	{
		dataInReceived = false;
		printf("\nPlease input command: \r\n- 1 Send Data \r\n- 2 Exit \r\n");
		printf(">");

		fgets(command,LINE_LEN,stdin);
		command[strlen(command)-1] = '\0';

		if (!strcmp(command, "1"))
		{
			char text[LINE_LEN];
			printf("Please enter data to send: ");
			fgets(text,LINE_LEN,stdin);
			text[strlen(text)-1] = '\0';
			ret_code = pipeclient.SetDataToSend(strcat(text,"\r\n"), strlen(text) + 2);

			if (ret_code)
			{
				printf("Sending failed: %i - %s\n", ret_code, pipeclient.GetLastError());
			}
			else
			{
				printf("Waiting for response...\n");
				while(!dataInReceived)
					pipeclient.DoEvents();
			}
		}
		else if (!strcmp(command, "2"))
		{
			goto done;
		}
		else
		{
			printf("Command not recognized.\n");
		}
	}

done:
	if (pipeclient.GetConnected())
	{
		pipeclient.Disconnect();
	}
	printf( "\nExiting... (press enter)\n" );
	getchar();

	return 0;
}






