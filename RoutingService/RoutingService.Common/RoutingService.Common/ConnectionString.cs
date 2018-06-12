namespace RoutingService.Common
{
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    [DataContract]
    public class ConnectionString
    {
        #region Fields

        private string _dataSource;
        private string _initialCatalog;
        private string _userId;
        private string _userPassword;

        #endregion

        #region Properties

        [DataMember]
        public string DataSource {
            get => _dataSource;
            private set => _dataSource = value;
        }

        [DataMember]
        public string InitialCatalog
        {
            get => _initialCatalog;
            private set => _initialCatalog = value;
        }

        [DataMember]
        public string UserId
        {
            get => _userId;
            private set => _userId = value;
    }

        [DataMember]
        public string UserPassword
        {
            get => _userPassword;
            private set => _userPassword = value;
        }

        #endregion

        #region Constructors

        public ConnectionString(SqlConnectionStringBuilder connectionString)
        {
            this.DataSource = connectionString.DataSource;
            this.InitialCatalog = connectionString.InitialCatalog;
            this.UserId = connectionString.UserID;
            this.UserPassword = connectionString.Password;
        }

        public ConnectionString(string dataSource, string initialCatalog, string userId, string userPassowrd)
        {
            this.DataSource = dataSource;
            this.InitialCatalog = initialCatalog;
            this.UserId = userId;
            this.UserPassword = userPassowrd;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = this.DataSource,
                InitialCatalog = this.InitialCatalog,
                UserID = this.UserId,
                Password = this.UserPassword
            };

            return builder.ToString();
        }

        #endregion
    }
}