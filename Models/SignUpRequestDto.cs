using System.ComponentModel.DataAnnotations;

namespace CognitoPasswordlessSmsApi.Models
{
    public class SignUpRequestDto
    {
        [Required, Phone] 
        public string PhoneNumber { get; set; } = string.Empty; // Format: +1234567890
    }

    public class InitiateLoginDto
    {
        [Required, Phone] 
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class VerifySmsChallengeDto
    {
        [Required, Phone] 
        public string PhoneNumber { get; set; } = string.Empty;
        [Required] 
        public string Session { get; set; } = string.Empty;
        [Required] 
        public string OtpCode { get; set; } = string.Empty;
    }
}