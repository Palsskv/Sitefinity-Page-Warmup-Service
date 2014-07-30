using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PageCompiler.Model;
using Telerik.Sitefinity.Scheduling;
using System.ServiceModel.Activation;
using System.ServiceModel;

namespace PageCompiler.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required),
    ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class PageCompilationService : IPageCompilationService
    {
        public TaskResponseModel Stop(string key)
        {
            var response = new TaskResponseModel();

            try
            {
                Guid taskKey = Guid.Parse(key);
                CompilePagesTask.StopTask(taskKey);
                
                response.Message = "Stopped successfully";
                response.TaskKey = key;

                return response;
            }
            catch (Exception ex)
            {
                response.Message = String.Format("Failed to stop - {0}<br/><br/>{1}", ex.Message, ex.StackTrace);
                return response;
            }
        }

        public TaskResponseModel Start(string startNodeUrl, bool isRootNode)
        {
            var response = new TaskResponseModel();

            try
            {
                Guid newKey = Guid.NewGuid();
                SchedulingManager schedulingManager = SchedulingManager.GetManager();

                CompilationModel model = new CompilationModel();
                model.StartNodeUrl = startNodeUrl;
                model.IsRootNode = isRootNode;

                CompilePagesTask newTask = new CompilePagesTask()
                {
                    Key = newKey.ToString(),
                    ExecuteTime = DateTime.UtcNow.AddMinutes(1)
                };

                newTask.Model = model;

                schedulingManager.AddTask(newTask);
                schedulingManager.SaveChanges();

                response.Message = "Started successfully";
                var newTaskData = schedulingManager.GetTaskData().FirstOrDefault(t => t.Key == newTask.Key);
                response.TaskKey = newTaskData.Id.ToString();

                return response;
            }
            catch (Exception ex)
            {
                response.Message = String.Format("Failed to start - {0}<br/><br/>{1}", ex.Message, ex.StackTrace);
                return response;
            }
        }

        public void StopTasks()
        {
            SchedulingManager schedulingManager = SchedulingManager.GetManager();
            var tasks = schedulingManager.GetTaskData().Where(t => t.TaskName.Contains("CompilePagesTask"));
            foreach (var task in tasks)
            {
                CompilePagesTask.StopTask(task.Id);
            }
            schedulingManager.SaveChanges();
        }

        public void ClearTasks()
        {
            var schedulingManager = SchedulingManager.GetManager();
            var tasks = schedulingManager.GetTaskData().Where(t => t.TaskName.Contains("CompilePagesTask"));
            foreach (var task in tasks)
            {
                schedulingManager.DeleteItem(task);
            }
            schedulingManager.SaveChanges();
        }
        public TaskResponseModel Status()
        {
            var result = new TaskResponseModel();
            var schedulingManager = SchedulingManager.GetManager();
            var task = schedulingManager.GetTaskData().FirstOrDefault(t => t.TaskName.Contains("CompilePagesTask"));
            if (task != null && task.TaskData != null)
            {
                var data = CompilePagesTask.Parse(task.TaskData);
                result.TaskProgress = data.CurrentIndex;
                result.PagesCount = data.PageCount;
            }
            
            return result;
        }
    }
}
