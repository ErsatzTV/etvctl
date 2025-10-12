using etvctl.Models;
using YamlDotNet.Serialization;

namespace etvctl;

[YamlStaticContext]
[YamlSerializable(typeof(RootModel))]
[YamlSerializable(typeof(ChannelModel))]
[YamlSerializable(typeof(SmartCollectionModel))]
public partial class YamlStaticContext : YamlDotNet.Serialization.StaticContext
{
}
