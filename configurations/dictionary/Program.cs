using System.Text.Json;
using dictionary;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions<DictionaryOptions>()
    .Configure<IConfiguration>((options, configuration) =>
    {
        configuration.GetSection("Strings").Bind(options.Strings);

        var modelSettings = new Dictionary<string, int>();
        configuration.GetSection("ModelSettings").Bind(modelSettings);

        foreach (var modelSetting in modelSettings)
        {
            var modelType = Type.GetType(modelSetting.Key);
            if (modelType is not null)
            {
                options.ModelSettings[modelType] = modelSetting.Value;
            }
        }
    });

var app = builder.Build();

app.MapGet("/", (IOptions<DictionaryOptions> options) => options.Value);

app.Run();
