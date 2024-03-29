namespace User.Abstractions
{
    public interface ICheckInputData
    {
        public bool CheckLengthPassword(string password);
        public bool CheckDifficultPassword(string password);
        public bool CheckEmail(string email);
    }
}
