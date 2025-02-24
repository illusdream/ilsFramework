namespace ilsFramework
{
    public class FloatParser : Parser<float>
    {
        public override bool TryParse(string text, out object value)
        {
            if (float.TryParse(text, out float result))
            {
                value = result;
                return true;
            }
            value = null;
            return false;
        }
    }
}