using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrandChase.IO.Packet;
using GrandChase.Net.Client;
using GrandChase.Security;
using GrandChase.IO;

namespace GrandChase.Function
{
    public class Loading
    {

        string[] InitLoading = new string[4];
        string[] MatchLoading = new string[3];
        string[] SquareLoading = new string[3];
        string[] HackList = new string[0];
        public string[] SHAFileList = new string[0];
        public string GuildMarkURL;

        public Loading()
        {
            InitLoading[0] = "Load1_1.dds";
            InitLoading[1] = "Load1_2.dds";
            InitLoading[2] = "Load1_3.dds";
            InitLoading[3] = "LoadGauge.dds";

            MatchLoading[0] = "ui_match_load1.dds";
            MatchLoading[1] = "ui_match_load2.dds";
            MatchLoading[2] = "ui_match_load3.dds";

            SquareLoading[0] = "Square.lua";
            SquareLoading[1] = "SquareObject.lua";
            SquareLoading[2] = "Square3DObject.lua";

            AddHack("GCMaster.dll");
            AddHack("GCMasterUSA.dll");
            AddHack("GCTrainerDll.dll");
            AddHack("GrandChaseL.dll");
            AddHack("MachineCore2.dll");
            AddHack("PeneLoco.dll");
            AddHack("Pichula.dll");
            AddHack("Pichulon.dll");
            AddHack("main2.dll");
            AddHack("mamawevo.dll");
            AddHack("perro2.dll");

            AddCheckFile("ai.kom");
            AddCheckFile("main.exe");
            AddCheckFile("script.kom");

            GuildMarkURL = "http://127.0.0.1/";
        }

        public void AddHack(string hack)
        {
            Array.Resize<string>(ref HackList, HackList.Length + 1);

            HackList[HackList.Length - 1] = hack;
        }

        public void AddCheckFile(string filename)
        {
            Array.Resize<string>(ref SHAFileList, SHAFileList.Length + 1);

            SHAFileList[SHAFileList.Length - 1] = filename;
        }

        public void NotifyContentInfo(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(CenterOpcodes.ENU_CLIENT_CONTENTS_FIRST_INIT_INFO_ACK))
            {
                oPacket.WriteHexString("00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 01 00 00 00");
                oPacket.WriteHexString("02 00 00 00 03 00 00 00 04 00 00 00 01 00 00 00 00");

                oPacket.WriteInt(InitLoading.Length);
                oPacket.WriteInt(InitLoading[0].Length * 2);
                oPacket.WriteUnicodeString(InitLoading[0]);
                oPacket.WriteInt(InitLoading[1].Length * 2);
                oPacket.WriteUnicodeString(InitLoading[1]);
                oPacket.WriteInt(InitLoading[2].Length * 2);
                oPacket.WriteUnicodeString(InitLoading[2]);
                oPacket.WriteInt(InitLoading[3].Length * 2);
                oPacket.WriteUnicodeString(InitLoading[3]);
                oPacket.WriteHexString("00 00 00 02 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00");

                oPacket.WriteInt(MatchLoading.Length);
                oPacket.WriteInt(MatchLoading[0].Length * 2);
                oPacket.WriteUnicodeString(MatchLoading[0]);
                oPacket.WriteInt(MatchLoading[1].Length * 2);
                oPacket.WriteUnicodeString(MatchLoading[1]);
                oPacket.WriteInt(MatchLoading[2].Length * 2);
                oPacket.WriteUnicodeString(MatchLoading[2]);
                oPacket.WriteInt(0);

                oPacket.WriteInt(SquareLoading.Length);
                oPacket.WriteInt(0);
                oPacket.WriteInt(SquareLoading[0].Length * 2);
                oPacket.WriteUnicodeString(SquareLoading[0]);
                oPacket.WriteInt(1);
                oPacket.WriteInt(SquareLoading[1].Length * 2);
                oPacket.WriteUnicodeString(SquareLoading[1]);
                oPacket.WriteInt(2);
                oPacket.WriteInt(SquareLoading[2].Length * 2);
                oPacket.WriteUnicodeString(SquareLoading[2]);
                oPacket.WriteHexString("00 00 00 03 00 00 00 00 00 00 00 01 00 00 00 02 00 00 00 00");

                oPacket.WriteInt(HackList.Length);
                for( int i = 0; i <= HackList.Length - 1; i++)
                {
                    oPacket.WriteInt(HackList[i].Length * 2);
                    oPacket.WriteUnicodeString(HackList[i]);
                }

                oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);

                cs.Send(oPacket);
            }
        }

        public void NotifySHAFile(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(CenterOpcodes.ENU_SHAFILENAME_LIST_ACK))
            {
                oPacket.WriteInt(0);
                oPacket.WriteInt(SHAFileList.Length);
                for (int i = 0; i <= SHAFileList.Length - 1; i++)
                {
                    oPacket.WriteInt(SHAFileList[i].Length * 2);
                    oPacket.WriteUnicodeString(SHAFileList[i]);
                }

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }
    }
}
