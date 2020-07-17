using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OFTENCOFTAPI.Services.SendMail
{
    public class Attachment
    {
        public Attachment(string fileName, object content)
        {
            this.Content = content;
            this.FileName = fileName;
            this.Type = AttachmentType.Text;
        }

        public enum AttachmentType
        {
            Json,
            Text
        }

        public object Content { get; set; }
        public string FileName { get; set; }
        public AttachmentType Type { get; set; }

        public async Task<MemoryStream> ContentToStreamAsync()
        {
            string text;

            switch (Type)
            {
                case AttachmentType.Json:
                    text = Newtonsoft.Json.JsonConvert.SerializeObject(this.Content);
                    break;
                case AttachmentType.Text:
                    text = this.Content.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8);
            await writer.WriteAsync(text);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }
    }
}
