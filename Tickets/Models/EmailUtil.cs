using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using Tickets.Controllers;
using Tickets.Models.Enums;


namespace Tickets.Models
{
    public class EmailUtil
    {

        public EmailUtil()
        {
            ///  SendReports();
        }

        private static readonly Timer emailTimer = new Timer();
        //intervalo de tiempo en el cual se ejecutara la tarea
        // private static readonly double executeTime = 1000 * 60 * 60 * 12;
        private static readonly double executeTime = 1000 * 60 * 60 * 12;
        private static readonly string emailWhoSend = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfigured"];
        private static readonly string emailPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfiguredPassword"];
        private static readonly int emailPort = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfiguredPort"]);
        private static readonly string displayNameFrom = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailDisplayName"];
        //smtp properties
        private static readonly string emailHost = System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfiguredHost"];
        private static readonly bool emailSsl = Convert.ToBoolean(System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfiguredSsl"]);
        private static readonly int emailTimeOut = Convert.ToInt32(System.Web.Configuration.WebConfigurationManager.AppSettings["CurrentEmailConfiguredTimeOut"]);
        public static void StartTimers()
        {
            emailTimer.Elapsed += new ElapsedEventHandler(ProgramatedTimer);
            emailTimer.Enabled = true;
            emailTimer.Interval = executeTime;
            emailTimer.Start();
        }

        public static void ProgramatedTimer(Object obj, ElapsedEventArgs e)
        {
            emailTimer.AutoReset = true;
            int currentHour = ((DateTime.Now.Hour + 11) % 12) + 1;
            string division = DateTime.Now.Hour > 12 ? "PM" : "AM";
            int startIn = 0;

            if (currentHour != 7)
            {
                // if (currentHour > 7 && currentHour < 12 && division == "AM") { startIn = (12 - currentHour) + (12 - 5); }
                if (currentHour == 12 && division == "AM") { startIn = (12 - 5); }
                if (currentHour < 12 && division == "AM") { startIn = (7 - currentHour); }
                if (currentHour == 12 && division == "PM") { startIn = (12) + (12 - 5); }
                if (currentHour < 12 && division == "PM") { startIn = (12 - currentHour) + (12 - 5); }
                if (startIn < 0) { startIn *= -1; }
                var thread = new System.Threading.Thread(() =>
               {
                   System.Threading.Thread.Sleep(1000 * 60 * 60 * startIn);
               });
                thread.Start();
            }
            else
            { SendOverdueBillEmail(); }

        }

        public static void SendOverdueBillEmail()
        {
            StringBuilder strBuilderErrors = new StringBuilder();
            List<OverduelBill> overduelBillList = null;
            List<Employee> overduelRecipientsList = null;
            ReportByEmail overduelBill = null;
            using (var context = new TicketsEntities())
            {
                context.procDelOverduelBill();
                context.procPopulateOveduelBill();
                overduelBillList = context.OverduelBills.Where(o => DbFunctions.TruncateTime(o.OverduelBillAddedDate) == DbFunctions.TruncateTime(DateTime.Now)).ToList();
                if (overduelBillList.Count != 0)
                {
                    overduelBill = context.ReportByEmails.Where(r => r.ModuleName == (int)ReportByEmailEnum.OVERDUEL_BILL).Select(r => r).FirstOrDefault();
                    if (overduelBill != null)
                    {
                        int[] recipientsIds = Array.ConvertAll(overduelBill.Recipients.Split(','), s => int.Parse(s));
                        overduelRecipientsList = context.Employees.Where(e => recipientsIds.Contains(e.Id)).ToList();
                    }

                }
            }
            if (overduelBillList.Count != 0 && overduelRecipientsList != null && overduelBill != null)
            {
                var sentTo = overduelRecipientsList.Select(o => new
                {
                    Name = o.Name + " " + o.LastName,
                    EmailAddress = o.Email
                });

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailWhoSend, displayNameFrom)
                };
                foreach (var to in sentTo)
                {
                    try
                    {
                        MailAddress e = new MailAddress(to.EmailAddress, to.Name);
                        mail.To.Add(e);
                    }
                    catch
                    {
                        strBuilderErrors.AppendFormat("Los siguientes destinatarios no pudieron ser enviados {0}, {1}", to.EmailAddress, to.Name);
                    }
                }


                try
                {
                    StringBuilder destinatariosbuilder = new StringBuilder();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    doc.Load(HostingEnvironment.MapPath("\\email\\Templates\\template_overduelbill.html"));
                    doc.GetElementbyId("notification-title").InnerHtml = overduelBill.Name;
                    destinatariosbuilder.Append("<p><strong>Para Sr(a):</strong> ");
                    for (int i = 0; i < overduelRecipientsList.Count; i++)
                    {
                        destinatariosbuilder.AppendFormat("{0} {1} - {2}, ", overduelRecipientsList[i].Name, overduelRecipientsList[i].LastName, overduelRecipientsList[i].Email);
                    }
                    destinatariosbuilder.Append("</p>");
                    doc.GetElementbyId("destinatarios").InnerHtml = destinatariosbuilder.ToString();
                    doc.GetElementbyId("message").InnerHtml = "<p>" + overduelBill.Message + "</p>";
                    destinatariosbuilder.Clear();

                    for (int i = 0; i < overduelBillList.Count; i++)
                    {
                        destinatariosbuilder.Append("<tr>");
                        destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + overduelBillList[i].InvoiceId + "</td>");
                        destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + overduelBillList[i].RaffleName + " </td>");
                        destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + overduelBillList[i].ClientName + " </td>");
                        destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + overduelBillList[i].ClientTradeName + " </td>");
                        destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + overduelBillList[i].OverduelBillDate + " </td>");
                        destinatariosbuilder.Append("</tr>");
                    }
                    // var html = new HtmlAgilityPack.HtmlDocument();
                    //html.LoadHtml(destinatariosbuilder.ToString());

                    doc.GetElementbyId("details").InnerHtml = (doc.GetElementbyId("details").InnerHtml + destinatariosbuilder.ToString());
                    mail.Subject = overduelBill.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = doc.DocumentNode.OuterHtml;


                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = emailHost;

                        smtp.Port = emailPort;
                        smtp.EnableSsl = emailSsl;
                        smtp.Timeout = emailTimeOut;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailWhoSend, emailPassword);
                        smtp.Send(mail);
                    };


                }
                catch (Exception)
                {

                }
            }
        }

        public static void SendClientEmail(ReportByEmailEnum byEmail, Client client)
        {

            ReportByEmail newClientReport = null;
            var employeesRecipientList = new List<Employee>();
            StringBuilder strBuilderErrors = new StringBuilder();

            using (var context = new TicketsEntities())
            {
                newClientReport = context.ReportByEmails.Where(r => r.ModuleName == (int)byEmail).FirstOrDefault();
                if (newClientReport != null)
                {
                    if (newClientReport.Recipients != null)
                    {
                        int[] recipientsIds = Array.ConvertAll(newClientReport.Recipients.Split(','), s => int.Parse(s));
                        employeesRecipientList = context.Employees.Where(e => recipientsIds.Contains(e.Id)).ToList();
                    }
                }
            }
            if (newClientReport != null && employeesRecipientList != null && employeesRecipientList.Count != 0)
            {
                ClientStatuEnum clientStatus = (ClientStatuEnum)client.Statu;
                string status = "";
                if (clientStatus == ClientStatuEnum.Created) { status = "Creado"; }
                if (clientStatus == ClientStatuEnum.Approbed) { status = "Aprobado"; }
                if (clientStatus == ClientStatuEnum.InProcess) { status = "En proceso"; }
                if (clientStatus == ClientStatuEnum.Suspended) { status = "Suspendido"; }

                var sentTo = employeesRecipientList.Select(o => new
                {
                    Name = o.Name + " " + o.LastName,
                    EmailAddress = o.Email
                });

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailWhoSend, displayNameFrom)
                };
                foreach (var to in sentTo)
                {
                    try
                    {
                        MailAddress e = new MailAddress(to.EmailAddress, to.Name);
                        mail.To.Add(e);
                    }
                    catch
                    {
                        strBuilderErrors.AppendFormat("Los siguientes destinatarios no pudieron ser enviados {0}, {1}", to.EmailAddress, to.Name);
                    }
                }


                try
                {
                    StringBuilder destinatariosbuilder = new StringBuilder();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    doc.Load(HostingEnvironment.MapPath("\\email\\Templates\\template.html"));
                    doc.GetElementbyId("notification-title").InnerHtml = newClientReport.Name;
                    destinatariosbuilder.Append("<p><strong>Para Sr(a):</strong> ");
                    for (int i = 0; i < employeesRecipientList.Count; i++)
                    {
                        destinatariosbuilder.AppendFormat("{0} {1} - {2}, ", employeesRecipientList[i].Name, employeesRecipientList[i].LastName, employeesRecipientList[i].Email);
                    }
                    destinatariosbuilder.Append("</p>");
                    doc.GetElementbyId("destinatarios").InnerHtml = destinatariosbuilder.ToString();
                    doc.GetElementbyId("message").InnerHtml = "<p>" + newClientReport.Message + "</p>";
                    destinatariosbuilder.Clear();

                    destinatariosbuilder.Append("<tr>");
                    destinatariosbuilder.Append("<th style =\"background-color: #49AE6E; color:#fff; padding:10px;\">Codigo</th>");
                    destinatariosbuilder.Append("<th style =\"background-color: #49AE6E; color:#fff; padding:10px;\">Nombre</th>");
                    destinatariosbuilder.Append("<th style =\"background-color: #49AE6E; color:#fff; padding:10px;\">Nombre comercial</th>");
                    destinatariosbuilder.Append("<th style =\"background-color: #49AE6E; color:#fff; padding:10px;\">Estatus</th>");
                    destinatariosbuilder.Append("</tr>");

                    destinatariosbuilder.Append("<tr>");
                    destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + client.Id + "</td>");
                    destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + client.Name + " </td>");
                    destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + client.Tradename + " </td>");
                    destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + status + " </td>");
                    destinatariosbuilder.Append("</tr>");



                    // var html = new HtmlAgilityPack.HtmlDocument();
                    //html.LoadHtml(destinatariosbuilder.ToString());

                    doc.GetElementbyId("details").InnerHtml = (destinatariosbuilder.ToString());
                    mail.Subject = newClientReport.Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = doc.DocumentNode.OuterHtml;


                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = emailHost;

                        smtp.Port = emailPort;
                        smtp.EnableSsl = emailSsl;
                        smtp.Timeout = emailTimeOut;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(emailWhoSend, emailPassword);
                        smtp.Send(mail);
                    };


                }
                catch (Exception)
                {

                }
            }
        }

        public static void SendReportsEmail(ReportByEmailEnum byEmail, int raffleId, HttpRequestBase request)
        {
            try
            {
                string currentUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/') + "/";
                string MajorAwardInline = string.Format("<a href='{2}Reports/MajorAwardInline?raffleId={0}'>{1}</a>", raffleId, string.Format("PREMIOS MAYORES PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string SpecialAwardInline = string.Format("<a href='{2}Reports/SpecialAwardInline?raffleId={0}'>{1}</a>", raffleId, string.Format("LISTA DE PREMIOS ESPECIALES DEL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string IntermAwardInline = string.Format("<a href='{2}/Reports/IntermAwardInline?raffleId={0}'>{1}</a>", raffleId, string.Format("LISTA DE PREMIOS INTERMEDIOS PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string MinorAwardInline = string.Format("<a href='{2}/Reports/MinorAwardInline?raffleId={0}'>{1}</a>", raffleId, string.Format("LISTA DE PREMIOS MENORES PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                //more

                string Rafflevirtual = string.Format("<a href='{2}#/raffle/virtual/{0}'>{1}</a>", raffleId, string.Format("SORTEO VIRTUAL NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string ReturnedNumbers = string.Format("<a href='{2}Reports/ReturnedNumbers?raffleId={0}'>{1}</a>", raffleId, string.Format("LISTADO DE NÚMEROS PREMIADOS DEVUELTOS PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string ReturnedNumbersGroup = string.Format("<a href='{2}Reports/ReturnedNumbersGroup?raffleId={0}'>{1}</a>", raffleId, string.Format("DEVOLUCION DE BILLETES DE GRUPO DEL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string ReturnedNumbersClient = string.Format("<a href='{2}Reports/ReturnedNumbersClient?raffleId={0}'>{1}</a>", raffleId, string.Format("DEVOLUCION DE BILLETES POR CLIENTES DEL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);

                string ReturnedNumbersAward = string.Format("<a href='{2}Reports/ReturnedNumbersAward?raffleId={0}'>{1}</a>", raffleId, string.Format("LISTADO DE NÚMEROS PREMIADOS DEVUELTOS PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);
                string RaffleGeneralOver = string.Format("<a href='{2}Reports/RaffleGeneralOver?raffleId={0}'>{1}</a>", raffleId, string.Format("DETALLE DE FACTURAS PARA EL SORTEO NO. {0} EN FECHA {1}", raffleId, DateTime.Now.ToLongDateString().ToUpper()), currentUrl);

                ReportByEmail newClientReport = null;
                var employeesRecipientList = new List<Employee>();
                StringBuilder strBuilderErrors = new StringBuilder();



                using (var context = new TicketsEntities())
                {
                    newClientReport = context.ReportByEmails.Where(r => r.ModuleName == (int)byEmail).FirstOrDefault();
                    if (newClientReport != null)
                    {
                        if (newClientReport.Recipients != null)
                        {
                            int[] recipientsIds = Array.ConvertAll(newClientReport.Recipients.Split(','), s => int.Parse(s));
                            employeesRecipientList = context.Employees.Where(e => recipientsIds.Contains(e.Id)).ToList();
                        }
                    }
                }

                var sentTo = employeesRecipientList.Select(o => new
                {
                    Name = o.Name + " " + o.LastName,
                    EmailAddress = o.Email
                });

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(emailWhoSend, displayNameFrom)
                };
                foreach (var to in sentTo)
                {
                    try
                    {
                        MailAddress e = new MailAddress(to.EmailAddress, to.Name);
                        mail.To.Add(e);
                    }
                    catch
                    {
                        strBuilderErrors.AppendFormat("Los siguientes destinatarios no pudieron ser enviados {0}, {1}", to.EmailAddress, to.Name);
                    }
                }



                StringBuilder destinatariosbuilder = new StringBuilder();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.Load(HostingEnvironment.MapPath("\\email\\Templates\\template.html"));
                doc.GetElementbyId("notification-title").InnerHtml = newClientReport.Name;
                destinatariosbuilder.Append("<p><strong>Para Sr(a):</strong> ");
                for (int i = 0; i < employeesRecipientList.Count; i++)
                {
                    destinatariosbuilder.AppendFormat("{0} {1} - {2} ", employeesRecipientList[i].Name, employeesRecipientList[i].LastName, employeesRecipientList[i].Email);
                }
                destinatariosbuilder.Append("</p>");
                doc.GetElementbyId("destinatarios").InnerHtml = destinatariosbuilder.ToString();
                doc.GetElementbyId("message").InnerHtml = "<p>" + newClientReport.Message + "</p>";
                destinatariosbuilder.Clear();

                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<th style =\"background-color: #49AE6E; color:#fff; padding:10px;\">Nombre Sorteo</th>");
                destinatariosbuilder.Append("</tr>");

                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + MajorAwardInline + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + SpecialAwardInline + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + IntermAwardInline + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + MinorAwardInline + " </td>");
                destinatariosbuilder.Append("</tr>");

                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + Rafflevirtual + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + ReturnedNumbers + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + ReturnedNumbersGroup + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + ReturnedNumbersClient + " </td>");
                destinatariosbuilder.Append("</tr>");

                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + ReturnedNumbersAward + " </td>");
                destinatariosbuilder.Append("</tr>");
                //
                destinatariosbuilder.Append("<tr>");
                destinatariosbuilder.Append("<td style=\"border: 1px solid #dddddd; text-align: left;padding: 8px;\"> " + RaffleGeneralOver + " </td>");
                destinatariosbuilder.Append("</tr>");


                // var html = new HtmlAgilityPack.HtmlDocument();
                //html.LoadHtml(destinatariosbuilder.ToString());

                doc.GetElementbyId("details").InnerHtml = (destinatariosbuilder.ToString());
                mail.Subject = newClientReport.Subject;
                mail.IsBodyHtml = true;
                mail.Body = doc.DocumentNode.OuterHtml;


                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = emailHost;

                    smtp.Port = emailPort;
                    smtp.EnableSsl = emailSsl;
                    smtp.Timeout = emailTimeOut;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailWhoSend, emailPassword);
                    smtp.Send(mail);
                };


            }
            catch (Exception)
            {

            }
        }


        public static void SendReports()
        {
            //using(var context = new TicketsEntities())
            //{

            //    var entity = context.Raffles.Where(r => r.Id == 3742).FirstOrDefault();
            //  //  RenderReportToPdf(entity, "\\Views\\Reports\\MajorAwardInline.cshtml", "MajorAwardInline",ReportByEmailEnum.MODIFIED_CLIENT);
            //    //RenderReportToPdf(entity, "\\Views\\Reports\\MinorAwardInline.cshtml", "MinorAwardInline");
            //    //RenderReportToPdf(entity, "\\Views\\Reports\\IntermAwardInline.cshtml", "IntermAwardInline");
            //    RenderReportToPdf(entity, "\\Views\\Reports\\SpecialAwardInline.cshtml", "SpecialAwardInline", ReportByEmailEnum.MODIFIED_CLIENT);
            //}
        }

        public static void RenderReportToPdf(dynamic model, string viewPath, string awardName, ReportByEmailEnum reportEnum)
        {
            try
            {
                //string reportName = string.Format("{0}{1}{2}", awardName, DateTime.Now.Date.ToString("ddMMyyyy"), model.Id + ".pdf");
                //var st = new StringWriter();
                //var contextWrapper = new HttpContextWrapper(HttpContext.Current);
                //var routeData = new RouteData();
                //var controllerContext = new ControllerContext(new RequestContext(contextWrapper, routeData), new ReportsController());
                //var razor = new RazorView(controllerContext, viewPath, null, false, null);
                //razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
                //string render = st.ToString();
                /// createPdf();
                //rotativaSample();

                //   StringBuilder destinatariosbuilder = new StringBuilder();
                //   HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                //   document.LoadHtml(render);

                //   render = document.DocumentNode.OuterHtml;
                //   document.Save(HostingEnvironment.MapPath("\\email\\pdf\\" + reportName + ".html"), Encoding.UTF8);

                //  var htmlToPdf = new HtmlToPdfConverter();
                ////   htmlToPdf
                //////   new FileStream(HostingEnvironment.MapPath("\\email\\pdf\\" + reportName), FileMode.Create);
                //   htmlToPdf.GeneratePdf(render, null, HostingEnvironment.MapPath("\\email\\pdf\\" + reportName));



                //Document doc = new Document(PageSize.LETTER);
                ////PdfWriter writer = PdfWriter.GetInstance(doc,
                ////                new FileStream(@"C:\prueba.pdf", FileMode.Create));

                //HTMLWorker htmlparser = new HTMLWorker(doc);

                //StringReader sr = new StringReader(render);
                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(HostingEnvironment.MapPath("\\email\\pdf\\"+ reportName), FileMode.Create));
                //    doc.Open();

                //    htmlparser.Parse(sr);
                //    doc.Close();

                //    byte[] bytes = memoryStream.ToArray();
                //    memoryStream.Close();
                //}

                //ReportByEmail reportToEmployee = null;
                //List<Employee> recipientList = new List<Employee>();
                //StringBuilder strBuilderErrors = new StringBuilder();
                //using (var context = new TicketsEntities())
                //{
                //    reportToEmployee = context.ReportByEmail.Where(r => r.ModuleName == (int)reportEnum).Select(r => r).FirstOrDefault();

                //    int[] recipientsIds = Array.ConvertAll(reportToEmployee.Recipients.Split(','), s => int.Parse(s));
                //    recipientList = context.Employees.Where(e => recipientsIds.Contains(e.Id)).ToList();
                //}
                //    var sentTo = recipientList.Select(o => new
                //    {
                //        Name = o.Name + " " + o.LastName,
                //        EmailAddress = o.Email
                //    });

                //    MailMessage mail = new MailMessage();
                //    mail.From = new MailAddress(emailWhoSend, displayNameFrom);
                //    foreach (var to in sentTo)
                //    {
                //        try
                //        {
                //            MailAddress e = new MailAddress(to.EmailAddress, to.Name);
                //            mail.To.Add(e);
                //        }
                //        catch
                //        {
                //            strBuilderErrors.AppendFormat("Los siguientes destinatarios no pudieron ser enviados {0}, {1}", to.EmailAddress, to.Name);
                //        }
                //    }

                //mail.IsBodyHtml = true;
                //mail.Body = render;

                //using (SmtpClient smtp = new SmtpClient())
                //{
                //    smtp.Host = emailHost;

                //    smtp.Port = emailPort;
                //    smtp.EnableSsl = emailSsl;
                //    smtp.Timeout = emailTimeOut;
                //    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //    smtp.UseDefaultCredentials = false;
                //    smtp.Credentials = new NetworkCredential(emailWhoSend, emailPassword);
                //    smtp.Send(mail);
                //};


            }
            catch (Exception)
            {

            }

            //RenderRazorViewToString(new Controller() viewName,  model);
        }
        public static string RenderPartialToString(object model, string filePath)
        {
            var st = new StringWriter();
            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new ReportsController());
            var razor = new RazorView(controllerContext, filePath, null, false, null);
            razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
            return st.ToString();
        }

        //public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        //{
        //    controller.ViewData.Model = model;
        //    using (var sw = new StringWriter())
        //    {
        //        var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
        //        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
        //        viewResult.View.Render(viewContext, sw);
        //        viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
        //        return sw.GetStringBuilder().ToString();
        //    }
        //}

        //    public static void createPdf(string render ="")
        //    {
        //        LoadSettings settings = new LoadSettings();

        //        var document = new HtmlToPdfDocument
        //        {
        //            GlobalSettings =
        //        {
        //            ProduceOutline = true,
        //            DocumentTitle = "Pretty Websites",
        //            //PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
        //            Margins =
        //            {
        //                All = 1.375,
        //                Unit = Unit.Centimeters
        //            }
        //        },
        //            Objects = {
        //          //  new ObjectSettings { HtmlText = render },
        //            new ObjectSettings { PageUrl = "http://localhost:61913/Reports/IntermAwardInline?raffleId=3742", LoadSettings = settings  },
        //            //new ObjectSettings { PageUrl = "www.microsoft.com" },
        //            //new ObjectSettings { PageUrl = "www.github.com" }
        //        }
        //        };

        //        IConverter converter =
        //new StandardConverter(
        //    new PdfToolset(
        //        new Win32EmbeddedDeployment(
        //            new TempFolderDeployment())));

        //        byte[] result = converter.Convert(document);
        //        FileStream fs = new FileStream(@"e:\prueba.pdf", FileMode.Create);
        //        fs.Write(result, 0, result.Length);
        //        fs.Close();

        //    }



        //public static void ToPdfFile(string actionName, ControllerContext context,int rafleId)
        //{

        //    var actionPDF = new Rotativa.ActionAsPdf(actionName, new { raffleId = rafleId })//some route values))
        //    {

        //        //FileName = "TestView.pdf",
        //        PageSize = Size.A4,
        //        PageOrientation = Rotativa.Options.Orientation.Landscape,
        //        PageMargins = { Left = 1, Right = 1 }
        //    };
        //    byte[] applicationPDFData = actionPDF.BuildPdf(context);
        //    File.WriteAllBytes(HostingEnvironment.MapPath("\\email\\pdf\\" +string.Format("{0}_{1}",actionName,rafleId)), applicationPDFData);
        //}


        //internal class Win32EmbeddedDeployment : IDeployment
        //{
        //    private TempFolderDeployment tempFolderDeployment;

        //    public Win32EmbeddedDeployment(TempFolderDeployment tempFolderDeployment)
        //    {
        //        this.tempFolderDeployment = tempFolderDeployment;
        //    }

        //    public string Path
        //    {
        //        get
        //        {
        //            return @"e:\test.pdf";
        //        }
        //    }
        //}
    }

}
