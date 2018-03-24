using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GrandChase.IO.Packet;
using GrandChase.Net.Client;
using GrandChase.Security;
using GrandChase.IO;
using GrandChase.Data;
using GrandChase.Utilities;
using Manager.Factories;


namespace GrandChase.Function
{
    public class moedas
    {
        public int virtualCash;

        public void LoadVP(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT  `Cash` FROM  `currentcashvirtual` WHERE LoginUID = '{0}' AND Login = '{1}'", cs.LoginUID, cs.Login);
            virtualCash = Convert.ToInt32(ds.Tables[0].Rows[0]["Cash"].ToString());
            LogFactory.GetLog("Virtual Cash: ").LogInfo("" + virtualCash);
            if (ds.Tables[0].Rows.Count == 0)
            {
                virtualCash = 1;
            }
        }

        public void CurrentVirtualCash(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_CURRENT_VIRTUAL_CASH_NOT))
            {
                oPacket.WriteInt(virtualCash);
                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }


    }
}
