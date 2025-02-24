using UnityEngine;

namespace ilsFramework
{
    public class Vector2IntParser : Parser<Vector2Int>
    {
        public override bool TryParse(string text, out object value)
        {
            if (text.StartsWith('(') && text.EndsWith(')'))
            {
                var innerText = text.Substring(1, text.Length - 2);
                var values = innerText.Split(',');
                if (values.Length == 2)
                {
                    if (int.TryParse(values[0],out var x) && int.TryParse(values[1],out var y))
                    {
                        value = new Vector2Int(x, y);
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }
    }
}