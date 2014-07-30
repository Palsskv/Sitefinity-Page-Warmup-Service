using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PageCompiler.Events;
using Telerik.Sitefinity.Scheduling;
using Telerik.Sitefinity.Services;
using PageCompiler.Model;
using System.Xml.Serialization;

namespace PageCompiler
{
    public class CompilePagesTask : ScheduledTask
    {
        private const string TaskNameConst = "PageCompiler.CompilePagesTask";

        private CompilationModel _model;

        private void CheckProgress(ProgressChangedEvent e)
        {
            SchedulingManager manager = SchedulingManager.GetManager();
            var taskData = manager.GetTaskData().FirstOrDefault(t => t.Id == e.TaskId);

            if (e.Code == ProgressCode.Stop)
            {
                taskData.Status = Telerik.Sitefinity.Scheduling.Model.TaskStatus.Stopped;
            }

            if (e.Code == ProgressCode.Status)
            {
                if (taskData != null && taskData.TaskData != null)
                {
                    var data = CompilePagesTask.Parse(taskData.TaskData);
                    data.CurrentIndex = e.Index;
                    data.PageCount = e.PageCount;
                    taskData.ScheduleData = CompilePagesTask.Serialize(data);
                }
            }

            manager.SaveChanges();
        }

        public CompilePagesTask()
        {
            EventHub.Unsubscribe<ProgressChangedEvent>(CheckProgress);
            EventHub.Subscribe<ProgressChangedEvent>(CheckProgress);
        }

        public CompilePagesTask(CompilationModel model)
            : this()
        {
            if (model == null)
                throw new ArgumentNullException("The compilation model is null.");

            this._model = model;
        }

        public CompilationModel Model
        {
            get { return _model; }
            set { this._model = value; }
        }

        public static bool TaskStopped { get; set; }

        public static void StopTask(Guid taskId)
        {
            try
            {
                CompilePagesTask.TaskStopped = true;
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Restarts the task
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="model"></param>
        public static void RestartTask(Guid taskId, CompilationModel model)
        {
            try
            {
                CompilePagesTask.StopTask(taskId);
                var manager = SchedulingManager.GetManager();

                Guid newKey = Guid.NewGuid();
                CompilePagesTask newTask = new CompilePagesTask()
                {
                    Key = newKey.ToString(),
                    ExecuteTime = DateTime.UtcNow.AddSeconds(10),
                    Id = Guid.NewGuid()
                };

                newTask.Model = model;

                manager.AddTask(newTask);

                manager.SaveChanges();

                EventHub.Raise(new ProgressChangedEvent
                {
                    Code = ProgressCode.Restart,
                    TaskId = newTask.Id,
                    Message = string.Format("({0}) - Attempting to start task back up with URL: {1} - New task id: {2} - Restarting in 10 seconds... ", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"), model.StartNodeUrl, newTask.Id)
                });
            }
            catch (Exception ex)
            {
                EventHub.Raise(new ProgressChangedEvent
                {
                    Code = ProgressCode.Error,
                    Message = string.Format("({0}) - Failed to start task. Error: {1}<br/><br/>Stack trace:<br/>{2}", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"), ex.Message, ex.StackTrace)
                });
            }
        }

        /// <summary>
        /// Executes a new task
        /// </summary>
        public override void ExecuteTask()
        {
            try
            {
                var action = new CompilePagesAction(this._model);
                EventHub.Raise(new ProgressChangedEvent
                {
                    Code = ProgressCode.General,
                    Message = string.Format("({0}) - Task created in the system: {1}", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"), this.Id),
                    TaskId = this.Id
                });

                CompilePagesTask.TaskStopped = false;
                action.StartCompilation(Id);
            }
            catch (Exception ex)
            {
                EventHub.Raise(new ProgressChangedEvent
                {
                    Code = ProgressCode.Error,
                    Message = string.Format("({0}) - Task {1}: threw an error {2}", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"), this.Id, ex.Message),
                    TaskId = this.Id
                });
            }
        }

        /// <summary>
        /// Parses the custom task data
        /// </summary>
        /// <param name="customData"></param>
        /// <returns></returns>
        internal static CompilationModel Parse(string customData)
        {
            if (string.IsNullOrEmpty(customData))
            {
                throw new ArgumentNullException("customData is null");
            }

            var serializer = new XmlSerializer(typeof(CompilationModel));

            using (TextReader reader = new StringReader(customData))
            {
                return serializer.Deserialize(reader) as CompilationModel;
            }
        }

        internal static string Serialize(CompilationModel model)
        {
            var serializer = new XmlSerializer(typeof(CompilationModel));
            var serializedContent = string.Empty;

            using (TextWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, model);
                serializedContent = writer.ToString();
            }

            return serializedContent;
        }

        public override string GetCustomData()
        {
            return CompilePagesTask.Serialize(this._model);
        }

        public override void SetCustomData(string customData)
        {
            this._model = CompilePagesTask.Parse(customData);
        }

        public override string TaskName
        {
            get { return CompilePagesTask.TaskNameConst; }
        }
    }
}
