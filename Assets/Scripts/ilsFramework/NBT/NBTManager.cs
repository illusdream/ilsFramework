using System;
using System.Collections.Generic;

namespace ilsFramework
{
    public class NBTManager : ManagerSingleton<NBTManager>,IManager,IAssemblyForeach
    {
        private SerializerCollection Serializers;
        public void Init()
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
        public void Update()
        {
            
        }

        public void LateUpdate()
        {
            
        }

        public void FixedUpdate()
        {
            
        }

        public void OnDestroy()
        {
            
        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            
        }

        public SerializerCollection GetSerializers()
        {
            return Serializers;
        }


    }
}