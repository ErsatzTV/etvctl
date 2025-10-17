using etvctl.Api;
using etvctl.Generator;

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

    [EtvPrinterOrder(1)]
    public string? Name { get; set; }

    [EtvPrinterOrder(2)]
    public int ThreadCount { get; set; }

    [EtvPrinterOrder(3)]
    public HardwareAccelerationKind HardwareAcceleration { get; set; }

    [EtvPrinterOrder(4)]
    public string? VaapiDisplay { get; set; }

    [EtvPrinterOrder(5)]
    public VaapiDriver? VaapiDriver { get; set; }

    [EtvPrinterOrder(6)]
    public string? VaapiDevice { get; set; }

    [EtvPrinterOrder(7)]
    public int? QsvExtraHardwareFrames { get; set; }

    [EtvPrinterOrder(8)]
    public string? Resolution { get; set; }

    [EtvPrinterOrder(9)]
    public ScalingBehavior? ScalingBehavior { get; set; }

    [EtvPrinterOrder(10)]
    public FFmpegProfileVideoFormat? VideoFormat { get; set; }

    [EtvPrinterOrder(11)]
    public string? VideoProfile { get; set; }

    [EtvPrinterOrder(12)]
    public string? VideoPreset { get; set; }

    [EtvPrinterOrder(13)]
    public bool? AllowBFrames { get; set; }

    [EtvPrinterOrder(14)]
    public FFmpegProfileBitDepth? BitDepth { get; set; }

    [EtvPrinterOrder(15)]
    public int? VideoBitrate { get; set; }

    [EtvPrinterOrder(16)]
    public int? VideoBufferSize { get; set; }

    [EtvPrinterOrder(17)]
    public FFmpegProfileTonemapAlgorithm? TonemapAlgorithm { get; set; }

    [EtvPrinterOrder(18)]
    public FFmpegProfileAudioFormat? AudioFormat { get; set; }

    [EtvPrinterOrder(19)]
    public int? AudioBitrate { get; set; }

    [EtvPrinterOrder(20)]
    public int? AudioBufferSize { get; set; }

    [EtvPrinterOrder(21)]
    public NormalizeLoudnessMode? NormalizeLoudnessMode { get; set; }

    [EtvPrinterOrder(22)]
    public int? AudioChannels { get; set; }

    [EtvPrinterOrder(23)]
    public int? AudioSampleRate { get; set; }

    [EtvPrinterOrder(24)]
    public bool? NormalizeFramerate { get; set; }

    [EtvPrinterOrder(25)]
    public bool? DeinterlaceVideo { get; set; }

    public RenameModel? Rename { get; set; }
}
