FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim as build
WORKDIR /app
EXPOSE 80
COPY Shared/FreeCourse.Shared/*.csproj Shared/FreeCourse.Shared/
COPY Services/FakePayment/FreeCourse.Service.FakePayment/*.csproj Services/FakePayment/FreeCourse.Service.FakePayment/
RUN dotnet restore Services/FakePayment/FreeCourse.Service.FakePayment/*.csproj
COPY . .
RUN dotnet publish Services/FakePayment/FreeCourse.Service.FakePayment/*.csproj -c Release -o out
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim as runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet","FreeCourse.Service.FakePayment.dll" ]