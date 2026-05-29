using Microsoft.AspNetCore.Mvc;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using CognitoPasswordlessSmsApi.Models;


namespace CognitoPasswordlessSmsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly string _clientId = "YOUR_AWS_COGNITO_APP_CLIENT_ID";

        public AuthController(IAmazonCognitoIdentityProvider cognitoClient)
        {
            _cognitoClient = cognitoClient;
        }


        /// <summary>
        /// Registers a user using only their phone number without executing explicit sign-up confirmation.
        /// </summary>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto model)
        {
            try
            {
                var signUpRequest = new SignUpRequest
                {
                    ClientId = _clientId,
                    Username = model.PhoneNumber, // Phone number serves as the primary username identifier
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType { Name = "phone_number", Value = model.PhoneNumber }
                    }
                };

                var response = await _cognitoClient.SignUpAsync(signUpRequest);

                return Ok(new
                {
                    Message = "User registered successfully. You can proceed directly to login.",
                    UserSub = response.UserSub
                });
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        /// <summary>
        /// Initiates the login flow using the USER_AUTH choice-based pipeline.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitiateLogin([FromBody] InitiateLoginDto model)
        {
            try
            {
                var authRequest = new InitiateAuthRequest
                {
                    ClientId = _clientId,
                    AuthFlow = AuthFlowType.USER_AUTH, // Crucial: Leverages the choice-based engine
                    AuthParameters = new Dictionary<string, string>
                    {
                        { "USERNAME", model.PhoneNumber },
                        { "PREFERRED_CHALLENGE", "SMS_OTP" } // Directly forces Cognito to issue an SMS OTP
                    }
                };

                var response = await _cognitoClient.InitiateAuthAsync(authRequest);

                // For the passwordless SMS flow, Cognito directly returns an SMS_OTP challenge name
                return Ok(new
                {
                    ChallengeName = response.ChallengeName?.Value,
                    Session = response.Session, // Must be passed to verify-login to maintain connection state
                    Message = "An OTP code has been dispatched to your registered phone number."
                });
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Verifies the incoming SMS OTP code against the tracking session state.
        /// </summary>
        [HttpPost("verify-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyLogin([FromBody] VerifySmsChallengeDto model)
        {
            try
            {
                var challengeRequest = new RespondToAuthChallengeRequest
                {
                    ClientId = _clientId,
                    ChallengeName = ChallengeNameType.SMS_OTP,
                    Session = model.Session, // Passes forward the active tracking session token
                    ChallengeResponses = new Dictionary<string, string>
                    {
                        { "USERNAME", model.PhoneNumber },
                        { "SMS_OTP_CODE", model.OtpCode } // The numeric verification code typed by the user
                    }
                };

                var response = await _cognitoClient.RespondToAuthChallengeAsync(challengeRequest);

                // Authentication succeeded. User attributes are automatically verified here.
                return Ok(new
                {
                    Message = "Authentication verified successfully.",
                    Tokens = new
                    {
                        IdToken = response.AuthenticationResult.IdToken,
                        AccessToken = response.AuthenticationResult.AccessToken,
                        RefreshToken = response.AuthenticationResult.RefreshToken
                    }
                });
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

    }
}
