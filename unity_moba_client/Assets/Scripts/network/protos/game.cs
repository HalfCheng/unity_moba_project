//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: game.proto
namespace gprotocol
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GuestLoginReq")]
  public partial class GuestLoginReq : global::ProtoBuf.IExtensible
  {
    public GuestLoginReq() {}
    
    private string _guest_key;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"guest_key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string guest_key
    {
      get { return _guest_key; }
      set { _guest_key = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserCenterInfo")]
  public partial class UserCenterInfo : global::ProtoBuf.IExtensible
  {
    public UserCenterInfo() {}
    
    private string _unick;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"unick", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string unick
    {
      get { return _unick; }
      set { _unick = value; }
    }
    private int _uface;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uface", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uface
    {
      get { return _uface; }
      set { _uface = value; }
    }
    private int _usex;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"usex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int usex
    {
      get { return _usex; }
      set { _usex = value; }
    }
    private int _uvip;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"uvip", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uvip
    {
      get { return _uvip; }
      set { _uvip = value; }
    }
    private int _uid;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GuestLoginRes")]
  public partial class GuestLoginRes : global::ProtoBuf.IExtensible
  {
    public GuestLoginRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private UserCenterInfo _uinfo = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"uinfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public UserCenterInfo uinfo
    {
      get { return _uinfo; }
      set { _uinfo = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EditProfileReq")]
  public partial class EditProfileReq : global::ProtoBuf.IExtensible
  {
    public EditProfileReq() {}
    
    private string _unick;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"unick", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string unick
    {
      get { return _unick; }
      set { _unick = value; }
    }
    private int _uface;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uface", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uface
    {
      get { return _uface; }
      set { _uface = value; }
    }
    private int _usex;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"usex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int usex
    {
      get { return _usex; }
      set { _usex = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EditProfileRes")]
  public partial class EditProfileRes : global::ProtoBuf.IExtensible
  {
    public EditProfileRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AccountUpgradeReq")]
  public partial class AccountUpgradeReq : global::ProtoBuf.IExtensible
  {
    public AccountUpgradeReq() {}
    
    private string _uname;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"uname", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string uname
    {
      get { return _uname; }
      set { _uname = value; }
    }
    private string _upwd_md5;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"upwd_md5", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string upwd_md5
    {
      get { return _upwd_md5; }
      set { _upwd_md5 = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AccountUpgradeRes")]
  public partial class AccountUpgradeRes : global::ProtoBuf.IExtensible
  {
    public AccountUpgradeRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UnameLoginReq")]
  public partial class UnameLoginReq : global::ProtoBuf.IExtensible
  {
    public UnameLoginReq() {}
    
    private string _uname;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"uname", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string uname
    {
      get { return _uname; }
      set { _uname = value; }
    }
    private string _upwd_md5;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"upwd_md5", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string upwd_md5
    {
      get { return _upwd_md5; }
      set { _upwd_md5 = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UnameLoginRes")]
  public partial class UnameLoginRes : global::ProtoBuf.IExtensible
  {
    public UnameLoginRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private UserCenterInfo _uinfo = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"uinfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public UserCenterInfo uinfo
    {
      get { return _uinfo; }
      set { _uinfo = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LoginOutRes")]
  public partial class LoginOutRes : global::ProtoBuf.IExtensible
  {
    public LoginOutRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"UserGameInfo")]
  public partial class UserGameInfo : global::ProtoBuf.IExtensible
  {
    public UserGameInfo() {}
    
    private int _uchip;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"uchip", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uchip
    {
      get { return _uchip; }
      set { _uchip = value; }
    }
    private int _uexp;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uexp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uexp
    {
      get { return _uexp; }
      set { _uexp = value; }
    }
    private int _uvip;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"uvip", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uvip
    {
      get { return _uvip; }
      set { _uvip = value; }
    }
    private int _uchip2;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"uchip2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uchip2
    {
      get { return _uchip2; }
      set { _uchip2 = value; }
    }
    private int _uchip3;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"uchip3", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uchip3
    {
      get { return _uchip3; }
      set { _uchip3 = value; }
    }
    private int _udata1;
    [global::ProtoBuf.ProtoMember(6, IsRequired = true, Name=@"udata1", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int udata1
    {
      get { return _udata1; }
      set { _udata1 = value; }
    }
    private int _udata2;
    [global::ProtoBuf.ProtoMember(7, IsRequired = true, Name=@"udata2", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int udata2
    {
      get { return _udata2; }
      set { _udata2 = value; }
    }
    private int _udata3;
    [global::ProtoBuf.ProtoMember(8, IsRequired = true, Name=@"udata3", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int udata3
    {
      get { return _udata3; }
      set { _udata3 = value; }
    }
    private int _bonues_status;
    [global::ProtoBuf.ProtoMember(9, IsRequired = true, Name=@"bonues_status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int bonues_status
    {
      get { return _bonues_status; }
      set { _bonues_status = value; }
    }
    private int _bonues;
    [global::ProtoBuf.ProtoMember(10, IsRequired = true, Name=@"bonues", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int bonues
    {
      get { return _bonues; }
      set { _bonues = value; }
    }
    private int _days;
    [global::ProtoBuf.ProtoMember(11, IsRequired = true, Name=@"days", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int days
    {
      get { return _days; }
      set { _days = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetUgameInfoRes")]
  public partial class GetUgameInfoRes : global::ProtoBuf.IExtensible
  {
    public GetUgameInfoRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private UserGameInfo _uinfo = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"uinfo", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public UserGameInfo uinfo
    {
      get { return _uinfo; }
      set { _uinfo = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"RecvLoginBonuesRes")]
  public partial class RecvLoginBonuesRes : global::ProtoBuf.IExtensible
  {
    public RecvLoginBonuesRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"WorldUChipRankInfo")]
  public partial class WorldUChipRankInfo : global::ProtoBuf.IExtensible
  {
    public WorldUChipRankInfo() {}
    
    private string _unick;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"unick", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string unick
    {
      get { return _unick; }
      set { _unick = value; }
    }
    private int _uface;
    [global::ProtoBuf.ProtoMember(2, IsRequired = true, Name=@"uface", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uface
    {
      get { return _uface; }
      set { _uface = value; }
    }
    private int _usex;
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"usex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int usex
    {
      get { return _usex; }
      set { _usex = value; }
    }
    private int _uvip;
    [global::ProtoBuf.ProtoMember(4, IsRequired = true, Name=@"uvip", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uvip
    {
      get { return _uvip; }
      set { _uvip = value; }
    }
    private int _uchip;
    [global::ProtoBuf.ProtoMember(5, IsRequired = true, Name=@"uchip", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int uchip
    {
      get { return _uchip; }
      set { _uchip = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetWorldRankUchipRes")]
  public partial class GetWorldRankUchipRes : global::ProtoBuf.IExtensible
  {
    public GetWorldRankUchipRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private readonly global::System.Collections.Generic.List<WorldUChipRankInfo> _rank_info = new global::System.Collections.Generic.List<WorldUChipRankInfo>();
    [global::ProtoBuf.ProtoMember(2, Name=@"rank_info", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<WorldUChipRankInfo> rank_info
    {
      get { return _rank_info; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetSysMsgReq")]
  public partial class GetSysMsgReq : global::ProtoBuf.IExtensible
  {
    public GetSysMsgReq() {}
    
    private int _ver_num;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"ver_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ver_num
    {
      get { return _ver_num; }
      set { _ver_num = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GetSysMsgRes")]
  public partial class GetSysMsgRes : global::ProtoBuf.IExtensible
  {
    public GetSysMsgRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private int _ver_num = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ver_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int ver_num
    {
      get { return _ver_num; }
      set { _ver_num = value; }
    }
    private readonly global::System.Collections.Generic.List<string> _sys_msgs = new global::System.Collections.Generic.List<string>();
    [global::ProtoBuf.ProtoMember(3, Name=@"sys_msgs", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<string> sys_msgs
    {
      get { return _sys_msgs; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"LoginLogicRes")]
  public partial class LoginLogicRes : global::ProtoBuf.IExtensible
  {
    public LoginLogicRes() {}
    
    private int _status;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"status", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int status
    {
      get { return _status; }
      set { _status = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"Stype")]
    public enum Stype
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"INVALID_STYPE", Value=0)]
      INVALID_STYPE = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Auth", Value=1)]
      Auth = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"System", Value=2)]
      System = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Logic", Value=3)]
      Logic = 3
    }
  
    [global::ProtoBuf.ProtoContract(Name=@"Cmd")]
    public enum Cmd
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"INVALID_CMD", Value=0)]
      INVALID_CMD = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGuestLoginReq", Value=1)]
      eGuestLoginReq = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGuestLoginRes", Value=2)]
      eGuestLoginRes = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eRelogin", Value=3)]
      eRelogin = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eUserLostConn", Value=4)]
      eUserLostConn = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eEditProfileReq", Value=5)]
      eEditProfileReq = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eEditProfileRes", Value=6)]
      eEditProfileRes = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eAccountUpgradeReq", Value=7)]
      eAccountUpgradeReq = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eAccountUpgradeRes", Value=8)]
      eAccountUpgradeRes = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eUnameLoginReq", Value=9)]
      eUnameLoginReq = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eUnameLoginRes", Value=10)]
      eUnameLoginRes = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eLoginOutReq", Value=11)]
      eLoginOutReq = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eLoginOutRes", Value=12)]
      eLoginOutRes = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetUgameInfoReq", Value=13)]
      eGetUgameInfoReq = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetUgameInfoRes", Value=14)]
      eGetUgameInfoRes = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eRecvLoginBonuesReq", Value=15)]
      eRecvLoginBonuesReq = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eRecvLoginBonuesRes", Value=16)]
      eRecvLoginBonuesRes = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetWorldRankUchipReq", Value=17)]
      eGetWorldRankUchipReq = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetWorldRankUchipRes", Value=18)]
      eGetWorldRankUchipRes = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetSysMsgReq", Value=19)]
      eGetSysMsgReq = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eGetSysMsgRes", Value=20)]
      eGetSysMsgRes = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eLoginLogicReq", Value=21)]
      eLoginLogicReq = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"eLoginLogicRes", Value=22)]
      eLoginLogicRes = 22
    }
  
}