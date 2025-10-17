using etvctl.Api;
using etvctl.Generator;
using etvctl.Models;

namespace etvctl.Planning;

[EtvPrinter("FFmpeg Profile", typeof(FFmpegProfileModel), typeof(FFmpegFullProfileResponseModel))]
public partial class FFmpegProfilePrinter : BaseComparer
{
    private static bool ShouldIncludeProperty(FFmpegProfileModel ffmpegProfile, string snakeCasePropertyName)
    {
        return snakeCasePropertyName switch
        {
            "vaapi_device" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "vaapi_display" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "vaapi_driver" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "qsv_extra_hardware_frames" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv,
            _ => true
        };
    }

    private static bool ShouldIncludeProperty(FFmpegFullProfileResponseModel ffmpegProfile, string snakeCasePropertyName)
    {
        return snakeCasePropertyName switch
        {
            "vaapi_device" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "vaapi_display" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "vaapi_driver" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv or HardwareAccelerationKind.Vaapi,
            "qsv_extra_hardware_frames" => ffmpegProfile.HardwareAcceleration is HardwareAccelerationKind.Qsv,
            _ => true
        };
    }
}
