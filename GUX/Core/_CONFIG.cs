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
    public class _CONFIG : ISerializable, IDisposable
    {
        public int _SERVER_PORT { get; set; }
        public int _TEST_DRIVER_TIMEOUT { get; set; }
        public int _DRIVER_WAIT_TIMEOUT { get; set; }
        public int _DRIVER_WAIT_POLLING_INTERVAL { get; set; }

        public bool _UI_TOP_MOST { get; set; }
        public bool _UI_TRANSPARENT { get; set; }

        public int _THREADS_CONCURRENCY { get; set; }
        public int _THREADS_INTERVAL { get; set; }

        public string _DRIVE_LETTER { get; set; }
        public string _VMS_DIRECTORY { get; set; }
        public bool _FAILED_AUTO_RETRY { get; set; }
        public int _FAILED_MAX_RETRIES { get; set; }
        public int _FAILED_ACTION_MAX_RETRIES { get; set; }

        public Scenario _DEFAULT_SCENARIO { get; set; }
        public bool _WARMUP_AUTORUN { get; set; }
        public bool _WARMUP_PROCEED_INBOX_FOLDER { get; set; }
        public int _WARMUP_MAX_TREATED_INBOX_EMAILS { get; set; }

        public _CONFIG()
        {

        }

        public _CONFIG(SerializationInfo info, StreamingContext context)
        {
            try
            {
                if (info == null)
                    throw new NotImplementedException();
                this._SERVER_PORT = info.GetInt32("_SERVER_PORT");
                this._DRIVE_LETTER = info.GetString("_DRIVE_LETTER").DecodeString();
                this._TEST_DRIVER_TIMEOUT = info.GetInt32("_TEST_DRIVER_TIMEOUT");
                this._DRIVER_WAIT_TIMEOUT = info.GetInt32("_DRIVER_WAIT_TIMEOUT");
                this._DRIVER_WAIT_POLLING_INTERVAL = info.GetInt32("_DRIVER_WAIT_POLLING_INTERVAL");
                this._UI_TOP_MOST = info.GetBoolean("_UI_TOP_MOST");
                this._UI_TRANSPARENT = info.GetBoolean("_UI_TRANSPARENT");
                this._THREADS_CONCURRENCY = info.GetInt32("_THREADS_CONCURRENCY");
                this._THREADS_INTERVAL = info.GetInt32("_THREADS_INTERVAL");
                this._VMS_DIRECTORY = info.GetString("_VMS_DIRECTORY").DecodeString();
                this._FAILED_AUTO_RETRY = info.GetBoolean("_FAILED_AUTO_RETRY");
                this._FAILED_MAX_RETRIES = info.GetInt32("_FAILED_MAX_RETRIES");
                this._FAILED_ACTION_MAX_RETRIES = info.GetInt32("_FAILED_ACTION_MAX_RETRIES");
                this._DEFAULT_SCENARIO = (Scenario)info.GetValue("_DEFAULT_SCENARIO", typeof(Scenario));
                this._WARMUP_AUTORUN = info.GetBoolean("_WARMUP_AUTORUN");
                this._WARMUP_PROCEED_INBOX_FOLDER = info.GetBoolean("_WARMUP_PROCEED_INBOX_FOLDER");
                this._WARMUP_MAX_TREATED_INBOX_EMAILS = info.GetInt32("_WARMUP_MAX_TREATED_INBOX_EMAILS");
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
                info.AddValue("_SERVER_PORT", this._SERVER_PORT);
                info.AddValue("_DRIVE_LETTER", this._DRIVE_LETTER.EncodeString());
                info.AddValue("_TEST_DRIVER_TIMEOUT", this._TEST_DRIVER_TIMEOUT);
                info.AddValue("_DRIVER_WAIT_TIMEOUT", this._DRIVER_WAIT_TIMEOUT);
                info.AddValue("_DRIVER_WAIT_POLLING_INTERVAL", this._DRIVER_WAIT_POLLING_INTERVAL);
                info.AddValue("_UI_TOP_MOST", this._UI_TOP_MOST);
                info.AddValue("_UI_TRANSPARENT", this._UI_TRANSPARENT);
                info.AddValue("_THREADS_CONCURRENCY", this._THREADS_CONCURRENCY);
                info.AddValue("_THREADS_INTERVAL", this._THREADS_INTERVAL);
                info.AddValue("_VMS_DIRECTORY", this._VMS_DIRECTORY.EncodeString());
                info.AddValue("_FAILED_AUTO_RETRY", this._FAILED_AUTO_RETRY);
                info.AddValue("_FAILED_MAX_RETRIES", this._FAILED_MAX_RETRIES);
                info.AddValue("_FAILED_ACTION_MAX_RETRIES", this._FAILED_ACTION_MAX_RETRIES);
                info.AddValue("_DEFAULT_SCENARIO", this._DEFAULT_SCENARIO, typeof(Scenario));
                info.AddValue("_WARMUP_AUTORUN", this._WARMUP_AUTORUN);
                info.AddValue("_WARMUP_PROCEED_INBOX_FOLDER", this._WARMUP_PROCEED_INBOX_FOLDER);
                info.AddValue("_WARMUP_MAX_TREATED_INBOX_EMAILS", this._WARMUP_MAX_TREATED_INBOX_EMAILS);
            }
            catch (Exception c)
            {
                Console.WriteLine(c.Message);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
