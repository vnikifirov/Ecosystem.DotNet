namespace Users.Web.MVC.Models
{
    // Class container to hold a Radom User.
    public class User
    {
        private string picture;
        private string name;
        private string email;
        private string phone;
        private string dob;

        public string Picture
        {
            get { return picture; }
            set { picture = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Dob
        {
            get { return dob; }
            set { dob = value; }
        }
    }
}