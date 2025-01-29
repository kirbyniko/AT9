namespace AT9.Models.AbstractTheatre
{
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class YouTubeAPI
    {
        private static string API_KEY = "AIzaSyCceNDKhII8RroPujzdP0jtdoqZcaBfzsA";

        public static string GetThumbnail(string videoid)
        {
            return $"https://img.youtube.com/vi/{videoid}/1.jpg";
        

        }
        public static string ExtractVideoId(string url)
        {
            string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|(?:watch\?v=))|youtu\.be\/)([a-zA-Z0-9_-]{11})";

            Regex regex = new Regex(pattern);

            Match match = regex.Match(url);

            return match.Success ? match.Groups[1].Value : null;
        }

        public static string EmbedYouTubeVideo(string videoId) 
        {
            return $"https://www.youtube.com/embed/{videoId}";

        }




        public static async Task<bool> IsEmbeddableYouTubeVideoAsync(string videolink)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = API_KEY
            });

            if (videolink.Contains("shorts"))
            {
                return false;
            }

            try
            {
                string videoId = ExtractVideoId(videolink)
                    ;
                var request = youtubeService.Videos.List("snippet,contentDetails,status");
                request.Id = videoId;

                var response = await request.ExecuteAsync();

                if (response.Items.Count == 0)
                {
                    return false; 
                }

                var video = response.Items[0];

                bool isEmbeddable = video.Status.Embeddable ?? true;

                return isEmbeddable;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking video: {ex.Message}");
                return false;
            }
        }
    }

}
