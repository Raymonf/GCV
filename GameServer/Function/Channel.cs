using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrandChase.IO.Packet;
using GrandChase.Net;
using GrandChase.Net.Client;
using Common;
using GrandChase.IO;
using Manager.Factories;

namespace GrandChase.Function
{
    public class Channel
    {
        public string ChannelName { get; set; }
        public ushort MaxUsers { get; set; }
        public byte Level { get; set; }
        public Dictionary<string, ClientSession> UsersMap { get; set; }
        public List<ClientSession> UsersList { get; set; }
        public ushort CurrentUsers
        {
            get { return (ushort)(UsersList == null ? 0 : UsersList.Count); }
        }
        public Dictionary<ushort, Room> RoomsMap { get; set; }
        public List<Room> RoomsList { get; set; }
        public int CurrentRooms
        {
            get { return RoomsList == null ? 0 : RoomsList.Count; }
        }

        public object _lock = new object();

        public Channel()
        {
            UsersMap = new Dictionary<string, ClientSession>();
            UsersList = new List<ClientSession>();

            RoomsMap = new Dictionary<ushort, Room>();
            RoomsList = new List<Room>();
        }

        public ushort GetEmptyRoom()
        {
            lock (_lock)
            {
                for (ushort i = 0; i < ushort.MaxValue; i++)
                {
                    // max 65535 rooms

                    if (RoomsMap.ContainsKey((ushort)i)) continue;

                    return (ushort)i;
                }
            }

            return ushort.MaxValue;
        }

        public Room GetRoom(ushort roomID)
        {
            lock (_lock)
            {
                if (RoomsMap.ContainsKey(roomID)) return RoomsMap[roomID];

                return null;
            }
        }

