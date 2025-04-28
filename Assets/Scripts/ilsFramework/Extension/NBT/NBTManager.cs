using System;
using System.Collections.Generic;
using ilsFramework.Core;

namespace ilsFramework.NBT
{
    public class NBTManager : ManagerSingleton<NBTManager>,IManager,IAssemblyForeach
    {
        private SerializerCollection Serializers;
        
        public override void OnInit()
        {
            
        }

        public void ForeachCurrentAssembly(Type[] types)
        {
            List<Type> typesToSerialize = new List<Type>();
            foreach (var type in types)
            {
                if (typeof(ITagSerializer).IsAssignableFrom(type)&& !type.IsInterface && !type.IsAbstract)
                {
                    typesToSerialize.Add(type);
                }
            }
            Serializers = new SerializerCollection();
            Serializers.FillSerializers(typesToSerialize);
        }

        public override void OnUpdate()
        {
            
        }


        public override void OnLateUpdate()
        {
            
        }

        public override void OnLogicUpdate()
        {
            
        }

        public override void OnFixedUpdate()
        {
            
        }
        public override void OnDestroy()
        {
            
        }

        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
            throw new NotImplementedException();
        }

        public SerializerCollection GetSerializers()
        {
            return Serializers;
        }


    }
}