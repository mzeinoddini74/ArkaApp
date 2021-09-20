using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArkaApp.Models
{
    public enum IsEnabledEnumModel
    {
        DeActive = 0,
        Active = 1,
        All = 2
    }

    public enum UserRolesEnumModel
    {
        SuperAdmin = 1,
        Admin = 2,
        GoldUser = 3,
        SilverUser = 4,
        User = 7
    }

    public enum PriorityEnumModel
    {
        Highest = 1,
        DontCare = 2,
        Lowest = 3
    }
    public enum GenderEnumModel
    {
        DontCare = 1,
        Female = 2,
        Male = 3
    }

    public enum ResponseEnumModel
    {
        Ok = 200,
        BadRequest = 400,
        Unauthorized = 401,
    }

    public enum ActionEnumModel
    {
        Follower = 1,
        Like = 2,
        Comment = 3,
        View = 4,
        Explore = 5,
        DirectPost = 6,
        DirectContent = 7,
        TelegramPost = 8,
        TelegramUserPost = 9,
        TelegramContent = 10,
        TelegramUserContent = 11
    }

}