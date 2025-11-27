FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5262

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj", "SM-Post/Post.Cmd/Post.Cmd.Api/"]
COPY ["SM-Post/Post.Cmd/Post.Cmd.Domain/Post.Cmd.Domain.csproj", "SM-Post/Post.Cmd/Post.Cmd.Domain/"]
COPY ["SM-Post/Post.Cmd/Post.Cmd.Infrastructure/Post.Cmd.Infrastructure.csproj", "SM-Post/Post.Cmd/Post.Cmd.Infrastructure/"]
COPY ["SM-Post/Post.Common/Post.Common.csproj", "SM-Post/Post.Common/"]
COPY ["CQRS-ES/CQRS.Core/CQRS.Core.csproj", "CQRS-ES/CQRS.Core/"]

RUN dotnet restore "SM-Post/Post.Cmd/Post.Cmd.Api/Post.Cmd.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SM-Post/Post.Cmd/Post.Cmd.Api"
RUN dotnet build "Post.Cmd.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.Cmd.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Cmd.Api.dll"]
