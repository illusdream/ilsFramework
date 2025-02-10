using System;

namespace ilsFramework
{
    public class TestResourceReferenceMaker : IResourceReferenceMaker
    {
        public string[] GetTargetResourceFileExtensions()
        {
           return new string[0];
        }

        public Type[] GetTargetResourceTypes()
        {
           return new Type[0];
        }

        public void AddTargetResourceInfo(ResouceInfo resouceInfo)
        {
           
        }

        public void CreateReferenceFile()
        {
            
        }
    }
}