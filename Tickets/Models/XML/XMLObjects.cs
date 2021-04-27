using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tickets.Models.XML
{

    public class TicketAllocationNumber
    {
        public string TiketNumber { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
    }

    public class TicketAllocationNumberExtraordinario
    {
        public string TiketNumber { get; set; }
    }

    public class TicketAllocateXML
    {
        public int RaffleId { get; set; }
        public string RaffleDate { get; set; }
        public string CreateDate { get; set; }
        public string User { get; set; }
        public int Allocation { get; set; }
        public List<TicketAllocationNumberExtraordinario> ticketAllocationNumberExtraordinarios { get; set; }
        public List<TicketAllocationNumber> TicketAllocationNumbers { get; set; }
    }

    public class TicketNumbers
    {
        public string TicketNumber { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public List<Award> Awards { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("TicketNumber")]
    public class TicketNumber
    {
        public string TiketNumber { get; set; }
        public int FractionFrom { get; set; }
        public int FractionTo { get; set; }
        public List<Award> Awards { get; set; }
    }
    public class Award
    {
        public int AwardId { get; set; }
        public string AwardName { get; set; }
        public decimal AwardFractionPrice { get; set; }
        public decimal AwardPrice { get; set; }
    }

    public class AwardNumbesXML
    {
        public int RaffleId { get; set; }
        public string RaffleDate { get; set; }
        public string CreateDate { get; set; }
        public string User { get; set; }
        public List<TicketNumber> TicketNumbers { get; set; }
    }

    [Serializable()]
    public class InvoiceTicketNumber
    {
        [System.Xml.Serialization.XmlElement("TicketNumber")]
        public string TicketNumber { get; set; }

        [System.Xml.Serialization.XmlElement("FractionFrom")]
        public int FractionFrom { get; set; }

        [System.Xml.Serialization.XmlElement("FractionTo")]
        public int FractionTo { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("InvoiceXML")]
    public class InvoiceXML
    {
        [System.Xml.Serialization.XmlElement("RaffleId")]
        public int RaffleId { get; set; }

        [System.Xml.Serialization.XmlElement("RaffleDate")]
        public string RaffleDate { get; set; }


        [XmlArray("InvoiceTicketNumbers")]
        [XmlArrayItem("InvoiceTicketNumber", typeof(InvoiceTicketNumber))]

        public InvoiceTicketNumber[] InvoiceTicketNumbers { get; set; }
    }

    [Serializable()]
    public class TicketNumberAward
    {
        [System.Xml.Serialization.XmlElement("TiketNumber")]
        public string TicketNumber { get; set; }

        [System.Xml.Serialization.XmlElement("FractionFrom")]
        public int FractionFrom { get; set; }

        [System.Xml.Serialization.XmlElement("FractionTo")]
        public int FractionTo { get; set; }
    }

    [Serializable()]
    [System.Xml.Serialization.XmlRoot("TicketPayedXML")]
    public class TicketPayedXML
    {
        [System.Xml.Serialization.XmlElement("RaffleId")]
        public int RaffleId { get; set; }

        [System.Xml.Serialization.XmlElement("RaffleDate")]
        public string RaffleDate { get; set; }

        [XmlArray("TicketNumbers")]
        [XmlArrayItem("TicketNumberAward", typeof(TicketNumberAward))]
        public TicketNumberAward[] TicketNumbers { get; set; }
    }
}