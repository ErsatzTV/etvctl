using etvctl.Models;
using etvctl.Models.Config;
using YamlDotNet.Serialization;

namespace etvctl;

[YamlStaticContext]
[YamlSerializable(typeof(ConfigModel))]
[YamlSerializable(typeof(OrganizationModel))]
[YamlSerializable(typeof(OrganizationOverridesModel))]
[YamlSerializable(typeof(FileOrganization))]
[YamlSerializable(typeof(TemplateModel))]
[YamlSerializable(typeof(ChannelModel))]
[YamlSerializable(typeof(SmartCollectionModel))]
[YamlSerializable(typeof(RenameModel))]
public partial class YamlStaticContext : YamlDotNet.Serialization.StaticContext
{
}
