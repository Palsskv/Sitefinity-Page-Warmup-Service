# Sitefinity Page Warmup Service
Automatically warm up published Sitefinity pages.

## Setup
* Set the project references for `Telerik.Sitefinity`, `Telerik.Sitefinity.Model` and other required Telerik assemblies
* Reference the PageCompiler from your Web Project
* Build and run the solution

## Usage
Page precompilation/warmup is handled by the `PageCompilationService`.

The service can be either invoked automatically upon application startup by adding the following code inside `Global.asax`.

```c#
public class Global : System.Web.HttpApplication
    {
		protected void Application_Start(object sender, EventArgs e)
        {
            Bootstrapper.Initialized += new EventHandler<Telerik.Sitefinity.Data.ExecutedEventArgs>(Bootstrapper_Initialized);
        }
		
		void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                CompilationTaskRunner.RunTask();
            }
        }
}    
```

Alternatively, you can register the PageCompilationService as a WCF Web Service, by creating a `PageCompilation.svc` file in `Services\Compilation` with the following content.
```
<%@ ServiceHost Language="C#" Debug="true"
    Service="PageCompiler.Services.PageCompilationService"
    Factory="Telerik.Sitefinity.Web.Services.WcfHostFactory" %>
```
Then you can access /Services/Compilation/PageCompilation.svc in your browser or from a service client and invoke the service methods.