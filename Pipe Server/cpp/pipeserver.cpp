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

class MyPipeServer : public PipeServer
{
	int FireConnected(PipeServerConnectedEventParams *e)
	{
		printf("PipeClient [%i] has connected.\r\n", e->ConnectionId);
		return 0;
	}

	int FireDisconnected(PipeServerDisconnectedEventParams *e)
	{
		printf("PipeClient [%i] disconnected.\r\n", e->ConnectionId);
		return 0;
	}

	int FireDataIn(PipeServerDataInEventParams *e)
	{
		printf("Echoing '%s' back to client [%i].\r\n", e->Text, e->ConnectionId);
    this->SetDataToSend(e->ConnectionId,e->Text,strlen(e->Text));
    this->SetDataToSend(e->ConnectionId,"\r\n",2);

		return 0;
	}
};

int main(int argc, char* argv[])
{
	MyPipeServer pipeserver;

	char buffer[LINE_LEN];

	printf("*****************************************************************\n");
	printf("* This demo shows how to use the PipeServer component to accept *\n");
	printf("* connections from a PipeClient.                                *\n");
	printf("*****************************************************************\n");

	printf("Pipe Name (MyPipeServer): ");
	fgets(buffer, LINE_LEN, stdin);
	buffer[strlen(buffer)-1] = '\0';
	if(strlen(buffer) == 0) strcat(buffer,"MyPipeServer");
	pipeserver.SetPipeName(buffer);
	pipeserver.SetDefaultEOL("\r\n", 2);


	int ret_code = pipeserver.SetListening(true);

	if(ret_code)
	{
		printf("Error: %i - %s\r\n", ret_code, pipeserver.GetLastError());
		goto done;
	}

	printf("Listening...\r\n");

	while(true)
	{
		pipeserver.DoEvents();
	}

done:
	if (pipeserver.GetListening())
	{
		pipeserver.SetListening(false);
	}

	printf( "Exiting... (press enter)\n" );
	getchar();

	return 0;
}






