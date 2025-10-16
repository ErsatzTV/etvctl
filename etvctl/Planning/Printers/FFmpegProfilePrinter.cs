using etvctl.Api;
using etvctl.Models;
using Spectre.Console;

namespace etvctl.Planning;

public partial class FFmpegProfilePrinter : BaseComparer
{
    public static void Print(PlanModel plan)
    {
        if (plan.FFmpegProfiles.ToAdd.Count != 0)
        {
            foreach (var ffmpegProfile in plan.FFmpegProfiles.ToAdd)
            {
                AnsiConsole.MarkupLine($"  # FFmpeg Profile \"{ffmpegProfile.Name}\" will be created");
                AnsiConsole.MarkupLine($"  [green]+ resource \"ffmpeg_profile\" \"{ffmpegProfile.Name}\" {{[/]");
                AnsiConsole.MarkupLine($"  [green]    + name:\t\"{ffmpegProfile.Name}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + thread_count:\t\"{ffmpegProfile.ThreadCount}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + hardware_acceleration:\t\"{ffmpegProfile.HardwareAcceleration}\"[/]");

                if (ffmpegProfile.HardwareAcceleration is (HardwareAccelerationKind.Vaapi
                    or HardwareAccelerationKind.Qsv))
                {
                    AnsiConsole.MarkupLine($"  [green]    + vaapi_display:\t\"{ffmpegProfile.VaapiDisplay}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + vaapi_driver:\t\"{ffmpegProfile.VaapiDriver}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + vaapi_device:\t\"{ffmpegProfile.VaapiDevice}\"[/]");
                }

                if (ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv)
                {
                    AnsiConsole.MarkupLine($"  [green]    + qsv_extra_hardware_frames:\t\"{ffmpegProfile.QsvExtraHardwareFrames}\"[/]");
                }

                AnsiConsole.MarkupLine($"  [green]    + resolution:\t\"{ffmpegProfile.Resolution}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + scaling_behavior:\t\"{ffmpegProfile.ScalingBehavior}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + video_format:\t\"{ffmpegProfile.VideoFormat}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + video_profile:\t\"{ffmpegProfile.VideoProfile}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + video_preset:\t\"{ffmpegProfile.VideoPreset}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + allow_b_frames:\t\"{ffmpegProfile.AllowBFrames}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + bit_depth:\t\"{ffmpegProfile.BitDepth}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + video_bitrate:\t\"{ffmpegProfile.VideoBitrate}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + video_buffer_size:\t\"{ffmpegProfile.VideoBufferSize}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + tonemap_algorithm:\t\"{ffmpegProfile.TonemapAlgorithm}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + audio_format:\t\"{ffmpegProfile.AudioFormat}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + audio_bitrate:\t\"{ffmpegProfile.AudioBitrate}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + audio_buffer_size:\t\"{ffmpegProfile.AudioBufferSize}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + normalize_loudness_mode:\t\"{ffmpegProfile.NormalizeLoudnessMode}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + audio_channels:\t\"{ffmpegProfile.AudioChannels}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + audio_sample_rate:\t\"{ffmpegProfile.AudioSampleRate}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + normalize_framerate:\t\"{ffmpegProfile.NormalizeFramerate}\"[/]");
                AnsiConsole.MarkupLine($"  [green]    + deinterlace_video:\t\"{ffmpegProfile.DeinterlaceVideo}\"[/]");

                AnsiConsole.MarkupLine("  [green]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }

        if (plan.FFmpegProfiles.ToUpdate.Count != 0)
        {
            foreach ((FFmpegProfileModel newValue, FFmpegFullProfileResponseModel oldValue) in plan.FFmpegProfiles
                         .ToUpdate)
            {
                AnsiConsole.MarkupLine($"  # FFmpeg Profile \"{oldValue.Name}\" will be changed");
                AnsiConsole.MarkupLine($"  [yellow]~ resource \"ffmpeg_profile\" \"{oldValue.Name}\" {{[/]");

                if (!string.Equals(newValue.Name, oldValue.Name))
                {
                    AnsiConsole.MarkupLine("  [red]    - name:\t\"{0}\"[/]", Markup.Escape(oldValue.Name));
                    AnsiConsole.MarkupLine("  [green]    + name:\t\"{0}\"[/]", Markup.Escape(newValue.Name!));
                }

                if (oldValue.ThreadCount != newValue.ThreadCount)
                {
                    AnsiConsole.MarkupLine($"  [red]    - thread_count:\t\"{oldValue.ThreadCount}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + thread_count:\t\"{newValue.ThreadCount}\"[/]");
                }

                if (oldValue.HardwareAcceleration != newValue.HardwareAcceleration)
                {
                    AnsiConsole.MarkupLine($"  [red]    - hardware_acceleration:\t\"{oldValue.HardwareAcceleration}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + hardware_acceleration:\t\"{newValue.HardwareAcceleration}\"[/]");
                }

                bool isQsv = oldValue.HardwareAcceleration is HardwareAccelerationKind.Qsv ||
                             newValue.HardwareAcceleration is HardwareAccelerationKind.Qsv;

                bool isVaapiOrQsv = isQsv || (oldValue.HardwareAcceleration is HardwareAccelerationKind.Vaapi ||
                                              newValue.HardwareAcceleration is HardwareAccelerationKind.Vaapi);

                if (isVaapiOrQsv)
                {
                    if (HasStringChanges(oldValue.VaapiDisplay, newValue.VaapiDisplay))
                    {
                        AnsiConsole.MarkupLine($"  [red]    - vaapi_display:\t\"{oldValue.VaapiDisplay}\"[/]");
                        AnsiConsole.MarkupLine($"  [green]    + vaapi_display:\t\"{newValue.VaapiDisplay}\"[/]");
                    }

                    if (oldValue.VaapiDriver != newValue.VaapiDriver)
                    {
                        AnsiConsole.MarkupLine($"  [red]    - vaapi_driver:\t\"{oldValue.VaapiDriver}\"[/]");
                        AnsiConsole.MarkupLine($"  [green]    + vaapi_driver:\t\"{newValue.VaapiDriver}\"[/]");
                    }

                    if (HasStringChanges(oldValue.VaapiDevice, newValue.VaapiDevice))
                    {
                        AnsiConsole.MarkupLine($"  [red]    - vaapi_device:\t\"{oldValue.VaapiDevice}\"[/]");
                        AnsiConsole.MarkupLine($"  [green]    + vaapi_device:\t\"{newValue.VaapiDevice}\"[/]");
                    }
                }

                if (isQsv && oldValue.QsvExtraHardwareFrames != newValue.QsvExtraHardwareFrames)
                {
                    AnsiConsole.MarkupLine(
                        $"  [red]    - qsv_extra_hardware_frames:\t\"{oldValue.QsvExtraHardwareFrames}\"[/]");
                    AnsiConsole.MarkupLine(
                        $"  [green]    + qsv_extra_hardware_frames:\t\"{newValue.QsvExtraHardwareFrames}\"[/]");
                }

                if (HasStringChanges(oldValue.Resolution, newValue.Resolution))
                {
                    AnsiConsole.MarkupLine($"  [red]    - resolution:\t\"{oldValue.Resolution}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + resolution:\t\"{newValue.Resolution}\"[/]");
                }

                if (oldValue.ScalingBehavior != newValue.ScalingBehavior)
                {
                    AnsiConsole.MarkupLine($"  [red]    - scaling_behavior:\t\"{oldValue.ScalingBehavior}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + scaling_behavior:\t\"{newValue.ScalingBehavior}\"[/]");
                }

                if (oldValue.VideoFormat != newValue.VideoFormat)
                {
                    AnsiConsole.MarkupLine($"  [red]    - video_format:\t\"{oldValue.VideoFormat}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + video_format:\t\"{newValue.VideoFormat}\"[/]");
                }

                if (HasStringChanges(oldValue.VideoProfile, newValue.VideoProfile))
                {
                    AnsiConsole.MarkupLine($"  [red]    - video_profile:\t\"{oldValue.VideoProfile}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + video_profile:\t\"{newValue.VideoProfile}\"[/]");
                }

                if (HasStringChanges(oldValue.VideoPreset, newValue.VideoPreset))
                {
                    AnsiConsole.MarkupLine($"  [red]    - video_preset:\t\"{oldValue.VideoPreset}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + video_preset:\t\"{newValue.VideoPreset}\"[/]");
                }

                if (oldValue.AllowBFrames != newValue.AllowBFrames)
                {
                    AnsiConsole.MarkupLine($"  [red]    - allow_b_frames:\t\"{oldValue.AllowBFrames}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + allow_b_frames:\t\"{newValue.AllowBFrames}\"[/]");
                }

                if (oldValue.BitDepth != newValue.BitDepth)
                {
                    AnsiConsole.MarkupLine($"  [red]    - bit_depth:\t\"{oldValue.BitDepth}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + bit_depth:\t\"{newValue.BitDepth}\"[/]");
                }

                if (oldValue.VideoBitrate != newValue.VideoBitrate)
                {
                    AnsiConsole.MarkupLine($"  [red]    - video_bitrate:\t\"{oldValue.VideoBitrate}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + video_bitrate:\t\"{newValue.VideoBitrate}\"[/]");
                }

                if (oldValue.VideoBufferSize != newValue.VideoBufferSize)
                {
                    AnsiConsole.MarkupLine($"  [red]    - video_buffer_size:\t\"{oldValue.VideoBufferSize}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + video_buffer_size:\t\"{newValue.VideoBufferSize}\"[/]");
                }

                if (oldValue.TonemapAlgorithm != newValue.TonemapAlgorithm)
                {
                    AnsiConsole.MarkupLine($"  [red]    - tonemap_algorithm:\t\"{oldValue.TonemapAlgorithm}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + tonemap_algorithm:\t\"{newValue.TonemapAlgorithm}\"[/]");
                }

                if (oldValue.AudioFormat != newValue.AudioFormat)
                {
                    AnsiConsole.MarkupLine($"  [red]    - audio_format:\t\"{oldValue.AudioFormat}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + audio_format:\t\"{newValue.AudioFormat}\"[/]");
                }

                if (oldValue.AudioBitrate != newValue.AudioBitrate)
                {
                    AnsiConsole.MarkupLine($"  [red]    - audio_bitrate:\t\"{oldValue.AudioBitrate}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + audio_bitrate:\t\"{newValue.AudioBitrate}\"[/]");
                }

                if (oldValue.AudioBufferSize != newValue.AudioBufferSize)
                {
                    AnsiConsole.MarkupLine($"  [red]    - audio_buffer_size:\t\"{oldValue.AudioBufferSize}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + audio_buffer_size:\t\"{newValue.AudioBufferSize}\"[/]");
                }

                if (oldValue.NormalizeLoudnessMode != newValue.NormalizeLoudnessMode)
                {
                    AnsiConsole.MarkupLine($"  [red]    - normalize_loudness_mode:\t\"{oldValue.NormalizeLoudnessMode}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + normalize_loudness_mode:\t\"{newValue.NormalizeLoudnessMode}\"[/]");
                }

                if (oldValue.AudioChannels != newValue.AudioChannels)
                {
                    AnsiConsole.MarkupLine($"  [red]    - audio_channels:\t\"{oldValue.AudioChannels}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + audio_channels:\t\"{newValue.AudioChannels}\"[/]");
                }

                if (oldValue.AudioSampleRate != newValue.AudioSampleRate)
                {
                    AnsiConsole.MarkupLine($"  [red]    - audio_sample_rate:\t\"{oldValue.AudioSampleRate}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + audio_sample_rate:\t\"{newValue.AudioSampleRate}\"[/]");
                }

                if (oldValue.NormalizeFramerate != newValue.NormalizeFramerate)
                {
                    AnsiConsole.MarkupLine($"  [red]    - normalize_framerate:\t\"{oldValue.NormalizeFramerate}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + normalize_framerate:\t\"{newValue.NormalizeFramerate}\"[/]");
                }

                if (oldValue.DeinterlaceVideo != newValue.DeinterlaceVideo)
                {
                    AnsiConsole.MarkupLine($"  [red]    - deinterlace_video:\t\"{oldValue.DeinterlaceVideo}\"[/]");
                    AnsiConsole.MarkupLine($"  [green]    + deinterlace_video:\t\"{newValue.DeinterlaceVideo}\"[/]");
                }

                AnsiConsole.MarkupLine("  [yellow]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }

        if (plan.FFmpegProfiles.ToRemove.Count != 0)
        {
            foreach (var ffmpegProfile in plan.FFmpegProfiles.ToRemove)
            {
                AnsiConsole.MarkupLine($"  # FFmpeg Profile \"{ffmpegProfile.Name}\" will be deleted");
                AnsiConsole.MarkupLine($"  [red]- resource \"ffmpeg_profile\" \"{ffmpegProfile.Name}\" {{[/]");
                AnsiConsole.MarkupLine($"  [red]    - name:\t\"{ffmpegProfile.Name}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - thread_count:\t\"{ffmpegProfile.ThreadCount}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - hardware_acceleration:\t\"{ffmpegProfile.HardwareAcceleration}\"[/]");

                if (ffmpegProfile.HardwareAcceleration is (HardwareAccelerationKind.Vaapi
                    or HardwareAccelerationKind.Qsv))
                {
                    AnsiConsole.MarkupLine($"  [red]    - vaapi_display:\t\"{ffmpegProfile.VaapiDisplay}\"[/]");
                    AnsiConsole.MarkupLine($"  [red]    - vaapi_driver:\t\"{ffmpegProfile.VaapiDriver}\"[/]");
                    AnsiConsole.MarkupLine($"  [red]    - vaapi_device:\t\"{ffmpegProfile.VaapiDevice}\"[/]");
                }

                if (ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv)
                {
                    AnsiConsole.MarkupLine($"  [red]    - qsv_extra_hardware_frames:\t\"{ffmpegProfile.QsvExtraHardwareFrames}\"[/]");
                }

                AnsiConsole.MarkupLine($"  [red]    - resolution:\t\"{ffmpegProfile.Resolution}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - scaling_behavior:\t\"{ffmpegProfile.ScalingBehavior}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - video_format:\t\"{ffmpegProfile.VideoFormat}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - video_profile:\t\"{ffmpegProfile.VideoProfile}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - video_preset:\t\"{ffmpegProfile.VideoPreset}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - allow_b_frames:\t\"{ffmpegProfile.AllowBFrames}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - bit_depth:\t\"{ffmpegProfile.BitDepth}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - video_bitrate:\t\"{ffmpegProfile.VideoBitrate}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - video_buffer_size:\t\"{ffmpegProfile.VideoBufferSize}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - tonemap_algorithm:\t\"{ffmpegProfile.TonemapAlgorithm}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - audio_format:\t\"{ffmpegProfile.AudioFormat}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - audio_bitrate:\t\"{ffmpegProfile.AudioBitrate}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - audio_buffer_size:\t\"{ffmpegProfile.AudioBufferSize}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - normalize_loudness_mode:\t\"{ffmpegProfile.NormalizeLoudnessMode}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - audio_channels:\t\"{ffmpegProfile.AudioChannels}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - audio_sample_rate:\t\"{ffmpegProfile.AudioSampleRate}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - normalize_framerate:\t\"{ffmpegProfile.NormalizeFramerate}\"[/]");
                AnsiConsole.MarkupLine($"  [red]    - deinterlace_video:\t\"{ffmpegProfile.DeinterlaceVideo}\"[/]");
                AnsiConsole.MarkupLine("  [red]  }[/]");
            }

            AnsiConsole.MarkupLine("");
        }
    }
}
