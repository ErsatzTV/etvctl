using YamlDotNet.Serialization;

namespace etvctl.Models.Config;

public class OrganizationOverridesModel
{
    [YamlMember(Alias = "ffmpeg_profile", ApplyNamingConventions = false)]
    public FileOrganization? FFmpegProfile { get; set; }

    [YamlMember(Alias = "smart_collection", ApplyNamingConventions = false)]
    public FileOrganization? SmartCollection { get; set; }
}
