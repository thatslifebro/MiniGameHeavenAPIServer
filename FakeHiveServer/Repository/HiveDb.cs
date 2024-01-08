using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using APIServer.Services;
using System.Reflection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using ZLogger;
using APIServer.Model.DAO;

namespace APIServer.Repository
{
    public class HiveDb : IHiveDb
    {
        readonly IOptions<DbConfig> _dbConfig;
        readonly ILogger<HiveDb> _logger;
        IDbConnection _dbConn;
        readonly SqlKata.Compilers.MySqlCompiler _compiler;
        readonly QueryFactory _queryFactory;

        public HiveDb(ILogger<HiveDb> logger, IOptions<DbConfig> dbConfig)
        {
            _logger = logger;
            _dbConfig = dbConfig;

            Open();

            _compiler = new SqlKata.Compilers.MySqlCompiler();
            _queryFactory = new QueryFactory(_dbConn, _compiler);
        }
        public void Dispose()
        {
            Close();
        }

        public async Task<ErrorCode> CreateAccountAsync(string email, string pw)
        {
            try
            {
                

                string saltValue = Security.SaltString();
                string hashingPassword = Security.MakeHashingPassWord(saltValue, pw);
                Console.WriteLine(DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"));
                int count = await _queryFactory.Query("account").InsertAsync(new HdbAccountInsert
                {
                    player_id = null,
                    email = email,
                    salt_value = saltValue,
                    hashed_pw = hashingPassword,
                    create_dt = DateTime.Now.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss"),
                    recent_login_dt = null,
                });
                _logger.ZLogDebug(
                $"[CreateAccount] email: {email}, salt_value : {saltValue}, hashed_pw:{hashingPassword}");

                return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
            }
            catch (Exception ex)
            {
                _logger.ZLogError(
                $"[AccoutDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, Email: {email}");
                return ErrorCode.CreateAccountFailException;
            }
        }

        public async Task<Tuple<ErrorCode, Int64>> VerifyUser(string email, string pw)
        {
            try
            {
                Model.DAO.HdbAccount userInfo = await _queryFactory.Query("account")
                                        .Where("Email", email)
                                        .FirstOrDefaultAsync<Model.DAO.HdbAccount>();

                if (userInfo is null || userInfo.player_id == 0)
                {
                    return new Tuple<ErrorCode, Int64>(ErrorCode.LoginFailUserNotExist, 0);
                }

                

                string hashingPassword = Security.MakeHashingPassWord(userInfo.salt_value, pw);
                if (userInfo.hashed_pw != hashingPassword)
                {
                    return new Tuple<ErrorCode, Int64>(ErrorCode.LoginFailPwNotMatch, 0);
                }

                return new Tuple<ErrorCode, Int64>(ErrorCode.None, userInfo.player_id);
            }
            catch (Exception e)
            {
                return new Tuple<ErrorCode, Int64>(ErrorCode.LoginFailException, 0);
            }
        }

        public ErrorCode VerifyToken(Int64 player_id,string token)
        {
            if(JWT.PlayerIdFromToken(token)==player_id)
            {
                return ErrorCode.None;
            }
            return ErrorCode.VerifyTokenFail;
        }

        void Open()
        {
            _dbConn = new MySqlConnection(_dbConfig.Value.HiveDb);

            _dbConn.Open();
        }
        void Close()
        {
            _dbConn.Close();
        }
    }

    

}


public class DbConfig
{
    public string HiveDb { get; set; }

}