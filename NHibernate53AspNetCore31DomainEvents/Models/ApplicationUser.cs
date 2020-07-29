namespace NHibernate53AspNetCore31DomainEvents.Models
{
    public class ApplicationUser :
        IdentityUser<long>
    {
        private User _user;

        public virtual User User
        {
            get => _user ??= new User(null, null);
            set => _user = value;
        }

        public override string UserName
        {
            get => User.Login;
            set => User.Login = value;
        }

        public override string Email
        {
            get => User.Email;
            set => User.Email = value;
        }

        public override bool LockoutEnabled
        {
            get => User.Active;
            set => User.Active = value;
        }

        public override string PasswordHash
        {
            get => User.Password;
            set => User.Password = value;
        }
    }
}
