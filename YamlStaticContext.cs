using etvctl.Models;
using YamlDotNet.Serialization;

namespace etvctl;

[YamlStaticContext]
[YamlSerializable(typeof(ConfigModel))]
[YamlSerializable(typeof(RootModel))]
[YamlSerializable(typeof(ChannelModel))]
[YamlSerializable(typeof(SmartCollectionModel))]
[YamlSerializable(typeof(RenameModel))]
public partial class YamlStaticContext : YamlDotNet.Serialization.StaticContext
{
}
