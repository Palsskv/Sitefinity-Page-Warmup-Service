using PageCompiler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace PageCompiler.Services
{
    [ServiceContract]
    interface IPageCompilationService
    {
        [OperationContract, WebGet(UriTemplate = "Stop/?key={key}", ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(typeof(TaskResponseModel))]
        TaskResponseModel Stop(string key);

        [OperationContract, WebGet(UriTemplate = "Start/?startNodeUrl={startNodeUrl}&isRootNode={isRootNode}", ResponseFormat = WebMessageFormat.Json)]
        [ServiceKnownType(typeof(TaskResponseModel))]
        TaskResponseModel Start(string startNodeUrl, bool isRootNode);

        [OperationContract, WebGet(UriTemplate = "ClearTasks", ResponseFormat = WebMessageFormat.Json)]
        void ClearTasks();

        [OperationContract, WebGet(UriTemplate = "Status", ResponseFormat = WebMessageFormat.Json)]
        TaskResponseModel Status();
    }
}
