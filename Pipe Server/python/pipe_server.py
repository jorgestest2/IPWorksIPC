#
# IPWorks IPC 2022 Python Edition - Demo Application
#
# Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
#

import sys
import string
from ipworksipc import *

input = sys.hexversion<0x03000000 and raw_input or input


def fireConnected(e):
  print("PipeClient %s has connected.\r\n "%str(pipeserver.get_connection_id(0)))

def fireDataIn(e):
  print("Echoing '%s' back to client.\r\n"%e.text.decode("utf-8"))
  pipeserver.set_data_to_send(int(pipeserver.get_connection_id(0)),e.text)

def fireDisconnected(e):
  print("PipeClient %s has disconnected.\r\n "%str(pipeserver.get_connection_id(0)))

def fireError(e):
  print(e.description)

def fireReadyToSend(e):
  print("Client %s is ready to send"%str(pipeserver.get_connection_id(0)))

pipeserver = PipeServer()
pipeserver.on_connected = fireConnected
pipeserver.on_data_in = fireDataIn
pipeserver.on_disconnected = fireDisconnected
pipeserver.on_error = fireError
pipeserver.on_ready_to_send = fireReadyToSend

print("*****************************************************************\n")
print("* This demo shows how to use the PipeServer component to accept *\n")
print("* connections from a PipeClient.                                *\n")
print("*****************************************************************\n")
try:
  servername = input("Pipe Name: ")
  if servername == "":
    servername = "MyPipeServer"
  pipeserver.set_pipe_name(servername)

  pipeserver.listening = True
  print(servername + " Listening")

  while pipeserver.listening:
    pipeserver.do_events()
except IPWorksIPCError as e:
  print("ERROR %s" %e.message)

