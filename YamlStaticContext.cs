using etvctl.Models;
using YamlDotNet.Serialization;

namespace etvctl;

[YamlStaticContext]
[YamlSerializable(typeof(RootModel))]
[YamlSerializable(typeof(ChannelModel))]
public partial class YamlStaticContext : YamlDotNet.Serialization.StaticContext
{
}
