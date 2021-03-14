using Common.Data;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            userInfo = info;
        }


        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

        public MapDefine CurrentMap { get; set; }


        public GameObject CurrentCharacterObject { get; set; }


        public int CurrentID { get => CurrentCharacter.Id; }
        public int CharacterLv { get => CurrentCharacter.Level; set => CurrentCharacter.Level = value; }
        public void Reset()
        {
            CharacterLv =1;
            CurrentCharacter = null;
            CurrentMap = null;
            CurrentCharacterObject = null;
        }
    }
}
