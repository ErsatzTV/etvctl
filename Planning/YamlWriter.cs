using etvctl.Models;
using etvctl.Models.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace etvctl.Planning;

public static class YamlWriter
{
    public static async Task WriteTemplate(
        ConfigModel config,
        TemplateModel templateModel,
        CancellationToken cancellationToken = default)
    {
        var serializer = new StaticSerializerBuilder(new YamlStaticContext())
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new FileOrganizationTypeConverter())
            .Build();

        var defaultOrganization = config.Organization?.Default ?? FileOrganization.SingleFile;

        // write each resource type

        string singleFileName = Path.Combine(config.Template!, "ersatztv.yml");
        string collectionsFolderName = Path.Combine(config.Template!, "collections");
        string smartCollectionsFileName = Path.Combine(collectionsFolderName, "smart.yml");
        string smartCollectionsFolderName = Path.Combine(config.Template!, "collections", "smart");

        // smart collections
        var smartCollectionOrganization = config.Organization?.Overrides?.SmartCollection ?? defaultOrganization;
        if (smartCollectionOrganization is FileOrganization.FilePerType)
        {
            if (!Directory.Exists(collectionsFolderName))
            {
                Directory.CreateDirectory(collectionsFolderName);
            }

            if (Directory.Exists(smartCollectionsFolderName))
            {
                Directory.Delete(smartCollectionsFolderName, true);
            }

            await File.WriteAllTextAsync(
                smartCollectionsFileName,
                serializer.Serialize(templateModel.SmartCollections),
                cancellationToken);

            // done writing smart collections; don't write again
            templateModel.SmartCollections.Clear();
        }
        else if (smartCollectionOrganization is FileOrganization.FilePerResource)
        {
            if (!Directory.Exists(smartCollectionsFolderName))
            {
                Directory.CreateDirectory(smartCollectionsFolderName);
            }
            else
            {
                Directory.Delete(smartCollectionsFolderName, true);
                Directory.CreateDirectory(smartCollectionsFolderName);
            }

            foreach (var smartCollection in templateModel.SmartCollections)
            {
                if (string.IsNullOrWhiteSpace(smartCollection.Name))
                {
                    continue;
                }

                string fileName = Path.Combine(
                    smartCollectionsFolderName,
                    $"{smartCollection.Name!.Replace(" ", "-")}.yml");
                await File.WriteAllTextAsync(
                    fileName,
                    serializer.Serialize(smartCollection),
                    cancellationToken);
            }

            // done writing smart collections; don't write again
            templateModel.SmartCollections.Clear();
        }
        else
        {
            if (File.Exists(smartCollectionsFileName))
            {
                File.Delete(smartCollectionsFileName);
            }

            if (Directory.Exists(smartCollectionsFolderName))
            {
                Directory.Delete(smartCollectionsFolderName, true);
            }
        }

        // write any remaining types to the single file
        if (File.Exists(singleFileName))
        {
            File.Delete(singleFileName);
        }

        if (!templateModel.IsEmpty())
        {
            await File.WriteAllTextAsync(singleFileName, serializer.Serialize(templateModel), cancellationToken);
        }
    }
}
