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
    public class Scenario : ISerializable
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string actions { get; set; }
        public string keyword { get; set; }
        public string date { get; set; }

        public Scenario()
        {

        }

        public Scenario(SerializationInfo info, StreamingContext context)
        {
            try
            {
                if (info == null)
                    throw new NotImplementedException();
                this.ID = info.GetInt32("ID");
                this.name = info.GetString("name").DecodeString();
                this.actions = info.GetString("actions").DecodeString();
                this.keyword = info.GetString("keyword").DecodeString();
                this.date = info.GetString("date").DecodeString();
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                if (info == null)
                    throw new NotImplementedException();
                info.AddValue("ID", this.ID);
                info.AddValue("name", this.name.EncodeString());
                info.AddValue("actions", this.actions.EncodeString());
                info.AddValue("keyword", this.keyword.EncodeString());
                info.AddValue("date", this.date.EncodeString());
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }
    }
}
