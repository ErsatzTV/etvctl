using YamlDotNet.Serialization;

namespace etvctl.Models;

public class TemplateModel
{
    [YamlMember(Alias = "ffmpeg_profiles")]
    public List<FFmpegProfileModel> FFmpegProfiles { get; set; } = [];

    [YamlMember(Alias = "smart_collections")]
    public List<SmartCollectionModel> SmartCollections { get; set; } = [];

    public bool IsEmpty() => FFmpegProfiles.Count == 0 && SmartCollections.Count == 0;
}
