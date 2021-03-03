#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class ServicesUserServiceWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(Services.UserService);
			Utils.BeginObjectRegister(type, L, translator, 0, 9, 4, 4);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Dispose", _m_Dispose);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ConnectToServer", _m_ConnectToServer);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnGameServerDisconnect", _m_OnGameServerDisconnect);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendLogin", _m_SendLogin);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendRegister", _m_SendRegister);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendCharacterCreate", _m_SendCharacterCreate);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendGameLeave", _m_SendGameLeave);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnGameLeave", _m_OnGameLeave);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SendGameEnter", _m_SendGameEnter);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnLogin", _g_get_OnLogin);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnRegister", _g_get_OnRegister);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnCharacterCreate", _g_get_OnCharacterCreate);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "OnUserGameEnter", _g_get_OnUserGameEnter);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnLogin", _s_set_OnLogin);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnRegister", _s_set_OnRegister);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnCharacterCreate", _s_set_OnCharacterCreate);
            Utils.RegisterFunc(L, Utils.SETTER_IDX, "OnUserGameEnter", _s_set_OnUserGameEnter);
            
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					Services.UserService gen_ret = new Services.UserService();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to Services.UserService constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Dispose(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Dispose(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ConnectToServer(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.ConnectToServer(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnGameServerDisconnect(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _result = LuaAPI.xlua_tointeger(L, 2);
                    string _reason = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.OnGameServerDisconnect( _result, _reason );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendLogin(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _user = LuaAPI.lua_tostring(L, 2);
                    string _psw = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.SendLogin( _user, _psw );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendRegister(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _user = LuaAPI.lua_tostring(L, 2);
                    string _psw = LuaAPI.lua_tostring(L, 3);
                    
                    gen_to_be_invoked.SendRegister( _user, _psw );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendCharacterCreate(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    string _charName = LuaAPI.lua_tostring(L, 2);
                    int _characterClass = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.SendCharacterCreate( _charName, _characterClass );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendGameLeave(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.SendGameLeave(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnGameLeave(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    object _sender = translator.GetObject(L, 2, typeof(object));
                    SkillBridge.Message.UserGameLeaveResponse _response = (SkillBridge.Message.UserGameLeaveResponse)translator.GetObject(L, 3, typeof(SkillBridge.Message.UserGameLeaveResponse));
                    
                    gen_to_be_invoked.OnGameLeave( _sender, _response );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SendGameEnter(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _characterinx = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.SendGameEnter( _characterinx );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnLogin(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnLogin);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnRegister(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnRegister);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnCharacterCreate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnCharacterCreate);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_OnUserGameEnter(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.OnUserGameEnter);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnLogin(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnLogin = translator.GetDelegate<UnityEngine.Events.UnityAction<SkillBridge.Message.Result, string>>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnRegister(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnRegister = translator.GetDelegate<UnityEngine.Events.UnityAction<SkillBridge.Message.Result, string>>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnCharacterCreate(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnCharacterCreate = translator.GetDelegate<UnityEngine.Events.UnityAction<SkillBridge.Message.Result, string, System.Collections.Generic.List<SkillBridge.Message.NCharacterInfo>>>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_OnUserGameEnter(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                Services.UserService gen_to_be_invoked = (Services.UserService)translator.FastGetCSObj(L, 1);
                gen_to_be_invoked.OnUserGameEnter = translator.GetDelegate<UnityEngine.Events.UnityAction<SkillBridge.Message.Result, string, SkillBridge.Message.NCharacterInfo>>(L, 2);
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}
