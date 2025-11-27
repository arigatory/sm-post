FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5263

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore
COPY ["SM-Post/Post.Query/Post.Query.Api/Post.Query.Api.csproj", "SM-Post/Post.Query/Post.Query.Api/"]
COPY ["SM-Post/Post.Query/Post.Query.Domain/Post.Query.Domain.csproj", "SM-Post/Post.Query/Post.Query.Domain/"]
COPY ["SM-Post/Post.Query/Post.Query.Infrastructure/Post.Query.Infrastructure.csproj", "SM-Post/Post.Query/Post.Query.Infrastructure/"]
COPY ["SM-Post/Post.Common/Post.Common.csproj", "SM-Post/Post.Common/"]
COPY ["CQRS-ES/CQRS.Core/CQRS.Core.csproj", "CQRS-ES/CQRS.Core/"]

RUN dotnet restore "SM-Post/Post.Query/Post.Query.Api/Post.Query.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/SM-Post/Post.Query/Post.Query.Api"
RUN dotnet build "Post.Query.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Post.Query.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Post.Query.Api.dll"]
