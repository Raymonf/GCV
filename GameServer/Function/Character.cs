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
    public class Character
    {
        public struct sEquip
        {
            public int ItemUID;
            public int ItemID;
        }

        public struct sSkill
        {
            public int SkillGroup;
            public int SkillID;
        }

        public struct sChar
        {
            public int CharType;
            public int Promotion;
            public int Exp;
            public int Level;
            public bool WeaponChange;
            public int WeaponChangeID;
            public int Pet;
            public int Win;
            public int Loss;
            public sEquip[] Equip;
            public sSkill[] MySkill;
            public sSkill[] EquipSkill;
        }

        public sChar[] MyChar = new sChar[0];

        public bool isHaveChar(int findCharType)
        {
            for (int i = 0; i < MyChar.Length; i++)
            {
                if (MyChar[i].CharType == findCharType)
                    return true;
            }

            return false;
        }

        public int findCharIndex(int findCharType)
        {
            for (int i = 0; i < MyChar.Length; i++)
            {
                if (MyChar[i].CharType == findCharType)
                    return i;
            }

            return -1;
        }

        public void LoadCharacter(ClientSession cs)
        {
            DataSet ds = new DataSet();
            Database.Query(ref ds, "SELECT * FROM `character` WHERE LoginUID = '{0}' ORDER BY CharType ASC", cs.LoginUID);

            // 캐릭터 배열 사이즈 늘리기
            Array.Resize(ref MyChar, ds.Tables[0].Rows.Count);

            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                MyChar[i].CharType = Convert.ToInt32(ds.Tables[0].Rows[i]["CharType"].ToString());
                MyChar[i].Promotion = Convert.ToInt32(ds.Tables[0].Rows[i]["Promotion"].ToString());
                MyChar[i].Exp = Convert.ToInt32(ds.Tables[0].Rows[i]["Exp"].ToString());
                MyChar[i].Level = Convert.ToInt32(ds.Tables[0].Rows[i]["Level"].ToString());
                MyChar[i].WeaponChange = Convert.ToInt32(ds.Tables[0].Rows[i]["WeaponChange"].ToString()) == 1 ? true : false;
                MyChar[i].WeaponChangeID = Convert.ToInt32(ds.Tables[0].Rows[i]["WeaponChangeID"].ToString());
                MyChar[i].Pet = Convert.ToInt32(ds.Tables[0].Rows[i]["Pet"].ToString());

                // 장착중인 장비
                DataSet ds2 = new DataSet();
                Database.Query(ref ds2, "SELECT * FROM `equipment` WHERE LoginUID = '{0}' AND CharType = '{1}'", cs.LoginUID, MyChar[i].CharType);

                // 장비 구조체 사이즈 조정
                Array.Resize(ref MyChar[i].Equip, ds2.Tables[0].Rows.Count);
                for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                {
                    MyChar[i].Equip[j].ItemUID = Convert.ToInt32(ds2.Tables[0].Rows[j]["ItemUID"].ToString());
                    MyChar[i].Equip[j].ItemID = Convert.ToInt32(ds2.Tables[0].Rows[j]["ItemID"].ToString());
                }

                // 장착스킬, 배운스킬 구조체 임시로 0으로 세팅
                Array.Resize(ref MyChar[i].EquipSkill, 0);
                Array.Resize(ref MyChar[i].MySkill, 0);
            }
        }

        public void OnEquipItem(ClientSession cs, InPacket ip)
        {
            int LoginIDLen = ip.ReadInt();
            string LoginID = ip.ReadUnicodeString(LoginIDLen);
            ip.ReadInt(); // 00 00 00 00
            byte CharNum = ip.ReadByte();
            for(byte i = 0; i < CharNum; i++)
            {
                byte TargetChar = ip.ReadByte();
                int EquipCount = ip.ReadInt();

                int MyCharPos = -1;
                for (int t = 0; t < cs.MyCharacter.MyChar.Length; t++)
                    if (cs.MyCharacter.MyChar[t].CharType == TargetChar)
                        MyCharPos = t;

                // 내가 가진 캐릭터 목록에 없다
                if (MyCharPos == -1)
                    continue;

                Array.Resize(ref cs.MyCharacter.MyChar[MyCharPos].Equip, EquipCount);

                for(int j = 0; j < EquipCount; j++)
                {
                    int ItemID = ip.ReadInt();
                    ip.ReadInt(); // 00 00 00 01
                    int ItemUID = ip.ReadInt();
                    ip.ReadInt(); // 00 00 00 00
                    ip.ReadInt(); // 00 00 00 00
                    ip.ReadInt(); // 00 00 00 00
                    ip.ReadInt(); // 00 00 00 00
                    ip.ReadByte(); // 00 00 00
                    ip.ReadByte(); //
                    ip.ReadByte(); //

                    cs.MyCharacter.MyChar[MyCharPos].Equip[j].ItemID = ItemID;
                    cs.MyCharacter.MyChar[MyCharPos].Equip[j].ItemUID = ItemUID;
                }

                // 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                ip.Skip(99);
            }

            // 패킷 구조 똑같이 보내면 된다.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_EQUIP_ITEM_BROAD))
            {
                oPacket.WriteInt(cs.Login.Length * 2);
                oPacket.WriteUnicodeString(cs.Login);
                oPacket.WriteInt(0);
                oPacket.WriteByte((byte)cs.MyCharacter.MyChar.Length);

                for( int i = 0; i < cs.MyCharacter.MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                    oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip.Length);
                    for( int j = 0; j < cs.MyCharacter.MyChar[i].Equip.Length; j++)
                    {
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemID);
                        oPacket.WriteInt(1);
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemUID);
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteByte(); // 00 00 00
                        oPacket.WriteByte(); //
                        oPacket.WriteByte(); // 
                    }

                    // 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                    oPacket.Skip(99);
                }

                oPacket.WriteInt(0); // 그냥

                oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, ++cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }

            // 내가 지금 방에 입장하고 있으면 방 인원한테 장비가 바뀌었다고 알려준다.
            if (cs.CurrentRoom == null)
                return;

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_EQUIP_ITEM_BROAD))
            {
                oPacket.WriteInt(cs.Login.Length);
                oPacket.WriteUnicodeString(cs.Login);

                oPacket.WriteByte(2); // ???

                oPacket.WriteInt(cs.MyCharacter.MyChar.Length);
                for (int i = 0; i < cs.MyCharacter.MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                    oPacket.WriteInt(0); // 00 00 00 00
                    oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip.Length);
                    for (int j = 0; j < cs.MyCharacter.MyChar[i].Equip.Length; j++)
                    {
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemID);
                        oPacket.WriteInt(1);
                        oPacket.WriteInt(cs.MyCharacter.MyChar[i].Equip[j].ItemUID);
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteInt(); // 00 00 00 00
                        oPacket.WriteByte(); // 00 00 00
                        oPacket.WriteByte(); //
                        oPacket.WriteByte(); // 
                    }

                    oPacket.Skip(61);
                    oPacket.WriteHexString("FF FF");
                    oPacket.Skip(32);
                    oPacket.WriteInt(cs.LoginUID);
                }

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (cs.CurrentRoom.Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(cs.CurrentRoom.Slot[i].cs.CRYPT_KEY, cs.CurrentRoom.Slot[i].cs.CRYPT_HMAC, cs.CurrentRoom.Slot[i].cs.CRYPT_PREFIX, cs.CurrentRoom.Slot[i].cs.CRYPT_COUNT);
                        cs.CurrentRoom.Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnChangeEquipInRoom(ClientSession cs, InPacket ip)
        {
            // 방에서 장비가 변경됐다. 방이 아니면 패킷 무시
            if (cs.CurrentRoom == null)
                return;

            // 일단 장비 바꾼놈이 있다는 사실을 알려준다
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_CHANGE_LOOK_EQUIP_NOT))
            {
                oPacket.WriteInt(cs.LoginUID);
                oPacket.WriteInt(cs.MyCharacter.MyChar.Length);

                for (int i = 0; i < cs.MyCharacter.MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)cs.MyCharacter.MyChar[i].CharType);
                    oPacket.WriteInt(0); // 00 00 00 00
                }

                // 방에 있는 모든 유저에게 전송해야함.
                for (int i = 0; i < 6; i++)
                {
                    if (cs.CurrentRoom.Slot[i].Active == true)
                    {
                        oPacket.CompressAndAssemble(cs.CurrentRoom.Slot[i].cs.CRYPT_KEY, cs.CurrentRoom.Slot[i].cs.CRYPT_HMAC, cs.CurrentRoom.Slot[i].cs.CRYPT_PREFIX, cs.CurrentRoom.Slot[i].cs.CRYPT_COUNT);
                        cs.CurrentRoom.Slot[i].cs.Send(oPacket); // 패킷 보내고
                        oPacket.CancelAssemble(); // 다시 패킷 복구
                    }
                }
            }
        }

        public void OnTrainSkill(ClientSession cs, InPacket ip)
        {
            // 스킬을 배운다. 일단은 그냥 배우게 허락한다.
            int SkillID = ip.ReadInt();

            // 배운 스킬 목록에 넣는다. (캐릭터 구분이 현재 불가능하므로 모든캐릭터에 넣어버린다...)
            for(int i = 0; i < MyChar.Length; i++)
            {
                Array.Resize(ref MyChar[i].MySkill, MyChar[i].MySkill.Length + 1);
                MyChar[i].MySkill[MyChar[i].MySkill.Length - 1].SkillID = SkillID;
            }

            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_SKILL_TRAINING_ACK))
            {
                oPacket.WriteInt(0); // 성공 여부인가봄
                oPacket.WriteInt(SkillID);

                oPacket.Assemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }

        public void OnSetSkill(ClientSession cs, InPacket ip)
        {
            //Log.Hex("패킷", ip.ToArray());

            int unknown1 = ip.ReadInt();
            int LoginUID = ip.ReadInt();
            int UnknownCharNum = ip.ReadInt();
            for(int i = 0; i < UnknownCharNum; i++)
            {
                int c = ip.ReadByte(); // 캐릭터 번호
                byte u1= ip.ReadByte(); // ???
                int u2 = ip.ReadInt(); // ???
                LogFactory.GetLog("Main").LogInfo("    {0} / {1} / {2}", c, u1, u2);
            }

            int CharNum = ip.ReadInt();
            for (int i = 0; i < CharNum; i++)
            {
                int CharType = ip.ReadByte(); // 캐릭터 번호
                ip.ReadByte(); // ?
                ip.ReadInt(); // ? 00 00 00 02
                ip.ReadInt(); // ? 00 00 00 00

                int SkillCount = ip.ReadInt();

                LogFactory.GetLog("Main").LogInfo("Caracteres {0} / {1} habilidades do número", CharType, SkillCount);

                int CharPos = findCharIndex(CharType); // 캐릭터가 있는 배열 위치
                // 캐릭터가 있으면 동작
                if(CharPos != -1)
                {
                    Array.Resize(ref MyChar[CharPos].EquipSkill, SkillCount);

                    // 스킬 개수만큼 배열에 담는다
                    for (int j = 0; j < SkillCount; j++)
                    {
                        ip.ReadInt(); // ? 00 00 00 00
                        int SkillGruop = ip.ReadInt(); // 아마도 스킬 그룹일 가능성 높음.
                        int SkillID = ip.ReadInt();

                        MyChar[CharPos].EquipSkill[j].SkillGroup = SkillGruop;
                        MyChar[CharPos].EquipSkill[j].SkillID = SkillID;
                    }
                }

                ip.ReadInt(); // 00 00 00 01
                ip.ReadInt(); // 00 00 00 00
            }

            // 패킷은 똑같이 보내면 된다.
            using (OutPacket oPacket = new OutPacket(GameOpcodes.EVENT_SET_SKILL_BROAD))
            {
                oPacket.WriteInt(0);

                byte[] data = ip.ToArray();
                data = BytesUtil.ReadBytes(data, 4, data.Length - 4);

                oPacket.WriteBytes(data);

                /*
                oPacket.WriteInt(0); // ??
                oPacket.WriteInt(LoginUID);

                oPacket.WriteInt(MyChar.Length);
                for (int i = 0; i < MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)i); // 캐릭터 번호
                    oPacket.WriteByte(0); // ???
                    oPacket.WriteInt(); // ???
                }

                oPacket.WriteInt(MyChar.Length);
                for (int i = 0; i < MyChar.Length; i++)
                {
                    oPacket.WriteByte((byte)MyChar[i].CharType);
                    oPacket.WriteByte(0);
                    oPacket.WriteInt(2);
                    oPacket.WriteInt(0);

                    oPacket.WriteInt(MyChar[i].EquipSkill.Length);
                    for (int j = 0; j < MyChar[i].EquipSkill.Length; j++)
                    {
                        oPacket.WriteInt(0);
                        oPacket.WriteInt(MyChar[i].EquipSkill[j].SkillGroup);
                        oPacket.WriteInt(MyChar[i].EquipSkill[j].SkillID);
                    }

                    oPacket.WriteInt(1);
                    oPacket.WriteInt(0);
                }
                */

                oPacket.CompressAndAssemble(cs.CRYPT_KEY, cs.CRYPT_HMAC, cs.CRYPT_PREFIX, cs.CRYPT_COUNT);
                cs.Send(oPacket);
            }
        }
    }
}
 