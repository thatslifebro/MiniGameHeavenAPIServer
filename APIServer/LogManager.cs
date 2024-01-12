
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

public static class LogManager
{
    public enum EventType
    {
        CreateAccount = 101,
        Login = 102,
        LoginAddRedis = 103,
        Logout = 104,
        FriendAdd = 201,
        FriendList = 202,
        FriendReceivedReqList = 203,
        FriendSentReqList = 204,
        FriendDelete = 205,
        FriendCancelReq = 206,
        GameList = 301,
        GameUnlock = 302,
    }

    private static ILoggerFactory s_loggerFactory;

    public static Dictionary<EventType, EventId> EventIdDic = new()
    {
        { EventType.CreateAccount, new EventId((int)EventType.CreateAccount, "CreateAccount") },
        { EventType.Login, new EventId((int)EventType.Login, "Login") },
        { EventType.LoginAddRedis, new EventId((int)EventType.LoginAddRedis, "LoginAddRedis") },
        { EventType.Logout, new EventId((int)EventType.Logout, "Logout") },
        { EventType.FriendAdd, new EventId((int)EventType.FriendAdd, "FriendAdd") },
        { EventType.FriendList, new EventId((int)EventType.FriendList, "FriendList") },
        { EventType.FriendReceivedReqList, new EventId((int)EventType.FriendReceivedReqList, "FriendReceivedReqList") },
        { EventType.FriendSentReqList, new EventId((int)EventType.FriendSentReqList, "FriendSentReqList") },
        { EventType.FriendDelete, new EventId((int)EventType.FriendDelete, "FriendDelete") },
        { EventType.FriendCancelReq, new EventId((int)EventType.FriendCancelReq, "FriendCancelReq") },
        { EventType.GameList, new EventId((int)EventType.GameList, "GameList") },
        { EventType.GameUnlock, new EventId((int)EventType.GameUnlock, "GameUnlock") },
    };

    public static ILogger Logger { get; private set; }

    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        s_loggerFactory = loggerFactory;
        Logger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>() where T : class
    {
        return s_loggerFactory.CreateLogger<T>();
    }
}