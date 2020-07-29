namespace NHibernate53AspNetCore31DomainEvents.Models
{
    public class User :
        IAudit
    {
        public virtual long Id { get; set; }
        public virtual string Login { get; set; }
        public virtual string Email { get; set; }
        public virtual string Password { get; set; }
        public virtual bool Active { get; set; } = true;

        protected User()
        {
        }

        public User(
            string login,
            string email)
        {
            Login = login;
            Email = email;
        }
    }
}