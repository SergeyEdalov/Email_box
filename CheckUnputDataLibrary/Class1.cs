namespace CheckUnputDataLibrary
{
    public static class Class1
    {
        public static bool CheckLengthPassword(string password)
        {
            int lengthPassword = password.Length;
            int optimalLength = 6;

            if (lengthPassword < optimalLength) { return false; }

            return true;
        }
        public static bool CheckDifficultPassword(string password)
        {
            if (password.Any(char.IsLetter)
                && password.Any(char.IsDigit)
                && password.Any(char.IsPunctuation)
                && password.Any(char.IsLower)
                && password.Any(char.IsUpper)) { return true; }
            return false;
        }
        public static bool CheckEmail(string email)
        {
            bool isValidFormat = System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return isValidFormat;
        }
    }
}
