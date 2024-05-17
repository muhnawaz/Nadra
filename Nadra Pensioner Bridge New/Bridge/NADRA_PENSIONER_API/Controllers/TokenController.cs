using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using NADRA_PENSIONER_API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NADRA_PENSIONER_API;

namespace NADRA_PENSIONER_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly BusinessLogic _businessLogic;
        private readonly AppSettings _appSettings;
        private readonly string _id;
        private readonly string _psw;

        public TokenController(IOptions<AppSettings> appSettings, BusinessLogic businessLogic)
        {
            _appSettings = appSettings.Value;
            _id = _appSettings.ID;
            _psw = _appSettings.PSW;
            _businessLogic = businessLogic;
        }


        [HttpPost]
        public JsonResult getToken([FromBody] UserRequest requestBody)
        {
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"{"JSON\\Setup.json"}");
            var JSONSetup = System.IO.File.ReadAllText(folderDetails);
            //SOURCE jsonObj = JsonConvert.DeserializeObject(JSON);
            JSONSetupCredentials setupCredentials = JsonConvert.DeserializeObject<JSONSetupCredentials>(JSONSetup);


            //primaryAccNo = "194282733310008101761" , bitmap = "F23A800100E080100000000006000000"

            // 13 is due to the fixed length of account in ISO 
            if (_businessLogic.UserAuthentication(setupCredentials.SOURCE, requestBody.ID, requestBody.Password))
            {
              //  if (userDetails.ID.Equals(_id) && userDetails.Password.Equals(_psw))
           // {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescrirptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, "1"),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials
                                             (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescrirptor);
                var jwt_token = tokenHandler.WriteToken(token);

                return new JsonResult(new { message = "Successful", accesssToken = jwt_token });
            }
            else
            {
                return new JsonResult(new { message = "ID or Password is invalid", accesssToken = "" });
            }
        }
    }
}