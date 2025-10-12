using YamlDotNet.Serialization;

namespace etvctl.Models.Config;

public class OrganizationOverridesModel
{
    [YamlMember(Alias = "smart_collection", ApplyNamingConventions = false)]
    public FileOrganization? SmartCollection { get; set; }
}
