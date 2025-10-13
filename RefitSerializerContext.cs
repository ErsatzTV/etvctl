using System.Text.Json.Serialization;
using etvctl.Api;

namespace etvctl;

[JsonSerializable(typeof(ChannelResponseModel))]
[JsonSerializable(typeof(ICollection<ChannelResponseModel>))]
[JsonSerializable(typeof(CombinedVersion))]
[JsonSerializable(typeof(SmartCollectionResponseModel))]
[JsonSerializable(typeof(ICollection<SmartCollectionResponseModel>))]
[JsonSerializable(typeof(CreateSmartCollection))]
[JsonSerializable(typeof(UpdateSmartCollection))]
[JsonSerializable(typeof(FFmpegFullProfileResponseModel))]
[JsonSerializable(typeof(ICollection<FFmpegFullProfileResponseModel>))]
[JsonSerializable(typeof(HardwareAccelerationKind))]
[JsonSerializable(typeof(CreateFFmpegProfile))]
[JsonSerializable(typeof(UpdateFFmpegProfile))]
internal sealed partial class RefitSerializerContext : JsonSerializerContext;
