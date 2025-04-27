using System;

namespace ilsFramework.Core
{
    public class ilsStringUtils
    {
        public static (string, string) SplitAtLastSlash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var lastSlashIndex = input.LastIndexOf('/');
            if (lastSlashIndex == -1)
                // 没有斜杠时，返回 [原字符串, 空字符串]
                return (input, string.Empty);

            // 分割为两部分：斜杠前和斜杠后
            var beforeSlash = input.Substring(0, lastSlashIndex);
            var afterSlash = input.Substring(lastSlashIndex + 1);
            return new ValueTuple<string, string>(beforeSlash, afterSlash);
        }
    }
}