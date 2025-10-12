using System.Text.Json.Serialization;
using etvctl.Api;

namespace etvctl;

[JsonSerializable(typeof(ChannelResponseModel))]
[JsonSerializable(typeof(ICollection<ChannelResponseModel>))]
[JsonSerializable(typeof(CombinedVersion))]
[JsonSerializable(typeof(SmartCollectionResponseModel))]
[JsonSerializable(typeof(ICollection<SmartCollectionResponseModel>))]
internal sealed partial class RefitSerializerContext : JsonSerializerContext;
