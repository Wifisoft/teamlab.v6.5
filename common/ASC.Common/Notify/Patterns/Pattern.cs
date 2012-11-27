using System;

namespace ASC.Notify.Patterns
{
    public class Pattern : IPattern
    {
        public const string HTMLContentType = "html";

        public const string TextContentType = "text";

        public const string RtfContentType = "rtf";


        public string ID { get; private set; }

        public string Name { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public string ContentType { get; internal set; }

        public string Styler { get; internal set; }

        
        public Pattern(string id, string name, string subject, string body, string contentType)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("id");
            if (subject == null) throw new ArgumentNullException("subject");
            if (body == null) throw new ArgumentNullException("body");
            ID = id;
            Name = name;
            Subject = subject;
            Body = body;
            ContentType = string.IsNullOrEmpty(contentType) ? TextContentType : contentType;
        }


        public override bool Equals(object obj)
        {
            var p = obj as IPattern;
            return p != null && p.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", ID, Name);
        }
    }
}