using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Models
{
    public class Logger
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public Logger(IHostingEnvironment hostingEnvironment) {
            _hostingEnvironment = hostingEnvironment;
        }
        static Logger() => new Logger(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "NationalGiveaway Logs"), "").Start();
        //static Logger() => new Logger(Path.Combine(Environment.GetFolderPath(AppDomain.CurrentDomain.BaseDirectory), "NationalGiveaway Logs"), "").Start();

        private static Logger Instance { get; set; }
        private static bool _isFirstInstance = true;
        private const string Tag = "Logger";
        private bool Started { get; set; }
        private Thread Thread { get; set; }
        private ConcurrentQueue<LogMessage> Queue { get; set; }
        private string LogDir { get; }
        private DateTime Today { get; set; }

        private bool Deep { get; }

        //private static string Path { get; set; }
        private string Prefix { get; }
        private Logger(string dir, string prefix, bool deep = true)
        {
            Deep = deep;
            LogDir = string.IsNullOrWhiteSpace(dir) ? "../Logs" : dir;
            Prefix = string.IsNullOrWhiteSpace(prefix) ? "" : prefix;
            CheckDirectory(DateTime.Today);

            if (_isFirstInstance)
            {
                Queue = new ConcurrentQueue<LogMessage>();
                _isFirstInstance = false;
                Queue.Enqueue(LogMessage.Log("Logger Initialized.", Tag));
            }
            else
                throw new Exception("Logger Instance Already Created.");

            Instance = this;
        }

        private void CheckDirectory(DateTime date)
        {
            var path = $"{LogDir}/{(Deep ? date.ToString("yyyy/MM MMMM/") : string.Empty)}";
            if (!string.IsNullOrWhiteSpace(path) && !Directory.Exists(path))
                Directory.CreateDirectory(path);
            Today = date.Date;
        }

        private void Start()
        {
            if (Started) return;

            Started = true;
            Queue.Enqueue(LogMessage.Log("Logger Started.", Tag));
            Thread = new Thread(() =>
            {
                while (Started)
                {
                    while (Queue.TryDequeue(out var message))
                        PersistLog(message);
                }

                //Remove thread reference so you don't have object leak
                Thread = null;
                _isFirstInstance = true;
            });
            Thread.Start();
        }

        public void Stop()
        {
            //Nothing to do
            if (!Started) return;
            //            lock (Queue)
            //            {
            if (Queue.Any())
                PersistLog(LogMessage.Log($"Shutting down, Dumping Pending {Queue.Count} Messages.", Tag));

            Started = false;
            //You will only reach this code if you are exiting the program
            //This block ensures the messages that were queued up will get dumped
            PurgeLog();
            //            }
        }

        public static void LogInfo(string text, string tag = null) => Instance?.Log(text, tag);
        public static void LogError(Exception ex) => Instance.Log(ex, null);

        public static void LogInfo(string text, string tag, bool logInConsole)
        {
            if (logInConsole)
                Console.WriteLine(LogMessage.Log(text, tag).ToString());
            Instance?.Log(text, tag);
        }

        public static void LogError(Exception ex, string tag, bool logInConsole)
        {
            if (logInConsole)
                Console.WriteLine(LogMessage.Log(ex.StackTrace, tag).ToString());
            Instance.Log(ex, tag);
        }

        #region Private Instance]

        private void Log(string text, string tag = null) => Queue.Enqueue(LogMessage.Log(text, tag));

        //        private void Log(object obj, string tag = null)
        //        {
        //            try
        //            {
        //                var t = (from prop in obj.GetType().GetRuntimeProperties()
        //                         where prop.CanRead
        //                         select prop)
        //                        .Aggregate("{", (current, prop) => (current == null ? null : current + ",") + $"\"{prop.Name}\": {prop.GetValue(obj)}") + "}";
        //                Log(t, tag);
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e);
        //            }
        //        }
        private void Log(Exception ex, string tag = null) => Log(ex.ToString(), tag);

        private void PersistLog(LogMessage message)
        {
            try
            {
                if (message == null) return;
                var path = $"{LogDir}/{(Deep ? message.Day.ToString("yyyy/MM MMMM/") : string.Empty)}";
                if (Today.Date != message.Day.Date)
                    CheckDirectory(DateTime.Today);
                var file = Path.Combine(path, Prefix + message.Day.ToString("yyyy-MM-dd") + ".txt");

                File.AppendAllText(file, $"{message}\n");
            }
            catch
            {
                //ignore
            }
        }

        private void PurgeLog()
        {
            try
            {
                if (!Queue.Any()) return;
                var items = Queue.ToArray().GroupBy(x => x.Day);
                Queue = new ConcurrentQueue<LogMessage>();
                foreach (var day in items)
                {
                    CheckDirectory(day.Key);
                    File.AppendAllLines(Path.Combine(LogDir, Prefix + day.Key.ToString("yyyy-MM-dd") + ".txt"),
                        day.Select(x => $"{x}"));
                }
            }
            catch
            {
                //ignore
            }
        }

        private class LogMessage
        {
            private DateTime Time { get; } = DateTime.Now;
            public DateTime Day => Time.Date;
            private string Text { get; set; }
            private string Id { get; set; }

            public static LogMessage Log(string text, string tag = null) => new LogMessage
            {
                Text = text,
                Id = tag
            };

            public override string ToString() =>
                $"{Time:hh:mm:sstt}\t{(string.IsNullOrWhiteSpace(Id) ? string.Empty : $"{Id}:\t")}{Text}";
        }

        #endregion
    }


    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Logger.LogInfo("Request:");
            Logger.LogInfo(request.ToString());
            if (request.Content != null)
            {
                Logger.LogInfo(await request.Content.ReadAsStringAsync());
            }

            //string Url = System.Web.HttpUtility.UrlDecode(request.RequestUri.ToString());
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            Logger.LogInfo("Response:");
            Logger.LogInfo(response.ToString());
            if (response.Content != null)
            {
                Logger.LogInfo(await response.Content.ReadAsStringAsync());
            }

            return response;
        }
    }
}