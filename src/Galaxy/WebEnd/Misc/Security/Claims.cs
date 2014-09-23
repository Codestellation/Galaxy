namespace Codestellation.Galaxy.WebEnd.Misc.Security
{
    public static class Claims
    {
        private const string AdminClaim = "Admin";

        public static readonly string[] User = new string[0];
        public static readonly string[] Admin = { AdminClaim };
    }
}