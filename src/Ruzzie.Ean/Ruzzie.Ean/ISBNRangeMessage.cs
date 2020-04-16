// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Ruzzie.Common.Types;

namespace Ruzzie.Ean
{
    namespace ISBN
    {
        [XmlRoot(ElementName = "Rule")]
        public class Rule
        {
            [XmlElement(ElementName = "Range")] public string Range { get; set; }= string.Empty;
            [XmlElement(ElementName = "Length")] public int Length { get; set; }
        }

        [XmlRoot(ElementName = "Rules")]
        public class Rules
        {
            [XmlElement(ElementName = "Rule")] public List<Rule>? Rule { get; set; }
        }

        [XmlRoot(ElementName = "EAN.UCC")]
        public class EAN_UCC
        {
            [XmlElement(ElementName = "Prefix")] public string Prefix { get; set; }= string.Empty;
            [XmlElement(ElementName = "Agency")] public string Agency { get; set; }= string.Empty;
            [XmlElement(ElementName = "Rules")] public Rules? Rules { get; set; }
        }

        [XmlRoot(ElementName = "EAN.UCCPrefixes")]
        public class EAN_UCCPrefixes
        {
            [XmlElement(ElementName = "EAN.UCC")] public List<EAN_UCC>? EAN_UCC { get; set; }
        }

        [XmlRoot(ElementName = "Group")]
        public class Group
        {
            [XmlElement(ElementName = "Prefix")] public string Prefix { get; set; } = string.Empty;
            [XmlElement(ElementName = "Agency")] public string Agency { get; set; } = string.Empty;
            [XmlElement(ElementName = "Rules")] public Rules? Rules { get; set; }
        }

        [XmlRoot(ElementName = "RegistrationGroups")]
        public class RegistrationGroups
        {
            [XmlElement(ElementName = "Group")] public List<Group>? Group { get; set; }
        }

        [XmlRoot(ElementName = "ISBNRangeMessage")]
        public class ISBNRangeMessage
        {
            [XmlElement(ElementName = "MessageSource")]
            public string MessageSource { get; set; } = string.Empty;

            [XmlElement(ElementName = "MessageSerialNumber")]
            public string MessageSerialNumber { get; set; } = string.Empty;

            [XmlElement(ElementName = "MessageDate")]
            public string MessageDate { get; set; } = string.Empty;

            [XmlElement(ElementName = "EAN.UCCPrefixes")]
            public EAN_UCCPrefixes? EAN_UCCPrefixes { get; set; }

            [XmlElement(ElementName = "RegistrationGroups")]
            public RegistrationGroups? RegistrationGroups { get; set; }

            private static XmlSerializer ISBNRangeMessageSerializer = new XmlSerializer(typeof(ISBNRangeMessage));

            public static ISBNRangeMessage LoadFromFile(string filename)
            {
                using var streamReader = File.OpenText(filename);
                return (ISBNRangeMessage) ISBNRangeMessageSerializer.Deserialize(streamReader);
            }

           public static Result<Err<LoadErrorKind>, ISBNRangeMessage> LoadFromEmbeddedResource()
           {
               string resourceName = "Ruzzie.Ean.ISBNRangeMessage.xml";
               try
               {
                   using var stream = typeof(ISBNRangeMessage).Assembly.GetManifestResourceStream(resourceName);
                   if (stream == null)
                   {
                       return new Err<LoadErrorKind>(
                           $"Resource not found. Returned Stream from GetManifestResourceStream for resource {resourceName} returned null.",
                           LoadErrorKind.ResourceStreamIsNull);
                   }

                   using var streamReader = new StreamReader(stream);
                   return (ISBNRangeMessage) ISBNRangeMessageSerializer.Deserialize(streamReader);
               }
               catch (Exception e)
               {
                   return new Err<LoadErrorKind>(
                       $"Unexpected exception occurred while loading the ISBNRangeMessage: {e.Message} for resource {resourceName}",
                       LoadErrorKind.UnexpectedError);
               }
           }

           public enum LoadErrorKind
           {
               ResourceStreamIsNull,
               UnexpectedError
           }
        }
    }
}