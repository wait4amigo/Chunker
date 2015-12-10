using System;
using System.Text;
using System.Web.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Chunker.DataAccess;
using Chunker.Model;
using Chunker.Utils;
using Chunker.Filters;

namespace Chunker.Controllers
{
    public class UserController : ApiController
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        public IHttpActionResult Post()
        {
            try
            {
                string body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                UserEnt ent = JsonConvert.DeserializeObject<UserEnt>(body);

                string errorInfo = string.Empty;

                if ((new User()).RegisterUser(ent, ref errorInfo))
                    return Ok("True");

                return BadRequest(errorInfo);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        [AuthenticationFilter]
        public IHttpActionResult Get()
        {
            _log.Info("User logged in");

            return Ok("True");
            /*try
            {
                string[] parts = UTF8Encoding.UTF8.GetString(Convert.FromBase64String(Request.Headers.Authorization.Parameter)).Split(':');
                if (parts.Length != 2)
                    return BadRequest("No Authorization Info");

                string errorInfo = string.Empty;
                if ((new User()).Login(parts[0], parts[1], ref errorInfo))
                    return Ok("True");

                if (errorInfo == "User not exist")
                    return NotFound();

                return BadRequest(errorInfo);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }*/
        }

        [AuthenticationFilter]
        public IHttpActionResult Put(string userGuid)
        {
            try
            {
                User user = new User();

                string errorInfo = string.Empty;
                UserEnt ent = user.GetUserByGuid(userGuid, ref errorInfo);
                if (ent == null)
                {
                    if (errorInfo.Equals("User not exist"))
                        return NotFound();

                    return BadRequest(errorInfo);
                }

                string code = Util.GenerateCheckCode(Util.VALIDATE_CODE_TYPE.CHARACTER_NUMBER, 6);

                string mailBody = string.Format("Your verification code is: {0} \r\n, it will expire in 10 minutes", code);

                if (!Mail.SendMail(ent.Email, "Your Verification Code", mailBody))
                    return BadRequest("Failed to send verification code email");

                if (!user.WriteVerificationCode(ent.UserGuid, code))
                    return BadRequest("Failed to save verification code");

                return Ok("True");
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }

        [AuthenticationFilter]
        public IHttpActionResult Put(string userGuid, string verifyCode)
        {
            try
            {
                string body = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
                UpdatePasswordEnt ent = JsonConvert.DeserializeObject<UpdatePasswordEnt>(body);

                string errorInfo = string.Empty;
                if ((new User()).UpdatePassword(userGuid, ent, verifyCode, ref errorInfo))
                    return Ok("True");

                if (errorInfo == "User not exist")
                    return NotFound();

                return BadRequest(errorInfo);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
                return BadRequest(ex.ToString());
            }
        }
    }
}
