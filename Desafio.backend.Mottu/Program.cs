using Desafio.backend.Mottu.Dominio.Interfaces;
using Desafio.backend.Mottu.Infraestrutura.Mensageria;
using Desafio.backend.Mottu.Infraestrutura.Repositorios;
using Desafio.backend.Mottu.Queue.Interfaces;
using Desafio.backend.Mottu.Queue;
using Desafio.backend.Mottu.Servico;
using MongoDB.Driver;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar o HttpClient
builder.Services.AddHttpClient();

// Obter as configura��es do RabbitMQ
var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");

// Configurar o ConnectionFactory usando as configura��es
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = rabbitMQConfig["HostName"],
        Port = int.Parse(rabbitMQConfig["Port"]),
        UserName = rabbitMQConfig["UserName"],
        Password = rabbitMQConfig["Password"]
    };
    return factory.CreateConnection();
});


// Configura��o do MongoDB
builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetValue<string>("MongoDB:ConnectionString");

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("MongoDB connection string is missing.");
    }

    return new MongoClient(connectionString);
});

// Configura��o da base de dados do MongoDB
builder.Services.AddSingleton(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName");

    if (string.IsNullOrEmpty(databaseName))
    {
        throw new InvalidOperationException("MongoDB database name is missing.");
    }

    return mongoClient.GetDatabase(databaseName);
});

// Inje��o de depend�ncias para os reposit�rios e servi�os
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<IMensageriaService, MensageriaService>();

// Obter a configura��o
var configuration = builder.Configuration;
builder.Services.AddScoped<IElasticLogService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var elasticUrl = configuration["ElasticConfiguration:Url"]; // Obter a URL da configura��o
    return new ElasticLogService(httpClientFactory, elasticUrl);
});

// Adicionando o consumidor como servi�o hospedado
builder.Services.AddHostedService<MotoCadastradaConsumidor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
