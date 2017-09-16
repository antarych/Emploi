namespace Frontend.Models
{
    public class ProfileIsNotCompletedResponse
    {
        public ProfileIsNotCompletedResponse(string _token)
        {
            token = _token;
        }

        public string token { get; set; }
    }
}