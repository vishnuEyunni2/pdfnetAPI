FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
# RUN pwd
COPY ["OCRWebAPP1.csproj", "OCRWebAPP1/"]
RUN dotnet restore "OCRWebAPP1/OCRWebAPP1.csproj"
WORKDIR "/src/OCRWebAPP1"
COPY . .
# RUN ls -lrt
RUN dotnet build "OCRWebAPP1.csproj" -c Release -o /app
# RUN cd ./bin ; ls -lrt

COPY Lib/ /app/Lib

RUN dir

# RUN ls -lrt /app

# RUN ls -lrt bin/Release/
# 
# RUN pwd ; echo "Done1"
# 
# RUN ls -lrt bin/Debug/netcoreapp2.1/
# 
# RUN pwd ; echo "Done2"
# 
# RUN ls -lrt bin/Release/netcoreapp2.1/

FROM build AS publish
RUN dotnet publish "OCRWebAPP1.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
# RUN ls -lrt
RUN dir

ENTRYPOINT ["dotnet", "OCRWebAPP1.dll"]