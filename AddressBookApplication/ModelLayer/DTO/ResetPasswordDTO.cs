namespace ModelLayer.DTO
{
    public class ResetPasswordDTO
    {
        public string Token {  get; set; }
        public string Email {  get; set; }
        public string NewPassword { get; set; }
    }
}
