namespace Movies.Api.Helpers
{
    public class JWTHelper
    {
        public string Issuer { get; set; }
        public string Audince { get; set; }
        public string Key { get; set; }

        public int DurationInMinutes {  get; set; }
    }
}
