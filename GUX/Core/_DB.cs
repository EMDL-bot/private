using GUX.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GUX.Core
{
    [Serializable]
    public class _DB : ISerializable
    {
        public string _SERVER { get; set; }
        public int _PORT { get; set; }
        public string _USER_ID { get; set; }
        public string _PASSWORD { get; set; }
        public string _DATABASE { get; set; }
        public bool _SECURITY { get; set; }
        public bool _SSL { get; set; }
        public bool _TRUST_SERVER_CERTIFICATE { get; set; }

        public _DB()
        {

        }

        public _DB(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            this._SERVER = info.GetString("Server").DecodeString();
            this._PORT = info.GetInt32("Port");
            this._USER_ID = info.GetString("UserID").DecodeString();
            this._PASSWORD = info.GetString("Password").DecodeString();
            this._DATABASE = info.GetString("Database").DecodeString();
            this._SECURITY = info.GetBoolean("Security");
            this._SSL = info.GetString("SSL").DecodeString() == "Required" ;
            this._TRUST_SERVER_CERTIFICATE = info.GetBoolean("TrustedServerCertificate");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("Server", this._SERVER.EncodeString());
            info.AddValue("Port", this._PORT);
            info.AddValue("UserID", this._USER_ID.EncodeString());
            info.AddValue("Password", this._PASSWORD.EncodeString());
            info.AddValue("Database", this._DATABASE.EncodeString());
            info.AddValue("Security", this._SECURITY);
            info.AddValue("SSL", this._SSL ? "Required".EncodeString() : "".EncodeString());
            info.AddValue("TrustedServerCertificate", this._TRUST_SERVER_CERTIFICATE);
        }

        public string ConnectionString()
        {
            return String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};Integrated Security=true;SslMode=Require;Trust Server Certificate=true;",
                        this._SERVER, this._PORT, this._USER_ID, this._PASSWORD, this._DATABASE);
        }
    }
}
