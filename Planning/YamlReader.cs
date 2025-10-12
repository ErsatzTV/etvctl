using etvctl.Models;
using etvctl.Models.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Planning;

public static class YamlReader
{
    public static async Task<TemplateModel> ReadTemplate(ConfigModel config, CancellationToken cancellationToken)
    {
        var deserializer = new StaticDeserializerBuilder(new YamlStaticContext())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        string folder = config.Template!;

        // check for single file first
        string singleFile = Path.Combine(folder, "ersatztv.yml");
        if (File.Exists(singleFile))
        {
            string singleFileText = await File.ReadAllTextAsync(singleFile, cancellationToken);
            return deserializer.Deserialize<TemplateModel>(singleFileText);
        }

        var result = new TemplateModel { SmartCollections = [] };

        // check for file per type
        string smartCollectionsFile = Path.Combine(folder, "collections", "smart.yml");
        if (File.Exists(smartCollectionsFile))
        {
            string smartCollectionsText = await File.ReadAllTextAsync(smartCollectionsFile, cancellationToken);
            result.SmartCollections.AddRange(
                deserializer.Deserialize<List<SmartCollectionModel>>(smartCollectionsText));
        }
        else
        {
            // load individual smart collections from folder
            string smartCollectionsFolder = Path.Combine(folder, "collections", "smart");
            if (Directory.Exists(smartCollectionsFolder))
            {
                IEnumerable<string> allFiles = Directory.EnumerateFiles(smartCollectionsFolder, "*.yml", SearchOption.TopDirectoryOnly);
                foreach (string file in allFiles)
                {
                    string smartCollectionText = await File.ReadAllTextAsync(file, cancellationToken);
                    result.SmartCollections.Add(deserializer.Deserialize<SmartCollectionModel>(smartCollectionText));
                }
            }
        }

        return result;
    }
}
