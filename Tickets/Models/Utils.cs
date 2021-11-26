using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Tickets.Filters;
using Tickets.Models.Enums;

namespace Tickets.Models
{
    [InitializeSimpleMembership]
    public static class Utils
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector,
        IEqualityComparer<TKey> comparer)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>(comparer);
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 20, 00, 00);

        public static string GetNumberUnit(long number)
        {
            var numberString = number.ToString();
            if (numberString.Length <= 1)
            {
                return "UNID";
            }
            else if (numberString.Length == 2)
            {
                return "DEC";
            }
            else if (numberString.Length == 3)
            {
                return "CENT";
            }
            else if (numberString.Length == 4)
            {
                var n = numberString.Substring(0, 1);
                return n + " MIL";
            }
            else if (numberString.Length == 5)
            {
                var n = numberString.Substring(0, 2);
                return n + " MIL";
            }
            else
            {
                return "";
            }
        }

        public static bool IdentifyBachIsPayedMinor(IdentifyBach identifyBach, List<RaffleAward> awards)
        {
            decimal totalAwardValue = 0;
            decimal totalPaymentValue = 0;
            int percentMayorista = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista ? 2 : 0;
            identifyBach.IdentifyNumbers.AsEnumerable().Join(awards.Where(a =>
               a.Award.TypesAwardId != (int)AwardTypeEnum.Mayors
               && a.Award.TypesAwardId != (int)AwardTypeEnum.WinFraction).AsEnumerable(), i => i.TicketAllocationNumber.Number, a => a.ControlNumber, (i, a) => new
               {
                   FractionTo = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionTo,
                   FractionFrom = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionFrom,
                   AwardValue = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Award.Value : a.Award.Value / (a.Raffle.Prospect.LeafNumber * a.Raffle.Prospect.LeafFraction)
               }).ToList().ForEach(n =>
                    totalAwardValue += ((n.FractionTo - n.FractionFrom + 1) + n.AwardValue));
            identifyBach.IdentifyBachPayments.AsEnumerable().ToList().ForEach(p => totalPaymentValue += p.Value);
            identifyBach.NoteCredits.AsEnumerable().ToList().ForEach(n => totalPaymentValue += n.TotalCash);
            return totalPaymentValue >= (totalAwardValue + (totalAwardValue * percentMayorista / 100)) && totalPaymentValue > 0;
        }

        public static bool IdentifyBachIsPayedMayor(IdentifyBach identifyBach, List<RaffleAward> awards)
        {
            decimal totalAwardValue = 0;
            decimal totalPaymentValue = 0;
            int percentMayorista = identifyBach.Client.GroupId == (int)ClientGroupEnum.Mayorista ? 2 : 0;
            identifyBach.IdentifyNumbers.AsEnumerable().Join(awards.Where(a =>
                a.Award.TypesAwardId == (int)AwardTypeEnum.Mayors
                || a.Award.TypesAwardId == (int)AwardTypeEnum.WinFraction).AsEnumerable(),
                i => i.TicketAllocationNumber.Number, a => a.ControlNumber, (i, a) => new
                {
                    FractionTo = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionTo,
                    FractionFrom = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Fraction : i.FractionFrom,
                    AwardValue = a.Award.ByFraction == (int)ByFractionEnum.S ? a.Award.Value : a.Award.Value / (a.Raffle.Prospect.LeafNumber * a.Raffle.Prospect.LeafFraction)
                }).ToList().ForEach(n =>
                totalAwardValue += ((n.FractionTo - n.FractionFrom + 1) + n.AwardValue));
            identifyBach.IdentifyBachPayments.AsEnumerable().ToList().ForEach(p => totalPaymentValue += p.Value);
            identifyBach.NoteCredits.AsEnumerable().ToList().ForEach(n => totalPaymentValue += n.TotalCash);
            return totalPaymentValue >= (totalAwardValue + (totalAwardValue * percentMayorista / 100)) && totalPaymentValue > 0;
        }

        public static long ToUnixTime(this DateTime dateTime)
        {
            dateTime = dateTime.AddDays(1);
            return (dateTime - UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static List<List<int>> SplitList(List<int> locations, int nSize = 30)
        {
            List<List<int>> list = new List<List<int>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static string SendMail(string subject, string body, string[] to)
        {
            MailMessage mail = new MailMessage();
            string error = "";
            foreach (string sentTo in to)
            {
                try
                {
                    MailAddress e = new MailAddress(sentTo);
                    mail.To.Add(sentTo);
                }
                catch
                {
                    error += sentTo + ", ";
                };
            }
            if (error != "")
            {
                return error;
            }
            try
            {
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);
                return error;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static string AddZeroToNumber(int lenght, int number)
        {
            string stringNumber = number.ToString();
            while (stringNumber.Length < lenght)
            {
                stringNumber = "0" + stringNumber;
            }
            return stringNumber;
        }

        public static void SaveLog(string userName, LogActionsEnum action, string module, object details = null)
        {
            var data = "";
            try
            {
                if (details != null)
                {
                    data = Newtonsoft.Json.JsonConvert.SerializeObject(details);
                }
            }
            catch { }
            var db = new TicketsEntities();
            var log = new Log()
            {
                Action = (int)action,
                Module = module,
                UserName = userName,
                CreateDate = DateTime.Now,
                Datos = data
            };
            db.Logs.Add(log);
            db.SaveChanges();
        }
    }
}
