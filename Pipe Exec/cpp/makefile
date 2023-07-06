# 
# IPWorks IPC 2022 C++ Edition - Sample Project
# 
# This sample project demonstrates the usage of IPWorks IPC in a 
# simple, straightforward way. This is not intended to be a complete 
# application. Error handling and other checks are simplified for clarity.
# 
# Copyright (c) 2023 /n software inc. www.nsoftware.com
# 

ifeq ($(shell uname -s), Darwin) # MACOS
R_PATH=-Wl,-rpath,../../lib64/,-rpath,.

FRAMEWORK = -framework Carbon -framework Security
LD_FLAGS = -lipworksipc.22.0 -L../../lib64/ -lresolv
LD_FLAGS_SRC = -lz -lresolv
OS_CFLAGS = -D UNIX -arch arm64
MACOS = "darwin% Darwin% macos%"

all:
	g++ $(OS_CFLAGS) -D UNIX -o pipeexec pipeexec.cpp  -I../../include/ $(LD_FLAGS) $(FRAMEWORK) $(R_PATH)

src: ../../src/ipworksipc.o
	g++ $(OS_CFLAGS) -o pipeexec ../../src/ipworksipc.o pipeexec.cpp  $(LD_FLAGS_SRC) $(FRAMEWORK)

../../src/ipworksipc.o:
	g++ -c -fno-exceptions -fno-rtti -fPIC -arch arm64 -o ../../src/ipworksipc.o ../../src/ipworksipc.cpp

else # LINUX
ifeq ($(shell uname -m), x86_64)
  LIB=lib64
else
  LIB=lib
endif

R_PATH=-Wl,-rpath,../../$(LIB)/,-rpath,.
LD_FLAGS = -lipworksipc -ldl -lpthread -L../../$(LIB)/ -ldl -lpthread
LD_FLAGS_SRC = -lz -ldl -lpthread

all:
	g++ -D UNIX -o pipeexec pipeexec.cpp  -I../../include/ $(LD_FLAGS) $(R_PATH) 

src: ../../src/ipworksipc.o
	g++ -D UNIX -o pipeexec ../../src/ipworksipc.o pipeexec.cpp  -I../../include/ $(LD_FLAGS_SRC)

../../src/ipworksipc.o:
	g++ -c -fno-exceptions -fno-rtti -fPIC -o ../../src/ipworksipc.o ../../src/ipworksipc.cpp

clean:
	rm -f ../../src/*.o
	rm -f pipeexec *.o
endif