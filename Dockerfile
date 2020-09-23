FROM mcr.microsoft.com/dotnet/core/runtime:2.1

COPY Sample/bin/Release/netcoreapp2.1/publish/ app/

ENTRYPOINT ["dotnet", "app/Sample.dll"]