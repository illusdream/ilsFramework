namespace ilsFramework
{
    //最搞笑的东西，但是没办法，因为我没做特殊处理
    public class StringParser : Parser<string>
    {
        public override bool TryParse(string text, out object value)
        {
            value = text;
            return true;
        }
    }
}