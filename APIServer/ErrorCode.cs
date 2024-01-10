using System;

// 1000 ~ 19999
public enum ErrorCode : UInt16
{
    None = 0,

    // Common 1000 ~
    UnhandleException = 1001,
    RedisFailException = 1002,
    InValidRequestHttpBody = 1003,
    AuthTokenFailWrongAuthToken = 1006,
    Hive_Fail_InvalidResponse = 1010,

    // Account 2000 ~
    CreateAccountFailException = 2001,
    CreateAccountAlreadyExistFail = 2002,
    CreateAccountDuplicateFail = 2003,
    LoginFailException = 2004,
    LoginFailUserNotExist = 2005,
    LoginFailPwNotMatch = 2006,
    LoginFailSetAuthToken = 2007,
    AuthTokenMismatch = 2008,
    AuthTokenNotFound = 2009,
    AuthTokenFailWrongKeyword = 2010,
    AuthTokenFailSetNx = 20011,
    AccountIdMismatch = 2012,
    DuplicatedLogin = 2013,
    CreateAccountFailInsert = 2014,
    LoginFailAddRedis = 2015,
    CheckAuthFailNotExist = 2016,
    CheckAuthFailNotMatch = 2017,
    CheckAuthFailException = 2018,

    // Character 3000 ~
    CreateCharacterRollbackFail = 3001,
    CreateCharacterFailNoSlot = 3002,
    CreateCharacterFailException = 3003,
    CharacterNotExist = 3004,
    CountCharactersFail = 3005,
    DeleteCharacterFail = 3006,
    GetCharacterInfoFail = 3007,
    InvalidCharacterInfo = 3008,
    GetCharacterItemsFail = 3009,
    CharacterCountOver = 3010,
    CharacterArmorTypeMisMatch = 3011,
    CharacterHelmetTypeMisMatch = 3012,
    CharacterCloakTypeMisMatch = 3012,
    CharacterDressTypeMisMatch = 3013,
    CharacterPantsTypeMisMatch = 3012,
    CharacterMustacheTypeMisMatch = 3012,
    CharacterArmorCodeMisMatch = 3013,
    CharacterHelmetCodeMisMatch = 3014,
    CharacterCloakCodeMisMatch = 3015,
    CharacterDressCodeMisMatch = 3016,
    CharacterPantsCodeMisMatch = 3017,
    CharacterMustacheCodeMisMatch = 3018,
    CharacterHairCodeMisMatch = 3019,
    CharacterCheckCodeError = 3010,
    CharacterLookTypeError = 3011,

    CharacterStatusChangeFail = 3012,
    CharacterIsExistGame = 3013,
    GetCharacterListFail = 3014,

    //GameDb 4000~ 
    GetGameDbConnectionFail = 4002,


    // MasterDb 5000 ~
    MasterDB_Fail_LoadData = 5001,
    MasterDB_Fail_InvalidData = 5002,
}