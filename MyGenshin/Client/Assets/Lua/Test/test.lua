a = {say = function()
        print("a")
    end}

b = {}

setmetatable(
    b,
    {say = function()
            print("meta")
        end}
)
print(b.say) --nil   如果b中不存在say 并不会调用元表中的say方法 
print('---------------------------')
setmetatable(
    b,
    {__index = function(table, key)
            print(table, key)
        end}
)
print(b.say)  -- 
print('---------------------------')
setmetatable(b, {__index = a})
b.say()
print('---------------------------')
b.say = function()
    print("b")
end

a.say()
b.say()
print('---------------------------')
b={}
c={}
setmetatable(b,{__newindex=c})
b.say = function()
    print("b")
end
print(b.say,c.say)    --nil ,func:say
c.say()      --b
                                        -- 元表中有 __newindex 时赋值只会给指向对象赋值
print('---------------------------')   
b={}
d={}
setmetatable(b,{__index=d,__newindex=d})
b.say = function()
    print("b")
end
print(b.say,d.say)    --func:say ,func:say
d.say()      --b

print('---------------------------')   

e={}
f={f='f'}
g={g='g'}
setmetatable(e,{__index=f})
setmetatable(e,{__index=g})
print(e.f,e.g) -- nil g                -- 不能同时set两个元表
