using etvctl.Api;

namespace etvctl.Models;

public class FFmpegProfileModel
{
    public FFmpegProfileModel()
    {
    }

    public FFmpegProfileModel(FFmpegFullProfileResponseModel model)
    {
        Name = model.Name;
        ThreadCount = model.ThreadCount;
        HardwareAcceleration = model.HardwareAcceleration;

        if (HardwareAcceleration is (HardwareAccelerationKind.Vaapi or HardwareAccelerationKind.Qsv))
        {
            VaapiDisplay = model.VaapiDisplay;
            VaapiDriver = model.VaapiDriver;
            VaapiDevice = model.VaapiDevice;
        }

        if (HardwareAcceleration is HardwareAccelerationKind.Qsv)
        {
            QsvExtraHardwareFrames = model.QsvExtraHardwareFrames;
        }

        Resolution = model.Resolution;
        ScalingBehavior = model.ScalingBehavior;
        VideoFormat = model.VideoFormat;
        VideoProfile = model.VideoProfile;
        VideoPreset = model.VideoPreset;
        AllowBFrames = model.AllowBFrames;
        BitDepth = model.BitDepth;
        VideoBitrate = model.VideoBitrate;
        VideoBufferSize = model.VideoBufferSize;
        TonemapAlgorithm = model.TonemapAlgorithm;
        AudioFormat = model.AudioFormat;
        AudioBitrate = model.AudioBitrate;
        AudioBufferSize = model.AudioBufferSize;
        NormalizeLoudnessMode = model.NormalizeLoudnessMode;
        AudioChannels = model.AudioChannels;
        AudioSampleRate = model.AudioSampleRate;
        NormalizeFramerate = model.NormalizeFramerate;
        DeinterlaceVideo = model.DeinterlaceVideo;
    }

    public string? Name { get; set; }
    public int ThreadCount { get; set; }
    public HardwareAccelerationKind HardwareAcceleration { get; set; }
    public string? VaapiDisplay { get; set; }
    public VaapiDriver? VaapiDriver { get; set; }
    public string? VaapiDevice { get; set; }
    public int? QsvExtraHardwareFrames { get; set; }
    public string? Resolution { get; set; }
    public ScalingBehavior? ScalingBehavior { get; set; }
    public FFmpegProfileVideoFormat? VideoFormat { get; set; }
    public string? VideoProfile { get; set; }
    public string? VideoPreset { get; set; }
    public bool? AllowBFrames { get; set; }
    public FFmpegProfileBitDepth? BitDepth { get; set; }
    public int? VideoBitrate { get; set; }
    public int? VideoBufferSize { get; set; }
    public FFmpegProfileTonemapAlgorithm? TonemapAlgorithm { get; set; }
    public FFmpegProfileAudioFormat? AudioFormat { get; set; }
    public int? AudioBitrate { get; set; }
    public int? AudioBufferSize { get; set; }
    public NormalizeLoudnessMode? NormalizeLoudnessMode { get; set; }
    public int? AudioChannels { get; set; }
    public int? AudioSampleRate { get; set; }
    public bool? NormalizeFramerate { get; set; }
    public bool? DeinterlaceVideo { get; set; }
}
