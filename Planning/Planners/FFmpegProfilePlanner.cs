using System.Net;
using etvctl.Api;
using etvctl.Models;
using Refit;
using Spectre.Console;

namespace etvctl.Planning;

public class FFmpegProfilePlanner(IErsatzTVv1 client) : BasePlanner<FFmpegProfileModel, FFmpegFullProfileResponseModel>
{
    public override async Task<ChangeSet<FFmpegProfileModel, FFmpegFullProfileResponseModel>> Plan(
        TemplateModel templateModel,
        CancellationToken cancellationToken)
    {
        // load current ffmpeg profiles
        ICollection<FFmpegFullProfileResponseModel> currentFFmpegProfiles =
            await client.GetFFmpegProfiles(cancellationToken);

        // diff and plan
        var toAdd = templateModel.FFmpegProfiles
            .Where(x => currentFFmpegProfiles.All(y => y.Name != x.Name))
            .Where(x => x.Rename?.From is null)
            .ToList();

        foreach (var profile in toAdd)
        {
            _ = await client.GetResolutionByName(profile.Resolution ?? string.Empty, cancellationToken);
        }

        var toUpdate = templateModel.FFmpegProfiles
            .Where(x => currentFFmpegProfiles.Any(y => y.Name == x.Name && HasChanges(y, x)))
            .Select(x => new Tuple<FFmpegProfileModel, FFmpegFullProfileResponseModel>(
                x,
                currentFFmpegProfiles.First(y => y.Name == x.Name)))
            .ToList();

        foreach (var ff in templateModel.FFmpegProfiles.Where(x => !string.IsNullOrWhiteSpace(x.Rename?.From)))
        {
            var from = currentFFmpegProfiles.FirstOrDefault(x => string.Equals(x.Name, ff.Rename?.From));
            if (from != null)
            {
                toUpdate.Add(new Tuple<FFmpegProfileModel, FFmpegFullProfileResponseModel>(ff, from));
            }
        }

        foreach ((FFmpegProfileModel profile, _) in toUpdate)
        {
            _ = await client.GetResolutionByName(profile.Resolution ?? string.Empty, cancellationToken);
        }

        var toRemove = currentFFmpegProfiles
            .Where(x => templateModel.FFmpegProfiles.All(y => y.Name != x.Name))
            .Where(x => templateModel.FFmpegProfiles.All(y => y.Rename?.From != x.Name))
            .ToList();

        return new ChangeSet<FFmpegProfileModel, FFmpegFullProfileResponseModel>
        {
            ToAdd = toAdd,
            ToRemove = toRemove,
            ToUpdate = toUpdate
        };
    }

