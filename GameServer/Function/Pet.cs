﻿using System;
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
    public class Pet
    {
        public struct sPet
        {
            public int PetUID;
            public int PetItemID;
            public int Exp;
            public int Health;
            public string Name;
            public int Slot1;
            public int Slot2;
            public int Level;
            public int Bind;
            public bool Evo;
        }

        public struct sTransformInfo
        {
            public int PetItemID;
            public int NextPetItemID;
        }

        public sPet[] pet = new sPet[0];
        public sTransformInfo[] transforminfo = new sTransformInfo[0];

        public void AddPetTrnasformInfo(int PetItemID, int NextPetItemID)
        {
            Array.Resize(ref transforminfo, transforminfo.Length + 1);
            transforminfo[transforminfo.Length].PetItemID = PetItemID;
            transforminfo[transforminfo.Length].NextPetItemID = NextPetItemID;
        }

        public int GetPetTransformInfo(int PetItemID)
        {
            for(int i = 0; i < transforminfo.Length; i++)
            {
                if (transforminfo[i].PetItemID == PetItemID)
                    return transforminfo[i].NextPetItemID;
            }


            return -1;
        }

        public void LoadPet(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT PetUID, PetItemID, Exp, Health, Name, Slot1, Slot2, Level, Bind, Evo FROM pet WHERE LoginUID = '{0}'", cs.LoginUID);

            // 인벤토리 크기 조정
            Array.Resize(ref pet, ds.Tables[0].Rows.Count);

            // 인벤토리 삽입
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                pet[i].PetUID = Convert.ToInt32(ds.Tables[0].Rows[i][0].ToString());
                pet[i].PetItemID = Convert.ToInt32(ds.Tables[0].Rows[i][1].ToString());
                pet[i].Exp = Convert.ToInt32(ds.Tables[0].Rows[i][2].ToString());
                pet[i].Health = Convert.ToInt32(ds.Tables[0].Rows[i][3].ToString());
                pet[i].Name = ds.Tables[0].Rows[i][4].ToString();
                pet[i].Slot1 = Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString());
                pet[i].Slot2 = Convert.ToInt32(ds.Tables[0].Rows[i][6].ToString());
                pet[i].Level = Convert.ToInt32(ds.Tables[0].Rows[i][7].ToString());
                pet[i].Bind = Convert.ToInt32(ds.Tables[0].Rows[i][8].ToString());
                pet[i].Evo = Convert.ToBoolean(Convert.ToInt32(ds.Tables[0].Rows[i][9].ToString()));
            }
        }
    }
}
