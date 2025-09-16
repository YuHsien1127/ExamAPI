namespace ExamAPI.Services
{
    public interface ITokenService
    {
        public string GenerateJwtToken(string userAccount, string role);
    }
}
