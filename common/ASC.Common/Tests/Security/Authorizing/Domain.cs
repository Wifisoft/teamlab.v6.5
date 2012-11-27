#if DEBUG
using System;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using AzAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Common.Tests.Security.Authorizing
{

    static class Domain
    {

        //Аккаунты

        public static readonly Guid accountAlientID = Guid.NewGuid();
        public static readonly IAccount accountAlient = new UserAccount(accountAlientID, "Djohn Doo");

        public static readonly Guid accountLevID = Guid.NewGuid();
        public static readonly IAccount accountLev = new UserAccount(accountLevID, "Lev");

        public static readonly Guid accountAntonID = Guid.NewGuid();
        public static readonly IAccount accountAnton = new UserAccount(accountAntonID, "anton");

        public static readonly Guid accountNikID = Guid.NewGuid();
        public static readonly IAccount accountNik = new UserAccount(accountNikID, "nikolay");

        public static readonly Guid accountValeryID = Guid.NewGuid();
        public static readonly IAccount accountValery = new UserAccount(accountValeryID, "Schumaher");

        public static readonly Guid accountKatID = Guid.NewGuid();
        public static readonly IAccount accountKat = new UserAccount(accountKatID, "Kat");

        public static readonly Guid accountMessangerServiceID = Guid.NewGuid();
        public static readonly IAccount accountMessangerService = new AccountS(accountMessangerServiceID, "Messanger Service");


        //Роли

        public static readonly Guid roleAVSID = Guid.NewGuid();
        public static readonly IRole roleAVS = new Role(roleAVSID, "AVS");

        public static readonly Guid roleAdministrationID = Guid.NewGuid();
        public static readonly IRole roleAdministration = new Role(roleAdministrationID, "administration");

        public static readonly Guid roleNETID = Guid.NewGuid();
        public static readonly IRole roleNET = new Role(roleNETID, ".NET Deparment");

        public static readonly Guid roleHRID = Guid.NewGuid();
        public static readonly IRole roleHR = new Role(roleHRID, "кадры");


        //IRoleProvider

        public static readonly RoleFactory RoleProvider = new RoleFactory();


        //добавление пользователей
        public static readonly Guid actionAddUserID = Guid.NewGuid();
        public static readonly AzAction actionAddUser = new AzAction(actionAddUserID, "add user");

        //удаление пользователей
        public static readonly Guid actionDeleteUserID = Guid.NewGuid();
        public static readonly AzAction actionDeleteUser = new AzAction(actionDeleteUserID, "delete user");

        //изменение своей контактной информации
        public static readonly Guid actionChangeSelfContactInfoID = Guid.NewGuid();
        public static readonly AzAction actionChangeSelfContactInfo = new AzAction(actionChangeSelfContactInfoID, "change self contact info");

        //просмотр информации о пользователях
        public static readonly Guid actionViewInfoID = Guid.NewGuid();
        public static readonly AzAction actionViewInfo = new AzAction(actionViewInfoID, "view user info");


        //категория работы с пользователями
        public static readonly Guid categoryUserManagerID = Guid.NewGuid();
        public static readonly AuthCategory categoryUserManager = new AuthCategory("user manager", new[] { actionAddUser, actionDeleteUser, actionChangeSelfContactInfo, actionViewInfo });


        //редактирование информации, связанной с отделом кадров
        public static readonly Guid actionHREditInfoID = Guid.NewGuid();
        public static readonly AzAction actionHREditInfo = new AzAction(actionHREditInfoID, "edit HR info");

        //просмотр информации, связанной с отделом кадров
        public static readonly Guid actionHRViewInfoID = Guid.NewGuid();
        public static readonly AzAction actionHRViewInfo = new AzAction(actionHRViewInfoID, "view HR info");

        //категория работы отдела кадров
        public static readonly Guid categoryHRID = Guid.NewGuid();
        public static readonly AuthCategory categoryHR = new AuthCategory("HR", new[] { actionHREditInfo, actionHRViewInfo });


        //коммитить код в svn
        public static readonly Guid actionNETCommitID = Guid.NewGuid();
        public static readonly AzAction actionNETCommit = new AzAction(actionNETCommitID, "SVN Commit");

        //просматривать код из в svn
        public static readonly Guid actionNETViewID = Guid.NewGuid();
        public static readonly AzAction actionNETView = new AzAction(actionNETViewID, "SVN View");

        public static readonly Guid categoryNETID = Guid.NewGuid();
        public static readonly AuthCategory categoryNET = new AuthCategory(".NET deparment Work", new[] { actionNETCommit, actionNETView });


        //PermissionProvider

        public static readonly PermissionFactory PermissionProvider = new PermissionFactory();


        static Domain()
        {
            //настроим вхождение в роли

            //++ явные задания, + следствия от ++
            //-- явные запрещения, - следствия от --
            //aa - явный аудит, a следствия от aa

            //                           action

            //Owner						  ++

            //Self						  ++

            //EveryOne                     

            //User                                  

            //roleAVS                     ++        

            //    roleHR                   +aa         
            //        accountKat           +a          
            //        accountLev           -a        

            //    roleNET                  +         
            //        accountAnton         +         
            //        accountNik           +         
            //        accountValery       --         

            //    roleAdministration      --          
            //        accountLev           -         

            //    accountLev               -a          
            //    accountAnton             +          
            //    accountNik               +          
            //    accountValery            -         
            //    accountKat               +a          

            //все входят в AVS, кроме Alient
            RoleProvider.AddAccountInRole(accountLev, roleAVS);
            RoleProvider.AddAccountInRole(accountAnton, roleAVS);
            RoleProvider.AddAccountInRole(accountNik, roleAVS);
            RoleProvider.AddAccountInRole(accountValery, roleAVS);
            RoleProvider.AddAccountInRole(accountKat, roleAVS);

            //а так же все группы
            RoleProvider.AddAccountInRole(roleHR, roleAVS);
            RoleProvider.AddAccountInRole(roleNET, roleAVS);
            RoleProvider.AddAccountInRole(roleAdministration, roleAVS);

            //лев входит в администрацию
            RoleProvider.AddAccountInRole(accountLev, roleAdministration);

            //катя и лев входят в HR
            RoleProvider.AddAccountInRole(accountKat, roleHR);
            RoleProvider.AddAccountInRole(accountLev, roleHR);

            //Антон, Николай, Валерий входят в .NET
            RoleProvider.AddAccountInRole(accountAnton, roleNET);
            RoleProvider.AddAccountInRole(accountNik, roleNET);
            RoleProvider.AddAccountInRole(accountValery, roleNET);


            //настроим права доступа
            PermissionProvider.AddAce(Domain.roleAVS, actionAddUser, AceType.Allow);
            PermissionProvider.AddAce(Domain.roleAdministration, actionAddUser, AceType.Deny);
            PermissionProvider.AddAce(Domain.accountValery, actionAddUser, AceType.Deny);

            PermissionProvider.AddAce(Constants.Owner, actionAddUser, AceType.Allow);
            PermissionProvider.AddAce(Constants.Self, actionAddUser, AceType.Allow);
        }
    }
}
#endif