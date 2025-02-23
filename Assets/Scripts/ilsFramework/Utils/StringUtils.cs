using System;

namespace ilsFramework
{
    public class StringUtils
    {
        public static (string,string) SplitAtLastSlash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            int lastSlashIndex = input.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                // 没有斜杠时，返回 [原字符串, 空字符串]
                return ( input, string.Empty );
            }
            else
            {
                // 分割为两部分：斜杠前和斜杠后
                string beforeSlash = input.Substring(0, lastSlashIndex);
                string afterSlash = input.Substring(lastSlashIndex + 1);
                return new  (beforeSlash, afterSlash);
            }
        }
    }
}