using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrandChase.IO.Packet;
using System.Net.Sockets;
using Common;
using GrandChase.IO;
using System.Net;
using GrandChase.Security;
using GrandChase.Function;
using Manager.Factories;

namespace GrandChase.Net.Client
{
    public class ClientSession : Session
    {
        // Security START
        public byte[] CRYPT_KEY { get; set; }
        public byte[] CRYPT_HMAC { get; set; }
        public byte[] CRYPT_PREFIX = new byte[2];
        public int CRYPT_COUNT;
        // Security END

        public Loading MyLoading = new Loading();
        public User MyUser = new User();
        public GCCommon MyCommon = new GCCommon();
        public Inventory MyInventory = new Inventory();
        public Character MyCharacter = new Character();
        public Pet MyPet = new Pet();
        public CharsInfo InfoChars = new CharsInfo();
        public Shop MyShop = new Shop();

        public int LoginUID;
        public string Login;
        public string Nick;
        public int GamePoint;
        public int VirtualCash;
        public int CurrentChar;
        public byte TutorialCheck =0;
        public int CharSlot = 0;
        public ArvoreDeTalentos Skilltree = new ArvoreDeTalentos();
        public Channel CurrentChannel { get; set; }
        public Room CurrentRoom { get; set; }

        // Option
        public bool InviteDeny;

        public int LastHeartBeat { get; set; }
        public uint IP { get; set; }
        public ushort Port { get; set; }

        public ClientSession(Socket pSocket) : base(pSocket)
        {
            IP = BitConverter.ToUInt32(IPAddress.Parse(GetIP()).GetAddressBytes(), 0);

            InitiateReceive(2, true);

            CurrentChannel = null;
            CurrentRoom = null;

            CRYPT_KEY = CryptoGenerators.GenerateKey();
            CRYPT_HMAC = CryptoGenerators.GenerateKey();
            byte[] TEMP_PREFIX = CryptoGenerators.GeneratePrefix(); // Prefix 

            LogFactory.GetLog("Main").LogHex("IV: ",CRYPT_KEY);
            LogFactory.GetLog("Main").LogHex("HMAC: ", CRYPT_HMAC);

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ACCEPT_CONNECTION_NOT))
            {
                oPacket.WriteBytes(TEMP_PREFIX);
                oPacket.WriteInt((int)8);
                oPacket.WriteBytes(CRYPT_HMAC);
                oPacket.WriteInt((int)8);
                oPacket.WriteBytes(CRYPT_KEY);
                oPacket.WriteHexString("00 00 00 01 00 00 00 00 00 00 00 00");

                oPacket.Assemble(CryptoConstants.GC_DES_KEY, CryptoConstants.GC_HMAC_KEY, CRYPT_PREFIX, ++CRYPT_COUNT);
                Send(oPacket);
            }

