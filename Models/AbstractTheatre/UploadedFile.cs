using Microsoft.AspNetCore.Components;

namespace AT9.Models.AbstractTheatre
{
    public class UploadedFile
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public byte[] StreamData { get; set; }
        public int Position;

        [Inject]
        protected SecurityService Security { get; set; }

        public bool HasError { get; set; } = false;
    }
}
