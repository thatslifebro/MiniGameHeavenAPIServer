﻿namespace APIServer.Repository;

public enum UserState
{
    Default = 0,
}

public class RediskeyExpireTime
{
    public const ushort NxKeyExpireSecond = 3;
    public const ushort RegistKeyExpireSecond = 6000;
    public const ushort LoginKeyExpireMin = 60;
    public const ushort TicketKeyExpireSecond = 6000; // 현재 테스트를 위해 티켓은 10분동안 삭제하지 않는다. 
}
