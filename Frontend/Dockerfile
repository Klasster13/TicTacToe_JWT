FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG API_URL=http://localhost:6543

WORKDIR /src

COPY TicTacToe.Client/TicTacToe.Client.csproj .
RUN dotnet restore TicTacToe.Client.csproj
COPY TicTacToe.Client/ .
RUN echo "{ \"ConnectionStrings\": { \"ApiConnection\": \"${API_URL}\" } }" > wwwroot/appsettings.json
RUN dotnet publish TicTacToe.Client.csproj -c Release -o /app/publish


FROM nginx:alpine

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

RUN chmod -R 755 /usr/share/nginx/html

EXPOSE 7654
