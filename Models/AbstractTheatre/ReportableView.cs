namespace AT9.Models.AbstractTheatre
{
    public class ReportableView()
    {
        public ReportableView(ImageView view) : this()
        {
            Date = view.ViewTime;
            IsVideo = false;

        }
        public ReportableView(VideoView view) : this()
        {
            Date = view.ViewTime;
            IsVideo = true;

        }

        public DateTime Date { get; set; } = new();

        public bool IsVideo { get; set; }

        public string ArtworkName { get; set; }


    }
}
