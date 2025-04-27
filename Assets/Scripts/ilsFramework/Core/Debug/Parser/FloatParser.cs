namespace ilsFramework.Core
{
    public class FloatParser : Parser<float>
    {
        public override bool TryParse(string text, out object value)
        {
            if (float.TryParse(text, out var result))
            {
                value = result;
                return true;
            }

            value = null;
            return false;
        }
    }
}