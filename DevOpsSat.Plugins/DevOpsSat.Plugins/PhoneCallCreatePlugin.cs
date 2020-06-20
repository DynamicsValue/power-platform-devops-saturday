using Microsoft.Xrm.Sdk;
using System;
using TypedEntities;
using System.Linq;

namespace DevOpsSat.Plugins
{
    public class PhoneCallCreatePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region Boilerplate
            // Obtain the execution context from the service provider.
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            #endregion

            if(context.InputParameters.Contains("Target") 
                && context.InputParameters["Target"] is Entity)
            {
                var phoneCall = (context.InputParameters["Target"] as Entity).ToEntity<PhoneCall>();

                using(var ctx = new XrmServiceContext(service))
                {
                    var existingPhoneCallHistoryRecord = (from ph in ctx.CreateQuery<ultra_phonecallhistory>()
                                                          where ph.ultra_contactid.Id == phoneCall.RegardingObjectId.Id
                                                          where ph.ultra_phonenumber == phoneCall.PhoneNumber
                                                          select ph).FirstOrDefault();

                    if(existingPhoneCallHistoryRecord == null)
                    {
                        var phoneCallHistory = new ultra_phonecallhistory()
                        {
                            ultra_contactid = phoneCall.RegardingObjectId,
                            ultra_phonenumber = phoneCall.PhoneNumber
                        };

                        service.Create(phoneCallHistory);
                    }
                }

                
            }

                          
        }
    }
}
