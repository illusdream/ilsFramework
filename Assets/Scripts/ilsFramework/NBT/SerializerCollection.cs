using System;
using System.Collections.Generic;

namespace ilsFramework
{
    public class SerializerCollection
    {
       Dictionary<string,ITagSerializer> IDtoTagSerializers = new Dictionary<string, ITagSerializer>();
       Dictionary<Type,ITagSerializer> TypeToTagSerializers = new Dictionary<Type,ITagSerializer>();
       public void FillSerializers(List<Type> SerializerTypes)
       {
           IDtoTagSerializers.Clear();
           TypeToTagSerializers.Clear();

           foreach (var type in SerializerTypes)
           {
               if (type != null)
               {
                   var instance = Activator.CreateInstance(type) as ITagSerializer;
                   if (instance == null)
                   {
                       continue;
                   }
                   IDtoTagSerializers.Add(instance.TargetType.FullName,instance);
                   TypeToTagSerializers.Add(instance.TargetType, instance);
               }
           }
       }

       public bool TryGetTagSerializer(string type, out ITagSerializer tagSerializer)
       {
           return IDtoTagSerializers.TryGetValue(type, out tagSerializer);
       }

       public bool TryGetTagSerializer<T>( out ITagSerializer tagSerializer,out string type)
       {
           type = String.Empty;
           if (TypeToTagSerializers.TryGetValue(typeof(T), out tagSerializer))
           {
               type = typeof(T).FullName;
               return true;
           }
           return false;
       }
    }
}