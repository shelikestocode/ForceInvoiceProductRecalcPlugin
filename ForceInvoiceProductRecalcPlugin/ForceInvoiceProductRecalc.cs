using System;
using Microsoft.Xrm.Sdk;

public class ForceInvoiceProductRecalc : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));

        if (context.Depth > 1)
            return;

        if (!context.InputParameters.Contains("Target"))
            return;

        var target = context.InputParameters["Target"] as Entity;
        if (target == null)
            return;

        string[] watchedFields =
        {
            "productid",
            "priceperunit",
            "quantity",
            "wattle_vattaxid",
            "baseamount",
            "tax",
            "manualdiscountamount",
            "extendedamount"
        };

        bool hasRelevantChange = false;

        foreach (var field in watchedFields)
        {
            if (target.Attributes.Contains(field))
            {
                hasRelevantChange = true;
                break;
            }
        }

        if (!hasRelevantChange)
            return;

        var serviceFactory =
            (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

        var service =
            serviceFactory.CreateOrganizationService(context.UserId);

        var update = new Entity(target.LogicalName)
        {
            Id = target.Id
        };

        service.Update(update);
    }
}