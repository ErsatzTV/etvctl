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
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitEmptyCollections)
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTypeConverter(new FileOrganizationTypeConverter())
            .Build();

        var defaultOrganization = config.Organization?.Default ?? FileOrganization.SingleFile;

        // write each resource type

        string singleFileName = Path.Combine(config.Template!, "ersatztv.yml");

        // ffmpeg profiles
        string ffmpegProfilesFolderName = Path.Combine(config.Template!, "ffmpeg_profiles");
        string ffmpegProfilesFileName = Path.Combine(config.Template!, "ffmpeg_profiles.yml");

        var ffmpegProfilesOrganization = config.Organization?.Overrides?.FFmpegProfile ?? defaultOrganization;
        if (ffmpegProfilesOrganization is FileOrganization.FilePerType)
        {
            if (Directory.Exists(ffmpegProfilesFolderName))
            {
                Directory.Delete(ffmpegProfilesFolderName, true);
            }

            await File.WriteAllTextAsync(
                ffmpegProfilesFileName,
                serializer.Serialize(templateModel.FFmpegProfiles),
                cancellationToken);

            // dont writing ffmpeg profiles; don't write again
            templateModel.FFmpegProfiles.Clear();
        }
        else if (ffmpegProfilesOrganization is FileOrganization.FilePerResource)
        {
            if (!Directory.Exists(ffmpegProfilesFolderName))
            {
                Directory.CreateDirectory(ffmpegProfilesFolderName);
            }
            else
            {
                Directory.Delete(ffmpegProfilesFolderName, true);
                Directory.CreateDirectory(ffmpegProfilesFolderName);
            }

            if (File.Exists(ffmpegProfilesFileName))
            {
                File.Delete(ffmpegProfilesFileName);
            }

            foreach (var ffmpegProfile in templateModel.FFmpegProfiles)
            {
                if (string.IsNullOrWhiteSpace(ffmpegProfile.Name))
                {
                    continue;
                }

                string fileName = Path.Combine(
                    ffmpegProfilesFolderName,
                    $"{ffmpegProfile.Name!.Replace(" ", "-")}.yml");
                await File.WriteAllTextAsync(
                    fileName,
                    serializer.Serialize(ffmpegProfile),
                    cancellationToken);
            }

            // done writing ffmpeg profiles; don't write again
            templateModel.FFmpegProfiles.Clear();
        }
        else
        {
            if (File.Exists(ffmpegProfilesFileName))
            {
                File.Delete(ffmpegProfilesFileName);
            }

            if (Directory.Exists(ffmpegProfilesFolderName))
            {
                Directory.Delete(ffmpegProfilesFolderName, true);
            }
        }

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

            if (File.Exists(smartCollectionsFileName))
            {
                File.Delete(smartCollectionsFileName);
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
