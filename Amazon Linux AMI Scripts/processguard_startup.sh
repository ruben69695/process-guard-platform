#!/bin/bash
# chkconfig: 345 99 10
# description: auto start processguardapi listener
#
case "$1" in
 'start')
   echo "Starting process guard API service, opening connections..."
   cd /var/aspnetcore/processguard
   dotnet ProcessGuardAPI.dll;;
   #dotnet /var/aspnetcore/processguard/ProcessGuardAPI.dll;;
  'stop')
   echo "Terminating process guard API service, closing connections..."
   killall dotnet;;
esac
