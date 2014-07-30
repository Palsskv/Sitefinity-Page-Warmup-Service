using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PageCompiler.Events;
using PageCompiler.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using PageCompiler.Utils;
using System.Net;
using Telerik.Sitefinity.Scheduling;

namespace PageCompiler
{
    public class CompilePagesAction
    {
        private CompilationModel _model;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model"></param>
        public CompilePagesAction(CompilationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("The model is null!");
            }

            this._model = model;
        }

        /// <summary>
        /// Starts the pages compilation
        /// </summary>
        /// <param name="taskId"></param>
        public void StartCompilation(Guid taskId)
        {
            int currentIdx = 0;
            SiteMapProvider provider = SiteMapBase.GetSiteMapProvider("FrontEndSiteMap");
            SiteMapNode root = null;

            if (this._model.IsRootNode)
            {
                root = provider.RootNode;
            }
            else
            {
                root = provider.FindSiteMapNode(this._model.StartNodeUrl);
            }

            if (root == null)
                throw new TaskStoppedException();

            this._model.PageCount = root.GetAllNodes().Count;

            var results = TraverseUtil.TraverseBfs<SiteMapNode>(root, node => node.ChildNodes as System.Collections.IEnumerable);

            try
            {
                foreach (var node in results)
                {
                    if (CompilePagesTask.TaskStopped)
                    {
                        throw new TaskStoppedException();
                    }

                    this._model.StartNodeUrl = node.Url;

                    if (!string.IsNullOrEmpty(node.Url))
                    {
                        currentIdx++;
                        EventHub.Raise(new ProgressChangedEvent
                        {
                            TaskId = taskId,
                            Code = ProgressCode.Status,
                            Index = currentIdx,
                            PageCount = this._model.PageCount,
                            Message = string.Format("Processed {0} out of {1}", currentIdx, this._model.PageCount)
                        });

                        var request = HttpWebRequest.Create(UrlPath.ResolveAbsoluteUrl(node.Url));
                        request.Method = "GET";
                        request.GetResponse();
                    }
                }
            }
            catch (TaskStoppedException)
            {
                EventHub.Raise(new ProgressChangedEvent
                {
                    TaskId = taskId,
                    Code = ProgressCode.Stop,
                    Message = string.Format("({0}) - Task stopped successfully", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"))
                });
            }
            catch (Exception ex)
            {
                EventHub.Raise(new ProgressChangedEvent
                {
                    Code = ProgressCode.Error,
                    Message = string.Format("({0}) - An error occurred: Error message: {1} \nStack trace:\n {2}", DateTime.Now.ToString("d/M/yyyy HH:mm:ss"), ex.Message, ex.StackTrace)
                });

                CompilePagesTask.RestartTask(taskId, this._model);
            }
        }
    }
}
