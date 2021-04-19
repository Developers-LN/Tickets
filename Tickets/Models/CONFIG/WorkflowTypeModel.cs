using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models
{
    public class WorkflowTypeModel
    {
        #region WorkflowType
       
        internal object GetWorkflowTypeList()
        {
            var context = new TicketsEntities();
            var workflowType = context.WorkflowTypes.Where(w => w.Statu != 9).AsEnumerable().Select(u => GetWorkflowTypeObject(u)).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Tipo de Flojo de Trabajo");
            return workflowType;
        }

        internal object WorkflowTypeCreate(WorkflowType workflowType)
        {
            var context = new TicketsEntities();
            if (workflowType.Id <= 0)
            {
                workflowType.CreateDate = DateTime.Now;
                workflowType.CreateUser = WebSecurity.CurrentUserId;
                context.WorkflowTypes.Add(workflowType);
            }
            else
            {
                var modifyWorkflowType = context.WorkflowTypes.FirstOrDefault(u => u.Id == workflowType.Id);
                modifyWorkflowType.Name = workflowType.Name;
                modifyWorkflowType.Description = workflowType.Description;
                modifyWorkflowType.Statu = workflowType.Statu;
            }
            context.SaveChanges();
            Utils.SaveLog(WebSecurity.CurrentUserName, workflowType.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Tipo de Flujo de Trabajo", this.GetWorkflowTypeObject(workflowType));
            return true;
        }

        internal object WorkflowTypeDelete(int workflowTypeId)
        {
            var context = new TicketsEntities();
            var workflowType = context.WorkflowTypes.FirstOrDefault(m => m.Id == workflowTypeId);
            if (workflowType != null)
            {
                workflowType.Statu = 9;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Catalogo", this.GetWorkflowTypeObject(workflowType));
            }
            return true;
        }

        internal object GetWorkflowTypeObject(WorkflowType workfowType)
        {
            var context = new TicketsEntities();
            var obj = new
            {
                workfowType.Id,
                workfowType.Name,
                workfowType.Description,
                workfowType.Statu,
                StatuDescription = context.Catalogs.FirstOrDefault(c => c.Id == workfowType.Statu).NameDetail,
                workfowType.CreateDate,
                workfowType.CreateUser
            };
            return obj;
        }
        #endregion  
    }
}