using System.Text.Json.Serialization;
using etvctl.Api;
using Refit;

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
[JsonSerializable(typeof(ProblemDetails))]
[JsonSerializable(typeof(ResolutionViewModel))]
[JsonSourceGenerationOptions(Converters = [
    typeof(JsonStringEnumConverter<HardwareAccelerationKind>),
    typeof(JsonStringEnumConverter<VaapiDriver>),
    typeof(JsonStringEnumConverter<ScalingBehavior>),
    typeof(JsonStringEnumConverter<FFmpegProfileVideoFormat>),
    typeof(JsonStringEnumConverter<FFmpegProfileBitDepth>),
    typeof(JsonStringEnumConverter<FFmpegProfileTonemapAlgorithm>),
    typeof(JsonStringEnumConverter<FFmpegProfileAudioFormat>),
    typeof(JsonStringEnumConverter<NormalizeLoudnessMode>)
])]
internal sealed partial class RefitSerializerContext : JsonSerializerContext;
