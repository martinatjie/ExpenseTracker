using ExpenseProcessing.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

// Load configuration from appsettings.json, user secrets, and environment variables
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables();

IConfiguration configuration = builder.Build();

var inputPath = configuration["PdfInputPath"];
var outputPath = configuration["PdfOutputPath"];

// Create a logger factory and logger instance for ITransformationService
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<ITransformationService>();

var transformationService = new TransformationService(logger);

/*using (var fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
{
    transformationService.TransformData(fileStream);
}*/

//transformationService.TransformData(inputPath);

transformationService.TransformDataAdvanced(inputPath, outputPath);
Console.ReadLine();
