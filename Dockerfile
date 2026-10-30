# ===== 阶段1：build —— 用 SDK镜像（含编译工具链，大）=====
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 精髓所在：先【只】拷贝项目文件，单独还原依赖
COPY TodoApi/TodoApi.csproj TodoApi/
RUN dotnet restore TodoApi/TodoApi.csproj

# 然后才拷贝源码，发布
COPY TodoApi/ TodoApi/
RUN dotnet publish TodoApi/TodoApi.csproj -c Release -o /app/publish

# ===== 阶段2：final —— 只有运行时，没有sdk，小 =====
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
#只从阶段1借走成果物
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApi.dll"]