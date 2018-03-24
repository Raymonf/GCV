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

namespace GrandChase.Function
{
    public class Inventory
    {
        public struct sInventory
        {
            public int ItemUID;
            public int ItemID;
            public int Quantity;
        }

        public sInventory[] inventory = new sInventory[0];

        public void LoadInventory(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT ItemUID, ItemID, Quantity FROM inventory WHERE LoginUID = '{0}'", cs.LoginUID);

            // 인벤토리 크기 조정
            Array.Resize(ref inventory, ds.Tables[0].Rows.Count);

            // 인벤토리 삽입
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                inventory[i].ItemUID = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                inventory[i].ItemID = Convert.ToInt32(ds.Tables[0].Rows[i][1].ToString());
                inventory[i].Quantity = Convert.ToInt32(ds.Tables[0].Rows[i][2].ToString());
            }
        }

        public void SendInventory(ClientSession cs)
        {
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_STAT_USER_HISTORY_NOT))
            {
                oPacket.WriteHexString("00 00 00 01 00 00 00 01");

                oPacket.WriteInt(inventory.Length);
                for (int i = 0; i < inventory.Length; i++)
                {
                    oPacket.WriteInt(inventory[i].ItemID); // ItemID
                    oPacket.WriteHexString("00 00 00 01");
                    oPacket.WriteInt(inventory[i].ItemUID); // ItemUID
                    oPacket.WriteInt(inventory[i].Quantity); // Quantity
                    oPacket.WriteInt(inventory[i].Quantity); // Quantity

                    // 아래 내용은 추후 알아내는대로 바꿀것.
                    oPacket.WriteHexString("00 00 00 00 00 00 FF FF FF FF 00 00 00 00 56 76 0D AA 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
                }

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        public int FindItemUIDbyID(int ID)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ItemID == ID)
                    return inventory[i].ItemUID;
            }
            return 0;
        }

        public int FindItemIDbyUID(int UID)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                if (inventory[i].ItemUID == UID)
                    return inventory[i].ItemID;
            }
            return 0;
        }
    }
}
