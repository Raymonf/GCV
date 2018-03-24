using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GrandChase.IO.Packet;
using GrandChase.Net;
using GrandChase.Net.Client;
using GrandChase.Data;
using GrandChase.Security;
using GrandChase.IO;
using GrandChase.Function;
using Manager.Factories;

namespace GrandChase.Function
{
    public class User
    {
        public int LenNameRoom;
        public String NameRoom;
        public byte roomid;
        public int guid;
        public int gamecategory;
        public int gamemode;
        public int subgamemode;
        public int randommap;
        public int map;
        public int matchmode;
        public uint UsersInRoom;
        public byte unk2;
        public uint MaxPlayers;
        public byte[] freeslots;
        public bool isPlay;
        public bool RandomMap;
        public byte CharType;

        //Account Infos
        public byte authLevel = 1;
        public byte TutorialCheck = 0;
        moedas coins = new moedas();

        public void OnCreateRoom(ClientSession cs, InPacket ip)
        {

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_CREATE_ROOM_ACK))//25
            {
                ushort blank = ip.ReadUShort();
                int LenNameRoom = ip.ReadInt();
                string NameRoom = ip.ReadString(LenNameRoom);
                roomid = ip.ReadByte();
                bool unk1 = false;
                bool Guild = ip.ReadBool();
                int LenPassRoom = ip.ReadInt();
                string RoomPass = ip.ReadString(LenPassRoom);
                UsersInRoom = ip.ReadUShort();
                MaxPlayers = ip.ReadUShort();
                isPlay = ip.ReadBool();
                unk2 = ip.ReadByte();
                matchmode = ip.ReadByte();
                gamemode = ip.ReadInt();
                int readMatch = ip.ReadInt();
                RandomMap = ip.ReadBool();
                map = ip.ReadInt();
                uint unkUint = ip.ReadUInt();//00 00 00 12
                short freeslotss = 6;//Missao = 4 , PVP = 6
                //LogFactory.GetLog("Main").LogInfo("MATCH MODE: "+matchmode);
                //LogFactory.GetLog("Main").LogInfo("GAME MODE: " + gamemode);
                //LogFactory.GetLog("Main").LogInfo("MAP: " + map);
                if (matchmode == 2)
                {
                    freeslotss = 4;//Missao = 4 , PVP = 6
                }
                if (matchmode == 0)
                {
                    freeslotss = 6;//Missao = 4 , PVP = 6
                }
                byte[] jump1 = ip.ReadBytes(97);//skip 97 bytes
                int LenLoginP = ip.ReadInt();
                string LoginP = ip.ReadString(LenLoginP);
                int LoginUID = ip.ReadInt();
                int lenNick = ip.ReadInt();
                string nick = ip.ReadString(lenNick);
                int indexChar = ip.ReadInt();
                CharType = ip.ReadByte();
                byte classe = ip.ReadByte();

                //////////////PRECISAM SER ARMAZENADOS EM UMA DB//////////////////
                int BonusLife = 3;
                ////////////////////////////////

                ushort usEmptyRoomSlot = cs.CurrentChannel.GetEmptyRoom();
                Room room = new Room();

                lock (cs.CurrentChannel._lock)
                {
                    cs.CurrentChannel.RoomsList.Add(room);
                    cs.CurrentChannel.RoomsMap.Add(usEmptyRoomSlot, room);
                }

                room.ID = usEmptyRoomSlot;

                room.RoomName = NameRoom;
                room.RoomPass = RoomPass;
                room.GameCategory = (int)matchmode;
                room.GameMode = (int)gamemode;
                room.ItemMode = 1;
                room.RandomMap = false;
                room.GameMap = (int)map;
                room.Kick = 3;
                room.Playing = false;

                //room.Slot[0].Active = true;

                for (int i = 0; i < freeslotss; i++)
                {
                    room.Slot[i].Active = false;
                    room.Slot[i].Open = true;
                    room.Slot[i].cs = null;
                    room.Slot[i].LoadStatus = 0;
                    room.Slot[i].State = 0;
                    room.Slot[i].AFK = false;
                    room.Slot[i].Leader = false;
                }


                /*for (int i = 0; i < 6; i++)
                {
                    room.Slot[i].Active = false;
                    room.Slot[i].Open = true;
                    room.Slot[i].cs = null;
                    room.Slot[i].LoadStatus = 0;
                    room.Slot[i].State = 0;
                    room.Slot[i].AFK = false;
                    room.Slot[i].Leader = false;
                }*/
                room.Slot[0].cs = cs;
                room.Slot[0].Leader = true;
                room.Slot[0].Open = false;

                cs.CurrentRoom = room;

                oPacket.WriteInt(0);
                oPacket.WriteInt(cs.Login.Length * 2);
                oPacket.WriteUnicodeString(cs.Login);
                oPacket.WriteInt(cs.LoginUID);
                oPacket.WriteInt(cs.Nick.Length * 2);
                oPacket.WriteUnicodeString(cs.Nick);
                oPacket.WriteInt(indexChar);//00 00 00 00
                oPacket.WriteByte(CharType);//00
                oPacket.WriteByte(classe);//00
                oPacket.WriteInt(0);//00 00 00 00
                oPacket.WriteHexString("FF 00 FF 00 FF 00 00 00 00 00 00 00 00 00 14 00 00 00 00 07 D0 00 00");
                oPacket.WriteInt(83);//00 00 00 53
                oPacket.WriteHexString("00 00 00 07 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 24 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 27 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 36 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 62 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 63 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 01 00 00 00 00 00 00 00 00 00 00 00 00");
                oPacket.WriteInt(cs.MyCharacter.MyChar.Length); //00 00 00 01 //Length Chars
                for (int a = 0; a < cs.MyCharacter.MyChar.Length; a++)
                {
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[a].CharType); //00
                    oPacket.WriteInt(0);//00 00 00 00
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[a].Promotion);//00
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[a].Promotion);//00
                    oPacket.WriteULong((ulong)cs.MyCharacter.MyChar[a].Exp);//00 00 00 00 00 00 00 64
                    oPacket.WriteInt((int)cs.MyCharacter.MyChar[a].Level);//00 00 00 01
                    oPacket.WriteULong((ulong)0);//00 00 00 00 00 00 00 00
                    oPacket.WriteInt(cs.MyCharacter.MyChar[a].Equip.Length);//00 00 00 06 //Length EQUIPS
                    for (int b = 0; b < cs.MyCharacter.MyChar[a].Equip.Length; b++)
                    {
                        oPacket.WriteInt(cs.MyCharacter.MyChar[a].Equip[b].ItemID);//00 05 CB AC
                        oPacket.WriteInt(0);//00 00 00 00
                        oPacket.WriteInt(cs.MyCharacter.MyChar[a].Equip[b].ItemUID);//00 35 95 86
                        oPacket.WriteHexString("00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00");
                    }
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00 00 00 00 00 00 00 00 04 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                    oPacket.WriteUInt((uint)a);
                    //00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00 00 00 00 00 00 00 00 04 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 07 D0 00 00 00 0A 00 00 00 00
                    oPacket.WriteHexString("00 00 07 D0 00 00 00 0A 00 00 00 00");
                }
                //00 02 00 00 00 02 00 03 01 41 10 00 00 01 04 01 41 D8 00 00 00 05 CB B6 00 00 00 00 00 35 95 87 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 00 01 40 A0 00 00 01 1C 01 3E 9E B8 52 00 05 CB C0 00 00 00 00 00 35 95 88 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 1C 01 3E 9E B8 52 01 04 01 41 D8 00 00 00 05 CB CA 00 00 00 00 00 35 95 89 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 00 01 40 A0 00 00 01 09 01 00 00 00 00 00 05 CB D4 00 00 00 00 00 35 95 8A 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 08 01 3D A3 A2 9C 01 03 01 41 10 00 00 00 06 52 CA 00 00 00 00 00 35 95 8B 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 0C 01 3D 23 6E 2F 01 0A 01 3E 0F 5C 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00 00 00 00 00 00 00 00 02 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 07 D0 00 00 00 0A 00 00 00 00 00 00 00 02 64 00 A8 C0 DA 30 31 C8 00 00 00 01 7F 09 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 E5 6A 00 00 00 00 00 35 95 8C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 41 00 52 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 01 00 00 00 00 F4 09 BD 04 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                //oPacket.WriteHexString("00 00 00 02 64 00 A8 C0 DA 30 31 C8 00 00 00 01 7F 09 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 E5 6A 00 00 00 00 00 35 95 8C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 41 00 52 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 01 00 00 00 00 F4 09 BD 04 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                oPacket.WriteHexString("00 00 00 02 64 00 A8 C0 DA 30 31 C8 00 00 00 01 7F 05 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 E5 6A 00 00 00 00 00 35 95 8C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 41 00 52 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 D2 F0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 01 00 00 00 00 74 97 F7 1B 00 00 00 00 00 00 00 00 02 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                oPacket.WriteInt(cs.CurrentRoom.ID);//00 00 00 00
                //LogFactory.GetLog("Main").LogInfo("ID da SALA: " + cs.CurrentRoom.ID);
                oPacket.WriteInt(LenNameRoom * 2);
                oPacket.WriteUnicodeString(NameRoom);
                oPacket.WriteHexString("01 00 00 00");
                oPacket.WriteShort((short)room.GetPlayerCount());
                //LogFactory.GetLog("Main").LogInfo("PLAYERS: " + room.GetPlayerCount());
                oPacket.WriteShort((short)(room.GetPlayerCount() + room.GetFreeSlot()));
                oPacket.WriteUShort((ushort)freeslotss);//00 04
                //LogFactory.GetLog("Main").LogInfo("SLOTS: " + freeslotss);
                oPacket.WriteHexString("00 01");
                oPacket.WriteByte((byte)matchmode);
                oPacket.WriteInt(gamemode);//00 00 00 07
                oPacket.WriteInt(0);//00 00 00 02
                oPacket.WriteInt(room.ItemMode);
                oPacket.WriteInt(map);
                oPacket.WriteHexString("12 00 01 01 01 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 01 5A F8 3F 4A 25 80 5A F8 3F 4A 25 E4 01 00 01 00 00 01 2C 00 00 00 14 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 41 00 52 00 00 04 01 00 00 00 01");
                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
                room.Slot[0].Active = true;
                ////LogFactory.GetLog("ROOM CHARS INFO:").LogHex("", oPacket.getBuffer());

            }
        }

        public void SendAccTime(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ACC_TIME_NOT))
            {
                oPacket.WriteHexString("00 00 00 02 07 E0 0A 03 01");

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        /*public void OnCharSelectJoin(ClientSession cs, InPacket ip)
        {
            //LogFactory.GetLog("Main").LogInfo("OnCharSelectJoin.");
            ////LogFactory.GetLog("Main").LogHex("Pacote", ip.ToArray());

            // 원래 이때 보내는건지를 모르겠다.
            cs.MyCommon.SendNoInvenItemList(cs);
            cs.MyCommon.SendStrengthMaterialInfo(cs);
            cs.MyCommon.SendWeeklyrewardList(cs);
            cs.MyCommon.SendMonthlyrewardList(cs);
            cs.MyCommon.SendMatchRankReward(cs);
            cs.MyCommon.SendHeroDungeonInfo(cs);
            cs.MyCommon.SendUserWeaponChange(cs);
            cs.MyCommon.SendNewCharCardInfo(cs);
            cs.MyCommon.SendVirtualCashLimitRatio(cs);
            cs.MyCommon.SendBadUserInfo(cs);
            cs.MyCommon.SendCollectionMission(cs);
            cs.MyCommon.SendHellTicketFreeMode(cs);
            cs.MyCommon.SendVIPItemList(cs);
            cs.MyCommon.SendCapsuleList(cs);
            cs.MyCommon.SendMissionPackList(cs);
        }*/
        public void OnCharSelectJoin(ClientSession cs, InPacket ip)
        {
            //Log.Warn("OnCharSelectJoin 처리기가 완성되지 않았습니다.");

            // 원래 이때 보내는건지를 모르겠다.

            cs.MyCommon.SendNoInvenItemList(cs);
            cs.MyCommon.SendStrengthMaterialInfo(cs);
            cs.MyCommon.SendWeeklyrewardList(cs);
            cs.MyCommon.SendMonthlyrewardList(cs);
            cs.MyCommon.SendMatchRankReward(cs);
            cs.MyCommon.SendHeroDungeonInfo(cs);
            cs.MyCommon.SendUserWeaponChange(cs);
            cs.MyCommon.SendNewCharCardInfo(cs);
            cs.MyCommon.SendVirtualCashLimitRatio(cs);
            cs.MyCommon.SendBadUserInfo(cs);
            cs.MyCommon.SendCollectionMission(cs);
            cs.MyCommon.SendHellTicketFreeMode(cs);
            cs.MyCommon.SendVIPItemList(cs);
            cs.MyCommon.SendCapsuleList(cs);
            cs.MyCommon.SendMissionPackList(cs);
            cs.MyCommon.SendRainbowEvent(cs);//new
            cs.MyCommon.escortinfo(cs); //EVENT_RAINBOW_EVENT_NOT
            cs.MyCommon.SendRKTondoItemList(cs);
            cs.MyCommon.SendCharistma(cs);
            cs.MyCommon.SendGachaLottery(cs);
            cs.MyCommon.SendItemTradeList(cs);
            cs.MyCommon.SendNPCGifts(cs);
            cs.MyCommon.SendItemCharPromotionLevel(cs);
            cs.MyCommon.SendGPAttributeInitItem(cs);
            cs.MyCommon.SendGPAttributeRandomItem(cs);
            cs.MyCommon.SendItemAttibuteRandomSelectList(cs);
            cs.MyCommon.SendItemAttibuteRandomItemList(cs);
            cs.MyCommon.SendMaxCharSPLevel(cs);
            cs.MyCommon.SendGuildLevelTable(cs);

            cs.MyCommon.SendGachaNoticePopupInfo(cs);//new2
            cs.MyCommon.SendReportUser(cs);
            cs.MyCommon.SendBuyVirtulCash(cs);
            cs.MyCommon.SendRankSearch(cs);
            cs.MyCommon.SendFullCoupleInfo(cs);
            cs.MyCommon.SendTodayPopupInfo(cs);
            cs.MyCommon.SendSubscriptionInfo(cs);
            cs.MyCommon.SendMyMatchRankInfo(cs);
            cs.MyCommon.SendNewPostLetterInfo(cs);
            cs.MyCommon.SendChangeJob(cs);
            cs.MyCommon.OnChoiceBoxList(cs);
            cs.MyCommon.OnExpPotionList(cs);
            cs.MyCommon.OnAgitStoreCatalog(cs);
        }


        public void OnRegisterNick(ClientSession cs, InPacket ip)
        {
            int GetNickLength = ip.ReadInt();
            string GetNick = ip.ReadUnicodeString(GetNickLength);

            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT * FROM `accounts` WHERE Nick = '{0}'", GetNick);

            if (ds.Tables[0].Rows.Count == 0)
            {
                // DB 갱신
                Database.ExecuteScript("UPDATE `account` SET Nick = '{0}' WHERE LoginUID = '{1}'", GetNick, cs.LoginUID);

                // 세션 갱신
                cs.Nick = GetNick;

                SendAccTime(cs);

                // 성공 패킷
                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_REGISTER_NICKNAME_ACK))
                {
                    oPacket.WriteInt(0); // 0 = 성공인가?
                    oPacket.WriteInt(GetNickLength);
                    oPacket.WriteUnicodeString(GetNick);

                    oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                }
            }
            else
            {
                // 실패 패킷
                // 값 가져와야함.
                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_REGISTER_NICKNAME_ACK))
                {
                    oPacket.WriteInt(-1); // 1 = 오류. 어쨌든 안 만들어짐
                    oPacket.WriteInt(GetNickLength);
                    oPacket.WriteUnicodeString(GetNick);

                    oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                }
            }
        }
        public void checkuserauth(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT  `authlevel` FROM  `userauthlevel` WHERE id = '{0}'", cs.LoginUID);

            if (ds.Tables[0].Rows.Count == 0)
            {
                authLevel = 1;
            }
            else
            {
                authLevel = Convert.ToByte(ds.Tables[0].Rows[0]["authlevel"].ToString());
            }
            LogFactory.GetLog("Main").LogInfo("Account is From GM: " + authLevel + " ID: " + cs.LoginUID);
        }
        public void CheckTutorial(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT  `active` FROM  `tutorialmode` WHERE id = '{0}'", cs.LoginUID);
            if (ds.Tables[0].Rows.Count == 0)
            {
                cs.TutorialCheck = 0;
            }
            else
            {
                cs.TutorialCheck = Convert.ToByte(ds.Tables[0].Rows[0]["active"].ToString());
            }
        }
        public void CheckSlotFromCharacters(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT  `Slots` FROM  `charslot` WHERE LoginUID = '{0}'", cs.LoginUID);
            if (ds.Tables[0].Rows.Count == 0)
            {
                cs.CharSlot = 0;
            }
            else
            {
                cs.CharSlot = Convert.ToInt32(ds.Tables[0].Rows[0]["Slots"].ToString());
            }
        }

        public void OnLogin(ClientSession cs, InPacket ip)
        {
            int IDLength = ip.ReadInt();
            string ID = ip.ReadString(IDLength);
            int PWLength = ip.ReadInt();
            string PW = ip.ReadString(PWLength);
            int unk = 100;
            int UnlockStages = 102;
            int sizeInventory = 250;


            LogFactory.GetLog("Main").LogInfo("Login Length: " + IDLength);
            LogFactory.GetLog("Main").LogInfo("Login: " + ID);
            LogFactory.GetLog("Main").LogInfo("Senha: " + PW);
            LogFactory.GetLog("Main").LogInfo("Senha Length: " + PWLength);


            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT * FROM `accounts` WHERE Login = '{0}' AND Passwd = '{1}'", ID, PW);

            if (ds.Tables[0].Rows.Count == 0)
            {

                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_VERIFY_ACCOUNT_ACK))
                {
                    oPacket.WriteHexString("00 00 00 14");
                    oPacket.WriteInt(IDLength * 2);
                    oPacket.WriteUnicodeString(ID);
                    oPacket.WriteHexString("00 00 00 00 00");

                    oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                }
            }
            else
            {
                // 로그인 성공

                // 기본 데이터 가져오기
                cs.Login = ID;
                cs.LoginUID = Convert.ToInt32(ds.Tables[0].Rows[0]["LoginUID"].ToString());
                cs.Nick = ds.Tables[0].Rows[0]["Nick"].ToString();
                cs.GamePoint = Convert.ToInt32(ds.Tables[0].Rows[0]["Gamepoint"].ToString());
                sizeInventory = Convert.ToInt32(ds.Tables[0].Rows[0]["sizeInventory"].ToString());

                // 로딩
                cs.MyCharacter.LoadCharacter(cs);
                cs.MyInventory.LoadInventory(cs);
                cs.MyPet.LoadPet(cs);
                checkuserauth(cs);
                CheckTutorial(cs);
                CheckSlotFromCharacters(cs);
                coins.LoadVP(cs);
                coins.CurrentVirtualCash(cs);

                cs.Skilltree.LoadSkill(cs);
                cs.MyCommon.SendExpTable(cs); //1249
                cs.MyInventory.SendInventory(cs); //230
                cs.MyCommon.SendServerTime(cs); //415
               /* using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_NEW_CHAR_CARD_INFO_NOT))
                {
                    oPacket.WriteHexString("00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 85 00 00 78 6E 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 78 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 82 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 8C 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 96 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 A0 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 AA 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 B4 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 BE 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 C8 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 D2 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 DC 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 E6 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 F0 00 00 00 01 00 00 00 00 00 00 00 00 00 00 78 FA 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 04 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 0E 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 18 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 22 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 2C 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 36 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 40 00 00 00 01 00 00 00 00 00 00 00 00 00 00 79 4A 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 C0 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 CA 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 D4 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 DE 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 E8 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 F2 00 00 00 01 00 00 00 00 00 00 00 00 00 00 85 FC 00 00 00 01 00 00 00 00 00 00 00 00 00 00 86 06 00 00 00 01 00 00 00 00 00 00 00 00 00 00 86 10 00 00 00 01 00 00 00 00 00 00 00 00 00 00 86 1A 00 00 00 01 00 00 00 00 00 00 00 00 00 00 86 24 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 8C 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 96 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 A0 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 AA 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 B4 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 BE 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 C8 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 D2 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 DC 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 E6 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 F0 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 FA 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 04 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 0E 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 18 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 22 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 2C 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 36 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 40 00 00 00 01 00 00 00 00 00 00 00 00 00 01 46 4A 00 00 00 01 00 00 00 00 00 00 00 00 00 01 9B 18 00 00 00 01 00 00 00 00 00 00 00 00 00 01 FF 40 00 00 00 01 00 00 00 00 00 00 00 00 00 01 FF 4A 00 00 00 01 00 00 00 00 00 00 00 00 00 02 1E 9E 00 00 00 01 00 00 00 00 00 00 00 00 00 02 29 A2 00 00 00 01 00 00 00 00 00 00 00 00 00 02 41 08 00 00 00 01 00 00 00 00 00 00 00 00 00 02 CA 56 00 00 00 01 00 00 00 00 00 00 00 00 00 02 CA 60 00 00 00 01 00 00 00 00 00 00 00 00 00 02 CA 6A 00 00 00 01 00 00 00 00 00 00 00 00 00 02 CA 74 00 00 00 01 00 00 00 00 00 00 00 00 00 02 D1 2C 00 00 00 01 00 00 00 00 00 00 00 00 00 02 D1 36 00 00 00 01 00 00 00 00 00 00 00 00 00 02 D1 40 00 00 00 01 00 00 00 00 00 00 00 00 00 02 D1 4A 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 54 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 5E 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 68 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 72 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 7C 00 00 00 01 00 00 00 00 00 00 00 00 00 02 E0 86 00 00 00 01 00 00 00 00 00 00 00 00 00 03 4A 76 00 00 00 01 00 00 00 00 00 00 00 00 00 03 4A 80 00 00 00 01 00 00 00 00 00 00 00 00 00 03 4A 8A 00 00 00 01 00 00 00 00 00 00 00 00 00 03 4A 94 00 00 00 01 00 00 00 00 00 00 00 00 00 04 89 86 00 00 00 01 00 00 00 00 00 00 00 00 00 04 89 90 00 00 00 01 00 00 00 00 00 00 00 00 00 05 0F 6E 00 00 00 01 00 00 00 00 00 00 00 00 00 05 0F 78 00 00 00 01 00 00 00 00 00 00 00 00 00 05 9A 42 00 00 00 01 00 00 00 00 00 00 00 00 00 06 E2 3A 00 00 00 01 00 00 00 00 00 00 00 00 00 09 33 1A 00 00 00 01 00 00 00 00 00 00 00 00 00 09 33 24 00 00 00 01 00 00 00 00 00 00 00 00 00 09 54 66 00 00 00 01 00 00 00 00 00 00 00 00 00 0A 1E 28 00 00 00 01 00 00 00 00 00 00 00 00 00 0A 1E 32 00 00 00 01 00 00 00 00 00 00 00 00 00 0C 55 08 00 00 00 01 00 00 00 00 00 00 00 00 00 0C 55 12 00 00 00 01 00 00 00 00 00 00 00 00 00 0D 72 94 00 00 00 01 00 00 00 00 00 00 00 00 00 0D 72 9E 00 00 00 01 00 00 00 00 00 00 00 00 00 0E E9 E4 00 00 00 01 00 00 00 00 00 00 00 00 00 0E E9 EE 00 00 00 01 00 00 00 00 00 00 00 00 00 0E E9 F8 00 00 00 01 00 00 00 00 00 00 00 00 00 0E EA 02 00 00 00 01 00 00 00 00 00 00 00 00 00 0E EA 0C 00 00 00 01 00 00 00 00 00 00 00 00 00 0E EA 16 00 00 00 01 00 00 00 00 00 00 00 00 00 0F 85 98 00 00 00 01 00 00 00 00 00 00 00 00 00 0F 85 A2 00 00 00 01 00 00 00 00 00 00 00 00 00 10 49 60 00 00 00 01 00 00 00 00 00 00 00 00 00 10 49 6A 00 00 00 01 00 00 00 00 00 00 00 00 00 10 6A 3A 00 00 00 01 00 00 00 00 00 00 00 00 00 10 6A 44 00 00 00 01 00 00 00 00 00 00 00 00 00 10 A5 18 00 00 00 01 00 00 00 00 00 00 00 00 00 10 A5 22 00 00 00 01 00 00 00 00 00 00 00 00 00 10 E6 E0 00 00 00 01 00 00 00 00 00 00 00 00 00 10 E6 EA 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A A6 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A B0 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A BA 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A C4 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A CE 00 00 00 01 00 00 00 00 00 00 00 00 00 12 6A D8 00 00 00 01 00 00 00 00 00 00 00 00 00 12 9F 26 00 00 00 01 00 00 00 00 00 00 00 00 00 12 9F 30 00 00 00 01 00 00 00 00 00 00 00 00 00 12 9F 3A 00 00 00 01 00 00 00 00 00 00 00 00 00 12 9F 44 00 00 00 01 00 00 00 00 00 00 00 00 00 12 9F 4E 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 24 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 38 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 42 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 4C 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 56 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 60 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 6A 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 74 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 7E 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 88 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 92 00 00 00 01 00 00 00 00 00 00 00 00 00 18 E5 FC 00 00 00 01 00 00 00 00 00 00 00 00 00 18 E6 06 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 14 DC DC 00 00 00 01 3B 9C 50 A1 00 00 00 00 58 ED B0 93 58 EC 5F 13 00 00 00 00 00 14 DC E6 00 00 00 01 3B 9C 50 A2 00 00 00 00 58 ED B0 93 58 EC 5F 13 00 00 00 00 00 00 00 53 00 00 00 07 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 24 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 27 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 36 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 62 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 63 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 0A 89 62 00 00 00 00 1C 46 C6 EB 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 02 01 41 00 00 00 01 00 01 40 A0 00 00 00 0A 89 6C 00 00 00 00 1C 46 C6 EC 00 02 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 02 00 00 00 00 01 02 00 00 00 03 00 0C 01 3D 23 6E 2F 01 09 01 00 00 00 00 02 04 01 41 D8 00 00 00 0A 89 76 00 00 00 00 1C 46 C6 ED 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 03 01 41 10 00 00 01 09 01 00 00 00 00 00 0A 89 80 00 00 00 00 1C 46 C6 EE 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 0A 01 3E 0F 5C 29 01 1C 01 3E 9E B8 52 00 0A 89 8A 00 00 00 00 1C 46 C6 F0 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 1C 01 3E 9E B8 52 01 08 01 3D A3 A2 9C 00 0A A0 BE 00 00 00 00 1C 46 C6 F1 00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 02 00 00 00 02 00 07 01 3F 00 00 00 01 0C 01 3D 23 6E 2F 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 01 68 00 00 01 90 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF");
                //    oPacket.WriteHexString("00 00 00 12 00 0F 1D E2 00 0F 1D E2 00 00 00 00 00 00 0F 1D EC 00 0F 1D EC 01 00 00 00 00 00 0F 1D F6 00 0F 1D F6 02 00 00 00 00 00 0F 1E 00 00 0F 1E 00 03 00 00 00 02 00 07 C2 FE 00 07 C3 08 00 0F 1E 0A 00 0F 1E 0A 04 00 00 00 03 00 07 C3 12 00 07 C3 1C 00 07 C3 26 00 0F 1E 14 00 0F 1E 14 05 00 00 00 02 00 07 C2 EA 00 07 C2 F4 00 0F 1E 1E 00 0F 1E 1E 06 00 00 00 03 00 07 C3 76 00 07 C3 80 00 07 C3 8A 00 0F 1E 28 00 0F 1E 28 07 00 00 00 03 00 07 C3 44 00 07 C3 4E 00 07 C3 58 00 0F 1E 32 00 0F 1E 32 08 00 00 00 00 00 0F 1E 3C 00 0F 1E 3C 09 00 00 00 03 00 07 C3 DA 00 07 C3 E4 00 07 C3 EE 00 0F 1E 46 00 0F 1E 46 0A 00 00 00 03 00 07 C4 0C 00 07 C4 16 00 07 C4 20 00 0F 1E 50 00 0F 1E 50 0B 00 00 00 03 00 07 C4 3E 00 07 C4 48 00 07 C4 52 00 0F 1E 5A 00 0F 1E 5A 0C 00 00 00 03 00 08 04 80 00 08 04 8A 00 08 04 94 00 0F 1E 64 00 0F 1E 64 0D 00 00 00 00 00 0F 1E 6E 00 0F 1E 6E 0E 00 00 00 00 00 0F 1E 78 00 0F 1E 78 0F 00 00 00 00 00 0F 1E 82 00 0F 1E 82 10 00 00 00 00 00 12 DF FE 00 12 DF FE 11 00 00 00 00");
                    oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                }
                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_STAT_USER_HISTORY_NOT))
                {
                    oPacket.WriteHexString("00 00 00 01 00 00 00 01 00 00 00 00");
                    oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                }*/
                Settings.Initialize();

                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_VERIFY_ACCOUNT_ACK))
                {
                    oPacket.WriteInt(IDLength * 2);
                    oPacket.WriteUnicodeString(ID);
                    oPacket.WriteInt(cs.Nick.Length * 2);
                    oPacket.WriteUnicodeString(cs.Nick);
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(PWLength);
                    oPacket.WriteString(PW);
                    oPacket.WriteHexString("00 00");
                    oPacket.WriteInt(cs.GamePoint);
                    oPacket.WriteHexString("07 C9 DD 01");
                    oPacket.WriteHexString("DA 30 31 7F");//GetIP //DA 30 31 C8 //01 00 00 7F
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                    oPacket.WriteByte(authLevel);
                    oPacket.WriteInt(unk);//00 00 00 64
                    oPacket.WriteBool(false);//00
                    oPacket.WriteByte(0);//00
                    oPacket.WriteInt(cs.MyCharacter.MyChar.Length);
                    //LogFactory.GetLog("Main").LogInfo("Total de Chars: " + cs.MyCharacter.MyChar.Length);
                    for (int i = 0; i < (cs.MyCharacter.MyChar.Length); i++)
                    {
                        //LogFactory.GetLog("Main").LogInfo("CharTYPE: " + cs.MyCharacter.MyChar[i].CharType);
                        //LogFactory.GetLog("Main").LogInfo("Classe: " + cs.MyCharacter.MyChar[i].Promotion);
                        //LogFactory.GetLog("Main").LogInfo("Exp: " + cs.MyCharacter.MyChar[i].Exp);

                        oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                        oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                        oPacket.WriteHexString("00 00 00 00");
                        oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].Promotion);
                        oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].Promotion);
                        oPacket.WriteULong((ulong)cs.MyCharacter.MyChar[i].Exp);
                        oPacket.WriteInt((byte)cs.MyCharacter.MyChar[i].Win);
                        oPacket.WriteInt((byte)cs.MyCharacter.MyChar[i].Loss);
                        oPacket.WriteInt((byte)cs.MyCharacter.MyChar[i].Win);
                        oPacket.WriteInt((byte)cs.MyCharacter.MyChar[i].Loss);
                        oPacket.WriteULong((ulong)cs.MyCharacter.MyChar[i].Exp);
                        oPacket.WriteUInt((uint)cs.MyCharacter.MyChar[i].Level); //Level
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip.Length);
                        //LogFactory.GetLog("Main").LogInfo("Total Items: " + cs.MyCharacter.MyChar[i].Equip.Length);
                        for (int j = 0; j < cs.MyCharacter.MyChar[i].Equip.Length; j++)
                        {
                            //LogFactory.GetLog("Main").LogInfo("ItemID: " + cs.MyCharacter.MyChar[i].Equip[j].ItemUID);
                            oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemID);
                            oPacket.WriteInt(1);
                            oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemUID);
                            //LogFactory.GetLog("Main").LogInfo("ItemUID: " + cs.MyCharacter.MyChar[i].Equip[j].ItemID);
                            oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                            //00 01 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00
                        }
                        oPacket.WriteInt(cs.Skilltree.SP);//00 00 00 02
                        oPacket.WriteHexString("00 00 00 05 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                        oPacket.WriteInt(i);//00 00 00 00
                        oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 07 D0 00 00 07 D0 00 00 00 0A 00 00 00 00");
                        oPacket.WriteInt(sizeInventory);//00 00 01 68
                        oPacket.WriteHexString("00 00 01 90 00 00 00 00");
                   }
                    ushort portSecond;
                    portSecond = Settings.GetUShort("GameServer/PortSecond");
                    oPacket.WriteUShort(portSecond); //9401//24 B9
                    oPacket.WriteInt(cs.LoginUID);// 00 0B A7 AC 
                    string serverName;
                    serverName = Settings.GetString("GameServer/ServerName");
                    oPacket.WriteInt(serverName.Length * 2);
                    oPacket.WriteUnicodeString(serverName);//oPacket.WriteHexString("00 00 00 12 52 00 65 00 62 00 6F 00 72 00 6E 00 20 00 53 00 35 00"); //Servidor Nome
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(83);
                    for (int j = 7; j <= 102; j++)
                    {
                        oPacket.WriteInt(j);
                        oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00");
                        oPacket.WriteInt(0);
                    }
                    oPacket.WriteInt(31558);//00 00 7B 46
                    oPacket.WriteHexString("00 00 00 00");
                    oPacket.WriteInt(24); //00 00 00 18
                    oPacket.WriteHexString("A2 00 7B 43 E1 68 7E EE");
                    oPacket.WriteInt(1);//00 00 00 01
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00");
                    oPacket.WriteInt("MsgServer_01".Length * 2);
                    oPacket.WriteUnicodeString("MsgServer_01");
                    string MsgServerIP = Settings.GetString("GameServer/MsgServerIP");
                    int MsgServerPort = Settings.GetInt("GameServer/MsgServerPort");
                    //oPacket.WriteInt(MsgServerIP.Length);
                    //oPacket.WriteUnicodeString(MsgServerIP + (ushort)MsgServerPort);
                    oPacket.WriteHexString("00 00 00 0C 37 34 2E 36 33 2E 32 34 38 2E 39 30 24 54 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF");
                    /*oPacket.WriteInt(MsgServerIP.Length);
                    oPacket.WriteUnicodeString(MsgServerIP);*/
                    oPacket.WriteHexString("00 00 00 0C 37 34 2E 36 33 2E 32 34 38 2E 39 30 00 00 00 00 00 00 00");
                    oPacket.WriteByte(authLevel);//03
                    oPacket.WriteHexString("58 F1 B8 84 58 F1 B8 84 00 00 00 00");
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 01 00 00 00 00 00 01");
                    oPacket.WriteHexString("00 00 00 00 61 D0 96 A0 00 46 09 92 E0 64 02 34 EC 3C 7E E0 40 00 00 00 00 09 FF FF FF 58 F0 57 50 70 09 92 E1");
                    oPacket.WriteHexString("00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 07 6C 00 15 D5 6A");
                    oPacket.WriteInt(4);//00 00 00 04 Slot de Chars
                    oPacket.WriteHexString("FF");
                    oPacket.WriteByte(cs.TutorialCheck);
                    oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                    cs.Send(oPacket);
                    //oPacket.WriteHexString("00 00 00 12 52 00 65 00 62 00 6F 00 72 00 6E 00 20 00 53 00 35 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 24 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 27 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 36 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 62 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 63 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 65 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 66 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 7B 46 00 00 00 00 00 00 00 18 A2 00 7B 43 E1 68 7E EE 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 18 4D 00 73 00 67 00 53 00 65 00 72 00 76 00 65 00 72 00 5F 00 30 00 31 00 00 00 00 0C 37 34 2E 36 33 2E 32 34 38 2E 39 30 24 54 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF FF FF FF FF 00 00 00 0C 37 34 2E 36 33 2E 32 34 38 2E 39 30 00 00 00 00 00 00 00 00 03 58 F1 B8 84 58 F1 B8 84 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 01 00 00 00 00 61 D0 96 A0 00 46 09 92 E0 64 02 34 EC 3C 7E E0 40 00 00 00 00 09 FF FF FF 58 F0 57 50 70 09 92 E1 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 07 6C 00 15 D5 6A");
                }
                cs.MyCommon.SendServerTime(cs);
                cs.MyCommon.SendDungeonTicketList(cs);
                cs.MyCommon.SendPetVestedItem(cs);
                cs.MyCommon.SendGraduateCharInfo(cs);
                cs.MyCommon.MissionDateChange(cs);
                cs.MyCommon.SendJumpingCharInfo(cs);
                cs.MyCommon.SendGuideCompleteInfo(cs);
                cs.MyCommon.SendFullLookInfo(cs);
                cs.MyCommon.SendFairyTreeBuff(cs);
                cs.MyCommon.SendChangeSlotEquip(cs);
                cs.MyCommon.HeroDungeonInfo(cs);
                cs.MyCommon.SendAgitInitSeed(cs);

            }
        }


        public void OnEnterChannel(ClientSession cs, InPacket ip)
        {
            // ----- 1 = 대전, 6 = 던전
            int Channel = ip.ReadInt();

            // 혹시 모르니 이미 접속중인 채널 빠져나가도록.
            OnLeaveChannel(cs);

            // ----- 채널 입장 하드코딩
            string chName = Channel == 1 ? "대전" : "던전";
            Channel ch = TSingleton<ChannelManager>.Instance.GetChannel(chName);

            //LogFactory.GetLog("Main").LogInfo("posição do jogador de campo {0} para {1} canais.", cs.Login, chName);

            if (ch != null)
                if (!ch.UsersMap.ContainsKey(cs.Login))
                {
                    lock (ch._lock)
                    {
                        ch.UsersList.Add(cs);
                        ch.UsersMap.Add(cs.Login, cs);

                        cs.CurrentChannel = ch;
                    }
                }
            // -----

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ENTER_CHANNEL_ACK))
            {
                oPacket.WriteHexString("00 00 00 00 00 57 F2 01 F0 57 F3 53 6F 00 00 00 00");

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        public void OnLeaveChannel(ClientSession cs)
        {
            // 채널 떠나기
            if (cs.CurrentChannel == null)
                return;

            Channel ch;
            ch = TSingleton<ChannelManager>.Instance.GetChannel("대전");
            lock (cs.CurrentChannel._lock)
            {
                if (ch.UsersList.Contains(cs))
                    ch.UsersList.Remove(cs);

                if (ch.UsersMap.ContainsKey(cs.Login))
                    ch.UsersMap.Remove(cs.Login);
            }

            ch = TSingleton<ChannelManager>.Instance.GetChannel("던전");
            lock (cs.CurrentChannel._lock)
            {
                if (ch.UsersList.Contains(cs))
                    ch.UsersList.Remove(cs);

                if (ch.UsersMap.ContainsKey(cs.Login))
                    ch.UsersMap.Remove(cs.Login);
            }

            cs.CurrentChannel = null;
        }

            /*
            if (cs.CurrentChannel != null)
            {
                lock (cs.CurrentChannel._lock)
                {
                    cs.CurrentChannel.UsersList.Remove(cs);
                    cs.CurrentChannel.UsersMap.Remove(cs.Login);

                    cs.CurrentChannel = null;
                }
            }
            */
    }
}
