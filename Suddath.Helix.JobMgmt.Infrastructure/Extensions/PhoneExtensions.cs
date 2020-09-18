namespace Suddath.Helix.JobMgmt.Infrastructure.Extensions
{
    public static class PhoneExtensions
    {
        public static string ToUnformatted(string formattedPhoneNumber)
        {
            var result = formattedPhoneNumber
                .Replace("+", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty);

            return result;
        }
    }
}