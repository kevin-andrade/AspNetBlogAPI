namespace Blog.Utils
{
    public class SlugUtils
    {
        public static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.ToLower()
                        .Trim()
                        .Replace(" ", "-")
                        .Replace("ã", "a")
                        .Replace("é", "e")
                        .Replace("ç", "c");
        }
    }
}
