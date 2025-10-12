using System.Text.Json.Serialization;
using etvctl.Api;

namespace etvctl;

[JsonSerializable(typeof(ChannelResponseModel))]
[JsonSerializable(typeof(ICollection<ChannelResponseModel>))]
internal sealed partial class RefitSerializerContext : JsonSerializerContext;
