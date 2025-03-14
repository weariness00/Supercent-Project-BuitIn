using Util;

namespace User
{
    public class UserManager : Singleton<UserManager>
    {
        public UserData userData = new ();
    }
}

