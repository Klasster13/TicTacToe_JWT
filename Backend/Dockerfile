FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY TicTacToe/TicTacToe.csproj .
RUN dotnet restore TicTacToe.csproj
COPY TicTacToe/ .
RUN dotnet publish TicTacToe.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:6543
EXPOSE 6543

ENTRYPOINT ["dotnet", "TicTacToe.dll"]
