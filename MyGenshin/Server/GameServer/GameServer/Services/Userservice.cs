using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>,IDisposable
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCharacterCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);

        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserCreateCharacterRequest>(this.OnCharacterCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }
        public void Init()
        {

        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)


        {
            sender.Session.Response.userLogin = new UserLoginResponse();
              
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User & u.Password == request.Passward).FirstOrDefault();
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户名或密码错误";
            }
            else
            {
                sender.Session.TUser = user;
                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo
                {
                    Player = new NPlayerInfo
                    {
                        Id = user.Player.ID
                    },
                    Id = (int)user.ID

                };

                foreach (TCharacter character in user.Player.Characters)
                {
                    NCharacterInfo nCharacterInfo = new NCharacterInfo()
                    {

                        Id = character.ID,
                        Name = character.Name,
                        Class = character.Class,
                        Level = character.Level,
                        ConfigId = character.TID
                        
                    };
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(nCharacterInfo);
                }
               
            }
            sender.SendResponse();
        }
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {

            sender.Session.Response.userRegister = new UserRegisterResponse();
            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在";
            }
            else
            {

                DBService.Instance.Entities.Users.Add(new TUser()
                {
                    Player = new TPlayer(),
                    Username = request.User,
                    Password = request.Passward
                });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }
            sender.SendResponse();
        }
        void OnCharacterCreate(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {

            sender.Session.Response. createChar = new UserCreateCharacterResponse();

            TCharacter character = DBService.Instance.Entities.Characters.Where(c => c.Name == request.Name).FirstOrDefault();
            if (character == null)
            {
                character = new TCharacter()
                {
                    Name = request.Name,
                    Class = (int)request.Class,
                    TID = (int)request.Class,
                    MapID = 1,
                    MapPosX = 32508,
                    MapPosY = 39126,
                    MapPosZ = 2233,
                    MapDirection = 9000,
                    Equiped = new byte[sizeof(int) * 10],
                    Gold = 10000,
                    Level = 1
                };
                character = DBService.Instance.Entities.Characters.Add(character);
                sender.Session.TUser.Player.Characters.Add(character);
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.createChar.Result = Result.Success;
                sender.Session.Response.createChar.Errormsg = "None";
                foreach (TCharacter chr in sender.Session.TUser.Player.Characters)
                {
                    NCharacterInfo nCharacterInfo = new NCharacterInfo()
                    {

                        Id = chr.ID,
                        Name = chr.Name,
                        Class = chr.Class,
                        Level=chr.Level
                    };
                    sender.Session.Response.createChar.Characters.Add(nCharacterInfo);
                }

            }
            else
            {
                sender.Session.Response.createChar.Result = Result.Failed;
                sender.Session.Response.createChar.Errormsg = "重复的角色名";

            }

            sender.SendResponse();
        }
        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter tchr = sender.Session.TUser.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("OnGameEnter TCharacterId: {0}", tchr.ID);
            Character character = CharacterManager.Instance.AddCharacter(tchr);

            // character.itemManager.AddItem(1, 1);

            sender.Session.Response.gameEnter = new UserGameEnterResponse()
            {
                Result = Result.Success,
                Errormsg = "None",
                Character = character.Info
            };
            SessionManager.Instance.AddSession(character.Id, sender);
            sender.SendResponse();
            Log.InfoFormat("OnGameEnter CharacterId: {0}", character.Id);
            MapManager.Instance[tchr.MapID].CharacterEnter(sender, character);
            sender.Session.Character = character;
            sender.Session.postProcesser = character;

        }
        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            
            if (sender.Session.Character == null) return;
            //Log.InfoFormat("OnGameLeave Character TUser:{0} in Map{1}", sender.Session.TUser.ID, sender.Session.Character.Info.mapId);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse()
            {
                Result = Result.Success,
                Errormsg = "None"

            };
            SessionManager.Instance.RemoveSession(sender.Session.Character.Id);
            sender.SendResponse();
            MapManager.Instance[sender.Session.Character.Info.mapId].CharacterLeave(sender.Session.Character);
            CharacterManager.Instance.RemoveCharacter(sender.Session.Character.Id);
            sender.Session.Character = null;
            

        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave： characterID:{0}:{1}", character.Id, character.Info.Name);
            CharacterManager.Instance.RemoveCharacter(character.Data.ID);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }


    }
}
