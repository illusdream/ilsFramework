using UnityEngine;

namespace ilsFramework
{
    public class Vector3Parser : Parser<Vector3>
    {
        public override bool TryParse(string text, out object value)
        {
            if (text.StartsWith('(') && text.EndsWith(')'))
            {
                var innerText = text.Substring(1, text.Length - 2);
                var values = innerText.Split(',');
                if (values.Length == 3)
                {
                    if (float.TryParse(values[0],out var x) && float.TryParse(values[1],out var y) && float.TryParse(values[2],out var z))
                    {
                        value = new Vector3(x,y,z);
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }
    }
}