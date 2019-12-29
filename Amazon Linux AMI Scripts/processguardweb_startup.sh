#!/bin/bash
# chkconfig: 345 99 10
# description: auto start processguardweb listener
#
case "$1" in
 'start')
   echo "Starting process guard Web..."
   cd /var/aspnetcore/processguardWeb
   dotnet Process\ Guard\ Web.dll;;
  'stop')
   echo "Terminating process guard Web..."
   killall dotnet;;
esac