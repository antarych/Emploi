
namespace UserManagement.Domain
{
    public class Contacts
    {
        public Contacts(string _name, string _value)
        {
            name = _name;
            value = _value;
        }

        protected Contacts()
        {
        }
        public  virtual int Id { get; set; }

        public virtual string name { get; set; }

        public virtual string value { get; set; }
    }
}
