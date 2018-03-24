using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrandChase.IO.Packet;
using GrandChase.Net.Client;
using GrandChase.Data;
using GrandChase.Security;
using GrandChase.IO;
using GrandChase.Function;
using GrandChase.Utilities;
using Manager.Factories;

namespace GrandChase.Function
{
    public class Room
    {
        public const int MAX_USERS = 6;
        public enum eGameMode
        {
            GC_GM_TUTORIAL = 0,
            GM_TEAM = 1,
            GM_SURVIVAL = 2,
            GC_GM_TAG_TEAM = 3,
            GC_GM_TAG_SURVIVAL = 4,
            GC_GM_GUILD_BATTLE = 5,
            GC_GM_INDIGO_TEAM = 6
        }
        public enum eItemMode
        {
            GM_ITEM = 0,
            GM_NOITEM = 1
        }
        public enum eGameCategory
        {
            GMC_MATCH = 0,
            GC_GMC_GUILD_BATTLE = 1,
            GC_GMC_DUNGEON = 2,
            GC_GMC_INDIGO = 3,
            GC_GMC_TUTORIAL = 4,
            GC_GMC_TAG_MATCH = 5,
            GC_GMC_MONSTER_CRUSADER = 6,
            GC_GMC_MONSTER_HUNT = 7,
            GC_GMC_DEATHMATCH = 8,
            GC_GMC_MINIGAME = 9,
            GC_GMC_ANGELS_EGG = 10,
            GC_GMC_CAPTAIN = 11,
            GC_GMC_DOTA = 12,
            GC_GMC_AGIT = 13,
            GC_GMC_AUTOMATCH = 14,
            GC_GMC_FATAL_DEATHMATCH = 15,
            GC_GMC_MONSTERMATCH = 16

        }
        public enum eGameStage
        {
            GC_GS_FOREST_OF_ELF = 0,
            GC_GS_SWAMP_OF_OBLIVION = 1,
            GC_GS_FLYING_SHIP = 2,
            GC_GS_VALLEY_OF_OATH = 3,
            GC_GS_FOGOTTEN_CITY = 4,
            GC_GS_BABEL_OF_X_MAS = 5,
            GC_GS_TEMPLE_OF_FIRE = 6,
            GC_GM_QUEST0 = 7, GC_GM_QUEST1 = 8, GC_GM_QUEST2 = 9, GC_GM_QUEST3 = 10, GC_GM_QUEST4 = 11, GC_GM_QUEST5 = 12, GC_GM_QUEST6 = 13, GC_GM_QUEST7 = 14, GC_GM_QUEST8 = 15, GC_GM_QUEST9 = 16, GC_GM_QUEST10 = 17, GC_GM_QUEST11 = 18, GC_GM_QUEST12 = 19, GC_GM_QUEST13 = 20, GC_GM_QUEST14 = 21, GC_GM_QUEST15 = 22, GC_GM_QUEST16 = 23, GC_GM_QUEST17 = 24, GC_GM_QUEST18 = 25, GC_GM_QUEST19 = 26, GC_GM_QUEST20 = 27, GC_GM_MONSTER_CRUSADER = 28, GC_GM_MONSTER_HUNT = 29, GC_GM_QUEST21 = 30, GC_GM_DEATH_TEAM = 31, GC_GM_DEATH_SURVIVAL = 32, GC_GM_MINIGAME_TREEDROP = 33, GC_GM_MINIGAME_BALLOON = 34, GC_GM_MINIGAME_BUTTERFRY = 35, GC_GM_QUEST22 = 36, GC_GM_ANGELS_EGG = 37, GC_GM_CAPTAIN = 38, GC_GM_QUEST23 = 39, GC_GM_QUEST24 = 40, GC_GM_QUEST25 = 41, GC_GM_QUEST26 = 42, GC_GM_QUEST27 = 43, GC_GM_QUEST28 = 44, GC_GM_QUEST29 = 45, GC_GM_QUEST30 = 46, GC_GM_QUEST31 = 47, GC_GM_QUEST32 = 48, GC_GM_QUEST33 = 49, GC_GM_QUEST34 = 50, GC_GM_QUEST35 = 51, GC_GM_QUEST36 = 52, GC_GM_QUEST37 = 53, GC_GM_QUEST38 = 54, GC_GM_QUEST39 = 55, GC_GM_QUEST40 = 56, GC_GM_QUEST41 = 57, GC_GM_QUEST42 = 58, GC_GM_QUEST43 = 59, GC_GM_QUEST44 = 60, GC_GM_QUEST45 = 61, GC_GM_QUEST46 = 62, GC_GM_QUEST47 = 63, GC_GM_QUEST48 = 64, GC_GM_DOTA = 65, GC_GM_AGIT = 66, GC_GM_QUEST49 = 67, GC_GM_QUEST50 = 68, GC_GM_QUEST51 = 69, GC_GM_QUEST52 = 70, GC_GM_QUEST53 = 71, GC_GM_QUEST54 = 72, GC_GM_QUEST55 = 73, GC_GM_QUEST56 = 74, GC_GM_QUEST57 = 75, GC_GM_QUEST58 = 76, GC_GM_AUTOMATCH_TEAM = 77, GC_GM_QUEST59 = 78, GC_GM_QUEST60 = 79, GC_GM_QUEST61 = 80, GC_GM_QUEST62 = 81, GC_GM_QUEST63 = 82, GC_GM_QUEST64 = 83, GC_GM_QUEST65 = 84, GC_GM_QUEST66 = 85, GC_GM_QUEST67 = 86, GC_GM_QUEST68 = 87, GC_GM_QUEST69 = 88, GC_GM_QUEST70 = 89, GC_GM_QUEST71 = 90, GC_GM_QUEST72 = 91, GC_GM_QUEST73 = 92, GC_GM_QUEST74 = 93, GC_GM_QUEST75 = 94, GC_GM_QUEST76 = 95, GC_GM_FATAL_DEATH_TEAM = 96, GC_GM_FATAL_DEATH_SURVIVAL = 97, GC_GM_QUEST77 = 98, GC_GM_QUEST78 = 99, GC_GM_QUEST79 = 100, GC_GM_QUEST80 = 101, GC_GM_QUEST81 = 102, GC_GM_QUEST82 = 103, GC_GM_MONSTER_TEAM = 104, GC_GM_MONSTER_SURVIVAL = 105, GC_GM_QUEST83 = 106
        }

        public struct sSlot
        {
            public bool Active; // 유저가 있나?
            public bool Open; // 열림 닫힘 (유저 있는 경우에도 닫힘)
            public ClientSession cs;
            public int LoadStatus;
            public byte State; // 1레디 3장비 4미션 6스킬트리
            public bool AFK;
            public int Spree;
            public bool Leader;
        }

        public ushort ID;
        public string RoomName;
        public string RoomPass;
        public int GameCategory;
        public int GameMode;
        public int ItemMode;
        public int GameMap;
        public bool RandomMap;
        public sSlot[] Slot = new sSlot[6];
        public int Kick;
        public bool Playing;

        public int FindSlotIndex(ClientSession cs)
        {
            for (int i = 0; i < 6; i++)
                if (Slot[i].cs == cs && Slot[i].Active == true)
                    return i;
            return -1;
        }

        public int GetPlayerCount()
        {
            int temp = 0;
            for (int i = 0; i < 6; i++)
                if (Slot[i].Active == true)
                    temp++;
            return temp;
        }

        public int GetFreeSlot()
        {
            int temp = 0;
            for (int i = 0; i < 6; i++)
                if (Slot[i].Open == true)
                    temp++;
            return temp;
        }

        public int GetTeamByCS(ClientSession cs)
        {
            for( int i = 0; i < 6; i++)
            {
                if (Slot[i].Active == true && Slot[i].cs == cs)
                    return i / 3; // 0~2 = 0, 3~5 = 1
            }
            return 0;
        }

