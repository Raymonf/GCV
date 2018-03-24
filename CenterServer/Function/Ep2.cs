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
    public class Ep2
    {
        public void EnuClientPing(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(CenterOpcodes.ENU_SHAFILENAME_LIST_ACK))
            {
                oPacket.WriteInt(0);
                oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        public void ClientsContent(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(CenterOpcodes.ENU_CLIENT_PING_CONFIG_ACK))
            {
                oPacket.WriteHexString("00 00 03 E8 00 00 0F A0 00 00 03 E8 00 00 00 01 01 FF FF FF FF");
                oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

    }
}