    public override async Task Apply(PlanModel plan, CancellationToken cancellationToken)
    {
        foreach (var toAdd in plan.FFmpegProfiles.ToAdd)
        {
            if (string.IsNullOrEmpty(toAdd.Name) || string.IsNullOrWhiteSpace(toAdd.Resolution))
            {
                continue;
            }

            ResolutionViewModel resolution = await client.GetResolutionByName(toAdd.Resolution, cancellationToken);

            await client.CreateFFmpegProfile(
                new CreateFFmpegProfile
                {
                    Name = toAdd.Name,
                    ThreadCount = toAdd.ThreadCount,
                    HardwareAcceleration = toAdd.HardwareAcceleration,
                    VaapiDisplay = toAdd.VaapiDisplay ?? "drm",
                    VaapiDriver = toAdd.VaapiDriver ?? VaapiDriver.Default,
                    VaapiDevice = toAdd.VaapiDevice ?? string.Empty,
                    QsvExtraHardwareFrames = toAdd.QsvExtraHardwareFrames,
                    ResolutionId = resolution.Id,
                    ScalingBehavior = toAdd.ScalingBehavior ?? ScalingBehavior.ScaleAndPad,
                    VideoFormat = toAdd.VideoFormat ?? FFmpegProfileVideoFormat.H264,
                    VideoProfile = toAdd.VideoProfile ?? string.Empty,
                    VideoPreset = toAdd.VideoPreset ?? string.Empty,
                    AllowBFrames = toAdd.AllowBFrames ?? false,
                    BitDepth = toAdd.BitDepth ?? FFmpegProfileBitDepth.EightBit,
                    VideoBitrate = toAdd.VideoBitrate ?? 2000,
                    VideoBufferSize = toAdd.VideoBufferSize ?? 4000,
                    TonemapAlgorithm = toAdd.TonemapAlgorithm ?? FFmpegProfileTonemapAlgorithm.Linear,
                    AudioFormat = toAdd.AudioFormat ?? FFmpegProfileAudioFormat.Aac,
                    AudioBitrate = toAdd.AudioBitrate ?? 192,
                    AudioBufferSize = toAdd.AudioBufferSize ?? 384,
                    NormalizeLoudnessMode = toAdd.NormalizeLoudnessMode ?? NormalizeLoudnessMode.Off,
                    AudioChannels = toAdd.AudioChannels ?? 2,
                    AudioSampleRate = toAdd.AudioSampleRate ?? 48,
                    NormalizeFramerate = toAdd.NormalizeFramerate ?? false,
                    DeinterlaceVideo = toAdd.DeinterlaceVideo ?? true
                },
                cancellationToken);
        }

        foreach ((FFmpegProfileModel toUpdateNew, FFmpegFullProfileResponseModel toUpdateOld) in plan.FFmpegProfiles
                     .ToUpdate)
        {
            if (string.IsNullOrWhiteSpace(toUpdateNew.Name) || string.IsNullOrWhiteSpace(toUpdateNew.Resolution))
            {
                continue;
            }

            ResolutionViewModel resolution = await client.GetResolutionByName(toUpdateNew.Resolution, cancellationToken);

            await client.UpdateFFmpegProfile(
                new UpdateFFmpegProfile
                {
                    FFmpegProfileId = toUpdateOld.Id,
                    Name = toUpdateNew.Name,
                    ThreadCount = toUpdateNew.ThreadCount,
                    HardwareAcceleration = toUpdateNew.HardwareAcceleration,
                    VaapiDisplay = toUpdateNew.VaapiDisplay ?? "drm",
                    VaapiDriver = toUpdateNew.VaapiDriver ?? VaapiDriver.Default,
                    VaapiDevice = toUpdateNew.VaapiDevice ?? string.Empty,
                    QsvExtraHardwareFrames = toUpdateNew.QsvExtraHardwareFrames,
                    ResolutionId = resolution.Id,
                    ScalingBehavior = toUpdateNew.ScalingBehavior ?? ScalingBehavior.ScaleAndPad,
                    VideoFormat = toUpdateNew.VideoFormat ?? FFmpegProfileVideoFormat.H264,
                    VideoProfile = toUpdateNew.VideoProfile ?? string.Empty,
                    VideoPreset = toUpdateNew.VideoPreset ?? string.Empty,
                    AllowBFrames = toUpdateNew.AllowBFrames ?? false,
                    BitDepth = toUpdateNew.BitDepth ?? FFmpegProfileBitDepth.EightBit,
                    VideoBitrate = toUpdateNew.VideoBitrate ?? 2000,
                    VideoBufferSize = toUpdateNew.VideoBufferSize ?? 4000,
                    TonemapAlgorithm = toUpdateNew.TonemapAlgorithm ?? FFmpegProfileTonemapAlgorithm.Linear,
                    AudioFormat = toUpdateNew.AudioFormat ?? FFmpegProfileAudioFormat.Aac,
                    AudioBitrate = toUpdateNew.AudioBitrate ?? 192,
                    AudioBufferSize = toUpdateNew.AudioBufferSize ?? 384,
                    NormalizeLoudnessMode = toUpdateNew.NormalizeLoudnessMode ?? NormalizeLoudnessMode.Off,
                    AudioChannels = toUpdateNew.AudioChannels ?? 2,
                    AudioSampleRate = toUpdateNew.AudioSampleRate ?? 48,
                    NormalizeFramerate = toUpdateNew.NormalizeFramerate ?? false,
                    DeinterlaceVideo = toUpdateNew.DeinterlaceVideo ?? true
                },
                cancellationToken);
        }

        foreach (var toRemove in plan.FFmpegProfiles.ToRemove)
        {
            try
            {
                await client.DeleteFFmpegProfile(toRemove.Id, cancellationToken);
            }
            catch (ApiException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                AnsiConsole.MarkupLine("[red] ffmpeg profile \"{0}\" cannot be deleted.[/]", Markup.Escape(toRemove.Name));
            }
        }
    }

    private static bool HasChanges(FFmpegFullProfileResponseModel current, FFmpegProfileModel template)
    {
        bool isQsv = template.HardwareAcceleration is HardwareAccelerationKind.Qsv ||
                     current.HardwareAcceleration is HardwareAccelerationKind.Qsv;
        bool isVaapiOrQsv = isQsv || (template.HardwareAcceleration is HardwareAccelerationKind.Vaapi ||
                                      current.HardwareAcceleration is HardwareAccelerationKind.Vaapi);

        return template.ThreadCount != current.ThreadCount ||
               template.HardwareAcceleration != current.HardwareAcceleration ||
               (isVaapiOrQsv && template.VaapiDisplay != current.VaapiDisplay) ||
               (isVaapiOrQsv && template.VaapiDriver != current.VaapiDriver) ||
               (isVaapiOrQsv && template.VaapiDevice != current.VaapiDevice) ||
               (isQsv && template.QsvExtraHardwareFrames != current.QsvExtraHardwareFrames) ||
               HasStringChanges(template.Resolution, current.Resolution) ||
               template.ScalingBehavior != current.ScalingBehavior ||
               template.VideoFormat != current.VideoFormat ||
               HasStringChanges(template.VideoProfile, current.VideoProfile) ||
               HasStringChanges(template.VideoPreset, current.VideoPreset) ||
               template.AllowBFrames != current.AllowBFrames ||
               template.BitDepth != current.BitDepth ||
               template.VideoBitrate != current.VideoBitrate ||
               template.VideoBufferSize != current.VideoBufferSize ||
               template.TonemapAlgorithm != current.TonemapAlgorithm ||
               template.AudioFormat != current.AudioFormat ||
               template.AudioBitrate != current.AudioBitrate ||
               template.AudioBufferSize != current.AudioBufferSize ||
               template.NormalizeLoudnessMode != current.NormalizeLoudnessMode ||
               template.AudioChannels != current.AudioChannels ||
               template.AudioSampleRate != current.AudioSampleRate ||
               template.NormalizeFramerate != current.NormalizeFramerate ||
               template.DeinterlaceVideo != current.DeinterlaceVideo;
    }
}