        public int GetSlotPosByCS(ClientSession cs)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Slot[i].Active == true && Slot[i].cs == cs)
                    return i;
            }
            return 0;
        }

        public ClientSession GetRoomLeaderCS()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Slot[i].Leader == true)
                    return Slot[i].cs;
            }
            return null;
        }

        public void SendJoinRoomInfoDivide(ClientSession cs)
        {
            // 방에 있는 유저 정보 불러온다.
            int TempCount = -1;
            for (int i = 0; i < 6; i++)
            {
                // 있는놈 정보만 알려준다.
                if (Slot[i].Active == false)
                    continue;

                TempCount++;

                using (OutPacket op = new OutPacket(GameOpcodes.EVENT_JOIN_ROOM_INFO_DIVIDE_ACK))
                {
                    op.WriteInt(0);
                    op.WriteInt(GetPlayerCount());
                    op.WriteInt(TempCount);
                    op.WriteInt(Slot[i].cs.Login.Length * 2);
                    op.WriteUnicodeString(Slot[i].cs.Login);
                    op.WriteInt(Slot[i].cs.LoginUID);
                    op.WriteInt(Slot[i].cs.Nick.Length * 2);
                    op.WriteUnicodeString(Slot[i].cs.Nick);
                    op.WriteInt(i);
                    op.WriteByte((byte)Slot[i].cs.CurrentChar);
                    op.WriteHexString("00 FF 00 FF 00 FF 00 00 00 00");
                    op.WriteByte((byte)GetTeamByCS(Slot[i].cs));
                    op.WriteHexString("01 00 00 00 0D 00 00 00 00 10 F4 00 00 00 00 00 4E 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 12 00 00 00 00 00 00 00 00 00 00 00 00 00 00 13 00 00 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 00 00 00 00 00 00 00 00 00 00 00 00 00 00 17 00 00 00 00 00 00 00 00 00 00 00 00 00 00 18 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 24 00 00 00 00 00 00 00 00 00 00 00 00 00 00 27 00 00 00 00 00 00 00 00 00 00 00 00 00 00 28 00 00 00 00 00 00 00 00 00 00 00 00 00 00 29 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 2F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 31 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 00 00 00 00 00 00 00 00 00 00 00 33 00 00 00 00 00 00 00 00 00 00 00 00 00 00 34 00 00 00 00 00 00 00 00 00 00 00 00 00 00 35 00 00 00 00 00 00 00 00 00 00 00 00 00 00 36 00 00 00 00 00 00 00 00 00 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00");
                    if (GetRoomLeaderCS() == Slot[i].cs)
                        op.WriteByte(1);
                    else
                        op.WriteByte(0);
                    op.WriteHexString("01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

                    op.WriteByte((byte)Slot[i].cs.MyCharacter.MyChar.Length);
                    //Log.Inform("캐릭터수: {0}", Slot[i].cs.MyCharacter.MyChar.Length);
                    for (int j = 0; j < Slot[i].cs.MyCharacter.MyChar.Length; j++)
                    {
                        //Log.Inform("    슬롯{0} 캐릭터idx{1}", i, j);

                        op.WriteByte((byte)Slot[i].cs.MyCharacter.MyChar[j].CharType);
                        op.WriteInt(0);
                        op.WriteByte((byte)Slot[i].cs.MyCharacter.MyChar[j].Promotion);
                        op.WriteInt(0);
                        op.WriteByte(0);
                        op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].Exp);
                        op.WriteByte(0);
                        op.WriteByte(0);
                        op.WriteByte(0);
                        op.WriteByte((byte)Slot[i].cs.MyCharacter.MyChar[j].Level);
                        op.WriteInt(0);
                        op.WriteInt(0);

                        op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].Equip.Length);
                        for (int k = 0; k < Slot[i].cs.MyCharacter.MyChar[j].Equip.Length; k++)
                        {
                            op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].Equip[k].ItemID);
                            op.WriteInt(1);
                            op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].Equip[k].ItemUID);
                            op.WriteInt(0);
                            op.WriteInt(0);
                            op.WriteInt(0);
                            op.WriteInt(0);
                            op.WriteByte(0);
                            op.WriteByte(0);
                            op.WriteByte(0);
                        }

                        //oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00 00 00 00 00 00 00 00 02 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 01 FF 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                        // 이 패킷이 아래 끝까지 분리됐음. ▼▼
                        op.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 01 00");

                        // 장착한 스킬
                        /*op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].EquipSkill.Length);
                        for (int k = 0; j < Slot[i].cs.MyCharacter.MyChar[j].EquipSkill.Length; j++)
                        {
                            op.WriteInt(0);
                            op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].EquipSkill[k].SkillGroup);
                            op.WriteInt(Slot[i].cs.MyCharacter.MyChar[j].EquipSkill[k].SkillID);
                        }*/
                        op.WriteInt(0);

                        // FF가 스킬포인트일지도.
                        op.WriteHexString("00 00 00 FF 00 00 00 A0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 01 FF 00 00 00 00 01 00");

                        // 배운 스킬
                        /*oPacket.WriteInt(cs.MyCharacter.MyChar[i].MySkill.Length);
                        for (int j = 0; j < cs.MyCharacter.MyChar[i].MySkill.Length; j++)
                        {
                            oPacket.WriteInt(cs.MyCharacter.MyChar[i].MySkill[j].SkillID);
                        }*/
                        op.WriteInt(0);

                        op.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07");
                        // ▲▲
                    }

                    op.WriteHexString("00 00 00 04 13 00 A8 C0 01 EC A8 C0 9B BA FE A9");
                    op.WriteIPFromString(Slot[i].cs.GetIP(), false);
                    op.WriteHexString("00 00 00 01 7E F5 00 00 00");
                    op.WriteByte(Slot[i].State);
                    op.WriteHexString("00 00 00 00 00 00 00 02 00 00 00 00 00 00 E5 6A 00 00 00 01 2C BD 52 5A 00 00 00 00 01 00 00 E5 88 00 00 00 01 2C BD 52 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 56 86 32 00 56 87 6E D4 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

                    op.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                    cs.Send(op);
                }
            }
        }

        public void OnChangeRoomInfo(ClientSession cs, InPacket ip)
        {
            // 보낸놈이 방장인가?
            bool isLeader = false;
            for(int i = 0; i < 6; i++)
            {
                if (Slot[i].Leader == true && Slot[i].cs == cs)
                    isLeader = true;
            }

            // 방장이 아닌데 감히?
            if (isLeader == false)
                return;

            int Flag = ip.ReadInt(); // 1이면 방 슬롯만 변경해준다.
            byte[] Unknown1 = ip.ReadBytes(3);
            int NewGameCategory = ip.ReadByte();
            int NewGameMode = ip.ReadInt();
            int NewItemMode = ip.ReadInt();
            bool NewRandomMap = ip.ReadBool();
            int NewMap = ip.ReadInt();
            int Unknown2 = ip.ReadInt();
            int Unknown3 = ip.ReadInt(); // 이게 0x00일때는 슬롯정보 변경이고 0x58일때는 게임모드 변경이다.
            int Unknown4 = ip.ReadInt();
            int Unknown5 = ip.ReadInt();
            int Unknown6 = ip.ReadInt();
            int Unknown7 = ip.ReadInt();
            int Unknown8 = ip.ReadInt();

            // 방게임모드 변경
            if (Flag == 0)
            {
                GameCategory = NewGameCategory;
                GameMode = NewGameMode;
                ItemMode = NewItemMode;
                RandomMap = NewRandomMap;
                GameMap = NewMap;
            }

            //Log.Inform("게임카테고리: {0}\n게임모드: {1}\n아이템모드: {2}\n랜덤여부: {3}\n맵: {4}", GameCategory, GameMode, ItemMode, RandomMap, GameMap);

            // 바꿀 슬롯 개수
            int ChangeSlotNum = (int)ip.ReadByte();
            byte[] TempSlotInfo = null;
            if (ChangeSlotNum > 0) // 바꿀 슬롯이 있다면.
            {
                //Log.Inform("{0}개의 슬롯변경", ChangeSlotNum);

                // 다시 뿌려주기 위해 슬롯정보를 복사
                TempSlotInfo = BytesUtil.ReadBytes(ip.ToArray(), ip.Position, ChangeSlotNum * 2);

                for (byte i = 0; i < ChangeSlotNum; i++)
                {
                    byte TargetSlot = ip.ReadByte();
                    bool TargetSlotState = ip.ReadBool();

                    Slot[TargetSlot].Open = TargetSlotState;

                    //Log.Inform("    슬롯{0}: {1}", TargetSlot, TargetSlotState);
                }
            }

            // 방 정보 전달
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_CHANGE_ROOM_INFO_BROAD))
            {
                oPacket.WriteHexString("00 00 00 00 00 00 00");
                oPacket.WriteByte((byte)GameCategory);
                oPacket.WriteInt(GameMode);
                oPacket.WriteInt(ItemMode);
                oPacket.WriteBool(RandomMap);
                oPacket.WriteInt(GameMap);
                oPacket.WriteInt(0);
                oPacket.WriteHexString("FF FF FF FF 00 00 00 00 00 00 00");
                oPacket.WriteShort((short)GetPlayerCount());
                oPacket.WriteShort((short)GetFreeSlot());

                for (int i = 0; i < 6; i++)
                    oPacket.WriteBool(Slot[i].Open);

                oPacket.WriteInt(ChangeSlotNum);

                // 변경할 슬롯 정보가 있다면
                if (ChangeSlotNum > 0)
                    oPacket.WriteBytes(TempSlotInfo);

                // 정확하지 않음 1이 잘 되었다고 알려주는거 같은데, 슬롯 변경마다 위치가 다름 개빡;
                if(ChangeSlotNum == 1)
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00");
                else if( ChangeSlotNum == 2)
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00");
                else
                    oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01");

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true) {
                        oPacket.Assemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnChangeUserInfo(ClientSession cs, InPacket ip)
        {
            //LogFactory.GetLog("Main").LogHex("OnChangeUserInfo ", ip.ToArray());
            
            int unknown1 = ip.ReadInt();
            int LoginUID = ip.ReadInt();
            byte flag = ip.ReadByte(); // 4이면 state 변경임
            int LoginLen = ip.ReadInt();
            string Login = ip.ReadUnicodeString(LoginLen);
            int Team = ip.ReadInt();
            int unknown3 = ip.ReadInt();
            byte Character = ip.ReadByte();
            int unknown4 = ip.ReadInt(); // FF 00 FF 00
            int unknown5 = ip.ReadInt(); // FF 00 00 00
            byte unknown6 = ip.ReadByte(); // 00
            byte state = ip.ReadByte(); // 레디/장비/미션/스킬트리 상태

            // 그럴리는 없어야하겠지만;;
            if (Login != cs.Login)
            {
                //Log.Inform("아이디가 이상합니다.\n패킷: {0}\n세션: {1}", Login, cs.Login);
                return;
            }
                

            // 없는 캐릭터를 요청했으므로 무시.
            if (cs.MyCharacter.isHaveChar(Character) == false)
            {
                return;
            }
                
            // 캐릭터 변경한다.
            cs.CurrentChar = Character;
            //Log.Inform("캐릭터 변경: {0}", Character);

            // 만약 요청한 팀이 다른가?
            if (GetTeamByCS(cs) != Team)
            {
                // 팀이 다르므로 자리를 변경해준다.
                int newpos = -1;
                if (Team == 0)
                {
                    for (int i = 0; i <= 2; i++) {
                        if (Slot[i].Active == false && Slot[i].Open == true)
                        {
                            newpos = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 3; i <= 5; i++)
                    {
                        if (Slot[i].Active == false && Slot[i].Open == true)
                        {
                            newpos = i;
                            break;
                        }
                    }
                }

                // 최적의 자리가 없다면 바꿀 수 없으므로 무시
                if (newpos == -1)
                    return;

                // 자리를 찾았으니 자리를 이동한다.
                int oldpos = GetSlotPosByCS(cs); // 내가 있는 슬롯

                // 새로운 자리에 나를 넣고,
                Slot[newpos].Active = Slot[oldpos].Active;
                Slot[newpos].Open = Slot[oldpos].Open;
                Slot[newpos].cs = Slot[oldpos].cs;
                Slot[newpos].Leader = Slot[oldpos].Leader;
                Slot[newpos].LoadStatus = Slot[oldpos].LoadStatus;
                Slot[newpos].State = Slot[oldpos].State;

                // 원래 자리를 비운다.
                Slot[oldpos].Active = false;
                Slot[oldpos].Open = true;
                Slot[oldpos].cs = null;
                Slot[oldpos].Leader = false;
                Slot[oldpos].LoadStatus = 0;
                Slot[oldpos].State = 0;
            }

            // 내가 어디에 있는가?
            int mypos = GetSlotPosByCS(cs);

            // 상태 변경요청이면 상태를 바꾼다
            if (flag == 4)
                Slot[mypos].State = state;

            // 유저 정보가 바뀌었다는 사실을 통보한다
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_CHANGE_ROOMUSER_INFO_BROAD))
            {
                oPacket.WriteInt(0);
                oPacket.WriteInt(cs.LoginUID);
                oPacket.WriteByte(4); // 캐릭터 바꿀때는 0으로 되는데 팀바꿀때는 4임. 0으로하면 카나반 누르면 유저가 안 내려감..
                oPacket.WriteInt(cs.Login.Length * 2);
                oPacket.WriteUnicodeString(cs.Login);
                oPacket.WriteInt(mypos / 3); // 팀번호인데 GetTeam하는거보다 이게 더 빠를듯.
                oPacket.WriteByte(0);
                oPacket.WriteByte(0);
                oPacket.WriteByte(0);
                oPacket.WriteByte((byte)mypos);
                oPacket.WriteByte((byte)cs.CurrentChar);
                oPacket.WriteHexString("FF 00 FF 00 FF 00 00 00 00");
                oPacket.WriteByte(Slot[mypos].State);
                oPacket.WriteByte(0);

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.Assemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        // 방에서 나간다.
        public void ProcessLeaveRoom(ClientSession cs)
        {
            // 내 위치
            int mypos = GetSlotPosByCS(cs);

            // 내가 방장인가?
            bool isLeader = Slot[mypos].Leader;

            // 내 위치 정보 삭제
            Slot[mypos].Active = false;
            Slot[mypos].Open = true;
            Slot[mypos].cs = null;
            Slot[mypos].Leader = false;
            cs.CurrentRoom = null;

            // 사람들이 남아있다면.
            if (GetPlayerCount() > 0)
            {
                // 그놈이 나갔다는 사실을 통보한다.
                using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_INFORM_USER_LEAVE_ROOM_NOT))
                {
                    oPacket.WriteInt(cs.Login.Length * 2);
                    oPacket.WriteUnicodeString(cs.Login);
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(cs.LoginUID);
                    oPacket.WriteInt(3);

                    // 방에 있는 모든 유저에게 전송해야함.
                    for (int i = 0; i < 6; i++)
                    {
                        if (Slot[i].Active == true)
                        {
                            oPacket.CompressAndAssemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                            Slot[i].cs.Send(oPacket); // 패킷 보내고
                            oPacket.CancelAssemble(); // 다시 패킷 복구
                        }
                    }
                }

                // 내가 방장이였다면 새로운 방장을 임명한다.
                if(isLeader == true)
                {
                    int newleaderpos = mypos; // 내 다음이 방장
                    for (int i = 0; i < 6; i++)
                    {
                        newleaderpos++;
                        if (newleaderpos == 6) newleaderpos -= 6; // 6이면 6을 빼준다.
                        if (Slot[newleaderpos].Active == true)
                        {
                            break;
                        }
                    }

                    // 새로운 방장은 언제나 환영이야
                    Slot[newleaderpos].Leader = true;
                    Slot[newleaderpos].State = 0;

                    // 유저놈들한테 방장이 바뀐 사실을 통보한다.
                    using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_HOST_MIGRATED_NOT))
                    {
                        oPacket.WriteInt(Slot[newleaderpos].cs.LoginUID);
                        oPacket.WriteByte(1);

                        // 방에 있는 모든 유저에게 전송해야함.
                        for (int i = 0; i < 6; i++)
                        {
                            if (Slot[i].Active == true)
                            {
                                oPacket.Assemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                                Slot[i].cs.Send(oPacket); // 패킷 보내고
                                oPacket.CancelAssemble(); // 다시 패킷 복구
                            }
                        }
                    }
                }
            }

            // 방에 사람들이 없다면 방을 파괴한다.
            if (GetPlayerCount() == 0)
            {
                lock (cs.CurrentChannel._lock)
                {
                    cs.CurrentChannel.RoomsList.Remove(this);
                    cs.CurrentChannel.RoomsMap.Remove(this.ID);
                }
            }
        }

        public void OnLeaveRoom(ClientSession cs, InPacket ip)
        {
            int Unknown1 = ip.ReadInt(); // 19

            // 방에서 나간다
            ProcessLeaveRoom(cs);

            // 방에서 나갔다는걸 나간놈한테 알려주자.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_LEAVE_ROOM_ACK))
            {
                oPacket.WriteInt(0);

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        // 방에서 나간거랑 거의 비슷하므로 변경시 같이 업데이트.
        public void OnLeaveGame(ClientSession cs, InPacket ip)
        {
            int Unknown1 = ip.ReadInt(); // 19

            // 방에서 나간다
            ProcessLeaveRoom(cs);

            // 게임에서 나갔다는걸 나간놈한테 알려주자.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_LEAVE_GAME_ACK))
            {
                oPacket.WriteInt(0);

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }


        public void StartGame(ClientSession cs, InPacket ip)
        {
            using (OutPacket op = new OutPacket(GameOpcodes.EVENT_START_GAME_BROAD))
            {
                op.WriteInt(0);
                op.WriteHexString("17 51 60 81 00 00 00 00 00 00 00 00 00 00 00 04 15 99 19 DF");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 2B 00 00 00 03 02 00 00 30 CA 00 00 00 01 FF FF FF FF FF FF FF FF 3F 80 00 00 FF FF FF FF 3C 10 24 C1");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 2B 00 00 00 03 02 00 00 E5 92 00 00 00 01 FF FF FF FF FF FF FF FF 3E 99 99 9A FF FF FF FF 3F 46 FC 2B");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 2B 00 00 00 03 02 00 0E 34 2C FF FF FF FF FF FF FF FF FF FF FF FF 3C 36 0B 55 FF FF FF FF 43 DA 0F 3A");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 2B 00 00 00 03 02 00 06 E8 52 FF FF FF FF FF FF FF FF FF FF FF FF 3D 92 49 28 FF FF FF FF 00 00 00 00 00 00 00 12 45 D6 11 30 00 00 00 00 00 00 00 1D 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 47 DD 5B 5C 00 00 00 00 00 00 00 1D 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 4D 55 CD 28 00 00 00 00 00 00 00 1D 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 5F 9F 9C BE 00 00 00 00 00 00 00 1E 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 61 70 F6 7E 00 00 00 00 00 00 00 1E 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 61 9D FF 36 00 00 00 00 00 00 00 1E 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 75 8A 9E D4 00 00 00 00 00 00 00 1F 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF A1 1D CB A3 00 00 00 00 00 00 00 1F 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF A9 15 65 03 00 00 00 00 00 00 00 1F 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF AB 24 4B 51 00 00 00 00 00 00 00 20 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF BB 9B 40 53 00 00 00 00 00 00 00 20 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF CB DE 64 4C 00 00 00 00 00 00 00 20 00 00 00 01 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF CD 9C AB 04 00 00 00 00 00 00 00 21 00 00 00 00 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF DB CC C8 F6 00 00 00 00 00 00 00 21 00 00 00 00 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF DF C2 2F E2 00 00 00 00 00 00 00 21 00 00 00 00 00 00 00 00 01 00 00 00 34 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF E7 EC 32 4A 00 00 00 00 00 00 00 2B 00 00 00 03 00 00 00 00 01 00 00 00 9C 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF E8 9F FD 10 00 00 00 00 00 00 00 2B 00 00 00 03 00 00 00 00 01 00 00 00 9C 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF E9 03 0D 6E 00 00 00 00 00 00 00 2B 00 00 00 03 00 00 00 00 01 00 00 00 9C 00 00 00 00 FF FF FF FF 3E 4C CC CD FF FF FF FF 00 00 00 01");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 05 00 00 00 01");
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 5A 00 00 00 07 00 00 00 00");//00 00 00 5A 00 00 00 07 00 00 00 00
                op.WriteInt(cs.LoginUID);
                op.WriteHexString("00 00 00 00 00 00 00");
                op.WriteByte((byte)GameCategory);//02
                op.WriteInt(GameMode);//00 00 00 09
                op.WriteInt(2);//00 00 00 02
                op.WriteBool(false);//00
                op.WriteInt(GameMap);//00 00 00 00
                op.WriteHexString("00 00 00 00 00 00 00 58 00 00 00 01 00 00 00");
                op.WriteShort((short)GetPlayerCount());//01 01
                op.WriteShort(4);//00 04 
                op.WriteHexString("00 01 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF FF FF");
                op.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(op);
                //LogFactory.GetLog("Main").LogInfo("STATING GAME...");
            }
        }


        public void OnGameStart(ClientSession cs, InPacket ip)
        {
            // 내 위치
            int mypos = GetSlotPosByCS(cs);

            // 방장이 아닌데 게임을 시작하려한다.
            if (Slot[mypos].Leader == false)
                return;

            // 모든 플레이어가 레디상태인가?
            int NeedReady = GetPlayerCount() - 1; // 방장을 제외한 인원 수만큼 레디해야함.
            int NowReady = 0;
            for( int i = 0; i < 6; i++)
                if (Slot[i].State == 1 && Slot[i].Active == true)
                    NowReady++;

            // 준비된 사람이 얼마 없는데..
            if (NeedReady < NowReady)
                return;

            // 게임중인 상태로 변경
            Playing = true;

            // 게임시작했다고 전송
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_START_GAME_BROAD))
            {
                oPacket.WriteInt(0);
                oPacket.WriteHexString("52 3A E9 A2");
                oPacket.Skip(20); // 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                oPacket.WriteInt(GetPlayerCount());
                for (int i = 0; i < 6; i++)
                    if (Slot[i].Active == true)
                        oPacket.WriteInt(Slot[i].cs.LoginUID);
                oPacket.WriteHexString("00 02 46 FE");
                oPacket.WriteInt(GetPlayerCount());
                for (int i = 0; i < 6; i++)
                    if (Slot[i].Active == true)
                    {
                        oPacket.WriteInt(Slot[i].cs.LoginUID);
                        oPacket.WriteHexString("00 00 01 04 00 00 00 6A");
                    }
                oPacket.WriteInt(0);

                oPacket.WriteInt(GetRoomLeaderCS().LoginUID);
                oPacket.Skip(7); // 00 00 00 00 00 00 00

                oPacket.WriteByte((byte)GameCategory);
                oPacket.WriteInt(GameMode);
                oPacket.WriteInt(ItemMode);
                oPacket.WriteBool(RandomMap);
                oPacket.WriteInt(GameMap);
                oPacket.WriteHexString("00 00 00 00 FF FF FF FF 00 00 00 01 00 00 00");
                oPacket.WriteShort((short)GetPlayerCount());
                oPacket.WriteShort((short)GetFreeSlot());
                for (int i = 0; i < 6; i++)
                    if (Slot[i].Active == true)
                        oPacket.WriteBool(Slot[i].Open);
                oPacket.WriteHexString("00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                oPacket.WriteInt(GetPlayerCount());
                for (int i = 0; i < 6; i++)
                    if(Slot[i].Active == true)
                    {
                        oPacket.WriteInt(Slot[i].cs.LoginUID);
                        oPacket.WriteByte((byte)Slot[i].cs.CurrentChar);
                        oPacket.WriteHexString("00 00 03 E8");
                    }
                oPacket.Skip(17); // 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        // 로딩 상태 전송
        public void OnLoadState(ClientSession cs, InPacket ip)
        {
            int ID = ip.ReadInt();
            int LoadPercent = ip.ReadInt();

            for (int i = 0; i < 6; i++)
                if (Slot[i].Active == true)
                    if (Slot[i].cs == cs)
                    {
                        Slot[i].LoadStatus = LoadPercent;
                        //LogFactory.GetLog("Main").LogInfo("Alterando o estado de carregamento do {0}:{1}", i, LoadPercent);
                    }

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_RELAY_LOADING_STATE))
            {
                oPacket.WriteInt(ID);
                oPacket.WriteInt(LoadPercent);

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.Assemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }
        
        public void OnLoadComplete(ClientSession cs, InPacket ip)
        {
            // 방인원 검사해서 로딩 상태 17미만인 사람이 있으면 무시
            // 어짜피 로딩 다 끝나야 오는거라 17이어도 된다.
            for (int i = 0; i < 6; i++)
                if (Slot[i].Active == true)
                    if (Slot[i].LoadStatus < 17)
                    {
                        //Log.Inform("OnLoadComplete - {0}은 아직 로딩되지 않았음. (현재 {1})", i, Slot[i].LoadStatus);
                       // return;
                    }

            //Log.Inform("OnLoadComplete - 로딩 완료 1");

            // 모든 유저가 로딩을 끝냈음을 보낸다.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_LOAD_COMPLETE_BROAD))
            {
                oPacket.WriteInt(0);

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnStageLoadComplete(ClientSession cs, InPacket ip)
        {
            // 방인원 검사해서 로딩 상태 17미만인 사람이 있으면 무시
            // 어짜피 로딩 다 끝나야 오는거라 17이어도 된다.
            for (int i = 0; i < 6; i++)
                if (Slot[i].Active == true)
                    if (Slot[i].LoadStatus < 17)
                    {
                        //Log.Inform("OnStageLoadComplete - {0}은 아직 로딩되지 않았음. (현재 {1})", i, Slot[i].LoadStatus);
            //            return;
                    }

            //Log.Inform("OnStageLoadComplete - 로딩 완료 2");

            // 모든 유저가 로딩을 끝냈음을 보낸다.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_STAGE_LOAD_COMPLETE_BROAD))
            {
                oPacket.WriteInt(0);

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnPingInfo(ClientSession cs, InPacket ip)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_ROOM_MEMBER_PING_INFO_ACK))
            {
                oPacket.WriteInt(GetPlayerCount());
                for (int i = 0; i < 6; i++)
                    if (Slot[i].Active == true)
                    {
                        oPacket.WriteInt(Slot[i].cs.LoginUID);
                        oPacket.WriteInt(0);
                    }

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket); // 패킷 보내고
                oPacket.CancelAssemble(); // 다시 패킷 복구
            }
        }

        public void OnIdleInfo(ClientSession cs, InPacket ip)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_GET_ROOMUSER_IDLE_STATE_ACK))
            {
                oPacket.WriteInt(GetPlayerCount());
                for (int i = 0; i < 6; i++)
                    if (Slot[i].Active == true)
                    {
                        oPacket.WriteInt(Slot[i].cs.LoginUID);

                        // 안되면 엔디안 바꿔주셈 지금 00 00 00 01임.
                        oPacket.WriteByte(0);
                        oPacket.WriteByte(0);
                        oPacket.WriteByte(0);
                        oPacket.WriteBool(Slot[i].AFK);
                    }

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        public void OnGameEnd(ClientSession cs, InPacket ip)
        {
            // 게임 끝났다.
            Playing = false;

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_END_GAME_BROAD))
            {
                oPacket.WriteInt(1); // 00 00 00 01

                oPacket.WriteInt(GetPlayerCount()); // 00 00 00 02
                for( int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == false)
                        continue;

                    oPacket.WriteInt(Slot[i].cs.Login.Length * 2);
                    oPacket.WriteUnicodeString(Slot[i].cs.Login);
                    oPacket.WriteInt(1); // 00 00 00 01
                    oPacket.WriteInt(8295); // 00 00 20 67
                    oPacket.WriteInt(150); // 00 00 00 96
                    oPacket.WriteInt(0); // 00 00 00 00
                    oPacket.WriteInt(4); // 00 00 00 04
                    oPacket.WriteInt(0); // 00 00 00 00
                    oPacket.WriteInt(0); // 00 00 00 00
                    oPacket.WriteHexString("00 00 00 02 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 0E 00 00 00 01 00 00 00 01 0E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 01 18 8C 00 00 00 01 2C DD 78 9F 00 00 00 01 00 00 00 01 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF 00 00 00 00 00 00 00 00 00 00 00 00");
                    oPacket.WriteHexString("00 00 00 4E 00 00 00 07 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 08 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 09 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0A 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0B 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0C 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0D 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0E 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 0F 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 10 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 11 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 12 00 00 00 01 01 01 00 00 00 00 00 00 00 00 00 13 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 14 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 15 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 16 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 17 00 00 00 01 07 01 00 01 00 00 00 00 00 00 00 18 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 19 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 1A 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 1B 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 1D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 1E 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 24 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 27 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 28 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 29 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 2A 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 2B 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 2C 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 2D 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 2E 00 00 00 01 03 01 00 00 00 01 00 00 00 00 00 2F 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 30 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 31 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 32 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 33 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 34 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 35 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 36 00 00 00 01 07 01 00 01 00 02 00 00 00 00 00 37 00 00 00 00 00 00 00 00 00 00 00 00 00 00 38 00 00 00 00 00 00 00 00 00 00 00 00 00 00 39 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 40 00 00 00 00 00 00 00 00 00 00 00 00 00 00 43 00 00 00 00 00 00 00 00 00 00 00 00 00 00 44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 45 00 00 00 00 00 00 00 00 00 00 00 00 00 00 46 00 00 00 00 00 00 00 00 00 00 00 00 00 00 47 00 00 00 00 00 00 00 00 00 00 00 00 00 00 48 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 4F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 51 00 00 00 00 00 00 00 00 00 00 00 00 00 00 52 00 00 00 00 00 00 00 00 00 00 00 00 00 00 53 00 00 00 00 00 00 00 00 00 00 00 00 00 00 54 00 00 00 01 01 00 00 00 00 00 00 00 00 00 00 55 00 00 00 00 00 00 00 00 00 00 00 00 00 00 56 00 00 00 00 00 00 00 00 00 00 00 00 00 00 57 00 00 00 00 00 00 00 00 00 00 00 00 00 00 58 00 00 00 00 00 00 00 00 00 00 00 00 00 00 59 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5E 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 0E 0E 00 00 00 00 00 00 01 CB 00 00 00 00 00 1B 0A F1 00 00 00 18 00 00 00 04 00 00 00 A0 00 00 00 04 00 00 00 A0 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 30 00 00 00 00 00 00 00 00 00 00 01 2C 00 00 00 00 00 00 00 00 00 00 00 00 14 00 07 0D BE 00 00 00 01 00 98 96 81 00 00 00 00 56 76 25 68 56 74 D3 E8 00 00 00 00 00 07 0D C8 00 00 00 01 00 98 96 82 00 00 00 00 56 76 25 68 56 74 D3 E8 00 00 00 00 00 07 0D D2 00 00 00 01 00 98 96 83 00 00 00 00 56 7B 21 D0 56 79 D0 50 00 00 00 00 00 07 0D DC 00 00 00 01 00 98 96 84 00 00 00 00 56 7B 21 D0 56 79 D0 50 00 00 00 00 00 07 19 08 00 00 00 01 00 98 96 81 00 00 00 00 56 76 25 68 56 74 D3 E8 00 00 00 00 00 07 19 12 00 00 00 01 00 98 96 82 00 00 00 00 56 76 25 68 56 74 D3 E8 00 00 00 00 00 07 22 18 00 00 00 01 00 98 97 69 00 00 00 00 56 76 25 E0 56 74 D4 60 00 00 00 00 00 07 22 2C 00 00 00 01 00 98 97 6B 00 00 00 00 56 76 25 E0 56 74 D4 60 00 00 00 00 00 07 22 90 00 00 00 01 00 98 97 75 00 00 00 00 56 76 25 E0 56 74 D4 60 00 00 00 00 00 07 24 52 00 00 00 01 00 98 96 81 00 00 00 01 56 76 25 68 56 74 D3 E8 00 00 00 01 00 07 24 5C 00 00 00 01 00 98 96 82 00 00 00 01 56 76 25 68 56 74 D3 E8 00 00 00 01 00 07 24 8E 00 00 00 01 00 98 96 87 00 00 00 00 56 7E 0A 30 56 7C B8 B0 00 00 00 00 00 07 24 98 00 00 00 01 00 98 96 88 00 00 00 00 56 7E 0A 30 56 7C B8 B0 00 00 00 00 00 07 24 A2 00 00 00 01 00 98 96 89 00 00 00 00 56 7E 0B D4 56 7C BA 54 00 00 00 00 00 07 24 AC 00 00 00 01 00 98 96 8A 00 00 00 00 56 7E 0B D4 56 7C BA 54 00 00 00 00 00 0A E8 58 00 00 00 01 00 98 96 81 00 00 00 00 56 7D 9E 60 56 7C 4C E0 00 00 00 00 00 0A E8 62 00 00 00 01 00 98 96 82 00 00 00 00 56 7D 9E 60 56 7C 4C E0 00 00 00 00 00 0A E8 6C 00 00 00 01 00 98 96 83 00 00 00 00 56 7E 07 9C 56 7C B6 1C 00 00 00 00 00 0A E8 76 00 00 00 01 00 98 96 84 00 00 00 00 56 7E 07 9C 56 7C B6 1C 00 00 00 00 00 12 9D FA 00 00 00 01 00 98 98 0F 00 00 00 01 56 85 69 24 56 84 17 A4 00 00 00 01 00 00 00 01 00 01 18 8C 00 00 00 01 00 00 00 00");
                    oPacket.WriteInt(0);
                    oPacket.WriteInt(75); // 00 00 00 4B
                    oPacket.WriteInt(132); // 00 00 00 84 // 느낌상 다음에 오는게 아이템같음.
                    oPacket.WriteUnicodeString("00 00 78 6E 00 00 00 01 00 00 00 01 00 00 00 14 00 00 00 00 00 00 00 01 00 00 00 14 00 00 00 00 00 00 78 78 00 00 00 01 00 00 00 01 00 00 00 15 00 00 00 00 00 00 00 01 00 00 00 15 00 00 00 00 00 00 78 82 00 00 00 01 00 00 00 01 00 00 00 16 00 00 00 00 00 00 00 01 00 00 00 16 00 00 00 00 00 00 78 8C 00 00 00 01 00 00 00 01 00 00 00 17 00 00 00 00 00 00 00 01 00 00 00 17 00 00 00 00 00 00 78 96 00 00 00 01 00 00 00 01 00 00 00 18 00 00 00 00 00 00 00 01 00 00 00 18 00 00 00 00 00 00 78 A0 00 00 00 01 00 00 00 01 00 00 00 19 00 00 00 00 00 00 00 01 00 00 00 19 00 00 00 00 00 00 78 AA 00 00 00 01 00 00 00 01 00 00 00 1A 00 00 00 00 00 00 00 01 00 00 00 1A 00 00 00 00 00 00 78 B4 00 00 00 01 00 00 00 01 00 00 00 1B 00 00 00 00 00 00 00 01 00 00 00 1B 00 00 00 00 00 00 78 BE 00 00 00 01 00 00 00 01 00 00 00 1C 00 00 00 00 00 00 00 01 00 00 00 1C 00 00 00 00 00 00 78 C8 00 00 00 01 00 00 00 01 00 00 00 1D 00 00 00 00 00 00 00 01 00 00 00 1D 00 00 00 00 00 00 78 D2 00 00 00 01 00 00 00 01 00 00 00 1E 00 00 00 00 00 00 00 01 00 00 00 1E 00 00 00 00 00 00 78 DC 00 00 00 01 00 00 00 01 00 00 00 1F 00 00 00 00 00 00 00 01 00 00 00 1F 00 00 00 00 00 00 78 E6 00 00 00 01 00 00 00 01 00 00 00 20 00 00 00 00 00 00 00 01 00 00 00 20 00 00 00 00 00 00 78 F0 00 00 00 01 00 00 00 01 00 00 00 21 00 00 00 00 00 00 00 01 00 00 00 21 00 00 00 00 00 00 78 FA 00 00 00 01 00 00 00 01 00 00 00 22 00 00 00 00 00 00 00 01 00 00 00 22 00 00 00 00 00 00 79 04 00 00 00 01 00 00 00 01 00 00 00 23 00 00 00 00 00 00 00 01 00 00 00 23 00 00 00 00 00 00 79 0E 00 00 00 01 00 00 00 01 00 00 00 24 00 00 00 00 00 00 00 01 00 00 00 24 00 00 00 00 00 00 79 18 00 00 00 01 00 00 00 01 00 00 00 25 00 00 00 00 00 00 00 01 00 00 00 25 00 00 00 00 00 00 79 22 00 00 00 01 00 00 00 01 00 00 00 26 00 00 00 00 00 00 00 01 00 00 00 26 00 00 00 00 00 00 79 2C 00 00 00 01 00 00 00 01 00 00 00 28 00 00 00 00 00 00 00 01 00 00 00 28 00 00 00 00 00 00 79 36 00 00 00 01 00 00 00 01 00 00 00 2A 00 00 00 00 00 00 00 01 00 00 00 2A 00 00 00 00 00 00 79 40 00 00 00 01 00 00 00 01 00 00 00 2C 00 00 00 00 00 00 00 01 00 00 00 2C 00 00 00 00 00 00 79 4A 00 00 00 01 00 00 00 01 00 00 00 2E 00 00 00 00 00 00 00 01 00 00 00 2E 00 00 00 00 00 00 85 C0 00 00 00 01 00 00 00 01 00 00 00 30 00 00 00 00 00 00 00 01 00 00 00 30 00 00 00 00 00 00 85 CA 00 00 00 01 00 00 00 01 00 00 00 32 00 00 00 00 00 00 00 01 00 00 00 32 00 00 00 00 00 00 85 D4 00 00 00 01 00 00 00 01 00 00 00 42 00 00 00 00 00 00 00 01 00 00 00 42 00 00 00 00 00 00 85 DE 00 00 00 01 00 00 00 01 00 00 00 44 00 00 00 00 00 00 00 01 00 00 00 44 00 00 00 00 00 00 85 E8 00 00 00 01 00 00 00 01 00 00 00 46 00 00 00 00 00 00 00 01 00 00 00 46 00 00 00 00 00 00 85 F2 00 00 00 01 00 00 00 01 00 00 00 48 00 00 00 00 00 00 00 01 00 00 00 48 00 00 00 00 00 00 85 FC 00 00 00 01 00 00 00 01 00 00 00 4A 00 00 00 00 00 00 00 01 00 00 00 4A 00 00 00 00 00 00 86 06 00 00 00 01 00 00 00 01 00 00 00 4C 00 00 00 00 00 00 00 01 00 00 00 4C 00 00 00 00 00 00 86 10 00 00 00 01 00 00 00 01 00 00 00 50 00 00 00 00 00 00 00 01 00 00 00 50 00 00 00 00 00 00 86 1A 00 00 00 01 00 00 00 01 00 00 00 52 00 00 00 00 00 00 00 01 00 00 00 52 00 00 00 00 00 00 86 24 00 00 00 01 00 00 00 01 00 00 00 56 00 00 00 00 00 00 00 01 00 00 00 56 00 00 00 00 00 01 45 8C 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 01 45 96 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 01 45 A0 00 00 00 01 00 00 00 01 00 00 00 02 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 00 00 01 45 AA 00 00 00 01 00 00 00 01 00 00 00 03 00 00 00 00 00 00 00 01 00 00 00 03 00 00 00 00 00 01 45 B4 00 00 00 01 00 00 00 01 00 00 00 04 00 00 00 00 00 00 00 01 00 00 00 04 00 00 00 00 00 01 45 BE 00 00 00 01 00 00 00 01 00 00 00 05 00 00 00 00 00 00 00 01 00 00 00 05 00 00 00 00 00 01 45 C8 00 00 00 01 00 00 00 01 00 00 00 06 00 00 00 00 00 00 00 01 00 00 00 06 00 00 00 00 00 01 45 D2 00 00 00 01 00 00 00 01 00 00 00 07 00 00 00 00 00 00 00 01 00 00 00 07 00 00 00 00 00 01 45 DC 00 00 00 01 00 00 00 01 00 00 00 08 00 00 00 00 00 00 00 01 00 00 00 08 00 00 00 00 00 01 45 E6 00 00 00 01 00 00 00 01 00 00 00 09 00 00 00 00 00 00 00 01 00 00 00 09 00 00 00 00 00 01 45 F0 00 00 00 01 00 00 00 01 00 00 00 0A 00 00 00 00 00 00 00 01 00 00 00 0A 00 00 00 00 00 01 45 FA 00 00 00 01 00 00 00 01 00 00 00 0B 00 00 00 00 00 00 00 01 00 00 00 0B 00 00 00 00 00 01 46 04 00 00 00 01 00 00 00 01 00 00 00 0C 00 00 00 00 00 00 00 01 00 00 00 0C 00 00 00 00 00 01 46 0E 00 00 00 01 00 00 00 01 00 00 00 0D 00 00 00 00 00 00 00 01 00 00 00 0D 00 00 00 00 00 01 46 18 00 00 00 01 00 00 00 01 00 00 00 0E 00 00 00 00 00 00 00 01 00 00 00 0E 00 00 00 00 00 01 46 22 00 00 00 01 00 00 00 01 00 00 00 0F 00 00 00 00 00 00 00 01 00 00 00 0F 00 00 00 00 00 01 46 2C 00 00 00 01 00 00 00 01 00 00 00 10 00 00 00 00 00 00 00 01 00 00 00 10 00 00 00 00 00 01 46 36 00 00 00 01 00 00 00 01 00 00 00 11 00 00 00 00 00 00 00 01 00 00 00 11 00 00 00 00 00 01 46 40 00 00 00 01 00 00 00 01 00 00 00 12 00 00 00 00 00 00 00 01 00 00 00 12 00 00 00 00 00 01 46 4A 00 00 00 01 00 00 00 01 00 00 00 13 00 00 00 00 00 00 00 01 00 00 00 13 00 00 00 00 00 01 9B 18 00 00 00 01 00 00 00 01 00 00 00 27 00 00 00 00 00 00 00 01 00 00 00 27 00 00 00 00 00 01 FF 40 00 00 00 01 00 00 00 01 00 00 00 29 00 00 00 00 00 00 00 01 00 00 00 29 00 00 00 00 00 01 FF 4A 00 00 00 01 00 00 00 01 00 00 00 2B 00 00 00 00 00 00 00 01 00 00 00 2B 00 00 00 00 00 02 1E 9E 00 00 00 01 00 00 00 01 00 00 00 2D 00 00 00 00 00 00 00 01 00 00 00 2D 00 00 00 00 00 02 29 A2 00 00 00 01 00 00 00 01 00 00 00 2F 00 00 00 00 00 00 00 01 00 00 00 2F 00 00 00 00 00 02 41 08 00 00 00 01 00 00 00 01 00 00 00 31 00 00 00 00 00 00 00 01 00 00 00 31 00 00 00 00 00 02 CA 56 00 00 00 01 00 00 00 01 00 00 00 33 00 00 00 00 00 00 00 01 00 00 00 33 00 00 00 00 00 02 CA 60 00 00 00 01 00 00 00 01 00 00 00 34 00 00 00 00 00 00 00 01 00 00 00 34 00 00 00 00 00 02 CA 6A 00 00 00 01 00 00 00 01 00 00 00 35 00 00 00 00 00 00 00 01 00 00 00 35 00 00 00 00 00 02 CA 74 00 00 00 01 00 00 00 01 00 00 00 36 00 00 00 00 00 00 00 01 00 00 00 36 00 00 00 00 00 02 D1 2C 00 00 00 01 00 00 00 01 00 00 00 37 00 00 00 00 00 00 00 01 00 00 00 37 00 00 00 00 00 02 D1 36 00 00 00 01 00 00 00 01 00 00 00 38 00 00 00 00 00 00 00 01 00 00 00 38 00 00 00 00 00 02 D1 40 00 00 00 01 00 00 00 01 00 00 00 39 00 00 00 00 00 00 00 01 00 00 00 39 00 00 00 00 00 02 D1 4A 00 00 00 01 00 00 00 01 00 00 00 3A 00 00 00 00 00 00 00 01 00 00 00 3A 00 00 00 00 00 02 E0 54 00 00 00 01 00 00 00 01 00 00 00 3B 00 00 00 00 00 00 00 01 00 00 00 3B 00 00 00 00 00 02 E0 5E 00 00 00 01 00 00 00 01 00 00 00 3C 00 00 00 00 00 00 00 01 00 00 00 3C 00 00 00 00 00 02 E0 68 00 00 00 01 00 00 00 01 00 00 00 3D 00 00 00 00 00 00 00 01 00 00 00 3D 00 00 00 00 00 02 E0 72 00 00 00 01 00 00 00 01 00 00 00 3E 00 00 00 00 00 00 00 01 00 00 00 3E 00 00 00 00 00 02 E0 7C 00 00 00 01 00 00 00 01 00 00 00 3F 00 00 00 00 00 00 00 01 00 00 00 3F 00 00 00 00 00 02 E0 86 00 00 00 01 00 00 00 01 00 00 00 40 00 00 00 00 00 00 00 01 00 00 00 40 00 00 00 00 00 03 4A 76 00 00 00 01 00 00 00 01 00 00 00 41 00 00 00 00 00 00 00 01 00 00 00 41 00 00 00 00 00 03 4A 80 00 00 00 01 00 00 00 01 00 00 00 43 00 00 00 00 00 00 00 01 00 00 00 43 00 00 00 00 00 03 4A 8A 00 00 00 01 00 00 00 01 00 00 00 45 00 00 00 00 00 00 00 01 00 00 00 45 00 00 00 00 00 03 4A 94 00 00 00 01 00 00 00 01 00 00 00 47 00 00 00 00 00 00 00 01 00 00 00 47 00 00 00 00 00 04 89 86 00 00 00 01 00 00 00 01 00 00 00 49 00 00 00 00 00 00 00 01 00 00 00 49 00 00 00 00 00 04 89 90 00 00 00 01 00 00 00 01 00 00 00 4B 00 00 00 00 00 00 00 01 00 00 00 4B 00 00 00 00 00 05 0F 6E 00 00 00 01 00 00 00 01 00 00 00 4D 00 00 00 00 00 00 00 01 00 00 00 4D 00 00 00 00 00 05 0F 78 00 00 00 01 00 00 00 01 00 00 00 4E 00 00 00 00 00 00 00 01 00 00 00 4E 00 00 00 00 00 05 9A 42 00 00 00 01 00 00 00 01 00 00 00 4F 00 00 00 00 00 00 00 01 00 00 00 4F 00 00 00 00 00 06 E2 3A 00 00 00 01 00 00 00 01 00 00 00 51 00 00 00 00 00 00 00 01 00 00 00 51 00 00 00 00 00 08 33 1A 00 00 00 01 00 00 00 01 00 00 00 53 00 00 00 00 00 00 00 01 00 00 00 53 00 00 00 00 00 08 33 24 00 00 00 01 00 00 00 01 00 00 00 54 00 00 00 00 00 00 00 01 00 00 00 54 00 00 00 00 00 09 54 66 00 00 00 01 00 00 00 01 00 00 00 55 00 00 00 00 00 00 00 01 00 00 00 55 00 00 00 00 00 0A 1E 28 00 00 00 01 00 00 00 01 00 00 00 5F 00 00 00 00 00 00 00 01 00 00 00 5F 00 00 00 00 00 0A 1E 32 00 00 00 01 00 00 00 01 00 00 00 60 00 00 00 00 00 00 00 01 00 00 00 60 00 00 00 00 00 0C 55 08 00 00 00 01 00 00 00 01 00 00 00 61 00 00 00 00 00 00 00 01 00 00 00 61 00 00 00 00 00 0C 55 12 00 00 00 01 00 00 00 01 00 00 00 62 00 00 00 00 00 00 00 01 00 00 00 62 00 00 00 00 00 0D 72 94 00 00 00 01 00 00 00 01 00 00 00 63 00 00 00 00 00 00 00 01 00 00 00 63 00 00 00 00 00 0D 72 9E 00 00 00 01 00 00 00 01 00 00 00 64 00 00 00 00 00 00 00 01 00 00 00 64 00 00 00 00 00 0E E9 E4 00 00 00 01 00 00 00 01 00 00 00 65 00 00 00 00 00 00 00 01 00 00 00 65 00 00 00 00 00 0E E9 EE 00 00 00 01 00 00 00 01 00 00 00 66 00 00 00 00 00 00 00 01 00 00 00 66 00 00 00 00 00 0E E9 F8 00 00 00 01 00 00 00 01 00 00 00 67 00 00 00 00 00 00 00 01 00 00 00 67 00 00 00 00 00 0E EA 02 00 00 00 01 00 00 00 01 00 00 00 68 00 00 00 00 00 00 00 01 00 00 00 68 00 00 00 00 00 0E EA 0C 00 00 00 01 00 00 00 01 00 00 00 6B 00 00 00 00 00 00 00 01 00 00 00 6B 00 00 00 00 00 0E EA 16 00 00 00 01 00 00 00 01 00 00 00 6B 00 00 00 00 00 00 00 01 00 00 00 6B 00 00 00 00 00 0F 85 98 00 00 00 01 00 00 00 01 00 00 00 69 00 00 00 00 00 00 00 01 00 00 00 69 00 00 00 00 00 0F 85 A2 00 00 00 01 00 00 00 01 00 00 00 6A 00 00 00 00 00 00 00 01 00 00 00 6A 00 00 00 00 00 10 49 60 00 00 00 01 00 00 00 01 00 00 00 6C 00 00 00 00 00 00 00 01 00 00 00 6C 00 00 00 00 00 10 49 6A 00 00 00 01 00 00 00 01 00 00 00 6D 00 00 00 00 00 00 00 01 00 00 00 6D 00 00 00 00 00 10 6A 3A 00 00 00 01 00 00 00 01 00 00 00 6E 00 00 00 00 00 00 00 01 00 00 00 6E 00 00 00 00 00 10 6A 44 00 00 00 01 00 00 00 01 00 00 00 6F 00 00 00 00 00 00 00 01 00 00 00 6F 00 00 00 00 00 10 A5 18 00 00 00 01 00 00 00 01 00 00 00 70 00 00 00 00 00 00 00 01 00 00 00 70 00 00 00 00 00 10 A5 22 00 00 00 01 00 00 00 01 00 00 00 71 00 00 00 00 00 00 00 01 00 00 00 71 00 00 00 00 00 10 E6 E0 00 00 00 01 00 00 00 01 00 00 00 72 00 00 00 00 00 00 00 01 00 00 00 72 00 00 00 00 00 10 E6 EA 00 00 00 01 00 00 00 01 00 00 00 73 00 00 00 00 00 00 00 01 00 00 00 73 00 00 00 00 00 12 6A A6 00 00 00 01 00 00 00 01 00 00 00 74 00 00 00 00 00 00 00 01 00 00 00 74 00 00 00 00 00 12 6A B0 00 00 00 01 00 00 00 01 00 00 00 75 00 00 00 00 00 00 00 01 00 00 00 75 00 00 00 00 00 12 6A BA 00 00 00 01 00 00 00 01 00 00 00 76 00 00 00 00 00 00 00 01 00 00 00 76 00 00 00 00 00 12 6A C4 00 00 00 01 00 00 00 01 00 00 00 77 00 00 00 00 00 00 00 01 00 00 00 77 00 00 00 00 00 12 6A CE 00 00 00 01 00 00 00 01 00 00 00 78 00 00 00 00 00 00 00 01 00 00 00 78 00 00 00 00 00 12 6A D8 00 00 00 01 00 00 00 01 00 00 00 79 00 00 00 00 00 00 00 01 00 00 00 79 00 00 00 00 00 12 9F 26 00 00 00 01 00 00 00 01 00 00 00 7A 00 00 00 00 00 00 00 01 00 00 00 7A 00 00 00 00 00 12 9F 30 00 00 00 01 00 00 00 01 00 00 00 7B 00 00 00 00 00 00 00 01 00 00 00 7B 00 00 00 00 00 12 9F 3A 00 00 00 01 00 00 00 01 00 00 00 7C 00 00 00 00 00 00 00 01 00 00 00 7C 00 00 00 00 00 12 9F 44 00 00 00 01 00 00 00 01 00 00 00 7D 00 00 00 00 00 00 00 01 00 00 00 7D 00 00 00 00 00 12 9F 4E 00 00 00 01 00 00 00 01 00 00 00 7E 00 00 00 00 00 00 00 01 00 00 00 7E 00 00 00 00 00 13 8C 24 00 00 00 01 00 00 00 01 00 00 00 7F 00 00 00 00 00 00 00 01 00 00 00 7F 00 00 00 00 00 13 8C 2E 00 00 00 01 00 00 00 00 00 00 00 00 00 13 8C 38 00 00 00 01 00 00 00 01 00 00 00 80 00 00 00 00 00 00 00 01 00 00 00 80 00 00 00 00 00 13 8C 42 00 00 00 01 00 00 00 01 00 00 00 85 00 00 00 00 00 00 00 01 00 00 00 85 00 00 00 00 00 13 8C 4C 00 00 00 01 00 00 00 01 00 00 00 81 00 00 00 00 00 00 00 01 00 00 00 81 00 00 00 00 00 13 8C 56 00 00 00 01 00 00 00 01 00 00 00 86 00 00 00 00 00 00 00 01 00 00 00 86 00 00 00 00 00 13 8C 60 00 00 00 01 00 00 00 01 00 00 00 82 00 00 00 00 00 00 00 01 00 00 00 82 00 00 00 00 00 13 8C 6A 00 00 00 01 00 00 00 01 00 00 00 87 00 00 00 00 00 00 00 01 00 00 00 87 00 00 00 00 00 13 8C 74 00 00 00 01 00 00 00 01 00 00 00 83 00 00 00 00 00 00 00 01 00 00 00 83 00 00 00 00 00 13 8C 7E 00 00 00 01 00 00 00 01 00 00 00 88 00 00 00 00 00 00 00 01 00 00 00 88 00 00 00 00 00 13 8C 88 00 00 00 01 00 00 00 01 00 00 00 84 00 00 00 00 00 00 00 01 00 00 00 84 00 00 00 00 00 13 8C 92 00 00 00 01 00 00 00 01 00 00 00 89 00 00 00 00 00 00 00 01 00 00 00 89 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 00 00 00 00 00 00 00 00 00 14 00 00 00 00");
                }

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnSetPressState(ClientSession cs, InPacket ip)
        {
            int state = ip.ReadInt();
            
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_PRESS_STATE_NOT))
            {
                oPacket.WriteInt(cs.LoginUID);
                oPacket.WriteInt(state);

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (Slot[i].Active == true)
                    {
                        oPacket.Assemble(Slot[i].cs.CRYPT_KEY, Slot[i].cs.CRYPT_HMAC, Slot[i].cs.CRYPT_PREFIX, Slot[i].cs.CRYPT_COUNT);
                        Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }
    }
}
