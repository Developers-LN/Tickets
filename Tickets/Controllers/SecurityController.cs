using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tickets.Models;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using WebMatrix.WebData;
using Tickets.Filters;
using System.DirectoryServices;
using Tickets.Models.Enums;
using System.Web.Configuration;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Tickets.Controllers
{
    [InitializeSimpleMembership]
    [Authorize]
    public class SecurityController : Controller
    {
        private static string __RETURN_URL__ = "__RETURN_URL__";

        // GET: Security/ChangePassword
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }
        
        //
        // Post: Security/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        public JsonResult ChangePassword(ChangePasswordModel model)
        {
            var userName = WebSecurity.CurrentUserName;
            var passwordResetToken = WebSecurity.GeneratePasswordResetToken(userName);
            WebSecurity.ResetPassword(passwordResetToken, model.Password);
            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { 
                    result = true
                }
            };
        }

        #region Login

        // GET: Security/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {

            string query = Request.QueryString["ReturnUrl"];
            if(!string.IsNullOrWhiteSpace(query))
            {
                Session["__RETURN_URL__"] = query;
            }
            return View();
        }

        // GET: Security/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult Login(LoginModel model)
        {
            string query = (String)Session["__RETURN_URL__"];
            var result = new JsonResult() { Data = false };
            if (ModelState.IsValid)
            {
                bool userActiveDirectory = WebConfigurationManager.AppSettings["UserActiveDirectory"].ToString().ToLower() == "true";

                if (userActiveDirectory)
                {
                    var domainName = WebConfigurationManager.AppSettings["ActiveDirectoryDomainName"].ToString();
                    if (LogonValid(domainName, model.Name, model.Password))
                    {
                        var context = new TicketsEntities();
                        var user = context.Users.FirstOrDefault(u => u.Name == model.Name);
                        if (user == null)
                        {
                            WebSecurity.CreateUserAndAccount(model.Name, model.Password);
                        }
                        if (user.Statu == (int)UserStatusEnum.Active)
                        {
                            if (WebSecurity.Login(model.Name, model.Password, persistCookie: false))
                            {
                                Utils.SaveLog(model.Name, LogActionsEnum.LoginActiveDirectory, "Iniciar sesión active directory");
                                result.Data = GetModuleForLoginUser(model.Name);
                            }
                        }
                    }
                }
                else
                {
                    var context = new TicketsEntities();
                    var user = context.Users.FirstOrDefault(u => u.Name == model.Name);
                    if (user != null)
                    {
                        if (user.Statu == (int)UserStatusEnum.Active)
                        {
                            if (WebSecurity.Login(model.Name, model.Password, persistCookie: true))
                            {
                                Utils.SaveLog(model.Name, LogActionsEnum.Login, "Iniciar sesión");
                                result.Data = GetModuleForLoginUser(model.Name);
                            }
                        }
                    }
                }
            }
            Session["__RETURN_URL__"] = null;
            return new JsonResult()
            {
                Data = new
                {
                    Data = result.Data,
                    Url = query
                }
            };
       
           
        }
        //
        // Post: /Users/Login
        //[HttpPost]
        //[AllowAnonymous]
        //public JsonResult Login(LoginModel model)
        //{
        //    string query = (String) Session["__RETURN_URL__"];
        //    var result = new JsonResult() { Data = false };
        //    if (ModelState.IsValid)
        //    {
        //        bool userActiveDirectory = WebConfigurationManager.AppSettings["UserActiveDirectory"].ToString().ToLower() == "true";

        //        if (userActiveDirectory)
        //        {
        //            var domainName = WebConfigurationManager.AppSettings["ActiveDirectoryDomainName"].ToString();
        //            if (LogonValid(domainName, model.Name, model.Password))
        //            {
        //                var context = new TicketsEntities();
        //                var user = context.Users.FirstOrDefault(u => u.Name == model.Name);
        //                if (user == null)
        //                {
        //                    WebSecurity.CreateUserAndAccount(model.Name, model.Password);
        //                }
        //                if (user.Statu == (int)UserStatusEnum.Active)
        //                {
        //                    if (WebSecurity.Login(model.Name, model.Password, persistCookie: false))
        //                    {
        //                        Utils.SaveLog(model.Name, LogActionsEnum.LoginActiveDirectory, "Iniciar sesión active directory");
        //                        result.Data = GetModuleForLoginUser(model.Name);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var context = new TicketsEntities();
        //            var user = context.Users.FirstOrDefault(u => u.Name == model.Name);
        //            if (user != null)
        //            {
        //                if (user.Statu == (int)UserStatusEnum.Active)
        //                {
        //                    if (WebSecurity.Login(model.Name, model.Password, persistCookie: true))
        //                    {
        //                        Utils.SaveLog(model.Name, LogActionsEnum.Login, "Iniciar sesión");
        //                        result.Data = GetModuleForLoginUser(model.Name);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //   if((bool)result.Data == true && !string.IsNullOrWhiteSpace(query))
        //    {
        //        return new JsonResult
        //        {
        //            Data = new
        //            {
        //                result = true,
        //                Url = query
        //            }
        //        };
        //    }
        //    return result;
        //}

        //
        // GET: /Users/VerifyPassword
        [HttpGet]
        [Authorize]
        public JsonResult VerifyPassword(string password)
        {
            var context = new TicketsEntities();
            var user = context.Users.AsEnumerable().Where(u => u.Id == WebSecurity.CurrentUserId).FirstOrDefault();

            var result = System.Web.Security.Membership.ValidateUser(user.Name, password);
            if (result == true)
            {
                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { result = result, message = "" } };
            }
            else
            {

                return new JsonResult() { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = new { result = result, message = "Esta no es la contraseña del usuario " + WebSecurity.CurrentUserName } };
            }
        }
        
        //
        // GET: /Users/VerifyUserLogin
        [HttpGet]
        [AllowAnonymous]
        public JsonResult VerifyUserLogin( )
        {
            var result = new JsonResult(){ Data = false, JsonRequestBehavior= JsonRequestBehavior.AllowGet };
            if (WebSecurity.IsAuthenticated)
            {
                result.Data = GetModuleForLoginUser(WebSecurity.CurrentUserName);
            }
            return result;
        }

        private string GetModuleForLoginUser(string userName)
        {
            var context = new TicketsEntities();
            var rol_modules = new List<Rol_Module>();
            context.webpages_Roles.Where(r => r.Users.Any(u => u.Name == userName)).ToList()
                .ForEach( r=> rol_modules.AddRange(r.Rol_Module));
            var modules = rol_modules.AsEnumerable().GroupBy( m=>m.Module.Name).Select( m=> new{
                name = m.FirstOrDefault().Module.Name,
                view = m.Where( s=>s.CanView).Any(),
                edit = m.Where( s=>s.CanEdit).Any(),
                delete = m.Where(s => s.CanDelete).Any(),
                add = m.Where(s => s.CanAdd).Any(),
                search = m.Where(s => s.CanSearch).Any()
            });
            return JsonConvert.SerializeObject( new{ name = userName, result = true, modules = modules});
        }


        //
        // Post: /Users/LogOff
        [HttpPost]
        [Authorize]
        public JsonResult LogOff(LoginModel model)
        {
            WebSecurity.Logout();
            return new JsonResult() { Data = true };
        }

        private bool LogonValid(string ldapDomain, string userName, string password)
        {
            DirectoryEntry de = new DirectoryEntry(@"LDAP://" + ldapDomain, userName, password);

            try
            {
                object o = de.NativeObject;
                return true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return false;
            }
        }

        #endregion

        #region Users
        // GET: Security/UserList
        [HttpGet]
        [Authorize]
        public ActionResult UserList()
        {
            return View();
        }

        // GET: Security/GetUserList
        [HttpGet]
        [Authorize]
        public JsonResult GetUserList()
        {
            var context = new TicketsEntities();
            var users = context.Users.Select(u => new
            {
                Name = u.Name,
                StatuDesc = u.Catalog.NameDetail,
                Statu = u.Statu,
                u.EmpleadoId,
                EmpleadoDesc = u.EmpleadoId.HasValue? context.Employees.Where( e=>e.Id == u.EmpleadoId).Select( e=> e.Name + " " + e.LastName).FirstOrDefault() : "",
                Password = "",
                PasswordRepeat = "",
                Id = u.Id
            }).ToList();
            var employes = context.Employees.Select(e => new { 
                e.Id,
                Name = e.Name + " " + e.LastName
            });
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Usuario");
            return new JsonResult() { Data = new { users, employes }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/RolListInUser
        [HttpGet]
        [Authorize]
        public ActionResult RolListInUser()
        {
            return View();
        }

        // GET: Security/GetRolListForUser
        [HttpGet]
        [Authorize]
        public JsonResult GetRolListForUser(int userId)
        {
            var context = new TicketsEntities();
            var rols = context.webpages_Roles.Select(r => new
            {
                Name = r.RoleName,
                Description = r.Description,
                Statu = r.Users.Any(u => u.Id == userId) ? "Asignado" : "No asignado",
                Id = r.RoleId
            }).ToList();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de roles para un usuario");
            return new JsonResult() { Data = rols, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/ManageRolInUserStatu
        [HttpPost]
        [Authorize]
        public JsonResult ManageRolInUserStatu(RolUserStatuModel model)
        {
            var context = new TicketsEntities();
            var user = context.Users.FirstOrDefault(u => u.Id == model.UserId);
            var rol = context.webpages_Roles.FirstOrDefault(r => r.RoleId == model.RolId);
            if (model.Statu == true)
            {
                user.webpages_Roles.Add(rol);
            }
            else
            {
                user.webpages_Roles.Remove(rol);
            }
            context.SaveChanges();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Roles a un usuario");
            return new JsonResult() { Data = model.Statu };
        }

        // GET: Security/UserCreate
        [HttpGet]
        [Authorize]
        public ActionResult UserCreate()
        {
            return View();
        }

        // GET: Security/UserCreate
        [HttpPost]
        [Authorize]
        public JsonResult UserCreate(UserCreateModel user)
        {
            var context = new TicketsEntities();
            if (user.Id <= 0)
            {
                WebSecurity.CreateUserAndAccount(user.Name, user.Password);
                var modifyUser = context.Users.FirstOrDefault(u => u.Name == user.Name);
                modifyUser.Statu = user.Statu;
                modifyUser.EmpleadoId = user.EmpleadoId;
            }
            else
            {
                if (user.Password != "")
                {
                    var passwordResetToken = WebSecurity.GeneratePasswordResetToken(user.Name);
                    WebSecurity.ResetPassword(passwordResetToken, user.Password);
                }
                var modifyUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
                modifyUser.Statu = user.Statu;
                modifyUser.EmpleadoId = user.EmpleadoId;
            }
            context.SaveChanges();
            
            Utils.SaveLog(WebSecurity.CurrentUserName, user.Id ==  0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Usuarios");
            return new JsonResult() { Data = true };
        }

        #endregion

        #region Roles
        // GET: Security/RolList
        [HttpGet]
        [Authorize]
        public ActionResult RolList()
        {
            return View();
        }

        // GET: Security/GetRolList
        [HttpGet]
        [Authorize]
        public JsonResult GetRolList()
        {
            var context = new TicketsEntities();
            var users = context.webpages_Roles.Select(u => new
            {
                RoleName = u.RoleName,
                Description =u.Description,
                RoleId = u.RoleId
            }).ToList();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Roles");
            return new JsonResult() { Data = users, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/RolCreate
        [HttpGet]
        [Authorize]
        public ActionResult RolCreate()
        {
            return View();
        }

        // GET: Security/RolCreate
        [HttpPost]
        [Authorize]
        public JsonResult RolCreate(webpages_Roles rol)
        {
            var context = new TicketsEntities();
            if (rol.RoleId <= 0)
            {
                context.webpages_Roles.Add(rol);
            }
            else
            {
                var modifyrol = context.webpages_Roles.FirstOrDefault(u => u.RoleId == rol.RoleId);
                modifyrol.RoleName = rol.RoleName;
                modifyrol.Description = rol.Description;
            }
            context.SaveChanges();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, rol.RoleId == 0 ? LogActionsEnum.Insert : LogActionsEnum.Update, "Roles");
            return new JsonResult() { Data = true };
        }

        // GET: Security/UserListInRol
        [HttpGet]
        [Authorize]
        public ActionResult UserListInRol()
        {
            return View();
        }

        // GET: Security/GetUserListForRol
        [HttpGet]
        [Authorize]
        public JsonResult GetUserListForRol(int rolId)
        {
            var context = new TicketsEntities();
            var users = context.Users.Select(u => new
            {
                Name = u.Name,
                Statu = u.webpages_Roles.Any(r => r.RoleId == rolId) ? "Asignado" : "No asignado",
                Id = u.Id
            }).ToList();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de usuario para un rol");
            return new JsonResult() { Data = users, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/ModuleListInRol
        [HttpGet]
        [Authorize]
        public ActionResult ModuleListInRol()
        {
            return View();
        }

        // GET: Security/GetModuleListInRol
        [HttpGet]
        [Authorize]
        public JsonResult GetModuleListInRol(int rolId)
        {
            var context = new TicketsEntities();
            var modules = context.Modules.Select(u => new
            {
                Name = u.Name,
                Description = u.Description,
                CanView = u.Rol_Module.Any(r => r.RolId == rolId && r.CanView),
                CanEdit = u.Rol_Module.Any(r => r.RolId == rolId && r.CanEdit),
                CanDelete = u.Rol_Module.Any(r => r.RolId == rolId && r.CanDelete),
                CanAdd = u.Rol_Module.Any(r => r.RolId == rolId && r.CanAdd),
                CanSearch = u.Rol_Module.Any(r => r.RolId == rolId && r.CanSearch),
                View = u.CanView,
                Edit = u.CanEdit,
                Delete = u.CanDelete,
                Add = u.CanAdd,
                Search = u.CanSearch,
                Id = u.Id
            }).ToList();
            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de Accesos para un rol");
            return new JsonResult() { Data = modules, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        
        //POST: Security/SaveModuleForRol
        [HttpPost]
        [Authorize]
        public JsonResult SaveModuleForRol(List<Rol_Module> models)
        {
            var context = new TicketsEntities();
            foreach (var model in models)
            {
                var rolModule = context.Rol_Module.FirstOrDefault(r => r.RolId == model.RolId && r.ModuleId == model.ModuleId);
                if (rolModule == null)
                {
                    context.Rol_Module.Add(model);
                }
                else
                {
                    rolModule.CanView = model.CanView;
                    rolModule.CanEdit = model.CanEdit;
                    rolModule.CanDelete = model.CanDelete;
                    rolModule.CanAdd = model.CanAdd;
                    rolModule.CanSearch = model.CanSearch;
                }
            }
            context.SaveChanges();

            Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Accessos de un rol", models.Select( m=>new {
                m.ModuleId,
                m.RolId,
                m.CanAdd,
                m.CanDelete,
                m.CanEdit,
                m.CanSearch,
                m.CanView
            }) );
            return new JsonResult() { Data = true };
        }

        //GET: Security/OfficeListInRol
        [HttpGet]
        [Authorize]
        public ActionResult OfficeListInRol()
        {
            return View();
        }

        //GET: Security/GetOfficeListInRol
        [HttpGet]
        [Authorize]
        public JsonResult GetOfficeListInRol(int rolId)
        {
            var context = new TicketsEntities();
            var offices = context.Agencies.Select(o => new
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                Statu = o.webpages_Roles.Any(r => r.RoleId == rolId) ? "Asignada" : "No asignada"
            }).ToList();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de sucursales para un rol");
            return new JsonResult() { Data = offices, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/ManageOfficeInInRolStatu
        [HttpPost]
        [Authorize]
        public JsonResult ManageOfficeInInRolStatu(RolOfficeStatuModel model)
        {
            var context = new TicketsEntities();
            var office = context.Agencies.FirstOrDefault(u => u.Id == model.OfficeId);
            var rol = context.webpages_Roles.FirstOrDefault(r => r.RoleId == model.RolId);
            if (model.Statu == true)
            {
                office.webpages_Roles.Add(rol);
            }
            else
            {
                office.webpages_Roles.Remove(rol);
            }
            context.SaveChanges();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Update, "Sucursales a un usuario");
            return new JsonResult() { Data = model.Statu };
        }
        #endregion

        #region Logs
        // GET: Security/Logs
        [HttpGet]
        [Authorize]
        public ActionResult Logs()
        {
            return View();
        }

        // GET: Security/GetLogs
        [HttpGet]
        [Authorize]
        public JsonResult GetLogs()
        {
          

            var context = new TicketsEntities();
            var catalogs = context.Catalogs.Where( c=> c.IdGroup == (int)CatalogGroupEnum.LogActions).ToList();
            var logs = context.Logs.OrderByDescending(l => l.CreateDate).Take(1000).ToList().AsEnumerable()
                .Select(l => new
                {
                    actionDesc = catalogs.FirstOrDefault(c => c.Id == l.Action).NameDetail,
                    moduleName = l.Module,
                    userName = l.UserName,
                    createDate = l.CreateDate.ToString(),
                }).ToList();
            
            return new JsonResult()
            {
                Data = new
                {
                    result = true,
                    logs
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
               
            };
        }

        #endregion

        #region Modules
        // GET: Security/ModuleList
        [HttpGet]
        [Authorize]
        public ActionResult ModuleList()
        {
            return View();
        }

        // GET: Security/GetModuleList
        [HttpGet]
        [Authorize]
        public JsonResult GetModuleList()
        {
            var context = new TicketsEntities();
            var modules = context.Modules.Select(u => new
            {
                Name = u.Name,
                Description = u.Description,
                CanEdit = u.CanEdit,
                CanDelete = u.CanDelete,
                CanAdd = u.CanAdd,
                CanView = u.CanView,
                CanSearch = u.CanSearch,
                Id = u.Id
            }).ToList();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.View, "Listado de modulo");

            return new JsonResult() { Data = modules, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // GET: Security/ModuleCreate
        [HttpGet]
        [Authorize]
        public ActionResult ModuleCreate()
        {
            return View();
        }

        // POST: Security/ModuleCreate
        [HttpPost]
        [Authorize]
        public JsonResult ModuleCreate(Module module)
        {
            var context = new TicketsEntities();
            if (module.Id <= 0)
            {
                context.Modules.Add(module);
            }
            else
            {
                var modifyModule = context.Modules.FirstOrDefault(u => u.Id == module.Id);
                modifyModule.Name = module.Name;
                modifyModule.Description = module.Description;
                modifyModule.CanAdd = module.CanAdd;
                modifyModule.CanDelete = module.CanDelete;
                modifyModule.CanEdit = module.CanEdit;
                modifyModule.CanSearch = module.CanSearch;
                modifyModule.CanView = module.CanView;
            }
            context.SaveChanges();
            context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, module.Id == 0? LogActionsEnum.Insert : LogActionsEnum.Update, "Modulo");
            return new JsonResult() { Data = true };
        }

        // POST: Security/ModuleDelete
        [HttpPost]
        [Authorize]
        public JsonResult ModuleDelete(int moduleId)
        {
            var context = new TicketsEntities();
            var module = context.Modules.FirstOrDefault(m => m.Id == moduleId);
            if (module != null)
            {
                context.Modules.Remove(module);
                context.SaveChanges(); Utils.SaveLog(WebSecurity.CurrentUserName, LogActionsEnum.Delete, "Modulo");
            }
            return new JsonResult();
        }

        #endregion

        #region Encrypt
        public string EncryptPassowrd(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        public string DecryptPassword(string cipher)
        {
            if (cipher == null) throw new ArgumentNullException("cipher");

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decrypted);
        }
        #endregion
    }
}