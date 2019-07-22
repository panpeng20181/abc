using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using xx;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net.Http;
using System.Net;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var db = new PetaPoco.Database("DefaultConnection");
            //var  sql1 = new Sql("CREATE TABLE #tb1(Id   varchar(50)     not null)");

            //db.Execute(sql1);

            var sql2 = new Sql(@"select op.*,o.OrganizationName,op.StudysiteNo, o.SubDomain  from sms_orgProject  op
            left join cts_organization o on op.organizationId=o.id
            where op.projectid =@0 and o.OrganizationType=@1 and isnull(op.isdeleted,0)<>1 and isnull(o.isdeleted,0)<>1", "593169721116912623", 2);
            var kk=db.Fetch<PersonalProject>(sql2);
            var ssj = db.LastArgs;
            var ks = db.LastCommand;
            var s=db.LastSQL;
              
            var k12 = GetOrgProjects("593079586929400170", OrganizationType.StudySite);
            var k2=GetOrgProjectsString("593079586929400170", OrganizationType.StudySite);


            //(!input.isSiteDataCollection && input.SubjectVisitIds != null&& input.SubjectVisitIdsString == null ? " and #subjectVisit.Id in " + string.Format("('{0}')", string.Join("','", input.SubjectVisitIds)) : (!input.isSiteDataCollection && input.SubjectVisitIds != null ? " and #subjectVisit.Id in " + string.Format("({0})", string.Join("','", input.SubjectVisitIds)) : ""))
            //(!input.isSiteDataCollection && input.SubjectVisitIds != null ? " and #subjectVisit.Id in " + string.Format("('{0}')", string.Join("','", input.SubjectVisitIds)) : "")
            //    // Create the article
            //    var a = new test();

            //    a.name = "我的标题";
            //    a.type = 4;

            //    db.Insert("test", "id", a);



            //    var result = db.Page<test>(1, 2, // <-- page number and items per page
            //"SELECT * FROM test WHERE type=@0 ORDER BY id DESC", 3);



            //    var ssd = db.Query<test>("SELECT * FROM test");
            //    // 查询所有数据    
            //    foreach (var aaa in db.Query<test>("SELECT * FROM test"))
            //    {
            //        //var ss = aa;
            //        Response.Write(aaa.name);
            //        //Console.WriteLine("{0} - {1}", a.article_id, a.title);
            //    }
            //    //查询标量
            //    long count = db.ExecuteScalar<long>("SELECT Count(*) FROM test");
            //    Response.Write(count);
            //    //查询单条数据
            //    var aa = db.SingleOrDefault<test>("SELECT * FROM test WHERE id=@0", 1);
            //    Response.Write(aa.name);
            return View();
        }

        public ActionResult About()
        {
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            ////buf是读取的文件
            //response.Content = new ByteArrayContent(buff);
            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetMimeType(fileextend));//设置文件的Content-Type（MIME）
            //var content_disposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            //content_disposition.FileName = filename;
            //response.Content.Headers.ContentDisposition = content_disposition;
            //return response;


            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public static List<PersonalProject> GetOrgProjects(string projectId, OrganizationType organizationType)
        {
            var db = new PetaPoco.Database("DefaultConnection");
            var sql = new Sql(@"select op.*,o.OrganizationName,op.StudysiteNo, o.SubDomain  from sms_orgProject  op
            left join cts_organization o on op.organizationId=o.id
            where op.projectid=@0 and o.OrganizationType=@1 and isnull(op.isdeleted,0)<>1 and isnull(o.isdeleted,0)<>1", projectId, organizationType);
            return db.Fetch<PersonalProject>(sql);
        }
        /// <summary>
        /// 查询所有参与这个项目的项目信息--返回SQL字符串
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static string GetOrgProjectsString(string projectId, OrganizationType organizationType)
        {
            var db = new PetaPoco.Database("DefaultConnection");
            var sql = @"select op.Id  from sms_orgProject  op
            left join cts_organization o on op.organizationId=o.id
            where op.projectid='{0}' and o.OrganizationType={1} and isnull(op.isdeleted,0)<>1 and isnull(o.isdeleted,0)<>1";
            var sss = string.Format(sql, projectId, (int)organizationType);
            return string.Format(sql, projectId, (int)organizationType);
        }
        public enum OrganizationType
        {
            [Description("平台")]
            Platform = 0,
            [Description("申办方")]
            Sponsor,
            [Description("机构")]
            StudySite,
            [Description("伦理")]
            Ethic,
            [Description("SMO")]
            SMO,
            [Description("CRO")]
            CRO,
            [Description("EDC")]
            EDC,
            [Description("IWRS")]
            IWRS,
            [Description("数据管理")]
            DataManage,
            [Description("统计")]
            Statistics,
            [Description("稽查")]
            Inspection,
            [Description("中心实验室")]
            CentralLaboratory,
            [Description("冷链")]
            ColdChain,
            [Description("中心影像")]
            CenterImage,
            [Description("中心病理")]
            CentralPathology,
            [Description("保险")]
            Insurance,
            [Description("药物管理")]
            DrugManage,
            [Description("印刷")]
            Print
        }
        public class PersonalProject
        {
            public string Id { get; set; }

            public string OrganizationId { get; set; }

            public string OrganizationName { get; set; }

            public string SubDomain { get; set; }

            public string CreatedUserId { get; set; }

            public string ProjectId { get; set; }

            public string ProjectName { get; set; }

            public bool IsLeaderDept { get; set; }

            public string OrgProjectNo { get; set; }

            public int SitePlanSubjectCount { get; set; }

            public ProjectStatus ProjectStatus { get; set; }

            public string ResponsibleDept { get; set; }

            public string ResponsibleDeptName { get; set; }

            public string StudySitePI { get; set; }

            public string StudySitePIName { get; set; }

            public bool IsCreated { get; set; }

            public bool IsAccept { get; set; }

            public bool IsDeleted { get; set; }

            public string AcceptComment { get; set; }

            public DateTime? SubmitDate { get; set; }

            public string StudysiteNo { get; set; }

            public DateTime? PlanCompleteDate { get; set; }

            public string Comment { get; set; }
            //HIS系统是否开单
            public bool HisRegisterType { get; set; }
        }
        public enum ProjectStatus
        {
            //草稿
            [Description("草稿")]
            Draft = 1,
            //待初审
            [Description("待初审")]
            FristTrial,
            //初审失败
            [Description("初审失败")]
            TrialFail,
            //立项审批
            [Description("立项审批")]
            ProjectApproval,
            //伦理审批
            [Description("伦理审批")]
            EthiclApproval,
            //合同签署
            [Description("合同签署")]
            ContractSigning,
            //项目启动
            [Description("项目启动")]
            ProjectStart,
            //入组完成
            [Description("入组完成")]
            GroupCompleted,
            //项目结题
            [Description("项目结题")]
            ProjectCompleted,
            //终止
            [Description("终止")]
            ProjectTermination,
            //拒绝
            [Description("拒绝")]
            Refuse,
            //入组阶段
            [Description("入组阶段")]
            Group,
            //随访阶段
            [Description("随访阶段")]
            Visit,
            //暂停
            [Description("暂停")]
            Pause,
            //立项通过形式审查
            [Description("立项通过形式审查")]
            PassFormatCheck
        }

    }
}