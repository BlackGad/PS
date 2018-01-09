namespace PS.Extensions
{
    public static class StringExtensions
    {
        #region Static members

        public static int Occurrences(this string input, char value)
        {
            var count = 0;
            for (int index = 0; index < input.Length; index++)
            {
                if (input[index] == value) count ++;
            }
            return count;
        }

        #endregion
    }
}