            // Prefix
            CRYPT_PREFIX = TEMP_PREFIX;
        }

        public string GetIP()
        {
            if( _socket == null ) return "0.0.0.0";

            IPEndPoint remoteIpEndPoint = _socket.RemoteEndPoint as IPEndPoint;
            return ( remoteIpEndPoint.Address.ToString() );
        }

        public override void OnPacket( InPacket iPacket )
        {
            try
            {
                iPacket.Decrypt(CRYPT_KEY);

                GameOpcodes uOpcode = (GameOpcodes)iPacket.ReadUShort();
                int uSize = iPacket.ReadInt(); 
                bool isCompress = iPacket.ReadBool();
                int cSize = 0;
                if (isCompress == true)
                {
                    cSize = iPacket.ReadInt();
                    LogFactory.GetLog("Main").LogWarning("[{0}] pacote comprimido chegada {1} ({2})", Login, (int)uOpcode, uOpcode.ToString());
                } else
                {
                    LogFactory.GetLog("Main").LogInfo("[{0}] de chegada de pacotes {1} ({2})", Login, (int)uOpcode, uOpcode.ToString());
                }

                LogFactory.GetLog("Main").LogHex("Pacote", iPacket.ToArray());

                switch ( uOpcode )
                {
                    case GameOpcodes.EVENT_HEART_BIT_NOT: // 0
                        OnHeartBeatNot();
                        break;
                    case GameOpcodes.EVENT_CHANGE_CHARACTER_INFO_REQ: //1618
                        InfoChars.ChangeCharInfo(this);
                        break;
                    case GameOpcodes.EVENT_VERIFY_ACCOUNT_REQ: // 2
                        MyUser.OnLogin(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_SET_CURRENT_CHARACTER_REQ:
                        MyCommon.SetCurretChar(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_GET_FULL_SP_INFO_REQ: // 423 0x1A7
                        MyCommon.OnGetFullSPInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_PET_COSTUM_LIST_REQ: // 517 0x205
                        MyCommon.OnPetCostumList(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_INVEN_BUFF_ITEM_LIST_REQ: // 1226 0x04CA
                        MyCommon.OnInvenBuffItemList(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_DEPOT_INFO_REQ: // 1340 0x053C
                        MyCommon.OnDepotInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_STAT_CLIENT_INFO: // 226 0x00E2
                        MyCommon.OnStatClientInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_COST_RATE_FOR_GAMBLE_BUY_REQ: // 871 0x0367
                        MyCommon.OnCostRateForGambleBuy(this);
                        break;
                    case GameOpcodes.EVENT_REGISTER_NICKNAME_REQ: // 134 0x0086
                        MyUser.OnRegisterNick(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_SET_IDLE_STATE_REQ: // 835 0x0343
                        MyCommon.OnSetIDLE(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHAR_SELECT_JOIN_REQ: // 1557 0x0613
                        MyUser.OnCharSelectJoin(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHOICE_BOX_LIST_REQ: // 1012 0x03F4
                        MyCommon.OnChoiceBoxList(this);
                        break;
                    case GameOpcodes.EVENT_EXP_POTION_LIST_REQ: // 1338 0x053A
                        MyCommon.OnExpPotionList(this);
                        break;
                    case GameOpcodes.EVENT_AGIT_STORE_CATALOG_REQ: // 1114 0x054A
                        MyCommon.OnAgitStoreCatalog(this);
                        break;
                    case GameOpcodes.EVENT_AGIT_STORE_MATERIAL_REQ: // 1116 0x045C
                        MyCommon.OnAgitStoreMaterial(this);
                        break;
                    case GameOpcodes.EVENT_AGIT_MAP_CATALOGUE_REQ: // 1106 0x0452
                        MyCommon.OnAgitMapCatalogue(this);
                        break;
                    case GameOpcodes.EVENT_FAIRY_TREE_LV_TABLE_REQ: // 1184 0x04A0
                        MyCommon.OnFaityTreeLvTable(this);
                        break;
                    case GameOpcodes.EVENT_INVITE_DENY_NOT: // 348 0x015C
                        InviteDeny = iPacket.ReadBool();
                        break;
                    case GameOpcodes.EVENT_GET_USER_DONATION_INFO_REQ: // 523 0x020B
                        MyCommon.OnGetUserDonationInfo(this);
                        break;
                    case GameOpcodes.EVENT_RECOMMEND_FULL_INFO_REQ: // 567 0x0237
                        MyCommon.OnRecommentUser(this);
                        break;
                    case GameOpcodes.EVENT_USER_BINGO_DATA_REQ: // 654 0x28E
                        MyCommon.OnUserBingoData(this);
                        break;
                    case GameOpcodes.EVENT_CHANNEL_LIST_REQ: // 14 0x0E
                        MyCommon.OnChannelList(this);
                        break;
                    case GameOpcodes.EVENT_DONATION_INFO_REQ: // 525 0x020D
                        MyCommon.OnDonationInfo(this);
                        break;
                    case GameOpcodes.EVENT_ENTER_CHANNEL_REQ: // 12 0x0C
                        MyUser.OnEnterChannel(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_LEAVE_CHANNEL_NOT: // 26
                        MyUser.OnLeaveChannel(this);
                        break;
                    case GameOpcodes.EVENT_CREATE_ROOM_REQ: 
                        MyUser.OnCreateRoom(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHANGE_ROOM_INFO_REQ: 
                        CurrentRoom.OnChangeRoomInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHANGE_ROOMUSER_INFO_REQ: 
                        CurrentRoom.OnChangeUserInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_LEAVE_ROOM_REQ: 
                        CurrentRoom.OnLeaveRoom(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_ROOM_LIST_REQ: 
                        CurrentChannel.OnRoomList(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_JOIN_ROOM_REQ: 
                        CurrentChannel.OnJoinRoom(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_START_GAME_REQ:
                        CurrentRoom.StartGame(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_RELAY_LOADING_STATE: 
                        CurrentRoom.OnLoadState(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_LOAD_COMPLETE_NOT: 
                        //MyUser.LoadComplete(this, iPacket);
                        CurrentRoom.OnLoadComplete(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_STAGE_LOAD_COMPLETE_NOT: 
                        //MyUser.StageLoadComplete(this, iPacket);
                        CurrentRoom.OnStageLoadComplete(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_ROOM_MEMBER_PING_INFO_REQ: 
                        CurrentRoom.OnPingInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_GET_ROOMUSER_IDLE_STATE_REQ: 
                        CurrentRoom.OnIdleInfo(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_END_GAME_REQ: 
                        CurrentRoom.OnGameEnd(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHAT_REQ:
                        CurrentChannel.OnChat(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_LEAVE_GAME_REQ:
                        CurrentRoom.OnLeaveGame(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_EQUIP_ITEM_REQ: 
                        MyCharacter.OnEquipItem(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_CHANGE_LOOK_EQUIP_REQ:
                        MyCharacter.OnChangeEquipInRoom(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_SET_PRESS_STATE_REQ:
                        CurrentRoom.OnSetPressState(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_SKILL_TRAINING_REQ:
                        MyCharacter.OnTrainSkill(this, iPacket);
                        break;
                    case GameOpcodes.EVENT_SET_SKILL_REQ:
                        MyCharacter.OnSetSkill(this, iPacket);
                        break;
                    case GameOpcodes.DB_EVENT_SYSTEM_GUIDE_COMPLETE_INFO_REQ:
                        MyCommon.SendGuideCompleteInfo(this);
                        break;
                    case GameOpcodes.EVENT_CASHBACK_EXTRA_RATIO_INFO_REQ:
                        MyShop.CashRatio(this);
                        break;
                    case GameOpcodes.EVENT_PACKAGE_INFO_REQ:
                        MyShop.packageInfo(this);
                        break;
                    case GameOpcodes.EVENT_PACKAGE_INFO_DETAIL_REQ:
                        MyShop.packageInfoDetail(this,iPacket);
                        break;

                    default:
                        {
                            LogFactory.GetLog("Main").LogWarning("pacote indefinido foi recebido. Opcode: {0}({1})", (int)uOpcode, uOpcode.ToString() );
                            LogFactory.GetLog("Main").LogHex("Pacote", iPacket.ToArray());
                            break;
                        }
                }
            }
            catch( Exception e )
            {
                LogFactory.GetLog("Main").LogFatal(e);
                Close();
            }
        }

        public override void OnDisconnect()
        {
            LogFactory.GetLog("Main").LogInfo("A conexão de Socket foi perdida. ID: {0}", Login);

            if( CurrentRoom != null )
                CurrentRoom.ProcessLeaveRoom(this);

            if (CurrentChannel != null)
                MyUser.OnLeaveChannel(this);


            TSingleton<ClientHolder>.Instance.DestoryAccount(this);
        }

        public void OnHeartBeatNot()
        {
            LastHeartBeat = Environment.TickCount;
        }
    }
}
