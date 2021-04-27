using System;
using System.Linq;
using Tickets.Models.Enums;
using WebMatrix.WebData;

namespace Tickets.Models
{
    public class WorkflowTypeUserModel
    {
        #region WorkflowTypeUser

        internal object GetWorkflowTypeUserList(int workflowTypeId)
        {
            var context = new TicketsEntities();
            var users = context.WorkflowType_User.Where(w => w.WorkflowTypeId == workflowTypeId && w.Statu != 9).Select(u => new
            {
                Id = u.Id,
                u.WorkflowTypeId,
                u.OrderApproval,
                u.TypeApproval,
                typeApprovalDesc = u.Catalog.NameDetail,
                u.UserId,
                UserDesc = u.User.Name
            }).ToList();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de usuario para aprovació");
            return users;
        }


        internal object WorkflowTypeUserCreate(WorkflowType_User workflowTypeUser)
        {
            var context = new TicketsEntities();
            if (workflowTypeUser.Id <= 0)
            {
                workflowTypeUser.CreateDate = DateTime.Now;
                workflowTypeUser.CreateUser = WebSecurity.CurrentUserId;
                workflowTypeUser.Statu = 1;
                context.WorkflowType_User.Add(workflowTypeUser);
            }
            else
            {
                var modifyWorkflowTypeUser = context.WorkflowType_User.FirstOrDefault(u => u.Id == workflowTypeUser.Id);
                modifyWorkflowTypeUser.UserId = workflowTypeUser.UserId;
                modifyWorkflowTypeUser.OrderApproval = workflowTypeUser.OrderApproval;
                modifyWorkflowTypeUser.TypeApproval = workflowTypeUser.TypeApproval;
            }
            context.SaveChanges();
            Utils.SaveLog(WebSecurity.CurrentUserName, workflowTypeUser.Id == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Usuario para aprovar", this.GetWorkflowTypeUserObject(workflowTypeUser));
            return true;
        }

        internal object WorkflowTypeUserDelete(int workflowTypeUserId)
        {
            var context = new TicketsEntities();
            var workflowTypeUser = context.WorkflowType_User.FirstOrDefault(m => m.Id == workflowTypeUserId);
            if (workflowTypeUser != null)
            {
                workflowTypeUser.Statu = 9;
                context.SaveChanges();
                Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Usuario para aprovar", this.GetWorkflowTypeUserObject(workflowTypeUser));
            }
            return true;
        }

        internal object GetWorkflowTypeUserObject(WorkflowType_User workfowTypeUser)
        {
            var obj = new
            {
                workfowTypeUser.Id,
                workfowTypeUser.TypeApproval,
                workfowTypeUser.OrderApproval,
                workfowTypeUser.UserId,
                workfowTypeUser.WorkflowTypeId,
                workfowTypeUser.CreateUser,
                workfowTypeUser.CreateDate,
                workfowTypeUser.Statu
            };
            return obj;
        }
        #endregion
    }
}