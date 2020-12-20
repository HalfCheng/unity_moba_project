using System.Collections.Generic;
using gprotocol;

public class user_info
{
    public string unick;
    public int usex;
    public int uface;
}

public class ugame : Singleton<ugame>
{
    public bool ISTEST = true;

    public string unick = "";
    public int uface = 1;
    public int usex = 0;
    public int uvip = 0;
    public bool is_guest = false;
    public string guest_key = null;
    public int zid = -1;
    public int matchid = -1;
    public int self_seatid = -1; //自己座位
    public int self_side = -1; //自己属于哪个阵营

    public UserGameInfo ugame_info;

    public List<UserArrived> other_users = new List<UserArrived>();
    public List<PlayerMatchInfo> players_math_info = null; //比赛玩家信息

    public user_info get_user_info(int seatid)
    {
        user_info uinfo = new user_info();
        if (seatid == self_seatid)
        {
            uinfo.unick = this.unick;
            uinfo.uface = this.uface;
            uinfo.usex = this.usex;
            return uinfo;
        }

        for (int i = 0; i < this.other_users.Count; i++)
        {
            if (this.other_users[i].seatid == seatid)
            {
                uinfo.unick = this.other_users[i].unick;
                uinfo.uface = this.other_users[i].uface;
                uinfo.usex = this.other_users[i].usex;
                return uinfo;
            }
        }

        return null;
    }

    public PlayerMatchInfo get_player_match_info(int seatid)
    {
        for (int i = 0; i < this.players_math_info.Count; i++)
        {
            if (this.players_math_info[i].seatid == seatid)
            {
                return this.players_math_info[i];
            }
        }

        return null;
    }

    public void save_ugame_info(UserGameInfo ugame_info)
    {
        this.ugame_info = ugame_info;
    }

    public void save_uinfo(UserCenterInfo uinfo, bool is_guest, string guest_key = null)
    {
        this.unick = uinfo.unick;
        this.uface = uinfo.uface;
        this.usex = uinfo.usex;
        this.uvip = uinfo.uvip;
        this.is_guest = is_guest;
        this.guest_key = guest_key;
    }

    public void save_edit_profile(string unick, int uface, int usex)
    {
        this.unick = unick;
        this.uface = uface;
        this.usex = usex;
    }

    public void user_login_out()
    {
    }
}