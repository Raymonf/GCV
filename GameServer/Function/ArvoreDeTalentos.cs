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
    public class ArvoreDeTalentos
    {
        public int skillid = 2;
        public int SP = 2;

        public void LoadSkill(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT   `skillID` FROM  `skilltree` WHERE LoginUID = '{0}' AND Login = '{1}' ", cs.LoginUID, cs.Login);
            skillid = Convert.ToInt32(ds.Tables[0].Rows[0]["skillID"].ToString());

            DataSet ds2 = new DataSet();
            Database.Query(ref ds2, "SELECT   `SP` FROM  `SPLeft` WHERE LoginUID = '{0}' ", cs.LoginUID);
            SP = Convert.ToInt32(ds2.Tables[0].Rows[0]["SP"].ToString());

            LogFactory.GetLog("INFOS ST").LogInfo("SP: " + SP + " SKILL: " + skillid);
        }


        public void setSkill(ClientSession cs, InPacket ip)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_SKILL_TRAINING_ACK))
            {
                skillid = ip.ReadInt();
                oPacket.WriteInt(0);
                oPacket.WriteInt(skillid);
                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
                /*Database.Query(ref ds, "SELECT  `skillID` FROM  `skilltree`", cs.LoginUID,cs.Login);
                skill2 = Convert.ToInt32(ds.Tables[0].Rows[0]["skillID"].ToString());
                if (skill2 == skillid)
                {
                    return;                        
                }
                else
                {*/
                DataSet ds2 = new DataSet();
                Database.Query(ref ds2, "INSERT INTO `skilltree` (`LoginUID`, `Login`, `skillID`) VALUES  ('{0}', '{1}', '{2}')", cs.LoginUID, cs.Login, skillid);
                //}
            }
        }

    }
}
