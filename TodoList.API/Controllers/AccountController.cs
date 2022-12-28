using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using ToDoListApi.Dtos;

namespace ToDoListApi.Controllers
{
    [Route("api/auth")]
    public class AccountController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ApplicationContext _context;

        public AccountController(IConfiguration configuration, ApplicationContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("token")]
        public IActionResult Token(LoginDto login)
        {
            var identity = GetIdentity(login);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            return Json(GetToken(identity));
        }

        [HttpPost("registration")]
        public IActionResult Registration(LoginDto login)
        {
            if(_context.Users.Where(user => user.Name == login.NickName).Count() != 0)
            {
                return BadRequest("User already exist");             
            }

            _context.Users.Add(new User() { Name = login.NickName, Password = HashPassword(login.Password) });
            _context.SaveChanges();

            var identity = GetIdentity(login);

            return Json(GetToken(identity));
        }

        private ClaimsIdentity GetIdentity(LoginDto login)
        {
            try
            {
                var user = _context.Users.Where(u => u.Name == login.NickName).First();
                bool verify = VerifyPassword(user.Password, login.Password);

                if (user == null || !verify)
                {
                    return null;
                }

                var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim("id", user.Id.ToString())
            };

                ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultIssuer);
                return claimsIdentity;
            }
            catch(Exception ex)
            {
                return null;
            }         
        }

        private string HashPassword(string password, byte[] salt = null)
        {
            if (salt == null || salt.Length != 16)
            {
                // generate a 128-bit salt using a secure PRNG
                salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
           
            return $"{hashed}:{Convert.ToBase64String(salt)}";
        }

        private bool VerifyPassword(string hashedPasswordWithSalt, string passwordToCheck)
        {
            // retrieve both salt and password from 'hashedPasswordWithSalt'
            var passwordAndHash = hashedPasswordWithSalt.Split(':');
            if (passwordAndHash == null || passwordAndHash.Length != 2)
                return false;
            var salt = Convert.FromBase64String(passwordAndHash[1]);
            if (salt == null)
                return false;
            // hash the given password
            var hashOfpasswordToCheck = HashPassword(passwordToCheck, salt);
            // compare both hashes
            if (String.Compare(passwordAndHash[0], hashOfpasswordToCheck.Split(':')[0]) == 0)
            {
                return true;
            }
            return false;
        }

        private object GetToken(ClaimsIdentity identity)
        {          
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: _configuration["JWT:ISSUER"],
                    audience: _configuration["JWT:AUDIENCE"],
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromDays(Int32.Parse(_configuration["JWT:LIFETIME"]))),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:KEY"])), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return response;
        }
    }
}
