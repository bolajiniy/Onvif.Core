using Onvif.Core.Client.Common;

namespace Onvif.Core.Client
{
    public class CameraFocusRequest : Request
    {
        public FocusMove FocusMove { get; set; }
    }
}
