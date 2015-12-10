using System;
using System.Data;
using Chunker.Model;
using Chunker.Utils;
using System.Globalization;

namespace Chunker.DataAccess
{
    public class User : DBBase
    {
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("Chunker");

        public User()
        {
        }

        public bool RegisterUser(UserEnt ent, ref string errorInfo)
        {
            _log.Info("Request to create user " + ent.Email);

            string sql = "select count(*) from user where email='" + ent.Email + "'";
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query user's data");

            int cnt = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
            if (cnt != 0)
            {
                errorInfo = "User already registered";
                return false;
            }

            string curTime = DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ");
            sql = string.Format("INSERT INTO user (username, email, user_guid, create_time, update_time, password) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')"
                , ent.Username, ent.Email, Util.GetMD5(ent.Email), curTime, curTime, ent.Password);
            cnt = ExecuteSql(sql);
            if (cnt == 0)
                throw new Exception("Failed to update database");

            _log.Info("User " + ent.Email + " created successfully");

            return true;
        }

        public bool UpdatePassword(string userGuid, UpdatePasswordEnt ent, string verifyCode, ref string errorInfo)
        {
            _log.Info("Request to update password for user " + userGuid);

            string existCode = GetVerificationCode(userGuid);
            if (existCode.Equals(string.Empty))
            {
                errorInfo = "No security code or code expired";
                return false;
            }

            if (!verifyCode.Equals(existCode))
            {
                errorInfo = "Security code incorrect";
                return false;
            }

            UserEnt userEnt = GetUserByGuid(userGuid, ref errorInfo);
            if (userEnt == null)
                return false;

            if (string.Compare(userEnt.Password, ent.OldPassword) != 0)
            {
                _log.Warn("User " + userGuid + "'s old password not correct while updating password");

                errorInfo = "Old password incorrect";
                return false;
            }

            string sql = string.Format("UPDATE user SET password='{0}' and update_time='{1}' WHERE user_guid='{2}'"
                , ent.NewPassword, DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"), userGuid);
            int cnt = ExecuteSql(sql);
            if (cnt == 0)
                throw new Exception("Failed to update database");

            _log.Info("User " + userGuid + "'s password updated successfully");

            return true;
        }

        private string GetVerificationCode(string userGuid)
        {
            string sql = "SELECT code, update_time FROM account_verification_code WHERE user_guid='" + userGuid + "'";
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query verification code");

            if (ds.Tables[0].Rows.Count == 0)
                return string.Empty;

            string code = ds.Tables[0].Rows[0]["code"].ToString();
            DateTime dt = DateTime.Parse(ds.Tables[0].Rows[0]["update_time"].ToString());
            if ((DateTime.UtcNow - dt).TotalMinutes > 10)
                return string.Empty;

            return code;
        }

        private bool ExistVerificationCode(string userGuid)
        {
            string sql = "SELECT count(*) FROM account_verification_code WHERE user_guid='" + userGuid + "'";
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to check verification code");

            if (ds.Tables[0].Rows.Count == 0)
                return false;

            return int.Parse(ds.Tables[0].Rows[0][0].ToString()) > 0;
        }

        public bool WriteVerificationCode(string userGuid, string verifyCode)
        {
            string sql = string.Empty;
            if (ExistVerificationCode(userGuid))
                sql = string.Format("UPDATE account_verification_code SET code='{0}' and update_time='{1}' WHERE user_guid='{2}' "
                    , verifyCode, DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"), userGuid);
            else
                sql = string.Format("INSERT INTO account_verification_code (user_guid, code, update_time) VALUES ('{0}', '{1}', '{2}')"
                    , userGuid, verifyCode, DateTime.UtcNow.ToString("yyyy-MM-ddThh:mm:ssZ"));

            int cnt = ExecuteSql(sql);
            if (cnt == 0)
                throw new Exception("Failed to insert/update database");

            return true;
        }

        public bool Login(string userGuid, string password, ref string errorInfo)
        {
            _log.Info("User " + userGuid + " logs in");

            UserEnt ent = GetUserByGuid(userGuid, ref errorInfo);
            if (ent == null)
                return false;


            if (string.Compare(ent.Password, password) != 0)
            {
                _log.Warn("User " + userGuid + "'s password not correct while logging in");

                errorInfo = "Password incorrect";
                return false;
            }

            _log.Info("User " + userGuid + " logged in");

            return true;
        }

        public UserEnt GetUserByEmail(string email, ref string errorInfo)
        {
            string sql = "SELECT username, user_guid, email, password, create_time, update_time FROM user WHERE email='" + email + "'";
            return GetUser(sql, ref errorInfo);
        }

        public UserEnt GetUserByGuid(string userGuid, ref string errorInfo)
        {
            string sql = "SELECT username, user_guid, email, password, create_time, update_time FROM user WHERE user_guid='" + userGuid + "'";
            return GetUser(sql, ref errorInfo);
        }

        public UserEnt GetUser(string sql, ref string errorInfo)
        {
            DataSet ds = Query(sql);
            if (ds == null)
                throw new Exception("Failed to query user's data");

            if (ds.Tables[0].Rows.Count == 0)
            {
                errorInfo = "User not exist";
                return null;
            }

            UserEnt ent = new UserEnt();
            ent.Username = ds.Tables[0].Rows[0]["username"].ToString();
            ent.UserGuid = ds.Tables[0].Rows[0]["user_guid"].ToString();
            ent.Email = ds.Tables[0].Rows[0]["email"].ToString();
            ent.Password = ds.Tables[0].Rows[0]["password"].ToString();
            ent.UpdateTime = ds.Tables[0].Rows[0]["create_time"].ToString();
            ent.CreateTime = ds.Tables[0].Rows[0]["update_time"].ToString();

            return ent;
        }
    }
}