        public void OnRoomList(ClientSession cs, InPacket ip)
        {
            byte RoomType = ip.ReadByte();
            //LogFactory.GetLog("Main").LogInfo("RoomTYPE: " + RoomType);

            // 그 뒤로
            // FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00
            // 어딘가에 페이지가 있겠다.

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ROOM_LIST_ACK))
            {
                int roomcount = 0;//default is 0
                //LogFactory.GetLog("Main").LogInfo("HUEHUE ROOMS Count: " + roomcount);
                foreach (Room room in RoomsList)
                {
                    // 대기방만 보여주는 것이라면...
                    if (RoomType == 1)
                    {
                        // 게임시작이거나, 남는자리가 없다면 생략
                        if (room.Playing == true || room.GetFreeSlot() == 0)
                            continue;
                    }
                    roomcount++;
                }
                oPacket.WriteInt(roomcount);
                for (int j = 0; j < roomcount; j++)
                {
                    /*ushort roomid = 38;//00 26
                    string nameRoom = "test";
                    string passRoom = "";//01
                    ushort fresslots = 4;
                    ushort players = 1;
                    bool playing = false;
                    string nick = "[ADM]Hiro";*/

                    //Room room = new Room();
                    /*
                    oPacket.WriteUShort(1);//00 26
                    oPacket.WriteInt("Vamos jogar!".Length * 2);//00 00 00 0C
                    oPacket.WriteUnicodeString("Vamos jogar!");//68 BE AC B9 68 BE AC B9 21 00 21 00
                    oPacket.WriteByte(1);
                    oPacket.WriteByte(0);//00
                    oPacket.WriteInt("".Length * 2);//00 00 00 00
                    oPacket.WriteUnicodeString("");
                    oPacket.WriteUShort(4);//00 04
                    oPacket.WriteUShort(1);//00 01
                    oPacket.WriteBool(false);//00
                    oPacket.WriteHexString("28 5C DA F2 02 02 00 00 00 08 32 25 A5 77 00 00 00 03 00 00 00 00 00 00 00 01");
                    oPacket.WriteInt("[ADM]SkelletonX".Length * 2);
                    oPacket.WriteUnicodeString("[ADM]SkelletonX");
                    oPacket.WriteHexString("02 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 04 5A 00 5A 00 01");
                    */
                    foreach (Room room in RoomsList)
                    {
                        ushort roomid = room.ID;//00 26
                        string nameRoom = room.RoomName;
                        string passRoom = room.RoomPass;//01
                        ushort fresslots = 4;
                        ushort players = (ushort)room.GetPlayerCount();
                        bool playing = room.Playing;
                        string nick = room.GetRoomLeaderCS().Nick;

                        //LogFactory.GetLog("Main").LogInfo("roomID: " + roomid);
                        //LogFactory.GetLog("Main").LogInfo("Name Room: " + nameRoom);
                        //LogFactory.GetLog("Main").LogInfo("Pass Room: " + passRoom);
                        //LogFactory.GetLog("Main").LogInfo("Slots: " + fresslots);
                        //LogFactory.GetLog("Main").LogInfo("players: " + players);
                        //LogFactory.GetLog("Main").LogInfo("Playing: " + playing);

                        if (RoomType == 1)
                        {
                            if (room.Playing == true || room.GetFreeSlot() == 0)
                                continue;
                        }
                        oPacket.WriteUShort(roomid);//00 26
                        oPacket.WriteInt(nameRoom.Length * 2);//00 00 00 0C
                        oPacket.WriteUnicodeString(nameRoom);//68 BE AC B9 68 BE AC B9 21 00 21 00
                        if (passRoom.Length > 0)//01
                        {
                            oPacket.WriteByte(0);
                        }
                        else
                        {
                            oPacket.WriteByte(1);
                        }
                        oPacket.WriteByte(0);//00
                        oPacket.WriteInt(passRoom.Length * 2);//00 00 00 00
                        oPacket.WriteUnicodeString(passRoom);
                        oPacket.WriteUShort(fresslots);//00 04
                        oPacket.WriteUShort(players);//00 01
                        oPacket.WriteBool(playing);//00
                        oPacket.WriteHexString("28 5C DA F2 02 02 00 00 00 08 32 25 A5 77 00 00 00 03 00 00 00 00 00 00 00 01");
                        oPacket.WriteInt(nick.Length * 2);
                        oPacket.WriteUnicodeString(nick);
                        oPacket.WriteHexString("02 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 04 5A 00 5A 00 01");
                    }
                }
                //}

                /* 방 정보 조합
                foreach (Room room in RoomsList)
                {
                    // 대기방만 보여주는 것이라면...
                    if( RoomType == 1 )
                    {
                        // 게임시작이거나, 남는자리가 없다면 생략
                        if (room.Playing == true || room.GetFreeSlot() == 0)
                            continue;
                    }

                    oPacket.WriteUShort(1);
                    oPacket.WriteInt("kkk".Length * 2);
                    oPacket.WriteUnicodeString("kkk");

                    if (room.RoomPass.Length > 0)
                        oPacket.WriteByte(0); // 비밀방
                    else
                        oPacket.WriteByte(1); // 공개방
                    oPacket.WriteByte(0);
                    oPacket.WriteInt("hue".Length * 2);
                    oPacket.WriteUnicodeString("hue");
                    //LogFactory.GetLog("Main").LogInfo("PasswdRoom: " + room.RoomPass);

                    // 빈자리 + 유저수 = 총 자리 개수
                    oPacket.WriteShort(3 + 1);
                    //LogFactory.GetLog("Main").LogInfo(3);
                    oPacket.WriteShort(1);
                    //LogFactory.GetLog("Main").LogInfo(1);
                    oPacket.WriteBool(false);
                    //LogFactory.GetLog("Main").LogInfo("Playing: " + room.Playing);

                    oPacket.WriteHexString("2E 02 1B 25 01 00 00 00 00 01 6B F9 38 77 00 00 00 0C 00 00 00 00 00 00 00 01");

                    oPacket.WriteInt("[ADM]SkelletonX".Length * 2);
                    //LogFactory.GetLog("Main").LogInfo("LEADER NICK: " + room.GetRoomLeaderCS().Nick);
                    oPacket.WriteUnicodeString("[ADM]SkelletonX");

                    oPacket.WriteHexString("0B 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01");
                }*/

                // 여태껏 조합된 방 정보를 압축한다.
                int RoomInfoSize = oPacket.ToArray().Length; // 압축하기 전 사이즈
                oPacket.CompressBuffer();
                byte[] RoomInfo = oPacket.getBuffer();

                oPacket.InitBuffer(); // 초기화

                // 실제 패킷 작성
                oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 01");
                oPacket.WriteInt(4 + RoomInfo.Length); // 압축된 정보크기 + 4(실제 크기버퍼)
                oPacket.WriteByte(1); // 압축된 데이터다.
                oPacket.WriteBytes(BitConverter.GetBytes(RoomInfoSize)); // 압축전 사이즈를 일반적으로 넣는다.
                oPacket.WriteBytes(RoomInfo); // 압축된 방 정보

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
                //LogFactory.GetLog("Main").LogHex("ROOMLIST: ", oPacket.getBuffer());
            }
        }



        public void OnRoomList2(ClientSession cs, InPacket ip)
        {
            byte RoomType = ip.ReadByte(); // 1 = 대기방보기, 0 = 모든방보기
            //LogFactory.GetLog("Main").LogInfo("RoomTYPE: " + RoomType);

            // 그 뒤로
            // FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00
            // 어딘가에 페이지가 있겠다.

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ROOM_LIST_ACK))
            {
                // 방 정보를 조합한 다음에 압축하고, 실제 패킷을 작성한다.

                // 방 개수 구하기
                int roomcount = 0;

                //LogFactory.GetLog("Main").LogInfo("ROOMS Count: " + roomcount);

                foreach (Room room in RoomsList)
                {
                    //LogFactory.GetLog("Main").LogInfo("PasswdRoom: " + room.RoomPass);
                    //LogFactory.GetLog("Main").LogInfo("Slots: " + room.GetFreeSlot());
                    //LogFactory.GetLog("Main").LogInfo("Players: " + room.GetPlayerCount());
                    //LogFactory.GetLog("Main").LogInfo("Playing: " + room.Playing);
                    // 대기방만 보여주는 것이라면...
                    if (RoomType == 1)
                    {
                        // 게임시작이거나, 남는자리가 없다면 생략
                        if (room.Playing == true || room.GetFreeSlot() == 0)
                            continue;
                    }
                    roomcount++;
                    //LogFactory.GetLog("Main").LogInfo("Rooms: " + roomcount);
                }
                oPacket.WriteInt(roomcount);

                // 방 정보 조합
                foreach (Room room in RoomsList)
                {
                    // 대기방만 보여주는 것이라면...
                    /*if (RoomType == 1)
                    {
                        // 게임시작이거나, 남는자리가 없다면 생략
                        if (room.Playing == true || room.GetFreeSlot() == 0)
                            continue;
                    }
                    */
                    oPacket.WriteUShort(room.ID);
                    oPacket.WriteInt(room.RoomName.Length * 2);
                    oPacket.WriteUnicodeString(room.RoomName);

                    if (room.RoomPass.Length > 0)
                        oPacket.WriteByte(0); // 비밀방
                    else
                        oPacket.WriteByte(1); // 공개방
                    oPacket.WriteByte(0);
                    oPacket.WriteInt(room.RoomPass.Length * 2);
                    oPacket.WriteUnicodeString(room.RoomPass);
                    //LogFactory.GetLog("Main").LogInfo("PasswdRoom: " + room.RoomPass);

                    // 빈자리 + 유저수 = 총 자리 개수
                    oPacket.WriteShort((short)(room.GetFreeSlot() + room.GetPlayerCount()));
                    //LogFactory.GetLog("Main").LogInfo("Slots: " + room.GetFreeSlot());
                    oPacket.WriteShort((short)room.GetPlayerCount());
                    //LogFactory.GetLog("Main").LogInfo("Players: " + room.GetPlayerCount());
                    oPacket.WriteBool(room.Playing);
                    //LogFactory.GetLog("Main").LogInfo("Playing: " + room.Playing);

                    oPacket.WriteHexString("2E 02 1B 25 01 00 00 00 00 01 6B F9 38 77 00 00 00 0C 00 00 00 00 00 00 00 01");

                    oPacket.WriteInt(room.GetRoomLeaderCS().Nick.Length * 2);
                    //LogFactory.GetLog("Main").LogInfo("LEADER NICK: " + room.GetRoomLeaderCS().Nick);
                    oPacket.WriteUnicodeString(room.GetRoomLeaderCS().Nick);

                    oPacket.WriteHexString("0B 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01");
                }

                // 여태껏 조합된 방 정보를 압축한다.
                int RoomInfoSize = oPacket.ToArray().Length; // 압축하기 전 사이즈
                oPacket.CompressBuffer();
                byte[] RoomInfo = oPacket.getBuffer();

                oPacket.InitBuffer(); // 초기화

                // 실제 패킷 작성
                oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 01");
                oPacket.WriteInt(4 + RoomInfo.Length); // 압축된 정보크기 + 4(실제 크기버퍼)
                oPacket.WriteByte(1); // 압축된 데이터다.
                oPacket.WriteBytes(BitConverter.GetBytes(RoomInfoSize)); // 압축전 사이즈를 일반적으로 넣는다.
                oPacket.WriteBytes(RoomInfo); // 압축된 방 정보

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
                //LogFactory.GetLog("Main").LogHex("ROOMLIST: ", oPacket.getBuffer());
            }
        }

        public void OnJoinRoom(ClientSession cs, InPacket ip)
        {
            ip.ReadByte(); // 0
            ip.ReadByte(); // 0
            ip.ReadByte(); // 0
            ip.ReadByte(); // 0
            ushort RoomID = ip.ReadUShort();
            int PassLen = ip.ReadInt();
            string Pass = "";
            if (PassLen > 0)
                Pass = ip.ReadUnicodeString(PassLen);
            //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM ID: "+RoomID);
            //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Length Password: " + PassLen);
            //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Password: " + Pass);

            // 패킷 내용은 겁나 긴데 필요하다면 나중에 추가하자.
            // 우선 이 내용만으로도 충분하다.

            Room room = GetRoom(RoomID); // 요청한 방

            // 방이 없으면 입장 실패
            if (room == null)
                goto cantjoin;

            // 자리가 없거나, 게임중이거나, 이미 참가한 방이 있다면
            if (room.GetFreeSlot() == 0 || room.Playing == true || cs.CurrentRoom != null)
                goto cantjoin;

            // 방 비밀번호가 다르면
            if (room.RoomPass != Pass)
                goto cantjoin;

            // 실제로 방에 입장시킨다.
            // 적절한 슬롯 구하기
            byte Team1 = 0, Team2 = 0;
            int EmptyPos1 = -1, EmptyPos2 = -1;
            for (int i = 0; i < 3; i++)
            {
                if (room.Slot[i].Active == true)
                    Team1++;
                if (EmptyPos1 == -1 && room.Slot[i].Open == true)
                    EmptyPos1 = i;
            }
            for (int i = 3; i < 6; i++)
            {
                if (room.Slot[i].Active == true)
                    Team2++;
                if (EmptyPos2 == -1 && room.Slot[i].Open == true)
                    EmptyPos2 = i;
            }

            // 세르딘이 많으면 카나반에 넣는다.
            int pos = EmptyPos1;
            if (Team1 >= Team2)
                pos = EmptyPos2;

            //LogFactory.GetLog("Main").LogInfo(" Sergio Dean {0} metade Kana {1} {2} uma determinada ranhura ({3}, {4})", Team1, Team2, pos, EmptyPos1, EmptyPos2);

            // 방 슬롯에 정보 입력
            room.Slot[pos].Active = true;
            room.Slot[pos].cs = cs;
            room.Slot[pos].Open = false;
            room.Slot[pos].Spree = 0;
            room.Slot[pos].Leader = false;
            room.Slot[pos].AFK = false;

            // 유저한테 방 입력
            cs.CurrentRoom = room;

            // 방에 있는 사람들한테 입장했다고 알려준다. (입장하는놈은 제외)
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_JOIN_ROOM_BROAD))
            {
                oPacket.WriteInt(cs.Login.Length * 2);
                oPacket.WriteUnicodeString(cs.Login);
                oPacket.WriteInt(cs.LoginUID);
                oPacket.WriteInt(cs.Nick.Length * 2);
                oPacket.WriteUnicodeString(cs.Nick);
                oPacket.WriteInt(pos);
                oPacket.WriteByte((byte)cs.CurrentChar);
                oPacket.WriteHexString("00 FF 00 FF 00 FF 00 00 00 00");
                oPacket.WriteByte((byte)(pos / 3)); // 팀을 구하면 됨. 빠르게 처리하기 위해서
                oPacket.WriteHexString("01 00 00 00 0D 00 00 00 00 10 F4 00 00 00 00 00 4E 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 24 00 00 00 00 00 00 00 00 00 00 00 00 00 00 27 00 00 00 00 00 00 00 00 00 00 00 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 00 00 00 00 00 00 00 00 00 00 00 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 36 00 00 00 00 00 00 00 00 00 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00");
                if (room.GetRoomLeaderCS() == cs)
                    oPacket.WriteByte(1);
                else
                    oPacket.WriteByte(0);
                oPacket.WriteHexString("01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

                oPacket.WriteByte((byte)cs.MyCharacter.MyChar.Length);
                for (int i = 0; i < cs.MyCharacter.MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                    //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Chartype: " + cs.MyCharacter.MyChar[i].CharType);
                    oPacket.WriteInt(0);
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].Promotion);
                    //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Chartype: " + cs.MyCharacter.MyChar[i].Promotion);
                    oPacket.WriteInt(0);
                    oPacket.WriteByte(0);
                    oPacket.WriteInt(cs.MyCharacter.MyChar[i].Exp);
                    //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Chartype: " + cs.MyCharacter.MyChar[i].Exp);
                    oPacket.WriteByte(0);
                    oPacket.WriteByte(0);
                    oPacket.WriteByte(0);
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].Level);
                    //LogFactory.GetLog("ROOM INFO:").LogInfo("ROOM Chartype: " + cs.MyCharacter.MyChar[i].Level);
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(0);

                    oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip.Length);
                    for (int j = 0; j < cs.MyCharacter.MyChar[i].Equip.Length; j++)
                    {
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemID);
                        oPacket.WriteInt(1);
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemUID);
                        oPacket.WriteInt(0);
                        oPacket.WriteInt(0);
                        oPacket.WriteInt(0);
                        oPacket.WriteInt(0);
                        oPacket.WriteByte(0);
                        oPacket.WriteByte(0);
                        oPacket.WriteByte(0);
                    }

                    //oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00 00 00 00 00 00 00 00 02 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 01 FF 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                    // 이 패킷이 아래 끝까지 분리됐음. ▼▼
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00");

                    // 장착한 스킬
                    /*
                    oPacket.WriteInt(cs.MyCharacter.MyChar[i].EquipSkill.Length);
                    for (int j = 0; j < cs.MyCharacter.MyChar[i].EquipSkill.Length; j++)
                    {
                        oPacket.WriteInt(0);
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].EquipSkill[j].SkillGroup);
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].EquipSkill[j].SkillID);
                    }*/
                    oPacket.WriteInt(0);

                    // FF가 스킬포인트일지도
                    oPacket.WriteHexString("00 00 00 FF 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 FF 00 00 00 00 00 00");

                    // 배운 스킬
                    /*oPacket.WriteInt(cs.MyCharacter.MyChar[i].MySkill.Length);
                    for (int j = 0; j < cs.MyCharacter.MyChar[i].MySkill.Length; j++)
                    {
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].MySkill[j].SkillID);
                    }*/
                    oPacket.WriteInt(0);

                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                    // ▲▲
                }
                oPacket.WriteHexString("00 00 00 04 13 00 A8 C0 01 EC A8 C0 9B BA FE A9");
                oPacket.WriteIPFromString(cs.GetIP(), false);
                oPacket.WriteHexString("00 00 00 01 7E F6 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 E5 6A 00 00 00 01 2C BD 52 5A 00 00 00 00 01 00 00 E5 88 00 00 00 01 2C BD 52 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 56 86 32 00 56 87 6E D4 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    // 나 자신은 제외한 나머지한테 전송
                    if (room.Slot[i].Active == true && room.Slot[i].cs != cs)
                    {
                        oPacket.Assemble(room.Slot[i].cs.CRYPT_KEY, room.Slot[i].cs.CRYPT_HMAC, room.Slot[i].cs.CRYPT_PREFIX, room.Slot[i].cs.CRYPT_COUNT);
                        room.Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }

            // 방에 접속했다고 알려준다.
            using (OutPacket op = new OutPacket(GameOpcodes.EVENT_JOIN_ROOM_INFO_ACK))
            {
                op.WriteUShort(RoomID);
                op.WriteInt(room.RoomName.Length * 2);
                op.WriteUnicodeString(room.RoomName);
                op.WriteByte(0);
                if (room.RoomPass.Length > 0)
                    op.WriteByte(1); // 비밀방
                else
                    op.WriteByte(0); // 공개방
                op.WriteInt(room.RoomPass.Length * 2);
                op.WriteUnicodeString(room.RoomPass);
                op.WriteShort((short)room.GetPlayerCount());
                op.WriteShort((short)room.GetFreeSlot());
                op.WriteHexString("00 0B");
                op.WriteByte((byte)room.GameCategory);
                op.WriteInt(room.GameMode);
                op.WriteInt(room.ItemMode);
                op.WriteBool(room.RandomMap);
                op.WriteInt(room.GameMap);
                op.WriteHexString("00 00 00 0C");
                for (int i = 0; i < 6; i++)
                    op.WriteBool(room.Slot[i].Open);
                op.WriteHexString("FF FF FF FF 00 00 00 00 00 00 00 00 01");
                op.WriteIPFromString(Server.UDPRelayIP, true);
                op.WriteShort(Server.UDPRelayPort);
                op.WriteIPFromString(Server.TCPRelayIP, true);
                op.WriteShort(Server.TCPRelayPort);

                op.WriteHexString("01 00 01 00 00 01 2C 00 00 00 14 00 02 4B 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 01 00 00 00 00");

                op.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(op);
            }

            room.SendJoinRoomInfoDivide(cs);
            //return;

        // 방에 입장할 수 없다.
        cantjoin:
            using (OutPacket op = new OutPacket(GameOpcodes.EVENT_JOIN_ROOM_INFO_DIVIDE_ACK))
            {
                op.WriteInt(6);
                op.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 01 30 00 00 00 F9 00 00 09 0D 00 00 00 00 00 00 00 00 F2 04 00 00 00 00 00 00 13 49 F4 FC 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 09 13 F2 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

                op.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(op);
            }
            //return;
        }

        public void OnChat(ClientSession cs, InPacket ip)
        {
            ip.ReadByte(); // 01
            ip.ReadInt(); // 00 00 00 00
            int WhereLen = ip.ReadInt();
            string Where = ip.ReadUnicodeString(WhereLen);
            ip.ReadInt(); // 00 00 00 00
            ip.ReadInt(); // 00 00 00 00
            ip.ReadInt(); // FF FF FF FF
            int ChatLen = ip.ReadInt();
            string Chat = ip.ReadUnicodeString(ChatLen);

            //LogFactory.GetLog("Main").LogInfo("Chat: {0}", Chat);

            using (OutPacket op = new OutPacket(GameOpcodes.EVENT_CHAT_NOT))
            {
                op.WriteByte(1);
                op.WriteInt(cs.LoginUID);
                op.WriteInt(cs.Nick.Length * 2);
                op.WriteUnicodeString(cs.Nick);
                op.WriteInt(0);
                op.WriteInt(0);
                op.WriteInt(-1);
                op.WriteInt(ChatLen);
                op.WriteUnicodeString(Chat);
                op.WriteInt(0);
                op.WriteInt(0);

                // 채널에 있는 모든 유저중 같은 방 (null이면 로비실이니까.)
                foreach (ClientSession u in UsersList)
                {
                    if (u.CurrentRoom == cs.CurrentRoom)
                    {
                        op.Assemble(u.CRYPT_KEY, u.CRYPT_HMAC, u.CRYPT_PREFIX, u.CRYPT_COUNT);
                        u.Send(op);
                        op.CancelAssemble(); // 버퍼 되돌리기
                    }
                }
            }
        }
    }

    public class ChannelManager : TSingleton<ChannelManager>
    {
        public Dictionary<string, Channel> m_mChannels { get; set; }

        public ChannelManager()
        {
            m_mChannels = new Dictionary<string, Channel>();
        }

        public bool AddChannel(string strChannelName, ushort usMaxUsers, byte nLevel)
        {
            if (m_mChannels.ContainsKey(strChannelName.ToLower())) return false;

            Channel ch = new Channel() { ChannelName = strChannelName, MaxUsers = usMaxUsers, Level = nLevel };

            m_mChannels.Add(strChannelName.ToLower(), ch);

            return true;
        }

        public Channel GetChannel(string strChannelName)
        {
            if (m_mChannels.ContainsKey(strChannelName.ToLower()))
            {
                return m_mChannels[strChannelName.ToLower()];
            }

            return null;
        }
    }
}
