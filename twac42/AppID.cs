using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace twac42
{
    class AppID
    {
        public AppID(string filename)
        {
            Key = "<NO KEY>";
            Secret = "<NO SECRET>";
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.CloseInput = true;
            using (XmlReader reader = XmlReader.Create(filename, settings)) {
                reader.MoveToContent();
                reader.ReadStartElement("AppID");
                Key = reader.ReadElementContentAsString("Key", "");
                Secret = reader.ReadElementContentAsString("Secret", "");
                reader.ReadEndElement();
            }
        }

        public string Key { get; private set; }
        public string Secret { get; private set; }
    }
}
