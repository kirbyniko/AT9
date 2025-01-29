using Xunit;
using System.Collections.Generic;
using System.Linq;
using AT9.Components.Pages;
using AT9.Models.AbstractTheatre;
using AT9;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

public class ShoppingPageTests
{

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("https://youtube.com/embed/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("http://www.youtube.com/v/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&feature=share", "dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ&t=1s", "dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/e/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/shorts/dQw4w9WgXcQ", "dQw4w9WgXcQ")]
    [InlineData("https://www.youtube.com/embed/dQw4w9WgXcQ?autoplay=1", "dQw4w9WgXcQ")]
    [InlineData("invalid_url", null)]
    [InlineData("", null)]
    public void ExtractVideoId_ReturnsExpectedId(string url, string expectedVideoId)
    {
        string result = ExtractVideoId(url);

        Assert.Equal(expectedVideoId, result);
    }

    public static string ExtractVideoId(string url)
    {
        string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|(?:watch\?v=))|youtu\.be\/)([a-zA-Z0-9_-]{11})";

        Regex regex = new Regex(pattern);

        Match match = regex.Match(url);

        return match.Success ? match.Groups[1].Value : null;
    }
}
   


