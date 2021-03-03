using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using Entities;
using Managers;

namespace Services
{
    [XLua.LuaCallCSharp]
    public class UserService : Singleton<UserService>, IDisposable
    {

        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string, List<NCharacterInfo>> OnCharacterCreate;
        public UnityEngine.Events.UnityAction<Result, string, NCharacterInfo> OnUserGameEnter;
        //public UnityEngine.Events.UnityAction<Result, string> OnMapEnter;
        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {

            Debug.Log("UserService Start");
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCharacterCreate);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
        }



        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }


        public void ConnectToServer()
        {
            
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if (this.pendingMessage != null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    //MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result, string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin != null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if (this.pendingMessage.Request.userRegister != null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.userLogin = new UserLoginRequest();
            netMessage.Request.userLogin.User = user;
            netMessage.Request.userLogin.Passward = psw;
            SendMessage(netMessage);
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {//登陆成功逻辑
                User.Instance.SetupUserInfo(response.Userinfo);
            };
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);

            }
        }


        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.userRegister = new UserRegisterRequest();
            netMessage.Request.userRegister.User = user;
            netMessage.Request.userRegister.Passward = psw;

            SendMessage(netMessage);
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);

            }
        }

        public void SendCharacterCreate(string charName, int characterClass)
        {
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest
                {
                    createChar = new UserCreateCharacterRequest()

                }
            };
            netMessage.Request.createChar.Class = characterClass;
            netMessage.Request.createChar.Name = charName;

            SendMessage(netMessage);
        }


        void OnUserCharacterCreate(object sender, UserCreateCharacterResponse response)
        {
           
            Debug.LogFormat("OnUserCharacterCreater:{0} [{1}]", response.Result, response.Errormsg);
            if (OnCharacterCreate != null)
            {
                User.Instance.Info.Player.Characters.Clear();
                User.Instance.Info.Player.Characters.AddRange(response.Characters);
                OnCharacterCreate(response.Result, response.Errormsg, response.Characters);
            }


        }

        public void SendGameLeave()
        {
            Debug.LogFormat("SendGameLeave:{0}", User.Instance.CurrentCharacter.Id);
            NetMessage netMessage = new NetMessage()
            {
                Request = new NetMessageRequest()
                {

                    gameLeave = new UserGameLeaveRequest()
                }
            };
            SendMessage(netMessage);
        }

        public void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            Debug.LogFormat("OnGameLeave:{0} [{1}]", response.Result, response.Errormsg);
        }
        public void SendGameEnter(int characterinx)
        {
            Debug.LogFormat("SendGameEnter:{0}", characterinx);           
            NetMessage netMessage = new NetMessage
            {
                Request = new NetMessageRequest
                {
                    gameEnter = new UserGameEnterRequest
                    {
                        characterIdx = characterinx
                    }
                }
            };
            this.SendMessage(netMessage);
        }

        private void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if (OnUserGameEnter != null)
            {
                OnUserGameEnter(response.Result, response.Errormsg, response.Character);
            }
            ItemManager.Instance.Init(response.Character);
            EquipManager.Instance.Init(response.Character);
            QuestManager.Instance.QuestInit(response.Character);
        }


        void SendMessage(NetMessage message)
        {
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

    }
}
