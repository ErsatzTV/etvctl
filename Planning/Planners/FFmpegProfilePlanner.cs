using etvctl.Api;
using etvctl.Models;

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

    public override Task Apply(PlanModel plan, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
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
