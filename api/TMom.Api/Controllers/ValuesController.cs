using Microsoft.AspNetCore.Mvc;
using TMom.Application;
using TMom.Application.Dto;
using TMom.Application.Service.IService;
using TMom.Domain.IRepository;
using TMom.Domain.Model;
using TMom.Domain.Model.Entity;
using TMom.Infrastructure.Helper;
using TMom.Infrastructure.MongoDB;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Data;

namespace TMom.Api.Controllers
{
    /// <summary>
    /// 测试API
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : BaseApiController<SysUser, int>
    {
        private IWebHostEnvironment _webHostEnvironment;
        private IUser _user;
        private ISysUserService _sysUserService;
        private ISysMenuService _sysMenuService;
        private ISysRoleMenuService _sysRoleMenuService;
        private ISysRoleMenuRepository _sysRoleMenuRepository;
        private readonly IRedisRepository _redis;
        private readonly IMongoRepo _mongoRepo;
        private readonly ISqlSugarClient _db;

        public ValuesController(IWebHostEnvironment webHostEnvironment
            , IUser user
            , ISysMenuService sysMenuService
            , ISysRoleMenuService sysRoleMenuService, ISysRoleMenuRepository sysRoleMenuRepository
            , IRedisRepository redis
            , IMongoRepo mongoRepo, ISysUserService sysUserService)
        {
            _webHostEnvironment = webHostEnvironment;
            _user = user;
            _sysMenuService = sysMenuService;
            _sysRoleMenuService = sysRoleMenuService;
            _sysRoleMenuRepository = sysRoleMenuRepository;
            _redis = redis;
            _mongoRepo = mongoRepo;
            _db = sysRoleMenuRepository.Db;
            _sysUserService = sysUserService;
        }

        /// <summary>
        /// 测试生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<object> GetVerifyCode(int width = 100, int height = 50, int len = 4)
        {
            var str = ImageHelper.GetVerifyCode(out string code, width, height, len);
            return Success<object>(new { img = str, code });
        }

        /// <summary>
        /// 测试读取Excel文件内容
        /// </summary>
        /// <param name="uploadFileDto"></param>
        /// <returns></returns>
        [HttpPost]
        public MessageModel<string> ReadExcelData([FromForm] UploadFileDto uploadFileDto)
        {
            List<string> list = new List<string>();
            var file = uploadFileDto.file.FirstOrDefault();
            var dt = ExcelHelper.ReadExcel(file);
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(dr[0].ToString());
            }
            return Success<string>(string.Join(",", list));
        }

        /// <summary>
        /// 测试接口导出数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> ExportByApi()
        {
            var data = await _sysMenuService.Query();
            var result = ExcelHelper.Export("菜单数据导出.xlsx", data);
            return result;
        }

        /// <summary>
        /// 测试MongoDB上传文件
        /// </summary>
        /// <param name="fileDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> MongoUpload([FromForm] UploadFileDto fileDto)
        {
            var file = fileDto.file.FirstOrDefault();
            var id = await _mongoRepo.UploadFile(file?.FileName, file?.OpenReadStream());
            return Success<string>(id.ToString());
        }

        /// <summary>
        /// 测试MongoDB下载文件
        /// </summary>
        /// <param name="mongoId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileStreamResult> MongoDownLoad(string mongoId = "626bb0d2c178a04a872a1fca")
        {
            var fileInfo = await _mongoRepo.GetFileById(mongoId);
            var fileStream = await _mongoRepo.DownloadFileStream(mongoId);
            return File(fileStream, "application/octet-stream", fileInfo.Filename);
        }

        /// <summary>
        /// 测试事务AOP
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<MessageModel<string>> TestAOP()
        {
            await _sysRoleMenuService.TestAOP();
            return Success();
        }

        /// <summary>
        /// 测试分表插入
        /// </summary>
        /// <param name="sysJobLogService"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<MessageModel<string>> TestSpilt([FromServices] ISysJobLogService sysJobLogService)
        {
            var sysJobLog = new SysJobLog()
            {
                SysJobId = 1,
                Content = "执行成功!",
                IsSuccess = true,
                ExecSeconds = 1
            };
            sysJobLog.UpdateCommonFields(1);
            sysJobLog.CreateTime = DateTime.Now.AddDays(10);
            var id = await sysJobLogService.Add(sysJobLog);
            return Success<string>(id.ToString());
        }

        /// <summary>
        /// 测试精确查找到某个分表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<string> TestSpiltGetTblName()
        {
            string tblName_1 = _db.SplitHelper<SysJobLog>().GetTableName(DateTime.Now);
            string tblName_2 = _db.SplitHelper<SysJobLog>().GetTableName(DateTime.Now.AddDays(10));
            string tblName_3 = _db.SplitHelper<SysJobLog>().GetTableName("2022-10-05");
            return Success<string>($"{tblName_1},{tblName_2},{tblName_3}");
        }

        /// <summary>
        /// 测试报表动态列(根据参数变化)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="dateS"></param>
        /// <param name="dateE"></param>
        /// <returns></returns>
        [HttpGet]
        public MessageModel<List<object>> TestDynamicColumns(string? val, DateTime? dateS, DateTime? dateE)
        {
            if (!dateS.HasValue && !dateE.HasValue)
            {
                var data = new List<object>()
                {
                    new
                    {
                        addr = "华东",
                        name = "一期"
                    },
                    new
                    {
                        addr = "华东",
                        name = "二期"
                    }
                };
                return Success(data);
            }
            else
            {
                Random random = new Random();
                var dateSum = new List<string>();
                for (var date = dateS; date <= dateE; date = date.Value.AddDays(1))
                {
                    dateSum.Add(date.Value.ToString("yyyy-MM-dd"));
                }
                dateSum = dateSum.Take(5).ToList();
                var obj_1 = JObject.FromObject(new
                {
                    addr = "华东",
                    name = "一期"
                });
                var obj_2 = JObject.FromObject(new
                {
                    addr = "华东",
                    name = "二期"
                });
                foreach (var date in dateSum)
                {
                    obj_1[date] = random.Next(100, 1000);
                    obj_2[date] = random.Next(100, 1000);
                }
                var data = new List<object>()
                {
                    obj_1, obj_2
                };
                return Success(data);
            }
        }

        /// <summary>
        /// 测试API报表直接返回数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<object> TestApiReport()
        {
            List<object> data = new List<object>();
            for (int i = 0; i < 23; i++)
            {
                data.Add(new { ind = i, code = $"{i}_code" });
            }
            return data;
        }
    }
}