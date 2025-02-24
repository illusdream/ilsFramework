namespace ilsFramework
{
    public class IntParser : Parser<int>
    {
        public override bool TryParse(string text, out object value)
        {
            value = default(int);
            if (int.TryParse(text,out var result))
            {
                value = result;
                return true;
            }
            return false;
        }
    }
}